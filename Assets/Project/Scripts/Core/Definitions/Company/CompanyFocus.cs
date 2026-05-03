namespace Project.Core.Definitions.Company
{
    /// <summary>
    /// Represents the market focus identity of the player's company selected during game setup.
    /// CompanyFocus is flavour and identity only. It must not lock, boost, gate, penalise,
    /// or restrict any gameplay mechanic. No system may branch on this value to alter outcomes.
    /// </summary>
    public enum CompanyFocus
    {
        ConsumerSoftware,
        EnterpriseSaaS,
        DeveloperTools,
        GamesAndEntertainment,
        HardwareDevices,
        CloudInfrastructure,
        Security,
        AIAndAutomation,
        PlatformEcosystems
    }
}
