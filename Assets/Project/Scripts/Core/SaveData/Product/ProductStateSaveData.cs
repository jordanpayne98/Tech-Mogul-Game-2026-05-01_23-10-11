using System.Collections.Generic;

namespace Project.Core.SaveData.Product
{
    /// <summary>
    /// Save data mirroring <c>ProductRuntimeState</c>.
    /// Nullable <c>GameDateTime</c> fields serialized as nullable int.
    /// Dictionary keys use enum member name strings.
    /// </summary>
    public sealed class ProductStateSaveData
    {
        public string ProductId;

        /// <summary>Serialized <c>ProductStatus</c> enum member name.</summary>
        public string Status;

        public int ProgressPercent;
        public List<string> AssignedTeamIds;

        /// <summary>Serialized nullable <c>GameDateTime LaunchDate</c> as total elapsed hours. Null if not yet launched.</summary>
        public int? LaunchDateTotalElapsedHours;

        public long TotalRevenueMinorUnits;
        public long CurrentMonthRevenueMinorUnits;
        public long UnitsSoldTotal;
        public long UnitsSoldThisMonth;
        public int ActiveUsers;
        public int ReviewScore;
        public int RecentReviewScore;
        public int MarketShareBasisPoints;

        /// <summary>Key = <c>ProductScoreDimension</c> enum member name. Value = score.</summary>
        public Dictionary<string, int> ScoreValues;

        public List<string> MonthlyRevenueHistoryIds;
    }
}
