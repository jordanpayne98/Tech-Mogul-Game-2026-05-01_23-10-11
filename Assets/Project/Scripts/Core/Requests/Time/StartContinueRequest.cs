using Project.Core.Definitions.Time;

namespace Project.Core.Requests.Time
{
    /// <summary>
    /// Request to start the Continue time advancement cycle.
    /// Consumed by ContinueUseCase → ContinueOrchestrator.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class StartContinueRequest
    {
        /// <summary>Time speed multiplier for this Continue session.</summary>
        public TimeSpeed Speed { get; }

        /// <summary>Whether to advance continuously until an interruption or step manually.</summary>
        public TimeAdvanceMode Mode { get; }

        /// <summary>Which interruption types are allowed to pause this Continue session.</summary>
        public InterruptionFilter Filter { get; }

        /// <summary>
        /// Creates a new StartContinueRequest.
        /// </summary>
        /// <param name="speed">Desired time speed.</param>
        /// <param name="mode">Desired advance mode.</param>
        /// <param name="filter">Desired interruption filter.</param>
        public StartContinueRequest(TimeSpeed speed, TimeAdvanceMode mode, InterruptionFilter filter)
        {
            Speed  = speed;
            Mode   = mode;
            Filter = filter;
        }
    }
}
