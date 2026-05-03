using System;
using Project.Core.Debugging;
using Project.Core.Events.Contract;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Contract
{
    /// <summary>
    /// Application use case that sets up the initial contract board for a new session.
    /// Generates <see cref="IContractTuning.InitialContractBoardSize"/> contracts,
    /// creates runtime state for each, adds them to the session, and marks the refresh date.
    ///
    /// Called once during new-session initialization. Not called on load (the board state is saved).
    ///
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public sealed class InitializeContractBoardUseCase
    {
        private readonly IContractService _contractService;
        private readonly IContractTuning  _tuning;
        private readonly IEventBus        _eventBus;

        public InitializeContractBoardUseCase(
            IContractService contractService,
            IContractTuning  tuning,
            IEventBus        eventBus)
        {
            _contractService = contractService;
            _tuning          = tuning;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Executes the board initialization.
        /// Generates contracts, populates sessionState lists, and sets the refresh date.
        /// </summary>
        /// <param name="sessionState">Live session state to populate.</param>
        /// <param name="currentDate">In-game date of initialization.</param>
        /// <param name="random">Seeded random instance for procedural generation.</param>
        public void Execute(GameSessionState sessionState, GameDateTime currentDate, Random random)
        {
            int count = _tuning.InitialContractBoardSize;

            var profiles = _contractService.GenerateContracts(count, _tuning, currentDate, random);

            foreach (var profile in profiles)
            {
                var state = _contractService.CreateContractRuntimeState(profile.Id);

                sessionState.ContractProfiles.Add(profile);
                sessionState.ContractStates.Add(state);
                sessionState.ContractBoardState.AvailableContractIds.Add(profile.Id);
            }

            sessionState.ContractBoardState.LastBoardRefreshDate = currentDate;

            _eventBus.Publish(new ContractBoardRefreshedEvent(profiles.Count, currentDate));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[InitializeContractBoardUseCase] Board initialized with {profiles.Count} contracts. Date: {currentDate}");
        }
    }
}
