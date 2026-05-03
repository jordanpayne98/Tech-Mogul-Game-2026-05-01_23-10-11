using System.Collections.Generic;

namespace Project.Core.SaveData.Market
{
    /// <summary>
    /// Save data mirroring <c>CompetitorProfile</c>.
    /// Enum fields stored as member name strings.
    /// </summary>
    public sealed class CompetitorSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>CompetitorArchetype</c> enum member name.</summary>
        public string Archetype;

        /// <summary>Serialized <c>ProductFamily</c> enum member names representing market focus areas.</summary>
        public List<string> MarketFocus;

        public int RiskAppetite;

        /// <summary>Serialized <c>CompetitorPricingStyle</c> enum member name.</summary>
        public string PricingStyle;

        /// <summary>Serialized <c>CompetitorMarketingStyle</c> enum member name.</summary>
        public string MarketingStyle;
    }
}
