namespace Project.Core.Results.Report
{
    /// <summary>
    /// Result returned after attempting to archive a report.
    /// Use static factory methods to construct instances.
    /// </summary>
    public sealed class ArchiveReportResult
    {
        /// <summary>True if the operation succeeded.</summary>
        public bool Success { get; }

        /// <summary>Reason for failure. Empty string when Success is true.</summary>
        public string FailureReason { get; }

        private ArchiveReportResult(bool success, string failureReason)
        {
            Success       = success;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful result.</summary>
        public static ArchiveReportResult Succeeded()
        {
            return new ArchiveReportResult(true, string.Empty);
        }

        /// <summary>Creates a failed result with the specified reason.</summary>
        public static ArchiveReportResult Failed(string reason)
        {
            return new ArchiveReportResult(false, reason);
        }
    }
}
