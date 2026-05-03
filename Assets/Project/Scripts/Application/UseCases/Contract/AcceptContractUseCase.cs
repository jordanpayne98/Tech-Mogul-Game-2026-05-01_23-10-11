using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Contract;
using Project.Core.Events.Contract;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Requests.Contract;
using Project.Core.Results.Contract;
using Project.Core.Runtime;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Contract
{
    /// <summary>
    /// Application use case for accepting a contract from the board.
    /// Validates availability, updates status, removes the contract from the board,
    /// and publishes <see cref="ContractAcceptedEvent"/>.
    ///
    /// Does NOT create a team assignment. The player must assign a team separately via
    /// <c>AssignTeamUseCase</c> with AssignmentType.ContractWork and AssignmentTargetType.Contract.
    ///
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public sealed class AcceptContractUseCase
    {
        private readonly IContractService _contractService;
        private readonly IEventBus        _eventBus;

        public AcceptContractUseCase(
            IContractService contractService,
            IEventBus        eventBus)
        {
            _contractService = contractService;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Executes the contract acceptance flow.
        /// </summary>
        /// <param name="request">Acceptance request containing the contract stable ID.</param>
        /// <param name="sessionState">Live session state.</param>
        /// <param name="currentDate">Current in-game date for expiry validation.</param>
        /// <returns>Result indicating success or a clear failure reason.</returns>
        public AcceptContractResult Execute(
            AcceptContractRequest request,
            GameSessionState      sessionState,
            GameDateTime          currentDate)
        {
            // 1. Find profile
            var profile = sessionState.ContractProfiles
                .FirstOrDefault(p => p.Id == request.ContractId);

            if (profile == null)
            {
                return AcceptContractResult.Failed("Contract not found.");
            }

            // 2. Find runtime state
            var state = sessionState.ContractStates
                .FirstOrDefault(s => s.ContractId == request.ContractId);

            if (state == null)
            {
                return AcceptContractResult.Failed("Contract runtime state not found.");
            }

            // 3. Validate status
            if (state.Status != ContractStatus.Available)
            {
                return AcceptContractResult.Failed($"Contract is not available. Current status: {state.Status}.");
            }

            // 4. Check expiry
            if (currentDate.TotalElapsedHours > profile.ExpiryDate.TotalElapsedHours)
            {
                state.Status = ContractStatus.Expired;
                sessionState.ContractBoardState.AvailableContractIds.Remove(request.ContractId);

                return AcceptContractResult.Failed("Contract has expired.");
            }

            // 5. Accept the contract
            state.Status      = ContractStatus.Accepted;
            state.AcceptedDate = currentDate;

            // 6. Remove from board
            sessionState.ContractBoardState.AvailableContractIds.Remove(request.ContractId);

            // 7. Publish event
            _eventBus.Publish(new ContractAcceptedEvent(request.ContractId));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[AcceptContractUseCase] Contract accepted. ContractId: {request.ContractId}, Date: {currentDate}");

            return AcceptContractResult.Succeeded(request.ContractId);
        }
    }
}
