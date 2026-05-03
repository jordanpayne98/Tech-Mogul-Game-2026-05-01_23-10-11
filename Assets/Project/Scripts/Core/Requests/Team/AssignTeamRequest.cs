using Project.Core.Definitions.Team;

namespace Project.Core.Requests.Team
{
    /// <summary>
    /// Request to assign a team to a specific work target.
    /// Consumed by the team assignment use case. No logic is executed inside this class.
    /// </summary>
    public sealed class AssignTeamRequest
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Stable ID of the team to assign. References TeamProfile.Id.</summary>
        public string TeamId { get; }

        /// <summary>The category of work the team will perform.</summary>
        public AssignmentType Type { get; }

        /// <summary>The category of entity the assignment is targeting.</summary>
        public AssignmentTargetType TargetType { get; }

        /// <summary>Stable ID of the specific entity being targeted.</summary>
        public string TargetId { get; }

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new AssignTeamRequest.
        /// </summary>
        /// <param name="teamId">Stable ID of the team to assign.</param>
        /// <param name="type">The type of work to assign.</param>
        /// <param name="targetType">The category of the assignment target.</param>
        /// <param name="targetId">Stable ID of the assignment target entity.</param>
        public AssignTeamRequest(string teamId, AssignmentType type, AssignmentTargetType targetType, string targetId)
        {
            TeamId     = teamId;
            Type       = type;
            TargetType = targetType;
            TargetId   = targetId;
        }
    }
}
