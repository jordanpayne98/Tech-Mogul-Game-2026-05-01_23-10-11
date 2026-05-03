namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Tuning interface for the market simulation system.
    /// Covers demand, trends, preference drift, expectation growth, and initialization defaults.
    /// Implemented by TuningConfig in Infrastructure.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public interface IMarketTuning
    {
        // ── Demand ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Default total demand assigned to each market category on initialization.
        /// Arbitrary unit scale; downstream systems normalise on read.
        /// </summary>
        int DefaultDemand { get; }

        /// <summary>
        /// Default annual growth rate for each market category in basis points (0–10000, 10000 = 100%).
        /// 200 = 2% annual growth.
        /// </summary>
        int DefaultGrowthRateBasisPoints { get; }

        /// <summary>
        /// Multiplier applied to the random volatility roll when computing weekly demand shift.
        /// Range: 0–1. Higher = wider weekly demand swings.
        /// </summary>
        float DemandVolatilityRange { get; }

        /// <summary>
        /// Maximum percentage (positive or negative) of random volatility applied each week.
        /// Used as the bound for random.Next(-N, N+1) in the demand shift formula.
        /// </summary>
        int WeeklyDemandAdjustmentPercent { get; }

        /// <summary>
        /// Minimum demand a market category can fall to. Demand never drops below this floor.
        /// </summary>
        int MinDemandPerCategory { get; }

        // ── Trends ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Multiplier applied to trend strength when computing trend effects on demand and preferences.
        /// 1.0 = strength points are applied as-is.
        /// </summary>
        float TrendImpactMultiplier { get; }

        /// <summary>
        /// Probability (0–100) that a new trend spawns each month boundary check.
        /// Ignored if MaxActiveTrends is already reached.
        /// </summary>
        int TrendSpawnChancePerMonthPercent { get; }

        /// <summary>Minimum strength (inclusive) assigned to a newly generated trend.</summary>
        int TrendMinStrength { get; }

        /// <summary>Maximum strength (inclusive) assigned to a newly generated trend.</summary>
        int TrendMaxStrength { get; }

        /// <summary>Minimum duration in months (inclusive) for a newly generated trend.</summary>
        int TrendMinDurationMonths { get; }

        /// <summary>Maximum duration in months (inclusive) for a newly generated trend.</summary>
        int TrendMaxDurationMonths { get; }

        /// <summary>
        /// Maximum number of active trends allowed at any time.
        /// New trends will not spawn if this limit is reached.
        /// </summary>
        int MaxActiveTrends { get; }

        /// <summary>Minimum number of market categories (inclusive) a new trend can affect.</summary>
        int TrendMinAffectedCategories { get; }

        /// <summary>Maximum number of market categories (inclusive) a new trend can affect.</summary>
        int TrendMaxAffectedCategories { get; }

        // ── Preference and Expectation Drift ─────────────────────────────────────

        /// <summary>
        /// Maximum preference points gained or lost per monthly drift step.
        /// Range for each adjustment: [1, PreferenceDriftRange].
        /// </summary>
        int PreferenceDriftRange { get; }

        /// <summary>
        /// Points added to each expectation threshold (TechnologyExpectation, PriceSensitivity,
        /// MarketingSensitivity, SupportExpectation) per month. Clamped to 100.
        /// </summary>
        int ExpectationGrowthPerMonth { get; }

        // ── Initialization Defaults ───────────────────────────────────────────────

        /// <summary>Initial TechnologyExpectation value for all market categories.</summary>
        int DefaultTechnologyExpectation { get; }

        /// <summary>Initial PriceSensitivity value for all market categories.</summary>
        int DefaultPriceSensitivity { get; }

        /// <summary>Initial MarketingSensitivity value for all market categories.</summary>
        int DefaultMarketingSensitivity { get; }

        /// <summary>Initial SupportExpectation value for all market categories.</summary>
        int DefaultSupportExpectation { get; }
    }
}
