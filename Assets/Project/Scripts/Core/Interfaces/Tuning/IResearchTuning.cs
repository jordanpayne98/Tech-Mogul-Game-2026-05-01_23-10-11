namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Tuning interface for the research system.
    /// Covers abstract work unit scaling and expertise/tooling multipliers
    /// that reduce the total work required to complete a research project.
    /// Implemented by TuningConfig in Infrastructure.
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public interface IResearchTuning
    {
        /// <summary>
        /// Abstract work units required per estimated day of research.
        /// Total required = EstimatedDurationDays * BaseResearchWorkUnitsPerDay,
        /// then divided by the combined expertise/tooling multiplier.
        /// Prototype: 24.
        /// </summary>
        int BaseResearchWorkUnitsPerDay { get; }

        /// <summary>
        /// Reduces required total work when the lead researcher has high expertise.
        /// Values greater than 1.0 speed up research; 1.0 means no effect.
        /// Applied as a divisor in the required work formula.
        /// Prototype: 1.2f.
        /// </summary>
        float LeadExpertiseMultiplier { get; }

        /// <summary>
        /// Reduces required total work when tooling is advanced.
        /// Values greater than 1.0 speed up research; 1.0 means no effect.
        /// Applied as a divisor together with LeadExpertiseMultiplier.
        /// Prototype: 1.0f.
        /// </summary>
        float ToolingMultiplier { get; }
    }
}
