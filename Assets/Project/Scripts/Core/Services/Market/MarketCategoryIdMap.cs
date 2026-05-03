using System;
using Project.Core.Definitions.Market;

namespace Project.Core.Services.Market
{
    /// <summary>
    /// Static utility providing deterministic semantic ID mapping for market categories.
    /// IDs are stable strings used as persistent references in runtime state and save data.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public static class MarketCategoryIdMap
    {
        /// <summary>
        /// Returns the deterministic semantic ID for the given market category type.
        /// </summary>
        /// <param name="categoryType">The market category type.</param>
        /// <returns>A stable semantic ID string (e.g. "market.game").</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the category type is not handled.</exception>
        public static string GetId(MarketCategoryType categoryType)
        {
            switch (categoryType)
            {
                case MarketCategoryType.Game:              return "market.game";
                case MarketCategoryType.DevelopmentTool:   return "market.development_tool";
                case MarketCategoryType.OperatingSystem:   return "market.operating_system";
                case MarketCategoryType.HardwarePlatform:  return "market.hardware_platform";
                case MarketCategoryType.WebPlatform:       return "market.web_platform";
                case MarketCategoryType.ProductivityApp:   return "market.productivity_app";
                case MarketCategoryType.BusinessSaaS:      return "market.business_saas";
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(categoryType),
                        $"[MarketCategoryIdMap] No semantic ID mapped for MarketCategoryType: {categoryType}.");
            }
        }

        /// <summary>
        /// Returns the market category type for the given semantic ID.
        /// Returns null if the ID is not recognised.
        /// </summary>
        /// <param name="id">The stable semantic ID (e.g. "market.game").</param>
        /// <returns>The corresponding <see cref="MarketCategoryType"/>, or null if unknown.</returns>
        public static MarketCategoryType? GetCategoryType(string id)
        {
            switch (id)
            {
                case "market.game":              return MarketCategoryType.Game;
                case "market.development_tool":  return MarketCategoryType.DevelopmentTool;
                case "market.operating_system":  return MarketCategoryType.OperatingSystem;
                case "market.hardware_platform": return MarketCategoryType.HardwarePlatform;
                case "market.web_platform":      return MarketCategoryType.WebPlatform;
                case "market.productivity_app":  return MarketCategoryType.ProductivityApp;
                case "market.business_saas":     return MarketCategoryType.BusinessSaaS;
                default:                         return null;
            }
        }
    }
}
