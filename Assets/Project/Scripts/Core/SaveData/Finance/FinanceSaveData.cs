using System.Collections.Generic;

namespace Project.Core.SaveData.Finance
{
    /// <summary>
    /// Save data mirroring <c>FinanceRuntimeState</c>.
    /// Nullable <c>GameDateTime</c> field serialized as nullable int.
    /// </summary>
    public sealed class FinanceSaveData
    {
        public string CompanyId;
        public long CashMinorUnits;
        public long MonthlyPayrollMinorUnits;
        public long MonthlyInfrastructureCostMinorUnits;
        public long MonthlySupportCostMinorUnits;
        public long MonthlyMarketingSpendMinorUnits;
        public long MonthlyResearchSpendMinorUnits;
        public long MonthlyManufacturingCostMinorUnits;
        public long MonthlyProductRevenueMinorUnits;
        public long MonthlyContractRevenueMinorUnits;
        public long MonthlyNetProfitLossMinorUnits;
        public int RunwayMonths;
        public bool IsRunwayStable;

        /// <summary>Serialized nullable <c>GameDateTime LastPayrollDate</c> as total elapsed hours. Null until the first payroll has run.</summary>
        public int? LastPayrollDateTotalElapsedHours;

        public List<string> TransactionIds;
        public List<string> MonthlySummaryIds;
    }
}
