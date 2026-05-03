namespace Project.Core.SaveData.Research
{
    /// <summary>
    /// Save data mirroring <c>ResearchProjectRuntimeState</c>.
    /// Nullable <c>GameDateTime</c> fields serialized as nullable int.
    /// Enum fields stored as member name strings.
    /// </summary>
    public sealed class ResearchProjectSaveData
    {
        public string ProjectId;

        /// <summary>Serialized <c>ResearchProjectStatus</c> enum member name.</summary>
        public string Status;

        public int ProgressPercent;
        public string AssignedTeamId;

        /// <summary>Serialized nullable <c>GameDateTime StartDate</c> as total elapsed hours. Null if not yet started.</summary>
        public int? StartDateTotalElapsedHours;

        /// <summary>Serialized nullable <c>GameDateTime CompletedDate</c> as total elapsed hours. Null if not yet completed.</summary>
        public int? CompletedDateTotalElapsedHours;

        /// <summary>Serialized nullable <c>GameDateTime EstimatedCompletionDate</c> as total elapsed hours. Null if not yet estimated.</summary>
        public int? EstimatedCompletionDateTotalElapsedHours;
    }
}
