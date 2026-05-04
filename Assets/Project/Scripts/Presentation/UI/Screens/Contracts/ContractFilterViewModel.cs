namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Display state for the Contracts screen right filter drawer.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and held by ContractsViewModel.
    /// Each filter property holds the display label of the active filter selection,
    /// or an empty string when that filter is not active.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractFilterViewModel
    {
        /// <summary>Active contract type filter label, or empty string if none.</summary>
        public string TypeFilter { get; }

        /// <summary>Active difficulty filter label, or empty string if none.</summary>
        public string DifficultyFilter { get; }

        /// <summary>Active deadline filter label, or empty string if none.</summary>
        public string DeadlineFilter { get; }

        /// <summary>Active payment range filter label, or empty string if none.</summary>
        public string PaymentFilter { get; }

        /// <summary>Active required roles filter label, or empty string if none.</summary>
        public string RequiredRolesFilter { get; }

        /// <summary>Active client segment filter label, or empty string if none.</summary>
        public string ClientSegmentFilter { get; }

        /// <summary>Active reputation requirement filter label, or empty string if none.</summary>
        public string ReputationRequirementFilter { get; }

        /// <summary>True when at least one filter is active.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Count of currently active filters. Drives the filter badge in the toolbar.</summary>
        public int ActiveFilterCount { get; }

        public ContractFilterViewModel(
            string typeFilter,
            string difficultyFilter,
            string deadlineFilter,
            string paymentFilter,
            string requiredRolesFilter,
            string clientSegmentFilter,
            string reputationRequirementFilter,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            TypeFilter                 = typeFilter;
            DifficultyFilter           = difficultyFilter;
            DeadlineFilter             = deadlineFilter;
            PaymentFilter              = paymentFilter;
            RequiredRolesFilter        = requiredRolesFilter;
            ClientSegmentFilter        = clientSegmentFilter;
            ReputationRequirementFilter = reputationRequirementFilter;
            HasActiveFilters           = hasActiveFilters;
            ActiveFilterCount          = activeFilterCount;
        }
    }
}
