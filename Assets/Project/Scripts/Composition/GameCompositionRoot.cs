using System;
using System.Collections.Generic;
using Project.Application.TickProcessors;
using Project.Application.UseCases.Company;
using Project.Application.UseCases.Competitor;
using Project.Application.UseCases.Contract;
using Project.Application.UseCases.Employee;
using Project.Application.UseCases.Event;
using Project.Application.UseCases.Finance;
using Project.Application.UseCases.Game;
using Project.Application.UseCases.Market;
using Project.Application.UseCases.Product;
using Project.Application.UseCases.Report;
using Project.Application.UseCases.Research;
using Project.Application.UseCases.Team;
using Project.Application.UseCases.Time;
using Project.Application.Validation;
using Project.Core.Debugging;
using Project.Core.Events;
using Project.Core.Interfaces;
using Project.Core.Runtime;
using Project.Core.Services;
using Project.Core.Services.Company;
using Project.Core.Services.Competitor;
using Project.Core.Services.Contract;
using Project.Core.Services.Employee;
using Project.Core.Services.Finance;
using Project.Core.Services.Market;
using Project.Core.Services.Product;
using Project.Core.Services.Report;
using Project.Core.Services.Research;
using Project.Core.Services.Team;
using Project.Core.Services.Time;
using Project.Core.Services.Event;
using Project.Infrastructure.Tuning;

namespace Project.Composition
{
    /// <summary>
    /// Composition-layer class that performs manual constructor injection of all Core services,
    /// Application use cases, tick processors, and orchestrators.
    ///
    /// GameBootstrapper creates this class and calls Initialize(). All dependency wiring lives here,
    /// keeping the bootstrapper thin and keeping game rules out of the Composition layer.
    ///
    /// Session binding: after NewGameOrchestrator creates a session, call BindSession(session) to
    /// push the new session reference into all processors and handlers that need it.
    ///
    /// No DI framework is used. All dependencies are resolved via constructor injection in a fixed
    /// order following the dependency graph: Core services first, then Application, then orchestrators.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class GameCompositionRoot : IDisposable
    {
        private readonly TuningConfig _tuningConfig;

        // ─── Exposed for bootstrapper and debug tools ──────────────────────────────

        public NewGameOrchestrator    NewGameOrchestrator    { get; private set; }
        public ContinueOrchestrator   ContinueOrchestrator   { get; private set; }
        public GameSessionValidator   GameSessionValidator    { get; private set; }
        public GameSessionState       CurrentSession          { get; set; }
        public DeterministicRandomSource RandomSource         { get; private set; }
        public InMemoryEventBus       EventBus                { get; private set; }

        // ─── Internal state for session binding ───────────────────────────────────

        // Tick processors that hold a session reference need to be re-created per session.
        // Session-bound processors are rebuilt in BindSession().
        private TickCoordinator     _tickCoordinator;
        private InMemoryEventBus    _eventBus;
        private ReportEventHandler  _reportEventHandler;

        // Core services (stateless, shared across sessions)
        private TimeService                _timeService;
        private CompanyService             _companyService;
        private EmployeeService            _employeeService;
        private TeamService                _teamService;
        private ProductService             _productService;
        private HardwareMetricsService     _hardwareMetricsService;
        private ContractService            _contractService;
        private FinanceService             _financeService;
        private MarketService              _marketService;
        private CompetitorService          _competitorService;
        private ResearchService            _researchService;
        private ReportService              _reportService;
        private EventCrisisService         _eventCrisisService;

        private bool _initialized;
        private bool _disposed;

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new GameCompositionRoot.
        /// Call Initialize() before using any exposed properties.
        /// </summary>
        /// <param name="tuningConfig">The loaded tuning config from Resources.</param>
        public GameCompositionRoot(TuningConfig tuningConfig)
        {
            _tuningConfig = tuningConfig ?? throw new ArgumentNullException(nameof(tuningConfig));
        }

        // ─── Initialization ───────────────────────────────────────────────────────

        /// <summary>
        /// Creates and wires all dependencies. Must be called once after construction.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "[GameCompositionRoot] Initialize called more than once — ignored.");
                return;
            }

            DebugLogger.Log(DebugCategory.Bootstrap, "[GameCompositionRoot] Initializing...");

