using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Project.Core.Debugging;
using Project.Core.Interfaces.Services;
using Project.Core.Requests.SaveLoad;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;
using Project.Core.SaveData;
using Project.Infrastructure.SaveLoad.Mappers;

namespace Project.Infrastructure.SaveLoad
{
    /// <summary>
    /// Concrete ISaveService implementation using Newtonsoft Json.NET with atomic file writes.
    /// Saves are written atomically via a .tmp rename pattern to prevent file corruption.
    /// Version mismatch is detected early via partial JObject parsing before full deserialization.
    /// All IO and deserialization exceptions are caught, logged, and returned as typed failure results.
    /// </summary>
    public sealed class SaveService : ISaveService
    {
        // ─── Constants ────────────────────────────────────────────────────────────

        private const string SaveFileExtension = ".json";
        private const string TempFileExtension = ".tmp";

        // ─── Fields ───────────────────────────────────────────────────────────────

        private readonly string _savesDirectory;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Converters        = new List<JsonConverter> { new StringEnumConverter() },
            NullValueHandling = NullValueHandling.Include,
            Formatting        = Formatting.Indented
        };

        // ─── Constructor ──────────────────────────────────────────────────────────

        public SaveService()
        {
            _savesDirectory = Path.Combine(UnityEngine.Application.persistentDataPath, "Saves");
        }

        // ─── ISaveService ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public SaveResult Save(GameSessionState session, SaveRequest request)
        {
            try
            {
                EnsureSavesDirectoryExists();

                // Use caller-supplied ID for fixed slots (e.g. quick save), otherwise generate a GUID.
                string saveId = string.IsNullOrEmpty(request.OverrideSaveId)
                    ? Guid.NewGuid().ToString()
                    : request.OverrideSaveId;

                GameSaveData saveData = GameSessionMapper.ToSaveData(session, saveId, request.SaveName, request.Type);

                string json        = JsonConvert.SerializeObject(saveData, SerializerSettings);
                string finalPath   = BuildSavePath(saveId);
                string tempPath    = finalPath + TempFileExtension;

                // Atomic write: write to temp first, then rename.
                File.WriteAllText(tempPath, json);

                if (File.Exists(finalPath))
                {
                    File.Delete(finalPath);
                }

                File.Move(tempPath, finalPath);

                DebugLogger.Log(DebugCategory.SaveLoad, $"Save written successfully. SaveId: {saveId}, Name: {request.SaveName}");

                return SaveResult.Succeeded(saveId);
            }
            catch (Exception ex)
            {
                string reason = $"Save failed: {ex.Message}";
                DebugLogger.LogError(DebugCategory.SaveLoad, reason);
                return SaveResult.Failed(reason);
            }
        }

        /// <inheritdoc/>
        public LoadResult Load(string saveId)
        {
            try
            {
                string filePath = BuildSavePath(saveId);

                if (!File.Exists(filePath))
                {
                    string notFoundReason = $"Save file not found for SaveId: {saveId}";
                    DebugLogger.LogError(DebugCategory.SaveLoad, notFoundReason);
                    return LoadResult.Failed(notFoundReason);
                }

                string json = File.ReadAllText(filePath);

                // Partial parse: check version before full deserialization to fail fast on mismatches.
                JObject jObject      = JObject.Parse(json);
                int     savedVersion = jObject.Value<int>("SaveVersion");

                if (savedVersion != GameSaveData.CurrentSaveVersion)
                {
                    string versionReason = $"Save version mismatch. File has version {savedVersion}, expected {GameSaveData.CurrentSaveVersion}. SaveId: {saveId}";
                    DebugLogger.LogError(DebugCategory.SaveLoad, versionReason);
                    return LoadResult.Failed(versionReason);
                }

                // Full deserialization from the same JSON string — no double file read.
                GameSaveData saveData = JsonConvert.DeserializeObject<GameSaveData>(json, SerializerSettings);

                if (saveData == null)
                {
                    string nullReason = $"Deserialized save data was null. SaveId: {saveId}";
                    DebugLogger.LogError(DebugCategory.SaveLoad, nullReason);
                    return LoadResult.Failed(nullReason);
                }

                GameSessionState session = GameSessionMapper.FromSaveData(saveData);

                DebugLogger.Log(DebugCategory.SaveLoad, $"Load succeeded. SaveId: {saveId}, Name: {saveData.SaveName}");

                return LoadResult.Succeeded(session);
            }
            catch (Exception ex)
            {
                string reason = $"Load failed for SaveId '{saveId}': {ex.Message}";
                DebugLogger.LogError(DebugCategory.SaveLoad, reason);
                return LoadResult.Failed(reason);
            }
        }

        /// <inheritdoc/>
        public List<SaveSlotInfo> ListSaves()
        {
            var result = new List<SaveSlotInfo>();

            try
            {
                EnsureSavesDirectoryExists();

                string[] files = Directory.GetFiles(_savesDirectory, $"*{SaveFileExtension}");

                foreach (string filePath in files)
                {
                    try
                    {
                        string  json    = File.ReadAllText(filePath);
                        JObject jObject = JObject.Parse(json);

                        var info = new SaveSlotInfo
                        {
                            SaveId           = jObject.Value<string>("SaveId"),
                            SaveName         = jObject.Value<string>("SaveName"),
                            SavedAtUtcIso8601 = jObject.Value<string>("SavedAtUtcIso8601"),
                            SaveVersion      = jObject.Value<int>("SaveVersion"),
                            SaveType         = jObject.Value<string>("SaveType")
                        };

                        result.Add(info);
                    }
                    catch (Exception fileEx)
                    {
                        // Skip corrupt or unreadable files — log a warning and continue.
                        DebugLogger.LogWarning(DebugCategory.SaveLoad, $"Skipping unreadable save file '{filePath}': {fileEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad, $"ListSaves failed: {ex.Message}");
            }

            return result;
        }

        /// <inheritdoc/>
        public DeleteResult Delete(string saveId)
        {
            try
            {
                string filePath = BuildSavePath(saveId);

                if (!File.Exists(filePath))
                {
                    string notFoundReason = $"Cannot delete — save file not found for SaveId: {saveId}";
                    DebugLogger.LogWarning(DebugCategory.SaveLoad, notFoundReason);
                    return DeleteResult.Failed(notFoundReason);
                }

                File.Delete(filePath);

                DebugLogger.Log(DebugCategory.SaveLoad, $"Save deleted. SaveId: {saveId}");

                return DeleteResult.Succeeded();
            }
            catch (Exception ex)
            {
                string reason = $"Delete failed for SaveId '{saveId}': {ex.Message}";
                DebugLogger.LogError(DebugCategory.SaveLoad, reason);
                return DeleteResult.Failed(reason);
            }
        }

        /// <inheritdoc/>
        public bool SaveExists(string saveId)
        {
            return File.Exists(BuildSavePath(saveId));
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private string BuildSavePath(string saveId)
        {
            return Path.Combine(_savesDirectory, saveId + SaveFileExtension);
        }

        private void EnsureSavesDirectoryExists()
        {
            if (!Directory.Exists(_savesDirectory))
            {
                Directory.CreateDirectory(_savesDirectory);
                DebugLogger.Log(DebugCategory.SaveLoad, $"Created saves directory at: {_savesDirectory}");
            }
        }
    }
}
