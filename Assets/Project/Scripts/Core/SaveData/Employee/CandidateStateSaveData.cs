namespace Project.Core.SaveData.Employee
{
    /// <summary>
    /// Save data mirroring <c>CandidateRuntimeState</c>.
    /// </summary>
    public sealed class CandidateStateSaveData
    {
        public string CandidateId;

        /// <summary>Serialized <c>OfferStatus</c> enum member name.</summary>
        public string OfferStatus;

        public int InterestLevel;
    }
}
