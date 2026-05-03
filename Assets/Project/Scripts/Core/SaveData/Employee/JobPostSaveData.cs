using System.Collections.Generic;

namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>JobPostProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> fields serialized as total elapsed hours.
    /// </summary>
    public sealed class JobPostSaveData
    {
        public string Id;

        /// <summary>Serialized <c>EmployeeRole</c> enum member name.</summary>
        public string Role;

        /// <summary>Serialized <c>Seniority</c> enum member name.</summary>
        public string Seniority;

        public long SalaryRangeMinMinorUnits;
        public long SalaryRangeMaxMinorUnits;

        /// <summary>Serialized <c>SkillCategory</c> enum member names.</summary>
        public List<string> RequiredSkills;

        /// <summary>Serialized <c>SkillCategory</c> enum member names.</summary>
        public List<string> PreferredSkills;

        /// <summary>Serialized <c>WorkPreference</c> enum member name.</summary>
        public string WorkPreference;

        public string CompanyPitch;
        public long HiringBudgetMinorUnits;

        /// <summary>Serialized <c>GameDateTime CreatedDate</c> as total elapsed hours.</summary>
        public int CreatedDateTotalElapsedHours;
    }
}
