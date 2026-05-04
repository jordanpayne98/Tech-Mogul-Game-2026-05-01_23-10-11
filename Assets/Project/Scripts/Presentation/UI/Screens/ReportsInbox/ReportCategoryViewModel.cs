namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Category rail IDs used in the Reports Inbox left panel.
    /// </summary>
    public static class ReportCategoryIds
    {
        public const string All              = "cat.all";
        public const string RequiresDecision = "cat.requires_decision";
        public const string Finance          = "cat.finance";
        public const string Products         = "cat.products";
        public const string Employees        = "cat.employees";
        public const string Teams            = "cat.teams";
        public const string Hiring           = "cat.hiring";
        public const string Market           = "cat.market";
        public const string Competitors      = "cat.competitors";
        public const string Infrastructure   = "cat.infrastructure";
        public const string Support          = "cat.support";
        public const string Contracts        = "cat.contracts";
        public const string Research         = "cat.research";
        public const string Archived         = "cat.archived";
    }

    /// <summary>
    /// Pure display-data class for one category entry in the Reports Inbox left rail.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — Phase 5 uses static data. Live counts wired in Phase 6+.
    /// </summary>
    public sealed class ReportCategoryViewModel
    {
        /// <summary>Stable category ID, e.g. "cat.finance".</summary>
        public string Id { get; }

        /// <summary>Display label for the category rail entry, e.g. "Finance".</summary>
        public string Label { get; }

        /// <summary>Pre-formatted total report count string, e.g. "12". Empty string if not shown.</summary>
        public string Count { get; }

        /// <summary>Pre-formatted unread report count string, e.g. "3". Empty string if none unread.</summary>
        public string UnreadCount { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        public ReportCategoryViewModel(
            string id,
            string label,
            string count,
            string unreadCount,
            string semanticState)
        {
            Id            = id;
            Label         = label;
            Count         = count;
            UnreadCount   = unreadCount;
            SemanticState = semanticState;
        }
    }
}
