using Project.Core.Runtime.Time;

namespace Project.Core.Events.Time
{
    /// <summary>
    /// Published via IEventBus after each hourly clock tick.
    /// Carries immutable snapshot data only — no mutable state references.
    /// </summary>
    public sealed class TimeAdvancedEvent
    {
        /// <summary>The game date immediately before this tick was applied.</summary>
        public GameDateTime PreviousDate { get; }

        /// <summary>The game date after this tick was applied.</summary>
        public GameDateTime NewDate { get; }

        /// <summary>Hours elapsed during this tick. Typically 1.</summary>
        public int ElapsedHours { get; }

        /// <summary>
        /// Creates a new TimeAdvancedEvent.
        /// </summary>
        /// <param name="previousDate">Game date before the tick.</param>
        /// <param name="newDate">Game date after the tick.</param>
        /// <param name="elapsedHours">Hours advanced.</param>
        public TimeAdvancedEvent(GameDateTime previousDate, GameDateTime newDate, int elapsedHours)
        {
            PreviousDate = previousDate;
            NewDate      = newDate;
            ElapsedHours = elapsedHours;
        }
    }
}
