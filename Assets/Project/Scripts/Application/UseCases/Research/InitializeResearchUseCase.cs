using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Interfaces.Services;
using Project.Core.Runtime;
using Project.Core.Runtime.Research;

namespace Project.Application.UseCases.Research
{
    /// <summary>
    /// Application use case for initializing the research portfolio state at session start.
    /// Creates a fresh ResearchRuntimeState, evaluates which projects are initially available,
    /// and populates AvailableProjectIds.
    ///
    /// Standalone — called after CreateCompanyUseCase, before the first tick.
    /// Caller orchestration deferred to Plan 2N.
    ///
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public sealed class InitializeResearchUseCase
    {
        private readonly IResearchService _researchService;

        public InitializeResearchUseCase(IResearchService researchService)
        {
            _researchService = researchService;
        }

        /// <summary>
        /// Initializes the research portfolio for a new session.
        /// Sets ResearchState and ResearchProjectStates on the session state.
        /// </summary>
        /// <param name="sessionState">Live session state to initialize.</param>
        public void Execute(GameSessionState sessionState)
        {
            // 1. Create a clean research runtime state for this company.
            var researchState = new ResearchRuntimeState
            {
                CompanyId            = sessionState.CompanyProfile?.Id ?? string.Empty,
                AvailableProjectIds  = new List<string>(),
                ActiveProjectIds     = new List<string>(),
                CompletedProjectIds  = new List<string>(),
                ObsoleteProjectIds   = new List<string>(),
                UnlockedCapabilityIds = new List<string>()
            };

            sessionState.ResearchState        = researchState;
            sessionState.ResearchProjectStates = new List<ResearchProjectRuntimeState>();

            // 2. Evaluate which projects are available from the start (no prerequisites).
            var initiallyAvailable = _researchService.EvaluateNewlyAvailableProjects(researchState);

            foreach (string projectId in initiallyAvailable)
            {
                researchState.AvailableProjectIds.Add(projectId);
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[InitializeResearchUseCase] Research initialized with {initiallyAvailable.Count} available projects.");
        }
    }
}
