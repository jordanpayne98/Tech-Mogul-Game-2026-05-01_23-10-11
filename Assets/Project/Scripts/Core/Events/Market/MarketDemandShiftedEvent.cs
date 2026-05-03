using Project.Core.Definitions.Market;

namespace Project.Core.Events.Market
{
    /// <summary>
    /// Published when a market category's total demand changes after a weekly demand adjustment.
    /// Consumers may react to significant demand shifts (e.g. updating reports, notifying the player).
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public sealed class MarketDemandShiftedEvent
    {
        /// <summary>The market category whose demand changed.</summary>
        public MarketCategoryType Category { get; }

        /// <summary>The new total demand value after the shift.</summary>
        public int NewDemand { get; }

        /// <summary>The total demand value before the shift.</summary>
        public int PreviousDemand { get; }

        /// <summary>
        /// Creates a new <see cref="MarketDemandShiftedEvent"/>.
        /// </summary>
        /// <param name="category">The affected market category.</param>
        /// <param name="newDemand">New total demand.</param>
        /// <param name="previousDemand">Previous total demand before shift.</param>
        public MarketDemandShiftedEvent(MarketCategoryType category, int newDemand, int previousDemand)
        {
            Category       = category;
            NewDemand      = newDemand;
            PreviousDemand = previousDemand;
        }
    }
}
