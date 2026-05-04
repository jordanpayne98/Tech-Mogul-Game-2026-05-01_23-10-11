namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure display-data class for the payroll summary panel on the Finance screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// DrillDownRouteId links to the Employees screen.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class PayrollSummaryViewModel
    {
        /// <summary>Pre-formatted total monthly payroll cost, e.g. "$48,000 / month".</summary>
        public string TotalPayroll { get; }

        /// <summary>Pre-formatted number of employees on payroll, e.g. "12 employees".</summary>
        public string EmployeeCount { get; }

        /// <summary>Pre-formatted average salary per employee, e.g. "$4,000 / month".</summary>
        public string AverageSalary { get; }

        /// <summary>Pre-formatted trend text, e.g. "+2 since last month". Empty string if no trend.</summary>
        public string TrendText { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>Route ID for drill-down navigation to the Employees screen, e.g. "screen.employees".</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the payroll summary panel responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public PayrollSummaryViewModel(
            string totalPayroll,
            string employeeCount,
            string averageSalary,
            string trendText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            TotalPayroll     = totalPayroll;
            EmployeeCount    = employeeCount;
            AverageSalary    = averageSalary;
            TrendText        = trendText;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
