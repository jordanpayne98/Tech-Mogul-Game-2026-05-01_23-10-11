using System.Collections.Generic;

namespace Project.Core.SaveData.Market
{
    /// <summary>
    /// Save data mirroring <c>TrendRuntimeState</c>.
    /// Enum fields stored as member name strings. Nullable <c>GameDateTime</c> serialized as nullable int.
    /// </summary>
    public sealed class TrendSaveData
    {
        public string Id;

        /// <summary>Serialized <c>TrendType</c> enum member name.</summary>
        public string Type;

        public int Strength;

        /// <summary>Serialized <c>MarketCategoryType</c> enum member names.</summary>
        public List<string> AffectedCategories;

        /// <summary>Serialized <c>GameDateTime StartDate</c> as total elapsed hours.</summary>
        public int StartDateTotalElapsedHours;

        /// <summary>Serialized nullable <c>GameDateTime EstimatedEndDate</c> as total elapsed hours. Null if open-ended.</summary>
        public int? EstimatedEndDateTotalElapsedHours;

        public bool IsActive;
    }
}
