using System;
using Project.Application.UseCases.SaveLoad;
using Project.Core.Debugging;
using Project.Core.Interfaces;
using Project.Core.Runtime;
using Project.Infrastructure.Debugging;
using Project.Infrastructure.Definitions;
using Project.Infrastructure.SaveLoad;
using Project.Infrastructure.Tuning;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Composition
{
    /// <summary>
    /// Entry point MonoBehaviour for the game. Loaded in the Main scene.
    /// Initializes the debug logging and tuning config systems on Awake,
    /// then delegates all dependency wiring to GameCompositionRoot.
    /// Phase 3 services (definitions, save/load, autosave) are wired after the composition root.
    ///
    /// Stays DontDestroyOnLoad so that config and wiring survive scene transitions.
    ///
    /// This is the one place in the project where Resources.Load is acceptable
    /// for config assets, per Tuning Reference Section 6 and Coding Standard Section 8.
    /// All other systems receive configs via explicit dependency injection.
    /// </summary>
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private DebugConfig          _debugConfig;
        private TuningConfig         _tuningConfig;
        private GameCompositionRoot  _compositionRoot;

        // ─── Phase 3 services ─────────────────────────────────────────────────────

        private DefinitionRegistry   _definitionRegistry;
        private SaveService          _saveService;
        private SaveSlotManager      _saveSlotManager;
        private AutosaveCoordinator  _autosaveCoordinator;
        private ExitSaveHandler      _exitSaveHandler;

        // ─── Phase 4 UI ───────────────────────────────────────────────────────────

        private UIShellController    _uiShellController;

        // ─── Session reference for autosave provider ──────────────────────────────

        /// <summary>
        /// Current active game session. Null until a game is started or loaded.
        /// Provided to AutosaveCoordinator via a lambda so it always captures the live reference.
        /// </summary>
        private GameSessionState _currentSession;

        /// <summary>Exposed for debug tools (e.g. DebugSmokeTestRunner).</summary>
        public GameCompositionRoot CompositionRoot => _compositionRoot;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            InitializeDebugLogger();
            InitializeTuningConfig();
            InitializeCompositionRoot();
            InitializePhase3();
            InitializePhase4UI();

            DebugLogger.Log(DebugCategory.Bootstrap, "GameBootstrapper initialized.", this);
        }

        private void OnDestroy()
        {
            _autosaveCoordinator?.Dispose();
            _compositionRoot?.Dispose();
        }

        // ─── Private initialization ───────────────────────────────────────────────

        private void InitializeDebugLogger()
        {
            _debugConfig = Resources.Load<DebugConfig>("DebugConfig");

            if (_debugConfig == null)
            {
                Debug.LogWarning("[Bootstrap] DebugConfig not found at Resources/DebugConfig.asset. " +
                                 "Debug logging will fall back to raw Debug.Log until config is restored.");
                return;
            }

            DebugLogger.Initialize(_debugConfig);
            DebugLogger.Log(DebugCategory.Bootstrap, "DebugConfig loaded and DebugLogger initialized.", this);
        }

        private void InitializeTuningConfig()
        {
            _tuningConfig = Resources.Load<TuningConfig>("TuningConfig");

            if (_tuningConfig == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "TuningConfig not found at Resources/TuningConfig.asset. " +
                    "Systems that depend on tuning values will receive null config.", this);
                return;
            }

            DebugLogger.Log(DebugCategory.Bootstrap, "TuningConfig loaded.", this);
        }

        private void InitializeCompositionRoot()
        {
            if (_tuningConfig == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "[Bootstrap] Cannot create GameCompositionRoot — TuningConfig is null.");
                return;
            }

            _compositionRoot = new GameCompositionRoot(_tuningConfig);
            _compositionRoot.Initialize();

            DebugLogger.Log(DebugCategory.Bootstrap, "GameCompositionRoot initialized.", this);
        }

        private void InitializePhase3()
        {
            if (_tuningConfig == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "[Bootstrap] Cannot initialize Phase 3 — TuningConfig is null.");
                return;
            }

            if (_compositionRoot == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "[Bootstrap] Cannot initialize Phase 3 — GameCompositionRoot is null.");
                return;
            }

            try
            {
                // ── Step 1: Definition registry ───────────────────────────────────

                _definitionRegistry = new DefinitionRegistry();
                var definitionLoader = new DefinitionLoader();
                definitionLoader.LoadAll(_definitionRegistry);

                DebugLogger.Log(DebugCategory.Bootstrap, "Definition registry loaded and locked.", this);

                // ── Step 2: Debug-only definition validation ──────────────────────

#if UNITY_EDITOR || DEBUG
                RunDefinitionRegistryValidator();
#endif

                // ── Step 3: Save service and slot manager ─────────────────────────

                _saveService     = new SaveService();
                _saveSlotManager = new SaveSlotManager(_saveService, _tuningConfig);

                DebugLogger.Log(DebugCategory.Bootstrap, "SaveService and SaveSlotManager initialized.", this);

                // ── Step 4: Autosave coordinator ──────────────────────────────────

                IEventBus eventBus = _compositionRoot.EventBus;

                _autosaveCoordinator = new AutosaveCoordinator(
                    slotManager:     _saveSlotManager,
                    eventBus:        eventBus,
                    sessionProvider: () => _currentSession);

                _autosaveCoordinator.Initialize();

                DebugLogger.Log(DebugCategory.Bootstrap, "AutosaveCoordinator initialized.", this);

                // ── Step 5: Exit save handler ─────────────────────────────────────

                _exitSaveHandler = gameObject.AddComponent<ExitSaveHandler>();
                _exitSaveHandler.SetCoordinator(_autosaveCoordinator);

                DebugLogger.Log(DebugCategory.Bootstrap, "ExitSaveHandler wired.", this);

                // ── Step 6: Debug-only save validators ────────────────────────────

#if UNITY_EDITOR || DEBUG
                RunSaveLoadRoundTripValidator();
                RunSaveSlotValidator();
#endif

                DebugLogger.Log(DebugCategory.Bootstrap, "Phase 3 infrastructure initialized.", this);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    $"[Bootstrap] Phase 3 initialization failed: {ex.Message}");
            }
        }

        private void InitializePhase4UI()
        {
            try
            {
                // ── Step 1: Find UIShell GameObject ───────────────────────────────────

                GameObject uiShellObject = GameObject.Find("UIShell");

                if (uiShellObject == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Bootstrap,
                        "[Bootstrap] UIShell GameObject not found in the scene. " +
                        "Phase 4 UI will not initialize. Add a UIShell GameObject with a UIDocument component.", this);
                    return;
                }

                // ── Step 2: Get UIDocument component ──────────────────────────────────

                UIDocument uiDocument = uiShellObject.GetComponent<UIDocument>();

                if (uiDocument == null)
                {
                    DebugLogger.LogError(DebugCategory.Bootstrap,
                        "[Bootstrap] UIShell GameObject found but has no UIDocument component. " +
                        "Phase 4 UI will not initialize.", this);
                    return;
                }

                // ── Step 3: Get or add UIShellController ──────────────────────────────

                _uiShellController = uiShellObject.GetComponent<UIShellController>();

                if (_uiShellController == null)
                {
                    _uiShellController = uiShellObject.AddComponent<UIShellController>();
                }

                // ── Step 4: Initialize the shell ──────────────────────────────────────

                _uiShellController.Initialize(uiDocument);

                DebugLogger.Log(DebugCategory.Bootstrap, "Phase 4 UI initialized.", this);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    $"[Bootstrap] Phase 4 UI initialization failed: {ex.Message}");
            }
        }

        // ─── Debug-only validators ────────────────────────────────────────────────
#if UNITY_EDITOR || DEBUG
        private void RunDefinitionRegistryValidator()
        {
            try
            {
                var validator = new Infrastructure.Definitions.Validation.DefinitionRegistryValidator(
                    _definitionRegistry,
                    _definitionRegistry);

                validator.Run();
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Bootstrap] DefinitionRegistryValidator threw an unexpected exception: {ex.Message}");
            }
        }

        private void RunSaveLoadRoundTripValidator()
        {
            try
            {
                var validator = new Infrastructure.SaveLoad.Validation.SaveLoadRoundTripValidator(_saveSlotManager);
                validator.Run();
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Bootstrap] SaveLoadRoundTripValidator threw an unexpected exception: {ex.Message}");
            }
        }

        private void RunSaveSlotValidator()
        {
            try
            {
                var validator = new Infrastructure.SaveLoad.Validation.SaveSlotValidator(
                    _saveSlotManager,
                    _autosaveCoordinator,
                    _exitSaveHandler != null,
                    _tuningConfig);

                validator.Run();
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Bootstrap] SaveSlotValidator threw an unexpected exception: {ex.Message}");
            }
        }
#endif
    }
}
