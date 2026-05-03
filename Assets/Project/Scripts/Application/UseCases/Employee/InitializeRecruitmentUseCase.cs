using System;
using Project.Core.Debugging;
using Project.Core.Events.Employee;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;

namespace Project.Application.UseCases.Employee
{
    /// <summary>
    /// Application-layer use case that generates the initial candidate pool for a new save.
    /// Must be called after CreateCompanyUseCase has initialized the session state.
    ///
    /// The initial pool is separate from CreateCompanyUseCase by design (Locked Decision #1).
    /// This allows recruitment to be re-initialized without recreating the company.
    /// </summary>
    public sealed class InitializeRecruitmentUseCase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeTuning  _tuning;
        private readonly IEventBus        _eventBus;

        public InitializeRecruitmentUseCase(
            IEmployeeService employeeService,
            IEmployeeTuning  tuning,
            IEventBus        eventBus)
        {
            _employeeService = employeeService;
            _tuning          = tuning;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Generates the initial candidate pool and populates the session state.
        /// </summary>
        /// <param name="sessionState">Active session state to populate.</param>
        /// <param name="random">Shared random instance.</param>
        public void Execute(GameSessionState sessionState, Random random)
        {
            if (sessionState == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[InitializeRecruitmentUseCase] sessionState is null. Cannot initialize recruitment.");
                return;
            }

            int startingCounter = sessionState.CandidateProfiles.Count;

            System.Collections.Generic.List<CandidateProfile> candidates =
                _employeeService.GenerateCandidates(
                    _tuning.InitialCandidatePoolSize,
                    startingCounter,
                    _tuning,
                    sessionState.TimeState.CurrentDate,
                    random);

            System.Collections.Generic.List<CandidateRuntimeState> candidateStates =
                _employeeService.CreateCandidateStates(candidates);

            foreach (CandidateProfile profile in candidates)
            {
                sessionState.CandidateProfiles.Add(profile);
                sessionState.RecruitmentState.CandidateIds.Add(profile.Id);
            }

            foreach (CandidateRuntimeState state in candidateStates)
            {
                sessionState.CandidateStates.Add(state);
            }

            sessionState.RecruitmentState.LastCandidatePoolRefreshDate = sessionState.TimeState.CurrentDate;

            _eventBus.Publish(new CandidatePoolRefreshedEvent(_tuning.InitialCandidatePoolSize));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[InitializeRecruitmentUseCase] Initial candidate pool generated: " +
                $"{_tuning.InitialCandidatePoolSize} candidates. " +
                $"Date: {sessionState.TimeState.CurrentDate.Year}/{sessionState.TimeState.CurrentDate.Month}");
        }
    }
}
