using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Events.Product;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Requests.Product;
using Project.Core.Results.Product;
using Project.Core.Runtime;

namespace Project.Application.UseCases.Product
{
    /// <summary>
    /// Application-layer use case for sunsetting a launched product.
    /// The product must be in Launched or Supported status.
    /// Teams are NOT automatically unassigned — the player must call UnassignTeamUseCase first.
    /// Only changes the product status to Sunset and publishes a ProductSunsetEvent.
    /// </summary>
    public sealed class SunsetProductUseCase
    {
        private readonly IProductService _productService;
        private readonly IEventBus       _eventBus;

        public SunsetProductUseCase(IProductService productService, IEventBus eventBus)
        {
            _productService = productService;
            _eventBus       = eventBus;
        }

        /// <summary>
        /// Executes the sunset product use case.
        /// Returns a failure result if the product is not found or is not eligible for sunset.
        /// On success, sets the product to Sunset status and publishes a ProductSunsetEvent.
        /// Teams assigned to the sunset product are NOT unassigned.
        /// </summary>
        public SunsetProductResult Execute(SunsetProductRequest request, GameSessionState sessionState)
        {
            // ── Find product runtime state ──────────────────────────────────────────

            var productState = sessionState.ProductStates
                .FirstOrDefault(s => s.ProductId == request.ProductId);

            if (productState == null)
            {
                return SunsetProductResult.Failed("Product not found");
            }

            // ── Validate sunset eligibility ─────────────────────────────────────────

            if (!_productService.ValidateSunset(productState, out string failureReason))
            {
                return SunsetProductResult.Failed(failureReason);
            }

            // ── Transition to Sunset ────────────────────────────────────────────────

            // Do NOT unassign teams. The player must explicitly call UnassignTeamUseCase.
            productState.Status = ProductStatus.Sunset;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[SunsetProductUseCase] Product sunset. ProductId: {request.ProductId}");

            // ── Publish event ───────────────────────────────────────────────────────

            _eventBus.Publish(new ProductSunsetEvent(request.ProductId));

            return SunsetProductResult.Succeeded();
        }
    }
}
