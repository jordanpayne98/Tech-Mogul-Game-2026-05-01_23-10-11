namespace Project.Core.Requests.Team
{
    /// <summary>
    /// Request to unassign a team from its current assignment.
    /// The team must have an active assignment; otherwise the use case returns a failure result.
    /// The previous AssignmentRuntimeState is NOT deleted — it is archived in AssignmentHistoryIds.
    /// </summary>
    public sealed class UnassignTeamRequest
    {
        public string TeamId { get; }

        public UnassignTeamRequest(string teamId)
        {
            TeamId = teamId;
        }
    }
}
