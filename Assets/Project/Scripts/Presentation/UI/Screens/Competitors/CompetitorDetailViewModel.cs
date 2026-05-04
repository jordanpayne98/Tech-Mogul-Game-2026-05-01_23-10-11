using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Display data for the competitor detail drawer/modal on the Competitors screen.
    /// Immutable after construction. No Unity dependencies.
    /// Populated when a competitor row is clicked and passed to the detail drawer view.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorDetailViewModel
    {
        /// <summary>Stable ID of the competitor being shown in the detail drawer.</summary>
        public string CompetitorId { get; }

        /// <summary>Display name of the competitor company, e.g. "Apex Systems".</summary>
        public string CompanyName { get; }

        /// <summary>
        /// Competitor archetype display label per GDD, e.g. "Incumbent Giant".
        /// Possible values: Incumbent Giant, Aggressive Startup, Research Lab,
        /// Hardware Manufacturer, Enterprise Specialist, Consumer Brand,
        /// Low-Cost Competitor, Platform Holder.
        /// </summary>
        public string Archetype { get; }

        /// <summary>Brief editorial summary of the competitor company for the detail header.</summary>
        public string CompanySummary { get; }

        /// <summary>Primary market focus description shown in the detail panel, e.g. "Enterprise Software".</summary>
        public string MarketFocus { get; }

        // ── Detail sections ──────────────────────────────────────────────────

        /// <summary>Ordered list of product rows in the competitor's portfolio.</summary>
        public IReadOnlyList<CompetitorProductRowViewModel> Products { get; }

        /// <summary>
        /// Display labels for recent product or market launch events, ordered most recent first.
        /// Each entry is a formatted display string, e.g. "Q3 2025 — DataCore Pro (Enterprise)".
        /// </summary>
        public IReadOnlyList<string> RecentLaunches { get; }

        /// <summary>
        /// Known competitive strength labels, e.g. "Strong enterprise sales network".
        /// Display-ready for bullet list rendering.
        /// </summary>
        public IReadOnlyList<string> KnownStrengths { get; }

        /// <summary>
        /// Known competitive risk labels, e.g. "High dependency on legacy revenue".
        /// Display-ready for bullet list rendering.
        /// </summary>
        public IReadOnlyList<string> KnownRisks { get; }

        /// <summary>
        /// Stable route IDs or display labels for related intelligence reports.
        /// Used to link from the detail drawer to report screens.
        /// </summary>
        public IReadOnlyList<string> RelatedReports { get; }

        // ── Visual state ─────────────────────────────────────────────────────

        /// <summary>
        /// Semantic visual state for this competitor, e.g. "neutral", "warning", "dominant", "unknown".
        /// Used by the View to apply the correct USS state class without hardcoding colours in C#.
        /// </summary>
        public string SemanticState { get; }

        public CompetitorDetailViewModel(
            string competitorId,
            string companyName,
            string archetype,
            string companySummary,
            string marketFocus,
            IReadOnlyList<CompetitorProductRowViewModel> products,
            IReadOnlyList<string> recentLaunches,
            IReadOnlyList<string> knownStrengths,
            IReadOnlyList<string> knownRisks,
            IReadOnlyList<string> relatedReports,
            string semanticState)
        {
            CompetitorId   = competitorId;
            CompanyName    = companyName;
            Archetype      = archetype;
            CompanySummary = companySummary;
            MarketFocus    = marketFocus;
            Products       = products;
            RecentLaunches = recentLaunches;
            KnownStrengths = knownStrengths;
            KnownRisks     = knownRisks;
            RelatedReports = relatedReports;
            SemanticState  = semanticState;
        }
    }
}
