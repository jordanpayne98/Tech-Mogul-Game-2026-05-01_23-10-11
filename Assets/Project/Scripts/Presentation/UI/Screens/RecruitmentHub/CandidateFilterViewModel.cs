namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure display-data class for the Recruitment Hub filter drawer state.
    /// Immutable after construction. No Unity dependencies.
    /// Created by RecruitmentHubController and passed to RecruitmentHubView.
    /// HasActiveFilters and ActiveFilterCount are derived from individual filter fields.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CandidateFilterViewModel
    {
        /// <summary>Selected role filter value, e.g. "Software Engineer". Empty string if none selected.</summary>
        public string RoleFilter { get; }

        /// <summary>Selected seniority filter value, e.g. "Senior". Empty string if none selected.</summary>
        public string SeniorityFilter { get; }

        /// <summary>Selected salary range filter display text, e.g. "$80k–$120k". Empty string if none selected.</summary>
        public string SalaryRangeFilter { get; }

        /// <summary>Selected availability filter value, e.g. "Immediately". Empty string if none selected.</summary>
        public string AvailabilityFilter { get; }

        /// <summary>Selected interest filter value, e.g. "High". Empty string if none selected.</summary>
        public string InterestFilter { get; }

        /// <summary>Selected skill tag filter text, e.g. "Python, React". Empty string if none selected.</summary>
        public string SkillTagsFilter { get; }

        /// <summary>Selected offer status filter value, e.g. "No Offer". Empty string if none selected.</summary>
        public string OfferStatusFilter { get; }

        /// <summary>Selected confidence level filter value, e.g. "High". Empty string if none selected.</summary>
        public string ConfidenceLevelFilter { get; }

        /// <summary>True when at least one filter field is active.</summary>
        public bool HasActiveFilters { get; }

        /// <summary>Number of active filter fields. Zero when no filters are applied.</summary>
        public int ActiveFilterCount { get; }

        public CandidateFilterViewModel(
            string roleFilter,
            string seniorityFilter,
            string salaryRangeFilter,
            string availabilityFilter,
            string interestFilter,
            string skillTagsFilter,
            string offerStatusFilter,
            string confidenceLevelFilter,
            bool hasActiveFilters,
            int activeFilterCount)
        {
            RoleFilter            = roleFilter;
            SeniorityFilter       = seniorityFilter;
            SalaryRangeFilter     = salaryRangeFilter;
            AvailabilityFilter    = availabilityFilter;
            InterestFilter        = interestFilter;
            SkillTagsFilter       = skillTagsFilter;
            OfferStatusFilter     = offerStatusFilter;
            ConfidenceLevelFilter = confidenceLevelFilter;
            HasActiveFilters      = hasActiveFilters;
            ActiveFilterCount     = activeFilterCount;
        }
    }
}
