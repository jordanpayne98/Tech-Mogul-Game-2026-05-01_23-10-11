namespace Project.Core.SaveData.Contract
{
    /// <summary>
    /// Save data mirroring <c>ContractRuntimeState</c>.
    /// Enum fields stored as member name strings. Nullable <c>GameDateTime</c> fields serialized as nullable int.
    /// </summary>
    public sealed class ContractStateSaveData
    {
        public string ContractId;

        /// <summary>Serialized <c>ContractStatus</c> enum member name.</summary>
        public string Status;

        /// <summary>Serialized <c>ContractOutcome</c> enum member name.</summary>
        public string Outcome;

        public string AssignedTeamId;
        public int ProgressPercent;
        public int MilestonesCompleted;
        public int QualityScore;

        /// <summary>Serialized nullable <c>GameDateTime AcceptedDate</c> as total elapsed hours. Null if not yet accepted.</summary>
        public int? AcceptedDateTotalElapsedHours;

        /// <summary>Serialized nullable <c>GameDateTime CompletedDate</c> as total elapsed hours. Null if not yet completed.</summary>
        public int? CompletedDateTotalElapsedHours;

        public long PaymentDueMinorUnits;
    }
}
