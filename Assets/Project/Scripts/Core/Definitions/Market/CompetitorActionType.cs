namespace Project.Core.Definitions.Market
{
    /// <summary>
    /// MVP action types available to AI competitors each simulation week.
    /// Competitors may perform at most one action per week.
    /// Defined in Plan 2J, GDD_10.5.
    /// </summary>
    public enum CompetitorActionType
    {
        /// <summary>Competitor launches a new product into one of its market focus categories.</summary>
        LaunchProduct,

        /// <summary>Competitor changes the price of one of its active products.</summary>
        ChangePrice,

        /// <summary>Competitor expands into a new market category.</summary>
        EnterMarket
    }
}
