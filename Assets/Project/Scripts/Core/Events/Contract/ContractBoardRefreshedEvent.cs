using Project.Core.Runtime.Time;

namespace Project.Core.Events.Contract
{
    /// <summary>
    /// Published when the contract board is refreshed with new contracts on a monthly boundary.
    /// Also published when the board is first initialized at session start.
    /// </summary>
    public sealed class ContractBoardRefreshedEvent
    {
        /// <summary>Number of new contracts added to the board in this refresh.</summary>
        public int NewContractCount { get; }

        /// <summary>The in-game date on which the refresh occurred.</summary>
        public GameDateTime RefreshDate { get; }

        /// <summary>
        /// Creates a new ContractBoardRefreshedEvent.
        /// </summary>
        public ContractBoardRefreshedEvent(int newContractCount, GameDateTime refreshDate)
        {
            NewContractCount = newContractCount;
            RefreshDate      = refreshDate;
        }
    }
}
