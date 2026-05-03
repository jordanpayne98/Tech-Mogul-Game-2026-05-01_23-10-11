using System.Collections.Generic;

namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>EmployeeRuntimeState</c>.
    /// Dictionary keys use enum member name strings to avoid non-string key serialization issues.
    /// </summary>
    public sealed class EmployeeStateSaveData
    {
        public string EmployeeId;

        /// <summary>Serialized <c>EmployeeRole</c> enum member name.</summary>
        public string Role;

        /// <summary>Serialized <c>Seniority</c> enum member name.</summary>
        public string Seniority;

        public long SalaryMinorUnits;

        /// <summary>Key = <c>SkillCategory</c> enum member name. Value = skill score (0–100).</summary>
        public Dictionary<string, int> Skills;

        public int CurrentAbility;
        public int Morale;
        public int BurnoutRisk;
        public int Loyalty;
        public int Ambition;
        public string CurrentTeamId;

        /// <summary>Serialized <c>EmploymentStatus</c> enum member name.</summary>
        public string Status;
    }
}
