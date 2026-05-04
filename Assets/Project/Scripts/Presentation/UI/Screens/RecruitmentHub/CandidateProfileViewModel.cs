using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure display-data class for the candidate detail modal in the Recruitment Hub.
    /// Immutable after construction. No Unity dependencies.
    /// Candidate information uncertainty is modelled via KnownSkills vs HiddenSkillSlots:
    /// HiddenSkillSlots entries display as "???" chips — the ViewModel must not expose hidden
    /// candidate data as certain (per Phase 5D Section 14 lock).
    /// SemanticState drives USS state classes in the View: "normal", "shortlisted", "offer_sent", "accepted", "rejected".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CandidateProfileViewModel
    {
        /// <summary>Stable candidate ID, e.g. "candidate.0001".</summary>
        public string CandidateId { get; }

        /// <summary>Display name of the candidate, e.g. "Alex Mercer".</summary>
        public string Name { get; }

        /// <summary>Target role label, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Seniority level label, e.g. "Senior".</summary>
        public string Seniority { get; }

        /// <summary>Pre-formatted salary expectation, e.g. "$120,000 / yr".</summary>
        public string SalaryExpectation { get; }

        /// <summary>Availability label, e.g. "Immediately" or "2 weeks".</summary>
        public string Availability { get; }

        /// <summary>Interest level label, e.g. "High", "Medium", "Low".</summary>
        public string Interest { get; }

        /// <summary>Confidence level label, e.g. "High", "Medium", "Low".</summary>
        public string Confidence { get; }

        /// <summary>
        /// Revealed skill tags. Each entry is a display-ready skill name, e.g. "Python".
        /// Only skills confirmed through scouting or interview are included here.
        /// </summary>
        public IReadOnlyList<string> KnownSkills { get; }

        /// <summary>
        /// Placeholder entries for skill slots that have not been revealed.
        /// Each entry displays as a "???" chip in the View per Section 14 lock.
        /// The count of entries reflects the number of hidden slots only.
        /// </summary>
        public IReadOnlyList<string> HiddenSkillSlots { get; }

        /// <summary>Interview status label, e.g. "Not Interviewed", "Scheduled", "Completed".</summary>
        public string InterviewStatus { get; }

        /// <summary>
        /// Trait confidence display labels. Each entry is a formatted trait + confidence pair,
        /// e.g. "Collaborative — High Confidence". Unknown traits use "??? — Unknown".
        /// </summary>
        public IReadOnlyList<string> TraitsConfidence { get; }

        /// <summary>
        /// Offer history display labels. Each entry is a formatted offer event summary,
        /// e.g. "Offer Sent — $115,000 — Rejected". Empty list if no offers have been made.
        /// </summary>
        public IReadOnlyList<string> OfferHistory { get; }

        /// <summary>Semantic state string: "normal", "shortlisted", "offer_sent", "accepted", "rejected".</summary>
        public string SemanticState { get; }

        /// <summary>
        /// True when an offer can be sent to this candidate.
        /// Always false in Phase 5 — offer submission is a Phase 6+ action.
        /// </summary>
        public bool CanSendOffer { get; }

        /// <summary>True when this candidate can be added to or removed from the shortlist.</summary>
        public bool CanShortlist { get; }

        public CandidateProfileViewModel(
            string candidateId,
            string name,
            string role,
            string seniority,
            string salaryExpectation,
            string availability,
            string interest,
            string confidence,
            IReadOnlyList<string> knownSkills,
            IReadOnlyList<string> hiddenSkillSlots,
            string interviewStatus,
            IReadOnlyList<string> traitsConfidence,
            IReadOnlyList<string> offerHistory,
            string semanticState,
            bool canSendOffer,
            bool canShortlist)
        {
            CandidateId       = candidateId;
            Name              = name;
            Role              = role;
            Seniority         = seniority;
            SalaryExpectation = salaryExpectation;
            Availability      = availability;
            Interest          = interest;
            Confidence        = confidence;
            KnownSkills       = knownSkills;
            HiddenSkillSlots  = hiddenSkillSlots;
            InterviewStatus   = interviewStatus;
            TraitsConfidence  = traitsConfidence;
            OfferHistory      = offerHistory;
            SemanticState     = semanticState;
            CanSendOffer      = canSendOffer;
            CanShortlist      = canShortlist;
        }
    }
}
