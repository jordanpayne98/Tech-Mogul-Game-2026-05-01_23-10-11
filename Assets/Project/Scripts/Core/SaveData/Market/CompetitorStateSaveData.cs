using System.Collections.Generic;

namespace Project.Core.SaveData.Market
{
    /// <summary>
    /// Save data mirroring <c>CompetitorRuntimeState</c>.
    /// Dictionary keys use enum member name strings.
    /// </summary>
    public sealed class CompetitorStateSaveData
    {
        public string CompetitorId;
        public int CashStrength;
        public int Reputation;
        public List<string> ProductIds;
        public int HiringStrength;
        public int ResearchStrength;
        public int LaunchCadence;

        /// <summary>Key = <c>MarketCategoryType</c> enum member name. Value = market share in basis points.</summary>
        public Dictionary<string, int> MarketShareBasisPoints;
    }
}
