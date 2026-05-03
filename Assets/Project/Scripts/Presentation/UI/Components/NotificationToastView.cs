using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Components
{
    /// <summary>
    /// Factory for the NotificationToast VisualElement.
    /// Creates a toast notification element with semantic variant support.
    /// The returned element is injected into the NotificationHost by the caller.
    ///
    /// Phase 4: Manual close only. Auto-dismiss is deferred to Phase 5.
    ///
    /// Supported variants:
    ///   "info"    — Left border and icon use info/blue colour.
    ///   "success" — Left border and icon use success/green colour.
    ///   "warning" — Left border and icon use warning/amber colour.
    ///   "danger"  — Left border and icon use danger/red colour.
    /// </summary>
    public static class NotificationToastView
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string DefaultVariant = "info";

        // ─── Public Factory ───────────────────────────────────────────────────────

        /// <summary>
        /// Creates a notification toast VisualElement.
        /// </summary>
        /// <param name="title">Toast title text.</param>
        /// <param name="message">Toast body message.</param>
        /// <param name="variant">Semantic variant: "info", "success", "warning", "danger".</param>
        public static VisualElement Create(string title, string message, string variant = DefaultVariant)
        {
            // ── Root ───────────────────────────────────────────────────────────────
            var root = new VisualElement();
            root.AddToClassList("notification-toast");
            ApplyVariantClass(root, variant);

            // ── Icon ───────────────────────────────────────────────────────────────
            var icon = new VisualElement();
            icon.AddToClassList("notification-toast__icon");
            root.Add(icon);

            // ── Content Block ──────────────────────────────────────────────────────
            var content = new VisualElement();
            content.AddToClassList("notification-toast__content");

            var titleLabel = new Label(title ?? string.Empty);
            titleLabel.AddToClassList("notification-toast__title");
            content.Add(titleLabel);

            var messageLabel = new Label(message ?? string.Empty);
            messageLabel.AddToClassList("notification-toast__message");
            content.Add(messageLabel);

            root.Add(content);

            // ── Close Button ───────────────────────────────────────────────────────
            var closeButton = new VisualElement();
            closeButton.AddToClassList("notification-toast__close");
            closeButton.RegisterCallback<ClickEvent>(_ => RemoveToast(root));
            root.Add(closeButton);

            return root;
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        /// <summary>Applies the semantic variant modifier class to the root element.</summary>
        private static void ApplyVariantClass(VisualElement root, string variant)
        {
            switch (variant)
            {
                case "success":
                    root.AddToClassList("notification-toast--success");
                    break;
                case "warning":
                    root.AddToClassList("notification-toast--warning");
                    break;
                case "danger":
                    root.AddToClassList("notification-toast--danger");
                    break;
                default:
                    // "info" and any unrecognised variant fall back to info.
                    root.AddToClassList("notification-toast--info");
                    break;
            }
        }

        /// <summary>Removes the toast element from its parent notification host.</summary>
        private static void RemoveToast(VisualElement toast)
        {
            toast.RemoveFromHierarchy();
        }
    }
}
