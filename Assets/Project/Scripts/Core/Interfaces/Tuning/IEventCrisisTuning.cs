namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the event and crisis system.
    /// Consumed by EventCrisisService and EventCrisisTickProcessor without depending on Infrastructure.
    /// Implemented by TuningConfig in Infrastructure via ITuningConfig.
    /// All prototype values are [Placeholder] — update as GDD_15 values are confirmed.
    /// Defined in Plan 2M, GDD_15.
    /// </summary>
    public interface IEventCrisisTuning
    {
        // ─── Scheduling ───────────────────────────────────────────────────────────

        /// <summary>
        /// How many game days must elapse between random event checks (e.g. market shock rolls).
        /// Prototype: 7.
        /// </summary>
        int EventCheckIntervalDays { get; }

        /// <summary>
        /// Minimum game days that must elapse between any two random events.
        /// Does not apply to threshold events — those always surface urgent conditions.
        /// Prototype: 14.
        /// </summary>
        int GlobalEventCooldownDays { get; }

        // ─── Market Shock ─────────────────────────────────────────────────────────

        /// <summary>
        /// Base probability (0–100) that a market shock event fires on each event check.
        /// Prototype: 10 (10% chance per check).
        /// </summary>
        int MarketShockProbabilityPercent { get; }

        /// <summary>
        /// Minimum demand shift magnitude for a market shock, in basis points.
        /// Prototype: 500 (5%).
        /// </summary>
        int MarketShockMinDemandShiftBasisPoints { get; }

        /// <summary>
        /// Maximum demand shift magnitude for a market shock, in basis points.
        /// Prototype: 2000 (20%).
        /// </summary>
        int MarketShockMaxDemandShiftBasisPoints { get; }

        // ─── Team Morale Crisis ───────────────────────────────────────────────────

        /// <summary>
        /// Team morale level (0–100) at or below which the team morale crisis fires.
        /// Prototype: 15.
        /// </summary>
        int TeamMoraleCrisisThreshold { get; }

        /// <summary>
        /// Additional morale penalty applied in basis points when a morale crisis fires.
        /// Prototype: -500 (equivalent to -5 morale points).
        /// </summary>
        int MoraleCrisisPenaltyBasisPoints { get; }

        // ─── Hardware Defect Spike ────────────────────────────────────────────────

        /// <summary>
        /// Hardware defect rate in basis points at or above which a defect spike crisis fires.
        /// Prototype: 500 (5%).
        /// </summary>
        int HardwareDefectSpikeThresholdBasisPoints { get; }

        /// <summary>
        /// Additional defect rate increase applied in basis points when a defect spike fires.
        /// Prototype: 200 (+2%).
        /// </summary>
        int DefectSpikeIncreaseBasisPoints { get; }
    }
}
