namespace Project.Core.Events.Team
{
    /// <summary>
    /// Published after an employee is added to a team.
    /// Consumers can use TeamId and EmployeeId to update dependent state.
    /// </summary>
    public sealed class TeamMemberAddedEvent
    {
        public string TeamId     { get; }
        public string EmployeeId { get; }

        public TeamMemberAddedEvent(string teamId, string employeeId)
        {
            TeamId     = teamId;
            EmployeeId = employeeId;
        }
    }
}
