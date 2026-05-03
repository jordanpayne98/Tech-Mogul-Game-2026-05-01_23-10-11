using System.Collections.Generic;

namespace Project.Core.SaveData.Contract
{
    /// <summary>
    /// Save data mirroring <c>ContractProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> fields serialized as total elapsed hours.
    /// </summary>
    public sealed class ContractSaveData
    {
        public string Id;
        public string ClientName;

        /// <summary>Serialized <c>ContractType</c> enum member name.</summary>
        public string Type;

        /// <summary>Serialized <c>ContractDifficulty</c> enum member name.</summary>
        public string Difficulty;

        /// <summary>Serialized <c>EmployeeRole</c> enum member names.</summary>
        public List<string> RequiredRoles;

        /// <summary>Serialized <c>SkillCategory</c> enum member names.</summary>
        public List<string> RequiredSkills;

        /// <summary>Serialized <c>GameDateTime PostedDate</c> as total elapsed hours.</summary>
        public int PostedDateTotalElapsedHours;

        /// <summary>Serialized <c>GameDateTime ExpiryDate</c> as total elapsed hours.</summary>
        public int ExpiryDateTotalElapsedHours;

        /// <summary>Serialized <c>GameDateTime Deadline</c> as total elapsed hours.</summary>
        public int DeadlineTotalElapsedHours;

        public long BasePaymentMinorUnits;
        public long ExcellentBonusMinorUnits;
        public long FailurePaymentMinorUnits;
        public int QualityTarget;
        public int MilestoneCount;
    }
}
