namespace Project.Core.Events.Company
{
    /// <summary>
    /// Published on the event bus after a new company and founder have been fully initialized.
    /// Listeners may react to this event to perform deferred initialization (e.g. candidate pool seeding).
    /// </summary>
    public sealed class CompanyCreatedEvent
    {
        /// <summary>Stable ID of the newly created company ("company.player").</summary>
        public string CompanyId { get; }

        /// <summary>Stable ID of the newly created founder ("founder.player").</summary>
        public string FounderId { get; }

        /// <summary>Starting cash balance in minor currency units.</summary>
        public long StartingCashMinorUnits { get; }

        public CompanyCreatedEvent(string companyId, string founderId, long startingCashMinorUnits)
        {
            CompanyId              = companyId;
            FounderId              = founderId;
            StartingCashMinorUnits = startingCashMinorUnits;
        }
    }
}
