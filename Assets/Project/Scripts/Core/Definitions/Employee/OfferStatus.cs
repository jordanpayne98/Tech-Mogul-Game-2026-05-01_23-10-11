namespace Project.Core.Definitions.Employee
{
    /// <summary>
    /// The current status of a salary offer made to a candidate.
    /// </summary>
    public enum OfferStatus
    {
        None,
        Pending,
        Accepted,
        Rejected,
        Negotiating,
        Expired
    }
}
