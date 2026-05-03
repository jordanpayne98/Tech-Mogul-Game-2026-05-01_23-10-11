using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.SaveLoad;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.SaveLoad;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;

namespace Project.Infrastructure.SaveLoad
{
    /// <summary>
    /// Concrete ISaveSlotManager implementation.
    /// Handles GUID generation, slot limit enforcement for manual saves, rolling autosave rotation,
    /// and quick save fixed-ID overwrite. All operations delegate file IO to ISaveService.
    /// All operations are logged via DebugLogger with DebugCategory.SaveLoad.
    /// </summary>
    public sealed class SaveSlotManager : ISaveSlotManager
    {
        private readonly ISaveService _saveService;
        private readonly ISaveTuning  _tuning;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new SaveSlotManager.
        /// </summary>
        /// <param name="saveService">Raw file IO service.</param>
        /// <param name="tuning">Save tuning configuration providing slot limits.</param>
        public SaveSlotManager(ISaveService saveService, ISaveTuning tuning)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _tuning      = tuning      ?? throw new ArgumentNullException(nameof(tuning));
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — Manual Save
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public SaveResult RequestManualSave(GameSessionState session, string saveName)
        {
            int manualCount = GetManualSaveCount();

            if (manualCount >= _tuning.MaxManualSaveSlots)
            {
                string reason = $"Maximum manual save slots reached ({_tuning.MaxManualSaveSlots}). Delete an existing save to continue.";
                DebugLogger.LogWarning(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] RequestManualSave rejected — {reason}");
                return SaveResult.Failed(reason);
            }

            SaveRequest request = new SaveRequest(saveName, SaveType.Manual);

            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] RequestManualSave — Name: {saveName}");

            SaveResult result = _saveService.Save(session, request);

            if (!result.Success)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] RequestManualSave failed — {result.FailureReason}");
            }
            else
            {
                DebugLogger.Log(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] Manual save succeeded — SaveId: {result.SaveId}");
            }

            return result;
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — Quick Save
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public SaveResult RequestQuickSave(GameSessionState session)
        {
            // Pass the fixed ID as an override so SaveService writes to the "quicksave" slot.
            SaveRequest request = new SaveRequest("Quick Save", SaveType.QuickSave, ISaveSlotManager.QuickSaveId);

            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] RequestQuickSave — SaveId: {ISaveSlotManager.QuickSaveId}");

            SaveResult result = _saveService.Save(session, request);

            if (!result.Success)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] RequestQuickSave failed — {result.FailureReason}");
            }
            else
            {
                DebugLogger.Log(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] Quick save succeeded — SaveId: {result.SaveId}");
            }

            return result;
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — Autosave
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public SaveResult RequestAutosave(GameSessionState session, string triggerReason)
        {
            int autosaveCount = GetAutosaveCount();

            if (autosaveCount >= _tuning.MaxAutosaveSlots)
            {
                // Rolling rotation: delete the oldest autosave before writing the new one.
                DeleteOldestAutosave();
            }

            string saveName = $"Autosave - {triggerReason}";
            SaveRequest request = new SaveRequest(saveName, SaveType.Autosave);

            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] RequestAutosave — Reason: {triggerReason}");

            SaveResult result = _saveService.Save(session, request);

            if (!result.Success)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] RequestAutosave failed — {result.FailureReason}");
            }
            else
            {
                DebugLogger.Log(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] Autosave succeeded — SaveId: {result.SaveId} | Reason: {triggerReason}");
            }

            return result;
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — Load
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public LoadResult LoadSave(string saveId)
        {
            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] LoadSave — SaveId: {saveId}");

            LoadResult result = _saveService.Load(saveId);

            if (!result.Success)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] LoadSave failed — {result.FailureReason}");
            }

            return result;
        }

        /// <inheritdoc/>
        public LoadResult LoadQuickSave()
        {
            return LoadSave(ISaveSlotManager.QuickSaveId);
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — Delete
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public DeleteResult DeleteSave(string saveId)
        {
            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] DeleteSave — SaveId: {saveId}");

            DeleteResult result = _saveService.Delete(saveId);

            if (!result.Success)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] DeleteSave failed — {result.FailureReason}");
            }

            return result;
        }

        // -------------------------------------------------------------------------
        // ISaveSlotManager — List / Count
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public List<SaveSlotInfo> ListSaves(SaveType? filterType = null)
        {
            List<SaveSlotInfo> all = _saveService.ListSaves();

            if (filterType == null)
            {
                return all;
            }

            string filterName = filterType.Value.ToString();
            return all.Where(s => string.Equals(s.SaveType, filterName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <inheritdoc/>
        public int GetAutosaveCount()
        {
            return ListSaves(SaveType.Autosave).Count;
        }

        /// <inheritdoc/>
        public int GetManualSaveCount()
        {
            return ListSaves(SaveType.Manual).Count;
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Finds and deletes the oldest autosave by SavedAtUtcIso8601 timestamp.
        /// If deletion fails, logs a warning but does not block the new autosave.
        /// </summary>
        private void DeleteOldestAutosave()
        {
            List<SaveSlotInfo> autosaves = ListSaves(SaveType.Autosave);

            if (autosaves.Count == 0)
            {
                return;
            }

            // Sort ascending by UTC timestamp string — ISO 8601 sorts lexicographically.
            autosaves.Sort((a, b) => string.Compare(a.SavedAtUtcIso8601, b.SavedAtUtcIso8601, StringComparison.Ordinal));

            SaveSlotInfo oldest = autosaves[0];

            DebugLogger.Log(DebugCategory.SaveLoad,
                $"[SaveSlotManager] Rolling autosave rotation — deleting oldest autosave: {oldest.SaveId} ({oldest.SavedAtUtcIso8601})");

            DeleteResult deleteResult = _saveService.Delete(oldest.SaveId);

            if (!deleteResult.Success)
            {
                DebugLogger.LogWarning(DebugCategory.SaveLoad,
                    $"[SaveSlotManager] Failed to delete oldest autosave '{oldest.SaveId}': {deleteResult.FailureReason}. New autosave will still proceed.");
            }
        }
    }
}
