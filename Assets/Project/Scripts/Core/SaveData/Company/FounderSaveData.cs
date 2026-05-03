namespace Project.Core.SaveData.Company
{
    /// <summary>
    /// Save data mirroring <c>FounderProfile</c>.
    /// All enum fields are stored as their member name strings for serializer compatibility.
    /// </summary>
    public sealed class FounderSaveData
    {
        public string Id;
        public string Name;

        /// <summary>Serialized <c>FounderBackground</c> enum member name.</summary>
        public string Background;
    }
}
