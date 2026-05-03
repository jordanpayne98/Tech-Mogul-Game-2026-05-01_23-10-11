namespace Project.Core.Definitions.Market
{
    /// <summary>
    /// High-level market categories. 7 MVP categories aligned with ProductCategory.
    /// Used to classify which market a product or competitor operates in.
    /// </summary>
    public enum MarketCategoryType
    {
        Game,
        DevelopmentTool,
        OperatingSystem,
        HardwarePlatform,
        WebPlatform,
        ProductivityApp,
        BusinessSaaS
    }
}
