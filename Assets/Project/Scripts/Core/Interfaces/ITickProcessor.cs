using Project.Core.Results.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces
{
    /// <summary>
    /// Interface for domain tick processors.
    /// Each time-driven domain implements its own processor.
    /// Processors are registered with TickCoordinator and called in ProcessingOrder each tick.
    /// </summary>
    public interface ITickProcessor
    {
        /// <summary>Unique stable name used for logging and duplicate detection.</summary>
        string ProcessorName { get; }

        /// <summary>
        /// Processing order relative to other registered processors.
        /// Lower values execute first. See Phase 2 Locked Decisions for the canonical order.
        /// </summary>
        int ProcessingOrder { get; }

        /// <summary>
        /// Processes one simulation tick. Called once per hour of advancement.
        /// Must not fail silently — return Failed(reason) on broken state.
        /// </summary>
        /// <param name="context">Immutable tick metadata including boundary flags.</param>
        TickResult ProcessTick(TickContext context);
    }
}
