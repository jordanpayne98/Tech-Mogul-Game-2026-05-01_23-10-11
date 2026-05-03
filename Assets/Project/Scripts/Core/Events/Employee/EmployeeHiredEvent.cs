namespace Project.Core.Events.Employee
{
    /// <summary>
    /// Published after a candidate successfully accepts a hire offer and is converted to an employee.
    /// </summary>
    public sealed class EmployeeHiredEvent
    {
        /// <summary>Stable ID of the newly created employee record.</summary>
        public string EmployeeId { get; }

        /// <summary>Stable ID of the company that hired the employee.</summary>
        public string CompanyId { get; }

        public EmployeeHiredEvent(string employeeId, string companyId)
        {
            EmployeeId = employeeId;
            CompanyId  = companyId;
        }
    }
}
