using System;

namespace Project.Core.Runtime.Time
{
    /// <summary>
    /// Immutable value object representing a specific point in game time.
    /// All time in the simulation is expressed as a GameDateTime.
    /// The calendar uses a simplified fixed model: 30 days per month, 12 months per year.
    /// </summary>
    public sealed class GameDateTime : IEquatable<GameDateTime>
    {
        // -------------------------------------------------------------------------
        // Calendar constants — GDD_02 fixed time model
        // -------------------------------------------------------------------------

        public const int HoursPerDay   = 24;
        public const int DaysPerMonth  = 30;
        public const int MonthsPerYear = 12;
        public const int DaysPerYear   = 360;
        public const int HoursPerMonth = 720;
        public const int HoursPerYear  = 8640;

        // -------------------------------------------------------------------------
        // Static defaults
        // -------------------------------------------------------------------------

        /// <summary>
        /// The default start date for a new game session: Year 2026, Month 1, Day 1, Hour 9.
        /// </summary>
        public static readonly GameDateTime DefaultStart = new GameDateTime(2026, 1, 1, 9);

        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>The game year (e.g. 2026).</summary>
        public int Year { get; }

        /// <summary>The game month, 1-indexed (1–12).</summary>
        public int Month { get; }

        /// <summary>The game day within the month, 1-indexed (1–30).</summary>
        public int Day { get; }

        /// <summary>The hour of day, 0-indexed (0–23).</summary>
        public int Hour { get; }

        /// <summary>
        /// Total elapsed hours from the epoch (Year 1, Month 1, Day 1, Hour 0).
        /// Computed from Year, Month, Day, Hour using the fixed calendar constants.
        /// Formula: ((Year - 1) * HoursPerYear) + ((Month - 1) * HoursPerMonth) + ((Day - 1) * HoursPerDay) + Hour
        /// </summary>
        public int TotalElapsedHours { get; }

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new GameDateTime. All values must be valid for the fixed calendar model.
        /// </summary>
        /// <param name="year">Game year, must be greater than 0.</param>
        /// <param name="month">Month (1–12).</param>
        /// <param name="day">Day (1–30).</param>
        /// <param name="hour">Hour (0–23).</param>
        public GameDateTime(int year, int month, int day, int hour)
        {
            Year  = year;
            Month = month;
            Day   = day;
            Hour  = hour;

            // Formula: ((Year - 1) * HoursPerYear) + ((Month - 1) * HoursPerMonth) + ((Day - 1) * HoursPerDay) + Hour
            TotalElapsedHours = ((Year - 1) * HoursPerYear)
                              + ((Month - 1) * HoursPerMonth)
                              + ((Day - 1) * HoursPerDay)
                              + Hour;
        }

        // -------------------------------------------------------------------------
        // Equality
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public bool Equals(GameDateTime other)
        {
            if (other is null)
            {
                return false;
            }

            return Year  == other.Year
                && Month == other.Month
                && Day   == other.Day
                && Hour  == other.Hour;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as GameDateTime);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day, Hour);
        }

        // -------------------------------------------------------------------------
        // Arithmetic
        // -------------------------------------------------------------------------

        /// <summary>
        /// Returns a new GameDateTime representing this date advanced by the given number of hours.
        /// The result must not fall before the epoch (TotalElapsedHours must remain >= 0).
        /// </summary>
        /// <param name="hours">Number of hours to add. Must not produce a negative total.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the result would have a negative TotalElapsedHours.
        /// </exception>
        public GameDateTime AddHours(int hours)
        {
            int newTotal = TotalElapsedHours + hours;

            if (newTotal < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(hours),
                    $"AddHours({hours}) would produce a negative TotalElapsedHours ({newTotal}). " +
                    $"Current TotalElapsedHours is {TotalElapsedHours}.");
            }

            return FromTotalHours(newTotal);
        }

        /// <summary>
        /// Constructs a GameDateTime from a total elapsed hours value.
        /// This is the inverse of TotalElapsedHours.
        /// FromTotalHours(0) returns Year 1, Month 1, Day 1, Hour 0 (the epoch).
        /// </summary>
        /// <param name="totalHours">Total elapsed hours from the epoch. Must be >= 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when totalHours is negative.</exception>
        public static GameDateTime FromTotalHours(int totalHours)
        {
            if (totalHours < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(totalHours),
                    $"totalHours must be >= 0. Received: {totalHours}.");
            }

            int year      = (totalHours / HoursPerYear) + 1;
            int remainder = totalHours % HoursPerYear;
            int month     = (remainder / HoursPerMonth) + 1;
            remainder     = remainder % HoursPerMonth;
            int day       = (remainder / HoursPerDay) + 1;
            int hour      = remainder % HoursPerDay;

            return new GameDateTime(year, month, day, hour);
        }

        // -------------------------------------------------------------------------
        // String representation
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Year {Year}, Month {Month:D2}, Day {Day:D2}, Hour {Hour:D2}:00";
        }
    }
}
