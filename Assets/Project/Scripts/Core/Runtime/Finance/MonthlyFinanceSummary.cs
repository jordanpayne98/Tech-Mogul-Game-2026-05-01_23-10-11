using System.Collections.Generic;
using Project.Core.Definitions.Finance;

namespace Project.Core.Runtime.Finance
{
    /// <summary>
    /// Snapshot of financial performance for a single game month.
    /// Created at month-end and stored for historical reference and UI display.
    /// All monetary values are in minor currency units (e.g. pence for GBP).
    /// </summary>
    public sealed class MonthlyFinanceSummary
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable unique ID for this monthly summary.</summary>
        public string Id;

        /// <summary>The year-month period this summary covers.</summary>
        public FinancePeriodKey Period;

        // -------------------------------------------------------------------------
        // Cash position
        // -------------------------------------------------------------------------

        /// <summary>Cash balance at the start of the period in minor units.</summary>
        public long CashAtStartMinorUnits;

        /// <summary>Cash balance at the end of the period in minor units.</summary>
        public long CashAtEndMinorUnits;

        // -------------------------------------------------------------------------
        // Revenue and expenses
        // -------------------------------------------------------------------------

        /// <summary>Total revenue received during the period in minor units.</summary>
        public long TotalRevenueMinorUnits;

        /// <summary>Total expenses paid during the period in minor units.</summary>
        public long TotalExpensesMinorUnits;

        /// <summary>
        /// Net profit or loss for the period in minor units.
        /// Positive = profit. Negative = loss.
        /// Should equal TotalRevenueMinorUnits minus TotalExpensesMinorUnits.
        /// </summary>
        public long NetProfitLossMinorUnits;

        // -------------------------------------------------------------------------
        // Breakdowns
        // -------------------------------------------------------------------------

        /// <summary>Revenue in minor units broken down by source.</summary>
        public Dictionary<RevenueSource, long> RevenueBySource = new Dictionary<RevenueSource, long>();

        /// <summary>Expenses in minor units broken down by category.</summary>
        public Dictionary<ExpenseCategory, long> ExpensesByCategory = new Dictionary<ExpenseCategory, long>();

        // -------------------------------------------------------------------------
        // Runway
        // -------------------------------------------------------------------------

        /// <summary>
        /// Estimated months of cash runway at the end of this period.
        /// Calculated from CashAtEndMinorUnits and the projected monthly burn at period close.
        /// </summary>
        public int RunwayMonths;

        /// <summary>
        /// True if runway was trending stable or improving at period close; false if declining.
        /// </summary>
        public bool IsRunwayStable;

        // -------------------------------------------------------------------------
        // Payroll delta
        // -------------------------------------------------------------------------

        /// <summary>
        /// Change in total monthly payroll during this period in minor units.
        /// Positive = payroll increased (hiring/raises). Negative = payroll decreased (departures/cuts).
        /// </summary>
        public long PayrollChangeMinorUnits;
    }
}
