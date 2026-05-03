namespace Project.Core.SaveData.Team
{
    /// <summary>
    /// Save data mirroring <c>AssignmentRuntimeState</c>.
    /// Enum fields stored as member name strings. Nullable <c>GameDateTime</c> serialized as nullable int.
    /// </summary>
    public sealed class AssignmentSaveData
    {
        public string Id;

        /// <summary>Serialized <c>AssignmentType</c> enum member name.</summary>
        public string Type;

        /// <summary>Serialized <c>AssignmentTargetType</c> enum member name.</summary>
        public string TargetType;

        public string TargetId;
        public string TeamId;
        public int ProgressPercent;
        public int RawProgressPoints;

        /// <summary>Serialized <c>GameDateTime StartDate</c> as total elapsed hours.</summary>
        public int StartDateTotalElapsedHours;

        /// <summary>Serialized nullable <c>GameDateTime EstimatedCompletionDate</c> as total elapsed hours. Null if not yet estimated.</summary>
        public int? EstimatedCompletionDateTotalElapsedHours;
    }
}
