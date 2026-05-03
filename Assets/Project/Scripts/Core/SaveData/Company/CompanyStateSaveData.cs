using System.Collections.Generic;

namespace Project.Core.SaveData.Company
{
    /// <summary>
    /// Save data mirroring <c>CompanyRuntimeState</c>.
    /// All relational references use stable ID strings.
    /// </summary>
    public sealed class CompanyStateSaveData
    {
        public string CompanyId;
        public int Reputation;
        public List<string> EmployeeIds;
        public List<string> TeamIds;
        public List<string> ProductIds;
        public List<string> ContractIds;
        public List<string> ResearchProjectIds;
        public List<string> ReportIds;
        public List<string> MarketPositionIds;
    }
}
