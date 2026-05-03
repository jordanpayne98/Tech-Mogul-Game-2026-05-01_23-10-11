namespace Project.Core.Results.Research
{
    /// <summary>
    /// Result of a <see cref="Project.Core.Requests.Research.StartResearchRequest"/>.
    /// Use the static factory methods <see cref="Succeeded"/> and <see cref="Failed"/> to construct instances.
    /// </summary>
    public sealed class StartResearchResult
    {
        /// <summary>True if the research project was successfully started.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string when <see cref="Success"/> is true.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// Stable ID of the research project that was started.
        /// Empty string when <see cref="Success"/> is false.
        /// </summary>
        public string ResearchProjectId { get; }

        private StartResearchResult(bool success, string failureReason, string researchProjectId)
        {
            Success = success;
            FailureReason = failureReason;
            ResearchProjectId = researchProjectId;
        }

        /// <summary>Creates a successful result identifying which research project was started.</summary>
        public static StartResearchResult Succeeded(string researchProjectId)
        {
            return new StartResearchResult(true, string.Empty, researchProjectId);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        public static StartResearchResult Failed(string reason)
        {
            return new StartResearchResult(false, reason, string.Empty);
        }
    }
}
