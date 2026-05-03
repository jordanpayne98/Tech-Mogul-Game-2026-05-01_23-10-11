using System.Collections.Generic;
using Project.Core.Definitions.Employee;

namespace Project.Core.Requests.Employee
{
    /// <summary>
    /// Request to create a new job post for a specific role and seniority.
    /// Job posts are stored data only. No active candidate filtering occurs in MVP.
    /// </summary>
    public sealed class PostJobRequest
    {
        /// <summary>The role being hired for.</summary>
        public EmployeeRole Role { get; }

        /// <summary>The seniority level being targeted for this role.</summary>
        public Seniority Seniority { get; }

        /// <summary>Lower bound of the advertised salary range in minor currency units.</summary>
        public long SalaryRangeMinMinorUnits { get; }

        /// <summary>Upper bound of the advertised salary range in minor currency units.</summary>
        public long SalaryRangeMaxMinorUnits { get; }

        /// <summary>Skill categories that a candidate must have to be considered for this post.</summary>
        public List<SkillCategory> RequiredSkills { get; }

        /// <summary>Skill categories that are desirable but not mandatory for this post.</summary>
        public List<SkillCategory> PreferredSkills { get; }

        /// <summary>The working arrangement this post offers.</summary>
        public WorkPreference WorkPreference { get; }

        /// <summary>Player-authored pitch text describing the company or role to attract candidates.</summary>
        public string CompanyPitch { get; }

        /// <summary>Total budget allocated for this hire in minor currency units.</summary>
        public long HiringBudgetMinorUnits { get; }

        public PostJobRequest(
            EmployeeRole role,
            Seniority seniority,
            long salaryRangeMinMinorUnits,
            long salaryRangeMaxMinorUnits,
            List<SkillCategory> requiredSkills,
            List<SkillCategory> preferredSkills,
            WorkPreference workPreference,
            string companyPitch,
            long hiringBudgetMinorUnits)
        {
            Role                       = role;
            Seniority                  = seniority;
            SalaryRangeMinMinorUnits   = salaryRangeMinMinorUnits;
            SalaryRangeMaxMinorUnits   = salaryRangeMaxMinorUnits;
            RequiredSkills             = requiredSkills;
            PreferredSkills            = preferredSkills;
            WorkPreference             = workPreference;
            CompanyPitch               = companyPitch;
            HiringBudgetMinorUnits     = hiringBudgetMinorUnits;
        }
    }
}
