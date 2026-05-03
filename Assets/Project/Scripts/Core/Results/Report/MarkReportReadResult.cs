namespace Project.Core.Results.Report
{
    /// <summary>
    /// Result returned after attempting to mark a report as read.
    /// Use static factory methods to construct instances.
    /// </summary>
    public sealed class MarkReportReadResult
    {
        /// <summary>True if the operation succeeded.</summary>
        public bool Success { get; }

        /// <summary>Reason for failure. Empty string when Success is true.</summary>
        public string FailureReason { get; }

        private MarkReportReadResult(bool success, string failureReason)
        {
            Success       = success;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful result.</summary>
        public static MarkReportReadResult Succeeded()
        {
            return new MarkReportReadResult(true, string.Empty);
        }

        /// <summary>Creates a failed result with the specified reason.</summary>
        public static MarkReportReadResult Failed(string reason)
        {
            return new MarkReportReadResult(false, reason);
        }
    }
}
