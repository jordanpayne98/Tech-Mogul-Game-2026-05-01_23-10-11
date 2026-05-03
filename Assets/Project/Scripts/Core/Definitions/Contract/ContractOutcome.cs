namespace Project.Core.Definitions.Contract
{
    /// <summary>
    /// The final outcome of a contract once it has been completed or failed.
    /// None indicates the contract has not yet reached a terminal state.
    /// Failed contracts affect payment only; there is no separate reputation impact field.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public enum ContractOutcome
    {
        None,
        Excellent,
        Accepted,
        Failed
    }
}
