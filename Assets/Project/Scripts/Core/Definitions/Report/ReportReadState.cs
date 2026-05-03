namespace Project.Core.Definitions.Report
{
    /// <summary>
    /// Tracks whether the player has read a specific report.
    /// Stored per report ID in InboxRuntimeState.
    /// </summary>
    public enum ReportReadState
    {
        Unread,
        Read
    }
}
