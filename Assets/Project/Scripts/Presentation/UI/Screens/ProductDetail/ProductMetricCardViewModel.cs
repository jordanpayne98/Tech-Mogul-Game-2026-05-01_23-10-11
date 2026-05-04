namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Pure display-data class for a single overview metric card on the Product Detail screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — static data only in Phase 5.
    /// </summary>
    public sealed class ProductMetricCardViewModel
    {
        /// <summary>Stable ID, e.g. "metric.current_phase".</summary>
        public string Id { get; }

        /// <summary>Display label, e.g. "Current Phase".</summary>
        public string Label { get; }

        /// <summary>Pre-formatted value string, e.g. "Beta".</summary>
        public string Value { get; }

        /// <summary>Pre-formatted trend text, e.g. "+2 weeks". Empty string if no trend.</summary>
        public string TrendText { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>Route ID for drill-down navigation. Empty string if not navigable.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the card responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public ProductMetricCardViewModel(
            string id,
            string label,
            string value,
            string trendText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Id               = id;
            Label            = label;
            Value            = value;
            TrendText        = trendText;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
