using Project.Core.Definitions.Market;

namespace Project.Core.Events.Market
{
    /// <summary>
    /// Published when a market trend's state changes.
    /// Covers three cases: spawning (IsActive = true, new trend), decay (strength updated),
    /// and expiry (IsActive = false, Strength = 0). A single event type suffices for MVP.
    /// Defined in Plan 2I, GDD_10.
    /// </summary>
    public sealed class TrendChangedEvent
    {
        /// <summary>Stable GUID-based ID of the trend instance.</summary>
        public string TrendId { get; }

        /// <summary>The type of the trend.</summary>
        public TrendType Type { get; }

        /// <summary>Whether the trend is currently active after this change.</summary>
        public bool IsActive { get; }

        /// <summary>Current strength of the trend after this change. Range: 0–100.</summary>
        public int Strength { get; }

        /// <summary>
        /// Creates a new <see cref="TrendChangedEvent"/>.
        /// </summary>
        /// <param name="trendId">Stable ID of the trend instance.</param>
        /// <param name="type">The trend type.</param>
        /// <param name="isActive">Whether the trend is still active.</param>
        /// <param name="strength">Current strength after the change.</param>
        public TrendChangedEvent(string trendId, TrendType type, bool isActive, int strength)
        {
            TrendId  = trendId;
            Type     = type;
            IsActive = isActive;
            Strength = strength;
        }
    }
}
