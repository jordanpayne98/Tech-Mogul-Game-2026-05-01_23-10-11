namespace Project.Core.Events.Market
{
    /// <summary>
    /// Published when an AI competitor launches a new product into the market.
    /// Consumed by ReportEventHandler to generate a competitor launch report.
    /// </summary>
    public sealed class CompetitorProductLaunchedEvent
    {
        /// <summary>Stable ID of the competitor that launched the product.</summary>
        public string CompetitorId { get; }

        /// <summary>Stable ID of the competitor product that was launched.</summary>
        public string CompetitorProductId { get; }

        public CompetitorProductLaunchedEvent(string competitorId, string competitorProductId)
        {
            CompetitorId        = competitorId;
            CompetitorProductId = competitorProductId;
        }
    }
}
