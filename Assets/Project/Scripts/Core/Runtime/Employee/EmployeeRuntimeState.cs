using System.Collections.Generic;
using Project.Core.Definitions.Employee;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Mutable runtime state for an active employee.
    /// Linked to the corresponding <see cref="EmployeeProfile"/> via <see cref="EmployeeId"/>.
    /// All values here change during gameplay and must be persisted separately from the profile.
    /// </summary>
    public sealed class EmployeeRuntimeState
    {
        /// <summary>Stable ID matching the linked <see cref="EmployeeProfile.Id"/>.</summary>
        public string EmployeeId;

        /// <summary>The employee's current functional role (may change over time).</summary>
        public EmployeeRole Role;

        /// <summary>The employee's current seniority level (may be promoted).</summary>
        public Seniority Seniority;

        /// <summary>Current salary in minor currency units (e.g. cents).</summary>
        public long SalaryMinorUnits;

        /// <summary>
        /// Current skill scores per category (0–100 per skill).
        /// Skills can grow or decline during gameplay.
        /// </summary>
        public Dictionary<SkillCategory, int> Skills;

        /// <summary>Current overall ability score (0–100). Derived from skills and experience.</summary>
        public int CurrentAbility;

        /// <summary>Current morale level (0–100). Affects productivity and retention.</summary>
        public int Morale;

        /// <summary>Current burnout risk (0–100). High values increase resignation probability.</summary>
        public int BurnoutRisk;

        /// <summary>Current loyalty level (0–100). Affects offer resistance and retention.</summary>
        public int Loyalty;

        /// <summary>Current ambition level (0–100). Affects promotion expectations and departure risk.</summary>
        public int Ambition;

        /// <summary>
        /// Stable ID of the team this employee is currently assigned to.
        /// Null if the employee is unassigned.
        /// </summary>
        public string CurrentTeamId;

        /// <summary>Current employment status in the company.</summary>
        public EmploymentStatus Status;
    }
}
