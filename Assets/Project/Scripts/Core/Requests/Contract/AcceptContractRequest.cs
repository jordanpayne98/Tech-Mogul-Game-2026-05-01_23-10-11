namespace Project.Core.Requests.Contract
{
    /// <summary>
    /// Request to accept a contract from the marketplace.
    /// Consumed by the contract acceptance use case. No logic is executed inside this class.
    /// Stub — acceptance logic is deferred to the Application layer use case (Phase 2).
    /// </summary>
    public sealed class AcceptContractRequest
    {
        /// <summary>Stable ID of the contract to accept. Must match a <c>ContractProfile.Id</c>.</summary>
        public string ContractId { get; }

        /// <summary>
        /// Creates a new AcceptContractRequest.
        /// </summary>
        /// <param name="contractId">Stable ID of the contract to accept.</param>
        public AcceptContractRequest(string contractId)
        {
            ContractId = contractId;
        }
    }
}
