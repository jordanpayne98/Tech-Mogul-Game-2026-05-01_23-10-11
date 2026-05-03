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
    /// Daily hardware-specific post-launch metrics processor.
    /// Runs at Order 400 each simulation day, after SoftwareMetricsTickProcessor (Order 350).
    /// Processes unit sales, defect accumulation, and warranty costs for all launched Hardware products.
    ///
    /// Also recalculates review score monthly and resets monthly counters.
    ///
    /// Depends on the concrete HardwareMetricsService (not IProductService) because
    /// it calls hardware-specific methods not present on the interface.
    /// </summary>
    public sealed class HardwareMetricsTickProcessor : ITickProcessor
    {
        private readonly HardwareMetricsService _hardwareService;
        private readonly IProductTuning         _tuning;
        private readonly GameSessionState       _sessionState;
        private readonly Random                 _random;

        public string ProcessorName   => "HardwareMetricsTickProcessor";
        public int    ProcessingOrder => 400;

        public HardwareMetricsTickProcessor(
            HardwareMetricsService hardwareService,
            IProductTuning         tuning,
            GameSessionState       sessionState,
            Random                 random)
        {
            _hardwareService = hardwareService;
            _tuning          = tuning;
            _sessionState    = sessionState;
            _random          = random;
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
                // Only process launched or supported hardware products.
                if (productState.Status != ProductStatus.Launched
                    && productState.Status != ProductStatus.Supported)
                {
                    continue;
                }

                // Find the matching profile to determine family.
                var profile = _sessionState.ProductProfiles
                    .FirstOrDefault(p => p.Id == productState.ProductId);

                if (profile == null || profile.Family != ProductFamily.Hardware)
                {
                    continue;
                }

                // Find hardware metrics.
                var metrics = _sessionState.HardwareMetrics
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                if (metrics == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[HardwareMetricsTickProcessor] No HardwareRuntimeMetrics found for ProductId: {productState.ProductId}. Skipping.");
                    continue;
                }

                // Run daily metrics update.
                _hardwareService.UpdateDailyMetrics(productState, metrics, _tuning, _random);

                // On a month boundary, recalculate review score and reset monthly counters.
                if (context.IsMonthBoundary)
                {
                    int reviewScore = _hardwareService.ComputeHardwareReviewScore(
                        metrics, profile.QualityTarget, _tuning);

                    productState.ReviewScore       = reviewScore;
                    productState.RecentReviewScore = reviewScore;

                    // Month-boundary skip: finance closes the month at Order 700 before new-month accumulation begins.
                    // ResetMonthlyCounters is NOT called here — FinanceTickProcessor (Order 700) will reset
                    // WarrantyCostThisMonthMinorUnits after snapshotting it for expense calculation.

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[HardwareMetricsTickProcessor] Monthly review recalculated. ProductId: {productState.ProductId}, ReviewScore: {reviewScore}");
                }
            }

            return TickResult.Succeeded();
        }
    }
}
