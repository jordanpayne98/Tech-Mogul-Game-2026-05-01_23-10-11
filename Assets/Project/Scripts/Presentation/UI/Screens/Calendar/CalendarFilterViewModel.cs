using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Display state for the event type filter selections on the Calendar screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarFilterViewModel
    {
        /// <summary>
        /// Set of event type category stable IDs currently selected by the player.
        /// An empty list means no type filter is active (all events are shown).
        /// Examples: "product.target_release", "contract.deadline", "finance.payroll".
        /// </summary>
        public IReadOnlyList<string> SelectedEventTypes { get; }

        /// <summary>True when at least one event type filter is currently selected.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Number of currently active filter selections.</summary>
        public int ActiveFilterCount { get; }

        public CalendarFilterViewModel(
            IReadOnlyList<string> selectedEventTypes,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            SelectedEventTypes = selectedEventTypes;
            HasActiveFilters   = hasActiveFilters;
            ActiveFilterCount  = activeFilterCount;
        }
    }
}
