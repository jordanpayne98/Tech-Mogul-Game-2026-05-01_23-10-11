namespace Project.Core.Events.Contract
{
    /// <summary>
    /// Published when a contract fails — either due to a missed deadline or
    /// completion with a quality score below the minimum accepted threshold.
    /// PaymentDueMinorUnits is stored on ContractRuntimeState and read by Plan 2H.
    /// </summary>
    public sealed class ContractFailedEvent
    {
        /// <summary>Stable ID of the failed contract.</summary>
        public string ContractId { get; }

        /// <summary>Human-readable description of why the contract failed.</summary>
        public string Reason { get; }

        /// <summary>
        /// Partial payment owed in minor currency units (may be 0 if no failure payment applies).
        /// Read by Plan 2H to apply to FinanceRuntimeState.
        /// </summary>
        public long PaymentDueMinorUnits { get; }

        /// <summary>
        /// Reputation change computed for this outcome. Negative value.
        /// Applied to CompanyRuntimeState.Reputation by Plan 2H.
        /// </summary>
        public float ReputationChange { get; }

        /// <summary>
        /// Creates a new ContractFailedEvent.
        /// </summary>
        public ContractFailedEvent(
            string contractId,
            string reason,
            long   paymentDueMinorUnits,
            float  reputationChange)
        {
            ContractId           = contractId;
            Reason               = reason;
            PaymentDueMinorUnits = paymentDueMinorUnits;
            ReputationChange     = reputationChange;
        }
    }
}
