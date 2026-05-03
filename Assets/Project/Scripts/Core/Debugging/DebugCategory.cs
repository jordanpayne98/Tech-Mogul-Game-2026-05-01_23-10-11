namespace Project.Core.Debugging
{
    /// <summary>
    /// Genre-neutral debug categories used for filtering log output via DebugConfig.
    /// Do not add game-specific categories until the related GDD system is defined.
    /// </summary>
    public enum DebugCategory
    {
        General,
        Bootstrap,
        Configuration,
        DataDefinitions,
        Validation,
        SaveLoad,
        UI,
        Navigation,
        Input,
        Audio,
        Visual,
        Performance,
        Testing,
        Simulation
    }
}
