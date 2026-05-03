namespace Project.Core.Definitions.Report
{
    /// <summary>
    /// UI-facing filter options available in the player inbox.
    /// Distinct from ReportCategory, which classifies the report's simulation domain.
    /// </summary>
    public enum InboxFilter
    {
        All,
        RequiresDecision,
        Finance,
        Products,
        Employees,
        Teams,
        Hiring,
        Market,
        Competitors,
        Infrastructure,
        Support,
        Contracts,
        Research,
        Archived
    }
}
