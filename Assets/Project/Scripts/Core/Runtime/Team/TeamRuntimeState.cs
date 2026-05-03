using System.Collections.Generic;

namespace Project.Core.Runtime.Team
{
    /// <summary>
    /// Mutable runtime state for a team.
    /// Tracks live membership, morale, cohesion, workload, and assignment linkage.
    /// No LeadId — leadership is resolved by seniority at the use-case layer.
    /// </summary>
    public sealed class TeamRuntimeState
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Stable ID of the team this state belongs to. References TeamProfile.Id.</summary>
        public string TeamId { get; set; }

        /// <summary>Stable IDs of all current team members.</summary>
        public List<string> MemberIds { get; set; } = new List<string>();

        /// <summary>
        /// Team cohesion level. Integer percentage in the range 0–100.
        /// Reflects how well the team works together.
        /// </summary>
        public int Cohesion { get; set; }

        /// <summary>
        /// Team morale level. Integer percentage in the range 0–100.
        /// Reflects overall team motivation and wellbeing.
        /// </summary>
        public int Morale { get; set; }

        /// <summary>
        /// Current workload level. Integer percentage in the range 0–100.
        /// Reflects how much capacity the team is currently using.
        /// </summary>
        public int Workload { get; set; }

        /// <summary>
        /// Stable ID of the team's current assignment, or null if unassigned.
        /// References AssignmentRuntimeState.Id.
        /// </summary>
        public string CurrentAssignmentId { get; set; }

        /// <summary>Ordered history of stable assignment IDs this team has completed or left.</summary>
        public List<string> AssignmentHistoryIds { get; set; } = new List<string>();
    }
}
