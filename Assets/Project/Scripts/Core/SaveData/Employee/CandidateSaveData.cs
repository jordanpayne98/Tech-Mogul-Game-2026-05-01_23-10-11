using System.Collections.Generic;

namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>CandidateProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> fields serialized as total elapsed hours.
    /// Dictionary keys use enum member name strings.
    /// </summary>
    public sealed class CandidateSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>EmployeeRole</c> enum member name.</summary>
        public string Role;

        /// <summary>Serialized <c>Seniority</c> enum member name.</summary>
        public string Seniority;

        public long SalaryExpectationMinorUnits;

        /// <summary>Key = <c>SkillCategory</c> enum member name. Value = visible skill score.</summary>
        public Dictionary<string, int> VisibleSkills;

        public int CurrentAbilityEstimate;
        public int PotentialMin;
        public int PotentialMax;

        /// <summary>Serialized <c>EmployeeTrait</c> enum member names.</summary>
        public List<string> Traits;

        /// <summary>Serialized <c>WorkPreference</c> enum member name.</summary>
        public string WorkPreference;

        /// <summary>Serialized <c>GameDateTime AvailabilityDate</c> as total elapsed hours.</summary>
        public int AvailabilityDateTotalElapsedHours;

        public int ConfidenceLevel;
    }
}
