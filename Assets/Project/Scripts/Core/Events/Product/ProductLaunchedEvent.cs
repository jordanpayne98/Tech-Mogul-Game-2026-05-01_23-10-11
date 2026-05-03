using Project.Core.Runtime.Time;

namespace Project.Core.Events.Product
{
    /// <summary>
    /// Published after a product is successfully launched by the player.
    /// Consumers may use this to trigger market entry logic, UI updates,
    /// or analytics.
    /// </summary>
    public sealed class ProductLaunchedEvent
    {
        /// <summary>Stable ID of the launched product.</summary>
        public string ProductId { get; }

        /// <summary>In-game date on which the product was launched.</summary>
        public GameDateTime LaunchDate { get; }

        public ProductLaunchedEvent(string productId, GameDateTime launchDate)
        {
            ProductId  = productId;
            LaunchDate = launchDate;
        }
    }
}
