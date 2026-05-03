using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Modals
{
    /// <summary>
    /// Programmatic detail modal frame — large or xlarge modal with:
    ///   - Avatar block (circular, initials, optional status dot)
    ///   - Identity block (name, subtitle, metadata)
    ///   - Status pill area
    ///   - Close button
    ///   - Horizontal tab bar with per-tab content switching
    ///   - Card grid body area
    ///   - Footer with primary and secondary action slots
    ///
    /// Phase 4 usage: register via ModalRouter factory.
    /// Phase 5+ usage: supply real entity data via context object.
    /// </summary>
    public sealed class DetailModalFrameView
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string IsActiveClass = "is-active";
        private const string HiddenClass   = "hidden";

        // ─── References ───────────────────────────────────────────────────────────

        private readonly Label         _nameLabel;
        private readonly Label         _subtitleLabel;
        private readonly Label         _avatarInitials;
        private readonly VisualElement _statusArea;
        private readonly VisualElement _closeButton;
        private readonly VisualElement _tabBar;
        private readonly VisualElement _tabContentHost;
        private readonly VisualElement _cardGrid;
        private readonly VisualElement _footerSecondary;
        private readonly VisualElement _footerPrimary;

        private readonly Dictionary<string, (VisualElement tab, VisualElement content)> _tabs;
        private string _activeTabId;

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>Root element to be inserted into the modal host.</summary>
        public VisualElement Root { get; }

        /// <summary>Card grid body container — inject card elements here.</summary>
        public VisualElement CardGrid => _cardGrid;

        /// <summary>Callback invoked when the close button is clicked.</summary>
        public Action OnCloseRequested { get; set; }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <param name="name">Entity display name shown in the identity block.</param>
        /// <param name="subtitle">Optional subtitle below the name.</param>
        /// <param name="sizeClass">Size modifier: modal-frame--large | modal-frame--xlarge.</param>
        public DetailModalFrameView(
            string name,
            string subtitle  = null,
            string sizeClass = "modal-frame--large")
        {
            _tabs = new Dictionary<string, (VisualElement, VisualElement)>(StringComparer.Ordinal);

            // ── Root ───────────────────────────────────────────────────────────────
            Root = new VisualElement();
            Root.AddToClassList("modal-frame");
            Root.AddToClassList(sizeClass);
            Root.AddToClassList("detail-modal");

            // ── Header ─────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList("modal-frame__header");
            header.AddToClassList("detail-modal__header");

            // Avatar
            var avatarWrapper = new VisualElement();
            avatarWrapper.AddToClassList("detail-modal__avatar");

            _avatarInitials = new Label("??");
            _avatarInitials.AddToClassList("detail-modal__avatar-initials");
            avatarWrapper.Add(_avatarInitials);
            header.Add(avatarWrapper);

            // Identity block
            var identityBlock = new VisualElement();
            identityBlock.AddToClassList("detail-modal__identity");

            _nameLabel = new Label(name ?? string.Empty);
            _nameLabel.AddToClassList("detail-modal__identity-name");
            identityBlock.Add(_nameLabel);

            _subtitleLabel = new Label(subtitle ?? string.Empty);
            _subtitleLabel.AddToClassList("detail-modal__identity-subtitle");

            if (string.IsNullOrEmpty(subtitle))
            {
                _subtitleLabel.AddToClassList(HiddenClass);
            }

            identityBlock.Add(_subtitleLabel);
            header.Add(identityBlock);

            // Status area
            _statusArea = new VisualElement();
            _statusArea.AddToClassList("detail-modal__status");
            header.Add(_statusArea);

            // Close button
            _closeButton = new VisualElement();
            _closeButton.AddToClassList("modal-frame__close");
            _closeButton.RegisterCallback<ClickEvent>(_ => OnCloseRequested?.Invoke());
            header.Add(_closeButton);

            Root.Add(header);

            // ── Tab Bar ────────────────────────────────────────────────────────────
            _tabBar = new VisualElement();
            _tabBar.AddToClassList("detail-modal__tabs");
            Root.Add(_tabBar);

            // Tab content host — only the active tab content is visible.
            _tabContentHost = new VisualElement();
            _tabContentHost.AddToClassList("detail-modal__tab-content-host");

            // ── Body ───────────────────────────────────────────────────────────────
            var body = new ScrollView(ScrollViewMode.Vertical);
            body.AddToClassList("modal-frame__body");
            body.AddToClassList("detail-modal__body");

            _cardGrid = new VisualElement();
            _cardGrid.AddToClassList("detail-modal__card-grid");
            body.Add(_tabContentHost);
            body.Add(_cardGrid);

            Root.Add(body);

            // ── Footer ─────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList("modal-frame__footer");
            footer.AddToClassList("detail-modal__footer");

            _footerSecondary = new VisualElement();
            _footerSecondary.AddToClassList("modal-frame__footer-secondary");
            footer.Add(_footerSecondary);

            _footerPrimary = new VisualElement();
            _footerPrimary.AddToClassList("modal-frame__footer-primary");
            footer.Add(_footerPrimary);

            Root.Add(footer);
        }

        // ─── Methods ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the avatar initials text and applies an optional status dot colour class.
        /// </summary>
        /// <param name="initials">1-2 character initials shown inside the avatar circle.</param>
        /// <param name="statusColourClass">
        /// Optional USS class for the status dot colour, e.g. "status-dot--success".
        /// Pass null to hide the status dot.
        /// </param>
        public void SetAvatar(string initials, string statusColourClass = null)
        {
            _avatarInitials.text = initials ?? string.Empty;

            // Status dot — child of avatar with status colour class.
            var existingDot = Root.Q<VisualElement>("AvatarStatusDot");
            if (existingDot != null)
            {
                existingDot.RemoveFromHierarchy();
            }

            if (!string.IsNullOrEmpty(statusColourClass))
            {
                var avatarWrapper = Root.Q(className: "detail-modal__avatar");
                if (avatarWrapper != null)
                {
                    var dot = new VisualElement();
                    dot.name = "AvatarStatusDot";
                    dot.AddToClassList("detail-modal__avatar-status-dot");
                    dot.AddToClassList(statusColourClass);
                    avatarWrapper.Add(dot);
                }
            }
        }

        /// <summary>Sets the status pill label and pill modifier class in the status area.</summary>
        /// <param name="label">Text shown in the status pill.</param>
        /// <param name="pillClass">USS modifier class for the pill colour, e.g. "pill--success".</param>
        public void SetStatus(string label, string pillClass)
        {
            _statusArea.Clear();

            var pill = new Label(label ?? string.Empty);
            pill.AddToClassList("detail-modal__status-pill");

            if (!string.IsNullOrEmpty(pillClass))
            {
                pill.AddToClassList(pillClass);
            }

            _statusArea.Add(pill);
        }

        /// <summary>
        /// Adds a tab to the horizontal tab bar and registers its content panel.
        /// The first tab added is automatically selected.
        /// </summary>
        /// <param name="tabId">Stable ID used to select the tab programmatically.</param>
        /// <param name="label">Display label shown in the tab button.</param>
        /// <param name="content">Content VisualElement shown when this tab is active.</param>
        public void AddTab(string tabId, string label, VisualElement content)
        {
            if (string.IsNullOrEmpty(tabId) || _tabs.ContainsKey(tabId))
            {
                return;
            }

            // Tab button
            var tab = new Label(label ?? tabId);
            tab.AddToClassList("detail-modal__tab");
            tab.RegisterCallback<ClickEvent>(_ => SelectTab(tabId));
            _tabBar.Add(tab);

            // Content panel — hidden by default.
            content.AddToClassList(HiddenClass);
            _tabContentHost.Add(content);

            _tabs[tabId] = (tab, content);

            // Auto-select first tab.
            if (_tabs.Count == 1)
            {
                SelectTab(tabId);
            }
        }

        /// <summary>Switches the visible content to the tab with the given ID.</summary>
        public void SelectTab(string tabId)
        {
            if (!_tabs.ContainsKey(tabId))
            {
                return;
            }

            // Deactivate current.
            if (_activeTabId != null && _tabs.TryGetValue(_activeTabId, out var current))
            {
                current.tab.RemoveFromClassList(IsActiveClass);
                current.content.AddToClassList(HiddenClass);
            }

            // Activate target.
            var target = _tabs[tabId];
            target.tab.AddToClassList(IsActiveClass);
            target.content.RemoveFromClassList(HiddenClass);

            _activeTabId = tabId;
        }
    }
}
