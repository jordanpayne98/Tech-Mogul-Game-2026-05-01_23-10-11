namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure display-data class for a single candidate row in the Recruitment Hub table.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "shortlisted", "offer_sent", "accepted", "rejected".
    /// Candidate information uncertainty: VisibleSkills and PotentialEstimate may contain "???" for unrevealed data
    /// per Phase 5D Section 14 lock. The ViewModel must not expose hidden candidate data as certain.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CandidateRowViewModel
    {
        /// <summary>Stable candidate ID, e.g. "candidate.0001".</summary>
        public string Id { get; }

        /// <summary>Display name of the candidate, e.g. "Alex Mercer".</summary>
        public string Name { get; }

        /// <summary>Target role label, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Seniority level label, e.g. "Senior".</summary>
        public string Seniority { get; }

        /// <summary>Pre-formatted salary expectation, e.g. "$120,000 / yr".</summary>
        public string SalaryExpectation { get; }

        /// <summary>
        /// Comma-separated visible skill tags, e.g. "Python, React".
        /// May contain "???" chips for unrevealed skill slots.
        /// </summary>
        public string VisibleSkills { get; }

        /// <summary>
        /// Pre-formatted potential estimate label, e.g. "High" or "???".
        /// "???" indicates unrevealed potential data per Section 14 lock.
        /// </summary>
        public string PotentialEstimate { get; }

        /// <summary>Availability label, e.g. "Immediately" or "2 weeks".</summary>
        public string Availability { get; }

        /// <summary>Interest level label, e.g. "High", "Medium", "Low".</summary>
        public string Interest { get; }

        /// <summary>Offer status label, e.g. "No Offer", "Offer Sent", "Accepted", "Rejected".</summary>
        public string OfferStatus { get; }

        /// <summary>Confidence level label, e.g. "High", "Medium", "Low".</summary>
        public string Confidence { get; }

        /// <summary>Semantic state string: "normal", "shortlisted", "offer_sent", "accepted", "rejected".</summary>
        public string SemanticState { get; }

        /// <summary>True when this candidate row is on the shortlist.</summary>
        public bool IsShortlisted { get; }

        /// <summary>True when this row responds to click/tap for detail expansion.</summary>
        public bool IsClickable { get; }

        public CandidateRowViewModel(
            string id,
            string name,
            string role,
            string seniority,
            string salaryExpectation,
            string visibleSkills,
            string potentialEstimate,
            string availability,
            string interest,
            string offerStatus,
            string confidence,
            string semanticState,
            bool isShortlisted,
            bool isClickable)
        {
            Id                = id;
            Name              = name;
            Role              = role;
            Seniority         = seniority;
            SalaryExpectation = salaryExpectation;
            VisibleSkills     = visibleSkills;
            PotentialEstimate = potentialEstimate;
            Availability      = availability;
            Interest          = interest;
            OfferStatus       = offerStatus;
            Confidence        = confidence;
            SemanticState     = semanticState;
            IsShortlisted     = isShortlisted;
            IsClickable       = isClickable;
        }
    }
}
