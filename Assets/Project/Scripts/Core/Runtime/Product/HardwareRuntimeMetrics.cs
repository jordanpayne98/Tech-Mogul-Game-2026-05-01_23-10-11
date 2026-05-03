using Project.Core.Definitions.Product;

namespace Project.Core.Runtime.Product
{
    /// <summary>
    /// Mutable domain-specific metrics for Hardware products.
    /// Tracks manufacturing economics, defect rates, warranty costs,
    /// inventory levels, component availability, and return rates.
    /// DefectRateBasisPoints and ReturnRateBasisPoints use a 0–10000 range (100 = 1%).
    /// All monetary fields use minor currency units (e.g. cents).
    /// </summary>
    public sealed class HardwareRuntimeMetrics
    {
        /// <summary>Stable ID of the ProductProfile these metrics belong to.</summary>
        public string ProductId;

        /// <summary>Bill of Materials tier currently selected for this hardware product.</summary>
        public BOMTier BOMTier;

        /// <summary>Manufacturing cost per unit in minor currency units.</summary>
        public long ManufacturingCostPerUnitMinorUnits;

        /// <summary>Margin earned per unit sold in minor currency units.</summary>
        public long UnitMarginMinorUnits;

        /// <summary>
        /// Rate of defective units produced, expressed in basis points (0–10000).
        /// 100 basis points = 1% defect rate.
        /// </summary>
        public int DefectRateBasisPoints;

        /// <summary>Total warranty costs incurred in the current simulation month in minor currency units.</summary>
        public long WarrantyCostThisMonthMinorUnits;

        /// <summary>Number of units manufactured and ready for sale at launch.</summary>
        public int LaunchStock;

        /// <summary>Number of units currently held in inventory.</summary>
        public int CurrentInventory;

        /// <summary>
        /// Availability score for key components required to manufacture this product.
        /// Higher values indicate better supply chain health.
        /// </summary>
        public int ComponentAvailability;

        /// <summary>
        /// Rate at which sold units are returned by customers, expressed in basis points (0–10000).
        /// 100 basis points = 1% return rate.
        /// </summary>
        public int ReturnRateBasisPoints;
    }
}
