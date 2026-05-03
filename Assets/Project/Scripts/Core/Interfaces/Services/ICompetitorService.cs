using System;
using System.Collections.Generic;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Domain service interface for AI competitor simulation operations.
    /// Stateless — all state is passed as parameters. No state references held.
    /// Does not publish events.
    /// Consumed by CompetitorTickProcessor and InitializeCompetitorsUseCase
    /// without depending on Infrastructure.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public interface ICompetitorService
    {
        // ── Initialization ────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a competitor profile with GUID ID, random archetype, random market focus (1-2 categories),
        /// random pricing/marketing style, and archetype-driven defaults.
        /// </summary>
        CompetitorProfile GenerateCompetitorProfile(
            ICompetitorTuning           tuning,
            List<MarketCategoryType>    allCategories,
            Random                      random);

        /// <summary>
        /// Creates a competitor runtime state from the given profile using archetype defaults.
        /// </summary>
        CompetitorRuntimeState CreateCompetitorRuntimeState(CompetitorProfile profile);

        /// <summary>
        /// Creates a pre-launched product in the competitor's primary market focus category.
        /// Adds the product ID to state.ProductIds and sets state.LastLaunchDate.
        /// </summary>
        CompetitorProductRuntimeState GenerateInitialProduct(
            CompetitorProfile           profile,
            CompetitorRuntimeState      state,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random);

        // ── Action Selection ──────────────────────────────────────────────────────

        /// <summary>
        /// Rolls action chance and selects a weighted action type.
        /// Returns null if no action triggered (chance gate failed).
        /// </summary>
        CompetitorActionType? SelectAction(
            CompetitorRuntimeState      state,
            CompetitorProfile           profile,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random);

        // ── Launch ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Checks cadence cooldown AND monthly launch chance gate.
        /// Returns true only if both pass.
        /// </summary>
        bool CanLaunch(
            CompetitorRuntimeState      state,
            GameDateTime                currentDate,
            ICompetitorTuning           tuning,
            Random                      random);

        /// <summary>
        /// Applies launch_quality, launch_price, and launch_marketing formulas.
        /// Returns a new CompetitorProductRuntimeState. Does not mutate state.
        /// </summary>
        CompetitorProductRuntimeState LaunchProduct(
            CompetitorProfile           profile,
            CompetitorRuntimeState      state,
            ICompetitorTuning           tuning,
            GameDateTime                currentDate,
            Random                      random);

        // ── Price Change ──────────────────────────────────────────────────────────

        /// <summary>
        /// Selects a random active product owned by state and applies price_change formula.
        /// Returns (productId, newPrice), or null if no active products.
        /// </summary>
        (string productId, long newPrice)? ChangePrice(
            CompetitorRuntimeState                  state,
            List<CompetitorProductRuntimeState>     products,
            ICompetitorTuning                       tuning,
            Random                                  random);

        // ── Market Expansion ──────────────────────────────────────────────────────

        /// <summary>
        /// Picks a random category not in profile.MarketFocus.
        /// Returns null if already in all categories.
        /// </summary>
        MarketCategoryType? ExpandMarket(
            CompetitorProfile           profile,
            List<MarketCategoryType>    allCategories,
            Random                      random);

        // ── Market Share ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns price percentile (0-100).
        /// Lowest price → 0, highest → 100, single product → 50, all same → 50.
        /// </summary>
        int ComputePricePercentile(long priceMinorUnits, List<long> allPricesInCategory);

        /// <summary>
        /// Applies tuning-weighted market share score formula.
        /// Returns score as a float (not normalized).
        /// </summary>
        float ComputeProductScore(
            int                 qualityValue,
            int                 priceScore,
            int                 marketingValue,
            ICompetitorTuning   tuning);

        /// <summary>
        /// Largest-remainder allocation ensuring total basis points = 10000.
        /// Returns empty list if input is empty.
        /// </summary>
        List<(string productId, int shareBasisPoints)> DistributeShares(
            List<(string productId, float score)> productScores);
    }
}
