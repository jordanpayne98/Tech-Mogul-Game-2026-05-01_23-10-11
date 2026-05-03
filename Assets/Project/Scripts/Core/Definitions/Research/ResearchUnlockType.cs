namespace Project.Core.Definitions.Research
{
    /// <summary>
    /// The category of capability unlocked when a research project completes.
    /// Used by <see cref="ResearchUnlock"/> to describe what a completed project grants.
    /// Values map to GDD_13 research unlock definitions.
    /// </summary>
    public enum ResearchUnlockType
    {
        NewProductType,
        NewFeature,
        BetterInfrastructure,
        BetterComponents,
        BetterManufacturing,
        BetterQA,
        BetterSecurity,
        BetterMarketingAnalytics,
        NewRevenueModel,
        EcosystemFeature
    }
}
