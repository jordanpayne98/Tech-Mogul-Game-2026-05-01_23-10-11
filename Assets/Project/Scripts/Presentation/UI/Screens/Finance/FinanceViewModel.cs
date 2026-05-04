using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Finance overview screen (screen.finance).
    /// Immutable after construction. No Unity dependencies.
    /// Created by FinanceController and passed to FinanceView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FinanceViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Finance".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there is no finance history.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there is no finance history.</summary>
        public string EmptyStateBody { get; }

        // ── Finance content ──────────────────────────────────────────────────

        /// <summary>KPI cards displayed in the top KPI row (cash, net profit/loss, runway, revenue, payroll, support/infra cost).</summary>
        public IReadOnlyList<FinanceKpiViewModel> KpiCards { get; }

        /// <summary>Revenue breakdown rows (product, contract, hardware unit, subscription).</summary>
        public IReadOnlyList<FinanceBreakdownRowViewModel> RevenueRows { get; }

        /// <summary>Expense breakdown rows (payroll, infrastructure, support, marketing, research, hardware prototyping, manufacturing).</summary>
        public IReadOnlyList<FinanceBreakdownRowViewModel> ExpenseRows { get; }

        /// <summary>Monthly finance report rows ordered by period, most recent first.</summary>
        public IReadOnlyList<MonthlyFinanceReportRowViewModel> MonthlyReports { get; }

        /// <summary>Product revenue history rows, one per active product.</summary>
        public IReadOnlyList<ProductRevenueRowViewModel> ProductRevenueHistory { get; }

        /// <summary>Payroll summary panel data linking to the Employees screen.</summary>
        public PayrollSummaryViewModel PayrollSummary { get; }

        // ── Warning states ───────────────────────────────────────────────────

        /// <summary>True when there are no monthly finance reports yet (new-game empty state).</summary>
        public bool HasNoFinanceHistory { get; }

        /// <summary>True when cash runway is below the warning threshold.</summary>
        public bool HasLowRunwayWarning { get; }

        /// <summary>True when available cash is zero or negative (hiring-block / critical state).</summary>
        public bool HasNegativeCash { get; }

        /// <summary>True when no products are generating revenue this month.</summary>
        public bool HasNoProductRevenue { get; }

        public FinanceViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<FinanceKpiViewModel> kpiCards,
            IReadOnlyList<FinanceBreakdownRowViewModel> revenueRows,
            IReadOnlyList<FinanceBreakdownRowViewModel> expenseRows,
            IReadOnlyList<MonthlyFinanceReportRowViewModel> monthlyReports,
            IReadOnlyList<ProductRevenueRowViewModel> productRevenueHistory,
            PayrollSummaryViewModel payrollSummary,
            bool hasNoFinanceHistory,
            bool hasLowRunwayWarning,
            bool hasNegativeCash,
            bool hasNoProductRevenue)
        {
            ScreenTitle            = screenTitle;
            ScreenSubtitle         = screenSubtitle;
            IsLoading              = isLoading;
            HasError               = hasError;
            ErrorMessage           = errorMessage;
            EmptyStateTitle        = emptyStateTitle;
            EmptyStateBody         = emptyStateBody;
            KpiCards               = kpiCards;
            RevenueRows            = revenueRows;
            ExpenseRows            = expenseRows;
            MonthlyReports         = monthlyReports;
            ProductRevenueHistory  = productRevenueHistory;
            PayrollSummary         = payrollSummary;
            HasNoFinanceHistory    = hasNoFinanceHistory;
            HasLowRunwayWarning    = hasLowRunwayWarning;
            HasNegativeCash        = hasNegativeCash;
            HasNoProductRevenue    = hasNoProductRevenue;
        }
    }
}
