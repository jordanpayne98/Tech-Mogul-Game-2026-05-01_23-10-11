using System;
using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Routing
{
    /// <summary>
    /// Full implementation of <see cref="IModalRouter"/>.
    /// Manages a single modal at a time within the ModalHost element.
    ///
    /// Responsibilities:
    ///   - Maintain a registry of modal factories keyed by stable modal ID.
    ///   - Show/hide the ModalHost as modals open and close.
    ///   - Create a backdrop element that closes the modal on click
    ///     unless the active modal marks itself as RequireExplicitDismiss.
    ///   - Fall back to a placeholder modal for unregistered IDs.
    ///   - Fire ModalOpened/ModalClosed events for observers.
    /// </summary>
    public sealed class ModalRouter : IModalRouter
    {
        // ─── Constants ───────────────────────────────────────────────────────────────

        private const string HiddenClass   = "hidden";
        private const string BackdropClass = "modal-backdrop";

        // ─── State ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _modalHost;
        private readonly Dictionary<string, Func<object, VisualElement>> _factories;

        // Tracks whether the currently displayed modal requires explicit dismissal.
        private bool _requireExplicitDismiss;

        // ─── IModalRouter ────────────────────────────────────────────────────────────

        public bool IsModalOpen      { get; private set; }
        public string CurrentModalId { get; private set; }
        public event Action<string> ModalOpened;
        public event Action         ModalClosed;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <param name="modalHost">The ModalHost VisualElement from the shell UXML.</param>
        public ModalRouter(VisualElement modalHost)
        {
            _modalHost = modalHost ?? throw new ArgumentNullException(nameof(modalHost));
            _factories = new Dictionary<string, Func<object, VisualElement>>(StringComparer.Ordinal);

            // Ensure the modal host has the layout class for flex-centering.
            if (!_modalHost.ClassListContains("modal-host"))
            {
                _modalHost.AddToClassList("modal-host");
            }

            // Ensure host starts hidden.
            if (!_modalHost.ClassListContains(HiddenClass))
            {
                _modalHost.AddToClassList(HiddenClass);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Registers a factory that creates a modal VisualElement for the given stable ID.
        /// The factory receives the optional context object passed to <see cref="OpenModal"/>.
        /// Subsequent calls with the same ID overwrite the previous factory.
        /// </summary>
        public void RegisterModal(string modalId, Func<object, VisualElement> factory)
        {
            if (string.IsNullOrEmpty(modalId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "ModalRouter.RegisterModal called with null or empty modalId. Registration skipped.");
                return;
            }

            if (factory == null)
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"ModalRouter.RegisterModal called with null factory for '{modalId}'. Registration skipped.");
                return;
            }

            _factories[modalId] = factory;

            DebugLogger.Log(DebugCategory.Navigation,
                $"ModalRouter: registered factory for modal '{modalId}'.");
        }

        /// <summary>
        /// Opens the modal with the given stable ID.
        /// If a modal is already open it is closed first, then the new one opens.
        /// Falls back to a placeholder modal for unregistered IDs.
        /// </summary>
        public void OpenModal(string modalId, object context = null)
        {
            if (string.IsNullOrEmpty(modalId))
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "ModalRouter.OpenModal called with null or empty modalId. Aborted.");
                return;
            }

            // Close any currently open modal first (no event fired for the intermediate close).
            if (IsModalOpen)
            {
                CloseModalInternal(fireEvent: false);
            }

            // ── Show modal host ──────────────────────────────────────────────────────
            _modalHost.RemoveFromClassList(HiddenClass);

            // ── Create backdrop ──────────────────────────────────────────────────────
            var backdrop = new VisualElement();
            backdrop.AddToClassList(BackdropClass);
            backdrop.RegisterCallback<ClickEvent>(OnBackdropClicked);
            _modalHost.Add(backdrop);

            // ── Create modal content ─────────────────────────────────────────────────
            VisualElement modalElement;

            if (_factories.TryGetValue(modalId, out Func<object, VisualElement> factory))
            {
                modalElement = factory.Invoke(context);
                DebugLogger.Log(DebugCategory.Navigation,
                    $"ModalRouter: opened registered modal '{modalId}'.");
            }
            else
            {
                modalElement = CreatePlaceholderModal(modalId);
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"ModalRouter: no factory registered for '{modalId}'. Showing placeholder modal.");
            }

            _modalHost.Add(modalElement);

            // ── Capture RequireExplicitDismiss from root element if set ───────────────
            // Conventions: the factory may tag the root element with the USS class
            // "modal-require-explicit-dismiss" to prevent backdrop click-to-close.
            _requireExplicitDismiss = modalElement.ClassListContains("modal-require-explicit-dismiss");

            // ── Update state ─────────────────────────────────────────────────────────
            IsModalOpen    = true;
            CurrentModalId = modalId;

            ModalOpened?.Invoke(modalId);
        }

        /// <summary>
        /// Closes the currently open modal and hides the modal host.
        /// Logs a warning when called while no modal is open.
        /// </summary>
        public void CloseModal()
        {
            if (!IsModalOpen)
            {
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    "ModalRouter.CloseModal called but no modal is currently open.");
                return;
            }

            CloseModalInternal(fireEvent: true);
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        /// <summary>Internal close path. Fires event only when <paramref name="fireEvent"/> is true.</summary>
        private void CloseModalInternal(bool fireEvent)
        {
            string closedId = CurrentModalId;

            _modalHost.Clear();
            _modalHost.AddToClassList(HiddenClass);

            IsModalOpen           = false;
            CurrentModalId        = null;
            _requireExplicitDismiss = false;

            DebugLogger.Log(DebugCategory.Navigation,
                $"ModalRouter: closed modal '{closedId}'.");

            if (fireEvent)
            {
                ModalClosed?.Invoke();
            }
        }

        /// <summary>Backdrop click handler — respects RequireExplicitDismiss.</summary>
        private void OnBackdropClicked(ClickEvent evt)
        {
            if (_requireExplicitDismiss)
            {
                DebugLogger.Log(DebugCategory.Navigation,
                    $"ModalRouter: backdrop clicked but modal '{CurrentModalId}' requires explicit dismiss. Ignored.");
                return;
            }

            CloseModal();
        }

        /// <summary>Creates a placeholder modal element for an unregistered modal ID.</summary>
        private static VisualElement CreatePlaceholderModal(string modalId)
        {
            var frame = new VisualElement();
            frame.AddToClassList("modal-frame");
            frame.AddToClassList("modal-frame--standard");

            var title = new Label($"[Placeholder] Modal: {modalId}");
            title.AddToClassList("modal-frame__title");
            frame.Add(title);

            var message = new Label("[Placeholder] This modal will be implemented later.");
            message.AddToClassList("modal-frame__body");
            frame.Add(message);

            return frame;
        }
    }
}
