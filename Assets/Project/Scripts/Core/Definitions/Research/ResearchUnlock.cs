namespace Project.Core.Definitions.Research
{
    /// <summary>
    /// Describes a single capability granted when a research project completes.
    /// Value type embedded directly within <see cref="ResearchProjectDefinition.Unlocks"/>.
    /// The TargetId uses stable ID format (e.g. "product_type.smartphone").
    /// </summary>
    public struct ResearchUnlock
    {
        /// <summary>The category of unlock being granted.</summary>
        public ResearchUnlockType Type;

        /// <summary>
        /// Stable ID of the target being unlocked or improved.
        /// Must not be a display name — must survive renaming.
        /// </summary>
        public string TargetId;

        /// <summary>Human-readable description of what this unlock provides.</summary>
        public string Description;
    }
}
