namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Identifies the kind of state mutation a game event effect applies.
    /// Only ModifyDemand, ModifyTeamMorale, and ModifyDefectRate are used in Plan 2M.
    /// ModifyCash and ModifyReputation are forward-compatible placeholders.
    /// </summary>
    public enum GameEventEffectType
    {
        ModifyDemand,
        ModifyTeamMorale,
        ModifyDefectRate,
        ModifyCash,
        ModifyReputation
    }
}
