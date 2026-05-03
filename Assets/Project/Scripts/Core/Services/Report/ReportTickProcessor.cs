using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Report
{
    /// <summary>
    /// Tick processor that generates periodic boundary-triggered reports.
    /// Runs at ProcessingOrder 1100, after all domain processors have finalized their data.
    ///
    /// Per monthly boundary:
    ///   - Generates one MonthlyFinanceReport (if a finance summary exists for the month).
    ///   - Generates one MonthlyProductPerformanceReport per launched/supported product.
    ///   - Generates one MarketTrendReport covering all active market categories.
    ///
    /// Report generation never produces interruptions.
    /// Defined in Plan 2L, GDD_14.
    /// </summary>
    public sealed class ReportTickProcessor : ITickProcessor
    {
        private readonly IReportService  _reportService;
        private readonly GameSessionState _sessionState;

        public string ProcessorName   => "ReportTickProcessor";
        public int    ProcessingOrder => 1100;

        public ReportTickProcessor(IReportService reportService, GameSessionState sessionState)
        {
            _reportService = reportService;
            _sessionState  = sessionState;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            if (!context.IsMonthBoundary)
            {
                return TickResult.Succeeded();
            }

            GenerateMonthlyFinanceReport(context);
            GenerateMonthlyProductPerformanceReports(context);
            GenerateMarketTrendReport(context);

            return TickResult.Succeeded();
        }

        // ── Monthly finance report ─────────────────────────────────────────────────

        private void GenerateMonthlyFinanceReport(TickContext context)
        {
            if (_sessionState.FinanceState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportTickProcessor] FinanceState is null. Skipping MonthlyFinanceReport.");
                return;
            }

            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportTickProcessor] InboxState is null. Skipping MonthlyFinanceReport.");
                return;
            }

            // The latest summary is the last entry — ProcessMonthlyFinanceUseCase runs at Order 700.
            var summary = _sessionState.MonthlyFinanceSummaries?.LastOrDefault();

            if (summary == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportTickProcessor] No MonthlyFinanceSummary found. Skipping MonthlyFinanceReport.");
                return;
            }

            var result = _reportService.GenerateMonthlyFinanceReport(
                context.CurrentDate,
                summary,
                _sessionState.FinanceState,
                _sessionState.InboxState);

            if (!result.Success)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportTickProcessor] MonthlyFinanceReport generation failed: {result.FailureReason}");
            }
        }

        // ── Monthly product performance reports ───────────────────────────────────

        private void GenerateMonthlyProductPerformanceReports(TickContext context)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportTickProcessor] InboxState is null. Skipping product performance reports.");
                return;
            }

            var launchedStatuses = new HashSet<ProductStatus>
            {
                ProductStatus.Launched,
                ProductStatus.Updating,
                ProductStatus.Supported,
                ProductStatus.Mature,
                ProductStatus.Declining
            };

            var activeProducts = _sessionState.ProductStates?
                .Where(p => launchedStatuses.Contains(p.Status))
                .ToList();

            if (activeProducts == null || activeProducts.Count == 0)
            {
                return;
            }

            foreach (var productState in activeProducts)
            {
                SoftwareRuntimeMetrics softwareMetrics = _sessionState.SoftwareMetrics?
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                HardwareRuntimeMetrics hardwareMetrics = _sessionState.HardwareMetrics?
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                var result = _reportService.GenerateMonthlyProductPerformanceReport(
                    context.CurrentDate,
                    productState,
                    softwareMetrics,
                    hardwareMetrics,
                    _sessionState.InboxState);

                if (!result.Success)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ReportTickProcessor] ProductPerformanceReport failed for ProductId {productState.ProductId}: {result.FailureReason}");
                }
            }
        }

        // ── Market trend report ────────────────────────────────────────────────────

        private void GenerateMarketTrendReport(TickContext context)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportTickProcessor] InboxState is null. Skipping MarketTrendReport.");
                return;
            }

            var marketStates = _sessionState.MarketCategoryStates;

            if (marketStates == null || marketStates.Count == 0)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    "[ReportTickProcessor] No market category states. Skipping MarketTrendReport.");
                return;
            }

            var result = _reportService.GenerateMarketTrendReport(
                context.CurrentDate,
                marketStates,
                _sessionState.InboxState);

            if (!result.Success)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportTickProcessor] MarketTrendReport generation failed: {result.FailureReason}");
            }
        }
    }
}
