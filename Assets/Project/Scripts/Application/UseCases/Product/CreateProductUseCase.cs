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
    /// Application-layer use case for creating a new product (Software or Hardware).
    /// Validates the request, creates all required runtime state objects,
    /// adds them to GameSessionState, and publishes a ProductCreatedEvent.
    /// </summary>
    public sealed class CreateProductUseCase
    {
        private readonly IProductService       _productService;
        private readonly ProductService        _concreteService;
        private readonly HardwareMetricsService _hardwareService;
        private readonly IProductTuning        _tuning;
        private readonly IEventBus             _eventBus;

        public CreateProductUseCase(
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
        /// Executes the create product use case.
        /// Returns a failure result if validation fails.
        /// On success, adds the product profile, runtime state, budget, and family-specific metrics
        /// to the session state and publishes a ProductCreatedEvent.
        /// </summary>
        public CreateProductResult Execute(CreateProductRequest request, GameSessionState sessionState)
        {
            // ── Validation ─────────────────────────────────────────────────────────

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return CreateProductResult.Failed("Product name is required");
            }

            if (request.PriceMinorUnits < 0)
            {
                return CreateProductResult.Failed("Price cannot be negative");
            }

            if (request.FeatureScope < 0 || request.FeatureScope > 100)
            {
                return CreateProductResult.Failed("Feature scope must be 0-100");
            }

            if (request.QualityTarget < 0 || request.QualityTarget > 100)
            {
                return CreateProductResult.Failed("Quality target must be 0-100");
            }

            // Freemium is not yet supported — blocks creation until Plan 2H.
            if (request.RevenueModel == RevenueModel.Freemium)
            {
                return CreateProductResult.Failed("Freemium revenue model is not yet supported");
            }

            // ── Product family determination ────────────────────────────────────────

            var family = _productService.DeriveFamily(request.Category);

            // Hardware-specific validation.
            if (family == ProductFamily.Hardware)
            {
                if (request.LaunchStockQuantity <= 0)
                {
                    return CreateProductResult.Failed("Hardware products require a positive launch stock quantity");
                }
            }

            // ── Create product entities ─────────────────────────────────────────────

            var profile      = _productService.CreateProductProfile(request, sessionState.TimeState.CurrentDate);
            var runtimeState = _productService.CreateProductRuntimeState(profile.Id);
            var budget       = _productService.CreateProductBudgetProfile(profile.Id, request);

            // ── Add to session state ────────────────────────────────────────────────

            sessionState.ProductProfiles.Add(profile);
            sessionState.ProductStates.Add(runtimeState);
            sessionState.ProductBudgets.Add(budget);

            // Family-specific metrics creation.
            if (family == ProductFamily.Software)
            {
                var softwareMetrics = _concreteService.CreateSoftwareMetrics(profile.Id);
                sessionState.SoftwareMetrics.Add(softwareMetrics);
            }
            else if (family == ProductFamily.Hardware)
            {
                var hardwareMetrics = _hardwareService.CreateHardwareMetrics(
                    profile.Id,
                    request.BOMTier,
                    request.LaunchStockQuantity,
                    _tuning);
                sessionState.HardwareMetrics.Add(hardwareMetrics);
            }

            // ── Publish event and log ───────────────────────────────────────────────

            _eventBus.Publish(new ProductCreatedEvent(profile.Id, family));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[CreateProductUseCase] Product created. Id: {profile.Id}, Name: {profile.Name}, "
                + $"Family: {family}, Category: {profile.Category}");

            return CreateProductResult.Succeeded(profile.Id);
        }
    }
}
