using System.Collections.Generic;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Contract
{
    /// <summary>
    /// Mutable runtime state tracking the state of the contract board.
    /// Stores which contracts are currently available and when the board was last refreshed.
    /// Follows the same board-state pattern as <see cref="Project.Core.Runtime.Employee.RecruitmentRuntimeState"/>.
    /// </summary>
    public sealed class ContractBoardRuntimeState
    {
        /// <summary>
        /// Stable IDs of contracts currently visible on the board (Status == Available).
        /// Contracts are removed from this list when accepted or expired.
        /// </summary>
        public List<string> AvailableContractIds;

        /// <summary>
        /// The in-game date the board was last refreshed with new contracts.
        /// Null before the first board initialization.
        /// </summary>
        public GameDateTime LastBoardRefreshDate;

        /// <summary>
        /// Initializes the board state with an empty available contract list.
        /// </summary>
        public ContractBoardRuntimeState()
        {
            AvailableContractIds = new List<string>();
        }
    }
}
