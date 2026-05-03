namespace Project.Core.Events.Team
{
    /// <summary>
    /// Published after a team is successfully assigned to work on a target entity.
    /// Consumers can use TeamId and AssignmentId to look up the AssignmentRuntimeState.
    /// </summary>
    public sealed class TeamAssignedEvent
    {
        public string TeamId       { get; }
        public string AssignmentId { get; }

        public TeamAssignedEvent(string teamId, string assignmentId)
        {
            TeamId       = teamId;
            AssignmentId = assignmentId;
        }
    }
}
