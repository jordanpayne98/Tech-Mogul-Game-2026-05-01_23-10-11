namespace Project.Core.Definitions.Time
{
    /// <summary>
    /// Controls how fast game time advances while the simulation is running.
    /// Paused halts all advancement. Speed1x through Speed3x are multiplier tiers.
    /// </summary>
    public enum TimeSpeed
    {
        Paused,
        Speed1x,
        Speed2x,
        Speed3x
    }
}
