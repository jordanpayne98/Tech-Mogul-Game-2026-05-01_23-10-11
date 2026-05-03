namespace Project.Core.Events.Contract
{
    /// <summary>
    /// Published when a contract milestone is reached during tick processing.
    /// MilestoneIndex is 0-based (first milestone = 0).
    /// </summary>
    public sealed class ContractMilestoneCompletedEvent
    {
        /// <summary>Stable ID of the contract that reached the milestone.</summary>
        public string ContractId { get; }

        /// <summary>Zero-based index of the milestone that was just completed.</summary>
        public int MilestoneIndex { get; }

        /// <summary>
        /// Creates a new ContractMilestoneCompletedEvent.
        /// </summary>
        public ContractMilestoneCompletedEvent(string contractId, int milestoneIndex)
        {
            ContractId     = contractId;
            MilestoneIndex = milestoneIndex;
        }
    }
}
