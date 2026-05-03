using System.Collections.Generic;

namespace Project.Core.SaveData.Product
{
    /// <summary>
    /// Save data mirroring <c>ProductProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> fields serialized as total elapsed hours.
    /// </summary>
    public sealed class ProductSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>ProductFamily</c> enum member name.</summary>
        public string Family;

        /// <summary>Serialized <c>ProductCategory</c> enum member name.</summary>
        public string Category;

        public string TargetMarketSegmentId;
        public string CustomerSegmentId;

        /// <summary>Serialized <c>RevenueModel</c> enum member name.</summary>
        public string RevenueModel;

        public long PriceMinorUnits;
        public int FeatureScope;
        public int QualityTarget;

        /// <summary>Serialized <c>GameDateTime CreatedDate</c> as total elapsed hours.</summary>
        public int CreatedDateTotalElapsedHours;

        /// <summary>Serialized <c>GameDateTime TargetReleaseDate</c> as total elapsed hours.</summary>
        public int TargetReleaseDateTotalElapsedHours;

        public List<string> SupportedPlatformIds;
        public bool RequiresSupport;
    }
}
