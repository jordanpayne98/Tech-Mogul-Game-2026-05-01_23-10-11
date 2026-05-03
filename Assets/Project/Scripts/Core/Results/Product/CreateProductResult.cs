namespace Project.Core.Results.Product
{
    /// <summary>
    /// Result returned after attempting to create a new product.
    /// Use the static factory methods to construct instances.
    /// </summary>
    public sealed class CreateProductResult
    {
        /// <summary>Whether the product creation succeeded.</summary>
        public bool Success { get; }

        /// <summary>Stable ID of the newly created product. Only valid when Success is true.</summary>
        public string ProductId { get; }

        /// <summary>Human-readable reason for failure. Only valid when Success is false.</summary>
        public string FailureReason { get; }

        private CreateProductResult(bool success, string productId, string failureReason)
        {
            Success       = success;
            ProductId     = productId;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Creates a successful result carrying the stable ID of the newly created product.
        /// </summary>
        /// <param name="productId">Stable ID assigned to the new product.</param>
        public static CreateProductResult Succeeded(string productId)
        {
            return new CreateProductResult(true, productId, string.Empty);
        }

        /// <summary>
        /// Creates a failure result with a descriptive reason.
        /// </summary>
        /// <param name="reason">Human-readable description of why product creation failed.</param>
        public static CreateProductResult Failed(string reason)
        {
            return new CreateProductResult(false, string.Empty, reason);
        }
    }
}
