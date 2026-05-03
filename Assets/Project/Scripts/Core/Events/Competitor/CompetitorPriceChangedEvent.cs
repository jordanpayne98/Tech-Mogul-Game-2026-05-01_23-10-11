namespace Project.Core.Events.Competitor
{
    /// <summary>
    /// Published when an AI competitor changes the price of one of its active products.
    /// Defined in Plan 2J, GDD_10.5.
    /// </summary>
    public sealed class CompetitorPriceChangedEvent
    {
        /// <summary>Stable ID of the competitor that changed the price.</summary>
        public string CompetitorId { get; }

        /// <summary>Stable ID of the competitor product whose price changed.</summary>
        public string ProductId { get; }

        /// <summary>The new price in minor currency units after the change.</summary>
        public long NewPriceMinorUnits { get; }

        public CompetitorPriceChangedEvent(string competitorId, string productId, long newPriceMinorUnits)
        {
            CompetitorId       = competitorId;
            ProductId          = productId;
            NewPriceMinorUnits = newPriceMinorUnits;
        }
    }
}
