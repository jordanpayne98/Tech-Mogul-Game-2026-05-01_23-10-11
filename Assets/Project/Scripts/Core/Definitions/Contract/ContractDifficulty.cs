namespace Project.Core.Definitions.Contract
{
    /// <summary>
    /// The difficulty tier of a contract, affecting required skill levels and potential payment.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public enum ContractDifficulty
    {
        Easy,
        Standard,
        Hard,
        Expert
    }
}
