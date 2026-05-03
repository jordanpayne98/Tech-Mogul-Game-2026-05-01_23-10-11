namespace Project.Core.Results.SaveLoad
{
    /// <summary>
    /// Lightweight metadata for a save file, populated by partial JSON parsing.
    /// Used by ISaveService.ListSaves() to return save slot data without full deserialization.
    /// </summary>
    public sealed class SaveSlotInfo
    {
        /// <summary>Stable unique ID identifying the save file on disk.</summary>
        public string SaveId { get; set; }

        /// <summary>Human-readable display name for the save slot.</summary>
        public string SaveName { get; set; }

        /// <summary>UTC timestamp of when the save was written, as an ISO 8601 string.</summary>
        public string SavedAtUtcIso8601 { get; set; }

        /// <summary>Schema version recorded in the save file.</summary>
        public int SaveVersion { get; set; }

        /// <summary>Serialized SaveType member name (e.g. "Manual", "Autosave", "QuickSave").</summary>
        public string SaveType { get; set; }
    }
}
