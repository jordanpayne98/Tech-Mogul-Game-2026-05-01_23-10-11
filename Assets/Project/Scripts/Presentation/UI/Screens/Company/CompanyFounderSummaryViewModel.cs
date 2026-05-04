using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure display-data class for the founder/founding team summary card on the Company screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyFounderSummaryViewModel
    {
        /// <summary>Display name of the primary founder, e.g. "Jordan Rivera".</summary>
        public string FounderName { get; }

        /// <summary>Founder's title or role label, e.g. "CEO & Co-founder".</summary>
        public string FounderTitle { get; }

        /// <summary>Short background summary for the founder, e.g. "Former engineer at Meridian Labs".</summary>
        public string FounderBackground { get; }

        /// <summary>Display names of additional founding team members beyond the primary founder.</summary>
        public IReadOnlyList<string> FoundingTeamMembers { get; }

        /// <summary>True when FoundingTeamMembers contains at least one entry.</summary>
        public bool HasFoundingTeam { get; }

        /// <summary>Route ID for the founder/team detail drill-down screen.</summary>
        public string DrillDownRouteId { get; }

        /// <summary>True when the card is navigable to the drill-down screen.</summary>
        public bool IsClickable { get; }

        public CompanyFounderSummaryViewModel(
            string founderName,
            string founderTitle,
            string founderBackground,
            IReadOnlyList<string> foundingTeamMembers,
            bool hasFoundingTeam,
            string drillDownRouteId,
            bool isClickable)
        {
            FounderName         = founderName;
            FounderTitle        = founderTitle;
            FounderBackground   = founderBackground;
            FoundingTeamMembers = foundingTeamMembers;
            HasFoundingTeam     = hasFoundingTeam;
            DrillDownRouteId    = drillDownRouteId;
            IsClickable         = isClickable;
        }
    }
}
