namespace Project.Core.Definitions.Product
{
    /// <summary>
    /// Defines how a product generates revenue from customers.
    /// </summary>
    public enum RevenueModel
    {
        OneTimePurchase,
        Subscription,

        /// <summary>Free base product with paid premium features or tiers. Non-functional until Plan 2H implements revenue calculation for this model.</summary>
        Freemium
    }
}
