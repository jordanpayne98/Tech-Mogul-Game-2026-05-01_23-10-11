using System;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Modals
{
    /// <summary>
    /// Factory for the ConfirmModal VisualElement.
    /// Builds the confirmation dialog tree programmatically and wires callbacks.
    ///
    /// Supported variants:
    ///   "confirm"  — Confirm button uses primary/teal colour (default).
    ///   "danger"   — Confirm button uses danger/red colour.
    ///   "warning"  — Confirm button uses warning/amber colour, icon uses warning colour.
    ///   "info"     — Icon uses info/blue colour.
    ///   "error"    — Icon uses danger colour.
    /// </summary>
    public static class ConfirmModalView
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string DefaultVariant = "confirm";

        // ─── Public Factory ───────────────────────────────────────────────────────

        /// <summary>
        /// Creates a complete confirm modal VisualElement tree.
        /// The returned element is ready to be inserted into the modal host by ModalRouter.
        /// </summary>
        /// <param name="title">Dialog title text.</param>
        /// <param name="message">Dialog body message.</param>
        /// <param name="onConfirm">Callback invoked when the Confirm button is clicked.</param>
        /// <param name="onCancel">Callback invoked when the Cancel button is clicked.</param>
        /// <param name="variant">Semantic variant: "confirm", "danger", "warning", "info", "error".</param>
        public static VisualElement Create(
            string title,
            string message,
            Action onConfirm,
            Action onCancel,
            string variant = DefaultVariant)
        {
            // ── Root ───────────────────────────────────────────────────────────────
            var root = new VisualElement();
            root.AddToClassList("modal-frame");
            root.AddToClassList("modal-frame--small");
            root.AddToClassList("confirm-modal");

            ApplyVariantClass(root, variant);

            // ── Header ─────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList("modal-frame__header");
            header.AddToClassList("confirm-modal__header");

            var icon = new VisualElement();
            icon.AddToClassList("confirm-modal__icon");
            icon.AddToClassList("hidden");
            ApplyIconVariantClass(icon, variant);
            header.Add(icon);

            var titleLabel = new Label(title ?? "Confirm Action");
            titleLabel.AddToClassList("modal-frame__title");
            titleLabel.AddToClassList("confirm-modal__title");
            header.Add(titleLabel);

            root.Add(header);

            // ── Body ───────────────────────────────────────────────────────────────
            var body = new VisualElement();
            body.AddToClassList("modal-frame__body");
            body.AddToClassList("confirm-modal__body");

            var messageLabel = new Label(message ?? "Are you sure you want to proceed?");
            messageLabel.AddToClassList("confirm-modal__message");
            body.Add(messageLabel);

            root.Add(body);

            // ── Footer ─────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList("modal-frame__footer");

            var secondaryActions = new VisualElement();
            secondaryActions.AddToClassList("modal-frame__footer-secondary");
            footer.Add(secondaryActions);

            var primaryActions = new VisualElement();
            primaryActions.AddToClassList("modal-frame__footer-primary");

            var cancelButton = new Button(onCancel);
            cancelButton.text = "Cancel";
            cancelButton.AddToClassList("confirm-modal__cancel");
            primaryActions.Add(cancelButton);

            var confirmButton = new Button(onConfirm);
            confirmButton.text = "Confirm";
            confirmButton.AddToClassList("confirm-modal__confirm");
            primaryActions.Add(confirmButton);

            footer.Add(primaryActions);
            root.Add(footer);

            return root;
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        /// <summary>Applies the root-level variant modifier class.</summary>
        private static void ApplyVariantClass(VisualElement root, string variant)
        {
            switch (variant)
            {
                case "danger":
                    root.AddToClassList("confirm-modal--danger");
                    break;
                case "warning":
                    root.AddToClassList("confirm-modal--warning");
                    break;
                // "confirm", "info", "error", and unknown variants use no root modifier.
            }
        }

        /// <summary>
        /// Applies an icon variant class and un-hides the icon for variants
        /// that include a semantic icon.
        /// </summary>
        private static void ApplyIconVariantClass(VisualElement icon, string variant)
        {
            switch (variant)
            {
                case "info":
                    icon.AddToClassList("icon--info");
                    icon.RemoveFromClassList("hidden");
                    break;
                case "warning":
                    icon.RemoveFromClassList("hidden");
                    break;
                case "error":
                case "danger":
                    icon.RemoveFromClassList("hidden");
                    break;
                // "confirm" keeps icon hidden per spec.
            }
        }
    }
}
