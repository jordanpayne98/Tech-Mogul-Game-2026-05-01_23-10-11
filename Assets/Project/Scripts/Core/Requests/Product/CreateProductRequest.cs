using System.Collections.Generic;
using Project.Core.Definitions.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Requests.Product
{
    /// <summary>
    /// Carries all data required to initiate the creation of a new product.
    /// Submitted by the player through the application layer.
    /// All monetary fields use minor currency units (e.g. cents).
    /// </summary>
    public sealed class CreateProductRequest
    {
        /// <summary>Player-defined display name for the product.</summary>
        public string Name;

        /// <summary>The specific product category (Game, DevelopmentTool, OperatingSystem, HardwarePlatform).</summary>
        public ProductCategory Category;

        /// <summary>Stable ID referencing the target market segment definition.</summary>
        public string TargetMarketSegmentId;

        /// <summary>Stable ID referencing the target customer segment definition.</summary>
        public string CustomerSegmentId;

        /// <summary>How this product will generate revenue from customers.</summary>
        public RevenueModel RevenueModel;

        /// <summary>Intended product price in minor currency units.</summary>
        public long PriceMinorUnits;

        /// <summary>Scope of features planned for this product.</summary>
        public int FeatureScope;

        /// <summary>Target quality score (0–100) for this product at launch.</summary>
        public int QualityTarget;

        /// <summary>The in-game date the player is targeting for product launch.</summary>
        public GameDateTime TargetReleaseDate;

        /// <summary>Stable IDs referencing platform definitions this product should support.</summary>
        public List<string> SupportedPlatformIds;

        /// <summary>Total development budget in minor currency units.</summary>
        public long DevelopmentBudgetMinorUnits;

        /// <summary>Monthly pre-launch marketing budget in minor currency units.</summary>
        public long PreLaunchMarketingMonthlyBudgetMinorUnits;

        /// <summary>Monthly post-launch marketing budget in minor currency units.</summary>
        public long PostLaunchMarketingMonthlyBudgetMinorUnits;

        /// <summary>Monthly post-launch support resourcing budget in minor currency units.</summary>
        public long PostLaunchSupportMonthlyBudgetMinorUnits;

        // ─── Hardware-specific fields ─────────────────────────────────────────────

        /// <summary>
        /// Bill of Materials tier for hardware products.
        /// Determines manufacturing cost and defect rate multipliers.
        /// Ignored for software products.
        /// </summary>
        public BOMTier BOMTier;

        /// <summary>
        /// Number of units to manufacture for launch.
        /// Ignored for software products. Must be positive for hardware products.
        /// </summary>
        public int LaunchStockQuantity;
    }
}
