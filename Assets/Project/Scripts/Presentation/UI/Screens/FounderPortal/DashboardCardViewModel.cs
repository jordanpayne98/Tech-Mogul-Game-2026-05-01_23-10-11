using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Pure display-data class for one main dashboard summary card in the Founder Portal.
    /// Covers card types: Inbox, Products, Team, Hiring, Milestones, Market, Infrastructure.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger".
    /// </summary>
    public sealed class DashboardCardViewModel
    {
        /// <summary>Stable ID, e.g. "card.inbox".</summary>
        public string Id { get; }

        /// <summary>Card title, e.g. "Inbox / Requires Decision".</summary>
        public string Title { get; }

        /// <summary>Short summary description or count, e.g. "3 items pending".</summary>
        public string SummaryText { get; }

        /// <summary>Semantic state string: "normal", "warning", or "danger".</summary>
        public string SemanticState { get; }

        /// <summary>Target screen route ID for drill-down navigation.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>Whether the card responds to click/tap input.</summary>
        public bool IsClickable { get; }

        /// <summary>Optional 2–3 pre-formatted preview line strings for the card body.</summary>
        public IReadOnlyList<string> DetailLines { get; }

        public DashboardCardViewModel(
            string id,
            string title,
            string summaryText,
            string semanticState,
            string drillDownRouteId,
            bool isClickable,
            IReadOnlyList<string> detailLines)
        {
            Id               = id;
            Title            = title;
            SummaryText      = summaryText;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
            DetailLines      = detailLines;
        }
    }
}
