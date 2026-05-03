using System.Collections.Generic;
using Project.Core.Definitions.Finance;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Finance domain service interface.
    /// Stateless — all methods receive state as parameters and return computed values or new record instances.
    /// Does not publish events. Does not mutate any state.
    /// Implemented by FinanceService in Core/Services/Finance.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public interface IFinanceService
    {
        // ── Payroll ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Sums SalaryMinorUnits for all employees in the list.
        /// Returns 0 if the list is empty.
        /// See formula.finance.payroll.
        /// </summary>
        /// <param name="activeEmployees">Active employees to include in the payroll total.</param>
        long ComputePayroll(List<EmployeeRuntimeState> activeEmployees);

        // ── Software Revenue ──────────────────────────────────────────────────────

        /// <summary>
        /// Computes monthly software product revenue.
        /// OneTimePurchase: UnitsSoldThisMonth * PriceMinorUnits.
        /// Subscription: ActiveUsers * PriceMinorUnits.
        /// Freemium: always returns 0 (non-functional in Plan 2H).
        /// Returns 0 for products not in Launched or Supported status.
        /// See formula.finance.software_revenue.
        /// </summary>
        /// <param name="profile">Static product profile containing pricing and revenue model.</param>
        /// <param name="productState">Live runtime state containing monthly counters.</param>
        long ComputeSoftwareProductRevenue(ProductProfile profile, ProductRuntimeState productState);

        // ── Hardware Revenue ──────────────────────────────────────────────────────

        /// <summary>
        /// Computes hardware net revenue after retailer margin deduction.
        /// grossRevenue = UnitsSoldThisMonth * PriceMinorUnits.
        /// retailerMargin = (long)(grossRevenue * HardwareRetailerMarginPercent / 100.0).
        /// Returns grossRevenue - retailerMargin.
        /// Returns 0 for products not in Launched or Supported status.
        /// See formula.finance.hardware_revenue.
        /// </summary>
        long ComputeHardwareNetRevenue(ProductProfile profile, ProductRuntimeState productState, IFinanceTuning tuning);

        /// <summary>
        /// Computes hardware cost of goods sold for units sold this month.
        /// COGS = UnitsSoldThisMonth * ManufacturingCostPerUnitMinorUnits.
        /// </summary>
        long ComputeHardwareCOGS(ProductRuntimeState productState, HardwareRuntimeMetrics metrics);

        // ── Recurring Expenses ────────────────────────────────────────────────────

        /// <summary>
        /// [Placeholder] Computes monthly infrastructure expense.
        /// infraExpense = launchedProductCount * tuning.BaseInfrastructureCostPerProductMinorUnits.
        /// </summary>
        long ComputeInfrastructureExpense(int launchedProductCount, IFinanceTuning tuning);

        /// <summary>
        /// Computes total monthly marketing spend across all applicable products.
        /// For launched products: PostLaunchMarketingMonthlyBudgetMinorUnits.
        /// For pre-launch products: PreLaunchMarketingMonthlyBudgetMinorUnits.
        /// Products without a matching budget profile contribute 0.
        /// </summary>
        long ComputeMarketingExpense(List<ProductBudgetProfile> budgets, List<ProductRuntimeState> productStates, List<ProductProfile> profiles);

        /// <summary>
        /// Computes total monthly support spend across all launched products that require support.
        /// Uses PostLaunchSupportMonthlyBudgetMinorUnits for products where RequiresSupport is true.
        /// </summary>
        long ComputeSupportExpense(List<ProductBudgetProfile> budgets, List<ProductProfile> profiles, List<ProductRuntimeState> productStates);

        /// <summary>
        /// [Placeholder] Computes monthly manufacturing overhead for active hardware products.
        /// manufacturingExpense = activeHardwareProductCount * tuning.BaseManufacturingMonthlyCostMinorUnits.
        /// This is overhead cost — separate from COGS which is computed by ComputeHardwareCOGS.
        /// </summary>
        long ComputeManufacturingExpense(int activeHardwareProductCount, IFinanceTuning tuning);

        /// <summary>
        /// [Placeholder] Computes monthly research overhead for active research projects.
        /// researchExpense = activeResearchProjectCount * tuning.BaseResearchMonthlyCostMinorUnits.
        /// </summary>
        long ComputeResearchExpense(int activeResearchProjectCount, IFinanceTuning tuning);

        // ── Runway ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes months of cash runway at current burn rate.
        /// monthlyNetBurn = totalMonthlyExpenses - totalMonthlyRevenue.
        /// If monthlyNetBurn &lt;= 0: returns (int.MaxValue, true) — company is profitable.
        /// Otherwise: returns ((int)(cashMinorUnits / monthlyNetBurn), false).
        /// See formula.finance.runway_months.
        /// </summary>
        /// <returns>Tuple of (runwayMonths, isRunwayStable).</returns>
        (int runwayMonths, bool isStable) ComputeRunway(long cashMinorUnits, long totalMonthlyExpenses, long totalMonthlyRevenue);

        // ── Transaction Helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Creates a GUID-identified expense TransactionRecord.
        /// Type is set to Expense. RevenueSource is null.
        /// </summary>
        TransactionRecord CreateExpenseTransaction(
            ExpenseCategory category,
            long amountMinorUnits,
            string description,
            GameDateTime date,
            string relatedEntityId,
            string relatedEntityType);

        /// <summary>
        /// Creates a GUID-identified income TransactionRecord.
        /// Type is set to Income. ExpenseCategory is null.
        /// </summary>
        TransactionRecord CreateIncomeTransaction(
            RevenueSource source,
            long amountMinorUnits,
            string description,
            GameDateTime date,
            string relatedEntityId,
            string relatedEntityType);

        // ── Summary Helper ────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a GUID-identified MonthlyFinanceSummary with computed totals and net profit/loss.
        /// TotalRevenueMinorUnits is summed from revenueBySource values.
        /// TotalExpensesMinorUnits is summed from expensesByCategory values.
        /// NetProfitLossMinorUnits = totalRevenue - totalExpenses.
        /// </summary>
        MonthlyFinanceSummary CreateMonthlySummary(
            FinancePeriodKey period,
            long cashAtStart,
            long cashAtEnd,
            Dictionary<RevenueSource, long> revenueBySource,
            Dictionary<ExpenseCategory, long> expensesByCategory,
            int runwayMonths,
            bool isRunwayStable,
            long payrollChange);
    }
}
