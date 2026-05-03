using Project.Core.Definitions.Contract;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Contract
{
    /// <summary>
    /// Mutable runtime state for a single active contract.
    /// Tracks live progress, team assignment, and terminal outcome.
    /// The static structure of the contract is held in <see cref="ContractProfile"/>.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public sealed class ContractRuntimeState
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable ID matching the associated <see cref="ContractProfile.Id"/>.</summary>
        public string ContractId;

        // -------------------------------------------------------------------------
        // Status and outcome
        // -------------------------------------------------------------------------

        /// <summary>Current lifecycle status of the contract.</summary>
        public ContractStatus Status;

        /// <summary>Terminal outcome of the contract. None until the contract reaches a terminal state.</summary>
        public ContractOutcome Outcome;

        // -------------------------------------------------------------------------
        // Assignment
        // -------------------------------------------------------------------------

        /// <summary>
        /// Stable ID of the team currently assigned to this contract.
        /// Null when no team has been assigned.
        /// </summary>
        public string AssignedTeamId;

        // -------------------------------------------------------------------------
        // Progress
        // -------------------------------------------------------------------------

        /// <summary>Overall completion percentage in the range 0–100.</summary>
        public int ProgressPercent;

        /// <summary>Number of milestones completed so far.</summary>
        public int MilestonesCompleted;

        /// <summary>Current quality score in the range 0–100.</summary>
        public int QualityScore;

        // -------------------------------------------------------------------------
        // Timestamps
        // -------------------------------------------------------------------------

        /// <summary>
        /// The in-game date the contract was accepted by the player.
        /// Null if the contract has not yet been accepted.
        /// </summary>
        public GameDateTime AcceptedDate;

        /// <summary>
        /// The in-game date the contract reached a terminal state (Completed or Failed).
        /// Null if the contract has not yet completed.
        /// </summary>
        public GameDateTime CompletedDate;

        // -------------------------------------------------------------------------
        // Payment
        // -------------------------------------------------------------------------

        /// <summary>
        /// Amount owed by the client in minor currency units, computed when the contract
        /// reaches a terminal state (Completed or Failed). Zero until outcome is determined.
        /// Read by ProcessMonthlyFinanceUseCase to apply the cash movement to FinanceRuntimeState.
        /// </summary>
        public long PaymentDueMinorUnits;

        /// <summary>
        /// True once the monthly finance cycle has applied PaymentDueMinorUnits to cash.
        /// Prevents duplicate payment application across multiple monthly cycles.
        /// Set to true by ProcessMonthlyFinanceUseCase after the payment is credited.
        /// </summary>
        public bool PaymentApplied;
    }
}
