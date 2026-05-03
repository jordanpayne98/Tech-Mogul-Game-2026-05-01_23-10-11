namespace Project.Core.Events.Team
{
    /// <summary>
    /// Published after a team is successfully created.
    /// Consumers can use TeamId to look up the TeamProfile and TeamRuntimeState.
    /// </summary>
    public sealed class TeamCreatedEvent
    {
        public string TeamId { get; }

        public TeamCreatedEvent(string teamId)
        {
            TeamId = teamId;
        }
    }
}
