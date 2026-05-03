using Project.Core.Definitions.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Events.Time
{
    /// <summary>
    /// Published via IEventBus when the Continue cycle begins.
    /// Carries immutable snapshot data only — no mutable state references.
    /// </summary>
    public sealed class ContinueStartedEvent
    {
        /// <summary>The game date at which Continue was started.</summary>
        public GameDateTime StartDate { get; }

        /// <summary>The time speed at which Continue was started.</summary>
        public TimeSpeed Speed { get; }

        /// <summary>
        /// Creates a new ContinueStartedEvent.
        /// </summary>
        /// <param name="startDate">Game date when Continue began.</param>
        /// <param name="speed">Active time speed.</param>
        public ContinueStartedEvent(GameDateTime startDate, TimeSpeed speed)
        {
            StartDate = startDate;
            Speed     = speed;
        }
    }
}
