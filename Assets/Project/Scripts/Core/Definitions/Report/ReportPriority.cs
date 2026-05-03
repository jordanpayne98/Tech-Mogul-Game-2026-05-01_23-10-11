namespace Project.Core.Definitions.Report
{
    /// <summary>
    /// Indicates the urgency level of a report.
    /// Used to sort inbox items and surface critical decisions to the player promptly.
    /// </summary>
    public enum ReportPriority
    {
        Critical,
        Important,
        Routine
    }
}
