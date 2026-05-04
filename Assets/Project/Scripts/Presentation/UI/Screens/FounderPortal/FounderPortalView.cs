using System;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Founder Portal screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates KPI cards, dashboard cards, quick action rows, activity rows, and up-next items.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FounderPortalView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from FounderPortalScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a KPI card is clicked. Argument is the card's DrillDownRouteId.</summary>
        public event Action<string> OnKpiCardClicked;

        /// <summary>Fired when a dashboard card is clicked. Argument is the card's DrillDownRouteId.</summary>
        public event Action<string> OnDashboardCardClicked;

        /// <summary>Fired when a quick action is activated. Argument is the action's TargetRouteId.</summary>
        public event Action<string> OnQuickActionClicked;

        /// <summary>Fired when a recent activity row is clicked. Argument is the item's DrillDownRouteId.</summary>
        public event Action<string> OnRecentActivityClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly VisualElement _screenHeader;
        private readonly Label         _headerTitle;
        private readonly Label         _headerSubtitle;

        // ─── Dynamic content containers ──────────────────────────────────────────────

        private readonly VisualElement _kpiGrid;
        private readonly VisualElement _dashboardMain;
        private readonly VisualElement _dashboardSide;
        private readonly VisualElement _upNextCard;
        private readonly VisualElement _upNextList;
        private readonly VisualElement _quickActionsCard;
        private readonly VisualElement _quickActionsList;
        private readonly VisualElement _recentActivityList;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;
        private readonly Label _recentActivityTitle;
        private readonly Label _upNextTitle;
        private readonly Label _quickActionsTitle;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public FounderPortalView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "FounderPortalView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _contentContainer = QueryElement(root, "ContentContainer");
            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");

            // ── Header ───────────────────────────────────────────────────────────────

            _screenHeader   = QueryElement(root, "ScreenHeader");
            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Dynamic containers ───────────────────────────────────────────────────

            _kpiGrid            = QueryElement(root, "KpiGrid");
            _dashboardMain      = QueryElement(root, "DashboardMain");
            _dashboardSide      = QueryElement(root, "DashboardSide");
            _upNextCard         = QueryElement(root, "UpNextCard");
            _upNextList         = QueryElement(root, "UpNextList");
            _quickActionsCard   = QueryElement(root, "QuickActionsCard");
            _quickActionsList   = QueryElement(root, "QuickActionsList");
            _recentActivityList = QueryElement(root, "RecentActivityList");

            // ── Text labels ──────────────────────────────────────────────────────────

            _errorMessage        = root.Q<Label>("ErrorMessage");
            _emptyStateTitle     = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody      = root.Q<Label>("EmptyStateBody");
            _recentActivityTitle = root.Q<Label>("RecentActivityTitle");
            _upNextTitle         = root.Q<Label>("UpNextTitle");
            _quickActionsTitle   = root.Q<Label>("QuickActionsTitle");

            LogIfNull(_errorMessage,        "ErrorMessage");
            LogIfNull(_emptyStateTitle,     "EmptyStateTitle");
            LogIfNull(_emptyStateBody,      "EmptyStateBody");
            LogIfNull(_recentActivityTitle, "RecentActivityTitle");
            LogIfNull(_upNextTitle,         "UpNextTitle");
            LogIfNull(_quickActionsTitle,   "QuickActionsTitle");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic card/row lists on each call.
        /// </summary>
        public void Bind(FounderPortalViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "FounderPortalView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No data available.");
                return;
            }

            // ── Loading state ────────────────────────────────────────────────────────

            if (viewModel.IsLoading)
            {
                SetVisible(_loadingState,     true);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       false);
                return;
            }

            // ── Error state ──────────────────────────────────────────────────────────

            if (viewModel.HasError)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            // ── Empty state ──────────────────────────────────────────────────────────

            bool hasContent = (viewModel.KpiCards != null    && viewModel.KpiCards.Count    > 0)
                           || (viewModel.DashboardCards != null && viewModel.DashboardCards.Count > 0);

            if (!hasContent)
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       true);

                if (_emptyStateTitle != null)
                {
                    _emptyStateTitle.text = viewModel.EmptyStateTitle;
                }

                if (_emptyStateBody != null)
                {
                    _emptyStateBody.text = viewModel.EmptyStateBody;
                }

                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, true);
            SetVisible(_errorState,       false);
            SetVisible(_emptyState,       false);

            // Header
            if (_headerTitle != null)
            {
                _headerTitle.text = viewModel.ScreenTitle;
            }

            if (_headerSubtitle != null)
            {
                _headerSubtitle.text = viewModel.ScreenSubtitle;
            }

            // KPI Grid
            BuildKpiGrid(viewModel);

            // Dashboard main and side content
            BuildDashboardMain(viewModel);

            // Up-next milestones
            BuildUpNextList(viewModel);

            // Quick actions
            BuildQuickActionsList(viewModel);

            // Recent activity
            BuildRecentActivityList(viewModel);

            // Root warning state classes
            ApplyRootWarningClasses(viewModel);
        }

        // ─── Private — card builders ─────────────────────────────────────────────────

        private void BuildKpiGrid(FounderPortalViewModel viewModel)
        {
            if (_kpiGrid == null)
            {
                return;
            }

            _kpiGrid.Clear();

            if (viewModel.KpiCards == null)
            {
                return;
            }

            foreach (KpiCardViewModel card in viewModel.KpiCards)
            {
                VisualElement cardEl = CreateKpiCard(card);
                _kpiGrid.Add(cardEl);
            }
        }

        private VisualElement CreateKpiCard(KpiCardViewModel card)
        {
            var container = new VisualElement();
            container.AddToClassList("founder-portal__kpi-card");
            ApplySemanticStateClass(container, card.SemanticState);

            if (card.IsClickable)
            {
                container.AddToClassList("is-clickable");
            }

            var labelEl = new Label(card.Label);
            labelEl.AddToClassList("founder-portal__kpi-card__label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(card.Value);
            valueEl.AddToClassList("founder-portal__kpi-card__value");
            valueEl.AddToClassList("text-heading");

            var trendEl = new Label(card.TrendText);
            trendEl.AddToClassList("founder-portal__kpi-card__trend");
            trendEl.AddToClassList("text-caption");

            container.Add(labelEl);
            container.Add(valueEl);
            container.Add(trendEl);

            if (card.IsClickable)
            {
                string routeId = card.DrillDownRouteId;
                container.RegisterCallback<ClickEvent>(_ => OnKpiCardClicked?.Invoke(routeId));
            }

            return container;
        }

        private void BuildDashboardMain(FounderPortalViewModel viewModel)
        {
            if (_dashboardMain == null)
            {
                return;
            }

            _dashboardMain.Clear();

            if (viewModel.DashboardCards == null)
            {
                return;
            }

            foreach (DashboardCardViewModel card in viewModel.DashboardCards)
            {
                VisualElement cardEl = CreateDashboardCard(card);
                _dashboardMain.Add(cardEl);
            }
        }

        private VisualElement CreateDashboardCard(DashboardCardViewModel card)
        {
            var container = new VisualElement();
            container.AddToClassList("founder-portal__dashboard-card");
            ApplySemanticStateClass(container, card.SemanticState);

            if (card.IsClickable)
            {
                container.AddToClassList("is-clickable");
            }

            var titleEl = new Label(card.Title);
            titleEl.AddToClassList("founder-portal__card-title");
            titleEl.AddToClassList("text-subheading");

            var summaryEl = new Label(card.SummaryText);
            summaryEl.AddToClassList("founder-portal__dashboard-card__summary");
            summaryEl.AddToClassList("text-body");

            container.Add(titleEl);
            container.Add(summaryEl);

            if (card.DetailLines != null)
            {
                foreach (string line in card.DetailLines)
                {
                    var lineEl = new Label(line);
                    lineEl.AddToClassList("founder-portal__dashboard-card__detail");
                    lineEl.AddToClassList("text-caption");
                    container.Add(lineEl);
                }
            }

            if (card.IsClickable)
            {
                string routeId = card.DrillDownRouteId;
                container.RegisterCallback<ClickEvent>(_ => OnDashboardCardClicked?.Invoke(routeId));
            }

            return container;
        }

        private void BuildUpNextList(FounderPortalViewModel viewModel)
        {
            if (_upNextList == null)
            {
                return;
            }

            _upNextList.Clear();

            UpNextItemViewModel upNext = viewModel.UpNextItem;

            if (upNext == null)
            {
                return;
            }

            if (_upNextTitle != null)
            {
                _upNextTitle.text = upNext.Title;
            }

            if (!upNext.HasItems || upNext.Items == null || upNext.Items.Count == 0)
            {
                var emptyLabel = new Label(upNext.EmptyMessage);
                emptyLabel.AddToClassList("text-body");
                emptyLabel.AddToClassList("text-muted");
                _upNextList.Add(emptyLabel);
                return;
            }

            foreach (string item in upNext.Items)
            {
                var itemEl = new VisualElement();
                itemEl.AddToClassList("founder-portal__up-next-item");

                var itemLabel = new Label(item);
                itemLabel.AddToClassList("text-body");
                itemEl.Add(itemLabel);

                _upNextList.Add(itemEl);
            }
        }

        private void BuildQuickActionsList(FounderPortalViewModel viewModel)
        {
            if (_quickActionsList == null)
            {
                return;
            }

            _quickActionsList.Clear();

            if (_quickActionsTitle != null && viewModel.QuickActions != null)
            {
                // Title is set in UXML; only override if needed.
            }

            if (viewModel.QuickActions == null)
            {
                return;
            }

            foreach (QuickActionViewModel action in viewModel.QuickActions)
            {
                VisualElement rowEl = CreateQuickActionRow(action);
                _quickActionsList.Add(rowEl);
            }
        }

        private VisualElement CreateQuickActionRow(QuickActionViewModel action)
        {
            var row = new VisualElement();
            row.AddToClassList("founder-portal__quick-action-item");

            if (!action.IsEnabled)
            {
                row.AddToClassList("is-disabled");
            }

            if (action.IsEnabled)
            {
                row.AddToClassList("is-clickable");
            }

            var iconEl = new VisualElement();
            iconEl.AddToClassList("founder-portal__quick-action-icon");

            if (!string.IsNullOrEmpty(action.IconClass))
            {
                iconEl.AddToClassList(action.IconClass);
            }

            var labelEl = new Label(action.Label);
            labelEl.AddToClassList("founder-portal__quick-action-label");
            labelEl.AddToClassList("text-body");

            row.Add(iconEl);
            row.Add(labelEl);

            if (action.IsEnabled)
            {
                string routeId = action.TargetRouteId;
                row.RegisterCallback<ClickEvent>(_ => OnQuickActionClicked?.Invoke(routeId));
            }

            return row;
        }

        private void BuildRecentActivityList(FounderPortalViewModel viewModel)
        {
            if (_recentActivityList == null)
            {
                return;
            }

            _recentActivityList.Clear();

            if (viewModel.RecentActivityItems == null)
            {
                return;
            }

            foreach (RecentActivityItemViewModel item in viewModel.RecentActivityItems)
            {
                VisualElement rowEl = CreateActivityRow(item);
                _recentActivityList.Add(rowEl);
            }
        }

        private VisualElement CreateActivityRow(RecentActivityItemViewModel item)
        {
            var row = new VisualElement();
            row.AddToClassList("founder-portal__activity-item");
            ApplySemanticStateClass(row, item.SemanticState);

            if (item.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            var timestampEl = new Label(item.Timestamp);
            timestampEl.AddToClassList("founder-portal__activity-item__timestamp");
            timestampEl.AddToClassList("text-caption");
            timestampEl.AddToClassList("text-muted");

            var descriptionEl = new Label(item.Description);
            descriptionEl.AddToClassList("founder-portal__activity-item__description");
            descriptionEl.AddToClassList("text-body");

            var categoryEl = new Label(item.CategoryLabel);
            categoryEl.AddToClassList("founder-portal__activity-item__category");
            categoryEl.AddToClassList("pill");
            categoryEl.AddToClassList("pill--info");

            row.Add(timestampEl);
            row.Add(descriptionEl);
            row.Add(categoryEl);

            if (item.IsClickable)
            {
                string routeId = item.DrillDownRouteId;
                row.RegisterCallback<ClickEvent>(_ => OnRecentActivityClicked?.Invoke(routeId));
            }

            return row;
        }

        // ─── Private — state helpers ─────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_emptyState,       false);
            SetVisible(_errorState,       true);

            if (_errorMessage != null)
            {
                _errorMessage.text = message ?? string.Empty;
            }
        }

        private void ApplyRootWarningClasses(FounderPortalViewModel viewModel)
        {
            bool hasAnyWarning = viewModel.HasLowRunwayWarning
                              || viewModel.HasHighWorkloadWarning
                              || viewModel.HasUnreadDecisions
                              || viewModel.HasInfrastructureRisk;

            if (hasAnyWarning)
            {
                Root.AddToClassList("has-warning");
            }
            else
            {
                Root.RemoveFromClassList("has-warning");
            }
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            if (element == null)
            {
                return;
            }

            if (visible)
            {
                element.RemoveFromClassList("is-hidden");
            }
            else
            {
                element.AddToClassList("is-hidden");
            }
        }

        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        // ─── Private — query helpers ─────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            LogIfNull(element, name);
            return element;
        }

        private static void LogIfNull(object element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"FounderPortalView: element '{name}' not found in UXML root.");
            }
        }
    }
}
