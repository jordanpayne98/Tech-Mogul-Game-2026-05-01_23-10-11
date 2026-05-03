using Project.Core.Definitions.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Team
{
    /// <summary>
    /// Mutable runtime state for a single team assignment.
    /// Tracks what the team is working on, who is assigned, and how far along the work is.
    /// EstimatedCompletionDate is nullable — it may be unknown at assignment creation time.
    /// </summary>
    public sealed class AssignmentRuntimeState
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Stable unique ID for this assignment. Used for all persistent references.</summary>
        public string Id { get; set; }

        /// <summary>The category of work being performed.</summary>
        public AssignmentType Type { get; set; }

        /// <summary>The category of entity this assignment is targeting.</summary>
        public AssignmentTargetType TargetType { get; set; }

        /// <summary>Stable ID of the specific entity being targeted (e.g. a product ID or contract ID).</summary>
        public string TargetId { get; set; }

        /// <summary>Stable ID of the team executing this assignment. References TeamProfile.Id.</summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Current progress of this assignment. Integer percentage in the range 0–100.
        /// 100 means the assignment is complete.
        /// Written by domain plans (2E–2G). Set to 0 at assignment creation.
        /// </summary>
        public int ProgressPercent { get; set; }

        /// <summary>
        /// Accumulated raw progress points from team work ticks.
        /// Additive only — never decreases. Starts at 0 when the assignment is created.
        /// Domain plans (2E, 2F, 2G, 2K) define the total required points and derive ProgressPercent.
        /// </summary>
        public int RawProgressPoints { get; set; }

        /// <summary>The in-game date on which this assignment started.</summary>
        public GameDateTime StartDate { get; set; }

        /// <summary>
        /// The projected in-game date on which this assignment will complete.
        /// Null if the completion date is not yet estimated.
        /// </summary>
        public GameDateTime EstimatedCompletionDate { get; set; }
    }
}
