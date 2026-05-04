namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure display-data class for a single row in a revenue or expense breakdown panel.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// DrillDownRouteId is empty string when the row is not clickable.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FinanceBreakdownRowViewModel
    {
        /// <summary>Display label for this revenue or expense category, e.g. "Product Revenue".</summary>
        public string Category { get; }

        /// <summary>Pre-formatted amount string, e.g. "$42,000".</summary>
        public string Amount { get; }

        /// <summary>Pre-formatted percentage of total, e.g. "34%".</summary>
        public string Percentage { get; }

        /// <summary>Pre-formatted trend text, e.g. "+8% vs last month". Empty string if no trend.</summary>
        public string TrendText { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>Route ID for drill-down navigation. Empty string when not navigable.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the row responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public FinanceBreakdownRowViewModel(
            string category,
            string amount,
            string percentage,
            string trendText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Category        = category;
            Amount          = amount;
            Percentage      = percentage;
            TrendText       = trendText;
            SemanticState   = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable     = isClickable;
        }
    }
}
