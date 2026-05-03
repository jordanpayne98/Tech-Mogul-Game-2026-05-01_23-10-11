using System;
using Project.Core.Interfaces;
using Project.Core.Runtime;

namespace Project.Core.Services
{
    /// <summary>
    /// Core implementation of IRandomSource backed by System.Random with call tracking.
    /// Reconstructs RNG position on load by advancing the PRNG by CallCount steps from the Seed.
    /// All Next/NextFloat calls increment RandomRuntimeState.CallCount to keep state serialisable.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class DeterministicRandomSource : IRandomSource
    {
        private readonly Random             _random;
        private readonly RandomRuntimeState _state;

        /// <summary>
        /// The underlying System.Random instance.
        /// Exposed so Application-layer code that takes System.Random as a parameter can share the same
        /// deterministic stream. Callers that use this directly must not duplicate random calls that
        /// would be tracked by IRandomSource — prefer using the IRandomSource interface when possible.
        /// </summary>
        public Random SystemRandom => _random;

        /// <summary>
        /// Creates a DeterministicRandomSource from the given state.
        /// If state.CallCount > 0, the internal RNG is fast-forwarded by that many steps
        /// to restore the position from a previous save.
        /// </summary>
        /// <param name="state">Mutable runtime state that tracks seed and call count.</param>
        public DeterministicRandomSource(RandomRuntimeState state)
        {
            _state  = state ?? throw new ArgumentNullException(nameof(state));
            _random = new Random(state.Seed);

            // Restore position for loaded saves.
            for (int i = 0; i < state.CallCount; i++)
            {
                _random.Next();
            }
        }

        // ─── IRandomSource ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int Next(int maxExclusive)
        {
            int result = _random.Next(maxExclusive);
            _state.CallCount++;
            return result;
        }

        /// <inheritdoc/>
        public int Next(int minInclusive, int maxExclusive)
        {
            int result = _random.Next(minInclusive, maxExclusive);
            _state.CallCount++;
            return result;
        }

        /// <inheritdoc/>
        public float NextFloat()
        {
            float result = (float)_random.NextDouble();
            _state.CallCount++;
            return result;
        }
    }
}
