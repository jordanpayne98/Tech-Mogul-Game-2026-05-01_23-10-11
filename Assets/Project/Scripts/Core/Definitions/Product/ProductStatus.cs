namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Represents the lifecycle stage of a product from initial conception through end-of-life.
    /// </summary>
    public enum ProductStatus
    {
        Draft,
        InResearch,
        InConcept,
        InPrototype,
        InManufacturingPrep,
        InDevelopment,
        InQA,
        ReadyForLaunch,
        Launched,
        Updating,
        Supported,
        Mature,
        Declining,
        Sunset,
        Cancelled
    }
}
