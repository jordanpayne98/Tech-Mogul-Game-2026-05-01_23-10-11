namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Dimensions used to evaluate a product's scored attributes.
    /// Score values use a 0–100 range per dimension.
    /// </summary>
    public enum ProductScoreDimension
    {
        Overall,
        Quality,
        Creativity,
        Stability
    }
}
