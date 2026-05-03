namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the time system.
    /// Services that drive time advancement depend on this interface rather than
    /// the full ITuningConfig, maintaining domain isolation.
    /// </summary>
    public interface ITimeTuning
    {
        /// <summary>Hours advanced per simulation tick.</summary>
        int DefaultTickHours { get; }

        /// <summary>
        /// Safety cap on the number of ticks processed per Continue press.
        /// Prototype default: 8640 (one game year).
        /// </summary>
        int MaxTicksPerContinue { get; }
    }
}
