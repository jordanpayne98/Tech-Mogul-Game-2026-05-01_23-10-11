using Project.Core.Definitions.Time;

namespace Project.Core.Runtime.Time
{
    /// <summary>
    /// Mutable runtime state for the game clock.
    /// Owned by the time system and updated as time advances.
    /// Created via CreateDefault() for a new game session.
    /// </summary>
    public sealed class TimeRuntimeState
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>The current in-game date and time.</summary>
        public GameDateTime CurrentDate { get; set; }

        /// <summary>
        /// Total elapsed hours since the epoch.
        /// Mirrors CurrentDate.TotalElapsedHours but kept separately for
        /// efficient comparison and scheduling lookups.
        /// </summary>
        public int TotalElapsedHours { get; set; }

        /// <summary>Current time speed multiplier. Paused means no advancement.</summary>
        public TimeSpeed Speed { get; set; }

        /// <summary>Whether time advances manually or runs until an interruption occurs.</summary>
        public TimeAdvanceMode AdvanceMode { get; set; }

        /// <summary>Which interruption types are allowed to pause continuous time advancement.</summary>
        public InterruptionFilter Filter { get; set; }

        /// <summary>True while the time system is actively advancing the clock.</summary>
        public bool IsAdvancing { get; set; }

        // -------------------------------------------------------------------------
        // Factory
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a default TimeRuntimeState for a new game session.
        /// Speed: Speed1x, AdvanceMode: ContinueUntilInterrupt, Filter: CriticalOnly,
        /// IsAdvancing: false, CurrentDate: GameDateTime.DefaultStart.
        /// </summary>
        public static TimeRuntimeState CreateDefault()
        {
            GameDateTime startDate = GameDateTime.DefaultStart;

            return new TimeRuntimeState
            {
                CurrentDate       = startDate,
                TotalElapsedHours = startDate.TotalElapsedHours,
                Speed             = TimeSpeed.Speed1x,
                AdvanceMode       = TimeAdvanceMode.ContinueUntilInterrupt,
                Filter            = InterruptionFilter.CriticalOnly,
                IsAdvancing       = false
            };
        }
    }
}
