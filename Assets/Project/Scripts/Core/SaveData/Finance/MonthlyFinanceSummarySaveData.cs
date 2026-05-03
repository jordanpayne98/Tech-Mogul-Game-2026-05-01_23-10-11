using System.Collections.Generic;

namespace Project.Core.SaveData.Finance
{
    /// <summary>
    /// Save data mirroring <c>MonthlyFinanceSummary</c>.
    /// <c>FinancePeriodKey</c> is flattened to <c>PeriodYear</c> and <c>PeriodMonth</c> ints.
    /// Dictionary keys use enum member name strings.
    /// </summary>
    public sealed class MonthlyFinanceSummarySaveData
    {
        public string Id;

        /// <summary>Flattened from <c>FinancePeriodKey.Year</c>.</summary>
        public int PeriodYear;

        /// <summary>Flattened from <c>FinancePeriodKey.Month</c>.</summary>
        public int PeriodMonth;

        public long CashAtStartMinorUnits;
        public long CashAtEndMinorUnits;
        public long TotalRevenueMinorUnits;
        public long TotalExpensesMinorUnits;
        public long NetProfitLossMinorUnits;

        /// <summary>Key = <c>RevenueSource</c> enum member name. Value = revenue in minor units.</summary>
        public Dictionary<string, long> RevenueBySource;

        /// <summary>Key = <c>ExpenseCategory</c> enum member name. Value = expenses in minor units.</summary>
        public Dictionary<string, long> ExpensesByCategory;

        public int RunwayMonths;
        public bool IsRunwayStable;
        public long PayrollChangeMinorUnits;
    }
}
