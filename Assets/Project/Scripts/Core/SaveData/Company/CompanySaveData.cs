namespace Project.Core.SaveData.Company
{
    /// <summary>
    /// Save data mirroring <c>CompanyProfile</c>.
    /// All enum fields are stored as their member name strings for serializer compatibility.
    /// <c>GameDateTime</c> is serialized as <c>int FoundedDateTotalElapsedHours</c>.
    /// </summary>
    public sealed class CompanySaveData
    {
        public string Id;
        public string Name;
        public string FounderId;
        public string LogoIconId;
        public string BrandColourHex;

        /// <summary>Serialized <c>CompanyFocus</c> enum member name.</summary>
        public string Focus;

        public string Location;

        /// <summary>Serialized <c>GameDateTime FoundedDate</c> as total elapsed hours.</summary>
        public int FoundedDateTotalElapsedHours;
    }
}
