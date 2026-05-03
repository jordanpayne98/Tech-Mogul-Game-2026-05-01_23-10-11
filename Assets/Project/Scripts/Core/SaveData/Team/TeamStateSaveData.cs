using System.Collections.Generic;

namespace Project.Core.SaveData.Team
{
    /// <summary>
    /// Save data mirroring <c>TeamRuntimeState</c>.
    /// </summary>
    public sealed class TeamStateSaveData
    {
        public string TeamId;
        public List<string> MemberIds;
        public int Cohesion;
        public int Morale;
        public int Workload;
        public string CurrentAssignmentId;
        public List<string> AssignmentHistoryIds;
    }
}