            // ── Step 1: Event bus ────────────────────────────────────────────────

            _eventBus = new InMemoryEventBus();
            EventBus  = _eventBus;

            // ── Step 2: Core services (all stateless) ────────────────────────────

            _timeService           = new TimeService();
            _companyService        = new CompanyService();
            _employeeService       = new EmployeeService();
            _teamService           = new TeamService();
            _productService        = new ProductService();
            _hardwareMetricsService = new HardwareMetricsService();
            _contractService       = new ContractService();
            _financeService        = new FinanceService();
            _marketService         = new MarketService();
            _competitorService     = new CompetitorService();
            _researchService       = new ResearchService();
            _reportService         = new ReportService(_eventBus, _tuningConfig);
            _eventCrisisService    = new EventCrisisService();

            // ── Step 3: Application use cases ─────────────────────────────────────

            var createCompanyUseCase            = new CreateCompanyUseCase(_companyService, _tuningConfig, _eventBus);
            var postJobUseCase                  = new PostJobUseCase(_eventBus);
            var sendOfferUseCase                = new SendOfferUseCase(_employeeService, _tuningConfig, _eventBus);
            var initializeRecruitmentUseCase    = new InitializeRecruitmentUseCase(_employeeService, _tuningConfig, _eventBus);

            var createTeamUseCase               = new CreateTeamUseCase(_teamService, _tuningConfig, _eventBus);
            var assignTeamUseCase               = new AssignTeamUseCase(_teamService, _eventBus);
            var unassignTeamUseCase             = new UnassignTeamUseCase(_teamService, _eventBus);

            var createProductUseCase            = new CreateProductUseCase(_productService, _productService, _hardwareMetricsService, _tuningConfig, _eventBus);
            var launchProductUseCase            = new LaunchProductUseCase(_productService, _productService, _hardwareMetricsService, _tuningConfig, _eventBus);
            var sunsetProductUseCase            = new SunsetProductUseCase(_productService, _eventBus);

            var acceptContractUseCase           = new AcceptContractUseCase(_contractService, _eventBus);
            var initializeContractBoardUseCase  = new InitializeContractBoardUseCase(_contractService, _tuningConfig, _eventBus);

            var startResearchUseCase            = new StartResearchUseCase(_researchService, _teamService, _financeService, _eventBus);
            var initializeResearchUseCase       = new InitializeResearchUseCase(_researchService);

            var initializeMarketUseCase         = new InitializeMarketUseCase(_marketService, _tuningConfig);
            var initializeCompetitorsUseCase    = new InitializeCompetitorsUseCase(_competitorService, _tuningConfig);

            var markReportReadUseCase           = new MarkReportReadUseCase();
            var archiveReportUseCase            = new ArchiveReportUseCase();
            var deleteReportUseCase             = new DeleteReportUseCase();

            // ReportEventHandler is created without a session — BindSession() will provide it.
            // This constructor overload requires null session, which the handler guards against.
            // (Session is provided after NewGameOrchestrator returns via BindSession.)
            // Note: ReportEventHandler requires a non-null session. It is created and initialized
            // during BindSession() instead.

            // ── Step 4: Orchestrators that don't need session ─────────────────────

            GameSessionValidator = new GameSessionValidator();

            NewGameOrchestrator = new NewGameOrchestrator(
                createCompanyUseCase,
                initializeRecruitmentUseCase,
                initializeContractBoardUseCase,
                initializeMarketUseCase,
                initializeCompetitorsUseCase,
                initializeResearchUseCase,
                _eventBus);

            // TickCoordinator is created here; processors are added in BindSession.
            _tickCoordinator = new TickCoordinator(_eventBus);

            ContinueOrchestrator = new ContinueOrchestrator(
                _timeService,
                _tickCoordinator,
                _tuningConfig,
                _eventBus);

