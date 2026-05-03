using System.Collections.Generic;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Finance
{
    /// <summary>
    /// Authoritative mutable runtime state for the company's finances.
    /// CashMinorUnits is the single source of truth for current cash.
    /// All monetary values are stored in minor currency units (e.g. pence for GBP).
    /// Example: £50,000 = 5_000_000L minor units.
    /// No funding system (VC rounds, loans) in Phase 1 — deferred entirely.
    /// </summary>
    public sealed class FinanceRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID of the company this state belongs to.</summary>
        public string CompanyId;

        // -------------------------------------------------------------------------
        // Cash — single source of truth
        // -------------------------------------------------------------------------

        /// <summary>
        /// Current cash balance in minor currency units.
        /// This is the authoritative cash value. CompanyRuntimeState must not hold a separate cash field.
        /// </summary>
        public long CashMinorUnits;

        // -------------------------------------------------------------------------
        // Monthly cost projections — used for runway calculation
        // -------------------------------------------------------------------------

        /// <summary>Total monthly payroll cost in minor units across all active employees.</summary>
        public long MonthlyPayrollMinorUnits;

        /// <summary>Total monthly infrastructure cost in minor units.</summary>
        public long MonthlyInfrastructureCostMinorUnits;

        /// <summary>Total monthly customer support cost in minor units.</summary>
        public long MonthlySupportCostMinorUnits;

        /// <summary>Total monthly marketing spend in minor units.</summary>
        public long MonthlyMarketingSpendMinorUnits;

        /// <summary>Total monthly research spend in minor units.</summary>
        public long MonthlyResearchSpendMinorUnits;

        /// <summary>Total monthly manufacturing cost in minor units.</summary>
        public long MonthlyManufacturingCostMinorUnits;

        // -------------------------------------------------------------------------
        // Monthly revenue projections
        // -------------------------------------------------------------------------

        /// <summary>Projected monthly product revenue (unit sales + subscriptions) in minor units.</summary>
        public long MonthlyProductRevenueMinorUnits;

        /// <summary>Projected monthly contract revenue in minor units.</summary>
        public long MonthlyContractRevenueMinorUnits;

        // -------------------------------------------------------------------------
        // Net position
        // -------------------------------------------------------------------------

        /// <summary>
        /// Monthly net profit or loss in minor units.
        /// Positive = profit. Negative = loss.
        /// </summary>
        public long MonthlyNetProfitLossMinorUnits;

        // -------------------------------------------------------------------------
        // Runway
        // -------------------------------------------------------------------------

        /// <summary>
        /// Estimated months of cash runway at current burn rate.
        /// Calculated from CashMinorUnits and monthly net position.
        /// </summary>
        public int RunwayMonths;

        /// <summary>
        /// True if runway is trending stable or improving; false if declining.
        /// Used by UI and simulation to surface financial health warnings.
        /// </summary>
        public bool IsRunwayStable;

        // -------------------------------------------------------------------------
        // Payroll tracking
        // -------------------------------------------------------------------------

        /// <summary>
        /// Date of the last payroll disbursement. Null until the first payroll has run.
        /// </summary>
        public GameDateTime LastPayrollDate;

        // -------------------------------------------------------------------------
        // Related record IDs
        // -------------------------------------------------------------------------

        /// <summary>Stable IDs of all TransactionRecords associated with this company.</summary>
        public List<string> TransactionIds = new List<string>();

        /// <summary>Stable IDs of all MonthlyFinanceSummaries associated with this company.</summary>
        public List<string> MonthlySummaryIds = new List<string>();
    }
}
