using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Event;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Team;

namespace Project.Core.Services.Event
{
    /// <summary>
    /// Stateless Core service that evaluates event eligibility and computes effects.
    /// Does not mutate any runtime state — returns computed GameEventEffect instances
    /// for the Application layer (EventCrisisTickProcessor) to apply.
    ///
    /// Three MVP evaluators:
    ///   EvaluateMarketShock          — probability-based demand shift on a random market category
    ///   EvaluateTeamMoraleCrises     — threshold-based morale penalty per qualifying team
    ///   EvaluateHardwareDefectSpikes — threshold-based defect rate increase per qualifying product
    ///
    /// Defined in Plan 2M, GDD_15.
    /// </summary>
    public sealed class EventCrisisService : IEventCrisisService
    {
        // ─── IEventCrisisService ──────────────────────────────────────────────────

        /// <inheritdoc/>
        public GameEventEffect EvaluateMarketShock(
            IReadOnlyList<MarketCategoryRuntimeState> marketStates,
            System.Random random,
            IEventCrisisTuning tuning)
        {
            if (marketStates == null || marketStates.Count == 0)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    "[EventCrisisService] EvaluateMarketShock: no market states available, skipping.");
                return null;
            }

            // Roll against the probability threshold.
            int roll = random.Next(100);
            if (roll >= tuning.MarketShockProbabilityPercent)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    $"[EventCrisisService] Market shock roll {roll} did not beat threshold {tuning.MarketShockProbabilityPercent}. No shock.");
                return null;
            }

            // Select a random market category.
            int categoryIndex = random.Next(marketStates.Count);
            MarketCategoryRuntimeState selectedCategory = marketStates[categoryIndex];

            // Compute demand shift magnitude in basis points.
            int shiftMagnitude = random.Next(
                tuning.MarketShockMinDemandShiftBasisPoints,
                tuning.MarketShockMaxDemandShiftBasisPoints + 1);

            // 50% chance the shift is negative (demand drop).
            if (random.Next(2) == 0)
            {
                shiftMagnitude = -shiftMagnitude;
            }

            string direction = shiftMagnitude >= 0 ? "increase" : "decrease";
            string description = $"{selectedCategory.CategoryType} demand {direction} of {shiftMagnitude / 100}% due to market shock.";

            DebugLogger.Log(DebugCategory.Simulation,
                $"[EventCrisisService] Market shock fired. Category: {selectedCategory.CategoryType}, Shift: {shiftMagnitude} bps.");

            return new GameEventEffect(
                type:             GameEventEffectType.ModifyDemand,
                targetEntityId:   selectedCategory.Id,
                valueBasisPoints: shiftMagnitude,
                description:      description);
        }

        /// <inheritdoc/>
        public List<GameEventEffect> EvaluateTeamMoraleCrises(
            IReadOnlyList<TeamRuntimeState> teamStates,
            IEventCrisisTuning tuning)
        {
            var effects = new List<GameEventEffect>();

            if (teamStates == null)
            {
                return effects;
            }

            foreach (TeamRuntimeState team in teamStates)
            {
                if (team.Morale <= tuning.TeamMoraleCrisisThreshold)
                {
                    string description =
                        $"Team morale crisis penalty of {tuning.MoraleCrisisPenaltyBasisPoints / 100} morale points applied.";

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[EventCrisisService] Team morale crisis triggered. TeamId: {team.TeamId}, Morale: {team.Morale}.");

                    effects.Add(new GameEventEffect(
                        type:             GameEventEffectType.ModifyTeamMorale,
                        targetEntityId:   team.TeamId,
                        valueBasisPoints: tuning.MoraleCrisisPenaltyBasisPoints,
                        description:      description));
                }
            }

            return effects;
        }

        /// <inheritdoc/>
        public List<GameEventEffect> EvaluateHardwareDefectSpikes(
            IReadOnlyList<HardwareRuntimeMetrics> hardwareMetrics,
            IEventCrisisTuning tuning)
        {
            var effects = new List<GameEventEffect>();

            if (hardwareMetrics == null)
            {
                return effects;
            }

            foreach (HardwareRuntimeMetrics metrics in hardwareMetrics)
            {
                if (metrics.DefectRateBasisPoints >= tuning.HardwareDefectSpikeThresholdBasisPoints)
                {
                    string description =
                        $"Hardware defect spike: defect rate increased by {tuning.DefectSpikeIncreaseBasisPoints} bps (+{tuning.DefectSpikeIncreaseBasisPoints / 100f:0.##}%).";

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[EventCrisisService] Hardware defect spike triggered. ProductId: {metrics.ProductId}, " +
                        $"DefectRate: {metrics.DefectRateBasisPoints} bps.");

                    effects.Add(new GameEventEffect(
                        type:             GameEventEffectType.ModifyDefectRate,
                        targetEntityId:   metrics.ProductId,
                        valueBasisPoints: tuning.DefectSpikeIncreaseBasisPoints,
                        description:      description));
                }
            }

            return effects;
        }
    }
}
