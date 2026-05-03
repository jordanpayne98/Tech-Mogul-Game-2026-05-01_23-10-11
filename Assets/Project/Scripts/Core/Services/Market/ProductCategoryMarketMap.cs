using System;
using Project.Core.Definitions.Market;
using Project.Core.Definitions.Product;

namespace Project.Core.Services.Market
{
    /// <summary>
    /// Static utility mapping ProductCategory to its corresponding MarketCategoryType.
    /// Used by CompetitorTickProcessor to determine which market a player product competes in.
    /// Mapping defined in Plan 2J implementation notes.
    /// </summary>
    public static class ProductCategoryMarketMap
    {
        /// <summary>
        /// Returns the MarketCategoryType corresponding to the given ProductCategory.
        /// Hardware categories (Peripheral, LaptopDesktopDevice, ServerDevice) map to HardwarePlatform.
        /// </summary>
        /// <param name="productCategory">The product category to map.</param>
        /// <returns>The corresponding market category type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the ProductCategory is not handled.</exception>
        public static MarketCategoryType GetMarketCategory(ProductCategory productCategory)
        {
            switch (productCategory)
            {
                case ProductCategory.Game:               return MarketCategoryType.Game;
                case ProductCategory.DevelopmentTool:    return MarketCategoryType.DevelopmentTool;
                case ProductCategory.OperatingSystem:    return MarketCategoryType.OperatingSystem;
                case ProductCategory.HardwarePlatform:   return MarketCategoryType.HardwarePlatform;
                case ProductCategory.Peripheral:         return MarketCategoryType.HardwarePlatform;
                case ProductCategory.LaptopDesktopDevice: return MarketCategoryType.HardwarePlatform;
                case ProductCategory.ServerDevice:       return MarketCategoryType.HardwarePlatform;
                case ProductCategory.WebPlatform:        return MarketCategoryType.WebPlatform;
                case ProductCategory.ProductivityApp:    return MarketCategoryType.ProductivityApp;
                case ProductCategory.BusinessSaaS:       return MarketCategoryType.BusinessSaaS;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(productCategory),
                        $"[ProductCategoryMarketMap] No MarketCategoryType mapped for ProductCategory: {productCategory}.");
            }
        }
    }
}
