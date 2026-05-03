namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Top-level family that groups all product types into Software or Hardware.
    /// Stored explicitly on ProductProfile for convenience even though it is derivable from ProductCategory.
    /// </summary>
    public enum ProductFamily
    {
        Software,
        Hardware
    }
}
