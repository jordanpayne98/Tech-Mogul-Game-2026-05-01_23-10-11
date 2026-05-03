namespace Project.Core.SaveData.Team
{
    /// <summary>
    /// Save data mirroring <c>TeamProfile</c>.
    /// Enum fields stored as member name strings. <c>GameDateTime</c> serialized as total elapsed hours.
    /// </summary>
    public sealed class TeamSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>TeamType</c> enum member name.</summary>
        public string Type;

        /// <summary>Serialized <c>GameDateTime CreatedDate</c> as total elapsed hours.</summary>
        public int CreatedDateTotalElapsedHours;
    }
}
