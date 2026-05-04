namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Display data for one research project card or row in the Research screen.
    /// Immutable after construction. No Unity dependencies.
    /// Used for both available and locked project lists.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchProjectRowViewModel
    {
        /// <summary>Stable ID for this research project, e.g. "research.project.compiler_optimisation".</summary>
        public string Id { get; }

        /// <summary>Display name of the research project.</summary>
        public string Name { get; }

        /// <summary>Display name of the research track this project belongs to.</summary>
        public string Track { get; }

        /// <summary>Required skill or discipline for this project, e.g. "Compiler Engineering".</summary>
        public string RequiredSkill { get; }

        /// <summary>Formatted duration of the research project, e.g. "8 weeks".</summary>
        public string Duration { get; }

        /// <summary>Formatted cost of the research project, e.g. "$120,000".</summary>
        public string Cost { get; }

        /// <summary>Short summary of what this project unlocks, e.g. "Advanced Compiler Toolkit".</summary>
        public string Unlocks { get; }

        /// <summary>Risk level label, e.g. "Low", "Medium", "High".</summary>
        public string RiskLevel { get; }

        /// <summary>Obsolescence risk label, e.g. "Low", "Medium", "High".</summary>
        public string ObsolescenceRisk { get; }

        /// <summary>Formatted prerequisites for this project. Empty string if none. Shown for locked projects.</summary>
        public string Prerequisites { get; }

        /// <summary>Display name of the team assigned to this project, or empty if unassigned.</summary>
        public string AssignedTeam { get; }

        /// <summary>Formatted completion estimate, e.g. "Week 14" or "Unassigned".</summary>
        public string CompletionEstimate { get; }

        /// <summary>Comma-separated display names of related products, or empty if none.</summary>
        public string RelatedProducts { get; }

        /// <summary>Status label for this project, e.g. "Available", "In Progress", "Completed", "Locked".</summary>
        public string Status { get; }

        /// <summary>
        /// Semantic visual state for this row.
        /// Drives USS class toggling — never raw colour values.
        /// Examples: "available", "in-progress", "completed", "locked", "has-obsolescence-warning".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when this row can be clicked to open the research detail modal.</summary>
        public bool IsClickable { get; }

        /// <summary>True when this project is locked due to unmet prerequisites.</summary>
        public bool IsLocked { get; }

        public ResearchProjectRowViewModel(
            string id,
            string name,
            string track,
            string requiredSkill,
            string duration,
            string cost,
            string unlocks,
            string riskLevel,
            string obsolescenceRisk,
            string prerequisites,
            string assignedTeam,
            string completionEstimate,
            string relatedProducts,
            string status,
            string semanticState,
            bool isClickable,
            bool isLocked)
        {
            Id                  = id;
            Name                = name;
            Track               = track;
            RequiredSkill       = requiredSkill;
            Duration            = duration;
            Cost                = cost;
            Unlocks             = unlocks;
            RiskLevel           = riskLevel;
            ObsolescenceRisk    = obsolescenceRisk;
            Prerequisites       = prerequisites;
            AssignedTeam        = assignedTeam;
            CompletionEstimate  = completionEstimate;
            RelatedProducts     = relatedProducts;
            Status              = status;
            SemanticState       = semanticState;
            IsClickable         = isClickable;
            IsLocked            = isLocked;
        }
    }
}
