namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>JobPostRuntimeState</c>.
    /// </summary>
    public sealed class JobPostStateSaveData
    {
        public string JobPostId;

        /// <summary>Serialized <c>JobPostStatus</c> enum member name.</summary>
        public string Status;
    }
}
