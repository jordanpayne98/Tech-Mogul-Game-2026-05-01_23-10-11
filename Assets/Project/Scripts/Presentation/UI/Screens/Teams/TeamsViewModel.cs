using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Teams management screen (screen.teams).
    /// Immutable after construction. No Unity dependencies.
    /// Created by TeamsController and passed to TeamsView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamsViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Teams".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there are no teams.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there are no teams.</summary>
        public string EmptyStateBody { get; }

        // ── Summary cards ────────────────────────────────────────────────────

        /// <summary>
        /// Ordered summary cards displayed in the top row.
        /// IDs follow <see cref="TeamSummaryCardIds"/>:
        /// total_teams, available, overloaded, avg_morale, avg_cohesion, role_gaps.
        /// </summary>
        public IReadOnlyList<TeamSummaryCardViewModel> SummaryCards { get; }

        // ── Team table ───────────────────────────────────────────────────────

        /// <summary>Team rows to display in the team management table, sorted by the Controller.</summary>
        public IReadOnlyList<TeamRowViewModel> Rows { get; }

        /// <summary>Stable ID of the currently selected team row. Empty string if none selected.</summary>
        public string SelectedTeamId { get; }

        // ── Derived state ─────────────────────────────────────────────────────

        /// <summary>True when there are no teams to display (before filtering; reflects roster emptiness).</summary>
        public bool HasNoTeams { get; }

        /// <summary>
        /// True when the player may create a new team via the create-team modal.
        /// Always false in Phase 5 per Section 14 lock. Phase 6+ wires core team creation logic.
        /// </summary>
        public bool CanCreateTeam { get; }

        public TeamsViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<TeamSummaryCardViewModel> summaryCards,
            IReadOnlyList<TeamRowViewModel> rows,
            string selectedTeamId,
            bool hasNoTeams,
            bool canCreateTeam)
        {
            ScreenTitle    = screenTitle;
            ScreenSubtitle = screenSubtitle;
            IsLoading      = isLoading;
            HasError       = hasError;
            ErrorMessage   = errorMessage;
            EmptyStateTitle = emptyStateTitle;
            EmptyStateBody  = emptyStateBody;
            SummaryCards   = summaryCards;
            Rows           = rows;
            SelectedTeamId = selectedTeamId;
            HasNoTeams     = hasNoTeams;
            CanCreateTeam  = canCreateTeam;
        }
    }
}
