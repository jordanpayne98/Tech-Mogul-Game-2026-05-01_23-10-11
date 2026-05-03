namespace Project.Core.Interfaces
{
    /// <summary>
    /// Interface for deterministic random number generation.
    /// All systems that require randomness depend on this interface instead of System.Random directly.
    /// Enables deterministic save/load by reconstructing state from Seed + CallCount.
    /// Defined in Plan 2N.
    /// </summary>
    public interface IRandomSource
    {
        /// <summary>Returns a non-negative integer in the range [0, maxExclusive).</summary>
        int Next(int maxExclusive);

        /// <summary>Returns a non-negative integer in the range [minInclusive, maxExclusive).</summary>
        int Next(int minInclusive, int maxExclusive);

        /// <summary>Returns a float in the range [0.0, 1.0].</summary>
        float NextFloat();
    }
}
