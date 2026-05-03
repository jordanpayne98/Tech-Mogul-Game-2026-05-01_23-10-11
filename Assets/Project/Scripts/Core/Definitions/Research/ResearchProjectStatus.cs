namespace Project.Core.Definitions.Research
{
    /// <summary>
    /// Lifecycle status of a research project from the player's perspective.
    /// Tracked in <see cref="Project.Core.Runtime.Research.ResearchProjectRuntimeState"/>.
    /// </summary>
    public enum ResearchProjectStatus
    {
        /// <summary>Prerequisite research has not been completed; this project is not yet visible or actionable.</summary>
        Locked,

        /// <summary>Prerequisites met; the project can be started by the player.</summary>
        Available,

        /// <summary>The project is currently being worked on by an assigned team.</summary>
        InProgress,

        /// <summary>The project has finished and its unlocks have been granted.</summary>
        Completed,

        /// <summary>The project was superseded by another technology and is no longer relevant.</summary>
        Obsolete
    }
}
