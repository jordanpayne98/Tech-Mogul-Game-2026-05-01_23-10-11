namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Display state for the active filter selections on the Competitors screen toolbar.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorFilterViewModel
    {
        /// <summary>
        /// Currently selected archetype filter label, or empty string if no archetype filter is active.
        /// Possible values per GDD: "Incumbent Giant", "Aggressive Startup", "Research Lab",
        /// "Hardware Manufacturer", "Enterprise Specialist", "Consumer Brand", "Low-Cost Competitor",
        /// "Platform Holder", or empty for all archetypes.
        /// </summary>
        public string ArchetypeFilter { get; }

        /// <summary>
        /// Currently selected primary market filter label, or empty string if no market filter is active.
        /// </summary>
        public string MarketFilter { get; }

        /// <summary>
        /// Currently selected status filter label, or empty string if no status filter is active.
        /// E.g. "Growing", "Stable", "Declining", or empty for all statuses.
        /// </summary>
        public string StatusFilter { get; }

        /// <summary>True when at least one filter is active. Used to show the clear-filters control.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Count of currently active filters. Displayed as a badge on the filter button.</summary>
        public int ActiveFilterCount { get; }

        public CompetitorFilterViewModel(
            string archetypeFilter,
            string marketFilter,
            string statusFilter,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            ArchetypeFilter  = archetypeFilter;
            MarketFilter     = marketFilter;
            StatusFilter     = statusFilter;
            HasActiveFilters = hasActiveFilters;
            ActiveFilterCount = activeFilterCount;
        }
    }
}
