using System;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Market screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates trend cards, category rows, and ranking rows.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// Market Screen shows data, not recommendations (per Section 14 lock).
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from MarketScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a category row is clicked. Argument is the category's stable ID.</summary>
        public event Action<string> OnCategoryClicked;

        /// <summary>Fired when a trend card is clicked. Argument is the trend's DrillDownRouteId.</summary>
        public event Action<string> OnTrendClicked;

        /// <summary>Fired when a ranking row is clicked. Argument is the row's DrillDownRouteId.</summary>
        public event Action<string> OnRankingRowClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Dynamic content containers ──────────────────────────────────────────────

        private readonly VisualElement _trendStrip;
        private readonly VisualElement _categoryGrid;
        private readonly VisualElement _rankingsList;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public MarketView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "MarketView: root VisualElement is null. View cannot be initialized.");

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

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Dynamic containers ───────────────────────────────────────────────────

            _trendStrip   = QueryElement(root, "TrendStrip");
            _categoryGrid = QueryElement(root, "CategoryGrid");
            _rankingsList = QueryElement(root, "RankingsList");

            // ── Text labels ──────────────────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic card/row lists on each call.
        /// </summary>
        public void Bind(MarketViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "MarketView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No market data available.");
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

            bool hasContent = (viewModel.Categories != null && viewModel.Categories.Count > 0)
                           || (viewModel.Rankings   != null && viewModel.Rankings.Count   > 0);

            if (!hasContent || viewModel.HasNoMarketData)
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

            // Player-position state class on root
            ApplyPlayerPositionClass(viewModel.HasPlayerPosition);

            // Trend strip
            BuildTrendStrip(viewModel);

            // Category grid
            BuildCategoryGrid(viewModel);

            // Rankings list
            BuildRankingsList(viewModel);
        }

        // ─── Private — section builders ──────────────────────────────────────────────

        private void BuildTrendStrip(MarketViewModel viewModel)
        {
            if (_trendStrip == null)
            {
                return;
            }

            _trendStrip.Clear();

            if (viewModel.Trends == null)
            {
                return;
            }

            foreach (MarketTrendViewModel trend in viewModel.Trends)
            {
                VisualElement cardEl = CreateTrendCard(trend);
                _trendStrip.Add(cardEl);
            }
        }

        private VisualElement CreateTrendCard(MarketTrendViewModel trend)
        {
            var container = new VisualElement();
            container.AddToClassList("market-screen__trend-card");
            ApplySemanticStateClass(container, trend.SemanticState);

            if (trend.IsClickable)
            {
                container.AddToClassList("is-clickable");
            }

            var titleEl = new Label(trend.Title);
            titleEl.AddToClassList("market-screen__trend-card__title");
            titleEl.AddToClassList("text-subheading");

            var summaryEl = new Label(trend.Summary);
            summaryEl.AddToClassList("market-screen__trend-card__summary");
            summaryEl.AddToClassList("text-caption");

            container.Add(titleEl);
            container.Add(summaryEl);

            if (trend.IsClickable)
            {
                string routeId = trend.DrillDownRouteId;
                container.RegisterCallback<ClickEvent>(_ => OnTrendClicked?.Invoke(routeId));
            }

            return container;
        }

        private void BuildCategoryGrid(MarketViewModel viewModel)
        {
            if (_categoryGrid == null)
            {
                return;
            }

            _categoryGrid.Clear();

            if (viewModel.Categories == null)
            {
                return;
            }

            foreach (MarketCategoryRowViewModel category in viewModel.Categories)
            {
                VisualElement rowEl = CreateCategoryRow(category);
                _categoryGrid.Add(rowEl);
            }
        }

        private VisualElement CreateCategoryRow(MarketCategoryRowViewModel category)
        {
            var row = new VisualElement();
            row.AddToClassList("market-screen__category-row");
            ApplySemanticStateClass(row, category.SemanticState);

            if (category.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            // Category name cell
            var nameEl = new Label(category.CategoryName);
            nameEl.AddToClassList("market-screen__category-row__name");
            nameEl.AddToClassList("text-body");

            // Demand cell
            var demandEl = new Label(category.Demand);
            demandEl.AddToClassList("market-screen__category-row__demand");
            demandEl.AddToClassList("text-caption");

            // Growth rate cell
            var growthEl = new Label(category.GrowthRate);
            growthEl.AddToClassList("market-screen__category-row__growth");
            growthEl.AddToClassList("text-caption");

            // Competitive intensity cell
            var intensityEl = new Label(category.CompetitiveIntensity);
            intensityEl.AddToClassList("market-screen__category-row__intensity");
            intensityEl.AddToClassList("text-caption");

            // Current leaders cell
            var leadersEl = new Label(category.CurrentLeaders);
            leadersEl.AddToClassList("market-screen__category-row__leaders");
            leadersEl.AddToClassList("text-caption");
            leadersEl.AddToClassList("text-muted");

            row.Add(nameEl);
            row.Add(demandEl);
            row.Add(growthEl);
            row.Add(intensityEl);
            row.Add(leadersEl);

            if (category.IsClickable)
            {
                string categoryId = category.Id;
                row.RegisterCallback<ClickEvent>(_ => OnCategoryClicked?.Invoke(categoryId));
            }

            return row;
        }

        private void BuildRankingsList(MarketViewModel viewModel)
        {
            if (_rankingsList == null)
            {
                return;
            }

            _rankingsList.Clear();

            if (viewModel.Rankings == null)
            {
                return;
            }

            foreach (MarketRankingRowViewModel ranking in viewModel.Rankings)
            {
                VisualElement rowEl = CreateRankingRow(ranking);
                _rankingsList.Add(rowEl);
            }
        }

        private VisualElement CreateRankingRow(MarketRankingRowViewModel ranking)
        {
            var row = new VisualElement();
            row.AddToClassList("market-screen__ranking-row");
            ApplySemanticStateClass(row, ranking.SemanticState);

            if (ranking.IsPlayerProduct)
            {
                // Player product rows are visually distinguished via USS.
                row.AddToClassList("is-player-product");
            }

            if (ranking.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            // Rank cell
            var rankEl = new Label(ranking.Rank);
            rankEl.AddToClassList("market-screen__ranking-row__rank");
            rankEl.AddToClassList("text-caption");

            // Product name cell
            var productEl = new Label(ranking.ProductName);
            productEl.AddToClassList("market-screen__ranking-row__product");
            productEl.AddToClassList("text-body");

            // Company name cell
            var companyEl = new Label(ranking.CompanyName);
            companyEl.AddToClassList("market-screen__ranking-row__company");
            companyEl.AddToClassList("text-caption");
            companyEl.AddToClassList("text-muted");

            // Score / market share cell
            var scoreEl = new Label(ranking.Score);
            scoreEl.AddToClassList("market-screen__ranking-row__score");
            scoreEl.AddToClassList("text-caption");

            row.Add(rankEl);
            row.Add(productEl);
            row.Add(companyEl);
            row.Add(scoreEl);

            if (ranking.IsClickable)
            {
                string routeId = ranking.DrillDownRouteId;
                row.RegisterCallback<ClickEvent>(_ => OnRankingRowClicked?.Invoke(routeId));
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

        private void ApplyPlayerPositionClass(bool hasPlayerPosition)
        {
            if (hasPlayerPosition)
            {
                Root.AddToClassList("has-player-position");
            }
            else
            {
                Root.RemoveFromClassList("has-player-position");
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
            if (string.IsNullOrEmpty(semanticState) || semanticState == "default")
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
                    $"MarketView: element '{name}' not found in UXML root.");
            }
        }
    }
}
