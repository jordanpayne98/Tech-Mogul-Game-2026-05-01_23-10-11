using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Domain service interface for market simulation.
    /// Stateless. No state references held. Does not publish events.
    /// Seeded System.Random is passed in from the caller (Application layer).
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public interface IMarketService
    {
        // ── Demand ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies formula.market.demand_shift to compute the new total demand for a market category.
        /// Takes weekly growth, random volatility, and active trend effects into account.
        /// Returns the new TotalDemand value, clamped to MinDemandPerCategory.
        /// </summary>
        int ComputeWeeklyDemandShift(
            MarketCategoryRuntimeState   category,
            List<TrendRuntimeState>      activeTrends,
            IMarketTuning                tuning,
            System.Random                random);

        // ── Trends ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Generates a new trend with a GUID ID, a random unused type, random strength,
        /// random duration, and random affected categories.
        /// Returns null if MaxActiveTrends is already reached or no available types remain.
        /// </summary>
        TrendRuntimeState GenerateTrend(
            List<TrendRuntimeState>   existingTrends,
            IMarketTuning             tuning,
            GameDateTime              currentDate,
            System.Random             random,
            List<MarketCategoryType>  allCategories);

        /// <summary>
        /// Applies formula.market.trend_decay to compute the new strength for a trend.
        /// Returns the new Strength value, clamped between 0 and InitialStrength.
        /// </summary>
        int ComputeTrendDecay(TrendRuntimeState trend, GameDateTime currentDate);

        /// <summary>
        /// Returns true when the given trend's EstimatedEndDate is at or before the current date.
        /// </summary>
        bool IsTrendExpired(TrendRuntimeState trend, GameDateTime currentDate);

        /// <summary>
        /// Filters the full trend list to trends that are active and affect the given category.
        /// </summary>
        List<TrendRuntimeState> GetActiveTrendsForCategory(
            List<TrendRuntimeState> allTrends,
            MarketCategoryType      category);

        // ── Preferences ───────────────────────────────────────────────────────────

        /// <summary>
        /// Applies formula.market.preference_drift to mutate the category's CustomerPreferences in place.
        /// Picks 1–2 random dimensions to boost, reduces others by equal amounts, then applies trend boosts.
        /// All values are clamped to [0, 100].
        /// </summary>
        void ComputePreferenceDrift(
            MarketCategoryRuntimeState category,
            List<TrendRuntimeState>    activeTrends,
            IMarketTuning              tuning,
            System.Random              random);

        // ── Expectations ──────────────────────────────────────────────────────────

        /// <summary>
        /// Applies formula.market.expectation_growth to mutate the category's expectation fields in place.
        /// Increments TechnologyExpectation, PriceSensitivity, MarketingSensitivity, and SupportExpectation
        /// by ExpectationGrowthPerMonth, clamped to 100.
        /// </summary>
        void ComputeExpectationGrowth(MarketCategoryRuntimeState category, IMarketTuning tuning);

        // ── Initialization ────────────────────────────────────────────────────────

        /// <summary>
        /// Creates an initial MarketCategoryRuntimeState with a deterministic semantic ID,
        /// default demand, equal segment and preference weights, and default expectation values.
        /// CompetitiveIntensity is 0. LeaderProductIds and ActiveTrendIds are empty lists.
        /// </summary>
        MarketCategoryRuntimeState CreateMarketCategoryState(
            MarketCategoryType categoryType,
            IMarketTuning      tuning);
    }
}
