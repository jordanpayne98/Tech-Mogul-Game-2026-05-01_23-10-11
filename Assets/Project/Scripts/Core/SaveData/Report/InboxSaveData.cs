using System.Collections.Generic;

namespace Project.Core.SaveData.Report
{
    /// <summary>
    /// Save data mirroring <c>InboxRuntimeState</c>.
    /// Dictionary values use enum member name strings.
    /// </summary>
    public sealed class InboxSaveData
    {
        public string CompanyId;
        public List<string> ReportIds;

        /// <summary>Key = report stable ID. Value = <c>ReportReadState</c> enum member name.</summary>
        public Dictionary<string, string> ReportReadStates;

        /// <summary>Key = report stable ID. Value = <c>ReportInboxState</c> enum member name.</summary>
        public Dictionary<string, string> ReportInboxStates;

        public int UnreadCount;
        public int DecisionRequiredCount;
    }
}
