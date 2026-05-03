namespace Project.Core.Results.Team
{
    /// <summary>
    /// Result returned after a CreateTeamRequest is processed.
    /// Use Succeeded(string) or Failed(string) static factories to construct instances.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class CreateTeamResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the team was created successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// The stable ID of the newly created team, if successful.
        /// Empty string on failure.
        /// </summary>
        public string TeamId { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private CreateTeamResult(bool success, string failureReason, string teamId)
        {
            Success       = success;
            FailureReason = failureReason;
            TeamId        = teamId;
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a successful result with the stable ID of the new team.
        /// </summary>
        /// <param name="teamId">Stable ID assigned to the created team.</param>
        public static CreateTeamResult Succeeded(string teamId)
        {
            return new CreateTeamResult(true, string.Empty, teamId);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable explanation of why team creation failed.</param>
        public static CreateTeamResult Failed(string reason)
        {
            return new CreateTeamResult(false, reason, string.Empty);
        }
    }
}
