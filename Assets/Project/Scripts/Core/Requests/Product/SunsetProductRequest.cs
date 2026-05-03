namespace Project.Core.Requests.Product
{
    /// <summary>
    /// Carries the data required to sunset a product.
    /// The product must be in Launched or Supported status for the request to succeed.
    /// Teams are NOT automatically unassigned — the player must do so manually.
    /// </summary>
    public sealed class SunsetProductRequest
    {
        /// <summary>Stable ID of the product to sunset.</summary>
        public string ProductId { get; }

        public SunsetProductRequest(string productId)
        {
            ProductId = productId;
        }
    }
}
