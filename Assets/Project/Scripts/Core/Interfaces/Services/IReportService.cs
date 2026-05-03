using System.Collections.Generic;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Event;
using Project.Core.Definitions.Product;
using Project.Core.Results.Report;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Core service interface for report generation and inbox delivery.
    /// All methods are stateless — state is passed in as parameters and mutated directly.
    /// Every method adds the generated report to InboxRuntimeState and returns a GenerateReportResult.
    /// Defined in Plan 2L, GDD_14, GDD_18.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Generates a monthly finance summary report and delivers it to the inbox.
        /// Priority is upgraded based on runway thresholds from IFinanceTuning.
        /// </summary>
        GenerateReportResult GenerateMonthlyFinanceReport(
            GameDateTime        date,
            MonthlyFinanceSummary summary,
            FinanceRuntimeState financeState,
            InboxRuntimeState   inbox);

        /// <summary>
        /// Generates a report for a candidate offer response (accepted or rejected).
        /// </summary>
        GenerateReportResult GenerateOfferResponseReport(
            GameDateTime      date,
            CandidateProfile  candidate,
            OfferStatus       response,
            InboxRuntimeState inbox);

        /// <summary>
        /// Generates a report for a completed contract.
        /// Priority is Important on failure, Routine otherwise.
        /// </summary>
        GenerateReportResult GenerateContractCompletionReport(
            GameDateTime         date,
            ContractRuntimeState contract,
            ContractOutcome      outcome,
            InboxRuntimeState    inbox);

        /// <summary>
        /// Generates a report when a product completes a development phase and moves to the next.
        /// </summary>
        GenerateReportResult GenerateProductProgressReport(
            GameDateTime         date,
            ProductRuntimeState  product,
            ProductStatus        completedPhase,
            ProductStatus        nextPhase,
            InboxRuntimeState    inbox);

        /// <summary>
        /// Generates a report when the player launches a product.
        /// </summary>
        GenerateReportResult GenerateProductLaunchReport(
            GameDateTime        date,
            ProductRuntimeState product,
            InboxRuntimeState   inbox);

        /// <summary>
        /// Generates a monthly product performance report.
        /// Pass null for the metrics type that does not apply to the product family.
        /// </summary>
        GenerateReportResult GenerateMonthlyProductPerformanceReport(
            GameDateTime           date,
            ProductRuntimeState    product,
            SoftwareRuntimeMetrics softwareMetrics,
            HardwareRuntimeMetrics hardwareMetrics,
            InboxRuntimeState      inbox);

        /// <summary>
        /// Generates a market trend summary report covering all active market categories.
        /// </summary>
        GenerateReportResult GenerateMarketTrendReport(
            GameDateTime                   date,
            List<MarketCategoryRuntimeState> marketStates,
            InboxRuntimeState              inbox);

        /// <summary>
        /// Generates a report when an AI competitor launches a new product.
        /// </summary>
        GenerateReportResult GenerateCompetitorLaunchReport(
            GameDateTime                 date,
            CompetitorRuntimeState       competitor,
            CompetitorProductRuntimeState product,
            InboxRuntimeState            inbox);

        /// <summary>
        /// Generates a report when a research project completes.
        /// </summary>
        GenerateReportResult GenerateResearchCompletionReport(
            GameDateTime                date,
            ResearchProjectRuntimeState project,
            List<string>                unlockedCapabilityIds,
            InboxRuntimeState           inbox);

        /// <summary>
        /// Generates a crisis report when a game event fires and its effects are applied.
        /// Called by ReportEventHandler when CrisisTriggeredEvent is received.
        /// Maps GameEventCategory to ReportCategory and GameEventSeverity to ReportPriority.
        /// RequiresDecision is always false (auto-resolve for MVP).
        /// Added in Plan 2M, GDD_15.
        /// </summary>
        GenerateReportResult GenerateCrisisReport(
            GameDateTime         date,
            string               eventDefinitionId,
            string               description,
            GameEventCategory    category,
            GameEventSeverity    severity,
            List<GameEventEffect> effects,
            InboxRuntimeState    inbox);
    }
}
