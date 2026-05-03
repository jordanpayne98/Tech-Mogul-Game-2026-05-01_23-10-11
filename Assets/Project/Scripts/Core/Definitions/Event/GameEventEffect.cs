namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Describes a concrete effect to apply when a game event fires.
    /// All magnitudes use basis points for consistency across effect types.
    ///
    /// Basis point conversion guide:
    ///   - Demand:      TotalDemand += TotalDemand * ValueBasisPoints / 10000
    ///   - Morale:      Morale      += ValueBasisPoints / 100  (e.g. -500 bps = -5 morale)
    ///   - Defect rate: DefectRateBasisPoints += ValueBasisPoints (direct addition)
    /// </summary>
    public sealed class GameEventEffect
    {
        /// <summary>The kind of state mutation this effect performs.</summary>
        public GameEventEffectType Type { get; }

        /// <summary>
        /// Stable ID of the affected entity (market category ID, team ID, product ID).
        /// Null for system-wide effects.
        /// </summary>
        public string TargetEntityId { get; }

        /// <summary>
        /// Effect magnitude in basis points. Sign indicates direction.
        /// Examples: -1500 = -15% demand shift, -500 = -5 morale, +200 = +2% defect rate.
        /// </summary>
        public int ValueBasisPoints { get; }

        /// <summary>Human-readable summary of this specific effect instance.</summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new GameEventEffect.
        /// </summary>
        /// <param name="type">The kind of mutation.</param>
        /// <param name="targetEntityId">Stable ID of the affected entity, or null for system-wide.</param>
        /// <param name="valueBasisPoints">Effect magnitude in basis points.</param>
        /// <param name="description">Human-readable summary.</param>
        public GameEventEffect(
            GameEventEffectType type,
            string targetEntityId,
            int valueBasisPoints,
            string description)
        {
            Type             = type;
            TargetEntityId   = targetEntityId;
            ValueBasisPoints = valueBasisPoints;
            Description      = description;
        }
    }
}
