namespace Project.Core.Definitions.Time
{
    /// <summary>
    /// Identifies the category of event that can interrupt continuous time advancement.
    /// Used alongside InterruptionFilter to determine which events pause the simulation.
    /// </summary>
    public enum InterruptionType
    {
        ProductPhaseComplete,
        ProductReadyForLaunch,
        CandidateResponse,
        ContractMilestone,
        ContractDeadline,
        ContractCompleted,
        ContractFailed,
        MonthlyFinanceReport,
        LowRunway,
        /// <summary>Cash runway has dropped to or below the critical threshold. Triggers a Continue interruption.</summary>
        CriticalRunway,
        CompetitorLaunch,
        InfrastructureIncident,
        MajorDefect,
        TeamMoraleCrisis,
        ResearchComplete,
        /// <summary>A research project has finished and its unlocks have been granted. Triggers a Continue interruption.</summary>
        ResearchCompleted,
        MarketTrendShift,
        ProductLaunchDay
    }
}
