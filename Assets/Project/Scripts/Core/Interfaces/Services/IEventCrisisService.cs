using System.Collections.Generic;
using Project.Core.Definitions.Event;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Team;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Core service interface for event eligibility evaluation and effect computation.
    /// Implementations are pure and stateless: they receive state as parameters,
    /// compute effects, and return them for the caller to apply.
    /// No state is mutated inside this service.
    /// Defined in Plan 2M, GDD_15.
    /// </summary>
    public interface IEventCrisisService
    {
        /// <summary>
        /// Rolls for a market shock event and returns the computed demand effect if it fires.
        /// Returns null if the probability roll fails (no shock this check).
        /// Selects a random market category and computes a signed demand shift magnitude.
        /// </summary>
        /// <param name="marketStates">All current market category runtime states.</param>
        /// <param name="random">Seeded random instance for deterministic results.</param>
        /// <param name="tuning">Event crisis tuning parameters.</param>
        GameEventEffect EvaluateMarketShock(
            IReadOnlyList<MarketCategoryRuntimeState> marketStates,
            System.Random random,
            IEventCrisisTuning tuning);

        /// <summary>
        /// Checks all teams for morale crisis conditions.
        /// Returns one effect per team whose morale is at or below the crisis threshold.
        /// Returns an empty list if no team qualifies.
        /// </summary>
        /// <param name="teamStates">All current team runtime states.</param>
        /// <param name="tuning">Event crisis tuning parameters.</param>
        List<GameEventEffect> EvaluateTeamMoraleCrises(
            IReadOnlyList<TeamRuntimeState> teamStates,
            IEventCrisisTuning tuning);

        /// <summary>
        /// Checks all hardware products for defect spike conditions.
        /// Returns one effect per hardware product whose defect rate meets or exceeds the spike threshold.
        /// Returns an empty list if no product qualifies.
        /// </summary>
        /// <param name="hardwareMetrics">All current hardware product runtime metrics.</param>
        /// <param name="tuning">Event crisis tuning parameters.</param>
        List<GameEventEffect> EvaluateHardwareDefectSpikes(
            IReadOnlyList<HardwareRuntimeMetrics> hardwareMetrics,
            IEventCrisisTuning tuning);
    }
}
