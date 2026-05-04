using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Research screen (screen.research).
    /// Immutable after construction. No Unity dependencies.
    /// Created by ResearchController and passed to ResearchView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Research".</summary>
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

        // ── Track tabs ───────────────────────────────────────────────────────

        /// <summary>All available research track tabs.</summary>
        public IReadOnlyList<ResearchTrackViewModel> Tracks { get; }

        /// <summary>Stable ID of the currently active track tab.</summary>
        public string ActiveTrackId { get; }

        // ── Project lists ────────────────────────────────────────────────────

        /// <summary>Research projects available for assignment in the active track.</summary>
        public IReadOnlyList<ResearchProjectRowViewModel> AvailableProjects { get; }

        /// <summary>Locked or future research projects in the active track (prerequisites not met).</summary>
        public IReadOnlyList<ResearchProjectRowViewModel> LockedProjects { get; }

        // ── Assigned research ────────────────────────────────────────────────

        /// <summary>Currently assigned research project panel data.</summary>
        public AssignedResearchViewModel AssignedResearch { get; }

        // ── Derived visibility flags ─────────────────────────────────────────

        /// <summary>True when AvailableProjects is empty for the active track.</summary>
        public bool HasNoAvailableProjects { get; }

        public ResearchViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            IReadOnlyList<ResearchTrackViewModel> tracks,
            string activeTrackId,
            IReadOnlyList<ResearchProjectRowViewModel> availableProjects,
            IReadOnlyList<ResearchProjectRowViewModel> lockedProjects,
            AssignedResearchViewModel assignedResearch,
            bool hasNoAvailableProjects)
        {
            ScreenTitle            = screenTitle;
            ScreenSubtitle         = screenSubtitle;
            IsLoading              = isLoading;
            HasError               = hasError;
            ErrorMessage           = errorMessage;
            EmptyStateTitle        = emptyStateTitle;
            EmptyStateBody         = emptyStateBody;
            Tracks                 = tracks;
            ActiveTrackId          = activeTrackId;
            AvailableProjects      = availableProjects;
            LockedProjects         = lockedProjects;
            AssignedResearch       = assignedResearch;
            HasNoAvailableProjects = hasNoAvailableProjects;
        }
    }
}
