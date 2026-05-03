using Project.Core.Definitions.Product;

namespace Project.Core.Events.Product
{
    /// <summary>
    /// Published after a product is successfully created.
    /// Consumers may use this to update UI state, trigger notifications,
    /// or initialise related systems.
    /// </summary>
    public sealed class ProductCreatedEvent
    {
        /// <summary>Stable ID of the newly created product.</summary>
        public string ProductId { get; }

        /// <summary>Top-level family of the created product (Software or Hardware).</summary>
        public ProductFamily Family { get; }

        public ProductCreatedEvent(string productId, ProductFamily family)
        {
            ProductId = productId;
            Family    = family;
        }
    }
}
