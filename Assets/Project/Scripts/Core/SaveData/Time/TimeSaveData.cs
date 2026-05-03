namespace Project.Core.SaveData.Time
{
    /// <summary>
    /// Save data mirroring <c>TimeRuntimeState</c>.
    /// <c>GameDateTime</c> fields are serialized as total elapsed hours.
    /// All enum fields are stored as their member name strings for serializer compatibility.
    /// Reconstruct <c>GameDateTime</c> via <c>GameDateTime.FromTotalHours(value)</c>.
    /// </summary>
    public sealed class TimeSaveData
    {
        /// <summary>Serialized <c>CurrentDate</c> as total elapsed hours.</summary>
        public int CurrentDateTotalElapsedHours;

        public int TotalElapsedHours;

        /// <summary>Serialized <c>TimeSpeed</c> enum member name.</summary>
        public string Speed;

        /// <summary>Serialized <c>TimeAdvanceMode</c> enum member name.</summary>
        public string AdvanceMode;

        /// <summary>Serialized <c>InterruptionFilter</c> enum member name.</summary>
        public string Filter;

        public bool IsAdvancing;
    }
}
