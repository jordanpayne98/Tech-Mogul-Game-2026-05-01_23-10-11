namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Display data for one unlock entry within a research project detail modal.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchUnlockViewModel
    {
        /// <summary>Display name of the unlock, e.g. "Advanced Compiler Toolkit".</summary>
        public string Name { get; }

        /// <summary>Short description of what the unlock enables.</summary>
        public string Description { get; }

        /// <summary>Category label grouping this unlock, e.g. "Tool", "Capability", "Feature".</summary>
        public string Category { get; }

        /// <summary>
        /// Semantic visual state for this unlock entry.
        /// Drives USS class toggling — never raw colour values.
        /// Examples: "default", "highlight", "preview".
        /// </summary>
        public string SemanticState { get; }

        public ResearchUnlockViewModel(
            string name,
            string description,
            string category,
            string semanticState)
        {
            Name          = name;
            Description   = description;
            Category      = category;
            SemanticState = semanticState;
        }
    }
}
