namespace Project.Core.Events.Finance
{
    /// <summary>
    /// Published at the end of the monthly finance cycle, after all revenue, expenses,
    /// contract payments, and runway have been computed.
    /// Subscribers can use this to refresh finance UI panels and dashboards.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class MonthlyFinanceProcessedEvent
    {
        /// <summary>Stable ID of the company whose monthly finance cycle completed.</summary>
        public string CompanyId { get; }

        /// <summary>Total revenue received during the closed month in minor currency units.</summary>
        public long RevenueMinorUnits { get; }

        /// <summary>Total expenses paid during the closed month in minor currency units.</summary>
        public long ExpensesMinorUnits { get; }

        /// <summary>Net profit or loss for the closed month in minor currency units. Positive = profit.</summary>
        public long NetMinorUnits { get; }

        /// <summary>Estimated months of cash runway at the end of the closed month.</summary>
        public int RunwayMonths { get; }

        /// <summary>
        /// Creates a new MonthlyFinanceProcessedEvent.
        /// </summary>
        public MonthlyFinanceProcessedEvent(
            string companyId,
            long revenueMinorUnits,
            long expensesMinorUnits,
            long netMinorUnits,
            int runwayMonths)
        {
            CompanyId        = companyId;
            RevenueMinorUnits  = revenueMinorUnits;
            ExpensesMinorUnits = expensesMinorUnits;
            NetMinorUnits      = netMinorUnits;
            RunwayMonths       = runwayMonths;
        }
    }
}
