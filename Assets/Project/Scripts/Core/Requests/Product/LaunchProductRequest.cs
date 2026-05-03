namespace Project.Core.Requests.Product
{
    /// <summary>
    /// Carries the data required to manually launch a product.
    /// The product must be in ReadyForLaunch status for the request to succeed.
    /// </summary>
    public sealed class LaunchProductRequest
    {
        /// <summary>Stable ID of the product to launch.</summary>
        public string ProductId { get; }

        public LaunchProductRequest(string productId)
        {
            ProductId = productId;
        }
    }
}
