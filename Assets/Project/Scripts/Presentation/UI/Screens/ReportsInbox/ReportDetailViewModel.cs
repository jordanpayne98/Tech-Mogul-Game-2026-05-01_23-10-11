using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Pure display-data class for the right-hand report preview and detail panel in the Reports Inbox.
    /// Immutable after construction. No Unity dependencies.
    /// Reports explain outcomes, changes, causes, and related entity links.
    /// Reports must not prescribe strategy or optimal player actions (per Section 14 lock).
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — Phase 5 uses static data. Live report content wired in Phase 6+.
    /// </summary>
    public sealed class ReportDetailViewModel
    {
        /// <summary>Stable report ID matching the selected ReportRowViewModel.</summary>
        public string ReportId { get; }

        /// <summary>Display title of the report, e.g. "Monthly Finance Summary — Month 3".</summary>
        public string Title { get; }

        /// <summary>Pre-formatted date string, e.g. "Month 3, Week 2".</summary>
        public string Date { get; }

        /// <summary>Category label, e.g. "Finance".</summary>
        public string Category { get; }

        /// <summary>Full summary or body text describing the report outcome.</summary>
        public string Summary { get; }

        /// <summary>Pre-formatted key number strings, e.g. "Revenue: $42,000". Empty list if none.</summary>
        public IReadOnlyList<string> KeyNumbers { get; }

        /// <summary>Pre-formatted strings describing what changed since the last report. Empty list if none.</summary>
        public IReadOnlyList<string> WhatChanged { get; }

        /// <summary>Pre-formatted strings describing cause indicators for the reported changes. Empty list if none.</summary>
        public IReadOnlyList<string> CauseIndicators { get; }

        /// <summary>Related entity links that allow drill-down navigation from this report.</summary>
        public IReadOnlyList<ReportRelatedEntityViewModel> RelatedEntities { get; }

        /// <summary>Action buttons available for this report in the detail panel.</summary>
        public IReadOnlyList<ReportActionViewModel> Actions { get; }

        /// <summary>True when this report requires a player decision before it can be resolved.</summary>
        public bool RequiresDecision { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        public ReportDetailViewModel(
            string reportId,
            string title,
            string date,
            string category,
            string summary,
            IReadOnlyList<string> keyNumbers,
            IReadOnlyList<string> whatChanged,
            IReadOnlyList<string> causeIndicators,
            IReadOnlyList<ReportRelatedEntityViewModel> relatedEntities,
            IReadOnlyList<ReportActionViewModel> actions,
            bool requiresDecision,
            string semanticState)
        {
            ReportId         = reportId;
            Title            = title;
            Date             = date;
            Category         = category;
            Summary          = summary;
            KeyNumbers       = keyNumbers;
            WhatChanged      = whatChanged;
            CauseIndicators  = causeIndicators;
            RelatedEntities  = relatedEntities;
            Actions          = actions;
            RequiresDecision = requiresDecision;
            SemanticState    = semanticState;
        }
    }
}
