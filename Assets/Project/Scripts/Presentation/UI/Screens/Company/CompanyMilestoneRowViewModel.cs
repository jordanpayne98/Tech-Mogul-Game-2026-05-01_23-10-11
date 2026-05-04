namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure display-data class for one row in the milestones/history list on the Company screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyMilestoneRowViewModel
    {
        /// <summary>Stable identifier for this milestone row, e.g. "milestone.first_product_shipped".</summary>
        public string Id { get; }

        /// <summary>Short display title for the milestone, e.g. "First Product Shipped".</summary>
        public string Title { get; }

        /// <summary>Human-readable date string for when the milestone occurred, e.g. "Q1 2024".</summary>
        public string Date { get; }

        /// <summary>Optional longer description or context for the milestone.</summary>
        public string Description { get; }

        /// <summary>
        /// Semantic state string used by the View to apply USS modifier classes.
        /// Allowed values: "normal", "archived".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when the row can be clicked to navigate to a detail view.</summary>
        public bool IsClickable { get; }

        /// <summary>Route ID for the milestone detail screen. Only relevant when IsClickable is true.</summary>
        public string DrillDownRouteId { get; }

        public CompanyMilestoneRowViewModel(
            string id,
            string title,
            string date,
            string description,
            string semanticState,
            bool isClickable,
            string drillDownRouteId)
        {
            Id               = id;
            Title            = title;
            Date             = date;
            Description      = description;
            SemanticState    = semanticState;
            IsClickable      = isClickable;
            DrillDownRouteId = drillDownRouteId;
        }
    }
}
