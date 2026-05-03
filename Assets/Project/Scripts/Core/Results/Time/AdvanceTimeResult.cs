namespace Project.Core.Results.Time
{
    /// <summary>
    /// Result returned after an AdvanceTimeRequest is processed.
    /// Use Succeeded(int) or Failed(string) static factories to construct instances.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class AdvanceTimeResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the time advance completed successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// The new TotalElapsedHours value after advancing, if successful.
        /// Zero on failure.
        /// </summary>
        public int NewTotalElapsedHours { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private AdvanceTimeResult(bool success, string failureReason, int newTotalElapsedHours)
        {
            Success               = success;
            FailureReason         = failureReason;
            NewTotalElapsedHours  = newTotalElapsedHours;
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a successful result with the updated total elapsed hours.
        /// </summary>
        /// <param name="newTotalElapsedHours">The total elapsed hours after the advance.</param>
        public static AdvanceTimeResult Succeeded(int newTotalElapsedHours)
        {
            return new AdvanceTimeResult(true, string.Empty, newTotalElapsedHours);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable explanation of why the advance failed.</param>
        public static AdvanceTimeResult Failed(string reason)
        {
            return new AdvanceTimeResult(false, reason, 0);
        }
    }
}
