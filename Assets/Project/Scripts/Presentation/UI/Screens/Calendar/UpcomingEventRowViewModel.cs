namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Display data for one row in the upcoming events list panel on the Calendar screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class UpcomingEventRowViewModel
    {
        /// <summary>Stable identifier for this event instance, e.g. "event.payroll.2027_06".</summary>
        public string Id { get; }

        /// <summary>Display title for the event, e.g. "Monthly Payroll Run".</summary>
        public string Title { get; }

        /// <summary>Formatted date display string, e.g. "15 Jun 2027".</summary>
        public string Date { get; }

        /// <summary>
        /// Event type category stable ID.
        /// Examples: "product.target_release", "contract.deadline", "finance.monthly_report",
        /// "hiring.candidate_response", "research.completion", "product.report",
        /// "finance.payroll", "market.competitor_report",
        /// "infrastructure.support_warning", "product.launch_day".
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Priority level for display ordering and visual treatment.
        /// Examples: "critical", "high", "normal", "low".
        /// </summary>
        public string Priority { get; }

        /// <summary>True when this event requires a player decision before or on the event date.</summary>
        public bool RequiresDecision { get; }

        /// <summary>
        /// Semantic display state for visual styling.
        /// Examples: "default", "urgent", "decided", "missed", "completed".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when clicking this row navigates to a detail view or decision screen.</summary>
        public bool IsClickable { get; }

        /// <summary>Screen or modal route target to navigate to when the row is clicked, or empty string if not routable.</summary>
        public string RouteTarget { get; }

        public UpcomingEventRowViewModel(
            string id,
            string title,
            string date,
            string category,
            string priority,
            bool requiresDecision,
            string semanticState,
            bool isClickable,
            string routeTarget)
        {
            Id               = id;
            Title            = title;
            Date             = date;
            Category         = category;
            Priority         = priority;
            RequiresDecision = requiresDecision;
            SemanticState    = semanticState;
            IsClickable      = isClickable;
            RouteTarget      = routeTarget;
        }
    }
}
