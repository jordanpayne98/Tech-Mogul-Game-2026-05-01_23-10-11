using Project.Core.Debugging;
using Project.Core.Events.Team;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Requests.Team;
using Project.Core.Results.Team;
using Project.Core.Runtime;
using Project.Core.Runtime.Team;

namespace Project.Application.UseCases.Team
{
    /// <summary>
    /// Application-layer use case for assigning a team to a specific work target.
    ///
    /// Flow:
    ///   1. Validates the request via ITeamService (team must exist, no active assignment,
    ///      has members, target ID non-empty).
    ///   2. Creates the AssignmentRuntimeState.
    ///   3. Adds it to GameSessionState.AssignmentStates.
    ///   4. Links it to the team via TeamRuntimeState.CurrentAssignmentId.
    ///   5. Publishes TeamAssignedEvent.
    ///   6. Returns AssignTeamResult.Succeeded(assignmentId).
    /// </summary>
    public sealed class AssignTeamUseCase
    {
        private readonly ITeamService _teamService;
        private readonly IEventBus    _eventBus;

        public AssignTeamUseCase(ITeamService teamService, IEventBus eventBus)
        {
            _teamService = teamService;
            _eventBus    = eventBus;
        }

        public AssignTeamResult Execute(AssignTeamRequest request, GameSessionState sessionState)
        {
            // ── 1. Validate ───────────────────────────────────────────────────────
            if (!_teamService.ValidateAssignment(request, sessionState, out string failureReason))
            {
                DebugLogger.LogWarning(
                    DebugCategory.Simulation,
                    $"[AssignTeamUseCase] Validation failed: {failureReason}");

                return AssignTeamResult.Failed(failureReason);
            }

            // ── 2. Find team state ────────────────────────────────────────────────
            TeamRuntimeState teamState = sessionState.TeamStates
                .Find(t => t.TeamId == request.TeamId);

            // ── 3. Create assignment ──────────────────────────────────────────────
            AssignmentRuntimeState assignment = _teamService.CreateAssignment(
                request,
                sessionState.TimeState.CurrentDate);

            // ── 4. Persist and link ───────────────────────────────────────────────
            sessionState.AssignmentStates.Add(assignment);
            teamState.CurrentAssignmentId = assignment.Id;

            // ── 5. Publish event ──────────────────────────────────────────────────
            _eventBus.Publish(new TeamAssignedEvent(request.TeamId, assignment.Id));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[AssignTeamUseCase] Team assigned. TeamId={request.TeamId} " +
                $"AssignmentId={assignment.Id} Type={request.Type} TargetId={request.TargetId}");

            // ── 6. Return success ─────────────────────────────────────────────────
            return AssignTeamResult.Succeeded(assignment.Id);
        }
    }
}
