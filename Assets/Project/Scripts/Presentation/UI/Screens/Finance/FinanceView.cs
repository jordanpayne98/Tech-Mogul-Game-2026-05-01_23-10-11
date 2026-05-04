using System;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Finance overview screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates KPI cards, breakdown rows, report rows, product revenue rows,
    /// and the payroll summary panel.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FinanceView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from FinanceScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a monthly report row is clicked. Argument is the DrillDownRouteId.</summary>
        public event Action<string> OnReportRowClicked;

        /// <summary>Fired when a product revenue row is clicked. Argument is the DrillDownRouteId.</summary>
        public event Action<string> OnProductRevenueClicked;

        /// <summary>Fired when the payroll summary panel is clicked.</summary>
        public event Action OnPayrollClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _contentContainer;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;
        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Dynamic content containers ──────────────────────────────────────────────

        private readonly VisualElement _kpiRow;
        private readonly VisualElement _revenueBreakdownList;
        private readonly Label         _revenueTotalValue;
        private readonly VisualElement _expenseBreakdownList;
        private readonly Label         _expenseTotalValue;
        private readonly VisualElement _reportsTable;
        private readonly VisualElement _payrollSection;
        private readonly VisualElement _payrollList;
        private readonly Label         _payrollTotalValue;
        private readonly VisualElement _productRevenueTable;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public FinanceView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "FinanceView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");
            _contentContainer = QueryElement(root, "ContentContainer");

            // ── Header ───────────────────────────────────────────────────────────────

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");
            _errorMessage   = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");
            LogIfNull(_errorMessage,   "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");

            // ── Dynamic containers ───────────────────────────────────────────────────

            _kpiRow               = QueryElement(root, "KpiRow");
            _revenueBreakdownList = QueryElement(root, "RevenueBreakdownList");
            _revenueTotalValue    = root.Q<Label>("RevenueTotalValue");
            _expenseBreakdownList = QueryElement(root, "ExpenseBreakdownList");
            _expenseTotalValue    = root.Q<Label>("ExpenseTotalValue");
            _reportsTable         = QueryElement(root, "ReportsTable");
            _payrollSection       = QueryElement(root, "PayrollSection");
            _payrollList          = QueryElement(root, "PayrollList");
            _payrollTotalValue    = root.Q<Label>("PayrollTotalValue");
            _productRevenueTable  = QueryElement(root, "ProductRevenueTable");

            LogIfNull(_revenueTotalValue,   "RevenueTotalValue");
            LogIfNull(_expenseTotalValue,   "ExpenseTotalValue");
            LogIfNull(_payrollTotalValue,   "PayrollTotalValue");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic card/row lists on each call.
        /// </summary>
        public void Bind(FinanceViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "FinanceView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No finance data available.");
                return;
            }

            // ── Loading state ────────────────────────────────────────────────────────

            if (viewModel.IsLoading)
            {
                SetVisible(_loadingState,     true);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       false);
                return;
            }

            // ── Error state ──────────────────────────────────────────────────────────

            if (viewModel.HasError)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            // ── Empty state ──────────────────────────────────────────────────────────

            if (viewModel.HasNoFinanceHistory)
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       true);

                if (_emptyStateTitle != null)
                {
                    _emptyStateTitle.text = viewModel.EmptyStateTitle;
                }

                if (_emptyStateBody != null)
                {
                    _emptyStateBody.text = viewModel.EmptyStateBody;
                }

                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, true);
            SetVisible(_errorState,       false);
            SetVisible(_emptyState,       false);

            // Header
            if (_headerTitle != null)
            {
                _headerTitle.text = viewModel.ScreenTitle;
            }

            if (_headerSubtitle != null)
            {
                _headerSubtitle.text = viewModel.ScreenSubtitle;
            }

            // KPI row
            BuildKpiRow(viewModel);

            // Revenue breakdown
            BuildRevenueBreakdown(viewModel);

            // Expense breakdown
            BuildExpenseBreakdown(viewModel);

            // Monthly reports table
            BuildReportsTable(viewModel);

            // Payroll summary
            BuildPayrollSummary(viewModel);

            // Product revenue history
            BuildProductRevenueTable(viewModel);

            // Root warning state classes
            ApplyRootWarningClasses(viewModel);
        }

        // ─── Private — KPI cards ─────────────────────────────────────────────────────

        private void BuildKpiRow(FinanceViewModel viewModel)
        {
            if (_kpiRow == null)
            {
                return;
            }

            _kpiRow.Clear();

            if (viewModel.KpiCards == null)
            {
                return;
            }

            foreach (FinanceKpiViewModel card in viewModel.KpiCards)
            {
                VisualElement cardEl = CreateKpiCard(card);
                _kpiRow.Add(cardEl);
            }
        }

        private static VisualElement CreateKpiCard(FinanceKpiViewModel card)
        {
            var container = new VisualElement();
            container.AddToClassList("finance__kpi-card");
            ApplySemanticStateClass(container, card.SemanticState);

            var labelEl = new Label(card.Label);
            labelEl.AddToClassList("finance__kpi-card__label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(card.Value);
            valueEl.AddToClassList("finance__kpi-card__value");
            valueEl.AddToClassList("text-heading");

            var trendEl = new Label(card.TrendText);
            trendEl.AddToClassList("finance__kpi-card__trend");
            trendEl.AddToClassList("text-caption");

            container.Add(labelEl);
            container.Add(valueEl);
            container.Add(trendEl);

            return container;
        }

        // ─── Private — breakdown rows ─────────────────────────────────────────────────

        private void BuildRevenueBreakdown(FinanceViewModel viewModel)
        {
            if (_revenueBreakdownList == null)
            {
                return;
            }

            _revenueBreakdownList.Clear();

            if (viewModel.RevenueRows == null)
            {
                return;
            }

            foreach (FinanceBreakdownRowViewModel row in viewModel.RevenueRows)
            {
                VisualElement rowEl = CreateBreakdownRow(row);
                _revenueBreakdownList.Add(rowEl);
            }

            // Update total label from last row's amount as a simple aggregate display.
            // [Placeholder] — Phase 6+ will sum from live data.
            if (_revenueTotalValue != null && viewModel.RevenueRows.Count > 0)
            {
                _revenueTotalValue.text = viewModel.RevenueRows[0].Amount;
            }
        }

        private void BuildExpenseBreakdown(FinanceViewModel viewModel)
        {
            if (_expenseBreakdownList == null)
            {
                return;
            }

            _expenseBreakdownList.Clear();

            if (viewModel.ExpenseRows == null)
            {
                return;
            }

            foreach (FinanceBreakdownRowViewModel row in viewModel.ExpenseRows)
            {
                VisualElement rowEl = CreateBreakdownRow(row);
                _expenseBreakdownList.Add(rowEl);
            }

            if (_expenseTotalValue != null && viewModel.ExpenseRows.Count > 0)
            {
                _expenseTotalValue.text = viewModel.ExpenseRows[0].Amount;
            }
        }

        private static VisualElement CreateBreakdownRow(FinanceBreakdownRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("finance__breakdown-row");
            ApplySemanticStateClass(rowEl, row.SemanticState);

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            var categoryEl = new Label(row.Category);
            categoryEl.AddToClassList("finance__breakdown-row__category");
            categoryEl.AddToClassList("text-body");

            var amountEl = new Label(row.Amount);
            amountEl.AddToClassList("finance__breakdown-row__amount");
            amountEl.AddToClassList("text-body-strong");

            var percentEl = new Label(row.Percentage);
            percentEl.AddToClassList("finance__breakdown-row__percent");
            percentEl.AddToClassList("text-caption");

            var trendEl = new Label(row.TrendText);
            trendEl.AddToClassList("finance__breakdown-row__trend");
            trendEl.AddToClassList("text-caption");

            rowEl.Add(categoryEl);
            rowEl.Add(amountEl);
            rowEl.Add(percentEl);
            rowEl.Add(trendEl);

            return rowEl;
        }

        // ─── Private — monthly reports table ─────────────────────────────────────────

        private void BuildReportsTable(FinanceViewModel viewModel)
        {
            if (_reportsTable == null)
            {
                return;
            }

            _reportsTable.Clear();

            if (viewModel.MonthlyReports == null)
            {
                return;
            }

            foreach (MonthlyFinanceReportRowViewModel report in viewModel.MonthlyReports)
            {
                VisualElement rowEl = CreateReportRow(report);
                _reportsTable.Add(rowEl);
            }
        }

        private VisualElement CreateReportRow(MonthlyFinanceReportRowViewModel report)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("finance__report-row");
            ApplySemanticStateClass(rowEl, report.SemanticState);

            if (report.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            var periodEl = new Label(report.Period);
            periodEl.AddToClassList("finance__report-row__period");
            periodEl.AddToClassList("text-body");

            var revenueEl = new Label(report.Revenue);
            revenueEl.AddToClassList("finance__report-row__revenue");
            revenueEl.AddToClassList("text-body");

            var expensesEl = new Label(report.Expenses);
            expensesEl.AddToClassList("finance__report-row__expenses");
            expensesEl.AddToClassList("text-body");

            var netEl = new Label(report.NetResult);
            netEl.AddToClassList("finance__report-row__net");
            netEl.AddToClassList("text-body-strong");

            rowEl.Add(periodEl);
            rowEl.Add(revenueEl);
            rowEl.Add(expensesEl);
            rowEl.Add(netEl);

            if (report.IsClickable)
            {
                string routeId = report.DrillDownRouteId;
                rowEl.RegisterCallback<ClickEvent>(_ => OnReportRowClicked?.Invoke(routeId));
            }

            return rowEl;
        }

        // ─── Private — payroll summary ────────────────────────────────────────────────

        private void BuildPayrollSummary(FinanceViewModel viewModel)
        {
            if (_payrollSection == null)
            {
                return;
            }

            PayrollSummaryViewModel payroll = viewModel.PayrollSummary;

            if (payroll == null)
            {
                return;
            }

            ApplySemanticStateClass(_payrollSection, payroll.SemanticState);

            if (payroll.IsClickable)
            {
                _payrollSection.AddToClassList("is-clickable");
                _payrollSection.RegisterCallback<ClickEvent>(_ => OnPayrollClicked?.Invoke());
            }

            // Update the payroll footer total value label.
            if (_payrollTotalValue != null)
            {
                _payrollTotalValue.text = payroll.TotalPayroll;
            }

            // Populate payroll list with summary rows.
            if (_payrollList != null)
            {
                _payrollList.Clear();

                // Employee count row
                VisualElement countRow = CreatePayrollSummaryRow("Employees on Payroll", payroll.EmployeeCount);
                _payrollList.Add(countRow);

                // Average salary row
                VisualElement avgRow = CreatePayrollSummaryRow("Average Salary", payroll.AverageSalary);
                _payrollList.Add(avgRow);

                // Trend row
                if (!string.IsNullOrEmpty(payroll.TrendText))
                {
                    VisualElement trendRow = CreatePayrollSummaryRow("Change", payroll.TrendText);
                    _payrollList.Add(trendRow);
                }
            }
        }

        private static VisualElement CreatePayrollSummaryRow(string label, string value)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("finance__payroll-summary-row");

            var labelEl = new Label(label);
            labelEl.AddToClassList("finance__payroll-summary-row__label");
            labelEl.AddToClassList("text-body");

            var valueEl = new Label(value);
            valueEl.AddToClassList("finance__payroll-summary-row__value");
            valueEl.AddToClassList("text-body-strong");

            rowEl.Add(labelEl);
            rowEl.Add(valueEl);

            return rowEl;
        }

        // ─── Private — product revenue table ─────────────────────────────────────────

        private void BuildProductRevenueTable(FinanceViewModel viewModel)
        {
            if (_productRevenueTable == null)
            {
                return;
            }

            _productRevenueTable.Clear();

            if (viewModel.ProductRevenueHistory == null)
            {
                return;
            }

            foreach (ProductRevenueRowViewModel product in viewModel.ProductRevenueHistory)
            {
                VisualElement rowEl = CreateProductRevenueRow(product);
                _productRevenueTable.Add(rowEl);
            }
        }

        private VisualElement CreateProductRevenueRow(ProductRevenueRowViewModel product)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("finance__product-revenue-row");
            ApplySemanticStateClass(rowEl, product.SemanticState);

            if (product.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            var nameEl = new Label(product.ProductName);
            nameEl.AddToClassList("finance__product-revenue-row__name");
            nameEl.AddToClassList("text-body");

            var monthRevenueEl = new Label(product.RevenueThisMonth);
            monthRevenueEl.AddToClassList("finance__product-revenue-row__month-revenue");
            monthRevenueEl.AddToClassList("text-body");

            var totalRevenueEl = new Label(product.RevenueTotal);
            totalRevenueEl.AddToClassList("finance__product-revenue-row__total-revenue");
            totalRevenueEl.AddToClassList("text-body-strong");

            var trendEl = new Label(product.TrendText);
            trendEl.AddToClassList("finance__product-revenue-row__trend");
            trendEl.AddToClassList("text-caption");

            rowEl.Add(nameEl);
            rowEl.Add(monthRevenueEl);
            rowEl.Add(totalRevenueEl);
            rowEl.Add(trendEl);

            if (product.IsClickable)
            {
                string routeId = product.DrillDownRouteId;
                rowEl.RegisterCallback<ClickEvent>(_ => OnProductRevenueClicked?.Invoke(routeId));
            }

            return rowEl;
        }

        // ─── Private — state helpers ─────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_emptyState,       false);
            SetVisible(_errorState,       true);

            if (_errorMessage != null)
            {
                _errorMessage.text = message ?? string.Empty;
            }
        }

        private void ApplyRootWarningClasses(FinanceViewModel viewModel)
        {
            bool hasAnyWarning = viewModel.HasLowRunwayWarning
                              || viewModel.HasNegativeCash
                              || viewModel.HasNoProductRevenue;

            if (hasAnyWarning)
            {
                Root.AddToClassList("has-warning");
            }
            else
            {
                Root.RemoveFromClassList("has-warning");
            }
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            if (element == null)
            {
                return;
            }

            if (visible)
            {
                element.RemoveFromClassList("is-hidden");
            }
            else
            {
                element.AddToClassList("is-hidden");
            }
        }

        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        // ─── Private — query helpers ─────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            LogIfNull(element, name);
            return element;
        }

        private static void LogIfNull(object element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"FinanceView: element '{name}' not found in UXML root.");
            }
        }
    }
}
