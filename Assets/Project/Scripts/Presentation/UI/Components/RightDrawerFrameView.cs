using System;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Components
{
    /// <summary>
    /// Programmatic view class for the generic right drawer frame.
    /// Builds the drawer VisualElement tree directly in C# and exposes
    /// typed access to the title, body, and footer action callbacks.
    ///
    /// Caller pattern:
    ///   var drawer = new RightDrawerFrameView("Filters");
    ///   drawer.OnCloseRequested = () => _drawerRouter.CloseDrawer();
    ///   drawer.Body.Add(myFilterContent);
    ///   return drawer.Root;
    ///
    /// The Root is injected into the RightDrawerHost by RightDrawerRouter.
    /// </summary>
    public class RightDrawerFrameView
    {
        // ─── References ───────────────────────────────────────────────────────────────

        private readonly Label         _titleLabel;
        private readonly VisualElement _closeButton;
        private readonly ScrollView    _bodyScrollView;
        private readonly Button        _clearButton;
        private readonly Button        _applyButton;

        // ─── Public API ───────────────────────────────────────────────────────────────

        /// <summary>Root element to be injected into the drawer host.</summary>
        public VisualElement Root { get; }

        /// <summary>Scrollable body container — inject drawer content here.</summary>
        public VisualElement Body => _bodyScrollView.contentContainer;

        /// <summary>Callback invoked when the close (X) button is clicked.</summary>
        public Action OnCloseRequested { get; set; }

        /// <summary>Callback invoked when the Clear button is clicked.</summary>
        public Action OnClearRequested { get; set; }

        /// <summary>Callback invoked when the Apply button is clicked.</summary>
        public Action OnApplyRequested { get; set; }

        // ─── Constructor ──────────────────────────────────────────────────────────────

        /// <param name="title">Initial drawer title text.</param>
        public RightDrawerFrameView(string title)
        {
            // ── Root ───────────────────────────────────────────────────────────────────
            Root = new VisualElement();
            Root.AddToClassList("right-drawer");
            Root.AddToClassList("is-open");

            // ── Header ─────────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList("right-drawer__header");

            _titleLabel = new Label(title);
            _titleLabel.AddToClassList("right-drawer__title");
            _titleLabel.AddToClassList("text-heading");
            header.Add(_titleLabel);

            _closeButton = new VisualElement();
            _closeButton.AddToClassList("right-drawer__close");
            _closeButton.AddToClassList("icon-button");
            _closeButton.RegisterCallback<ClickEvent>(_ => OnCloseRequested?.Invoke());
            header.Add(_closeButton);

            Root.Add(header);

            // ── Body ───────────────────────────────────────────────────────────────────
            _bodyScrollView = new ScrollView(ScrollViewMode.Vertical);
            _bodyScrollView.AddToClassList("right-drawer__body");
            Root.Add(_bodyScrollView);

            // ── Footer ─────────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList("right-drawer__footer");

            _clearButton = new Button(() => OnClearRequested?.Invoke());
            _clearButton.text = "Clear Filters";
            _clearButton.AddToClassList("base-button");
            _clearButton.AddToClassList("base-button--secondary");
            _clearButton.AddToClassList("right-drawer__footer-clear");
            footer.Add(_clearButton);

            _applyButton = new Button(() => OnApplyRequested?.Invoke());
            _applyButton.text = "Apply Filters";
            _applyButton.AddToClassList("base-button");
            _applyButton.AddToClassList("base-button--primary");
            _applyButton.AddToClassList("right-drawer__footer-apply");
            footer.Add(_applyButton);

            Root.Add(footer);
        }

        // ─── Methods ──────────────────────────────────────────────────────────────────

        /// <summary>Updates the drawer header title text.</summary>
        public void SetTitle(string title)
        {
            _titleLabel.text = title ?? string.Empty;
        }
    }
}
