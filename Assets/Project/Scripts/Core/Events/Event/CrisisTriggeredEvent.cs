using System.Collections.Generic;
using Project.Core.Definitions.Event;
using Project.Core.Definitions.Time;

namespace Project.Core.Events.Event
{
    /// <summary>
    /// Domain event published by EventCrisisTickProcessor when a game event fires and its
    /// effects have been applied to runtime state.
    ///
    /// Consumed by ReportEventHandler (Plan 2L) to generate a crisis report in the player inbox.
    /// One CrisisTriggeredEvent is published per fired event instance — threshold events may
    /// produce multiple publications in a single tick (one per qualifying team or product).
    /// </summary>
    public sealed class CrisisTriggeredEvent
    {
        // ─── Identity ─────────────────────────────────────────────────────────────

        /// <summary>GUID of the fired event instance. Matches GameEventRuntimeState.InstanceId.</summary>
        public string InstanceId { get; }

        /// <summary>Stable ID referencing the GameEventDefinition. Used for catalog lookups.</summary>
        public string EventDefinitionId { get; }

        // ─── Content ──────────────────────────────────────────────────────────────

        /// <summary>Resolved human-readable description of what happened (tokens substituted).</summary>
        public string Description { get; }

        // ─── Classification ───────────────────────────────────────────────────────

        /// <summary>Simulation domain of the event, used to map to ReportCategory.</summary>
        public GameEventCategory Category { get; }

        /// <summary>Urgency level, used to map to ReportPriority.</summary>
        public GameEventSeverity Severity { get; }

        /// <summary>The interruption type raised alongside this event, if any.</summary>
        public InterruptionType? InterruptionType { get; }

        // ─── Effects ──────────────────────────────────────────────────────────────

        /// <summary>Effects that were applied to runtime state when this event fired.</summary>
        public List<GameEventEffect> AppliedEffects { get; }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a CrisisTriggeredEvent.
        /// </summary>
        /// <param name="instanceId">GUID of the fired event instance.</param>
        /// <param name="eventDefinitionId">Stable event definition ID.</param>
        /// <param name="description">Resolved narrative description.</param>
        /// <param name="category">Simulation domain classification.</param>
        /// <param name="severity">Urgency level.</param>
        /// <param name="interruptionType">Interruption type, or null if not interrupting.</param>
        /// <param name="appliedEffects">Effects applied to runtime state.</param>
        public CrisisTriggeredEvent(
            string instanceId,
            string eventDefinitionId,
            string description,
            GameEventCategory category,
            GameEventSeverity severity,
            InterruptionType? interruptionType,
            List<GameEventEffect> appliedEffects)
        {
            InstanceId         = instanceId;
            EventDefinitionId  = eventDefinitionId;
            Description        = description;
            Category           = category;
            Severity           = severity;
            InterruptionType   = interruptionType;
            AppliedEffects     = appliedEffects;
        }
    }
}
