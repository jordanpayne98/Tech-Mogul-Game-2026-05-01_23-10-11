using System;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Modals
{
    /// <summary>
    /// Programmatic wrapper for the generic modal frame.
    /// Builds the ModalFrame VisualElement tree directly in C# and exposes
    /// typed access to the header, body, and footer areas.
    ///
    /// Caller pattern:
    ///   var frame = new ModalFrameView("Settings", "modal-frame--standard");
    ///   frame.OnCloseRequested = () => _modalRouter.CloseModal();
    ///   frame.Body.Add(myContent);
    ///   return frame.Root;
    /// </summary>
    public sealed class ModalFrameView
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string HiddenClass = "hidden";

        // ─── References ───────────────────────────────────────────────────────────

        private readonly Label         _titleLabel;
        private readonly Label         _subtitleLabel;
        private readonly VisualElement _closeButton;
        private readonly ScrollView    _bodyScrollView;
        private readonly VisualElement _footerSecondary;
        private readonly VisualElement _footerPrimary;

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>Root element to be inserted into the modal host.</summary>
        public VisualElement Root { get; }

        /// <summary>Scrollable body container — inject content here.</summary>
        public VisualElement Body => _bodyScrollView.contentContainer;

        /// <summary>Footer primary (right) actions container.</summary>
        public VisualElement Footer => _footerPrimary;

        /// <summary>Footer secondary (left) actions container.</summary>
        public VisualElement FooterSecondary => _footerSecondary;

        /// <summary>Callback invoked when the close button is clicked.</summary>
        public Action OnCloseRequested { get; set; }

        /// <summary>
        /// When true, backdrop click does not dismiss this modal.
        /// The ModalRouter checks this via the USS class "modal-require-explicit-dismiss"
        /// on the root element.
        /// </summary>
        public bool RequireExplicitDismiss
        {
            get => Root.ClassListContains("modal-require-explicit-dismiss");
            set
            {
                if (value)
                {
                    Root.AddToClassList("modal-require-explicit-dismiss");
                }
                else
                {
                    Root.RemoveFromClassList("modal-require-explicit-dismiss");
                }
            }
        }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <param name="title">Initial header title text.</param>
        /// <param name="sizeClass">
        /// Size-band modifier applied to the root: modal-frame--small | modal-frame--standard
        /// | modal-frame--large | modal-frame--xlarge. Defaults to modal-frame--standard.
        /// </param>
        public ModalFrameView(string title, string sizeClass = "modal-frame--standard")
        {
            // ── Root ───────────────────────────────────────────────────────────────
            Root = new VisualElement();
            Root.AddToClassList("modal-frame");
            Root.AddToClassList(sizeClass);

            // ── Header ─────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList("modal-frame__header");

            var titleBlock = new VisualElement();
            titleBlock.AddToClassList("modal-frame__title-block");

            _titleLabel = new Label(title);
            _titleLabel.AddToClassList("modal-frame__title");
            titleBlock.Add(_titleLabel);

            _subtitleLabel = new Label();
            _subtitleLabel.AddToClassList("modal-frame__subtitle");
            _subtitleLabel.AddToClassList(HiddenClass);
            titleBlock.Add(_subtitleLabel);

            header.Add(titleBlock);

            _closeButton = new VisualElement();
            _closeButton.AddToClassList("modal-frame__close");
            _closeButton.RegisterCallback<ClickEvent>(_ => OnCloseRequested?.Invoke());
            header.Add(_closeButton);

            Root.Add(header);

            // ── Body ───────────────────────────────────────────────────────────────
            _bodyScrollView = new ScrollView(ScrollViewMode.Vertical);
            _bodyScrollView.AddToClassList("modal-frame__body");
            Root.Add(_bodyScrollView);

            // ── Footer ─────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList("modal-frame__footer");

            _footerSecondary = new VisualElement();
            _footerSecondary.AddToClassList("modal-frame__footer-secondary");
            footer.Add(_footerSecondary);

            _footerPrimary = new VisualElement();
            _footerPrimary.AddToClassList("modal-frame__footer-primary");
            footer.Add(_footerPrimary);

            Root.Add(footer);
        }

        // ─── Methods ──────────────────────────────────────────────────────────────

        /// <summary>Sets the header title text.</summary>
        public void SetTitle(string title)
        {
            _titleLabel.text = title;
        }

        /// <summary>Sets the optional header subtitle. Passing null or empty hides it.</summary>
        public void SetSubtitle(string subtitle)
        {
            bool hasSubtitle = !string.IsNullOrEmpty(subtitle);
            _subtitleLabel.text = subtitle ?? string.Empty;

            if (hasSubtitle)
            {
                _subtitleLabel.RemoveFromClassList(HiddenClass);
            }
            else
            {
                _subtitleLabel.AddToClassList(HiddenClass);
            }
        }
    }
}
