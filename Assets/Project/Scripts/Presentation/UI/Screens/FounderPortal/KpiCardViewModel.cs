using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// KPI card IDs used in the Founder Portal KPI grid.
    /// </summary>
    public static class KpiCardIds
    {
        public const string Cash           = "kpi.cash";
        public const string BurnRate       = "kpi.burn_rate";
        public const string Runway         = "kpi.runway";
        public const string RevenueMtd     = "kpi.revenue_mtd";
        public const string ActiveAlerts   = "kpi.active_alerts";
        public const string ActiveProducts = "kpi.active_products";
    }

    /// <summary>
    /// Pure display-data class for a single KPI card in the Founder Portal KPI grid.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// </summary>
    public sealed class KpiCardViewModel
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

        /// <summary>Route ID for drill-down navigation, e.g. "screen.finance".</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the card responds to click/tap input.</summary>
        public bool IsClickable { get; }

        public KpiCardViewModel(
            string id,
            string label,
            string value,
            string trendText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Id               = id;
            Label            = label;
            Value            = value;
            TrendText        = trendText;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
