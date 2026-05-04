namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Display data for the currently assigned research panel within the Research screen.
    /// Immutable after construction. No Unity dependencies.
    /// Shown in the assigned research section regardless of active track tab.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class AssignedResearchViewModel
    {
        /// <summary>True when a research project is currently assigned and in progress.</summary>
        public bool HasAssignedResearch { get; }

        /// <summary>Display name of the currently assigned research project. Empty when HasAssignedResearch is false.</summary>
        public string ProjectName { get; }

        /// <summary>Display name of the team assigned to this research. Empty when no team is assigned.</summary>
        public string AssignedTeam { get; }

        /// <summary>Formatted progress value, e.g. "62%" or "Unstarted". Empty when HasAssignedResearch is false.</summary>
        public string Progress { get; }

        /// <summary>Formatted completion estimate, e.g. "Week 14" or "Unassigned". Empty when HasAssignedResearch is false.</summary>
        public string CompletionEstimate { get; }

        /// <summary>
        /// Semantic visual state for the assigned research panel.
        /// Drives USS class toggling — never raw colour values.
        /// Examples: "in-progress", "no-assignment", "nearing-completion".
        /// </summary>
        public string SemanticState { get; }

        public AssignedResearchViewModel(
            bool hasAssignedResearch,
            string projectName,
            string assignedTeam,
            string progress,
            string completionEstimate,
            string semanticState)
        {
            HasAssignedResearch = hasAssignedResearch;
            ProjectName         = projectName;
            AssignedTeam        = assignedTeam;
            Progress            = progress;
            CompletionEstimate  = completionEstimate;
            SemanticState       = semanticState;
        }
    }
}
