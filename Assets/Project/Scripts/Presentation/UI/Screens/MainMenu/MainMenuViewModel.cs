namespace Project.Presentation.UI.Screens.MainMenu
{
    /// <summary>
    /// Pure display-data class for the Main Menu screen.
    /// Immutable after construction. No Unity dependencies.
    /// Created by MainMenuController and passed to MainMenuView.
    /// </summary>
    public sealed class MainMenuViewModel
    {
        /// <summary>
        /// Controls the Continue button enabled/disabled state.
        /// Phase 4: always false — no save data exists yet.
        /// </summary>
        public bool HasSaveData { get; }

        /// <summary>
        /// Build version string for the footer, e.g. "Build v0.1".
        /// </summary>
        public string BuildVersion { get; }

        /// <summary>
        /// Copyright text for the footer, e.g. "(c) 2026 Tech Mogul. All rights reserved."
        /// </summary>
        public string CopyrightText { get; }

        public MainMenuViewModel(bool hasSaveData, string buildVersion, string copyrightText)
        {
            HasSaveData   = hasSaveData;
            BuildVersion  = buildVersion;
            CopyrightText = copyrightText;
        }
    }
}
