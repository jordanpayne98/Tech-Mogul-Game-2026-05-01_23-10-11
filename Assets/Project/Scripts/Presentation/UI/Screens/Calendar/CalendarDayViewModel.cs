using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Display data for one day cell in the calendar grid (month or week view).
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarDayViewModel
    {
        /// <summary>Formatted day number or label to display in the cell, e.g. "12".</summary>
        public string DayLabel { get; }

        /// <summary>True when this cell represents the current in-game date.</summary>
        public bool IsToday { get; }

        /// <summary>True when this cell belongs to the currently displayed month. False for overflow days from adjacent months.</summary>
        public bool IsCurrentMonth { get; }

        /// <summary>Events scheduled on this day, ordered chronologically.</summary>
        public IReadOnlyList<CalendarEventViewModel> Events { get; }

        /// <summary>
        /// Semantic display state for the day cell.
        /// Examples: "normal", "today", "overflow", "past".
        /// </summary>
        public string SemanticState { get; }

        public CalendarDayViewModel(
            string dayLabel,
            bool isToday,
            bool isCurrentMonth,
            IReadOnlyList<CalendarEventViewModel> events,
            string semanticState)
        {
            DayLabel        = dayLabel;
            IsToday         = isToday;
            IsCurrentMonth  = isCurrentMonth;
            Events          = events;
            SemanticState   = semanticState;
        }
    }
}
