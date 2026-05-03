namespace Project.Core.Events.Research
{
    /// <summary>
    /// Published by StartResearchUseCase when a research project is successfully started.
    /// Immutable snapshot — carries the project and team stable IDs.
    /// </summary>
    public sealed class ResearchStartedEvent
    {
        /// <summary>Stable ID of the research project that was started.</summary>
        public string ProjectId { get; }

        /// <summary>Stable ID of the team assigned to this project.</summary>
        public string TeamId { get; }

        public ResearchStartedEvent(string projectId, string teamId)
        {
            ProjectId = projectId;
            TeamId    = teamId;
        }
    }
}
