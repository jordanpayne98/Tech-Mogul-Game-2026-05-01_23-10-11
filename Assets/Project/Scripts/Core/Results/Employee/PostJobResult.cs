namespace Project.Core.Results.Employee
{
    /// <summary>
    /// Result of a PostJobRequest.
    /// Use the static factory methods <see cref="Succeeded"/> and <see cref="Failed"/> to construct instances.
    /// </summary>
    public sealed class PostJobResult
    {
        /// <summary>True if the job post was created successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string when <see cref="Success"/> is true.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// Stable ID of the newly created job post.
        /// Null or empty when <see cref="Success"/> is false.
        /// </summary>
        public string JobPostId { get; }

        private PostJobResult(bool success, string failureReason, string jobPostId)
        {
            Success       = success;
            FailureReason = failureReason;
            JobPostId     = jobPostId;
        }

        /// <summary>Creates a successful result with the new job post's stable ID.</summary>
        public static PostJobResult Succeeded(string jobPostId)
        {
            return new PostJobResult(true, string.Empty, jobPostId);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        public static PostJobResult Failed(string reason)
        {
            return new PostJobResult(false, reason, string.Empty);
        }
    }
}
