namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure display-data class for the reputation/status card on the Company screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyStatusCardViewModel
    {
        /// <summary>Short summary label for overall reputation, e.g. "Emerging Player".</summary>
        public string ReputationSummary { get; }

        /// <summary>Company stage label, e.g. "Seed Stage", "Series A".</summary>
        public string CompanyStage { get; }

        /// <summary>Company health display label, e.g. "Stable", "Under Pressure".</summary>
        public string HealthState { get; }

        /// <summary>
        /// Semantic state string used by the View to apply USS modifier classes.
        /// Allowed values: "normal", "warning", "danger", "success".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>Formatted count of major milestones reached, e.g. "3".</summary>
        public string MajorMilestonesCount { get; }

        /// <summary>Formatted count of active products, e.g. "2".</summary>
        public string ActiveProductCount { get; }

        /// <summary>Formatted count of current employees, e.g. "12".</summary>
        public string EmployeeCount { get; }

        /// <summary>Route ID for the products drill-down screen.</summary>
        public string ProductsDrillDownRouteId { get; }

        /// <summary>Route ID for the employees drill-down screen.</summary>
        public string EmployeesDrillDownRouteId { get; }

        public CompanyStatusCardViewModel(
            string reputationSummary,
            string companyStage,
            string healthState,
            string semanticState,
            string majorMilestonesCount,
            string activeProductCount,
            string employeeCount,
            string productsDrillDownRouteId,
            string employeesDrillDownRouteId)
        {
            ReputationSummary        = reputationSummary;
            CompanyStage             = companyStage;
            HealthState              = healthState;
            SemanticState            = semanticState;
            MajorMilestonesCount     = majorMilestonesCount;
            ActiveProductCount       = activeProductCount;
            EmployeeCount            = employeeCount;
            ProductsDrillDownRouteId = productsDrillDownRouteId;
            EmployeesDrillDownRouteId = employeesDrillDownRouteId;
        }
    }
}
