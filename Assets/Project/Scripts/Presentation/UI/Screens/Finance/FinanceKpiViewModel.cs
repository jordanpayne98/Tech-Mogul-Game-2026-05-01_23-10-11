namespace Project.Presentation.UI.Screens.Finance
{
    /// <summary>
    /// Stable KPI card IDs for the Finance screen KPI row.
    /// </summary>
    public static class FinanceKpiIds
    {
        public const string Cash              = "kpi.cash";
        public const string NetProfitLoss     = "kpi.net_profit_loss";
        public const string Runway            = "kpi.runway";
        public const string RevenueMtd        = "kpi.revenue_mtd";
        public const string Payroll           = "kpi.payroll";
        public const string SupportInfraCost  = "kpi.support_infra_cost";
    }

    /// <summary>
    /// Pure display-data class for a single KPI card on the Finance screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class FinanceKpiViewModel
    {
        /// <summary>Stable ID, e.g. "kpi.cash".</summary>
        public string Id { get; }

        /// <summary>Display label, e.g. "Cash / Available Funds".</summary>
        public string Label { get; }

        /// <summary>Pre-formatted value string, e.g. "$1,250,000".</summary>
        public string Value { get; }

        /// <summary>Pre-formatted trend text, e.g. "+12% this month". Empty string if no trend.</summary>
        public string TrendText { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        public FinanceKpiViewModel(
            string id,
            string label,
            string value,
            string trendText,
            string semanticState)
        {
            Id            = id;
            Label         = label;
            Value         = value;
            TrendText     = trendText;
            SemanticState = semanticState;
        }
    }
}
