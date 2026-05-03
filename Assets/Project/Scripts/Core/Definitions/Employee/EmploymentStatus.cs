namespace Project.Core.Definitions.Employee
{
    /// <summary>
    /// The current employment status of an employee record.
    /// PendingStart represents a hired candidate who has not yet begun work.
    /// </summary>
    public enum EmploymentStatus
    {
        PendingStart,
        Active,
        OnNotice,
        Resigned,
        Terminated
    }
}
