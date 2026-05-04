using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Finance overview screen.
    /// Builds a [Placeholder] static ViewModel with demo finance data, wires click callbacks
    /// to ScreenRouter, and calls View.Bind() to display the data.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FinanceController
    {
        private readonly FinanceView  _view;
        private readonly IScreenRouter _screenRouter;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public FinanceController(FinanceView view, IScreenRouter screenRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;

            _view.OnReportRowClicked      += HandleReportRowClicked;
            _view.OnProductRevenueClicked += HandleProductRevenueClicked;
            _view.OnPayrollClicked        += HandlePayrollClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            FinanceViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                "FinanceController: initialized with [Placeholder] data.");
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleReportRowClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FinanceController: report row clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandleProductRevenueClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"FinanceController: product revenue row clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        private void HandlePayrollClicked()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "FinanceController: payroll summary clicked — navigating to Employees screen.");

            _screenRouter.OpenScreen(ScreenIds.Employees);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] FinanceViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static FinanceViewModel BuildPlaceholderViewModel()
        {
            return new FinanceViewModel(
                screenTitle:            "Finance",
                screenSubtitle:         "Overview [Placeholder]",
                isLoading:              false,
                hasError:               false,
                errorMessage:           string.Empty,
                emptyStateTitle:        "No Finance History",
                emptyStateBody:         "Finance data will appear once your company completes its first month.",
                kpiCards:               BuildPlaceholderKpiCards(),
                revenueRows:            BuildPlaceholderRevenueRows(),
                expenseRows:            BuildPlaceholderExpenseRows(),
                monthlyReports:         BuildPlaceholderMonthlyReports(),
                productRevenueHistory:  BuildPlaceholderProductRevenue(),
                payrollSummary:         BuildPlaceholderPayrollSummary(),
                hasNoFinanceHistory:    false,
                hasLowRunwayWarning:    false,
                hasNegativeCash:        false,
                hasNoProductRevenue:    false);
        }

        private static IReadOnlyList<FinanceKpiViewModel> BuildPlaceholderKpiCards()
        {
            return new List<FinanceKpiViewModel>
            {
                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.Cash,
                    label:         "Cash / Available Funds",
                    value:         "$1,250,000",
                    trendText:     "+12% this month",
                    semanticState: "success"),

                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.NetProfitLoss,
                    label:         "Net Profit / Loss",
                    value:         "-$42,500",
                    trendText:     "Monthly net loss",
                    semanticState: "warning"),

                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.Runway,
                    label:         "Runway",
                    value:         "14.7 months",
                    trendText:     "Based on current burn rate",
                    semanticState: "normal"),

                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.RevenueMtd,
                    label:         "Revenue MTD",
                    value:         "$42,500",
                    trendText:     "+8% vs last month",
                    semanticState: "success"),

                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.Payroll,
                    label:         "Payroll",
                    value:         "$85,000",
                    trendText:     "Monthly payroll cost",
                    semanticState: "normal"),

                new FinanceKpiViewModel(
                    id:            FinanceKpiIds.SupportInfraCost,
                    label:         "Support / Infrastructure",
                    value:         "$12,000",
                    trendText:     "Monthly overhead",
                    semanticState: "normal"),
            };
        }

        private static IReadOnlyList<FinanceBreakdownRowViewModel> BuildPlaceholderRevenueRows()
        {
            return new List<FinanceBreakdownRowViewModel>
            {
                new FinanceBreakdownRowViewModel(
                    category:         "Product Revenue",
                    amount:           "$28,000",
                    percentage:       "66%",
                    trendText:        "+5% vs last month",
                    semanticState:    "success",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),

                new FinanceBreakdownRowViewModel(
                    category:         "Contract Revenue",
                    amount:           "$10,000",
                    percentage:       "24%",
                    trendText:        "Steady",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Contracts,
                    isClickable:      true),

                new FinanceBreakdownRowViewModel(
                    category:         "Hardware Unit Sales",
                    amount:           "$3,500",
                    percentage:       "8%",
                    trendText:        "First units sold",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new FinanceBreakdownRowViewModel(
                    category:         "Subscriptions",
                    amount:           "$1,000",
                    percentage:       "2%",
                    trendText:        "New this month",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),
            };
        }

        private static IReadOnlyList<FinanceBreakdownRowViewModel> BuildPlaceholderExpenseRows()
        {
            return new List<FinanceBreakdownRowViewModel>
            {
                new FinanceBreakdownRowViewModel(
                    category:         "Payroll",
                    amount:           "$85,000",
                    percentage:       "70%",
                    trendText:        "Steady",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Employees,
                    isClickable:      true),

                new FinanceBreakdownRowViewModel(
                    category:         "Infrastructure",
                    amount:           "$8,000",
                    percentage:       "7%",
                    trendText:        "+1 server tier",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Infrastructure,
                    isClickable:      true),

                new FinanceBreakdownRowViewModel(
                    category:         "Support",
                    amount:           "$4,000",
                    percentage:       "3%",
                    trendText:        "Steady",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new FinanceBreakdownRowViewModel(
                    category:         "Marketing",
                    amount:           "$6,000",
                    percentage:       "5%",
                    trendText:        "New campaign",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new FinanceBreakdownRowViewModel(
                    category:         "Research",
                    amount:           "$10,000",
                    percentage:       "8%",
                    trendText:        "Active sprint",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Research,
                    isClickable:      true),

                new FinanceBreakdownRowViewModel(
                    category:         "Hardware Prototyping",
                    amount:           "$5,000",
                    percentage:       "4%",
                    trendText:        "Prototype phase",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),

                new FinanceBreakdownRowViewModel(
                    category:         "Manufacturing",
                    amount:           "$4,000",
                    percentage:       "3%",
                    trendText:        "First production run",
                    semanticState:    "normal",
                    drillDownRouteId: string.Empty,
                    isClickable:      false),
            };
        }

        private static IReadOnlyList<MonthlyFinanceReportRowViewModel> BuildPlaceholderMonthlyReports()
        {
            return new List<MonthlyFinanceReportRowViewModel>
            {
                new MonthlyFinanceReportRowViewModel(
                    reportId:         "report.month_2024_03",
                    period:           "March 2024",
                    revenue:          "$42,500",
                    expenses:         "$122,000",
                    netResult:        "-$79,500",
                    semanticState:    "warning",
                    isClickable:      true,
                    drillDownRouteId: ScreenIds.Reports),

                new MonthlyFinanceReportRowViewModel(
                    reportId:         "report.month_2024_02",
                    period:           "February 2024",
                    revenue:          "$39,400",
                    expenses:         "$115,000",
                    netResult:        "-$75,600",
                    semanticState:    "warning",
                    isClickable:      true,
                    drillDownRouteId: ScreenIds.Reports),

                new MonthlyFinanceReportRowViewModel(
                    reportId:         "report.month_2024_01",
                    period:           "January 2024",
                    revenue:          "$18,000",
                    expenses:         "$100,000",
                    netResult:        "-$82,000",
                    semanticState:    "danger",
                    isClickable:      true,
                    drillDownRouteId: ScreenIds.Reports),
            };
        }

        private static IReadOnlyList<ProductRevenueRowViewModel> BuildPlaceholderProductRevenue()
        {
            return new List<ProductRevenueRowViewModel>
            {
                new ProductRevenueRowViewModel(
                    productId:        "product.alpha_app_001",
                    productName:      "[Placeholder] Alpha App",
                    revenueThisMonth: "$18,000",
                    revenueTotal:     "$42,000",
                    trendText:        "+5% vs last month",
                    semanticState:    "success",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),

                new ProductRevenueRowViewModel(
                    productId:        "product.beta_service_002",
                    productName:      "[Placeholder] Beta Service",
                    revenueThisMonth: "$10,000",
                    revenueTotal:     "$22,000",
                    trendText:        "Steady",
                    semanticState:    "normal",
                    drillDownRouteId: ScreenIds.Products,
                    isClickable:      true),
            };
        }

        private static PayrollSummaryViewModel BuildPlaceholderPayrollSummary()
        {
            return new PayrollSummaryViewModel(
                totalPayroll:     "$85,000 / month",
                employeeCount:    "12 employees",
                averageSalary:    "$7,083 / month",
                trendText:        "+2 employees since last month",
                semanticState:    "normal",
                drillDownRouteId: ScreenIds.Employees,
                isClickable:      true);
        }
    }
}
