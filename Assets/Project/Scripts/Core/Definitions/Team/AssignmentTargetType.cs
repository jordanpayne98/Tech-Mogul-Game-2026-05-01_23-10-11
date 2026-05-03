namespace Project.Core.Definitions.Team
{
    /// <summary>
    /// Identifies the category of entity that an assignment is targeting.
    /// Used alongside a stable target ID to resolve the assignment target at runtime.
    /// </summary>
    public enum AssignmentTargetType
    {
        Product,
        Contract,
        ResearchProject,
        Infrastructure,
        Company
    }
}
