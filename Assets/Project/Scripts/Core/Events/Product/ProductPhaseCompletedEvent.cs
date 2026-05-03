using Project.Core.Definitions.Product;

namespace Project.Core.Events.Product
{
    /// <summary>
    /// Published after a product automatically completes a development phase
    /// and transitions to the next phase.
    /// Consumers may use this to update UI state, show notifications,
    /// or trigger phase-specific logic.
    /// </summary>
    public sealed class ProductPhaseCompletedEvent
    {
        /// <summary>Stable ID of the product that completed a phase.</summary>
        public string ProductId { get; }

        /// <summary>The phase that was just completed.</summary>
        public ProductStatus CompletedPhase { get; }

        /// <summary>The phase the product has transitioned into.</summary>
        public ProductStatus NextPhase { get; }

        public ProductPhaseCompletedEvent(
            string productId,
            ProductStatus completedPhase,
            ProductStatus nextPhase)
        {
            ProductId      = productId;
            CompletedPhase = completedPhase;
            NextPhase      = nextPhase;
        }
    }
}
