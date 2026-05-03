using System;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Product
{
    /// <summary>
    /// Daily software-specific post-launch metrics processor.
    /// Runs at Order 350 each simulation day, after ProductPhaseTickProcessor (Order 300).
    /// Processes active user growth, churn, bug accumulation, support tickets,
    /// infrastructure load, and uptime for all launched Software products.
    ///
    /// Also recalculates review score monthly to reflect evolving metrics.
    ///
    /// Depends on the concrete ProductService (not IProductService) because
    /// it calls software-specific methods not present on the interface.
    /// </summary>
    public sealed class SoftwareMetricsTickProcessor : ITickProcessor
    {
        private readonly ProductService  _productService;
        private readonly IProductTuning  _tuning;
        private readonly GameSessionState _sessionState;
        private readonly Random          _random;

        public string ProcessorName   => "SoftwareMetricsTickProcessor";
        public int    ProcessingOrder => 350;

        public SoftwareMetricsTickProcessor(
            ProductService    productService,
            IProductTuning    tuning,
            GameSessionState  sessionState,
            Random            random)
        {
            _productService = productService;
            _tuning         = tuning;
            _sessionState   = sessionState;
            _random         = random;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            foreach (var productState in _sessionState.ProductStates)
            {
                // Only process launched or supported software products.
                if (productState.Status != ProductStatus.Launched
                    && productState.Status != ProductStatus.Supported)
                {
                    continue;
                }

                // Find the matching profile to determine family.
                var profile = _sessionState.ProductProfiles
                    .FirstOrDefault(p => p.Id == productState.ProductId);

                if (profile == null || profile.Family != ProductFamily.Software)
                {
                    continue;
                }

                // Find software metrics.
                var metrics = _sessionState.SoftwareMetrics
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                if (metrics == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[SoftwareMetricsTickProcessor] No SoftwareRuntimeMetrics found for ProductId: {productState.ProductId}. Skipping.");
                    continue;
                }

                // Find budget profile.
                var budget = _sessionState.ProductBudgets
                    .FirstOrDefault(b => b.ProductId == productState.ProductId);

                if (budget == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[SoftwareMetricsTickProcessor] No ProductBudgetProfile found for ProductId: {productState.ProductId}. Skipping.");
                    continue;
                }

                // Run daily metrics update.
                _productService.UpdateDailyMetrics(productState, metrics, budget, _tuning, _random);

                // On a month boundary, recalculate review score and reset monthly counters.
                if (context.IsMonthBoundary)
                {
                    int reviewScore = _productService.ComputeReviewScore(
                        productState, metrics, profile.QualityTarget, _tuning);

                    productState.ReviewScore       = reviewScore;
                    productState.RecentReviewScore = reviewScore;

                    // Month-boundary skip: finance closes the month at Order 700 before new-month accumulation begins.
                    // ResetMonthlyCounters is NOT called here — FinanceTickProcessor (Order 700) will reset
                    // UnitsSoldThisMonth and NewUsersThisMonth after snapshotting them for revenue calculation.

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[SoftwareMetricsTickProcessor] Monthly review recalculated. ProductId: {productState.ProductId}, ReviewScore: {reviewScore}");
                }
            }

            return TickResult.Succeeded();
        }
    }
}
