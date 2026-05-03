namespace Project.Core.Results.Contract
{
    /// <summary>
    /// Result returned after an AcceptContractRequest is processed.
    /// Use <see cref="Succeeded"/> or <see cref="Failed"/> static factories to construct instances.
    /// Stub — acceptance logic is deferred to the Application layer use case (Phase 2).
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class AcceptContractResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the contract was successfully accepted.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// Stable ID of the contract that was accepted, if successful.
        /// Empty string on failure.
        /// </summary>
        public string ContractId { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private AcceptContractResult(bool success, string failureReason, string contractId)
        {
            Success       = success;
            FailureReason = failureReason;
            ContractId    = contractId;
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a successful result confirming the contract was accepted.
        /// </summary>
        /// <param name="contractId">Stable ID of the accepted contract.</param>
        public static AcceptContractResult Succeeded(string contractId)
        {
            return new AcceptContractResult(true, string.Empty, contractId);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable explanation of why the acceptance failed.</param>
        public static AcceptContractResult Failed(string reason)
        {
            return new AcceptContractResult(false, reason, string.Empty);
        }
    }
}
