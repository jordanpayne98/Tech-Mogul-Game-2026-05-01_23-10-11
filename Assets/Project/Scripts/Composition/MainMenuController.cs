using Project.Core.Debugging;
using Project.Infrastructure.Debugging;
using Project.Presentation.UI.Screens.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Project.Composition
{
    /// <summary>
    /// Composition-layer MonoBehaviour for the MainMenu scene.
    /// Creates the ViewModel and View, handles button click events,
    /// and manages scene transitions.
    ///
    /// Initializes DebugLogger independently because GameBootstrapper
    /// does not exist in the MainMenu scene. Guards against double-init
    /// in case the player returns to this scene from MainGame while
    /// GameBootstrapper is still alive via DontDestroyOnLoad.
    ///
    /// Has zero simulation dependencies. Does not reference any
    /// Core.Runtime, Infrastructure.Definitions, or tick processors.
    ///
    /// Stable screen ID: screen.main_menu
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private MainMenuView      _view;
        private MainMenuViewModel _viewModel;

        private const string MainGameSceneName  = "MainGame";
        private const string CopyrightText      = "(c) 2026 Tech Mogul. All rights reserved.";

        private void Start()
        {
            EnsureDebugLoggerInitialized();

            if (_uiDocument == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "MainMenuController: UIDocument reference is null. " +
                    "Assign the UIDocument component in the Inspector.", this);
                return;
            }

            VisualElement root = _uiDocument.rootVisualElement;

            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "MainMenuController: rootVisualElement is null. " +
                    "Ensure the UIDocument has a valid UXML asset assigned.", this);
                return;
            }

            // ── Fix UIDocument TemplateContainer not stretching to fill viewport ──

            root.style.flexGrow = 1;
            if (root.childCount > 0 && root[0] != null)
            {
                root[0].style.flexGrow = 1;
            }

            // ── Create ViewModel ─────────────────────────────────────────────────────

            _viewModel = new MainMenuViewModel(
                hasSaveData:   false,
                buildVersion:  $"Build v{UnityEngine.Application.version}",
                copyrightText: CopyrightText);

            // ── Create View and bind ─────────────────────────────────────────────────

            _view = new MainMenuView(root);
            _view.Bind(_viewModel);

            // ── Subscribe to button events ───────────────────────────────────────────

            _view.OnContinueClicked  += HandleContinue;
            _view.OnNewGameClicked   += HandleNewGame;
            _view.OnLoadGameClicked  += HandleLoadGame;
            _view.OnSettingsClicked  += HandleSettings;
            _view.OnCreditsClicked   += HandleCredits;
            _view.OnExitClicked      += HandleExit;

            DebugLogger.Log(DebugCategory.Bootstrap,
                "MainMenuController initialized.", this);
        }

        private void OnDestroy()
        {
            if (_view == null)
            {
                return;
            }

            _view.OnContinueClicked  -= HandleContinue;
            _view.OnNewGameClicked   -= HandleNewGame;
            _view.OnLoadGameClicked  -= HandleLoadGame;
            _view.OnSettingsClicked  -= HandleSettings;
            _view.OnCreditsClicked   -= HandleCredits;
            _view.OnExitClicked      -= HandleExit;
        }

        // ─── Button handlers ─────────────────────────────────────────────────────────

        private void HandleContinue()
        {
            // Phase 4: Continue is disabled when HasSaveData is false.
            // View guards this, but log for debugging.
            DebugLogger.Log(DebugCategory.Navigation,
                "[MainMenu] Continue clicked. [Placeholder] No save data in Phase 4.", this);
        }

        private void HandleNewGame()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                $"[MainMenu] New Game clicked. Loading scene '{MainGameSceneName}'.", this);

            SceneManager.LoadScene(MainGameSceneName);
        }

        private void HandleLoadGame()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "[MainMenu] Load Game clicked. [Placeholder] Load game modal not yet implemented.", this);
        }

        private void HandleSettings()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "[MainMenu] Settings clicked. [Placeholder] Settings screen not yet implemented.", this);
        }

        private void HandleCredits()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "[MainMenu] Credits clicked. [Placeholder] Credits screen not yet implemented.", this);
        }

        private void HandleExit()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "[MainMenu] Exit clicked. Quitting application.", this);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Loads DebugConfig from Resources and initializes DebugLogger.
        /// Safe to call even if DebugLogger is already initialized
        /// (GameBootstrapper's DontDestroyOnLoad may have initialized it first).
        /// DebugLogger.Initialize() overwrites the existing config, which is acceptable
        /// when loading the same DebugConfig asset.
        /// </summary>
        private void EnsureDebugLoggerInitialized()
        {
            DebugConfig debugConfig = Resources.Load<DebugConfig>("DebugConfig");

            if (debugConfig == null)
            {
                Debug.LogWarning("[MainMenuController] DebugConfig not found at Resources/DebugConfig.asset. " +
                                 "Debug logging will fall back to raw Debug.Log.");
                return;
            }

            DebugLogger.Initialize(debugConfig);
        }
    }
}
