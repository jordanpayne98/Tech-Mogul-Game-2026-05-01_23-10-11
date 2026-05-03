namespace Project.Core.Definitions.Company
{
    /// <summary>
    /// Represents the starting capital tier selected during game setup.
    /// No cash value mapping is stored here. Cash mapping belongs in Phase 2B / TuningConfig.
    /// </summary>
    public enum CapitalPreset
    {
        Garage,
        Bootstrapped,
        SeedFunded,
        VentureStart,
        Sandbox
    }
}
