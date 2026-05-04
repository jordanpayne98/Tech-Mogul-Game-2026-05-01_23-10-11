namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure display-data class representing the current filter drawer selection state
    /// for the Employees roster screen (screen.employees).
    /// Immutable after construction. No Unity dependencies.
    /// Created by EmployeesController and passed to EmployeesView.
    /// Filter mutations happen in the Controller — this class is read-only display state only.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeeFilterViewModel
    {
        /// <summary>Selected team filter value, e.g. "All Teams". Empty string if unset.</summary>
        public string TeamFilter { get; }

        /// <summary>Selected role filter value, e.g. "Engineer". Empty string if unset.</summary>
        public string RoleFilter { get; }

        /// <summary>Selected seniority filter value, e.g. "Senior". Empty string if unset.</summary>
        public string SeniorityFilter { get; }

        /// <summary>Selected status filter value, e.g. "Active". Empty string if unset.</summary>
        public string StatusFilter { get; }

        /// <summary>Selected morale range filter value, e.g. "60–80". Empty string if unset.</summary>
        public string MoraleRangeFilter { get; }

        /// <summary>Selected salary range filter value, e.g. "$80k–$120k". Empty string if unset.</summary>
        public string SalaryRangeFilter { get; }

        /// <summary>Selected burnout risk filter value, e.g. "High". Empty string if unset.</summary>
        public string BurnoutRiskFilter { get; }

        /// <summary>Selected assignment state filter value, e.g. "Unassigned". Empty string if unset.</summary>
        public string AssignmentStateFilter { get; }

        /// <summary>True when any filter field is non-empty.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Number of filter fields that are currently active (non-empty).</summary>
        public int ActiveFilterCount { get; }

        public EmployeeFilterViewModel(
            string teamFilter,
            string roleFilter,
            string seniorityFilter,
            string statusFilter,
            string moraleRangeFilter,
            string salaryRangeFilter,
            string burnoutRiskFilter,
            string assignmentStateFilter,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            TeamFilter           = teamFilter;
            RoleFilter           = roleFilter;
            SeniorityFilter      = seniorityFilter;
            StatusFilter         = statusFilter;
            MoraleRangeFilter    = moraleRangeFilter;
            SalaryRangeFilter    = salaryRangeFilter;
            BurnoutRiskFilter    = burnoutRiskFilter;
            AssignmentStateFilter = assignmentStateFilter;
            HasActiveFilters     = hasActiveFilters;
            ActiveFilterCount    = activeFilterCount;
        }
    }
}
