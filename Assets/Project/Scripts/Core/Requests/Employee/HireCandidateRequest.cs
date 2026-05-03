namespace Project.Core.Requests.Employee
{
    /// <summary>
    /// Request to extend a formal hire offer to a candidate at the specified salary.
    /// Team assignment is a separate action and is not part of this request.
    /// </summary>
    public sealed class HireCandidateRequest
    {
        /// <summary>Stable ID of the candidate to hire.</summary>
        public string CandidateId { get; }

        /// <summary>The salary being offered in minor currency units (e.g. cents).</summary>
        public long OfferedSalaryMinorUnits { get; }

        public HireCandidateRequest(string candidateId, long offeredSalaryMinorUnits)
        {
            CandidateId = candidateId;
            OfferedSalaryMinorUnits = offeredSalaryMinorUnits;
        }
    }
}
