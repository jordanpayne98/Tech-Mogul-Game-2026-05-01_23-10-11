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
    /// Application-layer use case for unassigning a team from its current work.
    ///
    /// Flow:
    ///   1. Finds TeamRuntimeState by TeamId — fails if not found.
    ///   2. Fails if the team has no active assignment.
    ///   3. Archives the previous assignment ID into AssignmentHistoryIds.
    ///   4. Clears CurrentAssignmentId and sets Workload to 0.
    ///   5. Publishes TeamUnassignedEvent.
    ///   6. Returns UnassignTeamResult.Succeeded(previousAssignmentId).
    ///
    /// The AssignmentRuntimeState is NOT deleted from GameSessionState.AssignmentStates.
    /// It remains for historical reference. RawProgressPoints are preserved.
    /// </summary>
    public sealed class UnassignTeamUseCase
    {
        private readonly ITeamService _teamService;
        private readonly IEventBus    _eventBus;

        public UnassignTeamUseCase(ITeamService teamService, IEventBus eventBus)
        {
            _teamService = teamService;
            _eventBus    = eventBus;
        }

        public UnassignTeamResult Execute(UnassignTeamRequest request, GameSessionState sessionState)
        {
            // ── 1. Find team ──────────────────────────────────────────────────────
            TeamRuntimeState teamState = sessionState.TeamStates
                .Find(t => t.TeamId == request.TeamId);

            if (teamState == null)
            {
                DebugLogger.LogWarning(
                    DebugCategory.Simulation,
                    $"[UnassignTeamUseCase] Team not found. TeamId={request.TeamId}");

                return UnassignTeamResult.Failed("Team not found");
            }

            // ── 2. Guard: must have an active assignment ───────────────────────────
            if (teamState.CurrentAssignmentId == null)
            {
                DebugLogger.LogWarning(
                    DebugCategory.Simulation,
                    $"[UnassignTeamUseCase] Team has no active assignment. TeamId={request.TeamId}");

                return UnassignTeamResult.Failed("Team has no active assignment");
            }

            // ── 3. Archive and clear ──────────────────────────────────────────────
            string previousAssignmentId = teamState.CurrentAssignmentId;

            teamState.AssignmentHistoryIds.Add(previousAssignmentId);
            teamState.CurrentAssignmentId = null;
            teamState.Workload            = 0;

            // ── 4. Publish event ──────────────────────────────────────────────────
            _eventBus.Publish(new TeamUnassignedEvent(request.TeamId, previousAssignmentId));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[UnassignTeamUseCase] Team unassigned. TeamId={request.TeamId} " +
                $"PreviousAssignmentId={previousAssignmentId}");

            // ── 5. Return success ─────────────────────────────────────────────────
            return UnassignTeamResult.Succeeded(previousAssignmentId);
        }
    }
}
