namespace Project.Core.SaveData
{
    /// <summary>
    /// Save data for the deterministic RNG state.
    /// Captured at save time and restored on load to enable reproducible simulation from a given point.
    /// </summary>
    public sealed class RandomStateSaveData
    {
        /// <summary>The seed used to initialise the RNG.</summary>
        public int Seed;

        /// <summary>Internal state array for full RNG restoration.</summary>
        public int[] InternalState;
    }
}
