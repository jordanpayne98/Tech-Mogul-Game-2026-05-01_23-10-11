namespace Project.Core.SaveData.Product
{
    /// <summary>
    /// Save data mirroring <c>HardwareRuntimeMetrics</c>.
    /// Enum fields stored as member name strings.
    /// </summary>
    public sealed class HardwareMetricsSaveData
    {
        public string ProductId;

        /// <summary>Serialized <c>BOMTier</c> enum member name.</summary>
        public string BOMTier;

        public long ManufacturingCostPerUnitMinorUnits;
        public long UnitMarginMinorUnits;
        public int DefectRateBasisPoints;
        public long WarrantyCostThisMonthMinorUnits;
        public int LaunchStock;
        public int CurrentInventory;
        public int ComponentAvailability;
        public int ReturnRateBasisPoints;
    }
}
