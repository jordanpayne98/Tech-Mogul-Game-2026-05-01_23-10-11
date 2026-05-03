using System.Collections.Generic;
using Project.Core.Definitions.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Product
{
    /// <summary>
    /// Per-save entity that captures the player-defined profile for a product.
    /// Created when the player initiates a new product. Immutable intent data.
    /// Placed in Core/Runtime because it is player-authored per-save data,
    /// not a static design definition.
    /// </summary>
    public sealed class ProductProfile
    {
        /// <summary>Stable unique identifier for this product instance.</summary>
        public string Id;

        /// <summary>Player-defined display name for this product.</summary>
        public string Name;

        /// <summary>
        /// Top-level family: Software or Hardware.
        /// Stored explicitly for convenience; derivable from Category.
        /// </summary>
        public ProductFamily Family;

        /// <summary>The specific product category (Game, DevelopmentTool, OperatingSystem, HardwarePlatform).</summary>
        public ProductCategory Category;

        /// <summary>Stable ID referencing the target market segment definition.</summary>
        public string TargetMarketSegmentId;

        /// <summary>Stable ID referencing the target customer segment definition.</summary>
        public string CustomerSegmentId;

        /// <summary>How this product generates revenue from customers.</summary>
        public RevenueModel RevenueModel;

        /// <summary>Product price expressed in minor currency units (e.g. cents).</summary>
        public long PriceMinorUnits;

        /// <summary>Scope of features planned for this product. Higher values indicate broader feature sets.</summary>
        public int FeatureScope;

        /// <summary>Target quality score (0–100) the player is aiming for at launch.</summary>
        public int QualityTarget;

        /// <summary>The in-game date this product profile was created.</summary>
        public GameDateTime CreatedDate;

        /// <summary>The in-game date the player is targeting for product launch.</summary>
        public GameDateTime TargetReleaseDate;

        /// <summary>
        /// Stable IDs referencing platform definitions this product is intended to support.
        /// Applies primarily to Software products.
        /// </summary>
        public List<string> SupportedPlatformIds;

        /// <summary>Whether this product requires ongoing post-launch support resourcing.</summary>
        public bool RequiresSupport;
    }
}
