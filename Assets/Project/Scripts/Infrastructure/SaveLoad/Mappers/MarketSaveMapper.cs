using System;
using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Market;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Market domain runtime types to and from their save data equivalents.
    /// Covers CompetitorProfile, CompetitorRuntimeState, MarketCategoryRuntimeState,
    /// and TrendRuntimeState.
    /// All methods are static — this mapper holds no state.
    /// Dictionary enum keys convert to string keys using enum member names.
    /// Nullable GameDateTime fields convert to nullable int via TotalElapsedHours.
    /// </summary>
    public static class MarketSaveMapper
    {
        // ─── CompetitorProfile ────────────────────────────────────────────────────

        public static CompetitorSaveData ToSaveData(CompetitorProfile profile)
        {
            var marketFocus = new List<string>(profile.MarketFocus.Count);
            foreach (MarketCategoryType cat in profile.MarketFocus)
            {
                marketFocus.Add(cat.ToString());
            }

            return new CompetitorSaveData
            {
                Id            = profile.Id,
                Name          = profile.Name,
                Archetype     = profile.Archetype.ToString(),
                MarketFocus   = marketFocus,
                RiskAppetite  = profile.RiskAppetite,
                PricingStyle  = profile.PricingStyle.ToString(),
                MarketingStyle = profile.MarketingStyle.ToString()
            };
        }

        public static CompetitorProfile FromSaveData(CompetitorSaveData data)
        {
            var marketFocus = new List<MarketCategoryType>(data.MarketFocus.Count);
            foreach (string catStr in data.MarketFocus)
            {
                marketFocus.Add(Enum.Parse<MarketCategoryType>(catStr));
            }

            return new CompetitorProfile
            {
                Id             = data.Id,
                Name           = data.Name,
                Archetype      = Enum.Parse<CompetitorArchetype>(data.Archetype),
                MarketFocus    = marketFocus,
                RiskAppetite   = data.RiskAppetite,
                PricingStyle   = Enum.Parse<CompetitorPricingStyle>(data.PricingStyle),
                MarketingStyle = Enum.Parse<CompetitorMarketingStyle>(data.MarketingStyle)
            };
        }

        // ─── CompetitorRuntimeState ───────────────────────────────────────────────

        public static CompetitorStateSaveData ToSaveData(CompetitorRuntimeState state)
        {
            var marketShare = new Dictionary<string, int>(state.MarketShareBasisPoints.Count);
            foreach (KeyValuePair<MarketCategoryType, int> pair in state.MarketShareBasisPoints)
            {
                marketShare[pair.Key.ToString()] = pair.Value;
            }

            return new CompetitorStateSaveData
            {
                CompetitorId          = state.CompetitorId,
                CashStrength          = state.CashStrength,
                Reputation            = state.Reputation,
                ProductIds            = state.ProductIds,
                HiringStrength        = state.HiringStrength,
                ResearchStrength      = state.ResearchStrength,
                LaunchCadence         = state.LaunchCadence,
                MarketShareBasisPoints = marketShare
            };
        }

        public static CompetitorRuntimeState FromSaveData(CompetitorStateSaveData data)
        {
            var marketShare = new Dictionary<MarketCategoryType, int>(data.MarketShareBasisPoints.Count);
            foreach (KeyValuePair<string, int> pair in data.MarketShareBasisPoints)
            {
                marketShare[Enum.Parse<MarketCategoryType>(pair.Key)] = pair.Value;
            }

            return new CompetitorRuntimeState
            {
                CompetitorId           = data.CompetitorId,
                CashStrength           = data.CashStrength,
                Reputation             = data.Reputation,
                ProductIds             = data.ProductIds,
                HiringStrength         = data.HiringStrength,
                ResearchStrength       = data.ResearchStrength,
                LaunchCadence          = data.LaunchCadence,
                MarketShareBasisPoints = marketShare
            };
        }

        // ─── MarketCategoryRuntimeState ───────────────────────────────────────────

        public static MarketCategorySaveData ToSaveData(MarketCategoryRuntimeState state)
        {
            var segmentWeights = new Dictionary<string, int>(state.SegmentDemandWeights.Count);
            foreach (KeyValuePair<CustomerSegment, int> pair in state.SegmentDemandWeights)
            {
                segmentWeights[pair.Key.ToString()] = pair.Value;
            }

            var customerPrefs = new Dictionary<string, int>(state.CustomerPreferences.Count);
            foreach (KeyValuePair<CustomerPreference, int> pair in state.CustomerPreferences)
            {
                customerPrefs[pair.Key.ToString()] = pair.Value;
            }

            return new MarketCategorySaveData
            {
                Id                     = state.Id,
                CategoryType           = state.CategoryType.ToString(),
                TotalDemand            = state.TotalDemand,
                GrowthRateBasisPoints  = state.GrowthRateBasisPoints,
                SegmentDemandWeights   = segmentWeights,
                CustomerPreferences    = customerPrefs,
                CompetitiveIntensity   = state.CompetitiveIntensity,
                LeaderProductIds       = state.LeaderProductIds,
                TechnologyExpectation  = state.TechnologyExpectation,
                PriceSensitivity       = state.PriceSensitivity,
                MarketingSensitivity   = state.MarketingSensitivity,
                SupportExpectation     = state.SupportExpectation,
                ActiveTrendIds         = state.ActiveTrendIds
            };
        }

        public static MarketCategoryRuntimeState FromSaveData(MarketCategorySaveData data)
        {
            var segmentWeights = new Dictionary<CustomerSegment, int>(data.SegmentDemandWeights.Count);
            foreach (KeyValuePair<string, int> pair in data.SegmentDemandWeights)
            {
                segmentWeights[Enum.Parse<CustomerSegment>(pair.Key)] = pair.Value;
            }

            var customerPrefs = new Dictionary<CustomerPreference, int>(data.CustomerPreferences.Count);
            foreach (KeyValuePair<string, int> pair in data.CustomerPreferences)
            {
                customerPrefs[Enum.Parse<CustomerPreference>(pair.Key)] = pair.Value;
            }

            return new MarketCategoryRuntimeState
            {
                Id                    = data.Id,
                CategoryType          = Enum.Parse<MarketCategoryType>(data.CategoryType),
                TotalDemand           = data.TotalDemand,
                GrowthRateBasisPoints = data.GrowthRateBasisPoints,
                SegmentDemandWeights  = segmentWeights,
                CustomerPreferences   = customerPrefs,
                CompetitiveIntensity  = data.CompetitiveIntensity,
                LeaderProductIds      = data.LeaderProductIds,
                TechnologyExpectation = data.TechnologyExpectation,
                PriceSensitivity      = data.PriceSensitivity,
                MarketingSensitivity  = data.MarketingSensitivity,
                SupportExpectation    = data.SupportExpectation,
                ActiveTrendIds        = data.ActiveTrendIds
            };
        }

        // ─── TrendRuntimeState ────────────────────────────────────────────────────

        public static TrendSaveData ToSaveData(TrendRuntimeState state)
        {
            var affectedCategories = new List<string>(state.AffectedCategories.Count);
            foreach (MarketCategoryType cat in state.AffectedCategories)
            {
                affectedCategories.Add(cat.ToString());
            }

            // EstimatedEndDate is nullable — default (zero TotalElapsedHours) means no end date.
            int? estimatedEndHours = state.EstimatedEndDate.TotalElapsedHours == 0
                ? (int?)null
                : state.EstimatedEndDate.TotalElapsedHours;

            return new TrendSaveData
            {
                Id                                = state.Id,
                Type                              = state.Type.ToString(),
                Strength                          = state.Strength,
                AffectedCategories                = affectedCategories,
                StartDateTotalElapsedHours        = state.StartDate.TotalElapsedHours,
                EstimatedEndDateTotalElapsedHours = estimatedEndHours,
                IsActive                          = state.IsActive
            };
        }

        public static TrendRuntimeState FromSaveData(TrendSaveData data)
        {
            var affectedCategories = new List<MarketCategoryType>(data.AffectedCategories.Count);
            foreach (string catStr in data.AffectedCategories)
            {
                affectedCategories.Add(Enum.Parse<MarketCategoryType>(catStr));
            }

            GameDateTime estimatedEndDate = data.EstimatedEndDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.EstimatedEndDateTotalElapsedHours.Value)
                : default;

            return new TrendRuntimeState
            {
                Id                  = data.Id,
                Type                = Enum.Parse<TrendType>(data.Type),
                Strength            = data.Strength,
                AffectedCategories  = affectedCategories,
                StartDate           = GameDateTime.FromTotalHours(data.StartDateTotalElapsedHours),
                EstimatedEndDate    = estimatedEndDate,
                IsActive            = data.IsActive
            };
        }
    }
}
