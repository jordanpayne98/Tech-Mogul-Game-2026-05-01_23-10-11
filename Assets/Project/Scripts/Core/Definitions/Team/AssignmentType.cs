namespace Project.Core.Definitions.Team
{
    /// <summary>
    /// Identifies the specific kind of work a team is assigned to perform.
    /// Each value maps to a distinct phase or activity in the product or company lifecycle.
    /// </summary>
    public enum AssignmentType
    {
        ProductResearch,
        ProductConcept,
        ProductDesign,
        SoftwareDevelopment,
        HardwarePrototyping,
        QATesting,
        LaunchPreparation,
        PostLaunchSupport,
        ProductUpdates,
        MarketingCampaign,
        InfrastructureScaling,
        ContractWork,
        ResearchProject,
        ManufacturingSetup,
        CrisisResponse
    }
}
