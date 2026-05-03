namespace Project.Core.Results.Report
{
    /// <summary>
    /// Result returned after attempting to generate and deliver a report to the player inbox.
    /// Use static factory methods to construct instances.
    /// </summary>
    public sealed class GenerateReportResult
    {
        /// <summary>True if the report was successfully generated and delivered.</summary>
        public bool Success { get; }

        /// <summary>Stable ID of the generated report. Empty string when Success is false.</summary>
        public string ReportId { get; }

        /// <summary>Reason for failure. Empty string when Success is true.</summary>
        public string FailureReason { get; }

        private GenerateReportResult(bool success, string reportId, string failureReason)
        {
            Success       = success;
            ReportId      = reportId;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Creates a successful result with the stable ID of the generated report.
        /// </summary>
        public static GenerateReportResult Succeeded(string reportId)
        {
            return new GenerateReportResult(true, reportId, string.Empty);
        }

        /// <summary>
        /// Creates a failed result with a reason describing why report generation failed.
        /// </summary>
        public static GenerateReportResult Failed(string reason)
        {
            return new GenerateReportResult(false, string.Empty, reason);
        }
    }
}
