namespace Project.Core.Definitions.SaveLoad
{
    /// <summary>
    /// Distinguishes the category of a save operation.
    /// Used by SaveRequest and written into GameSaveData.SaveType for reference on load.
    /// </summary>
    public enum SaveType
    {
        /// <summary>Manually triggered by the player.</summary>
        Manual,

        /// <summary>Automatically triggered by the autosave system.</summary>
        Autosave,

        /// <summary>Triggered by the quick-save action.</summary>
        QuickSave
    }
}
