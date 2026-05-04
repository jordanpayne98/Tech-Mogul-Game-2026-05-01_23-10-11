using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Profile tab IDs used in the employee profile modal.
    /// </summary>
    public static class EmployeeProfileTabIds
    {
        public const string Overview     = "tab.overview";
        public const string Skills       = "tab.skills";
        public const string Performance  = "tab.performance";
        public const string Development  = "tab.development";
        public const string Compensation = "tab.compensation";
        public const string History      = "tab.history";
    }

    /// <summary>
    /// Pure display-data class for the employee profile detail modal.
    /// Immutable after construction. No Unity dependencies.
    /// Created by EmployeesController and passed to the profile modal View.
    /// SemanticState drives USS state classes in the View: "normal", "at-risk", "former".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeeProfileViewModel
    {
        /// <summary>Stable employee identifier, e.g. "employee.001".</summary>
        public string EmployeeId { get; }

        /// <summary>Display name, e.g. "Alex Rivera".</summary>
        public string Name { get; }

        /// <summary>Role display label, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Seniority display label, e.g. "Senior".</summary>
        public string Seniority { get; }

        /// <summary>Team display label, e.g. "Core Platform".</summary>
        public string Team { get; }

        /// <summary>Status display label, e.g. "Active", "On Leave", "Former".</summary>
        public string Status { get; }

        /// <summary>Pre-formatted salary string, e.g. "$95,000 / yr".</summary>
        public string Salary { get; }

        /// <summary>Pre-formatted morale value string, e.g. "72".</summary>
        public string Morale { get; }

        /// <summary>Pre-formatted burnout risk label, e.g. "Medium".</summary>
        public string BurnoutRisk { get; }

        /// <summary>Ordered list of stable tab IDs shown in the profile modal tab bar.</summary>
        public IReadOnlyList<string> ProfileTabs { get; }

        /// <summary>The currently active tab ID, e.g. "tab.overview".</summary>
        public string ActiveProfileTabId { get; }

        /// <summary>Skill bars displayed in the Skills tab.</summary>
        public IReadOnlyList<SkillBarViewModel> Skills { get; }

        /// <summary>Activity history rows displayed in the History tab.</summary>
        public IReadOnlyList<EmployeeActivityRowViewModel> ActivityHistory { get; }

        /// <summary>Semantic state string: "normal", "at-risk", or "former". Drives USS state class.</summary>
        public string SemanticState { get; }

        /// <summary>True when the profile allows inline editing actions (e.g. reassign, adjust salary).</summary>
        public bool IsEditable { get; }

        public EmployeeProfileViewModel(
            string employeeId,
            string name,
            string role,
            string seniority,
            string team,
            string status,
            string salary,
            string morale,
            string burnoutRisk,
            IReadOnlyList<string> profileTabs,
            string activeProfileTabId,
            IReadOnlyList<SkillBarViewModel> skills,
            IReadOnlyList<EmployeeActivityRowViewModel> activityHistory,
            string semanticState,
            bool isEditable)
        {
            EmployeeId       = employeeId;
            Name             = name;
            Role             = role;
            Seniority        = seniority;
            Team             = team;
            Status           = status;
            Salary           = salary;
            Morale           = morale;
            BurnoutRisk      = burnoutRisk;
            ProfileTabs      = profileTabs;
            ActiveProfileTabId = activeProfileTabId;
            Skills           = skills;
            ActivityHistory  = activityHistory;
            SemanticState    = semanticState;
            IsEditable       = isEditable;
        }
    }
}
