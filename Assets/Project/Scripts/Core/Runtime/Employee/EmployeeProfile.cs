using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Immutable per-save data describing a hired employee's identity and static attributes.
    /// Mutable runtime values (salary, morale, skills, etc.) are stored in <see cref="EmployeeRuntimeState"/>.
    /// </summary>
    public sealed class EmployeeProfile
    {
        /// <summary>Stable unique identifier for this employee record.</summary>
        public string Id;

        /// <summary>Display name of the employee.</summary>
        public string Name;

        /// <summary>Personality and background traits affecting gameplay behaviour.</summary>
        public List<EmployeeTrait> Traits;

        /// <summary>The employee's preferred working arrangement.</summary>
        public WorkPreference WorkPreference;

        /// <summary>The in-game date this employee was hired.</summary>
        public GameDateTime HireDate;

        /// <summary>Lower bound of the employee's hidden potential range (0–100).</summary>
        public int PotentialMin;

        /// <summary>Upper bound of the employee's hidden potential range (0–100).</summary>
        public int PotentialMax;
    }
}
