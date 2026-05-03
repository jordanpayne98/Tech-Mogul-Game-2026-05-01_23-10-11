namespace Project.Application
{
    /// <summary>
    /// Stable string constants for all right drawer IDs.
    /// IDs must not change without data migration — use these constants everywhere,
    /// never inline the raw strings at call sites.
    /// </summary>
    public static class DrawerIds
    {
        /// <summary>Generic filter drawer with search and category dropdowns.</summary>
        public const string Filter = "drawer.filter";
    }
}
