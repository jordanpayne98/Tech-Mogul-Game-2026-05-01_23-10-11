namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Display data for one team option in the Assign Team modal.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and passed to the Assign Team modal view.
    /// SemanticState drives USS state class toggling in the View.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class AssignTeamOptionViewModel
    {
        /// <summary>Stable team identifier used to submit the assignment request.</summary>
        public string TeamId { get; }

        /// <summary>Display name of the team, e.g. "Alpha Squad".</summary>
        public string TeamName { get; }

        /// <summary>Primary function or discipline of the team, e.g. "Backend Engineering".</summary>
        public string Function { get; }

        /// <summary>
        /// Skill fit summary label, e.g. "Good Match", "Partial Match", "Poor Match".
        /// Computed from team skills vs contract requirements.
        /// </summary>
        public string SkillFit { get; }

        /// <summary>Availability label, e.g. "Available", "Partially Available", "Occupied".</summary>
        public string Availability { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this team can be selected for assignment.</summary>
        public bool IsSelectable { get; }

        public AssignTeamOptionViewModel(
            string teamId,
            string teamName,
            string function,
            string skillFit,
            string availability,
            string semanticState,
            bool isSelectable)
        {
            TeamId        = teamId;
            TeamName      = teamName;
            Function      = function;
            SkillFit      = skillFit;
            Availability  = availability;
            SemanticState = semanticState;
            IsSelectable  = isSelectable;
        }
    }
}
