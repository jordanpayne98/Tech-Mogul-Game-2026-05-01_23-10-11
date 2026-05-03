using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime;
using Project.Core.Runtime.Market;

namespace Project.Application.UseCases.Market
{
    /// <summary>
    /// Application use case that sets up the initial market state for a new game session.
    /// Creates one MarketCategoryRuntimeState per MarketCategoryType enum value (7 for MVP)
    /// and clears the TrendStates list.
    ///
    /// Must be called after CreateCompanyUseCase and before the first simulation tick.
    /// Caller orchestration is deferred to Plan 2N.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public sealed class InitializeMarketUseCase
    {
        private readonly IMarketService _marketService;
        private readonly IMarketTuning  _tuning;

        public InitializeMarketUseCase(IMarketService marketService, IMarketTuning tuning)
        {
            _marketService = marketService;
            _tuning        = tuning;
        }

        /// <summary>
        /// Initializes all market category states and clears trend history on the given session state.
        /// </summary>
        /// <param name="sessionState">The active game session state to initialize market data on.</param>
        public void Execute(GameSessionState sessionState)
        {
            sessionState.MarketCategoryStates = new List<MarketCategoryRuntimeState>();
            sessionState.TrendStates          = new List<TrendRuntimeState>();

            foreach (MarketCategoryType categoryType in Enum.GetValues(typeof(MarketCategoryType)))
            {
                MarketCategoryRuntimeState categoryState = _marketService.CreateMarketCategoryState(categoryType, _tuning);
                sessionState.MarketCategoryStates.Add(categoryState);
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[InitializeMarketUseCase] Initialized {sessionState.MarketCategoryStates.Count} market categories.");
        }
    }
}
