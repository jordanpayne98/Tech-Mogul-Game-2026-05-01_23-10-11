using System;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Events.Contract;
using Project.Core.Events.Employee;
using Project.Core.Events.Event;
using Project.Core.Events.Market;
using Project.Core.Events.Product;
using Project.Core.Events.Research;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Runtime;

namespace Project.Application.UseCases.Report
{
    /// <summary>
    /// Application-layer event subscriber that generates milestone-triggered reports
    /// in response to domain events published via IEventBus.
    ///
    /// Subscribes to 7 domain events and delegates report generation to IReportService.
    /// Executes synchronously within the event publish cycle, so milestone reports are
    /// created in the same tick as the triggering event, before ReportTickProcessor
    /// runs at Order 1100.
    ///
    /// Must be initialized before the first tick and disposed after the last tick.
    /// Defined in Plan 2L, GDD_14. CrisisTriggeredEvent subscription added in Plan 2M.
    /// </summary>
    public sealed class ReportEventHandler : IDisposable
    {
        private readonly IEventBus        _eventBus;
        private readonly IReportService   _reportService;
        private readonly GameSessionState _sessionState;

        private bool _initialized;
        private bool _disposed;

        // Cached handler references for safe unsubscription
        private readonly Action<CandidateOfferRespondedEvent>    _offerRespondedHandler;
        private readonly Action<ContractCompletedEvent>           _contractCompletedHandler;
        private readonly Action<ProductPhaseCompletedEvent>       _productPhaseCompletedHandler;
        private readonly Action<ProductLaunchedEvent>             _productLaunchedHandler;
        private readonly Action<CompetitorProductLaunchedEvent>   _competitorProductLaunchedHandler;
        private readonly Action<ResearchCompletedEvent>           _researchCompletedHandler;
        private readonly Action<CrisisTriggeredEvent>             _crisisTriggeredHandler;

        public ReportEventHandler(
            IEventBus        eventBus,
            IReportService   reportService,
            GameSessionState sessionState)
        {
            _eventBus      = eventBus      ?? throw new ArgumentNullException(nameof(eventBus));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _sessionState  = sessionState  ?? throw new ArgumentNullException(nameof(sessionState));

            _offerRespondedHandler            = OnCandidateOfferResponded;
            _contractCompletedHandler         = OnContractCompleted;
            _productPhaseCompletedHandler     = OnProductPhaseCompleted;
            _productLaunchedHandler           = OnProductLaunched;
            _competitorProductLaunchedHandler = OnCompetitorProductLaunched;
            _researchCompletedHandler         = OnResearchCompleted;
            _crisisTriggeredHandler           = OnCrisisTriggered;
        }

