using System.Collections.Generic;

namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>EmployeeProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime HireDate</c> serialized as total elapsed hours.
    /// </summary>
    public sealed class EmployeeSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>EmployeeTrait</c> enum member names.</summary>
        public List<string> Traits;

        /// <summary>Serialized <c>WorkPreference</c> enum member name.</summary>
        public string WorkPreference;

        /// <summary>Serialized <c>GameDateTime HireDate</c> as total elapsed hours.</summary>
        public int HireDateTotalElapsedHours;

        public int PotentialMin;
        public int PotentialMax;
    }
}
