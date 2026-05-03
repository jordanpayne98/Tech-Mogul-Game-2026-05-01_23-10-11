namespace Project.Core.Definitions.Market
{
    /// <summary>
    /// Customer segments that make up the addressable market for products.
    /// Used to weight demand distribution across market categories.
    /// </summary>
    public enum CustomerSegment
    {
        Consumer,
        SmallBusiness,
        Enterprise,
        Developer,
        Gamer,
        Creator,
        Education,
        Government,
        HardwareEnthusiast,
        BudgetBuyer,
        PremiumBuyer
    }
}
