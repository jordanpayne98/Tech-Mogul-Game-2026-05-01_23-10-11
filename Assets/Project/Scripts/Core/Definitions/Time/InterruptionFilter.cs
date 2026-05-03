namespace Project.Core.Definitions.Time
{
    /// <summary>
    /// Controls which interruption types are allowed to pause continuous time advancement.
    /// Custom allows the player to define a bespoke set of active interruption types.
    /// </summary>
    public enum InterruptionFilter
    {
        CriticalOnly,
        ImportantAndCritical,
        All,
        Custom
    }
}
