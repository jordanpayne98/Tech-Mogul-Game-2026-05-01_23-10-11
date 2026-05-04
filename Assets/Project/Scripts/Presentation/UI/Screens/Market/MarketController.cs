using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Market overview screen.
    /// Builds a [Placeholder] static ViewModel with market data and wires click callbacks.
    /// Category click opens the detail drawer or modal when wired in Phase 6+.
    /// Market Screen shows data, not recommendations (per Section 14 lock).
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketController
    {
        private readonly MarketView    _view;
        private readonly IScreenRouter _screenRouter;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public MarketController(MarketView view, IScreenRouter screenRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;

            _view.OnCategoryClicked   += HandleCategoryClicked;
            _view.OnTrendClicked      += HandleTrendClicked;
            _view.OnRankingRowClicked += HandleRankingRowClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            MarketViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleCategoryClicked(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"MarketController: category clicked — '{categoryId}'. " +
                "[Placeholder] Category detail drawer will be wired in Phase 6+.");

            // [Placeholder] Category click opens detail drawer/modal in Phase 6+.
            // DrawerRouter.OpenDrawer(DrawerIds.MarketDetail, context: categoryId);
        }

        private void HandleTrendClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"MarketController: trend clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandleRankingRowClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"MarketController: ranking row clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] MarketViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// Includes 3 trend items, 6 market categories, and 10 ranking rows (1 player product highlighted).
        /// </summary>
        private static MarketViewModel BuildPlaceholderViewModel()
        {
            return new MarketViewModel(
                screenTitle:       "Market",
                screenSubtitle:    "Market Overview [Placeholder]",
                isLoading:         false,
                hasError:          false,
                errorMessage:      string.Empty,
                emptyStateTitle:   "No Market Data",
                emptyStateBody:    "Market data will become available once your company is active.",
                searchText:        string.Empty,
                trends:            BuildPlaceholderTrends(),
                categories:        BuildPlaceholderCategories(),
                rankings:          BuildPlaceholderRankings(),
                selectedCategoryId: string.Empty,
                hasNoMarketData:   false,
                hasPlayerPosition: true);
        }

        private static IReadOnlyList<MarketTrendViewModel> BuildPlaceholderTrends()
        {
            return new List<MarketTrendViewModel>
            {
                new MarketTrendViewModel(
                    id:               "trend.ai_adoption_wave",
                    title:            "AI Adoption Accelerating",
                    summary:          "[Placeholder] Enterprise AI tooling demand up 32% this quarter.",
                    semanticState:    "positive",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketTrendViewModel(
                    id:               "trend.supply_chain_risk",
                    title:            "Supply Chain Pressure",
                    summary:          "[Placeholder] Hardware component lead times extended by 6–8 weeks.",
                    semanticState:    "negative",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketTrendViewModel(
                    id:               "trend.saas_consolidation",
                    title:            "SaaS Market Consolidating",
                    summary:          "[Placeholder] Mid-tier vendors being acquired by enterprise players.",
                    semanticState:    "neutral",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),
            };
        }

        private static IReadOnlyList<MarketCategoryRowViewModel> BuildPlaceholderCategories()
        {
            return new List<MarketCategoryRowViewModel>
            {
                new MarketCategoryRowViewModel(
                    id:                     "category.productivity",
                    categoryName:           "Productivity",
                    demand:                 "High",
                    growthRate:             "+18% YoY",
                    competitiveIntensity:   "Intense",
                    currentLeaders:         "Apex Corp, Nova Systems",
                    technologyExpectations: "High — cloud-native required",
                    priceSensitivity:       "Medium",
                    supportExpectations:    "Standard business hours",
                    trendModifiers:         "AI adoption boost",
                    semanticState:          "high-growth",
                    isClickable:            true),

                new MarketCategoryRowViewModel(
                    id:                     "category.enterprise_saas",
                    categoryName:           "Enterprise SaaS",
                    demand:                 "Very High",
                    growthRate:             "+24% YoY",
                    competitiveIntensity:   "Intense",
                    currentLeaders:         "Apex Corp, Vector Dynamics",
                    technologyExpectations: "High — cutting-edge required",
                    priceSensitivity:       "Low",
                    supportExpectations:    "24/7 enterprise SLAs expected",
                    trendModifiers:         "AI adoption boost, SaaS consolidation",
                    semanticState:          "high-growth",
                    isClickable:            true),

                new MarketCategoryRowViewModel(
                    id:                     "category.gaming",
                    categoryName:           "Gaming",
                    demand:                 "Medium",
                    growthRate:             "+9% YoY",
                    competitiveIntensity:   "Moderate",
                    currentLeaders:         "Horizon Labs, Pixel Studios",
                    technologyExpectations: "Medium — performance focus",
                    priceSensitivity:       "High",
                    supportExpectations:    "Community-driven support",
                    trendModifiers:         "No major modifiers",
                    semanticState:          "default",
                    isClickable:            true),

                new MarketCategoryRowViewModel(
                    id:                     "category.hardware_peripherals",
                    categoryName:           "Hardware Peripherals",
                    demand:                 "Medium",
                    growthRate:             "+5% YoY",
                    competitiveIntensity:   "Low",
                    currentLeaders:         "Nova Systems, Bright Edge",
                    technologyExpectations: "Low — reliability focus",
                    priceSensitivity:       "High",
                    supportExpectations:    "Standard warranty support",
                    trendModifiers:         "Supply chain risk",
                    semanticState:          "default",
                    isClickable:            true),

                new MarketCategoryRowViewModel(
                    id:                     "category.developer_tools",
                    categoryName:           "Developer Tools",
                    demand:                 "High",
                    growthRate:             "+21% YoY",
                    competitiveIntensity:   "Moderate",
                    currentLeaders:         "Apex Corp, CodeStream",
                    technologyExpectations: "Very High — developer experience critical",
                    priceSensitivity:       "Low",
                    supportExpectations:    "Documentation and community",
                    trendModifiers:         "AI adoption boost",
                    semanticState:          "player-present",
                    isClickable:            true),

                new MarketCategoryRowViewModel(
                    id:                     "category.cloud",
                    categoryName:           "Cloud Infrastructure",
                    demand:                 "Very High",
                    growthRate:             "+30% YoY",
                    competitiveIntensity:   "Intense",
                    currentLeaders:         "Vector Dynamics, Apex Corp",
                    technologyExpectations: "High — reliability and uptime critical",
                    priceSensitivity:       "Medium",
                    supportExpectations:    "24/7 enterprise SLAs expected",
                    trendModifiers:         "AI adoption boost, SaaS consolidation",
                    semanticState:          "high-growth",
                    isClickable:            true),
            };
        }

        private static IReadOnlyList<MarketRankingRowViewModel> BuildPlaceholderRankings()
        {
            return new List<MarketRankingRowViewModel>
            {
                new MarketRankingRowViewModel(
                    rank:             "#1",
                    productName:      "ApexFlow Pro",
                    companyName:      "Apex Corp",
                    score:            "34.2%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: ScreenIds.Competitors,
                    isClickable:      true),

                new MarketRankingRowViewModel(
                    rank:             "#2",
                    productName:      "VectorSuite",
                    companyName:      "Vector Dynamics",
                    score:            "22.8%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: ScreenIds.Competitors,
                    isClickable:      true),

                new MarketRankingRowViewModel(
                    rank:             "#3",
                    productName:      "NovaDash",
                    companyName:      "Nova Systems",
                    score:            "14.5%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: ScreenIds.Competitors,
                    isClickable:      true),

                new MarketRankingRowViewModel(
                    rank:             "#4",
                    productName:      "[Placeholder] Your Product",
                    companyName:      "[Placeholder] Your Company",
                    score:            "8.1%",
                    semanticState:    "player-product",
                    isPlayerProduct:  true,
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),

                new MarketRankingRowViewModel(
                    rank:             "#5",
                    productName:      "HorizonPulse",
                    companyName:      "Horizon Labs",
                    score:            "7.3%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: ScreenIds.Competitors,
                    isClickable:      true),

                new MarketRankingRowViewModel(
                    rank:             "#6",
                    productName:      "BrightEdge Tools",
                    companyName:      "Bright Edge",
                    score:            "5.9%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketRankingRowViewModel(
                    rank:             "#7",
                    productName:      "CodeStream IDE",
                    companyName:      "CodeStream",
                    score:            "3.7%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketRankingRowViewModel(
                    rank:             "#8",
                    productName:      "Pixel Studio Pro",
                    companyName:      "Pixel Studios",
                    score:            "2.4%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketRankingRowViewModel(
                    rank:             "#9",
                    productName:      "StackWave Cloud",
                    companyName:      "StackWave",
                    score:            "0.9%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new MarketRankingRowViewModel(
                    rank:             "#10",
                    productName:      "GeneriSoft Basic",
                    companyName:      "GeneriSoft",
                    score:            "0.2%",
                    semanticState:    "competitor",
                    isPlayerProduct:  false,
                    drillDownRouteId: string.Empty,
                    isClickable:      false),
            };
        }
    }
}
