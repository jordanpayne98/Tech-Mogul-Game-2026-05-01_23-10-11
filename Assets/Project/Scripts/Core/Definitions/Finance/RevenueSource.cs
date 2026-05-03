namespace Project.Core.Definitions.Finance
{
    /// <summary>
    /// Sources of company revenue.
    /// Used on TransactionRecord and MonthlyFinanceSummary for revenue breakdowns.
    /// </summary>
    public enum RevenueSource
    {
        SoftwareUnitSales,
        SoftwareSubscriptions,
        HardwareUnitSales,
        ContractPayment
    }
}
