namespace Project.Core.SaveData.Product
{
    /// <summary>
    /// Save data mirroring <c>ProductBudgetProfile</c>.
    /// </summary>
    public sealed class ProductBudgetSaveData
    {
        public string ProductId;
        public long DevelopmentBudgetMinorUnits;
        public long PreLaunchMarketingMonthlyBudgetMinorUnits;
        public long PostLaunchMarketingMonthlyBudgetMinorUnits;
        public long PostLaunchSupportMonthlyBudgetMinorUnits;
    }
}
