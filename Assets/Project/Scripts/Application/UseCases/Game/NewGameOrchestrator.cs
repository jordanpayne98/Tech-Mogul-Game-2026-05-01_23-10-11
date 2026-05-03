using System;
using Project.Core.Debugging;
using Project.Core.Interfaces;
using Project.Core.Results.Game;
using Project.Core.Runtime;
using Project.Core.Services;
using Project.Application.UseCases.Company;
using Project.Application.UseCases.Competitor;
using Project.Application.UseCases.Contract;
using Project.Application.UseCases.Employee;
using Project.Application.UseCases.Market;
using Project.Application.UseCases.Research;
using Project.Core.Requests.Company;

namespace Project.Application.UseCases.Game
{
    /// <summary>
    /// Application-layer coordinator for full new-game state initialisation.
    /// Creates a GameSessionState, runs all domain initialisation use cases in order,
    /// and returns the assembled session.
    ///
    /// Orchestration order (per Plan 2N, GDD_17.5, GDD_18):
    ///   1. Create company, founder, and shell states (CreateCompanyUseCase)
    ///   2. Attach SessionId and RandomState
    ///   3. Initialize recruitment pool (InitializeRecruitmentUseCase)
    ///   4. Initialize contract board (InitializeContractBoardUseCase)
    ///   5. Initialize market categories (InitializeMarketUseCase)
    ///   6. Initialize competitors (InitializeCompetitorsUseCase)
    ///   7. Initialize research portfolio (InitializeResearchUseCase)
    ///
    /// Does not perform save/load. Does not create UI state. Does not publish events
    /// beyond those already emitted by delegated use cases.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class NewGameOrchestrator
    {
        private readonly CreateCompanyUseCase          _createCompany;
        private readonly InitializeRecruitmentUseCase  _initializeRecruitment;
        private readonly InitializeContractBoardUseCase _initializeContractBoard;
        private readonly InitializeMarketUseCase       _initializeMarket;
        private readonly InitializeCompetitorsUseCase  _initializeCompetitors;
        private readonly InitializeResearchUseCase     _initializeResearch;
        private readonly IEventBus                     _eventBus;

        // ─── Constructor ──────────────────────────────────────────────────────────

        public NewGameOrchestrator(
            CreateCompanyUseCase           createCompany,
            InitializeRecruitmentUseCase   initializeRecruitment,
            InitializeContractBoardUseCase initializeContractBoard,
            InitializeMarketUseCase        initializeMarket,
            InitializeCompetitorsUseCase   initializeCompetitors,
            InitializeResearchUseCase      initializeResearch,
            IEventBus                      eventBus)
        {
            _createCompany          = createCompany          ?? throw new ArgumentNullException(nameof(createCompany));
            _initializeRecruitment  = initializeRecruitment  ?? throw new ArgumentNullException(nameof(initializeRecruitment));
            _initializeContractBoard = initializeContractBoard ?? throw new ArgumentNullException(nameof(initializeContractBoard));
            _initializeMarket       = initializeMarket       ?? throw new ArgumentNullException(nameof(initializeMarket));
            _initializeCompetitors  = initializeCompetitors  ?? throw new ArgumentNullException(nameof(initializeCompetitors));
            _initializeResearch     = initializeResearch     ?? throw new ArgumentNullException(nameof(initializeResearch));
            _eventBus               = eventBus               ?? throw new ArgumentNullException(nameof(eventBus));
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Executes the full new-game initialisation flow.
        /// </summary>
        /// <param name="companyRequest">Player-supplied company setup data.</param>
        /// <param name="randomSeed">Seed for deterministic simulation. Must be non-zero.</param>
        /// <returns>A result containing the fully initialised session state on success.</returns>
        public NewGameResult Execute(CreateCompanyRequest companyRequest, int randomSeed)
        {
            if (companyRequest == null)
            {
                return NewGameResult.Failed("CreateCompanyRequest is null.");
            }

            if (randomSeed == 0)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "[NewGameOrchestrator] randomSeed is 0 — deterministic save/load will not work correctly.");
            }

            // ─── 1. Create company, founder, and shell states ─────────────────────

            var createResult = _createCompany.Execute(companyRequest);

            if (!createResult.Success)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    $"[NewGameOrchestrator] CreateCompanyUseCase failed: {createResult.FailureReason}");
                return NewGameResult.Failed($"Company creation failed: {createResult.FailureReason}");
            }

            GameSessionState session = createResult.SessionState;

            // ─── 2. Attach session identity and random state ──────────────────────

            session.SessionId   = System.Guid.NewGuid().ToString();
            session.RandomState = new RandomRuntimeState { Seed = randomSeed, CallCount = 0 };

            var randomSource = new DeterministicRandomSource(session.RandomState);

            // Use the shared underlying System.Random for use cases that accept System.Random.
            // These are all called during new-game setup, so CallCount remains consistent.
            System.Random sharedRandom = randomSource.SystemRandom;

            // ─── 3. Initialise recruitment pool ───────────────────────────────────

            _initializeRecruitment.Execute(session, sharedRandom);

            // ─── 4. Initialise contract board ─────────────────────────────────────

            _initializeContractBoard.Execute(session, session.TimeState.CurrentDate, sharedRandom);

            // ─── 5. Initialise market ─────────────────────────────────────────────

            _initializeMarket.Execute(session);

            // ─── 6. Initialise competitors ────────────────────────────────────────

            _initializeCompetitors.Execute(session, session.TimeState.CurrentDate, sharedRandom);

            // ─── 7. Initialise research portfolio ─────────────────────────────────

            _initializeResearch.Execute(session);

            // ─── 8. Initialise event tracking fields ──────────────────────────────

            // LastEventDates, EventHistory are initialised by GameSessionState constructor.
            // LastGlobalEventDate and LastEventCheckDate default to null (treated as epoch).

            // ─── 9. Log and return ────────────────────────────────────────────────

            DebugLogger.Log(DebugCategory.Bootstrap,
                $"[NewGameOrchestrator] New game initialised. " +
                $"Company: {createResult.CompanyId} | " +
                $"Founder: {createResult.FounderId} | " +
                $"Seed: {randomSeed} | " +
                $"Candidates: {session.CandidateProfiles?.Count ?? 0} | " +
                $"Contracts: {session.ContractStates?.Count ?? 0} | " +
                $"Markets: {session.MarketCategoryStates?.Count ?? 0} | " +
                $"Competitors: {session.CompetitorStates?.Count ?? 0} | " +
                $"Research projects: {session.ResearchProjectStates?.Count ?? 0}");

            return NewGameResult.Succeeded(session);
        }
    }
}
