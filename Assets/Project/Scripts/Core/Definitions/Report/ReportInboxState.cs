namespace Project.Core.Definitions.Report
{
    /// <summary>
    /// Tracks the inbox lifecycle state of a report.
    /// Stored per report ID in InboxRuntimeState.
    /// Separate from ReportReadState, which tracks read/unread status only.
    /// </summary>
    public enum ReportInboxState
    {
        Active,
        Archived,
        Deleted
    }
}
