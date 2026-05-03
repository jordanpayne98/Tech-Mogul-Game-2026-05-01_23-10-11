using System;

namespace Project.Core.Runtime.Finance
{
    /// <summary>
    /// Lightweight value type identifying a specific year-month period in the game calendar.
    /// Used as a dictionary key and lookup handle for MonthlyFinanceSummary records.
    /// </summary>
    public struct FinancePeriodKey : IEquatable<FinancePeriodKey>
    {
        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        /// <summary>The game year (e.g. 2026).</summary>
        public int Year;

        /// <summary>The game month, 1-indexed (1–12).</summary>
        public int Month;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        public FinancePeriodKey(int year, int month)
        {
            Year  = year;
            Month = month;
        }

        // -------------------------------------------------------------------------
        // Equality
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public bool Equals(FinancePeriodKey other)
        {
            return Year == other.Year && Month == other.Month;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is FinancePeriodKey other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month);
        }

        // -------------------------------------------------------------------------
        // String representation
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Year {Year}, Month {Month:D2}";
        }
    }
}
