using System.Collections.Generic;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Mutable runtime state for the recruitment system as a whole.
    /// Tracks the active candidate pool and open job posts for the current save.
    /// </summary>
    public sealed class RecruitmentRuntimeState
    {
        /// <summary>
        /// Stable IDs of all candidates currently in the recruitment pool.
        /// Each ID maps to a <see cref="CandidateProfile"/> and <see cref="CandidateRuntimeState"/>.
        /// </summary>
        public List<string> CandidateIds;

        /// <summary>
        /// Stable IDs of all active job posts.
        /// Each ID maps to a <see cref="JobPostProfile"/> and <see cref="JobPostRuntimeState"/>.
        /// </summary>
        public List<string> JobPostIds;

        /// <summary>The in-game date the candidate pool was last refreshed.</summary>
        public GameDateTime LastCandidatePoolRefreshDate;
    }
}
