namespace Project.Core.Definitions.Time
{
    /// <summary>
    /// Determines whether time advances one tick at a time (Manual) or
    /// runs continuously until an interruption is triggered (ContinueUntilInterrupt).
    /// </summary>
    public enum TimeAdvanceMode
    {
        Manual,
        ContinueUntilInterrupt
    }
}
