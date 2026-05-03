using System.Collections.Generic;
using Project.Core.Requests.SaveLoad;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Core interface for raw save/load file operations.
    /// Infrastructure provides the concrete implementation via SaveService.
    /// All methods are synchronous — async support is deferred to a later plan.
    /// </summary>
    public interface ISaveService
    {
        /// <summary>
        /// Serializes the given session state and writes it atomically to disk.
        /// Returns a SaveResult indicating success or failure with a reason.
        /// </summary>
        SaveResult Save(GameSessionState session, SaveRequest request);

        /// <summary>
        /// Reads the save file for the given save ID, validates its version,
        /// deserializes the full GameSaveData, and reconstructs a GameSessionState.
        /// Returns a LoadResult indicating success or failure with a reason.
        /// </summary>
        LoadResult Load(string saveId);

        /// <summary>
        /// Scans the saves directory and returns lightweight metadata for each valid save file.
        /// Corrupt or unreadable files are skipped with a warning log.
        /// </summary>
        List<SaveSlotInfo> ListSaves();

        /// <summary>
        /// Deletes the save file with the given save ID.
        /// Returns a DeleteResult indicating success or failure with a reason.
        /// </summary>
        DeleteResult Delete(string saveId);

        /// <summary>
        /// Returns true if a save file for the given save ID exists on disk.
        /// </summary>
        bool SaveExists(string saveId);
    }
}
