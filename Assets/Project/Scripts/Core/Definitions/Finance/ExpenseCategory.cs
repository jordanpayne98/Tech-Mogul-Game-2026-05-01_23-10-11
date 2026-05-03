namespace Project.Core.Definitions.Finance
{
    /// <summary>
    /// Categories for classifying company expenses.
    /// Used on TransactionRecord and MonthlyFinanceSummary for expense breakdowns.
    /// </summary>
    public enum ExpenseCategory
    {
        Payroll,
        Infrastructure,
        Marketing,
        Research,
        HardwarePrototyping,
        Manufacturing,
        Support,
        Recruiting
    }
}
