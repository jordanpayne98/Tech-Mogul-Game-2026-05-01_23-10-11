namespace Project.Core.Definitions.Employee
{
    /// <summary>
    /// The functional role of an employee within the company.
    /// Lead is a seniority level defined in <see cref="Seniority"/>, not a role value here.
    /// </summary>
    public enum EmployeeRole
    {
        SoftwareEngineer,
        HardwareEngineer,
        ProductDesigner,
        ProductManager,
        QAEngineer,
        InfrastructureEngineer,
        Researcher,
        DataAISpecialist,
        MarketingSpecialist,
        SalesBusinessDev,
        SupportSpecialist,
        OperationsSpecialist,
        RecruiterHR,
        FinanceAdmin
    }
}
