using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Pure display-data class for the team detail drawer/modal.
    /// Immutable after construction. No Unity dependencies.
    /// Created by TeamsController and passed to the team detail view.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state class toggling in the View.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamDetailViewModel
    {
        // ── Core identity ────────────────────────────────────────────────────

        /// <summary>Stable team identifier. Matches TeamsViewModel.SelectedTeamId and TeamRowViewModel.Id.</summary>
        public string TeamId { get; }

        /// <summary>Display name of the team, e.g. "Core Software".</summary>
        public string TeamName { get; }

        /// <summary>Function label, e.g. "Core Software", "QA &amp; Reliability", "Marketing".</summary>
        public string Function { get; }

        /// <summary>Display name of the team lead. Empty string if no lead is assigned.</summary>
        public string Lead { get; }

        // ── Assignment and workload ──────────────────────────────────────────

        /// <summary>Pre-formatted workload display string, e.g. "High" or "75%".</summary>
        public string Workload { get; }

        /// <summary>Display name of the current assignment. Empty string if the team is unassigned.</summary>
        public string CurrentAssignment { get; }

        /// <summary>Pre-formatted burnout risk label, e.g. "Low", "Moderate", "High".</summary>
        public string BurnoutRisk { get; }

        // ── Member and skill data ────────────────────────────────────────────

        /// <summary>Ordered list of compact member view models shown in the member roster section.</summary>
        public IReadOnlyList<TeamMemberMiniViewModel> Members { get; }

        /// <summary>
        /// Ordered list of pre-formatted skill coverage labels, e.g. "Engineering — High", "QA — Moderate".
        /// </summary>
        public IReadOnlyList<string> SkillCoverage { get; }

        /// <summary>
        /// Ordered list of pre-formatted assignment history entries, e.g. "Project Alpha — Completed Q2 Y1".
        /// </summary>
        public IReadOnlyList<string> AssignmentHistory { get; }

        /// <summary>Ordered list of role gap entries showing unfilled or understaffed roles.</summary>
        public IReadOnlyList<TeamGapViewModel> RoleGaps { get; }

        // ── Visual state ─────────────────────────────────────────────────────

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        public TeamDetailViewModel(
            string teamId,
            string teamName,
            string function,
            string lead,
            string workload,
            string currentAssignment,
            string burnoutRisk,
            IReadOnlyList<TeamMemberMiniViewModel> members,
            IReadOnlyList<string> skillCoverage,
            IReadOnlyList<string> assignmentHistory,
            IReadOnlyList<TeamGapViewModel> roleGaps,
            string semanticState)
        {
            TeamId            = teamId;
            TeamName          = teamName;
            Function          = function;
            Lead              = lead;
            Workload          = workload;
            CurrentAssignment = currentAssignment;
            BurnoutRisk       = burnoutRisk;
            Members           = members;
            SkillCoverage     = skillCoverage;
            AssignmentHistory = assignmentHistory;
            RoleGaps          = roleGaps;
            SemanticState     = semanticState;
        }
    }
}
