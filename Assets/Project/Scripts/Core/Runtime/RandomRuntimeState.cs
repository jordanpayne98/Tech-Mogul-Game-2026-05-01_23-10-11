namespace Project.Core.Runtime
{
    /// <summary>
    /// Serializable state for the deterministic random number generator.
    /// On save: persist Seed and CallCount.
    /// On load: reconstruct System.Random from Seed, then advance it CallCount times to restore position.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class RandomRuntimeState
    {
        /// <summary>The original seed used to initialise the PRNG.</summary>
        public int Seed { get; set; }

        /// <summary>Total number of Next/NextFloat calls made since initialisation.</summary>
        public int CallCount { get; set; }
    }
}
