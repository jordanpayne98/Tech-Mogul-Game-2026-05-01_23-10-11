namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Bill of Materials tier for hardware products.
    /// Determines the component quality level used during manufacturing.
    /// </summary>
    public enum BOMTier
    {
        Budget,
        Standard,
        Premium,
        Experimental
    }
}
