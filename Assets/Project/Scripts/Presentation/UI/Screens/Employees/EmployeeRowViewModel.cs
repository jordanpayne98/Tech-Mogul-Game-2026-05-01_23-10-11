namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure display-data class for a single employee row in the Employees roster table.
    /// Immutable after construction. No Unity dependencies.
    /// All column values are pre-formatted display strings.
    /// SemanticState drives USS state classes in the View: "normal", "at-risk", "former".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeeRowViewModel
    {
        /// <summary>Stable employee identifier, e.g. "employee.001".</summary>
        public string Id { get; }

        /// <summary>Display name, e.g. "Alex Rivera".</summary>
        public string Name { get; }

        /// <summary>Role display label, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Seniority display label, e.g. "Senior".</summary>
        public string Seniority { get; }

        /// <summary>Team display label, e.g. "Core Platform".</summary>
        public string Team { get; }

        /// <summary>Current assignment display label, e.g. "Project Alpha". Empty string if unassigned.</summary>
        public string CurrentAssignment { get; }

        /// <summary>Pre-formatted salary string, e.g. "$95,000 / yr".</summary>
        public string Salary { get; }

        /// <summary>Pre-formatted morale value string, e.g. "72".</summary>
        public string Morale { get; }

        /// <summary>Pre-formatted burnout risk label, e.g. "Medium".</summary>
        public string BurnoutRisk { get; }

        /// <summary>Pre-formatted ability value string, e.g. "68".</summary>
        public string Ability { get; }

        /// <summary>Pre-formatted potential range string, e.g. "70–85".</summary>
        public string PotentialRange { get; }

        /// <summary>Status display label, e.g. "Active", "On Leave", "Former".</summary>
        public string Status { get; }

        /// <summary>Pre-formatted start date string, e.g. "Jan 2023".</summary>
        public string StartDate { get; }

        /// <summary>Semantic state string: "normal", "at-risk", or "former". Drives USS state class.</summary>
        public string SemanticState { get; }

        /// <summary>True when the row responds to click/tap to open the profile modal.</summary>
        public bool IsClickable { get; }

        /// <summary>Route ID used to open the employee profile modal, e.g. "modal.employee_profile".</summary>
        public string DrillDownRouteId { get; }

        public EmployeeRowViewModel(
            string id,
            string name,
            string role,
            string seniority,
            string team,
            string currentAssignment,
            string salary,
            string morale,
            string burnoutRisk,
            string ability,
            string potentialRange,
            string status,
            string startDate,
            string semanticState,
            bool isClickable,
            string drillDownRouteId)
        {
            Id                = id;
            Name              = name;
            Role              = role;
            Seniority         = seniority;
            Team              = team;
            CurrentAssignment = currentAssignment;
            Salary            = salary;
            Morale            = morale;
            BurnoutRisk       = burnoutRisk;
            Ability           = ability;
            PotentialRange    = potentialRange;
            Status            = status;
            StartDate         = startDate;
            SemanticState     = semanticState;
            IsClickable       = isClickable;
            DrillDownRouteId  = drillDownRouteId;
        }
    }
}
