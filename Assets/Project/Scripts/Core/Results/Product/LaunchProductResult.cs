namespace Project.Core.Results.Product
{
    /// <summary>
    /// Result returned after attempting to launch a product.
    /// Use the static factory methods to construct instances.
    /// </summary>
    public sealed class LaunchProductResult
    {
        /// <summary>Whether the launch succeeded.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Only valid when Success is false.</summary>
        public string FailureReason { get; }

        /// <summary>Review score computed at launch. Only valid when Success is true.</summary>
        public int InitialReviewScore { get; }

        /// <summary>Initial active user count computed at launch. Only valid when Success is true.</summary>
        public int InitialActiveUsers { get; }

        private LaunchProductResult(
            bool success,
            string failureReason,
            int initialReviewScore,
            int initialActiveUsers)
        {
            Success             = success;
            FailureReason       = failureReason;
            InitialReviewScore  = initialReviewScore;
            InitialActiveUsers  = initialActiveUsers;
        }

        /// <summary>
        /// Creates a successful result carrying the computed launch metrics.
        /// </summary>
        public static LaunchProductResult Succeeded(int reviewScore, int activeUsers)
        {
            return new LaunchProductResult(true, string.Empty, reviewScore, activeUsers);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        public static LaunchProductResult Failed(string reason)
        {
            return new LaunchProductResult(false, reason, 0, 0);
        }
    }
}
