namespace Project.Core.Requests.Research
{
    /// <summary>
    /// Request to begin a research project and assign a team to it.
    /// Use <see cref="Project.Core.Results.Research.StartResearchResult"/> to inspect the outcome.
    /// </summary>
    public sealed class StartResearchRequest
    {
        /// <summary>Stable ID of the research project definition to start.</summary>
        public string ResearchProjectId { get; }

        /// <summary>Stable ID of the team being assigned to this research project.</summary>
        public string TeamId { get; }

        public StartResearchRequest(string researchProjectId, string teamId)
        {
            ResearchProjectId = researchProjectId;
            TeamId = teamId;
        }
    }
}
