namespace Project.Core.Runtime.Time
{
    /// <summary>
    /// Immutable value object carrying tick metadata for ITickProcessor implementations.
    /// All boundary flags are edge-triggered: they are true only on the first hour of the new period.
    /// Constructed by TimeService.AdvanceOneHour after each clock step.
    /// </summary>
    public sealed class TickContext
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>The game date immediately before this tick was applied.</summary>
        public GameDateTime PreviousDate { get; }

        /// <summary>The game date after this tick was applied.</summary>
        public GameDateTime CurrentDate { get; }

        /// <summary>Hours elapsed during this tick. Typically 1.</summary>
        public int ElapsedHours { get; }

        /// <summary>True on the first hour of a new calendar day.</summary>
        public bool IsDayBoundary { get; }

        /// <summary>True on the first hour of every 7th calendar day from the epoch.</summary>
        public bool IsWeekBoundary { get; }

        /// <summary>True on the first hour of a new calendar month.</summary>
        public bool IsMonthBoundary { get; }

        /// <summary>
        /// True on the first hour of a quarter-starting month (Month 1, 4, 7, or 10).
        /// Only fires on a month boundary.
        /// </summary>
        public bool IsQuarterBoundary { get; }

        /// <summary>True on the first hour of a new calendar year.</summary>
        public bool IsYearBoundary { get; }

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a TickContext and computes all boundary flags from the two adjacent dates.
        /// </summary>
        /// <param name="previousDate">Game date before the tick.</param>
        /// <param name="currentDate">Game date after the tick.</param>
        /// <param name="elapsedHours">Hours advanced in this tick.</param>
        public TickContext(GameDateTime previousDate, GameDateTime currentDate, int elapsedHours)
        {
            PreviousDate = previousDate;
            CurrentDate  = currentDate;
            ElapsedHours = elapsedHours;

            // ── Edge-triggered boundary flags ──────────────────────────────────
            // Year boundary: year component changed.
            IsYearBoundary = previousDate.Year != currentDate.Year;

            // Month boundary: month or year component changed.
            IsMonthBoundary = previousDate.Month != currentDate.Month
                           || previousDate.Year  != currentDate.Year;

            // Day boundary: any date component changed.
            IsDayBoundary = previousDate.Day   != currentDate.Day
                         || previousDate.Month != currentDate.Month
                         || previousDate.Year  != currentDate.Year;

            // Quarter boundary: fires only on a month boundary, at a quarter-start month.
            IsQuarterBoundary = IsMonthBoundary
                             && (currentDate.Month == 1
                              || currentDate.Month == 4
                              || currentDate.Month == 7
                              || currentDate.Month == 10);

            // Week boundary: fires only on a day boundary, every 7 calendar days from epoch.
            // totalElapsedDays uses 0-based day count from epoch for divisibility.
            int totalElapsedDays = ((currentDate.Year  - 1) * GameDateTime.DaysPerYear)
                                 + ((currentDate.Month - 1) * GameDateTime.DaysPerMonth)
                                 +  (currentDate.Day   - 1);

            IsWeekBoundary = IsDayBoundary && (totalElapsedDays % 7 == 0);
        }
    }
}