            _initialized = true;
            DebugLogger.Log(DebugCategory.Bootstrap, "[GameCompositionRoot] Initialization complete.");
        }

        // ─── Session binding ──────────────────────────────────────────────────────

        /// <summary>
        /// Binds a newly created or loaded GameSessionState to all session-dependent processors
        /// and handlers. Must be called after NewGameOrchestrator.Execute() returns a session.
        /// Re-creates session-bound objects; disposes any previous session binding.
        /// </summary>
        /// <param name="session">The active session to bind.</param>
        public void BindSession(GameSessionState session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            DebugLogger.Log(DebugCategory.Bootstrap,
                $"[GameCompositionRoot] Binding session '{session.SessionId}'...");

            CurrentSession = session;

            // Dispose old report event handler if present.
            _reportEventHandler?.Dispose();

            // Build a System.Random from the session's random state.
            var systemRandom = new Random(session.RandomState?.Seed ?? 42);

            // Advance by CallCount to restore position (mirrors DeterministicRandomSource).
            for (int i = 0; i < (session.RandomState?.CallCount ?? 0); i++)
            {
                systemRandom.Next();
            }

            // Re-create session-bound processors with the new session reference.
            // A new TickCoordinator is needed to avoid duplicate-registration errors.
            _tickCoordinator = new TickCoordinator(_eventBus);

            // Create all tick processors in ProcessingOrder:
            var teamProgressProcessor   = new TeamProgressTickProcessor(_teamService, _tuningConfig, session, _eventBus);
            var employeeProcessor       = new EmployeeTickProcessor(_employeeService, _tuningConfig, _tuningConfig, session, systemRandom, _eventBus);
            var productPhaseProcessor   = new ProductPhaseTickProcessor(_productService, _tuningConfig, session, _eventBus);
            var softwareMetricsProcessor = new SoftwareMetricsTickProcessor(_productService, _tuningConfig, session, systemRandom);
            var hardwareMetricsProcessor = new HardwareMetricsTickProcessor(_hardwareMetricsService, _tuningConfig, session, systemRandom);
            var contractProcessor       = new ContractTickProcessor(_contractService, _tuningConfig, _eventBus, session, systemRandom);
            var researchProcessor       = new ResearchTickProcessor(_researchService, _tuningConfig, _eventBus, session);

            var processMonthlyFinanceUseCase = new ProcessMonthlyFinanceUseCase(_financeService, _tuningConfig, _eventBus, session);
            var financeProcessor        = new FinanceTickProcessor(processMonthlyFinanceUseCase);

            var marketProcessor         = new MarketTickProcessor(_marketService, _tuningConfig, _eventBus, session, systemRandom);
            var competitorProcessor     = new CompetitorTickProcessor(_competitorService, _tuningConfig, _tuningConfig, _eventBus, session, systemRandom);
            var eventCrisisProcessor    = new EventCrisisTickProcessor(_eventCrisisService, _tuningConfig, _tuningConfig, _eventBus, session, systemRandom);

            var reportProcessor         = new ReportTickProcessor(_reportService, session);

            // Register all processors (TickCoordinator sorts by ProcessingOrder internally).
            _tickCoordinator.Register(teamProgressProcessor);
            _tickCoordinator.Register(employeeProcessor);
            _tickCoordinator.Register(productPhaseProcessor);
            _tickCoordinator.Register(softwareMetricsProcessor);
            _tickCoordinator.Register(hardwareMetricsProcessor);
            _tickCoordinator.Register(contractProcessor);
            _tickCoordinator.Register(researchProcessor);
            _tickCoordinator.Register(financeProcessor);
            _tickCoordinator.Register(marketProcessor);
            _tickCoordinator.Register(competitorProcessor);
            _tickCoordinator.Register(eventCrisisProcessor);
            _tickCoordinator.Register(reportProcessor);

            // Re-create and initialize the report event handler.
            _reportEventHandler = new ReportEventHandler(_eventBus, _reportService, session);
            _reportEventHandler.Initialize();

            // Re-create ContinueOrchestrator pointing to the new TickCoordinator.
            ContinueOrchestrator = new ContinueOrchestrator(
                _timeService,
                _tickCoordinator,
                _tuningConfig,
                _eventBus);

            DebugLogger.Log(DebugCategory.Bootstrap,
                "[GameCompositionRoot] Session bound. All processors registered.");
        }

        // ─── IDisposable ──────────────────────────────────────────────────────────

        /// <summary>
        /// Cleans up event subscriptions and resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _reportEventHandler?.Dispose();

            DebugLogger.Log(DebugCategory.Bootstrap, "[GameCompositionRoot] Disposed.");
        }
    }
}
