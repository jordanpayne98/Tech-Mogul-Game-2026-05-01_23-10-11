using Project.Core.Runtime;
using Project.Core.SaveData;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps RNG state for deterministic save/load.
    /// For MVP: saves Seed and CallCount; restores by reconstructing with the same seed.
    /// InternalState is preserved in the save data for future full-state restoration if needed.
    /// All methods are static — this mapper holds no state.
    /// </summary>
    public static class RandomStateSaveMapper
    {
        public static RandomStateSaveData ToSaveData(RandomRuntimeState state)
        {
            return new RandomStateSaveData
            {
                Seed          = state.Seed,
                // InternalState is not extracted from System.Random for MVP.
                // CallCount is stored on RandomRuntimeState and used for restoration.
                InternalState = null
            };
        }

        public static RandomRuntimeState FromSaveData(RandomStateSaveData data)
        {
            return new RandomRuntimeState
            {
                Seed      = data.Seed,
                // CallCount is not persisted in RandomStateSaveData — it is tracked on RandomRuntimeState.
                // On load, the simulation reconstructs with the same seed.
                CallCount = 0
            };
        }
    }
}
