namespace Project.Core.Results.Team
{
    /// <summary>
    /// Result returned after an UnassignTeamRequest is processed.
    /// Use Succeeded(string) or Failed(string) static factories to construct instances.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class UnassignTeamResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the team was unassigned successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// The stable ID of the assignment that was cleared, if successful.
        /// Empty string on failure.
        /// </summary>
        public string PreviousAssignmentId { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private UnassignTeamResult(bool success, string failureReason, string previousAssignmentId)
        {
            Success              = success;
            FailureReason        = failureReason;
            PreviousAssignmentId = previousAssignmentId;
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a successful result with the ID of the cleared assignment.
        /// </summary>
        /// <param name="previousAssignmentId">Stable ID of the assignment that was unlinked from the team.</param>
        public static UnassignTeamResult Succeeded(string previousAssignmentId)
        {
            return new UnassignTeamResult(true, string.Empty, previousAssignmentId);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable explanation of why unassignment failed.</param>
        public static UnassignTeamResult Failed(string reason)
        {
            return new UnassignTeamResult(false, reason, string.Empty);
        }
    }
}
