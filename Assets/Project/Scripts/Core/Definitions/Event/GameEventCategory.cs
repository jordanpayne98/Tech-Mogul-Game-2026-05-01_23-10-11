namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Classifies the simulation domain of a game event.
    /// Only Market, Team, and Product are used in Plan 2M.
    /// Employee, Finance, and Infrastructure are forward-compatible placeholders.
    /// </summary>
    public enum GameEventCategory
    {
        Market,
        Team,
        Product,
        Employee,
        Finance,
        Infrastructure
    }
}
