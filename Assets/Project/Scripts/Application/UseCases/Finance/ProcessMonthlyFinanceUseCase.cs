using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Finance;
using Project.Core.Definitions.Product;
using Project.Core.Definitions.Time;
using Project.Core.Events.Finance;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Finance
{
    /// <summary>
    /// Application use case coordinating the full monthly finance cycle.
    /// Reads from employee, product, contract, and finance state.
    /// Mutates FinanceRuntimeState, ProductRuntimeState (revenue fields and counter resets),
    /// and ContractRuntimeState.PaymentApplied.
    /// Does NOT mutate CompanyRuntimeState.Reputation (deferred to a future plan).
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class ProcessMonthlyFinanceUseCase
    {
        private readonly IFinanceService _financeService;
        private readonly IFinanceTuning  _tuning;
        private readonly IEventBus       _eventBus;
        private readonly GameSessionState _sessionState;

        /// <summary>
        /// Creates a new ProcessMonthlyFinanceUseCase.
        /// </summary>
        public ProcessMonthlyFinanceUseCase(
            IFinanceService  financeService,
            IFinanceTuning   tuning,
            IEventBus        eventBus,
            GameSessionState sessionState)
        {
            _financeService = financeService;
            _tuning         = tuning;
            _eventBus       = eventBus;
            _sessionState   = sessionState;
        }

        /// <summary>
        /// Executes the full monthly finance processing cycle.
        /// Should only be called on a month-boundary tick (enforced by FinanceTickProcessor).
        /// </summary>
        public TickResult Execute(TickContext context)
        {
            var financeState = _sessionState.FinanceState;
            var companyId    = financeState.CompanyId;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Starting monthly finance cycle. Date: {context.CurrentDate}");

            // ── 1. Snapshot cash at start ──────────────────────────────────────────
            long cashAtStart = financeState.CashMinorUnits;

            // ── Track revenue and expenses per category ────────────────────────────
            var revenueBySource    = new Dictionary<RevenueSource, long>();
            var expensesByCategory = new Dictionary<ExpenseCategory, long>();
            var transactions       = new List<Core.Runtime.Finance.TransactionRecord>();

            long totalSoftwareRevenue    = 0L;
            long totalHardwareNetRevenue = 0L;
            long totalContractRevenue    = 0L;
            long totalCOGS               = 0L;
            long totalWarranty           = 0L;

            // ── 2. Compute and apply payroll ───────────────────────────────────────
            var activeEmployees = _sessionState.EmployeeStates?
                .Where(e => e.Status == EmploymentStatus.Active)
                .ToList()
                ?? new List<Core.Runtime.Employee.EmployeeRuntimeState>();

            long previousPayroll = financeState.MonthlyPayrollMinorUnits;
            long payrollAmount   = _financeService.ComputePayroll(activeEmployees);

            var payrollTx = _financeService.CreateExpenseTransaction(
                ExpenseCategory.Payroll,
                payrollAmount,
                "Monthly payroll disbursement",
                context.CurrentDate,
                null,
                null);

            transactions.Add(payrollTx);
            AccumulateExpense(expensesByCategory, ExpenseCategory.Payroll, payrollAmount);
            financeState.CashMinorUnits           -= payrollAmount;
            financeState.MonthlyPayrollMinorUnits  = payrollAmount;
            financeState.LastPayrollDate           = context.CurrentDate;

            _eventBus.Publish(new PayrollProcessedEvent(companyId, payrollAmount, context.CurrentDate));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Payroll processed. Amount: {payrollAmount} minor units. Active employees: {activeEmployees.Count}");

            // ── 3. Compute software revenue (per launched software product) ─────────
            foreach (var productState in _sessionState.ProductStates ?? new List<Core.Runtime.Product.ProductRuntimeState>())
            {
                var profile = _sessionState.ProductProfiles?
                    .FirstOrDefault(p => p.Id == productState.ProductId);

                if (profile == null || profile.Family != ProductFamily.Software)
                {
                    continue;
                }

                if (productState.Status != ProductStatus.Launched
                    && productState.Status != ProductStatus.Supported)
                {
                    continue;
                }

                long revenue = _financeService.ComputeSoftwareProductRevenue(profile, productState);

                productState.CurrentMonthRevenueMinorUnits = revenue;
                productState.TotalRevenueMinorUnits       += revenue;

                if (revenue > 0L)
                {
                    var revenueSource = profile.RevenueModel == RevenueModel.Subscription
                        ? RevenueSource.SoftwareSubscriptions
                        : RevenueSource.SoftwareUnitSales;

                    var revenueTx = _financeService.CreateIncomeTransaction(
                        revenueSource,
                        revenue,
                        $"Software revenue: {profile.Name}",
                        context.CurrentDate,
                        productState.ProductId,
                        "Product");

                    transactions.Add(revenueTx);
                    AccumulateRevenue(revenueBySource, revenueSource, revenue);
                    financeState.CashMinorUnits += revenue;
                    totalSoftwareRevenue        += revenue;
                }
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Software revenue computed. Total: {totalSoftwareRevenue} minor units.");

            // ── 4. Compute hardware revenue and costs (per launched hardware product) ─
            foreach (var productState in _sessionState.ProductStates ?? new List<Core.Runtime.Product.ProductRuntimeState>())
            {
                var profile = _sessionState.ProductProfiles?
                    .FirstOrDefault(p => p.Id == productState.ProductId);

                if (profile == null || profile.Family != ProductFamily.Hardware)
                {
                    continue;
                }

                if (productState.Status != ProductStatus.Launched
                    && productState.Status != ProductStatus.Supported)
                {
                    continue;
                }

                var hardwareMetrics = _sessionState.HardwareMetrics?
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                if (hardwareMetrics == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ProcessMonthlyFinanceUseCase] No HardwareRuntimeMetrics found for ProductId: {productState.ProductId}. Skipping hardware revenue.");
                    continue;
                }

                long netRevenue = _financeService.ComputeHardwareNetRevenue(profile, productState, _tuning);
                long cogs       = _financeService.ComputeHardwareCOGS(productState, hardwareMetrics);
                long warranty   = hardwareMetrics.WarrantyCostThisMonthMinorUnits;

                productState.CurrentMonthRevenueMinorUnits = netRevenue;
                productState.TotalRevenueMinorUnits       += netRevenue;

                if (netRevenue > 0L)
                {
                    var hwRevenueTx = _financeService.CreateIncomeTransaction(
                        RevenueSource.HardwareUnitSales,
                        netRevenue,
                        $"Hardware net revenue (after retailer margin): {profile.Name}",
                        context.CurrentDate,
                        productState.ProductId,
                        "Product");

                    transactions.Add(hwRevenueTx);
                    AccumulateRevenue(revenueBySource, RevenueSource.HardwareUnitSales, netRevenue);
                    financeState.CashMinorUnits += netRevenue;
                    totalHardwareNetRevenue     += netRevenue;
                }

                if (cogs > 0L)
                {
                    var cogsTx = _financeService.CreateExpenseTransaction(
                        ExpenseCategory.Manufacturing,
                        cogs,
                        $"Hardware COGS (cost of goods sold): {profile.Name}",
                        context.CurrentDate,
                        productState.ProductId,
                        "Product");

                    transactions.Add(cogsTx);
                    AccumulateExpense(expensesByCategory, ExpenseCategory.Manufacturing, cogs);
                    financeState.CashMinorUnits -= cogs;
                    totalCOGS                  += cogs;
                }

                if (warranty > 0L)
                {
                    var warrantyTx = _financeService.CreateExpenseTransaction(
                        ExpenseCategory.Support,
                        warranty,
                        $"Hardware warranty costs: {profile.Name}",
                        context.CurrentDate,
                        productState.ProductId,
                        "Product");

                    transactions.Add(warrantyTx);
                    AccumulateExpense(expensesByCategory, ExpenseCategory.Support, warranty);
                    financeState.CashMinorUnits -= warranty;
                    totalWarranty              += warranty;
                }
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Hardware revenue computed. NetRevenue: {totalHardwareNetRevenue}, COGS: {totalCOGS}, Warranty: {totalWarranty} minor units.");

            // ── 5. Apply contract payments ─────────────────────────────────────────
            foreach (var contractState in _sessionState.ContractStates ?? new List<Core.Runtime.Contract.ContractRuntimeState>())
            {
                if (contractState.Outcome == ContractOutcome.None)
                {
                    continue;
                }

                if (contractState.PaymentDueMinorUnits <= 0L)
                {
                    continue;
                }

                if (contractState.PaymentApplied)
                {
                    continue;
                }

                var contractPaymentTx = _financeService.CreateIncomeTransaction(
                    RevenueSource.ContractPayment,
                    contractState.PaymentDueMinorUnits,
                    $"Contract payment applied: {contractState.ContractId}",
                    context.CurrentDate,
                    contractState.ContractId,
                    "Contract");

                transactions.Add(contractPaymentTx);
                AccumulateRevenue(revenueBySource, RevenueSource.ContractPayment, contractState.PaymentDueMinorUnits);
                financeState.CashMinorUnits += contractState.PaymentDueMinorUnits;
                totalContractRevenue        += contractState.PaymentDueMinorUnits;
                contractState.PaymentApplied = true;

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[ProcessMonthlyFinanceUseCase] Contract payment applied. ContractId: {contractState.ContractId}, Amount: {contractState.PaymentDueMinorUnits} minor units.");
            }

            // ── 6. Compute recurring expenses ──────────────────────────────────────

            // Infrastructure
            int launchedProductCount = (_sessionState.ProductStates ?? new List<Core.Runtime.Product.ProductRuntimeState>())
                .Count(s => s.Status == ProductStatus.Launched || s.Status == ProductStatus.Supported);

            long infraExpense = _financeService.ComputeInfrastructureExpense(launchedProductCount, _tuning);

            if (infraExpense > 0L)
            {
                var infraTx = _financeService.CreateExpenseTransaction(
                    ExpenseCategory.Infrastructure,
                    infraExpense,
                    "[Placeholder] Monthly infrastructure overhead",
                    context.CurrentDate,
                    null,
                    null);

                transactions.Add(infraTx);
                AccumulateExpense(expensesByCategory, ExpenseCategory.Infrastructure, infraExpense);
                financeState.CashMinorUnits -= infraExpense;
            }

            // Marketing
            long marketingExpense = _financeService.ComputeMarketingExpense(
                _sessionState.ProductBudgets,
                _sessionState.ProductStates,
                _sessionState.ProductProfiles);

            if (marketingExpense > 0L)
            {
                var marketingTx = _financeService.CreateExpenseTransaction(
                    ExpenseCategory.Marketing,
                    marketingExpense,
                    "Monthly marketing spend across all active products",
                    context.CurrentDate,
                    null,
                    null);

                transactions.Add(marketingTx);
                AccumulateExpense(expensesByCategory, ExpenseCategory.Marketing, marketingExpense);
                financeState.CashMinorUnits -= marketingExpense;
            }

            // Support
            long supportExpense = _financeService.ComputeSupportExpense(
                _sessionState.ProductBudgets,
                _sessionState.ProductProfiles,
                _sessionState.ProductStates);

            if (supportExpense > 0L)
            {
                var supportTx = _financeService.CreateExpenseTransaction(
                    ExpenseCategory.Support,
                    supportExpense,
                    "Monthly post-launch support spend",
                    context.CurrentDate,
                    null,
                    null);

                transactions.Add(supportTx);
                AccumulateExpense(expensesByCategory, ExpenseCategory.Support, supportExpense);
                financeState.CashMinorUnits -= supportExpense;
            }

            // Manufacturing overhead (active hardware products)
            int activeHwCount = (_sessionState.ProductStates ?? new List<Core.Runtime.Product.ProductRuntimeState>())
                .Count(s =>
                {
                    var p = _sessionState.ProductProfiles?.FirstOrDefault(x => x.Id == s.ProductId);
                    return p != null && p.Family == ProductFamily.Hardware
                        && (s.Status == ProductStatus.Launched || s.Status == ProductStatus.Supported);
                });

            long manufacturingExpense = _financeService.ComputeManufacturingExpense(activeHwCount, _tuning);

            if (manufacturingExpense > 0L)
            {
                var mfgTx = _financeService.CreateExpenseTransaction(
                    ExpenseCategory.Manufacturing,
                    manufacturingExpense,
                    "[Placeholder] Monthly hardware manufacturing overhead",
                    context.CurrentDate,
                    null,
                    null);

                transactions.Add(mfgTx);
                AccumulateExpense(expensesByCategory, ExpenseCategory.Manufacturing, manufacturingExpense);
                financeState.CashMinorUnits -= manufacturingExpense;
            }

            // Research overhead — [Placeholder] uses active research project count
            // Plan 2K will refine this to distinguish active vs completed projects.
            int activeResearchCount = _sessionState.ResearchState?.ActiveProjectIds?.Count ?? 0;
            long researchExpense    = _financeService.ComputeResearchExpense(activeResearchCount, _tuning);

            if (researchExpense > 0L)
            {
                var researchTx = _financeService.CreateExpenseTransaction(
                    ExpenseCategory.Research,
                    researchExpense,
                    "[Placeholder] Monthly research overhead",
                    context.CurrentDate,
                    null,
                    null);

                transactions.Add(researchTx);
                AccumulateExpense(expensesByCategory, ExpenseCategory.Research, researchExpense);
                financeState.CashMinorUnits -= researchExpense;
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Recurring expenses computed. Infra: {infraExpense}, Marketing: {marketingExpense}, Support: {supportExpense}, Mfg: {manufacturingExpense}, Research: {researchExpense} minor units.");

            // ── 7. Update FinanceRuntimeState projection fields ────────────────────
            long totalRevenue  = totalSoftwareRevenue + totalHardwareNetRevenue + totalContractRevenue;
            long totalExpenses = SumDictionary(expensesByCategory);

            financeState.MonthlyProductRevenueMinorUnits  = totalSoftwareRevenue + totalHardwareNetRevenue;
            financeState.MonthlyContractRevenueMinorUnits = totalContractRevenue;
            financeState.MonthlyInfrastructureCostMinorUnits = infraExpense;
            financeState.MonthlyMarketingSpendMinorUnits     = marketingExpense;
            financeState.MonthlySupportCostMinorUnits        = supportExpense + totalWarranty;
            financeState.MonthlyResearchSpendMinorUnits      = researchExpense;
            financeState.MonthlyManufacturingCostMinorUnits  = manufacturingExpense + totalCOGS;
            financeState.MonthlyNetProfitLossMinorUnits      = totalRevenue - totalExpenses;

            // ── 8. Compute runway ──────────────────────────────────────────────────
            var (runwayMonths, isStable) = _financeService.ComputeRunway(
                financeState.CashMinorUnits,
                totalExpenses,
                totalRevenue);

            financeState.RunwayMonths    = runwayMonths;
            financeState.IsRunwayStable  = isStable;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Runway computed. Months: {runwayMonths}, Stable: {isStable}. Cash: {financeState.CashMinorUnits} minor units.");

            // ── 9. Create MonthlyFinanceSummary ────────────────────────────────────
            // context.CurrentDate is the first moment of the new month — we close the previous month.
            var closingPeriod = context.CurrentDate.Month == 1
                ? new FinancePeriodKey(context.CurrentDate.Year - 1, 12)
                : new FinancePeriodKey(context.CurrentDate.Year, context.CurrentDate.Month - 1);

            long cashAtEnd      = financeState.CashMinorUnits;
            long payrollChange  = payrollAmount - previousPayroll;

            var summary = _financeService.CreateMonthlySummary(
                closingPeriod,
                cashAtStart,
                cashAtEnd,
                revenueBySource,
                expensesByCategory,
                runwayMonths,
                isStable,
                payrollChange);

            _sessionState.MonthlyFinanceSummaries.Add(summary);
            financeState.MonthlySummaryIds.Add(summary.Id);

            foreach (var tx in transactions)
            {
                _sessionState.TransactionRecords.Add(tx);
                financeState.TransactionIds.Add(tx.Id);
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ProcessMonthlyFinanceUseCase] Monthly summary created. Id: {summary.Id}, Period: {closingPeriod}, Net: {summary.NetProfitLossMinorUnits} minor units.");

            // ── 10. Reset consumed monthly activity counters ───────────────────────
            foreach (var productState in _sessionState.ProductStates ?? new List<Core.Runtime.Product.ProductRuntimeState>())
            {
                productState.UnitsSoldThisMonth = 0;

                // Reset software-specific monthly counters.
                var softwareMetrics = _sessionState.SoftwareMetrics?
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                if (softwareMetrics != null)
                {
                    softwareMetrics.NewUsersThisMonth = 0;
                }

                // Reset hardware-specific monthly counters.
                var hardwareMetrics = _sessionState.HardwareMetrics?
                    .FirstOrDefault(m => m.ProductId == productState.ProductId);

                if (hardwareMetrics != null)
                {
                    hardwareMetrics.WarrantyCostThisMonthMinorUnits = 0L;
                }
            }

            // ── 11. Publish events and check runway thresholds ─────────────────────
            _eventBus.Publish(new MonthlyFinanceProcessedEvent(
                companyId,
                totalRevenue,
                totalExpenses,
                totalRevenue - totalExpenses,
                runwayMonths));

            var interruptions = new List<InterruptionRequest>();

            if (!isStable && runwayMonths <= _tuning.CriticalRunwayThresholdMonths)
            {
                interruptions.Add(new InterruptionRequest(
                    InterruptionType.CriticalRunway,
                    companyId,
                    $"Critical runway warning: {runwayMonths} month(s) remaining."));

                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ProcessMonthlyFinanceUseCase] Critical runway threshold reached. Months: {runwayMonths}. Interrupting Continue.");
            }
            else if (!isStable && runwayMonths <= _tuning.LowRunwayThresholdMonths)
            {
                _eventBus.Publish(new LowRunwayWarningEvent(companyId, runwayMonths));

                DebugLogger.LogWarning(DebugCategory.Simulation,
                    $"[ProcessMonthlyFinanceUseCase] Low runway warning published. Months: {runwayMonths}.");
            }

            return interruptions.Count > 0
                ? TickResult.Succeeded(interruptions)
                : TickResult.Succeeded();
        }

        // ─── Private Helpers ──────────────────────────────────────────────────────

        private static void AccumulateRevenue(
            Dictionary<RevenueSource, long> dict,
            RevenueSource source,
            long amount)
        {
            if (dict.ContainsKey(source))
            {
                dict[source] += amount;
            }
            else
            {
                dict[source] = amount;
            }
        }

        private static void AccumulateExpense(
            Dictionary<ExpenseCategory, long> dict,
            ExpenseCategory category,
            long amount)
        {
            if (dict.ContainsKey(category))
            {
                dict[category] += amount;
            }
            else
            {
                dict[category] = amount;
            }
        }

        private static long SumDictionary<TKey>(Dictionary<TKey, long> dict)
        {
            long total = 0L;

            foreach (var kvp in dict)
            {
                total += kvp.Value;
            }

            return total;
        }
    }
}
