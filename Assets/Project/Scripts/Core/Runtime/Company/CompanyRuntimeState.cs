using System.Collections.Generic;

namespace Project.Core.Runtime.Company
{
    /// <summary>
    /// Mutable runtime state for the active company. Tracks all live relational IDs and company-level metrics.
    /// CashMinorUnits is intentionally absent — cash is authoritative on FinanceRuntimeState (Plan 1H).
    /// </summary>
    public sealed class CompanyRuntimeState
    {
        /// <summary>Stable ID referencing the associated CompanyProfile.</summary>
        public string CompanyId { get; set; }

        /// <summary>Current company reputation score.</summary>
        public int Reputation { get; set; }

        /// <summary>Stable IDs of all current employees.</summary>
        public List<string> EmployeeIds { get; set; }

        /// <summary>Stable IDs of all current teams.</summary>
        public List<string> TeamIds { get; set; }

        /// <summary>Stable IDs of all active or completed products.</summary>
        public List<string> ProductIds { get; set; }

        /// <summary>Stable IDs of all active or completed contracts.</summary>
        public List<string> ContractIds { get; set; }

        /// <summary>Stable IDs of all active or completed research projects.</summary>
        public List<string> ResearchProjectIds { get; set; }

        /// <summary>Stable IDs of all generated reports.</summary>
        public List<string> ReportIds { get; set; }

        /// <summary>Stable IDs of all tracked market positions.</summary>
        public List<string> MarketPositionIds { get; set; }

        public CompanyRuntimeState()
        {
            CompanyId = string.Empty;
            Reputation = 0;
            EmployeeIds = new List<string>();
            TeamIds = new List<string>();
            ProductIds = new List<string>();
            ContractIds = new List<string>();
            ResearchProjectIds = new List<string>();
            ReportIds = new List<string>();
            MarketPositionIds = new List<string>();
        }
    }
}
