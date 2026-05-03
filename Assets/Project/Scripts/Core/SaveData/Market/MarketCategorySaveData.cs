using System.Collections.Generic;

namespace Project.Core.SaveData.Market
{
    /// <summary>
    /// Save data mirroring <c>MarketCategoryRuntimeState</c>.
    /// Enum fields stored as member name strings. Dictionary keys use enum member name strings.
    /// </summary>
    public sealed class MarketCategorySaveData
    {
        public string Id;

        /// <summary>Serialized <c>MarketCategoryType</c> enum member name.</summary>
        public string CategoryType;

        public int TotalDemand;
        public int GrowthRateBasisPoints;

        /// <summary>Key = <c>CustomerSegment</c> enum member name. Value = demand weight.</summary>
        public Dictionary<string, int> SegmentDemandWeights;

        /// <summary>Key = <c>CustomerPreference</c> enum member name. Value = preference score.</summary>
        public Dictionary<string, int> CustomerPreferences;

        public int CompetitiveIntensity;
        public List<string> LeaderProductIds;
        public int TechnologyExpectation;
        public int PriceSensitivity;
        public int MarketingSensitivity;
        public int SupportExpectation;
        public List<string> ActiveTrendIds;
    }
}
