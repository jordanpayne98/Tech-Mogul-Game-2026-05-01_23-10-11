namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Display data for one event shown in a calendar day cell or event detail view.
    /// Immutable after construction. No Unity dependencies.
    ///
    /// Supported event categories:
    ///   product.target_release, contract.deadline, finance.monthly_report,
    ///   hiring.candidate_response, research.completion, product.report,
    ///   finance.payroll, market.competitor_report,
    ///   infrastructure.support_warning, product.launch_day.
    ///
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarEventViewModel
    {
        /// <summary>Stable identifier for this event instance, e.g. "event.payroll.2027_06".</summary>
        public string Id { get; }

        /// <summary>Display title for the event, e.g. "Monthly Payroll Run".</summary>
        public string Title { get; }

        /// <summary>Formatted date/time display string, e.g. "15 Jun 2027".</summary>
        public string DateTime { get; }

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

        /// <summary>Stable ID of the related entity (product, contract, employee, etc.), or empty string if none.</summary>
        public string RelatedEntityId { get; }

        /// <summary>True when this event requires a player decision before or on the event date.</summary>
        public bool RequiresDecision { get; }

        /// <summary>Short summary text describing the event context, shown in the detail panel.</summary>
        public string Summary { get; }

        /// <summary>Screen or modal route target to navigate to when the event is clicked, or empty string if not routable.</summary>
        public string RouteTarget { get; }

        /// <summary>
        /// Semantic display state for visual styling.
        /// Examples: "default", "urgent", "decided", "missed", "completed".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when clicking this event navigates to a detail view or decision screen.</summary>
        public bool IsClickable { get; }

        public CalendarEventViewModel(
            string id,
            string title,
            string dateTime,
            string category,
            string priority,
            string relatedEntityId,
            bool requiresDecision,
            string summary,
            string routeTarget,
            string semanticState,
            bool isClickable)
        {
            Id              = id;
            Title           = title;
            DateTime        = dateTime;
            Category        = category;
            Priority        = priority;
            RelatedEntityId = relatedEntityId;
            RequiresDecision = requiresDecision;
            Summary         = summary;
            RouteTarget     = routeTarget;
            SemanticState   = semanticState;
            IsClickable     = isClickable;
        }
    }
}
