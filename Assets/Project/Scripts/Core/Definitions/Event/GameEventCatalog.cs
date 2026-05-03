using System.Collections.Generic;
using Project.Core.Definitions.Time;

namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Static catalog of all registered game event definitions.
    /// Definitions are immutable and never modified after initialization.
    ///
    /// Pattern: Static class with IReadOnlyList&lt;T&gt; All and GetById(string).
    /// Consistent with the research definition catalog approach from Plan 2K.
    ///
    /// MVP events (Plan 2M):
    ///   event.market_shock          — random demand modifier
    ///   event.team_morale_crisis    — threshold: team morale drops critically
    ///   event.hardware_defect_spike — threshold: hardware defect rate exceeds threshold
    /// </summary>
    public static class GameEventCatalog
    {
        // ─── All registered definitions ───────────────────────────────────────────

        private static readonly List<GameEventDefinition> _all = new List<GameEventDefinition>
        {
            // ── Market Shock ──────────────────────────────────────────────────────
            // RandomProbability event that applies a sudden demand modifier to a random
            // market category. Backed by Plan 2I market state.
            new GameEventDefinition(
                eventId:             "event.market_shock",
                title:               "Market Shock",
                descriptionTemplate: "{CategoryName} demand shifted by {Percent}% due to an unexpected market shock.",
                category:            GameEventCategory.Market,
                severity:            GameEventSeverity.Moderate,
                triggerType:         GameEventTriggerType.RandomProbability,
                effectType:          GameEventEffectType.ModifyDemand,
                interruptsContinue:  true,
                interruptionType:    Definitions.Time.InterruptionType.MarketTrendShift,
                cooldownDays:        60,
                earliestGameDay:     30),

            // ── Team Morale Crisis ────────────────────────────────────────────────
            // Threshold-triggered event that fires when any team's morale drops to or
            // below the crisis threshold. Backed by Plan 2D team state and 2C morale.
            new GameEventDefinition(
                eventId:             "event.team_morale_crisis",
                title:               "Team Morale Crisis",
                descriptionTemplate: "Team {TeamName} has reached a morale crisis. Productivity will suffer until conditions improve.",
                category:            GameEventCategory.Team,
                severity:            GameEventSeverity.Major,
                triggerType:         GameEventTriggerType.ThresholdCheck,
                effectType:          GameEventEffectType.ModifyTeamMorale,
                interruptsContinue:  true,
                interruptionType:    Definitions.Time.InterruptionType.TeamMoraleCrisis,
                cooldownDays:        30,
                earliestGameDay:     14),

            // ── Hardware Defect Spike ─────────────────────────────────────────────
            // Threshold-triggered event that fires when any hardware product's defect
            // rate reaches or exceeds the spike threshold. Backed by Plan 2F metrics.
            new GameEventDefinition(
                eventId:             "event.hardware_defect_spike",
                title:               "Hardware Defect Spike",
                descriptionTemplate: "Product {ProductName} has experienced a defect rate spike. Quality control must be reviewed.",
                category:            GameEventCategory.Product,
                severity:            GameEventSeverity.Major,
                triggerType:         GameEventTriggerType.ThresholdCheck,
                effectType:          GameEventEffectType.ModifyDefectRate,
                interruptsContinue:  true,
                interruptionType:    Definitions.Time.InterruptionType.MajorDefect,
                cooldownDays:        45,
                earliestGameDay:     60)
        };

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>All registered game event definitions.</summary>
        public static IReadOnlyList<GameEventDefinition> All => _all;

        /// <summary>
        /// Looks up a game event definition by its stable ID.
        /// Returns null if no definition is registered with the given ID.
        /// </summary>
        /// <param name="eventId">Stable event ID (e.g. "event.market_shock").</param>
        public static GameEventDefinition GetById(string eventId)
        {
            foreach (GameEventDefinition definition in _all)
            {
                if (definition.EventId == eventId)
                {
                    return definition;
                }
            }

            return null;
        }
    }
}
