namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the competitor simulation system.
    /// Consumed by CompetitorService, CompetitorTickProcessor, and InitializeCompetitorsUseCase
    /// without depending on Infrastructure.
    /// All prototype values are [Placeholder] — update as GDD_10.5–10.7 values are confirmed.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public interface ICompetitorTuning
    {
        // ── Action Frequency ──────────────────────────────────────────────────────

        /// <summary>
        /// Probability (0–100) that a competitor attempts an action in a given week.
        /// Rolled first before action type selection.
        /// Prototype: 25.
        /// </summary>
        int CompetitorActionChancePerWeekPercent { get; }

        /// <summary>
        /// Probability (0–100) that a competitor passes the monthly launch chance gate.
        /// Must also pass cadence cooldown for the launch to proceed.
        /// Prototype: 15.
        /// </summary>
        int CompetitorLaunchChancePerMonthPercent { get; }

        // ── Price Changes ─────────────────────────────────────────────────────────

        /// <summary>
        /// Maximum percentage (positive or negative) by which a competitor may change a product price.
        /// Used as: random.Next(-range, range + 1) → changePercent.
        /// Prototype: 10.
        /// </summary>
        int CompetitorPriceChangeRangePercent { get; }

        // ── Marketing ─────────────────────────────────────────────────────────────

        /// <summary>
        /// [Placeholder] Multiplier applied to competitor marketing effectiveness calculations.
        /// Reserved for future use. Prototype: 1.0.
        /// </summary>
        float CompetitorMarketingEffectiveness { get; }

        // ── Initialization ────────────────────────────────────────────────────────

        /// <summary>
        /// Number of AI competitors generated at session start.
        /// Prototype: 4.
        /// </summary>
        int InitialCompetitorCount { get; }

        /// <summary>
        /// Number of products each competitor starts with.
        /// Prototype: 1.
        /// </summary>
        int CompetitorInitialProductCount { get; }

        // ── Launch Quality / Pricing ──────────────────────────────────────────────

        /// <summary>
        /// Minimum base quality (inclusive) rolled when a competitor launches a product.
        /// Prototype: 30.
        /// </summary>
        int CompetitorBaseQualityMin { get; }

        /// <summary>
        /// Maximum base quality (inclusive) rolled when a competitor launches a product.
        /// Prototype: 70.
        /// </summary>
        int CompetitorBaseQualityMax { get; }

        /// <summary>
        /// Minimum base price in minor currency units rolled at launch.
        /// Stored as major units in Inspector; exposed as minor units (x100).
        /// Prototype: 500_000 (£5,000).
        /// </summary>
        long CompetitorBasePriceMinMinorUnits { get; }

        /// <summary>
        /// Maximum base price in minor currency units rolled at launch.
        /// Stored as major units in Inspector; exposed as minor units (x100).
        /// Prototype: 5_000_000 (£50,000).
        /// </summary>
        long CompetitorBasePriceMaxMinorUnits { get; }

        /// <summary>
        /// Price multiplier applied when a competitor uses the Premium pricing style.
        /// Prototype: 1.5.
        /// </summary>
        float CompetitorPremiumPriceMultiplier { get; }

        /// <summary>
        /// Price multiplier applied when a competitor uses the LowCost pricing style.
        /// Also used (x0.8) for AggressiveDiscounting.
        /// Prototype: 0.6.
        /// </summary>
        float CompetitorLowCostPriceMultiplier { get; }

        /// <summary>
        /// Floor price in minor currency units. Competitor product price never falls below this.
        /// Stored as major units in Inspector; exposed as minor units (x100).
        /// Prototype: 100_000 (£1,000).
        /// </summary>
        long CompetitorMinPriceMinorUnits { get; }

        // ── Market Share ──────────────────────────────────────────────────────────

        /// <summary>
        /// Weight applied to quality score in the market share formula.
        /// totalWeight = QualityWeight + PriceWeight + MarketingWeight.
        /// Prototype: 40.
        /// </summary>
        int MarketShareQualityWeight { get; }

        /// <summary>
        /// Weight applied to price score (100 - pricePercentile) in the market share formula.
        /// Prototype: 30.
        /// </summary>
        int MarketSharePriceWeight { get; }

        /// <summary>
        /// Weight applied to marketing value in the market share formula.
        /// Prototype: 30.
        /// </summary>
        int MarketShareMarketingWeight { get; }

        /// <summary>
        /// CompetitiveIntensity points added per active product in a market category.
        /// Result is clamped to [0, 100].
        /// Prototype: 10.
        /// </summary>
        int IntensityPerProduct { get; }

        // ── Action Weights ────────────────────────────────────────────────────────

        /// <summary>
        /// Relative weight of the LaunchProduct action in the weighted action roll.
        /// Prototype: 50.
        /// </summary>
        int CompetitorLaunchChanceWeight { get; }

        /// <summary>
        /// Relative weight of the ChangePrice action in the weighted action roll.
        /// Prototype: 30.
        /// </summary>
        int CompetitorPriceChangeChanceWeight { get; }

        /// <summary>
        /// Relative weight of the EnterMarket action in the weighted action roll.
        /// Prototype: 20.
        /// </summary>
        int CompetitorExpandChanceWeight { get; }
    }
}
