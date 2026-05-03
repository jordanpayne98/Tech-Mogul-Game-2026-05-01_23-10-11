namespace Project.Core.Events.Team
{
    /// <summary>
    /// Published after a team is unassigned from its current work.
    /// PreviousAssignmentId references the AssignmentRuntimeState that was cleared.
    /// The AssignmentRuntimeState is not deleted — it remains for historical reference.
    /// </summary>
    public sealed class TeamUnassignedEvent
    {
        public string TeamId               { get; }
        public string PreviousAssignmentId { get; }

        public TeamUnassignedEvent(string teamId, string previousAssignmentId)
        {
            TeamId               = teamId;
            PreviousAssignmentId = previousAssignmentId;
        }
    }
}
