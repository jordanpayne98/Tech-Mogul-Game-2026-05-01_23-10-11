namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Action button IDs used in the Reports Inbox detail panel.
    /// </summary>
    public static class ReportActionIds
    {
        public const string Archive      = "action.archive";
        public const string Delete       = "action.delete";
        public const string Pin          = "action.pin";
        public const string MarkUnread   = "action.mark_unread";
        public const string JumpToEntity = "action.jump_to_entity";
        public const string CreateTask   = "action.create_task";
    }

    /// <summary>
    /// Pure display-data class for one action button in the Reports Inbox detail panel.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — archive/delete update placeholder UI state only in Phase 5.
    /// </summary>
    public sealed class ReportActionViewModel
    {
        /// <summary>Stable action ID, e.g. "action.archive".</summary>
        public string Id { get; }

        /// <summary>Display label for the action button, e.g. "Archive".</summary>
        public string Label { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>True when this action is available and the button responds to interaction.</summary>
        public bool IsEnabled { get; }

        public ReportActionViewModel(
            string id,
            string label,
            string semanticState,
            bool isEnabled)
        {
            Id            = id;
            Label         = label;
            SemanticState = semanticState;
            IsEnabled     = isEnabled;
        }
    }
}
