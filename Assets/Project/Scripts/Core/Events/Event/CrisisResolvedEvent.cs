namespace Project.Core.Events.Event
{
    /// <summary>
    /// Domain event published when an active crisis or game event instance has been resolved.
    /// Consumed by AutosaveCoordinator to trigger a post-resolution autosave.
    /// </summary>
    public sealed class CrisisResolvedEvent
    {
        /// <summary>GUID of the resolved event instance. Matches GameEventRuntimeState.InstanceId.</summary>
        public string InstanceId { get; }

        /// <summary>Stable ID referencing the GameEventDefinition that was resolved.</summary>
        public string EventDefinitionId { get; }

        /// <summary>
        /// Creates a new CrisisResolvedEvent.
        /// </summary>
        /// <param name="instanceId">GUID of the resolved event instance.</param>
        /// <param name="eventDefinitionId">Stable event definition ID.</param>
        public CrisisResolvedEvent(string instanceId, string eventDefinitionId)
        {
            InstanceId        = instanceId;
            EventDefinitionId = eventDefinitionId;
        }
    }
}
