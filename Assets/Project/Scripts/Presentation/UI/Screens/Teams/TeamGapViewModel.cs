namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Pure display-data class for a single role gap entry within the team detail view.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success", "muted".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamGapViewModel
    {
        /// <summary>Human-readable role name that is missing or understaffed, e.g. "Senior Engineer".</summary>
        public string RoleName { get; }

        /// <summary>Pre-formatted severity label, e.g. "Critical", "Moderate", "Low".</summary>
        public string SeverityText { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        public TeamGapViewModel(
            string roleName,
            string severityText,
            string semanticState)
        {
            RoleName      = roleName;
            SeverityText  = severityText;
            SemanticState = semanticState;
        }
    }
}
