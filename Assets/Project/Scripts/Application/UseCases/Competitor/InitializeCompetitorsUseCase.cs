using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Market;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Competitor
{
    /// <summary>
    /// Application use case that creates the initial set of AI competitors for a game session.
    /// Standalone — called after InitializeMarketUseCase, before first tick.
    /// Caller orchestration deferred to Plan 2N.
    /// Defined in Plan 2J, GDD_10.
    /// </summary>
    public sealed class InitializeCompetitorsUseCase
    {
        // ── Dependencies ──────────────────────────────────────────────────────────

        private readonly ICompetitorService _competitorService;
        private readonly ICompetitorTuning  _tuning;

        // ── Constructor ───────────────────────────────────────────────────────────

        public InitializeCompetitorsUseCase(
            ICompetitorService  competitorService,
            ICompetitorTuning   tuning)
        {
            _competitorService = competitorService ?? throw new ArgumentNullException(nameof(competitorService));
            _tuning            = tuning            ?? throw new ArgumentNullException(nameof(tuning));
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>
        /// Generates initial competitors and their starting products.
        /// Clears and repopulates CompetitorProfiles, CompetitorStates, and CompetitorProductStates.
        /// </summary>
        public void Execute(GameSessionState sessionState, GameDateTime currentDate, Random random)
        {
            if (sessionState == null) throw new ArgumentNullException(nameof(sessionState));
            if (currentDate  == null) throw new ArgumentNullException(nameof(currentDate));
            if (random       == null) throw new ArgumentNullException(nameof(random));

            sessionState.CompetitorProfiles      = new List<CompetitorProfile>();
            sessionState.CompetitorStates        = new List<CompetitorRuntimeState>();
            sessionState.CompetitorProductStates = new List<CompetitorProductRuntimeState>();

            var allCategories = new List<MarketCategoryType>(
                (MarketCategoryType[])Enum.GetValues(typeof(MarketCategoryType)));

            int productCount = 0;

            for (int i = 0; i < _tuning.InitialCompetitorCount; i++)
            {
                var profile = _competitorService.GenerateCompetitorProfile(_tuning, allCategories, random);
                var state   = _competitorService.CreateCompetitorRuntimeState(profile);

                sessionState.CompetitorProfiles.Add(profile);
                sessionState.CompetitorStates.Add(state);

                // Generate initial products.
                for (int p = 0; p < _tuning.CompetitorInitialProductCount; p++)
                {
                    if (profile.MarketFocus == null || profile.MarketFocus.Count == 0)
                    {
                        break;
                    }

                    var product = _competitorService.GenerateInitialProduct(profile, state, _tuning, currentDate, random);
                    sessionState.CompetitorProductStates.Add(product);
                    productCount++;
                }
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[InitializeCompetitorsUseCase] Initialized {sessionState.CompetitorProfiles.Count} competitors " +
                $"with {productCount} products.");
        }
    }
}
