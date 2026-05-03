using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Per-save data describing an open job post created by the player.
    /// Mutable lifecycle status is stored in <see cref="JobPostRuntimeState"/>.
    /// </summary>
    public sealed class JobPostProfile
    {
        /// <summary>Stable unique identifier for this job post record.</summary>
        public string Id;

        /// <summary>The role being hired for.</summary>
        public EmployeeRole Role;

        /// <summary>The seniority level being targeted for this role.</summary>
        public Seniority Seniority;

        /// <summary>Lower bound of the advertised salary range in minor currency units (e.g. cents).</summary>
        public long SalaryRangeMinMinorUnits;

        /// <summary>Upper bound of the advertised salary range in minor currency units (e.g. cents).</summary>
        public long SalaryRangeMaxMinorUnits;

        /// <summary>Skill categories that a candidate must have to be considered for this post.</summary>
        public List<SkillCategory> RequiredSkills;

        /// <summary>Skill categories that are desirable but not mandatory for this post.</summary>
        public List<SkillCategory> PreferredSkills;

        /// <summary>The working arrangement this post offers.</summary>
        public WorkPreference WorkPreference;

        /// <summary>Player-authored pitch text describing the company or role to attract candidates.</summary>
        public string CompanyPitch;

        /// <summary>Total budget allocated for this hire in minor currency units (e.g. cents).</summary>
        public long HiringBudgetMinorUnits;

        /// <summary>The in-game date this job post was created.</summary>
        public GameDateTime CreatedDate;
    }
}
