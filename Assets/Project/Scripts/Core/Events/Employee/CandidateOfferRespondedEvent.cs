using Project.Core.Definitions.Employee;

namespace Project.Core.Events.Employee
{
    /// <summary>
    /// Published after a candidate responds to a hire offer, whether accepted or rejected.
    /// </summary>
    public sealed class CandidateOfferRespondedEvent
    {
        /// <summary>Stable ID of the candidate who responded to the offer.</summary>
        public string CandidateId { get; }

        /// <summary>The candidate's response status.</summary>
        public OfferStatus Response { get; }

        public CandidateOfferRespondedEvent(string candidateId, OfferStatus response)
        {
            CandidateId = candidateId;
            Response    = response;
        }
    }
}
