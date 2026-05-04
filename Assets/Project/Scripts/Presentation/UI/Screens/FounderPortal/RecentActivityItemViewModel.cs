namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Pure display-data class for one row in the Founder Portal Recent Activity strip.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "info", "warning".
    /// </summary>
    public sealed class RecentActivityItemViewModel
    {
        /// <summary>Stable ID for this activity entry.</summary>
        public string Id { get; }

        /// <summary>Pre-formatted timestamp string, e.g. "Day 14, 09:00".</summary>
        public string Timestamp { get; }

        /// <summary>Human-readable description, e.g. "Hired John Smith — Senior Developer".</summary>
        public string Description { get; }

        /// <summary>Category label for grouping/display, e.g. "Hiring", "Product", "Finance".</summary>
        public string CategoryLabel { get; }

        /// <summary>Semantic state string: "normal", "info", or "warning".</summary>
        public string SemanticState { get; }

        /// <summary>Optional target screen route ID for drill-down navigation. Empty string if not clickable.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the row responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public RecentActivityItemViewModel(
            string id,
            string timestamp,
            string description,
            string categoryLabel,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Id               = id;
            Timestamp        = timestamp;
            Description      = description;
            CategoryLabel    = categoryLabel;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
