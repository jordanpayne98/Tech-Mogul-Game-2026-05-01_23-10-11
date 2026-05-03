using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Events.Team;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Team;
using Project.Core.Results.Team;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Team;

namespace Project.Application.UseCases.Team
{
    /// <summary>
    /// Application-layer use case for creating a new team.
    ///
    /// Flow:
    ///   1. Validates the request via ITeamService.
    ///   2. Creates the TeamProfile and TeamRuntimeState.
    ///   3. Adds both to GameSessionState.
    ///   4. Sets CurrentTeamId on each initial member's EmployeeRuntimeState.
    ///   5. Publishes TeamCreatedEvent and one TeamMemberAddedEvent per initial member.
    ///   6. Returns CreateTeamResult.Succeeded(teamId).
    /// </summary>
    public sealed class CreateTeamUseCase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamTuning  _tuning;
        private readonly IEventBus    _eventBus;

        public CreateTeamUseCase(ITeamService teamService, ITeamTuning tuning, IEventBus eventBus)
        {
            _teamService = teamService;
            _tuning      = tuning;
            _eventBus    = eventBus;
        }

        public CreateTeamResult Execute(CreateTeamRequest request, GameSessionState sessionState)
        {
            // ── 1. Validate ───────────────────────────────────────────────────────
            if (!_teamService.ValidateTeamCreation(request, sessionState, _tuning, out string failureReason))
            {
                DebugLogger.LogWarning(
                    DebugCategory.Simulation,
                    $"[CreateTeamUseCase] Validation failed: {failureReason}");

                return CreateTeamResult.Failed(failureReason);
            }

            // ── 2. Create profile and state ───────────────────────────────────────
            TeamProfile profile = _teamService.CreateTeamProfile(
                request.Name,
                request.Type,
                sessionState.TimeState.CurrentDate);

            TeamRuntimeState teamState = _teamService.CreateTeamState(
                profile.Id,
                request.MemberIds,
                _tuning);

            // ── 3. Add to session state ───────────────────────────────────────────
            sessionState.TeamProfiles.Add(profile);
            sessionState.TeamStates.Add(teamState);

            // ── 4. Sync CurrentTeamId on each initial member ──────────────────────
            foreach (string memberId in request.MemberIds)
            {
                EmployeeRuntimeState employeeState = sessionState.EmployeeStates
                    .Find(e => e.EmployeeId == memberId);

                if (employeeState != null)
                {
                    employeeState.CurrentTeamId = profile.Id;
                    _eventBus.Publish(new TeamMemberAddedEvent(profile.Id, memberId));
                }
            }

            // ── 5. Publish TeamCreatedEvent ───────────────────────────────────────
            _eventBus.Publish(new TeamCreatedEvent(profile.Id));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[CreateTeamUseCase] Team created. Id={profile.Id} Name='{profile.Name}' " +
                $"Members={request.MemberIds.Count}");

            // ── 6. Return success ─────────────────────────────────────────────────
            return CreateTeamResult.Succeeded(profile.Id);
        }
    }
}
