namespace Project.Core.Runtime.Product
{
    /// <summary>
    /// Per-save entity that stores the player-defined budget allocations for a product.
    /// References the owning ProductProfile by stable ID.
    /// All monetary values are expressed in minor currency units (e.g. cents).
    /// </summary>
    public sealed class ProductBudgetProfile
    {
        /// <summary>Stable ID of the ProductProfile this budget belongs to.</summary>
        public string ProductId;

        /// <summary>Total development budget allocated for this product in minor currency units.</summary>
        public long DevelopmentBudgetMinorUnits;

        /// <summary>Monthly marketing spend allocated for the pre-launch phase in minor currency units.</summary>
        public long PreLaunchMarketingMonthlyBudgetMinorUnits;

        /// <summary>Monthly marketing spend allocated after launch in minor currency units.</summary>
        public long PostLaunchMarketingMonthlyBudgetMinorUnits;

        /// <summary>Monthly support resourcing budget allocated after launch in minor currency units.</summary>
        public long PostLaunchSupportMonthlyBudgetMinorUnits;
    }
}
