using System;
using System.Collections.Generic;
using Project.Core.Definitions.Market;

namespace Project.Core.Services.Market
{
    /// <summary>
    /// Static mapping of trend types to boosted customer preference dimensions.
    /// Used by <see cref="MarketService"/> when computing monthly preference drift.
    /// All mappings are [Placeholder] for MVP. Can be expanded or made data-driven later.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public static class TrendPreferenceMap
    {
        /// <summary>
        /// Returns the customer preference dimensions that the given trend type boosts.
        /// Returns an empty list if no preferences are mapped for the trend.
        /// </summary>
        /// <param name="trendType">The macro trend type.</param>
        /// <returns>A list of boosted <see cref="CustomerPreference"/> values. [Placeholder]</returns>
        public static List<CustomerPreference> GetBoostedPreferences(TrendType trendType)
        {
            switch (trendType)
            {
                case TrendType.RemoteWorkBoom:
                    return new List<CustomerPreference> { CustomerPreference.Usability, CustomerPreference.Reliability };

                case TrendType.AIAutomationDemand:
                    return new List<CustomerPreference> { CustomerPreference.Features, CustomerPreference.Performance };

                case TrendType.SecurityPanic:
                    return new List<CustomerPreference> { CustomerPreference.Security };

                case TrendType.HardwareRefreshCycle:
                    return new List<CustomerPreference> { CustomerPreference.Performance, CustomerPreference.Features };

                case TrendType.DeveloperToolingShift:
                    return new List<CustomerPreference> { CustomerPreference.Features, CustomerPreference.Usability };

                case TrendType.CloudCostSpike:
                    return new List<CustomerPreference> { CustomerPreference.Price };

                case TrendType.Recession:
                    return new List<CustomerPreference> { CustomerPreference.Price };

                case TrendType.ChipShortage:
                    return new List<CustomerPreference> { CustomerPreference.Price, CustomerPreference.Reliability };

                case TrendType.SubscriptionFatigue:
                    return new List<CustomerPreference> { CustomerPreference.Price };

                case TrendType.OpenSourceSurge:
                    return new List<CustomerPreference> { CustomerPreference.Price, CustomerPreference.Features };

                case TrendType.GamingBoom:
                    return new List<CustomerPreference> { CustomerPreference.Performance, CustomerPreference.Features };

                case TrendType.EnterpriseComplianceWave:
                    return new List<CustomerPreference> { CustomerPreference.Security, CustomerPreference.Support };

                default:
                    return new List<CustomerPreference>();
            }
        }
    }
}
