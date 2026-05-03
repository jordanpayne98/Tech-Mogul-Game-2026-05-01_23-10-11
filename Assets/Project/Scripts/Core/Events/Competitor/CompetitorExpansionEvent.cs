using Project.Core.Definitions.Market;

namespace Project.Core.Events.Competitor
{
    /// <summary>
    /// Published when an AI competitor expands into a new market category.
    /// Defined in Plan 2J, GDD_10.5.
    /// </summary>
    public sealed class CompetitorExpansionEvent
    {
        /// <summary>Stable ID of the competitor that expanded.</summary>
        public string CompetitorId { get; }

        /// <summary>The new market category the competitor entered.</summary>
        public MarketCategoryType NewCategory { get; }

        public CompetitorExpansionEvent(string competitorId, MarketCategoryType newCategory)
        {
            CompetitorId = competitorId;
            NewCategory  = newCategory;
        }
    }
}
