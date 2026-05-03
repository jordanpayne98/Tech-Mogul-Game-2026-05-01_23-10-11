using Project.Core.Definitions.Company;

namespace Project.Core.Requests.Company
{
    /// <summary>
    /// Request data for creating a new company and founder at game setup.
    /// Stub — full validation and use case logic are deferred to later plans.
    /// </summary>
    public sealed class CreateCompanyRequest
    {
        public string FounderName { get; }
        public string CompanyName { get; }
        public string LogoIconId { get; }
        public string BrandColourHex { get; }
        public string Location { get; }
        public FounderBackground Background { get; }
        public CapitalPreset CapitalPreset { get; }
        public CompanyFocus Focus { get; }
        public SandboxDifficulty Difficulty { get; }

        /// <summary>Seed for deterministic market generation. Allows repeatable sandbox runs.</summary>
        public int MarketSeed { get; }

        public CreateCompanyRequest(
            string founderName,
            string companyName,
            string logoIconId,
            string brandColourHex,
            string location,
            FounderBackground background,
            CapitalPreset capitalPreset,
            CompanyFocus focus,
            SandboxDifficulty difficulty,
            int marketSeed)
        {
            FounderName = founderName;
            CompanyName = companyName;
            LogoIconId = logoIconId;
            BrandColourHex = brandColourHex;
            Location = location;
            Background = background;
            CapitalPreset = capitalPreset;
            Focus = focus;
            Difficulty = difficulty;
            MarketSeed = marketSeed;
        }
    }
}
