using Project.Core.Debugging;
using Project.Core.Definitions.Finance;
using Project.Core.Definitions.Research;
using Project.Core.Definitions.Team;
using Project.Core.Events.Research;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Requests.Research;
using Project.Core.Requests.Team;
using Project.Core.Results.Research;
using Project.Core.Runtime;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;
using Project.Core.Services.Research;

namespace Project.Application.UseCases.Research
{
    /// <summary>
    /// Application use case for starting a research project.
    /// Validates the request, deducts the one-time cost, creates project runtime state,
    /// creates the team assignment, updates the research portfolio, and publishes
    /// a ResearchStartedEvent.
    ///
    /// Does NOT create a separate assignment via AssignTeamUseCase — it creates the
    /// AssignmentRuntimeState directly through ITeamService.CreateAssignment to keep
    /// the flow atomic and to avoid double-validation.
    ///
    /// Caller is responsible for passing the current game date.
    /// Wiring deferred to Plan 2N.
    ///
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public sealed class StartResearchUseCase
    {
        private readonly IResearchService _researchService;
        private readonly ITeamService     _teamService;
        private readonly IFinanceService  _financeService;
        private readonly IEventBus        _eventBus;

        public StartResearchUseCase(
            IResearchService researchService,
            ITeamService     teamService,
            IFinanceService  financeService,
            IEventBus        eventBus)
        {
            _researchService = researchService;
            _teamService     = teamService;
            _financeService  = financeService;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Executes the start-research flow.
        /// </summary>
        /// <param name="request">Contains the research project ID and team ID to assign.</param>
        /// <param name="sessionState">Live session state.</param>
        /// <param name="currentDate">Current in-game date.</param>
        public StartResearchResult Execute(
            StartResearchRequest request,
            GameSessionState     sessionState,
            GameDateTime         currentDate)
        {
            // 1. Resolve definition.
            var definition = ResearchProjectCatalog.GetProject(request.ResearchProjectId);

            // 2. Validate.
            bool isValid = _researchService.ValidateStartResearch(
                request,
                sessionState.ResearchState,
                sessionState,
                definition?.CostMinorUnits ?? 0L,
                out string failureReason);

            if (!isValid)
            {
                return StartResearchResult.Failed(failureReason);
            }

            // 3. Deduct one-time research cost.
            long cost = definition.CostMinorUnits;

            var costTransaction = _financeService.CreateExpenseTransaction(
                ExpenseCategory.Research,
                cost,
                $"Research project started: {definition.Name}",
                currentDate,
                definition.Id,
                "ResearchProject");

            sessionState.FinanceState.CashMinorUnits -= cost;
            sessionState.TransactionRecords.Add(costTransaction);

            // 4. Create project runtime state.
            var projectState = new ResearchProjectRuntimeState
            {
                ProjectId      = definition.Id,
                Status         = ResearchProjectStatus.InProgress,
                ProgressPercent = 0,
                AssignedTeamId = request.TeamId,
                StartDate      = currentDate,
                CompletedDate  = null,
                EstimatedCompletionDate = null // [Placeholder] — estimation deferred
            };

            sessionState.ResearchProjectStates.Add(projectState);

            // 5. Create team assignment via ITeamService.
            var assignTeamRequest = new AssignTeamRequest(
                request.TeamId,
                AssignmentType.ResearchProject,
                AssignmentTargetType.ResearchProject,
                definition.Id);

            var assignment = _teamService.CreateAssignment(assignTeamRequest, currentDate);
            sessionState.AssignmentStates.Add(assignment);

            // Link the assignment back to the team runtime state.
            var teamState = sessionState.TeamStates
                .Find(t => t.TeamId == request.TeamId);

            if (teamState != null)
            {
                teamState.CurrentAssignmentId = assignment.Id;
            }

            // 6. Update research portfolio state.
            sessionState.ResearchState.AvailableProjectIds.Remove(definition.Id);
            sessionState.ResearchState.ActiveProjectIds.Add(definition.Id);

            // 7. Publish event.
            _eventBus.Publish(new ResearchStartedEvent(definition.Id, request.TeamId));

            // 8. Log.
            DebugLogger.Log(DebugCategory.Simulation,
                $"[StartResearchUseCase] Research project started. ProjectId: {definition.Id}, "
                + $"TeamId: {request.TeamId}, CostMinorUnits: {cost}, Date: {currentDate}");

            // 9. Return success.
            return StartResearchResult.Succeeded(request.ResearchProjectId);
        }
    }
}
