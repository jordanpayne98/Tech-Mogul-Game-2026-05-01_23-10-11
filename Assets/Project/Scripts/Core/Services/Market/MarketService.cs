using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Market
{
    /// <summary>
    /// Stateless domain service for market simulation.
    /// Implements demand adjustment, trend lifecycle, preference drift, expectation growth,
    /// and market category initialization.
    /// Seeded System.Random is always passed in from the caller — never stored.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public sealed class MarketService : IMarketService
    {
        // ── Demand ────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ComputeWeeklyDemandShift(
            MarketCategoryRuntimeState category,
            List<TrendRuntimeState>    activeTrends,
            IMarketTuning              tuning,
            Random                     random)
        {
            // formula.market.demand_shift
            double growthFactor = category.GrowthRateBasisPoints / 10000.0;
            int weeklyGrowth    = (int)(category.TotalDemand * growthFactor / 52.0);

            int volatility      = random.Next(-tuning.WeeklyDemandAdjustmentPercent,
                                              tuning.WeeklyDemandAdjustmentPercent + 1);
            int volatilityDelta = (int)(category.TotalDemand * volatility / 100.0 * tuning.DemandVolatilityRange);

            double trendEffect  = 0.0;
            if (activeTrends != null)
            {
                foreach (var trend in activeTrends)
                {
                    trendEffect += trend.Strength * tuning.TrendImpactMultiplier / 100.0;
                }
            }

            int trendDelta = (int)(category.TotalDemand * trendEffect / 100.0 / 52.0);

            return Math.Max(
                tuning.MinDemandPerCategory,
                category.TotalDemand + weeklyGrowth + volatilityDelta + trendDelta);
        }

        // ── Trends ────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public TrendRuntimeState GenerateTrend(
            List<TrendRuntimeState>  existingTrends,
            IMarketTuning            tuning,
            GameDateTime             currentDate,
            Random                   random,
            List<MarketCategoryType> allCategories)
        {
            // Guard: at capacity
            int activeCount = existingTrends?.Count(t => t.IsActive) ?? 0;
            if (activeCount >= tuning.MaxActiveTrends)
            {
                return null;
            }

            // Collect TrendType values not already held by an active trend
            var activeTrendTypes = new HashSet<TrendType>(
                existingTrends?.Where(t => t.IsActive).Select(t => t.Type)
                ?? Enumerable.Empty<TrendType>());

            var allTrendTypes = (TrendType[])Enum.GetValues(typeof(TrendType));
            var availableTypes = allTrendTypes.Where(t => !activeTrendTypes.Contains(t)).ToList();

            if (availableTypes.Count == 0)
            {
                return null;
            }

            // Pick random type, strength, and duration
            TrendType selectedType = availableTypes[random.Next(availableTypes.Count)];

            int initialStrength   = random.Next(tuning.TrendMinStrength, tuning.TrendMaxStrength + 1);
            int durationMonths    = random.Next(tuning.TrendMinDurationMonths, tuning.TrendMaxDurationMonths + 1);

            // Approximate month = 30 days. GameDateTime.AddHours is the only arithmetic available.
            GameDateTime estimatedEndDate = currentDate.AddHours(durationMonths * GameDateTime.DaysPerMonth * GameDateTime.HoursPerDay);

            // Pick affected categories
            int affectedCount = random.Next(
                tuning.TrendMinAffectedCategories,
                Math.Min(tuning.TrendMaxAffectedCategories + 1, (allCategories?.Count ?? 0) + 1));

            var selectedCategories = PickDistinct(allCategories, affectedCount, random);

            string guid = Guid.NewGuid().ToString("N");

            return new TrendRuntimeState
            {
                Id                = guid,
                Type              = selectedType,
                InitialStrength   = initialStrength,
                Strength          = initialStrength,
                AffectedCategories = selectedCategories,
                StartDate         = currentDate,
                EstimatedEndDate  = estimatedEndDate,
                IsActive          = true
            };
        }

        /// <inheritdoc/>
        public int ComputeTrendDecay(TrendRuntimeState trend, GameDateTime currentDate)
        {
            // formula.market.trend_decay
            if (trend.EstimatedEndDate == null || trend.StartDate == null)
            {
                return 0;
            }

            int totalDays     = (trend.EstimatedEndDate.TotalElapsedHours - trend.StartDate.TotalElapsedHours) / GameDateTime.HoursPerDay;
            int remainingDays = (trend.EstimatedEndDate.TotalElapsedHours - currentDate.TotalElapsedHours) / GameDateTime.HoursPerDay;

            if (totalDays <= 0)
            {
                return 0;
            }

            float decayRatio = Math.Clamp(remainingDays / (float)totalDays, 0f, 1f);
            int newStrength  = (int)Math.Floor(trend.InitialStrength * decayRatio);

            return Math.Clamp(newStrength, 0, trend.InitialStrength);
        }

        /// <inheritdoc/>
        public bool IsTrendExpired(TrendRuntimeState trend, GameDateTime currentDate)
        {
            if (trend.EstimatedEndDate == null)
            {
                return false;
            }

            return currentDate.TotalElapsedHours >= trend.EstimatedEndDate.TotalElapsedHours;
        }

        /// <inheritdoc/>
        public List<TrendRuntimeState> GetActiveTrendsForCategory(
            List<TrendRuntimeState> allTrends,
            MarketCategoryType      category)
        {
            if (allTrends == null)
            {
                return new List<TrendRuntimeState>();
            }

            return allTrends
                .Where(t => t.IsActive && t.AffectedCategories != null && t.AffectedCategories.Contains(category))
                .ToList();
        }

        // ── Preferences ───────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void ComputePreferenceDrift(
            MarketCategoryRuntimeState category,
            List<TrendRuntimeState>    activeTrends,
            IMarketTuning              tuning,
            Random                     random)
        {
            if (category.CustomerPreferences == null || category.CustomerPreferences.Count == 0)
            {
                return;
            }

            var allPreferences = new List<CustomerPreference>((CustomerPreference[])Enum.GetValues(typeof(CustomerPreference)));

            // Pick 1–2 dimensions to boost
            int boostCount     = random.Next(1, 3); // [1, 2]
            var boostedPicks   = PickDistinct(allPreferences, boostCount, random);

            // Compute gain amounts per dimension, and track totals to subtract from others
            var gainAmounts = new List<int>();
            foreach (var _ in boostedPicks)
            {
                gainAmounts.Add(random.Next(1, tuning.PreferenceDriftRange + 1));
            }

            // Boost selected preferences
            for (int i = 0; i < boostedPicks.Count; i++)
            {
                if (category.CustomerPreferences.ContainsKey(boostedPicks[i]))
                {
                    category.CustomerPreferences[boostedPicks[i]] =
                        Math.Clamp(category.CustomerPreferences[boostedPicks[i]] + gainAmounts[i], 0, 100);
                }
            }

            // Reduce the same number of different preferences by the same amounts
            var remainingPreferences = allPreferences.Where(p => !boostedPicks.Contains(p)).ToList();
            var reducedPicks         = PickDistinct(remainingPreferences, Math.Min(boostCount, remainingPreferences.Count), random);

            for (int i = 0; i < reducedPicks.Count; i++)
            {
                int reduceAmount = i < gainAmounts.Count ? gainAmounts[i] : gainAmounts[gainAmounts.Count - 1];

                if (category.CustomerPreferences.ContainsKey(reducedPicks[i]))
                {
                    category.CustomerPreferences[reducedPicks[i]] =
                        Math.Clamp(category.CustomerPreferences[reducedPicks[i]] - reduceAmount, 0, 100);
                }
            }

            // Apply trend boosts (additive, intentionally not balanced)
            if (activeTrends != null)
            {
                foreach (var trend in activeTrends)
                {
                    int boost = (int)(trend.Strength * tuning.TrendImpactMultiplier / 100.0);
                    if (boost <= 0)
                    {
                        continue;
                    }

                    var boostedPrefs = TrendPreferenceMap.GetBoostedPreferences(trend.Type);
                    foreach (var pref in boostedPrefs)
                    {
                        if (category.CustomerPreferences.ContainsKey(pref))
                        {
                            category.CustomerPreferences[pref] =
                                Math.Clamp(category.CustomerPreferences[pref] + boost, 0, 100);
                        }
                    }
                }
            }
        }

        // ── Expectations ──────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void ComputeExpectationGrowth(MarketCategoryRuntimeState category, IMarketTuning tuning)
        {
            // formula.market.expectation_growth
            category.TechnologyExpectation = Math.Min(100, category.TechnologyExpectation + tuning.ExpectationGrowthPerMonth);
            category.PriceSensitivity      = Math.Min(100, category.PriceSensitivity      + tuning.ExpectationGrowthPerMonth);
            category.MarketingSensitivity  = Math.Min(100, category.MarketingSensitivity  + tuning.ExpectationGrowthPerMonth);
            category.SupportExpectation    = Math.Min(100, category.SupportExpectation    + tuning.ExpectationGrowthPerMonth);
        }

        // ── Initialization ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public MarketCategoryRuntimeState CreateMarketCategoryState(
            MarketCategoryType categoryType,
            IMarketTuning      tuning)
        {
            string id = MarketCategoryIdMap.GetId(categoryType);

            // Initialize equal segment demand weights (10 per segment)
            var segmentWeights = new Dictionary<CustomerSegment, int>();
            foreach (CustomerSegment segment in Enum.GetValues(typeof(CustomerSegment)))
            {
                segmentWeights[segment] = 10;
            }

            // Initialize equal customer preferences (50 per dimension)
            var customerPreferences = new Dictionary<CustomerPreference, int>();
            foreach (CustomerPreference preference in Enum.GetValues(typeof(CustomerPreference)))
            {
                customerPreferences[preference] = 50;
            }

            return new MarketCategoryRuntimeState
            {
                Id                    = id,
                CategoryType          = categoryType,
                TotalDemand           = tuning.DefaultDemand,
                GrowthRateBasisPoints = tuning.DefaultGrowthRateBasisPoints,
                SegmentDemandWeights  = segmentWeights,
                CustomerPreferences   = customerPreferences,
                CompetitiveIntensity  = 0,
                LeaderProductIds      = new List<string>(),
                TechnologyExpectation = tuning.DefaultTechnologyExpectation,
                PriceSensitivity      = tuning.DefaultPriceSensitivity,
                MarketingSensitivity  = tuning.DefaultMarketingSensitivity,
                SupportExpectation    = tuning.DefaultSupportExpectation,
                ActiveTrendIds        = new List<string>()
            };
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Returns a list of up to <paramref name="count"/> distinct randomly selected elements
        /// from the source list. Order is not preserved.
        /// </summary>
        private static List<T> PickDistinct<T>(List<T> source, int count, Random random)
        {
            if (source == null || source.Count == 0 || count <= 0)
            {
                return new List<T>();
            }

            // Fisher-Yates partial shuffle to select count distinct items
            var pool    = new List<T>(source);
            var result  = new List<T>(Math.Min(count, pool.Count));
            int pickMax = Math.Min(count, pool.Count);

            for (int i = 0; i < pickMax; i++)
            {
                int j           = random.Next(i, pool.Count);
                T   temp        = pool[i];
                pool[i]         = pool[j];
                pool[j]         = temp;
                result.Add(pool[i]);
            }

            return result;
        }
    }
}
