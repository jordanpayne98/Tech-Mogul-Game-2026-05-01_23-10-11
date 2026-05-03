using System.Collections.Generic;

namespace Project.Core.SaveData.Research
{
    /// <summary>
    /// Save data mirroring <c>ResearchRuntimeState</c>.
    /// </summary>
    public sealed class ResearchSaveData
    {
        public string CompanyId;
        public List<string> AvailableProjectIds;
        public List<string> ActiveProjectIds;
        public List<string> CompletedProjectIds;
        public List<string> ObsoleteProjectIds;
        public List<string> UnlockedCapabilityIds;
    }
}
