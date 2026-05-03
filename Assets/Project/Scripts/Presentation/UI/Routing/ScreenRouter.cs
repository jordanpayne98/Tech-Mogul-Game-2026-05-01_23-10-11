using System;
using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using Project.Presentation.UI.Screens;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Routing
{
    /// <summary>
    /// Concrete implementation of <see cref="IScreenRouter"/>.
    /// Manages flat screen navigation within the ScreenHost element.
    /// Calling OpenScreen replaces the current screen entirely — no history, no back-stack.
    /// Unregistered screen IDs fall back to a PlaceholderScreen.
    /// </summary>
    public sealed class ScreenRouter : IScreenRouter
    {
        private readonly VisualElement _screenHost;
        private readonly Dictionary<string, Func<VisualElement>> _factories;

        // ─── IScreenRouter ──────────────────────────────────────────────────────────

        public string CurrentScreenId { get; private set; }
        public event Action<string> ScreenChanged;

        /// <param name="screenHost">The ScreenHost VisualElement from the shell UXML.</param>
        public ScreenRouter(VisualElement screenHost)
        {
            _screenHost = screenHost ?? throw new ArgumentNullException(nameof(screenHost));
            _factories  = new Dictionary<string, Func<VisualElement>>(StringComparer.Ordinal);
        }

        // ─── Public API ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Navigate to the screen with the given stable ID.
        /// Clears the ScreenHost and replaces its content.
        /// Uses a registered factory when available; falls back to PlaceholderScreen.
        /// </summary>
        public void OpenScreen(string screenId)
        {
            if (string.IsNullOrEmpty(screenId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "ScreenRouter.OpenScreen called with null or empty screenId. Navigation aborted.");
                return;
            }

            // Clear current content.
            _screenHost.Clear();

            VisualElement screen;

            if (_factories.TryGetValue(screenId, out Func<VisualElement> factory))
            {
                screen = factory.Invoke();
                DebugLogger.Log(DebugCategory.Navigation,
                    $"ScreenRouter: navigated to registered screen '{screenId}'.");
            }
            else
            {
                screen = CreatePlaceholderScreen(screenId);
                DebugLogger.Log(DebugCategory.Navigation,
                    $"ScreenRouter: no factory registered for '{screenId}'. Showing placeholder.");
            }

            _screenHost.Add(screen);

            CurrentScreenId = screenId;
            ScreenChanged?.Invoke(screenId);
        }

        /// <summary>
        /// Registers a factory function that produces the VisualElement for a given screen ID.
        /// Subsequent calls with the same ID overwrite the previous factory.
        /// </summary>
        public void RegisterScreen(string screenId, Func<VisualElement> factory)
        {
            if (string.IsNullOrEmpty(screenId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "ScreenRouter.RegisterScreen called with null or empty screenId. Registration skipped.");
                return;
            }

            if (factory == null)
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"ScreenRouter.RegisterScreen called with null factory for '{screenId}'. Registration skipped.");
                return;
            }

            _factories[screenId] = factory;

            DebugLogger.Log(DebugCategory.Navigation,
                $"ScreenRouter: registered factory for screen '{screenId}'.");
        }

        // ─── Private helpers ────────────────────────────────────────────────────────

        /// <summary>Creates a PlaceholderScreen element for an unregistered screen ID.</summary>
        private static VisualElement CreatePlaceholderScreen(string screenId)
        {
            return PlaceholderScreenView.Create(screenId);
        }
    }
}
