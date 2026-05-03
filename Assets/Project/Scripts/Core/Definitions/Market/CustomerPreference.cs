namespace Project.Core.Definitions.Market
{
    /// <summary>
    /// Dimensions of customer preference that influence buying decisions.
    /// Used to weight what customers in a market category value most.
    /// </summary>
    public enum CustomerPreference
    {
        Price,
        Reliability,
        Features,
        Usability,
        Brand,
        Security,
        Performance,
        Support,
        Ecosystem
    }
}
