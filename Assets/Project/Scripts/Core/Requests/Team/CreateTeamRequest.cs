using System.Collections.Generic;
using Project.Core.Definitions.Team;

namespace Project.Core.Requests.Team
{
    /// <summary>
    /// Request to create a new team with a given name, type, and initial member list.
    /// Consumed by the team creation use case. No logic is executed inside this class.
    /// </summary>
    public sealed class CreateTeamRequest
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Display name for the new team.</summary>
        public string Name { get; }

        /// <summary>The primary function this team is organised to perform.</summary>
        public TeamType Type { get; }

        /// <summary>Stable IDs of the initial team members to assign on creation.</summary>
        public List<string> MemberIds { get; }

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new CreateTeamRequest.
        /// </summary>
        /// <param name="name">Display name for the team.</param>
        /// <param name="type">Primary function of the team.</param>
        /// <param name="memberIds">Stable IDs of initial members.</param>
        public CreateTeamRequest(string name, TeamType type, List<string> memberIds)
        {
            Name      = name;
            Type      = type;
            MemberIds = memberIds;
        }
    }
}
