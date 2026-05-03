using System;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.MainMenu
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Main Menu.
    /// Queries named VisualElements from the UXML root on construction.
    /// Exposes button-click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// </summary>
    public sealed class MainMenuView
    {
        // ─── Button click events ─────────────────────────────────────────────────────

        public event Action OnContinueClicked;
        public event Action OnNewGameClicked;
        public event Action OnLoadGameClicked;
        public event Action OnSettingsClicked;
        public event Action OnCreditsClicked;
        public event Action OnExitClicked;

        // ─── Queried elements ────────────────────────────────────────────────────────

        private readonly VisualElement _buttonContinue;
        private readonly VisualElement _buttonNewGame;
        private readonly VisualElement _buttonLoadGame;
        private readonly VisualElement _buttonSettings;
        private readonly VisualElement _buttonCredits;
        private readonly VisualElement _buttonExit;

        private readonly Label _footerCopyright;
        private readonly Label _footerVersion;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Registers click callbacks for each button.
        /// Logs a warning via DebugLogger for any missing element.
        /// </summary>
        public MainMenuView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "MainMenuView: root VisualElement is null. View cannot be initialized.");
                return;
            }

            _buttonContinue = QueryElement(root, "ButtonContinue");
            _buttonNewGame  = QueryElement(root, "ButtonNewGame");
            _buttonLoadGame = QueryElement(root, "ButtonLoadGame");
            _buttonSettings = QueryElement(root, "ButtonSettings");
            _buttonCredits  = QueryElement(root, "ButtonCredits");
            _buttonExit     = QueryElement(root, "ButtonExit");

            _footerCopyright = root.Q<Label>("FooterCopyright");
            _footerVersion   = root.Q<Label>("FooterVersion");

            if (_footerCopyright == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "MainMenuView: 'FooterCopyright' label not found in UXML.");
            }

            if (_footerVersion == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "MainMenuView: 'FooterVersion' label not found in UXML.");
            }

            RegisterCallbacks();
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Sets footer text and toggles the Continue button disabled state.
        /// </summary>
        public void Bind(MainMenuViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "MainMenuView.Bind: viewModel is null. Skipping bind.");
                return;
            }

            if (_footerCopyright != null)
            {
                _footerCopyright.text = viewModel.CopyrightText;
            }

            if (_footerVersion != null)
            {
                _footerVersion.text = viewModel.BuildVersion;
            }

            if (_buttonContinue != null)
            {
                if (viewModel.HasSaveData)
                {
                    _buttonContinue.RemoveFromClassList("is-disabled");
                }
                else
                {
                    _buttonContinue.AddToClassList("is-disabled");
                }
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private void RegisterCallbacks()
        {
            RegisterClick(_buttonContinue, "ButtonContinue", OnContinueWithDisabledGuard);
            RegisterClick(_buttonNewGame,  "ButtonNewGame",  () => OnNewGameClicked?.Invoke());
            RegisterClick(_buttonLoadGame, "ButtonLoadGame", () => OnLoadGameClicked?.Invoke());
            RegisterClick(_buttonSettings, "ButtonSettings", () => OnSettingsClicked?.Invoke());
            RegisterClick(_buttonCredits,  "ButtonCredits",  () => OnCreditsClicked?.Invoke());
            RegisterClick(_buttonExit,     "ButtonExit",     () => OnExitClicked?.Invoke());
        }

        private void OnContinueWithDisabledGuard()
        {
            if (_buttonContinue != null && _buttonContinue.ClassListContains("is-disabled"))
            {
                return;
            }

            OnContinueClicked?.Invoke();
        }

        private static void RegisterClick(VisualElement element, string name, Action callback)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"MainMenuView: skipping click registration — '{name}' element is null.");
                return;
            }

            element.RegisterCallback<ClickEvent>(_ => callback?.Invoke());
        }

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);

            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"MainMenuView: element '{name}' not found in UXML root.");
            }

            return element;
        }
    }
}
