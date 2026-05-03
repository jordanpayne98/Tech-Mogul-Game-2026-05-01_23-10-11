namespace Project.Core.Results.Product
{
    /// <summary>
    /// Result returned after attempting to sunset a product.
    /// Use the static factory methods to construct instances.
    /// </summary>
    public sealed class SunsetProductResult
    {
        /// <summary>Whether the sunset operation succeeded.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Only valid when Success is false.</summary>
        public string FailureReason { get; }

        private SunsetProductResult(bool success, string failureReason)
        {
            Success       = success;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful result.</summary>
        public static SunsetProductResult Succeeded()
        {
            return new SunsetProductResult(true, string.Empty);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        public static SunsetProductResult Failed(string reason)
        {
            return new SunsetProductResult(false, reason);
        }
    }
}
