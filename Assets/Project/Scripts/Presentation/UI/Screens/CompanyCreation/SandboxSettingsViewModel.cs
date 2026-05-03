namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Immutable display-data for the sandbox configuration settings.
    /// </summary>
    public sealed class SandboxSettingsViewModel
    {
        public string MarketSize          { get; }
        public string CompetitorDensity   { get; }
        public string TechnologyPace      { get; }
        public string EconomicVolatility  { get; }
        public string HiringDifficulty    { get; }
        public string FailureMode         { get; }
        public string MarketSeed          { get; }

        public SandboxSettingsViewModel(
            string marketSize,
            string competitorDensity,
            string technologyPace,
            string economicVolatility,
            string hiringDifficulty,
            string failureMode,
            string marketSeed)
        {
            MarketSize         = marketSize         ?? "Standard";
            CompetitorDensity  = competitorDensity  ?? "Standard";
            TechnologyPace     = technologyPace     ?? "Standard";
            EconomicVolatility = economicVolatility ?? "Standard";
            HiringDifficulty   = hiringDifficulty   ?? "Standard";
            FailureMode        = failureMode        ?? "Standard";
            MarketSeed         = marketSeed         ?? string.Empty;
        }
    }
}
