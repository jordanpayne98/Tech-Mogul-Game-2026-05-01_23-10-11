using System;
using System.Collections.Generic;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Event;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime
{
    /// <summary>
    /// Root container for all per-save runtime state objects.
    /// This class has no logic beyond list initialisation and the CreateEmpty factory.
    /// It exists to give the Continue system and all use cases a single typed entry point
    /// to all live session data.
    ///
    /// Lookup by stable ID is the caller's responsibility (service or use case).
    /// Future plans add entities by appending to the appropriate list.
    /// </summary>
    public sealed class GameSessionState
    {
        // ─── Session identity ─────────────────────────────────────────────────────

        /// <summary>Stable GUID identifying this save session.</summary>
        public string SessionId { get; set; }

        // ─── Deterministic random ─────────────────────────────────────────────────

        /// <summary>Serialisable state for the deterministic RNG. Persisted on save, restored on load.</summary>
        public RandomRuntimeState RandomState { get; set; }

        // ─── Core identity ────────────────────────────────────────────────────────

        public CompanyProfile CompanyProfile { get; set; }
        public FounderProfile FounderProfile { get; set; }

        // ─── Company state ────────────────────────────────────────────────────────

        public CompanyRuntimeState CompanyState { get; set; }

        // ─── Time state ───────────────────────────────────────────────────────────

        public TimeRuntimeState TimeState { get; set; }

        // ─── Finance state ────────────────────────────────────────────────────────

        public FinanceRuntimeState FinanceState { get; set; }

        // ─── Recruitment state ────────────────────────────────────────────────────

        public RecruitmentRuntimeState RecruitmentState { get; set; }

        // ─── Inbox state ──────────────────────────────────────────────────────────

        public InboxRuntimeState InboxState { get; set; }

        // ─── Research state ───────────────────────────────────────────────────────

        public ResearchRuntimeState ResearchState { get; set; }

        // ─── Employee lists ───────────────────────────────────────────────────────

        public List<EmployeeProfile> EmployeeProfiles { get; set; }
        public List<EmployeeRuntimeState> EmployeeStates { get; set; }

        // ─── Candidate lists ──────────────────────────────────────────────────────

        public List<CandidateProfile> CandidateProfiles { get; set; }
        public List<CandidateRuntimeState> CandidateStates { get; set; }

        // ─── Team lists ───────────────────────────────────────────────────────────

        public List<TeamProfile> TeamProfiles { get; set; }
        public List<TeamRuntimeState> TeamStates { get; set; }
        public List<AssignmentRuntimeState> AssignmentStates { get; set; }

        // ─── Product lists ────────────────────────────────────────────────────────

        public List<ProductProfile> ProductProfiles { get; set; }
        public List<ProductRuntimeState> ProductStates { get; set; }
        public List<SoftwareRuntimeMetrics> SoftwareMetrics { get; set; }
        public List<HardwareRuntimeMetrics> HardwareMetrics { get; set; }
        public List<ProductBudgetProfile> ProductBudgets { get; set; }

        // ─── Contract lists ───────────────────────────────────────────────────────

        public List<ContractProfile> ContractProfiles { get; set; }
        public List<ContractRuntimeState> ContractStates { get; set; }
        public ContractBoardRuntimeState ContractBoardState { get; set; }

        // ─── Market lists ─────────────────────────────────────────────────────────

        public List<CompetitorProfile> CompetitorProfiles { get; set; }
        public List<CompetitorRuntimeState> CompetitorStates { get; set; }
        public List<CompetitorProductRuntimeState> CompetitorProductStates { get; set; }
        public List<MarketCategoryRuntimeState> MarketCategoryStates { get; set; }
        public List<TrendRuntimeState> TrendStates { get; set; }

        // ─── Report lists ─────────────────────────────────────────────────────────

        public List<ReportProfile> ReportProfiles { get; set; }

        // ─── Research lists ───────────────────────────────────────────────────────

        public List<ResearchProjectRuntimeState> ResearchProjectStates { get; set; }

        // ─── Job post lists ───────────────────────────────────────────────────────

        public List<JobPostProfile> JobPostProfiles { get; set; }
        public List<JobPostRuntimeState> JobPostStates { get; set; }

        // ─── Finance record lists ─────────────────────────────────────────────────

        public List<TransactionRecord> TransactionRecords { get; set; }
        public List<MonthlyFinanceSummary> MonthlyFinanceSummaries { get; set; }

        // ─── Event / crisis state ─────────────────────────────────────────────────

        /// <summary>All fired event instances for the current save. Grows over time.</summary>
        public List<GameEventRuntimeState> EventHistory { get; set; }

        /// <summary>
        /// Per-event cooldown tracker, keyed by a composite key of the form
        /// "{EventDefinitionId}" for random events or "{EventDefinitionId}:{TargetEntityId}"
        /// for per-entity threshold events. Records the date the event last fired.
        /// </summary>
        public Dictionary<string, GameDateTime> LastEventDates { get; set; }

        /// <summary>The date on which any random event last fired. Used for global cooldown gating.</summary>
        public GameDateTime LastGlobalEventDate { get; set; }

        /// <summary>The date on which the EventCrisisTickProcessor last performed a random event check.</summary>
        public GameDateTime LastEventCheckDate { get; set; }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes all list properties to empty lists.
        /// Non-list reference properties default to null and are set explicitly by the use case.
        /// </summary>
        public GameSessionState()
        {
            EmployeeProfiles = new List<EmployeeProfile>();
            EmployeeStates = new List<EmployeeRuntimeState>();

            CandidateProfiles = new List<CandidateProfile>();
            CandidateStates = new List<CandidateRuntimeState>();

            TeamProfiles = new List<TeamProfile>();
            TeamStates = new List<TeamRuntimeState>();
            AssignmentStates = new List<AssignmentRuntimeState>();

            ProductProfiles = new List<ProductProfile>();
            ProductStates = new List<ProductRuntimeState>();
            SoftwareMetrics = new List<SoftwareRuntimeMetrics>();
            HardwareMetrics = new List<HardwareRuntimeMetrics>();
            ProductBudgets = new List<ProductBudgetProfile>();

            ContractProfiles = new List<ContractProfile>();
            ContractStates = new List<ContractRuntimeState>();
            ContractBoardState = new ContractBoardRuntimeState();

            CompetitorProfiles = new List<CompetitorProfile>();
            CompetitorStates = new List<CompetitorRuntimeState>();
            CompetitorProductStates = new List<CompetitorProductRuntimeState>();
            MarketCategoryStates = new List<MarketCategoryRuntimeState>();
            TrendStates = new List<TrendRuntimeState>();

            ReportProfiles = new List<ReportProfile>();

            ResearchProjectStates = new List<ResearchProjectRuntimeState>();

            JobPostProfiles = new List<JobPostProfile>();
            JobPostStates = new List<JobPostRuntimeState>();

            TransactionRecords = new List<TransactionRecord>();
            MonthlyFinanceSummaries = new List<MonthlyFinanceSummary>();

            EventHistory     = new List<GameEventRuntimeState>();
            LastEventDates   = new Dictionary<string, GameDateTime>();
        }

        // ─── Factory ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a fully initialised empty session state with a seeded random state.
        /// All domain states are null — use cases populate them during new-game initialisation.
        /// </summary>
        /// <param name="seed">The random seed for deterministic simulation.</param>
        public static GameSessionState CreateEmpty(int seed)
        {
            return new GameSessionState
            {
                SessionId   = Guid.NewGuid().ToString(),
                RandomState = new RandomRuntimeState { Seed = seed, CallCount = 0 },
                TimeState   = TimeRuntimeState.CreateDefault()
            };
        }
    }
}
