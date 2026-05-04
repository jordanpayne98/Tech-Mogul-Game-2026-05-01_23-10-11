using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Display data for the research project detail modal (modal.research_detail).
    /// Immutable after construction. No Unity dependencies.
    /// Created by ResearchController and passed to ResearchView when a project row is clicked.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// Locked projects show prerequisites, not hidden formula values (per Section 14 lock).
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchDetailViewModel
    {
        /// <summary>Stable ID of the research project being shown.</summary>
        public string ProjectId { get; }

        /// <summary>Display name of the research project.</summary>
        public string Name { get; }

        /// <summary>Display name of the research track this project belongs to.</summary>
        public string Track { get; }

        /// <summary>Purpose description explaining why this research matters.</summary>
        public string Purpose { get; }

        /// <summary>Formatted requirements summary (skill, team size, prerequisites).</summary>
        public string Requirements { get; }

        /// <summary>
        /// Expected benefits summary shown as display-ready text.
        /// Must not expose exact hidden formula values per Section 14 lock.
        /// </summary>
        public string ExpectedBenefits { get; }

        /// <summary>List of unlock entries this project provides upon completion.</summary>
        public IReadOnlyList<ResearchUnlockViewModel> Unlocks { get; }

        /// <summary>Display names of products that are related to or benefit from this research.</summary>
        public IReadOnlyList<string> RelatedProducts { get; }

        /// <summary>Display names of teams eligible to be assigned to this project. [Placeholder] — Phase 6 wiring.</summary>
        public IReadOnlyList<string> AssignedTeamOptions { get; }

        /// <summary>Warning messages relevant to this project, e.g. obsolescence risk notices.</summary>
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>
        /// Semantic visual state for the detail modal.
        /// Drives USS class toggling — never raw colour values.
        /// Examples: "available", "in-progress", "completed", "locked", "has-warning".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>
        /// False in Phase 5 — team assignment is a Phase 6+ action.
        /// [Placeholder]
        /// </summary>
        public bool CanAssignTeam { get; }

        public ResearchDetailViewModel(
            string projectId,
            string name,
            string track,
            string purpose,
            string requirements,
            string expectedBenefits,
            IReadOnlyList<ResearchUnlockViewModel> unlocks,
            IReadOnlyList<string> relatedProducts,
            IReadOnlyList<string> assignedTeamOptions,
            IReadOnlyList<string> warnings,
            string semanticState,
            bool canAssignTeam)
        {
            ProjectId           = projectId;
            Name                = name;
            Track               = track;
            Purpose             = purpose;
            Requirements        = requirements;
            ExpectedBenefits    = expectedBenefits;
            Unlocks             = unlocks;
            RelatedProducts     = relatedProducts;
            AssignedTeamOptions = assignedTeamOptions;
            Warnings            = warnings;
            SemanticState       = semanticState;
            CanAssignTeam       = canAssignTeam;
        }
    }
}
