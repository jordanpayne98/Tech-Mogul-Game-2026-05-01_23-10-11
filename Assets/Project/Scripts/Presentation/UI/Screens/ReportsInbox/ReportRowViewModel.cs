namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Pure display-data class for one report row in the Reports Inbox centre list panel.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — Phase 5 uses static data. Live report list wired in Phase 6+.
    /// </summary>
    public sealed class ReportRowViewModel
    {
        /// <summary>Stable report ID used for selection and navigation.</summary>
        public string Id { get; }

        /// <summary>Display title of the report, e.g. "Monthly Finance Summary — Month 3".</summary>
        public string Title { get; }

        /// <summary>Pre-formatted date string, e.g. "Month 3, Week 2".</summary>
        public string Date { get; }

        /// <summary>Category label shown on the row, e.g. "Finance".</summary>
        public string Category { get; }

        /// <summary>Short summary or teaser line for the report body.</summary>
        public string Summary { get; }

        /// <summary>True when the report has not been read by the player.</summary>
        public bool IsUnread { get; }

        /// <summary>True when this report requires a player decision before it can be resolved.</summary>
        public bool RequiresDecision { get; }

        /// <summary>True when this report has been moved to the archive.</summary>
        public bool IsArchived { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>True when the row responds to click/tap input to open the detail panel.</summary>
        public bool IsClickable { get; }

        public ReportRowViewModel(
            string id,
            string title,
            string date,
            string category,
            string summary,
            bool isUnread,
            bool requiresDecision,
            bool isArchived,
            string semanticState,
            bool isClickable)
        {
            Id               = id;
            Title            = title;
            Date             = date;
            Category         = category;
            Summary          = summary;
            IsUnread         = isUnread;
            RequiresDecision = requiresDecision;
            IsArchived       = isArchived;
            SemanticState    = semanticState;
            IsClickable      = isClickable;
        }
    }
}
