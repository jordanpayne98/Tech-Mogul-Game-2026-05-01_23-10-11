namespace Project.Core.Events.Finance
{
    /// <summary>
    /// Published when runway drops to or below the low runway threshold but is above the critical threshold.
    /// Does not interrupt Continue. Used to surface a non-blocking financial health warning.
    /// If runway is at or below the critical threshold, an InterruptionRequest is raised instead.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class LowRunwayWarningEvent
    {
        /// <summary>Stable ID of the company with low runway.</summary>
        public string CompanyId { get; }

        /// <summary>Current estimated months of runway remaining.</summary>
        public int RunwayMonths { get; }

        /// <summary>
        /// Creates a new LowRunwayWarningEvent.
        /// </summary>
        public LowRunwayWarningEvent(string companyId, int runwayMonths)
        {
            CompanyId    = companyId;
            RunwayMonths = runwayMonths;
        }
    }
}
