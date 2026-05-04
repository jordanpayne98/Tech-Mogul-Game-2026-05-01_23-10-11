namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Compact display-data class for a team member shown inside the team detail drawer.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success", "muted".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamMemberMiniViewModel
    {
        /// <summary>Stable employee identifier. Used for drill-down navigation to the Employees screen.</summary>
        public string EmployeeId { get; }

        /// <summary>Display name of the employee, e.g. "Alex Mercer".</summary>
        public string Name { get; }

        /// <summary>Current role label within the team, e.g. "Lead Engineer", "QA Analyst".</summary>
        public string Role { get; }

        /// <summary>Pre-formatted morale value, e.g. "85%" or "Good".</summary>
        public string Morale { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when clicking this entry navigates to the employee detail view.</summary>
        public bool IsClickable { get; }

        public TeamMemberMiniViewModel(
            string employeeId,
            string name,
            string role,
            string morale,
            string semanticState,
            bool isClickable)
        {
            EmployeeId    = employeeId;
            Name          = name;
            Role          = role;
            Morale        = morale;
            SemanticState = semanticState;
            IsClickable   = isClickable;
        }
    }
}
