using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Stable tab IDs for the Recruitment Hub screen tab strip.
    /// </summary>
    public static class RecruitmentHubTabIds
    {
        public const string CandidatePool = "tab.candidate_pool";
        public const string Shortlist     = "tab.shortlist";
        public const string JobPosts      = "tab.job_posts";
        public const string OffersSent    = "tab.offers_sent";
        public const string Accepted      = "tab.accepted";
        public const string Rejected      = "tab.rejected";
    }

    /// <summary>
    /// Top-level aggregate ViewModel for the Recruitment Hub screen (screen.recruitment).
    /// Immutable after construction. No Unity dependencies.
    /// Created by RecruitmentHubController and passed to RecruitmentHubView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// Candidate information uncertainty is modelled via CandidateRowViewModel.VisibleSkills
    /// and CandidateProfileViewModel.HiddenSkillSlots per Phase 5D Section 14 lock.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class RecruitmentHubViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Recruitment Hub".</summary>
        public string ScreenTitle { get; }

        /// <summary>Optional subtitle or contextual description shown beneath the title.</summary>
        public string ScreenSubtitle { get; }

        /// <summary>True while the screen is loading data asynchronously.</summary>
        public bool IsLoading { get; }

        /// <summary>True when a data or service error has occurred.</summary>
        public bool HasError { get; }

        /// <summary>Error message displayed when HasError is true.</summary>
        public string ErrorMessage { get; }

        /// <summary>Title shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateTitle { get; }

        /// <summary>Body text shown in the empty-state panel when there is no content.</summary>
        public string EmptyStateBody { get; }

        // ── Tab and toolbar state ────────────────────────────────────────────

        /// <summary>
        /// Ordered list of stable tab IDs visible in the tab strip.
        /// Use RecruitmentHubTabIds constants for stable references.
        /// </summary>
        public IReadOnlyList<string> VisibleTabs { get; }

        /// <summary>Stable ID of the currently active tab, e.g. "tab.candidate_pool".</summary>
        public string ActiveTabId { get; }

        /// <summary>Current search text entered in the toolbar search field. Empty string if no search is active.</summary>
        public string SearchText { get; }

        /// <summary>Current filter drawer selection state.</summary>
        public CandidateFilterViewModel FilterState { get; }

        // ── Content collections ──────────────────────────────────────────────

        /// <summary>Candidate rows displayed in the active candidate tab (candidate pool, shortlist, etc.).</summary>
        public IReadOnlyList<CandidateRowViewModel> Rows { get; }

        /// <summary>Job post rows displayed in the Job Posts tab.</summary>
        public IReadOnlyList<JobPostRowViewModel> JobPosts { get; }

        /// <summary>
        /// Stable ID of the currently selected candidate row, used to highlight the active row.
        /// Empty string if no candidate is selected.
        /// </summary>
        public string SelectedCandidateId { get; }

        // ── Derived visibility flags ─────────────────────────────────────────

        /// <summary>True when the active tab has no candidate rows to display.</summary>
        public bool HasNoCandidates { get; }

        public RecruitmentHubViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<string> visibleTabs,
            string activeTabId,
            string searchText,
            CandidateFilterViewModel filterState,
            IReadOnlyList<CandidateRowViewModel> rows,
            IReadOnlyList<JobPostRowViewModel> jobPosts,
            string selectedCandidateId,
            bool hasNoCandidates)
        {
            ScreenTitle         = screenTitle;
            ScreenSubtitle      = screenSubtitle;
            IsLoading           = isLoading;
            HasError            = hasError;
            ErrorMessage        = errorMessage;
            EmptyStateTitle     = emptyStateTitle;
            EmptyStateBody      = emptyStateBody;
            VisibleTabs         = visibleTabs;
            ActiveTabId         = activeTabId;
            SearchText          = searchText;
            FilterState         = filterState;
            Rows                = rows;
            JobPosts            = jobPosts;
            SelectedCandidateId = selectedCandidateId;
            HasNoCandidates     = hasNoCandidates;
        }
    }
}
