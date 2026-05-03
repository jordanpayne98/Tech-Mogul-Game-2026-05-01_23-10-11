using System.Collections.Generic;
using Project.Core.Definitions.SaveLoad;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Core interface for managed save operations with slot limits and rolling rotation.
    /// Sits above ISaveService and enforces slot limits, autosave rotation, and quick save overwrite.
    /// Concrete implementation lives in Project.Infrastructure.SaveLoad.SaveSlotManager.
    /// </summary>
    public interface ISaveSlotManager
    {
        /// <summary>Fixed save ID used by the quick save slot. Always overwrites.</summary>
        const string QuickSaveId = "quicksave";

        /// <summary>
        /// Requests a manual save. Checks the manual save count against the configured limit.
        /// Returns SaveResult.Failed if the limit is reached. Otherwise generates a GUID save ID,
        /// builds a SaveRequest of type Manual, and delegates to ISaveService.
        /// </summary>
        /// <param name="session">Current game session state to serialize.</param>
        /// <param name="saveName">Human-readable name for the save slot.</param>
        SaveResult RequestManualSave(GameSessionState session, string saveName);

        /// <summary>
        /// Requests a quick save using the fixed ID "quicksave".
        /// Always overwrites the existing quick save slot. No slot limit enforced.
        /// </summary>
        /// <param name="session">Current game session state to serialize.</param>
        SaveResult RequestQuickSave(GameSessionState session);

        /// <summary>
        /// Requests an autosave. Checks the autosave count against the configured limit.
        /// If at or over the limit, deletes the oldest autosave before writing the new one.
        /// Save name is set to "Autosave - {triggerReason}".
        /// </summary>
        /// <param name="session">Current game session state to serialize.</param>
        /// <param name="triggerReason">Human-readable reason for this autosave (e.g. "monthly_boundary").</param>
        SaveResult RequestAutosave(GameSessionState session, string triggerReason);

        /// <summary>
        /// Loads the save file with the given save ID.
        /// Delegates to ISaveService.Load().
        /// </summary>
        /// <param name="saveId">Stable ID of the save file to load.</param>
        LoadResult LoadSave(string saveId);

        /// <summary>
        /// Loads the quick save slot using the fixed ID "quicksave".
        /// Delegates to ISaveService.Load().
        /// </summary>
        LoadResult LoadQuickSave();

        /// <summary>
        /// Deletes the save file with the given save ID.
        /// Delegates to ISaveService.Delete().
        /// </summary>
        /// <param name="saveId">Stable ID of the save file to delete.</param>
        DeleteResult DeleteSave(string saveId);

        /// <summary>
        /// Returns lightweight metadata for all save files, optionally filtered by SaveType.
        /// Delegates to ISaveService.ListSaves() and filters by type if specified.
        /// </summary>
        /// <param name="filterType">If specified, only saves of this type are returned. Null returns all.</param>
        List<SaveSlotInfo> ListSaves(SaveType? filterType = null);

        /// <summary>
        /// Returns the current number of autosave slots on disk.
        /// </summary>
        int GetAutosaveCount();

        /// <summary>
        /// Returns the current number of manual save slots on disk.
        /// </summary>
        int GetManualSaveCount();
    }
}
