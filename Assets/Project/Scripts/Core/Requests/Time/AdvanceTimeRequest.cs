using Project.Core.Definitions.Time;

namespace Project.Core.Requests.Time
{
    /// <summary>
    /// Request to advance the game clock by a given number of hours at a specified speed.
    /// Consumed by the time advance use case. No logic is executed inside this class.
    /// </summary>
    public sealed class AdvanceTimeRequest
    {
        /// <summary>Number of hours to advance. Must be greater than zero.</summary>
        public int Hours { get; }

        /// <summary>The time speed multiplier to use during this advance operation.</summary>
        public TimeSpeed Speed { get; }

        /// <summary>
        /// Creates a new AdvanceTimeRequest.
        /// </summary>
        /// <param name="hours">Number of hours to advance.</param>
        /// <param name="speed">Time speed multiplier for the advance.</param>
        public AdvanceTimeRequest(int hours, TimeSpeed speed)
        {
            Hours = hours;
            Speed = speed;
        }
    }
}
