using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Company profile screen (screen.company).
    /// Immutable after construction. No Unity dependencies.
    /// Created by CompanyController and passed to CompanyView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Company Profile".</summary>
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

        // ── Company-specific content ─────────────────────────────────────────

        /// <summary>Display data for the company identity card.</summary>
        public CompanyIdentityViewModel Identity { get; }

        /// <summary>Display data for the reputation/status card.</summary>
        public CompanyStatusCardViewModel Status { get; }

        /// <summary>Display data for the founder/founding team summary card.</summary>
        public CompanyFounderSummaryViewModel FounderSummary { get; }

        /// <summary>Ordered list of milestone/history rows.</summary>
        public IReadOnlyList<CompanyMilestoneRowViewModel> Milestones { get; }

        // ── Warning / empty states ───────────────────────────────────────────

        /// <summary>True when the Milestones list contains at least one entry.</summary>
        public bool HasMilestones { get; }

        /// <summary>True when required company profile data is absent or incomplete.</summary>
        public bool HasMissingProfileData { get; }

        /// <summary>True when reputation data is unavailable or not yet established.</summary>
        public bool HasUnknownReputation { get; }

        public CompanyViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            CompanyIdentityViewModel identity,
            CompanyStatusCardViewModel status,
            CompanyFounderSummaryViewModel founderSummary,
            IReadOnlyList<CompanyMilestoneRowViewModel> milestones,
            bool hasMilestones,
            bool hasMissingProfileData,
            bool hasUnknownReputation)
        {
            ScreenTitle           = screenTitle;
            ScreenSubtitle        = screenSubtitle;
            IsLoading             = isLoading;
            HasError              = hasError;
            ErrorMessage          = errorMessage;
            EmptyStateTitle       = emptyStateTitle;
            EmptyStateBody        = emptyStateBody;
            Identity              = identity;
            Status                = status;
            FounderSummary        = founderSummary;
            Milestones            = milestones;
            HasMilestones         = hasMilestones;
            HasMissingProfileData = hasMissingProfileData;
            HasUnknownReputation  = hasUnknownReputation;
        }
    }
}
