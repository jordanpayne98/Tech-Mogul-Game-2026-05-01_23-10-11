using Project.Core.Definitions.Employee;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Mutable runtime state for a recruitment candidate.
    /// Linked to the corresponding <see cref="CandidateProfile"/> via <see cref="CandidateId"/>.
    /// </summary>
    public sealed class CandidateRuntimeState
    {
        /// <summary>Stable ID matching the linked <see cref="CandidateProfile.Id"/>.</summary>
        public string CandidateId;

        /// <summary>The current status of any offer extended to this candidate.</summary>
        public OfferStatus OfferStatus;

        /// <summary>
        /// How interested this candidate is in joining the company (0–100).
        /// Influenced by company pitch, salary offer, and work preference match.
        /// </summary>
        public int InterestLevel;
    }
}
