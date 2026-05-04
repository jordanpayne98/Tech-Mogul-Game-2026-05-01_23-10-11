using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Competitors screen.
    /// Builds a [Placeholder] static ViewModel with 5-8 competitors across all GDD archetypes,
    /// wires click callbacks to ScreenRouter/DrawerRouter, and calls View.Bind() to display data.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorsController
    {
        private readonly CompetitorsView _view;
        private readonly IScreenRouter   _screenRouter;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public CompetitorsController(
            CompetitorsView view,
            IScreenRouter   screenRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;

            _view.OnCompetitorRowClicked += HandleCompetitorRowClicked;
            _view.OnFiltersButtonClicked += HandleFiltersButtonClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            CompetitorsViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                "CompetitorsController: initialized with [Placeholder] static competitor data.");
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleCompetitorRowClicked(string competitorId)
        {
            if (string.IsNullOrEmpty(competitorId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"CompetitorsController: competitor row clicked — ID '{competitorId}'. " +
                "[Placeholder] Detail drawer/modal wiring deferred to Phase 6+.");

            // Phase 5: log only — detail drawer wiring is a Phase 6+ concern.
            // When Phase 6 wires the DrawerRouter, this handler will open the competitor detail drawer.
        }

        private void HandleFiltersButtonClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "CompetitorsController: Filters button clicked. " +
                "[Placeholder] Filter drawer wiring deferred to Phase 6+.");

            // Phase 5: log only — filter drawer wiring is a Phase 6+ concern.
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] CompetitorsViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// Covers all 8 GDD archetypes across 7 competitor entries.
        /// </summary>
        private static CompetitorsViewModel BuildPlaceholderViewModel()
        {
            return new CompetitorsViewModel(
                screenTitle:          "Competitors",
                screenSubtitle:       "Track rival companies, archetypes, and market positions. [Placeholder]",
                isLoading:            false,
                hasError:             false,
                errorMessage:         string.Empty,
                emptyStateTitle:      "No Competitors Tracked",
                emptyStateBody:       "Competitor intelligence will appear here as the simulation progresses.",
                searchText:           string.Empty,
                rows:                 BuildPlaceholderRows(),
                selectedCompetitorId: string.Empty);
        }

        /// <summary>
        /// Builds 7 placeholder competitor rows covering all GDD archetypes.
        /// [Placeholder] — replace with simulation data in Phase 6+.
        /// </summary>
        private static IReadOnlyList<CompetitorRowViewModel> BuildPlaceholderRows()
        {
            return new List<CompetitorRowViewModel>
            {
                // ── Incumbent Giant ───────────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.apex_systems",
                    companyName:    "Apex Systems",
                    archetype:      "Incumbent Giant",
                    mainMarket:     "Enterprise Software",
                    reputation:     "94 / 100",
                    productCount:   "24 products",
                    recentLaunch:   "Q2 2025 — DataCore Pro",
                    marketPosition: "Market Leader",
                    trend:          "Stable",
                    semanticState:  "dominant",
                    isClickable:    true),

                // ── Aggressive Startup ────────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.nova_labs",
                    companyName:    "Nova Labs",
                    archetype:      "Aggressive Startup",
                    mainMarket:     "Cloud Infrastructure",
                    reputation:     "72 / 100",
                    productCount:   "4 products",
                    recentLaunch:   "Q3 2025 — CloudSprint v2",
                    marketPosition: "Challenger",
                    trend:          "Growing",
                    semanticState:  "warning",
                    isClickable:    true),

                // ── Research Lab ──────────────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.meridian_research",
                    companyName:    "Meridian Research",
                    archetype:      "Research Lab",
                    mainMarket:     "Research & Development",
                    reputation:     "88 / 100",
                    productCount:   "7 products",
                    recentLaunch:   "Q1 2025 — Meridian SDK 4",
                    marketPosition: "Niche Leader",
                    trend:          "Stable",
                    semanticState:  "neutral",
                    isClickable:    true),

                // ── Hardware Manufacturer ─────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.ironforge_tech",
                    companyName:    "Ironforge Tech",
                    archetype:      "Hardware Manufacturer",
                    mainMarket:     "Hardware / Devices",
                    reputation:     "81 / 100",
                    productCount:   "11 products",
                    recentLaunch:   "Q4 2024 — Forge Workstation X",
                    marketPosition: "Established",
                    trend:          "Stable",
                    semanticState:  "neutral",
                    isClickable:    true),

                // ── Enterprise Specialist ─────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.vertex_solutions",
                    companyName:    "Vertex Solutions",
                    archetype:      "Enterprise Specialist",
                    mainMarket:     "Enterprise Software",
                    reputation:     "79 / 100",
                    productCount:   "9 products",
                    recentLaunch:   "Q2 2025 — Vertex ERP Suite",
                    marketPosition: "Challenger",
                    trend:          "Growing",
                    semanticState:  "neutral",
                    isClickable:    true),

                // ── Consumer Brand ────────────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.lumina_apps",
                    companyName:    "Lumina Apps",
                    archetype:      "Consumer Brand",
                    mainMarket:     "Consumer Applications",
                    reputation:     "76 / 100",
                    productCount:   "6 products",
                    recentLaunch:   "Q3 2025 — Lumina Social 3",
                    marketPosition: "Niche Player",
                    trend:          "Declining",
                    semanticState:  "neutral",
                    isClickable:    true),

                // ── Low-Cost Competitor ───────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.fasttrack_software",
                    companyName:    "FastTrack Software",
                    archetype:      "Low-Cost Competitor",
                    mainMarket:     "Consumer Applications",
                    reputation:     "54 / 100",
                    productCount:   "18 products",
                    recentLaunch:   "Q3 2025 — QuickForm Lite",
                    marketPosition: "Volume Player",
                    trend:          "Stable",
                    semanticState:  "neutral",
                    isClickable:    true),

                // ── Platform Holder ───────────────────────────────────────────────────

                new CompetitorRowViewModel(
                    id:             "competitor.nexus_platform",
                    companyName:    "Nexus Platform",
                    archetype:      "Platform Holder",
                    mainMarket:     "Platform / Ecosystem",
                    reputation:     "91 / 100",
                    productCount:   "31 products",
                    recentLaunch:   "Q1 2025 — Nexus OS 6",
                    marketPosition: "Ecosystem Leader",
                    trend:          "Growing",
                    semanticState:  "dominant",
                    isClickable:    true),
            };
        }
    }
}
