namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Display state for the Products screen filter drawer.
    /// Immutable after construction. No Unity dependencies.
    /// Passed to ProductsView by ProductsController.
    /// [Placeholder] — Phase 5 uses static filter state. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductFilterViewModel
    {
        // ── Active filter selections ──────────────────────────────────────────

        /// <summary>Selected product family filter value, or empty string when none active.</summary>
        public string FamilyFilter { get; }

        /// <summary>Selected product type filter value, or empty string when none active.</summary>
        public string TypeFilter { get; }

        /// <summary>Selected product status filter value, or empty string when none active.</summary>
        public string StatusFilter { get; }

        /// <summary>Selected assigned team filter value, or empty string when none active.</summary>
        public string TeamFilter { get; }

        // ── Derived state ─────────────────────────────────────────────────────

        /// <summary>True when at least one filter selection is non-empty.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Number of filter slots currently active. Used for badge display on the filter button.</summary>
        public int ActiveFilterCount { get; }

        public ProductFilterViewModel(
            string familyFilter,
            string typeFilter,
            string statusFilter,
            string teamFilter,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            FamilyFilter      = familyFilter;
            TypeFilter        = typeFilter;
            StatusFilter      = statusFilter;
            TeamFilter        = teamFilter;
            HasActiveFilters  = hasActiveFilters;
            ActiveFilterCount = activeFilterCount;
        }
    }
}
