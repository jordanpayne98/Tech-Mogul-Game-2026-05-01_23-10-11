namespace Project.Application
{
    /// <summary>
    /// Stable string constants for all modal IDs.
    /// IDs must not change without data migration — use these constants everywhere,
    /// never inline the raw strings at call sites.
    /// </summary>
    public static class ModalIds
    {
        public const string Confirm   = "modal.confirm";
        public const string Info      = "modal.info";
        public const string Warning   = "modal.warning";
        public const string Error     = "modal.error";
        public const string Detail    = "modal.detail";
        public const string SaveGame  = "modal.save_game";
        public const string LoadGame  = "modal.load_game";
        public const string NewGame   = "modal.new_game";
        public const string Settings  = "modal.settings";
    }
}
