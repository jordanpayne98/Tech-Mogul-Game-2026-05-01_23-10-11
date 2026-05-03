namespace Project.Core.Events.Team
{
    /// <summary>
    /// Published after an employee is removed from a team.
    /// Consumers can use TeamId and EmployeeId to update dependent state.
    /// </summary>
    public sealed class TeamMemberRemovedEvent
    {
        public string TeamId     { get; }
        public string EmployeeId { get; }

        public TeamMemberRemovedEvent(string teamId, string employeeId)
        {
            TeamId     = teamId;
            EmployeeId = employeeId;
        }
    }
}
