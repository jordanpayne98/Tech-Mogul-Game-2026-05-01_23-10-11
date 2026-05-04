using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Top-level aggregate ViewModel for the Calendar screen (screen.calendar).
    /// Immutable after construction. No Unity dependencies.
    /// Created by CalendarScreenController and passed to CalendarScreenView.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarScreenViewModel
    {
        // ── Common screen fields ─────────────────────────────────────────────

        /// <summary>Main title shown in the screen header, e.g. "Calendar".</summary>
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

        // ── View state ───────────────────────────────────────────────────────

        /// <summary>Formatted display string for the currently visible date range, e.g. "June 2027".</summary>
        public string CurrentDateDisplay { get; }

        /// <summary>Active view mode identifier — "month" or "week".</summary>
        public string ActiveViewMode { get; }

        /// <summary>Current search text entered by the player. Empty string when no search is active.</summary>
        public string SearchText { get; }

        // ── Content ──────────────────────────────────────────────────────────

        /// <summary>Day cells for the currently visible calendar grid (month or week view).</summary>
        public IReadOnlyList<CalendarDayViewModel> Days { get; }

        /// <summary>Upcoming events shown in the upcoming events list panel.</summary>
        public IReadOnlyList<UpcomingEventRowViewModel> UpcomingEvents { get; }

        /// <summary>Current state of the event type filter selections.</summary>
        public CalendarFilterViewModel FilterState { get; }

        // ── Derived visibility flags ─────────────────────────────────────────

        /// <summary>True when there are no events in the current view range.</summary>
        public bool HasNoEvents { get; }

        /// <summary>True when all events have been filtered out by the active filter selection.</summary>
        public bool HasFilteredEmpty { get; }

        public CalendarScreenViewModel(
            string screenTitle,
            string screenSubtitle,
            bool isLoading,
            bool hasError,
            string errorMessage,
            string emptyStateTitle,
            string emptyStateBody,
            string currentDateDisplay,
            string activeViewMode,
            string searchText,
            IReadOnlyList<CalendarDayViewModel> days,
            IReadOnlyList<UpcomingEventRowViewModel> upcomingEvents,
            CalendarFilterViewModel filterState,
            bool hasNoEvents,
            bool hasFilteredEmpty)
        {
            ScreenTitle         = screenTitle;
            ScreenSubtitle      = screenSubtitle;
            IsLoading           = isLoading;
            HasError            = hasError;
            ErrorMessage        = errorMessage;
            EmptyStateTitle     = emptyStateTitle;
            EmptyStateBody      = emptyStateBody;
            CurrentDateDisplay  = currentDateDisplay;
            ActiveViewMode      = activeViewMode;
            SearchText          = searchText;
            Days                = days;
            UpcomingEvents      = upcomingEvents;
            FilterState         = filterState;
            HasNoEvents         = hasNoEvents;
            HasFilteredEmpty    = hasFilteredEmpty;
        }
    }
}
