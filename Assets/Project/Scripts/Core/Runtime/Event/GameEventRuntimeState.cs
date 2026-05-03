using System.Collections.Generic;
using Project.Core.Definitions.Event;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Event
{
    /// <summary>
    /// Mutable runtime record of a single fired event instance.
    /// Created by EventCrisisTickProcessor when an event fires.
    /// Added to GameSessionState.EventHistory for save/load persistence.
    ///
    /// Resolved is always true for MVP — all events auto-resolve.
    /// Player-choice crisis responses are out of scope for Plan 2M.
    /// </summary>
    public sealed class GameEventRuntimeState
    {
        /// <summary>GUID uniquely identifying this specific occurrence of the event.</summary>
        public string InstanceId { get; set; }

        /// <summary>Stable ID referencing the GameEventDefinition that generated this instance.</summary>
        public string EventDefinitionId { get; set; }

        /// <summary>The game date on which this event fired.</summary>
        public GameDateTime FiredDate { get; set; }

        /// <summary>Effects that were applied to runtime state when this event fired.</summary>
        public List<GameEventEffect> AppliedEffects { get; set; } = new List<GameEventEffect>();

        /// <summary>
        /// True if this event has been resolved.
        /// Always true in Plan 2M (auto-resolve). Decision-based resolution is deferred.
        /// </summary>
        public bool Resolved { get; set; }
    }
}
