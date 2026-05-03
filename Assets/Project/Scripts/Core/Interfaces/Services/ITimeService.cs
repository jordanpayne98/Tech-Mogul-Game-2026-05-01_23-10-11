using Project.Core.Definitions.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Time domain service interface.
    /// Defines the contract for advancing the game clock and mutating TimeRuntimeState.
    /// Implemented by TimeService in Core/Services/Time.
    /// Services depend on this interface, not the concrete class.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Advances the game clock by one hour, mutates the state, and returns
        /// a TickContext containing the previous and new dates with edge-triggered boundary flags.
        /// Returns null if state is null (logs error internally).
        /// </summary>
        /// <param name="state">Mutable runtime state to advance. Must not be null.</param>
        TickContext AdvanceOneHour(TimeRuntimeState state);

        /// <summary>Sets the time speed on the runtime state.</summary>
        /// <param name="state">Runtime state to mutate.</param>
        /// <param name="speed">New time speed.</param>
        void SetSpeed(TimeRuntimeState state, TimeSpeed speed);

        /// <summary>Sets the time advance mode on the runtime state.</summary>
        /// <param name="state">Runtime state to mutate.</param>
        /// <param name="mode">New advance mode.</param>
        void SetAdvanceMode(TimeRuntimeState state, TimeAdvanceMode mode);

        /// <summary>Sets the interruption filter on the runtime state.</summary>
        /// <param name="state">Runtime state to mutate.</param>
        /// <param name="filter">New interruption filter.</param>
        void SetInterruptionFilter(TimeRuntimeState state, InterruptionFilter filter);
    }
}
