using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Team
{
    /// <summary>
    /// ITickProcessor (Order 1) for daily team progress and workload refresh.
    ///
    /// On each day boundary, for every team that has an active assignment:
    ///   - Resolves the live member list from GameSessionState.
    ///   - Computes and applies daily progress points to AssignmentRuntimeState.RawProgressPoints.
    ///   - Grows team cohesion.
    ///   - Refreshes team workload.
    ///   - Updates team morale from member averages.
    ///
    /// Does NOT update ProgressPercent. Domain plans (2E, 2F, 2G, 2K) own that conversion.
    /// Runs at Order 1 so workload is refreshed before EmployeeTickProcessor (Order 2) checks overwork.
    /// </summary>
    public sealed class TeamProgressTickProcessor : ITickProcessor
    {
        private readonly ITeamService    _teamService;
        private readonly ITeamTuning     _tuning;
        private readonly GameSessionState _sessionState;
        private readonly IEventBus       _eventBus;

        // ─── ITickProcessor ───────────────────────────────────────────────────────

        public string ProcessorName  => "TeamProgressTickProcessor";
        public int    ProcessingOrder => 1;

        // ─── Constructor ──────────────────────────────────────────────────────────

        public TeamProgressTickProcessor(
            ITeamService    teamService,
            ITeamTuning     tuning,
            GameSessionState sessionState,
            IEventBus       eventBus)
        {
            _teamService  = teamService;
            _tuning       = tuning;
            _sessionState = sessionState;
            _eventBus     = eventBus;
        }

        // ─── ProcessTick ──────────────────────────────────────────────────────────

        public TickResult ProcessTick(TickContext context)
        {
            // Progress is daily only — skip sub-day ticks.
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            foreach (TeamRuntimeState teamState in _sessionState.TeamStates)
            {
                // Refresh workload regardless of assignment state.
                teamState.Workload = _teamService.ComputeWorkload(teamState);

                if (teamState.CurrentAssignmentId == null)
                {
                    // No assignment — skip progress and cohesion growth.
                    continue;
                }

                // Resolve the live member list for this team.
                List<EmployeeRuntimeState> members = ResolveMemberStates(teamState);

                // Update team morale from member averages before computing progress.
                _teamService.UpdateTeamMorale(teamState, members);

                // Find the active assignment.
                AssignmentRuntimeState assignment = _sessionState.AssignmentStates
                    .Find(a => a.Id == teamState.CurrentAssignmentId);

                if (assignment == null)
                {
                    DebugLogger.LogWarning(
                        DebugCategory.Simulation,
                        $"[TeamProgressTickProcessor] TeamId={teamState.TeamId} has CurrentAssignmentId " +
                        $"'{teamState.CurrentAssignmentId}' but no matching AssignmentRuntimeState was found. " +
                        "Skipping progress for this team.");
                    continue;
                }

                // Compute and apply daily progress.
                int dailyProgress = _teamService.ComputeDailyProgress(teamState, members, assignment.Type, _tuning);
                assignment.RawProgressPoints += dailyProgress;

                DebugLogger.Log(
                    DebugCategory.Simulation,
                    $"[TeamProgressTickProcessor] TeamId={teamState.TeamId} AssignmentId={assignment.Id} " +
                    $"dailyProgress={dailyProgress} totalRaw={assignment.RawProgressPoints} " +
                    $"Date={context.CurrentDate.Year}/{context.CurrentDate.Month}/{context.CurrentDate.Day}");

                // Grow cohesion (only when assigned).
                _teamService.UpdateCohesion(teamState, _tuning);
            }

            return TickResult.Succeeded();
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private List<EmployeeRuntimeState> ResolveMemberStates(TeamRuntimeState teamState)
        {
            List<EmployeeRuntimeState> result = new List<EmployeeRuntimeState>(teamState.MemberIds.Count);

            foreach (string memberId in teamState.MemberIds)
            {
                EmployeeRuntimeState employeeState = _sessionState.EmployeeStates
                    .Find(e => e.EmployeeId == memberId);

                if (employeeState != null)
                {
                    result.Add(employeeState);
                }
                else
                {
                    DebugLogger.LogWarning(
                        DebugCategory.Simulation,
                        $"[TeamProgressTickProcessor] MemberId='{memberId}' in TeamId='{teamState.TeamId}' " +
                        "has no matching EmployeeRuntimeState. Skipping this member.");
                }
            }

            return result;
        }
    }
}
