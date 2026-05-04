namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Pure display-data class for a single row in the monthly finance reports table.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// DrillDownRouteId is empty string when the row is not clickable.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MonthlyFinanceReportRowViewModel
    {
        /// <summary>Stable report ID used for navigation, e.g. "report.month_2024_01".</summary>
        public string ReportId { get; }

        /// <summary>Pre-formatted period label, e.g. "January 2024".</summary>
        public string Period { get; }

        /// <summary>Pre-formatted total revenue for the period, e.g. "$120,000".</summary>
        public string Revenue { get; }

        /// <summary>Pre-formatted total expenses for the period, e.g. "$95,000".</summary>
        public string Expenses { get; }

        /// <summary>Pre-formatted net result for the period, e.g. "+$25,000" or "-$10,000".</summary>
        public string NetResult { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        /// <summary>Whether the row responds to click/tap input to open the report detail.</summary>
        public bool IsClickable { get; }

        /// <summary>Route ID for drill-down navigation. Empty string when not navigable.</summary>
        public string DrillDownRouteId { get; }

        public MonthlyFinanceReportRowViewModel(
            string reportId,
            string period,
            string revenue,
            string expenses,
            string netResult,
            string semanticState,
            bool isClickable,
            string drillDownRouteId)
        {
            ReportId         = reportId;
            Period           = period;
            Revenue          = revenue;
            Expenses         = expenses;
            NetResult        = netResult;
            SemanticState    = semanticState;
            IsClickable      = isClickable;
            DrillDownRouteId = drillDownRouteId;
        }
    }
}
