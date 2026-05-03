namespace Project.Core.Definitions.Report
{
    /// <summary>
    /// Classifies a report by its simulation domain.
    /// Used internally to categorize reports for routing and filtering logic.
    /// Distinct from InboxFilter, which is a UI-facing player filter.
    /// </summary>
    public enum ReportCategory
    {
        Finance,
        Product,
        Employee,
        Team,
        Hiring,
        Market,
        Competitor,
        Infrastructure,
        Support,
        Contract,
        Research,
        Company,
        System
    }
}
