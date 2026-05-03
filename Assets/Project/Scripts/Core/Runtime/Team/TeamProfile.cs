using Project.Core.Definitions.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Team
{
    /// <summary>
    /// Per-save entity that records the stable identity and metadata of a team.
    /// Created when a team is formed and persisted across save/load.
    /// Mutable fields are owned by TeamRuntimeState, not here.
    /// </summary>
    public sealed class TeamProfile
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Stable unique ID for this team. Used for all persistent references.</summary>
        public string Id { get; set; }

        /// <summary>Display name for the team. May change without breaking references.</summary>
        public string Name { get; set; }

        /// <summary>The primary function this team is organised to perform.</summary>
        public TeamType Type { get; set; }

        /// <summary>The in-game date on which this team was created.</summary>
        public GameDateTime CreatedDate { get; set; }
    }
}
