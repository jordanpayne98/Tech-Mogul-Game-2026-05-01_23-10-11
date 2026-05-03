namespace Project.Core.Definitions.Employee
{
    /// <summary>
    /// The seniority level of an employee or candidate.
    /// Lead is a seniority label only — it does not represent a team manager role.
    /// Team management is handled separately via team assignment, not via seniority.
    /// </summary>
    public enum Seniority
    {
        Junior,
        Mid,
        Senior,
        Lead,
        Principal
    }
}
