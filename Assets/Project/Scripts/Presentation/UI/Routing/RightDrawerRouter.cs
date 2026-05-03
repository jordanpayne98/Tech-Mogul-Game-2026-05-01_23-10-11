using System;
using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Routing
{
    /// <summary>
    /// Full implementation of <see cref="IRightDrawerRouter"/>.
    /// Manages a single right drawer at a time within the RightDrawerHost element.
    ///
    /// Responsibilities:
    ///   - Maintain a registry of drawer content factories keyed by stable drawer ID.
    ///   - Show/hide the RightDrawerHost as drawers open and close.
    ///   - Clear previous drawer content before injecting new content.
    ///   - Fall back to placeholder content for unregistered drawer IDs.
    ///   - Fire DrawerOpened/DrawerClosed events for observers.
    ///
    /// Phase 4 note: drawer animation (slide-in) is deferred.
    ///              Opens and closes use instant show/hide only.
    /// </summary>
    public sealed class RightDrawerRouter : IRightDrawerRouter
    {
        // ─── Constants ───────────────────────────────────────────────────────────────

        private const string HiddenClass  = "hidden";
        private const string IsOpenClass  = "is-open";

        // ─── State ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _drawerHost;
        private readonly Dictionary<string, Func<object, VisualElement>> _factories;

        // ─── IRightDrawerRouter ──────────────────────────────────────────────────────

        public bool IsDrawerOpen      { get; private set; }
        public string CurrentDrawerId { get; private set; }
        public event Action<string> DrawerOpened;
        public event Action         DrawerClosed;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <param name="drawerHost">The RightDrawerHost VisualElement from the shell UXML.</param>
        public RightDrawerRouter(VisualElement drawerHost)
        {
            _drawerHost = drawerHost ?? throw new ArgumentNullException(nameof(drawerHost));
            _factories  = new Dictionary<string, Func<object, VisualElement>>(StringComparer.Ordinal);

            // Ensure host starts hidden.
            if (!_drawerHost.ClassListContains(HiddenClass))
            {
                _drawerHost.AddToClassList(HiddenClass);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Registers a factory that creates drawer content for the given stable drawer ID.
        /// The factory receives the optional context object passed to <see cref="OpenDrawer"/>.
        /// Subsequent calls with the same ID overwrite the previous factory.
        /// </summary>
        public void RegisterDrawer(string drawerId, Func<object, VisualElement> factory)
        {
            if (string.IsNullOrEmpty(drawerId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "RightDrawerRouter.RegisterDrawer called with null or empty drawerId. Registration skipped.");
                return;
            }

            if (factory == null)
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"RightDrawerRouter.RegisterDrawer called with null factory for '{drawerId}'. Registration skipped.");
                return;
            }

            _factories[drawerId] = factory;

            DebugLogger.Log(DebugCategory.Navigation,
                $"RightDrawerRouter: registered factory for drawer '{drawerId}'.");
        }

        /// <summary>
        /// Opens the drawer with the given stable ID.
        /// If a drawer is already open it is closed first, then the new one opens.
        /// Falls back to placeholder content for unregistered IDs.
        /// </summary>
        public void OpenDrawer(string drawerId, object context = null)
        {
            if (string.IsNullOrEmpty(drawerId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "RightDrawerRouter.OpenDrawer called with null or empty drawerId. Aborted.");
                return;
            }

            // Close any currently open drawer first (no event fired for the intermediate close).
            if (IsDrawerOpen)
            {
                CloseDrawerInternal(fireEvent: false);
            }

            // ── Show drawer host ─────────────────────────────────────────────────────
            _drawerHost.RemoveFromClassList(HiddenClass);
            _drawerHost.AddToClassList(IsOpenClass);

            // ── Create drawer content ────────────────────────────────────────────────
            VisualElement drawerContent;

            if (_factories.TryGetValue(drawerId, out Func<object, VisualElement> factory))
            {
                drawerContent = factory.Invoke(context);
                DebugLogger.Log(DebugCategory.Navigation,
                    $"RightDrawerRouter: opened registered drawer '{drawerId}'.");
            }
            else
            {
                drawerContent = CreatePlaceholderDrawer(drawerId);
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"RightDrawerRouter: no factory registered for '{drawerId}'. Showing placeholder drawer.");
            }

            _drawerHost.Add(drawerContent);

            // ── Update state ─────────────────────────────────────────────────────────
            IsDrawerOpen    = true;
            CurrentDrawerId = drawerId;

            DrawerOpened?.Invoke(drawerId);
        }

        /// <summary>
        /// Closes the currently open drawer and hides the drawer host.
        /// Logs a warning when called while no drawer is open.
        /// </summary>
        public void CloseDrawer()
        {
            if (!IsDrawerOpen)
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "RightDrawerRouter.CloseDrawer called but no drawer is currently open.");
                return;
            }

            CloseDrawerInternal(fireEvent: true);
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        /// <summary>Internal close path. Fires event only when <paramref name="fireEvent"/> is true.</summary>
        private void CloseDrawerInternal(bool fireEvent)
        {
            string closedId = CurrentDrawerId;

            _drawerHost.Clear();
            _drawerHost.RemoveFromClassList(IsOpenClass);
            _drawerHost.AddToClassList(HiddenClass);

            IsDrawerOpen    = false;
            CurrentDrawerId = null;

            DebugLogger.Log(DebugCategory.Navigation,
                $"RightDrawerRouter: closed drawer '{closedId}'.");

            if (fireEvent)
            {
                DrawerClosed?.Invoke();
            }
        }

        /// <summary>Creates placeholder drawer content for an unregistered drawer ID.</summary>
        private static VisualElement CreatePlaceholderDrawer(string drawerId)
        {
            var frame = new VisualElement();
            frame.AddToClassList("right-drawer");

            var title = new Label($"[Placeholder] Drawer: {drawerId}");
            title.AddToClassList("right-drawer__title");
            frame.Add(title);

            var message = new Label("[Placeholder] This drawer will be implemented later.");
            frame.Add(message);

            return frame;
        }
    }
}
