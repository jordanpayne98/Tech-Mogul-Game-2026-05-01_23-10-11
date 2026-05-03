namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the save slot management system.
    /// Services that manage save slot limits depend on this interface rather than
    /// the full ITuningConfig, maintaining domain isolation.
    /// </summary>
    public interface ISaveTuning
    {
        /// <summary>
        /// Maximum number of rolling autosave slots.
        /// When the count reaches this limit, the oldest autosave is deleted before writing a new one.
        /// Default: 5. Valid range: 1–20.
        /// </summary>
        int MaxAutosaveSlots { get; }

        /// <summary>
        /// Maximum number of manual save slots.
        /// When the count reaches this limit, RequestManualSave() returns a failure result.
        /// Default: 20. Valid range: 1–100.
        /// </summary>
        int MaxManualSaveSlots { get; }
    }
}
