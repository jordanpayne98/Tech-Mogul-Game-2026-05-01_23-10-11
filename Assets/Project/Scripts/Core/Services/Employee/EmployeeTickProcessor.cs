using System;
using Project.Core.Debugging;
using Project.Core.Definitions.Employee;
using Project.Core.Events.Employee;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Employee
{
    /// <summary>
    /// ITickProcessor implementation for daily and monthly employee state processing.
    /// Runs at Order 2, after TeamProgressTickProcessor (Order 1) has refreshed team workload.
    ///
    /// Daily processing (on IsDayBoundary):
    ///   - Burnout recovery for each active employee.
    ///   - Overwork morale decay and burnout accumulation for employees on teams
    ///     whose Workload exceeds OverloadThresholdPercent (120).
    ///     NOTE: With binary workload (assigned = 100), this code path is dormant in MVP
    ///     since 100 &lt; 120. It exists for future multi-assignment support.
    ///
    /// Monthly processing (on IsMonthBoundary): loyalty growth + candidate pool refresh.
    ///
    /// This processor holds GameSessionState, System.Random, and IEventBus as constructor
    /// dependencies. This is acceptable because tick processors are coordination objects
    /// that require access to session state across multiple entities.
    /// </summary>
    public sealed class EmployeeTickProcessor : ITickProcessor
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeTuning  _employeeTuning;
        private readonly ITeamTuning      _teamTuning;
        private readonly GameSessionState _sessionState;
        private readonly Random           _random;
        private readonly IEventBus        _eventBus;

        // ─── ITickProcessor ───────────────────────────────────────────────────────

        public string ProcessorName  => "EmployeeTickProcessor";
        public int    ProcessingOrder => 2;

        // ─── Constructor ──────────────────────────────────────────────────────────

        public EmployeeTickProcessor(
            IEmployeeService employeeService,
            IEmployeeTuning  employeeTuning,
            ITeamTuning      teamTuning,
            GameSessionState sessionState,
            Random           random,
            IEventBus        eventBus)
        {
            _employeeService = employeeService;
            _employeeTuning  = employeeTuning;
            _teamTuning      = teamTuning;
            _sessionState    = sessionState;
            _random          = random;
            _eventBus        = eventBus;
        }

        // ─── ProcessTick ──────────────────────────────────────────────────────────

        public TickResult ProcessTick(TickContext context)
        {
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            // ── Daily recovery: burnout recovery for all active employees ──────────
            foreach (EmployeeRuntimeState employee in _sessionState.EmployeeStates)
            {
                if (employee.Status == EmploymentStatus.Active)
                {
                    _employeeService.ProcessDailyRecovery(employee, _employeeTuning);
                }
            }

            // ── Overwork detection: morale decay and burnout for overloaded employees
            // Reads TeamRuntimeState.Workload refreshed by TeamProgressTickProcessor (Order 1).
            // In MVP binary workload (100) this condition is always false (100 < 120).
            // Code path exists for future multi-assignment overload scenarios.
            foreach (EmployeeRuntimeState employee in _sessionState.EmployeeStates)
            {
                if (employee.Status != EmploymentStatus.Active || employee.CurrentTeamId == null)
                {
                    continue;
                }

                TeamRuntimeState teamState = _sessionState.TeamStates
                    .Find(t => t.TeamId == employee.CurrentTeamId);

                if (teamState == null)
                {
                    continue;
                }

                if (teamState.Workload > _teamTuning.OverloadThresholdPercent)
                {
                    employee.Morale = Math.Max(
                        0,
                        employee.Morale - (int)_employeeTuning.MoraleDecayPerOverworkDay);

                    employee.BurnoutRisk = Math.Min(
                        100,
                        employee.BurnoutRisk + (int)_employeeTuning.BurnoutAccumulationPerOverworkDay);

                    DebugLogger.Log(
                        DebugCategory.Simulation,
                        $"[EmployeeTickProcessor] Overwork detected. EmployeeId={employee.EmployeeId} " +
                        $"TeamId={teamState.TeamId} Workload={teamState.Workload} " +
                        $"Morale={employee.Morale} BurnoutRisk={employee.BurnoutRisk}");
                }
            }

            // ── Monthly processing ────────────────────────────────────────────────
            if (context.IsMonthBoundary)
            {
                ProcessMonthly(context);
            }

            return TickResult.Succeeded();
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private void ProcessMonthly(TickContext context)
        {
            // Monthly loyalty growth for all active employees.
            foreach (EmployeeRuntimeState employee in _sessionState.EmployeeStates)
            {
                if (employee.Status == EmploymentStatus.Active)
                {
                    _employeeService.ProcessMonthlyLoyalty(employee, _employeeTuning);
                }
            }

            // Candidate pool refresh: add CandidatePoolRefreshCount new candidates.
            int startingCounter = _sessionState.CandidateProfiles.Count;

            System.Collections.Generic.List<CandidateProfile> newCandidates =
                _employeeService.GenerateCandidates(
                    _employeeTuning.CandidatePoolRefreshCount,
                    startingCounter,
                    _employeeTuning,
                    context.CurrentDate,
                    _random);

            System.Collections.Generic.List<CandidateRuntimeState> newStates =
                _employeeService.CreateCandidateStates(newCandidates);

            foreach (CandidateProfile profile in newCandidates)
            {
                _sessionState.CandidateProfiles.Add(profile);
                _sessionState.RecruitmentState.CandidateIds.Add(profile.Id);
            }

            foreach (CandidateRuntimeState state in newStates)
            {
                _sessionState.CandidateStates.Add(state);
            }

            _sessionState.RecruitmentState.LastCandidatePoolRefreshDate = context.CurrentDate;

            _eventBus.Publish(new CandidatePoolRefreshedEvent(_employeeTuning.CandidatePoolRefreshCount));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[EmployeeTickProcessor] Monthly refresh: added {_employeeTuning.CandidatePoolRefreshCount} candidates. " +
                $"Pool size now: {_sessionState.RecruitmentState.CandidateIds.Count}. " +
                $"Date: {context.CurrentDate.Year}/{context.CurrentDate.Month}");
        }
    }
}
