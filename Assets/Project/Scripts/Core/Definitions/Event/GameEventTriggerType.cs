namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Distinguishes how a game event is triggered.
    /// RandomProbability events fire based on a probability roll and global cooldown.
    /// ThresholdCheck events fire deterministically when a runtime value crosses a defined boundary.
    /// </summary>
    public enum GameEventTriggerType
    {
        RandomProbability,
        ThresholdCheck
    }
}
