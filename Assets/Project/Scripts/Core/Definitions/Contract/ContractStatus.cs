namespace Project.Core.Definitions.Contract
{
    /// <summary>
    /// The lifecycle status of a contract.
    /// Tracks progression from marketplace availability through completion or expiry.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public enum ContractStatus
    {
        Available,
        Accepted,
        InProgress,
        Completed,
        Failed,
        Expired
    }
}