        /// <summary>
        /// Subscribes to all 7 domain events. Must be called before the first simulation tick.
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;
            if (_disposed)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] Cannot initialize — handler has already been disposed.");
                return;
            }

            _eventBus.Subscribe(_offerRespondedHandler);
            _eventBus.Subscribe(_contractCompletedHandler);
            _eventBus.Subscribe(_productPhaseCompletedHandler);
            _eventBus.Subscribe(_productLaunchedHandler);
            _eventBus.Subscribe(_competitorProductLaunchedHandler);
            _eventBus.Subscribe(_researchCompletedHandler);
            _eventBus.Subscribe(_crisisTriggeredHandler);

            _initialized = true;

            DebugLogger.Log(DebugCategory.Simulation,
                "[ReportEventHandler] Initialized — subscribed to 7 domain events.");
        }

        /// <summary>
        /// Unsubscribes from all domain events. Call during session teardown.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _eventBus.Unsubscribe(_offerRespondedHandler);
            _eventBus.Unsubscribe(_contractCompletedHandler);
            _eventBus.Unsubscribe(_productPhaseCompletedHandler);
            _eventBus.Unsubscribe(_productLaunchedHandler);
            _eventBus.Unsubscribe(_competitorProductLaunchedHandler);
            _eventBus.Unsubscribe(_researchCompletedHandler);
            _eventBus.Unsubscribe(_crisisTriggeredHandler);

            _disposed = true;

            DebugLogger.Log(DebugCategory.Simulation,
                "[ReportEventHandler] Disposed — unsubscribed from all domain events.");
        }

        // ── Event handlers ─────────────────────────────────────────────────────────

        private void OnCandidateOfferResponded(CandidateOfferRespondedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping OfferResponseReport.");
                return;
            }

            var candidate = _sessionState.CandidateProfiles
                .FirstOrDefault(c => c.Id == evt.CandidateId);

            if (candidate == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] CandidateProfile not found for CandidateId: {evt.CandidateId}. Skipping OfferResponseReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping OfferResponseReport.");
                return;
            }

            var result = _reportService.GenerateOfferResponseReport(
                date, candidate, evt.Response, _sessionState.InboxState);

            LogResult("OfferResponseReport", result.Success, result.FailureReason);
        }

        private void OnContractCompleted(ContractCompletedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping ContractCompletionReport.");
                return;
            }

            var contract = _sessionState.ContractStates
                .FirstOrDefault(c => c.ContractId == evt.ContractId);

            if (contract == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] ContractRuntimeState not found for ContractId: {evt.ContractId}. Skipping ContractCompletionReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping ContractCompletionReport.");
                return;
            }

            var result = _reportService.GenerateContractCompletionReport(
                date, contract, evt.Outcome, _sessionState.InboxState);

            LogResult("ContractCompletionReport", result.Success, result.FailureReason);
        }

        private void OnProductPhaseCompleted(ProductPhaseCompletedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping ProductProgressReport.");
                return;
            }

            var product = _sessionState.ProductStates
                .FirstOrDefault(p => p.ProductId == evt.ProductId);

            if (product == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] ProductRuntimeState not found for ProductId: {evt.ProductId}. Skipping ProductProgressReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping ProductProgressReport.");
                return;
            }

            var result = _reportService.GenerateProductProgressReport(
                date, product, evt.CompletedPhase, evt.NextPhase, _sessionState.InboxState);

            LogResult("ProductProgressReport", result.Success, result.FailureReason);
        }

        private void OnProductLaunched(ProductLaunchedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping ProductLaunchReport.");
                return;
            }

            var product = _sessionState.ProductStates
                .FirstOrDefault(p => p.ProductId == evt.ProductId);

            if (product == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] ProductRuntimeState not found for ProductId: {evt.ProductId}. Skipping ProductLaunchReport.");
                return;
            }

            var result = _reportService.GenerateProductLaunchReport(
                evt.LaunchDate, product, _sessionState.InboxState);

            LogResult("ProductLaunchReport", result.Success, result.FailureReason);
        }

        private void OnCompetitorProductLaunched(CompetitorProductLaunchedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping CompetitorLaunchReport.");
                return;
            }

            var competitor = _sessionState.CompetitorStates
                .FirstOrDefault(c => c.CompetitorId == evt.CompetitorId);

            if (competitor == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] CompetitorRuntimeState not found for CompetitorId: {evt.CompetitorId}. Skipping CompetitorLaunchReport.");
                return;
            }

            var competitorProduct = _sessionState.CompetitorProductStates
                .FirstOrDefault(p => p.Id == evt.CompetitorProductId);

            if (competitorProduct == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] CompetitorProductRuntimeState not found for CompetitorProductId: {evt.CompetitorProductId}. Skipping CompetitorLaunchReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping CompetitorLaunchReport.");
                return;
            }

            var result = _reportService.GenerateCompetitorLaunchReport(
                date, competitor, competitorProduct, _sessionState.InboxState);

            LogResult("CompetitorLaunchReport", result.Success, result.FailureReason);
        }

        private void OnResearchCompleted(ResearchCompletedEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping ResearchCompletionReport.");
                return;
            }

            var project = _sessionState.ResearchProjectStates
                .FirstOrDefault(p => p.ProjectId == evt.ProjectId);

            if (project == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] ResearchProjectRuntimeState not found for ProjectId: {evt.ProjectId}. Skipping ResearchCompletionReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping ResearchCompletionReport.");
                return;
            }

            var result = _reportService.GenerateResearchCompletionReport(
                date, project, evt.UnlockedCapabilityIds, _sessionState.InboxState);

            LogResult("ResearchCompletionReport", result.Success, result.FailureReason);
        }

        private void OnCrisisTriggered(CrisisTriggeredEvent evt)
        {
            if (_sessionState.InboxState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] InboxState is null. Skipping CrisisReport.");
                return;
            }

            var date = _sessionState.TimeState?.CurrentDate;

            if (date == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ReportEventHandler] TimeState.CurrentDate is null. Skipping CrisisReport.");
                return;
            }

            var result = _reportService.GenerateCrisisReport(
                date,
                evt.EventDefinitionId,
                evt.Description,
                evt.Category,
                evt.Severity,
                evt.AppliedEffects,
                _sessionState.InboxState);

            LogResult("CrisisReport", result.Success, result.FailureReason);
        }

        // ── Private helpers ────────────────────────────────────────────────────────

        private static void LogResult(string reportName, bool success, string failureReason)
        {
            if (success)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    $"[ReportEventHandler] {reportName} generated successfully.");
            }
            else
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ReportEventHandler] {reportName} generation failed: {failureReason}");
            }
        }
    }
}
