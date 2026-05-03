using System.Collections.Generic;

namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>RecruitmentRuntimeState</c>.
    /// <c>GameDateTime</c> field serialized as total elapsed hours.
    /// </summary>
    public sealed class RecruitmentSaveData
    {
        public List<string> CandidateIds;
        public List<string> JobPostIds;

        /// <summary>Serialized <c>GameDateTime LastCandidatePoolRefreshDate</c> as total elapsed hours.</summary>
        public int LastCandidatePoolRefreshDateTotalElapsedHours;
    }
}
