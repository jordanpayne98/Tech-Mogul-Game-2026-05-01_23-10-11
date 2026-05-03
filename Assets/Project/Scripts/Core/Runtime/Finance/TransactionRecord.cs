using Project.Core.Definitions.Finance;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Finance
{
    /// <summary>
    /// Immutable record of a single financial transaction.
    /// Either ExpenseCategory or RevenueSource is set depending on TransactionType —
    /// a transaction cannot be both income and expense simultaneously.
    /// All amounts are in minor currency units (e.g. pence for GBP).
    /// </summary>
    public sealed class TransactionRecord
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable unique ID for this transaction.</summary>
        public string Id;

        // -------------------------------------------------------------------------
        // Classification
        // -------------------------------------------------------------------------

        /// <summary>Whether this transaction is incoming revenue or outgoing expense.</summary>
        public TransactionType Type;

        /// <summary>
        /// Expense category. Set when Type is Expense; null when Type is Income.
        /// </summary>
        public ExpenseCategory? ExpenseCategory;

        /// <summary>
        /// Revenue source. Set when Type is Income; null when Type is Expense.
        /// </summary>
        public RevenueSource? RevenueSource;

        // -------------------------------------------------------------------------
        // Amount
        // -------------------------------------------------------------------------

        /// <summary>Transaction amount in minor currency units. Always positive.</summary>
        public long AmountMinorUnits;

        // -------------------------------------------------------------------------
        // Description and entity linkage
        // -------------------------------------------------------------------------

        /// <summary>Human-readable description of the transaction (e.g. "Monthly payroll disbursement").</summary>
        public string Description;

        /// <summary>
        /// Stable ID of the entity that caused this transaction (e.g. employee ID, product ID, contract ID).
        /// Null if no specific entity is responsible.
        /// </summary>
        public string RelatedEntityId;

        /// <summary>
        /// Type name of the related entity (e.g. "Employee", "Product", "Contract").
        /// Stored as a stable string so it survives renames. Null if RelatedEntityId is null.
        /// </summary>
        public string RelatedEntityType;

        // -------------------------------------------------------------------------
        // Date
        // -------------------------------------------------------------------------

        /// <summary>The game date on which this transaction occurred.</summary>
        public GameDateTime Date;
    }
}
