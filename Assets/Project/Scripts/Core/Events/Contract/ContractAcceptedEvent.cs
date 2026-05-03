namespace Project.Core.Events.Contract
{
    /// <summary>
    /// Published when the player accepts a contract from the board.
    /// The contract status moves to Accepted. A team must then be assigned separately.
    /// </summary>
    public sealed class ContractAcceptedEvent
    {
        /// <summary>Stable ID of the accepted contract.</summary>
        public string ContractId { get; }

        /// <summary>
        /// Creates a new ContractAcceptedEvent.
        /// </summary>
        public ContractAcceptedEvent(string contractId)
        {
            ContractId = contractId;
        }
    }
}
