namespace Project.Core.Results.Report
{
    /// <summary>
    /// Result returned after attempting to delete a report.
    /// Use static factory methods to construct instances.
    /// </summary>
    public sealed class DeleteReportResult
    {
        /// <summary>True if the operation succeeded.</summary>
        public bool Success { get; }

        /// <summary>Reason for failure. Empty string when Success is true.</summary>
        public string FailureReason { get; }

        private DeleteReportResult(bool success, string failureReason)
        {
            Success       = success;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful result.</summary>
        public static DeleteReportResult Succeeded()
        {
            return new DeleteReportResult(true, string.Empty);
        }

        /// <summary>Creates a failed result with the specified reason.</summary>
        public static DeleteReportResult Failed(string reason)
        {
            return new DeleteReportResult(false, reason);
        }
    }
}
