using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Events.Product;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Product;
using Project.Core.Results.Product;
using Project.Core.Runtime;
using Project.Core.Services.Product;

namespace Project.Application.UseCases.Product
{
    /// <summary>
    /// Application-layer use case for manually launching a product.
    /// The product must be in ReadyForLaunch status.
    /// For software products, computes the initial review score and active user count.
    /// For hardware products, validates launch stock and computes the initial review score and unit margin.
    /// Updates the product runtime state and publishes a ProductLaunchedEvent.
    /// </summary>
    public sealed class LaunchProductUseCase
    {
        private readonly IProductService        _productService;
        private readonly ProductService         _concreteService;
        private readonly HardwareMetricsService _hardwareService;
        private readonly IProductTuning         _tuning;
        private readonly IEventBus              _eventBus;

        public LaunchProductUseCase(
            IProductService        productService,
            ProductService         concreteService,
            HardwareMetricsService hardwareService,
            IProductTuning         tuning,
            IEventBus              eventBus)
        {
            _productService  = productService;
            _concreteService = concreteService;
            _hardwareService = hardwareService;
            _tuning          = tuning;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Executes the launch product use case.
        /// Returns a failure result if the product is not found or is not ready for launch.
        /// On success, sets the product to Launched, calculates initial metrics (by family),
        /// and publishes a ProductLaunchedEvent.
        /// </summary>
        public LaunchProductResult Execute(LaunchProductRequest request, GameSessionState sessionState)
        {
            // ── Find product runtime state ──────────────────────────────────────────

            var productState = sessionState.ProductStates
                .FirstOrDefault(s => s.ProductId == request.ProductId);

            if (productState == null)
            {
                return LaunchProductResult.Failed("Product not found");
            }

            // ── Validate launch eligibility ─────────────────────────────────────────

            if (!_productService.ValidateLaunch(productState, out string failureReason))
            {
                return LaunchProductResult.Failed(failureReason);
            }

            // ── Find product profile ────────────────────────────────────────────────

            var profile = sessionState.ProductProfiles
                .FirstOrDefault(p => p.Id == request.ProductId);

            if (profile == null)
            {
                return LaunchProductResult.Failed("Product profile not found");
            }

            // ── Hardware-specific pre-launch validation ─────────────────────────────

            if (profile.Family == ProductFamily.Hardware)
            {
                var hardwareMetrics = sessionState.HardwareMetrics
                    .FirstOrDefault(m => m.ProductId == request.ProductId);

                if (hardwareMetrics == null
                    || hardwareMetrics.LaunchStock <= 0
                    || hardwareMetrics.CurrentInventory <= 0)
                {
                    return LaunchProductResult.Failed("Hardware product requires positive launch stock");
                }
            }

            // ── Transition to Launched ──────────────────────────────────────────────

            productState.Status     = ProductStatus.Launched;
            productState.LaunchDate = sessionState.TimeState.CurrentDate;

            // ── Software-specific launch processing ─────────────────────────────────

            if (profile.Family == ProductFamily.Software)
            {
                var metrics = sessionState.SoftwareMetrics
                    .FirstOrDefault(m => m.ProductId == request.ProductId);

                var budget = sessionState.ProductBudgets
                    .FirstOrDefault(b => b.ProductId == request.ProductId);

                if (metrics != null)
                {
                    // [Placeholder] Feature satisfaction derived from feature scope for MVP.
                    metrics.FeatureSatisfaction = _concreteService.ComputeFeatureSatisfaction(
                        profile.FeatureScope, _tuning);

                    // Compute review score using placeholder quality = qualityTarget for MVP.
                    int reviewScore = _concreteService.ComputeReviewScore(
                        productState, metrics, profile.QualityTarget, _tuning);

                    productState.ReviewScore       = reviewScore;
                    productState.RecentReviewScore = reviewScore;

                    // [Placeholder] Quality score = qualityTarget for MVP.
                    if (productState.ScoreValues.ContainsKey(ProductScoreDimension.Quality))
                    {
                        productState.ScoreValues[ProductScoreDimension.Quality] = profile.QualityTarget;
                    }

                    // Compute initial active user count.
                    int initialUsers = (budget != null)
                        ? _concreteService.ComputeInitialActiveUsers(budget, reviewScore, _tuning)
                        : _concreteService.ComputeInitialActiveUsers(
                            new Core.Runtime.Product.ProductBudgetProfile { ProductId = request.ProductId },
                            reviewScore,
                            _tuning);

                    productState.ActiveUsers = initialUsers;

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[LaunchProductUseCase] Software product launched. ProductId: {request.ProductId}, "
                        + $"ReviewScore: {reviewScore}, InitialActiveUsers: {initialUsers}");
                }
            }

            // ── Hardware-specific launch processing ─────────────────────────────────

            else if (profile.Family == ProductFamily.Hardware)
            {
                var hardwareMetrics = sessionState.HardwareMetrics
                    .FirstOrDefault(m => m.ProductId == request.ProductId);

                if (hardwareMetrics != null)
                {
                    // Compute hardware review score.
                    int reviewScore = _hardwareService.ComputeHardwareReviewScore(
                        hardwareMetrics, profile.QualityTarget, _tuning);

                    productState.ReviewScore       = reviewScore;
                    productState.RecentReviewScore = reviewScore;

                    // Set quality score, clamped to [0, 100].
                    int qualityScore = System.Math.Clamp(
                        profile.QualityTarget + _hardwareService.GetQualityBonus(hardwareMetrics.BOMTier, _tuning),
                        0, 100);

                    if (productState.ScoreValues.ContainsKey(ProductScoreDimension.Quality))
                    {
                        productState.ScoreValues[ProductScoreDimension.Quality] = qualityScore;
                    }

                    // [Placeholder] Unit margin: price minus manufacturing cost. Ignores retailer margin.
                    hardwareMetrics.UnitMarginMinorUnits =
                        profile.PriceMinorUnits - hardwareMetrics.ManufacturingCostPerUnitMinorUnits;

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[LaunchProductUseCase] Hardware product launched. ProductId: {request.ProductId}, "
                        + $"ReviewScore: {reviewScore}, QualityScore: {qualityScore}, "
                        + $"UnitMargin: {hardwareMetrics.UnitMarginMinorUnits}, "
                        + $"LaunchStock: {hardwareMetrics.LaunchStock}");
                }
            }

            // ── Publish event ───────────────────────────────────────────────────────

            _eventBus.Publish(new ProductLaunchedEvent(
                request.ProductId,
                sessionState.TimeState.CurrentDate));

            return LaunchProductResult.Succeeded(productState.ReviewScore, productState.ActiveUsers);
        }
    }
}
