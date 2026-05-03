namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Indicates the urgency level of a game event.
    /// Used to route the event to an appropriate interruption tier and report priority.
    /// </summary>
    public enum GameEventSeverity
    {
        Minor,
        Moderate,
        Major,
        Critical
    }
}
