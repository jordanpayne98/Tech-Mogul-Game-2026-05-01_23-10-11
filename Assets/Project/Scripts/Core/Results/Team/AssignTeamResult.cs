namespace Project.Core.Results.Team
{
    /// <summary>
    /// Result returned after an AssignTeamRequest is processed.
    /// Use Succeeded(string) or Failed(string) static factories to construct instances.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class AssignTeamResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the team was assigned successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// The stable ID of the newly created assignment, if successful.
        /// Empty string on failure.
        /// </summary>
        public string AssignmentId { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private AssignTeamResult(bool success, string failureReason, string assignmentId)
        {
            Success      = success;
            FailureReason = failureReason;
            AssignmentId = assignmentId;
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a successful result with the stable ID of the new assignment.
        /// </summary>
        /// <param name="assignmentId">Stable ID assigned to the created assignment.</param>
        public static AssignTeamResult Succeeded(string assignmentId)
        {
            return new AssignTeamResult(true, string.Empty, assignmentId);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable explanation of why the assignment failed.</param>
        public static AssignTeamResult Failed(string reason)
        {
            return new AssignTeamResult(false, reason, string.Empty);
        }
    }
}
