namespace Project.Core.Events.Product
{
    /// <summary>
    /// Published after a product is sunset by the player.
    /// Teams are NOT unassigned automatically — the player must unassign them via UnassignTeamUseCase.
    /// Consumers may use this to update UI state or trigger end-of-life analytics.
    /// </summary>
    public sealed class ProductSunsetEvent
    {
        /// <summary>Stable ID of the sunset product.</summary>
        public string ProductId { get; }

        public ProductSunsetEvent(string productId)
        {
            ProductId = productId;
        }
    }
}
