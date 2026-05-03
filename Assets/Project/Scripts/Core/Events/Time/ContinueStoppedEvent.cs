using Project.Core.Definitions.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Events.Time
{
    /// <summary>
    /// Published via IEventBus when the Continue cycle stops.
    /// StopReason is null for manual stops or safety-cap stops.
    /// Carries immutable snapshot data only — no mutable state references.
    /// </summary>
    public sealed class ContinueStoppedEvent
    {
        /// <summary>The game date at which Continue stopped.</summary>
        public GameDateTime StopDate { get; }

        /// <summary>
        /// The category of interruption that caused the stop, or null if stopped manually or by safety cap.
        /// </summary>
        public InterruptionType? StopReason { get; }

        /// <summary>
        /// Stable ID of the entity that raised the interruption.
        /// Empty string if stopped manually or by safety cap.
        /// </summary>
        public string SourceEntityId { get; }

        /// <summary>
        /// Creates a new ContinueStoppedEvent.
        /// </summary>
        /// <param name="stopDate">Game date when Continue stopped.</param>
        /// <param name="stopReason">Interruption type that caused the stop, or null.</param>
        /// <param name="sourceEntityId">Stable ID of the interrupting entity, or empty string.</param>
        public ContinueStoppedEvent(GameDateTime stopDate, InterruptionType? stopReason, string sourceEntityId)
        {
            StopDate      = stopDate;
            StopReason    = stopReason;
            SourceEntityId = sourceEntityId;
        }
    }
}
