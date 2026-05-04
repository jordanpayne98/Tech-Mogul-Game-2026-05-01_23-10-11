namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure display-data class for the offer modal in the Recruitment Hub.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "submitted", "accepted", "rejected".
    /// IsSubmittable is always false in Phase 5 — offer submission is a Phase 6+ action.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class OfferViewModel
    {
        /// <summary>Stable candidate ID this offer targets, e.g. "candidate.0001".</summary>
        public string CandidateId { get; }

        /// <summary>Display name of the candidate, e.g. "Alex Mercer".</summary>
        public string CandidateName { get; }

        /// <summary>Role label for the offer position, e.g. "Software Engineer".</summary>
        public string Role { get; }

        /// <summary>Pre-formatted offered salary, e.g. "$115,000 / yr".</summary>
        public string OfferedSalary { get; }

        /// <summary>Display name of the target team, e.g. "Core Engine Team".</summary>
        public string TargetTeam { get; }

        /// <summary>Current offer status label, e.g. "Draft", "Sent", "Accepted", "Rejected".</summary>
        public string OfferStatus { get; }

        /// <summary>Semantic state string: "normal", "submitted", "accepted", "rejected".</summary>
        public string SemanticState { get; }

        /// <summary>
        /// True when the offer form has valid input and can be submitted.
        /// Always false in Phase 5 — offer submission is a Phase 6+ action.
        /// </summary>
        public bool IsSubmittable { get; }

        public OfferViewModel(
            string candidateId,
            string candidateName,
            string role,
            string offeredSalary,
            string targetTeam,
            string offerStatus,
            string semanticState,
            bool isSubmittable)
        {
            CandidateId   = candidateId;
            CandidateName = candidateName;
            Role          = role;
            OfferedSalary = offeredSalary;
            TargetTeam    = targetTeam;
            OfferStatus   = offerStatus;
            SemanticState = semanticState;
            IsSubmittable = isSubmittable;
        }
    }
}
