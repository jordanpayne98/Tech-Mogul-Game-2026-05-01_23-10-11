using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Product Detail screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates metric cards, risk field rows, history rows, and placeholder tab panels.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductDetailView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from ProductDetailScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a tab button is clicked. Argument is the stable tab ID.</summary>
        public event Action<string> OnTabSelected;

        /// <summary>Fired when the back link is clicked.</summary>
        public event Action OnBackClicked;

        /// <summary>Fired when a metric card is clicked. Argument is the card's DrillDownRouteId.</summary>
        public event Action<string> OnMetricCardClicked;

        /// <summary>Fired when the Launch button is clicked.</summary>
        public event Action OnLaunchClicked;

        /// <summary>Fired when the Delay button is clicked.</summary>
        public event Action OnDelayClicked;

        /// <summary>Fired when the Cancel button is clicked.</summary>
        public event Action OnCancelClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _contentContainer;

        // ─── Error / empty text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label         _productName;
        private readonly Label         _productId;
        private readonly Label         _productFamily;
        private readonly Label         _productType;
        private readonly Label         _productMarket;
        private readonly Label         _productSegment;
        private readonly Label         _productPriceModel;
        private readonly Label         _productLaunchTarget;
        private readonly Label         _statusChipLabel;
        private readonly VisualElement _statusChip;
        private readonly VisualElement _backLink;

        // ─── Action bar ──────────────────────────────────────────────────────────────

        private readonly Button _launchButton;
        private readonly Button _delayButton;
        private readonly Button _cancelButton;

        // ─── Tab buttons ─────────────────────────────────────────────────────────────

        private readonly Dictionary<string, Button> _tabButtons = new Dictionary<string, Button>();

        // ─── Tab panels ──────────────────────────────────────────────────────────────

        private readonly VisualElement _overviewPanel;
        private readonly VisualElement _developmentPanel;
        private readonly VisualElement _qualityPanel;
        private readonly VisualElement _budgetPanel;
        private readonly VisualElement _marketingPanel;
        private readonly VisualElement _supportPanel;
        private readonly VisualElement _reportsPanel;
        private readonly VisualElement _historyPanel;
        private readonly VisualElement _competitorsPanel;

        // ─── Dynamic content containers ──────────────────────────────────────────────

        private readonly VisualElement _overviewCardGrid;
        private readonly VisualElement _historyList;

        // ─── Right risk panel ────────────────────────────────────────────────────────

        private readonly VisualElement _riskPanel;
        private readonly Label         _riskOverallValue;
        private readonly VisualElement _riskFieldList;
        private readonly VisualElement _highRiskWarning;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public ProductDetailView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "ProductDetailView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");
            _contentContainer = QueryElement(root, "ContentContainer");

            // ── Error / empty text ───────────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");

            // ── Header ───────────────────────────────────────────────────────────────

            _productName         = root.Q<Label>("ProductName");
            _productId           = root.Q<Label>("ProductId");
            _productFamily       = root.Q<Label>("ProductFamily");
            _productType         = root.Q<Label>("ProductType");
            _productMarket       = root.Q<Label>("ProductMarket");
            _productSegment      = root.Q<Label>("ProductSegment");
            _productPriceModel   = root.Q<Label>("ProductPriceModel");
            _productLaunchTarget = root.Q<Label>("ProductLaunchTarget");
            _statusChipLabel     = root.Q<Label>("StatusChipLabel");
            _statusChip          = QueryElement(root, "StatusChip");

            LogIfNull(_productName,         "ProductName");
            LogIfNull(_productId,           "ProductId");
            LogIfNull(_productFamily,       "ProductFamily");
            LogIfNull(_productType,         "ProductType");
            LogIfNull(_productMarket,       "ProductMarket");
            LogIfNull(_productSegment,      "ProductSegment");
            LogIfNull(_productPriceModel,   "ProductPriceModel");
            LogIfNull(_productLaunchTarget, "ProductLaunchTarget");
            LogIfNull(_statusChipLabel,     "StatusChipLabel");

            // ── Back link ────────────────────────────────────────────────────────────

            _backLink = QueryElement(root, "BackLink");
            if (_backLink != null)
            {
                _backLink.RegisterCallback<ClickEvent>(_ => OnBackClicked?.Invoke());
                _backLink.AddToClassList("is-clickable");
            }

            // ── Action bar ───────────────────────────────────────────────────────────

            _launchButton = root.Q<Button>("LaunchButton");
            _delayButton  = root.Q<Button>("DelayButton");
            _cancelButton = root.Q<Button>("CancelButton");

            LogIfNull(_launchButton, "LaunchButton");
            LogIfNull(_delayButton,  "DelayButton");
            LogIfNull(_cancelButton, "CancelButton");

            if (_launchButton != null)
            {
                _launchButton.RegisterCallback<ClickEvent>(_ => OnLaunchClicked?.Invoke());
            }

            if (_delayButton != null)
            {
                _delayButton.RegisterCallback<ClickEvent>(_ => OnDelayClicked?.Invoke());
            }

            if (_cancelButton != null)
            {
                _cancelButton.RegisterCallback<ClickEvent>(_ => OnCancelClicked?.Invoke());
            }

            // ── Tab buttons ──────────────────────────────────────────────────────────

            RegisterTabButton(root, "TabOverview",     ProductDetailTabIds.Overview);
            RegisterTabButton(root, "TabDevelopment",  ProductDetailTabIds.Development);
            RegisterTabButton(root, "TabQuality",      ProductDetailTabIds.Quality);
            RegisterTabButton(root, "TabBudget",       ProductDetailTabIds.Budget);
            RegisterTabButton(root, "TabMarketing",    ProductDetailTabIds.Marketing);
            RegisterTabButton(root, "TabSupport",      ProductDetailTabIds.Support);
            RegisterTabButton(root, "TabReports",      ProductDetailTabIds.Reports);
            RegisterTabButton(root, "TabHistory",      ProductDetailTabIds.History);
            RegisterTabButton(root, "TabCompetitors",  ProductDetailTabIds.Competitors);

            // ── Tab panels ───────────────────────────────────────────────────────────

            _overviewPanel     = QueryElement(root, "OverviewPanel");
            _developmentPanel  = QueryElement(root, "DevelopmentPanel");
            _qualityPanel      = QueryElement(root, "QualityPanel");
            _budgetPanel       = QueryElement(root, "BudgetPanel");
            _marketingPanel    = QueryElement(root, "MarketingPanel");
            _supportPanel      = QueryElement(root, "SupportPanel");
            _reportsPanel      = QueryElement(root, "ReportsPanel");
            _historyPanel      = QueryElement(root, "HistoryPanel");
            _competitorsPanel  = QueryElement(root, "CompetitorsPanel");

            // ── Dynamic content ──────────────────────────────────────────────────────

            _overviewCardGrid = QueryElement(root, "OverviewCardGrid");
            _historyList      = QueryElement(root, "HistoryList");

            // ── Risk panel ───────────────────────────────────────────────────────────

            _riskPanel       = QueryElement(root, "RiskPanel");
            _riskOverallValue = root.Q<Label>("RiskOverallValue");
            _riskFieldList   = QueryElement(root, "RiskFieldList");
            _highRiskWarning = QueryElement(root, "HighRiskWarning");

            LogIfNull(_riskOverallValue, "RiskOverallValue");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic card/row lists on each call.
        /// </summary>
        public void Bind(ProductDetailScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "ProductDetailView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No product data available.");
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

            bool hasContent = !string.IsNullOrEmpty(viewModel.ProductId);

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
            BindHeader(viewModel);

            // Action bar
            BindActionBar(viewModel);

            // Tabs
            ActivateTab(viewModel.ActiveTabId);

            // Overview metric cards
            BuildOverviewCards(viewModel);

            // Placeholder content for non-overview tabs
            BuildPlaceholderTabPanels();

            // Risk panel
            BindRiskPanel(viewModel);

            // Root semantic state classes
            ApplyRootSemanticClasses(viewModel);
        }

        // ─── Private — header ────────────────────────────────────────────────────────

        private void BindHeader(ProductDetailScreenViewModel viewModel)
        {
            SetLabelText(_productName,         viewModel.ProductName);
            SetLabelText(_productId,           viewModel.ProductId);
            SetLabelText(_productFamily,       viewModel.Family);
            SetLabelText(_productType,         viewModel.Type);
            SetLabelText(_productMarket,       viewModel.TargetMarket);
            SetLabelText(_productSegment,      viewModel.CustomerSegment);
            SetLabelText(_productPriceModel,   viewModel.PriceModel);
            SetLabelText(_productLaunchTarget, viewModel.LaunchTarget);
            SetLabelText(_statusChipLabel,     viewModel.Status);
        }

        // ─── Private — action bar ────────────────────────────────────────────────────

        private void BindActionBar(ProductDetailScreenViewModel viewModel)
        {
            // Phase 5: all action buttons are disabled — lifecycle state changes belong to Application/Core.
            SetButtonEnabled(_launchButton, viewModel.CanLaunch);
            SetButtonEnabled(_delayButton,  viewModel.CanDelay);
            SetButtonEnabled(_cancelButton, viewModel.CanCancel);
        }

        // ─── Private — tab management ────────────────────────────────────────────────

        private void RegisterTabButton(VisualElement root, string elementName, string tabId)
        {
            Button btn = root.Q<Button>(elementName);
            if (btn == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"ProductDetailView: tab button '{elementName}' not found in UXML root.");
                return;
            }

            _tabButtons[tabId] = btn;

            string capturedTabId = tabId;
            btn.RegisterCallback<ClickEvent>(_ => OnTabSelected?.Invoke(capturedTabId));
        }

        private void ActivateTab(string activeTabId)
        {
            if (string.IsNullOrEmpty(activeTabId))
            {
                return;
            }

            // Update tab button visual states.
            foreach (KeyValuePair<string, Button> pair in _tabButtons)
            {
                if (pair.Key == activeTabId)
                {
                    pair.Value.AddToClassList("is-active");
                }
                else
                {
                    pair.Value.RemoveFromClassList("is-active");
                }
            }

            // Show or hide each panel based on the active tab.
            SetPanelVisible(_overviewPanel,    activeTabId == ProductDetailTabIds.Overview);
            SetPanelVisible(_developmentPanel, activeTabId == ProductDetailTabIds.Development);
            SetPanelVisible(_qualityPanel,     activeTabId == ProductDetailTabIds.Quality);
            SetPanelVisible(_budgetPanel,      activeTabId == ProductDetailTabIds.Budget);
            SetPanelVisible(_marketingPanel,   activeTabId == ProductDetailTabIds.Marketing);
            SetPanelVisible(_supportPanel,     activeTabId == ProductDetailTabIds.Support);
            SetPanelVisible(_reportsPanel,     activeTabId == ProductDetailTabIds.Reports);
            SetPanelVisible(_historyPanel,     activeTabId == ProductDetailTabIds.History);
            SetPanelVisible(_competitorsPanel, activeTabId == ProductDetailTabIds.Competitors);
        }

        private static void SetPanelVisible(VisualElement panel, bool visible)
        {
            if (panel == null)
            {
                return;
            }

            if (visible)
            {
                panel.RemoveFromClassList("is-hidden");
            }
            else
            {
                panel.AddToClassList("is-hidden");
            }
        }

        // ─── Private — overview card builder ─────────────────────────────────────────

        private void BuildOverviewCards(ProductDetailScreenViewModel viewModel)
        {
            if (_overviewCardGrid == null)
            {
                return;
            }

            _overviewCardGrid.Clear();

            if (viewModel.OverviewCards == null)
            {
                return;
            }

            foreach (ProductMetricCardViewModel card in viewModel.OverviewCards)
            {
                VisualElement cardEl = CreateMetricCard(card);
                _overviewCardGrid.Add(cardEl);
            }
        }

        private VisualElement CreateMetricCard(ProductMetricCardViewModel card)
        {
            var container = new VisualElement();
            container.AddToClassList("product-detail__card");
            ApplySemanticStateClass(container, card.SemanticState);

            if (card.IsClickable)
            {
                container.AddToClassList("is-clickable");
            }

            var labelEl = new Label(card.Label);
            labelEl.AddToClassList("product-detail__card__label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(card.Value);
            valueEl.AddToClassList("product-detail__card__value");
            valueEl.AddToClassList("text-heading");

            container.Add(labelEl);
            container.Add(valueEl);

            if (!string.IsNullOrEmpty(card.TrendText))
            {
                var trendEl = new Label(card.TrendText);
                trendEl.AddToClassList("product-detail__card__trend");
                trendEl.AddToClassList("text-caption");
                container.Add(trendEl);
            }

            if (card.IsClickable)
            {
                string routeId = card.DrillDownRouteId;
                container.RegisterCallback<ClickEvent>(_ => OnMetricCardClicked?.Invoke(routeId));
            }

            return container;
        }

        // ─── Private — placeholder tab panels ────────────────────────────────────────

        /// <summary>
        /// Adds a single [Placeholder] label to each non-Overview tab panel.
        /// Phase 6+ will populate each panel with real content via dedicated tab builders.
        /// Only adds the label if the panel is empty to avoid duplication on re-bind.
        /// </summary>
        private void BuildPlaceholderTabPanels()
        {
            AddPlaceholderIfEmpty(_developmentPanel, "DevelopmentCardGrid",
                "[Placeholder] Development tab content — wired in Phase 6.");
            AddPlaceholderIfEmpty(_qualityPanel, "QualityCardGrid",
                "[Placeholder] Quality tab content — wired in Phase 6.");
            AddPlaceholderIfEmpty(_budgetPanel, "BudgetCardGrid",
                "[Placeholder] Budget tab content — wired in Phase 6.");
            AddPlaceholderIfEmpty(_marketingPanel, "MarketingCardGrid",
                "[Placeholder] Marketing tab content — wired in Phase 6.");
            AddPlaceholderIfEmpty(_supportPanel, "SupportCardGrid",
                "[Placeholder] Support tab content — wired in Phase 6.");
            AddPlaceholderIfEmpty(_reportsPanel, "ReportsList",
                "[Placeholder] Reports tab — links to Reports/Inbox wired in Phase 6.");
            AddPlaceholderIfEmpty(_competitorsPanel, "CompetitorsList",
                "[Placeholder] Competitors tab — comparison view wired in Phase 6.");
        }

        private static void AddPlaceholderIfEmpty(VisualElement panel, string childName, string message)
        {
            if (panel == null)
            {
                return;
            }

            VisualElement child = panel.Q<VisualElement>(childName);
            if (child == null || child.childCount > 0)
            {
                return;
            }

            var placeholder = new Label(message);
            placeholder.AddToClassList("text-body");
            placeholder.AddToClassList("text-muted");
            child.Add(placeholder);
        }

        // ─── Private — history builder ────────────────────────────────────────────────

        private void BuildHistoryList(ProductDetailScreenViewModel viewModel)
        {
            if (_historyList == null)
            {
                return;
            }

            _historyList.Clear();

            if (viewModel.HistoryRows == null || viewModel.HistoryRows.Count == 0)
            {
                var emptyLabel = new Label("[Placeholder] No history entries yet.");
                emptyLabel.AddToClassList("text-body");
                emptyLabel.AddToClassList("text-muted");
                _historyList.Add(emptyLabel);
                return;
            }

            foreach (ProductHistoryRowViewModel row in viewModel.HistoryRows)
            {
                VisualElement rowEl = CreateHistoryRow(row);
                _historyList.Add(rowEl);
            }
        }

        private static VisualElement CreateHistoryRow(ProductHistoryRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("product-detail__row");
            ApplySemanticStateClass(rowEl, row.SemanticState);

            var dateEl = new Label(row.Date);
            dateEl.AddToClassList("product-detail__row__date");
            dateEl.AddToClassList("text-caption");
            dateEl.AddToClassList("text-muted");

            var eventEl = new Label(row.Event);
            eventEl.AddToClassList("product-detail__row__event");
            eventEl.AddToClassList("text-body");

            var categoryEl = new Label(row.Category);
            categoryEl.AddToClassList("product-detail__row__category");
            categoryEl.AddToClassList("pill");

            rowEl.Add(dateEl);
            rowEl.Add(eventEl);
            rowEl.Add(categoryEl);

            return rowEl;
        }

        // ─── Private — risk panel ─────────────────────────────────────────────────────

        private void BindRiskPanel(ProductDetailScreenViewModel viewModel)
        {
            ProductRiskSummaryViewModel risk = viewModel.RiskSummary;

            if (risk == null)
            {
                SetVisible(_riskPanel, false);
                return;
            }

            SetVisible(_riskPanel, true);

            if (_riskOverallValue != null)
            {
                _riskOverallValue.text = risk.OverallScore;
            }

            BuildRiskFieldList(risk);

            // Apply semantic state class to risk panel.
            ApplySemanticStateClass(_riskPanel, risk.SemanticState);

            // Show or hide the high-risk warning.
            bool showWarning = viewModel.HasHighRisk;
            SetVisible(_highRiskWarning, showWarning);
        }

        private void BuildRiskFieldList(ProductRiskSummaryViewModel risk)
        {
            if (_riskFieldList == null)
            {
                return;
            }

            _riskFieldList.Clear();

            AddRiskRow("Quality",                  risk.Quality);
            AddRiskRow("Creativity",               risk.Creativity);
            AddRiskRow("Stability",                risk.Stability);
            AddRiskRow("Bug Risk",                 risk.BugRisk);
            AddRiskRow("QA Confidence",            risk.QaConfidence);
            AddRiskRow("Infrastructure Readiness", risk.InfrastructureReadiness);
            AddRiskRow("Support Readiness",        risk.SupportReadiness);
            AddRiskRow("Dev Budget",               risk.DevelopmentBudget);
            AddRiskRow("Pre-Launch Marketing",     risk.PreLaunchMarketingBudget);
            AddRiskRow("Post-Launch Marketing",    risk.PostLaunchMarketingBudget);
            AddRiskRow("Post-Launch Support",      risk.PostLaunchSupportBudget);
        }

        private void AddRiskRow(string label, string value)
        {
            if (_riskFieldList == null)
            {
                return;
            }

            var row = new VisualElement();
            row.AddToClassList("product-detail__risk-row");

            var labelEl = new Label(label);
            labelEl.AddToClassList("product-detail__risk-label");
            labelEl.AddToClassList("text-small");

            var valueEl = new Label(value ?? "—");
            valueEl.AddToClassList("product-detail__risk-value");
            valueEl.AddToClassList("text-body");

            row.Add(labelEl);
            row.Add(valueEl);
            _riskFieldList.Add(row);
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

        private void ApplyRootSemanticClasses(ProductDetailScreenViewModel viewModel)
        {
            // Remove all prior semantic state classes from root.
            Root.RemoveFromClassList("has-warning");
            Root.RemoveFromClassList("has-danger");
            Root.RemoveFromClassList("has-success");

            if (viewModel.HasHighRisk || viewModel.HasNoAssignedTeam)
            {
                Root.AddToClassList("has-warning");
            }

            string state = viewModel.SemanticState;
            if (!string.IsNullOrEmpty(state) && state != "normal")
            {
                Root.AddToClassList($"has-{state}");
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

        private static void SetButtonEnabled(Button button, bool enabled)
        {
            if (button == null)
            {
                return;
            }

            if (enabled)
            {
                button.RemoveFromClassList("is-disabled");
                button.SetEnabled(true);
            }
            else
            {
                button.AddToClassList("is-disabled");
                button.SetEnabled(false);
            }
        }

        private static void SetLabelText(Label label, string text)
        {
            if (label != null)
            {
                label.text = text ?? string.Empty;
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
                    $"ProductDetailView: element '{name}' not found in UXML root.");
            }
        }
    }
}
