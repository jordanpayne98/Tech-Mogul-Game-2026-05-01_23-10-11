using Project.Core.Definitions.Contract;

namespace Project.Core.Events.Contract
{
    /// <summary>
    /// Published when a contract reaches a successful terminal state (Accepted or Excellent outcome).
    /// PaymentDueMinorUnits is stored on ContractRuntimeState and read by Plan 2H for cash processing.
    /// </summary>
    public sealed class ContractCompletedEvent
    {
        /// <summary>Stable ID of the completed contract.</summary>
        public string ContractId { get; }

        /// <summary>Terminal outcome of the contract (Accepted or Excellent).</summary>
        public ContractOutcome Outcome { get; }

        /// <summary>
        /// Payment owed by the client in minor currency units.
        /// Read by Plan 2H to apply to FinanceRuntimeState.
        /// </summary>
        public long PaymentDueMinorUnits { get; }

        /// <summary>
        /// Reputation change computed for this outcome.
        /// Positive value. Applied to CompanyRuntimeState.Reputation by Plan 2H.
        /// </summary>
        public float ReputationChange { get; }

        /// <summary>
        /// Creates a new ContractCompletedEvent.
        /// </summary>
        public ContractCompletedEvent(
            string          contractId,
            ContractOutcome outcome,
            long            paymentDueMinorUnits,
            float           reputationChange)
        {
            ContractId           = contractId;
            Outcome              = outcome;
            PaymentDueMinorUnits = paymentDueMinorUnits;
            ReputationChange     = reputationChange;
        }
    }
}
