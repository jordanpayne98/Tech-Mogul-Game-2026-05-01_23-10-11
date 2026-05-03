using Project.Core.Definitions.Time;

namespace Project.Core.Requests.Time
{
    /// <summary>
    /// Data object representing a request to interrupt Continue time advancement.
    /// Produced by ITickProcessor implementations and evaluated by ContinueOrchestrator
    /// against the active InterruptionFilter after all processors complete for a tick.
    /// </summary>
    public sealed class InterruptionRequest
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>Category of the interruption, used for filter matching.</summary>
        public InterruptionType Type { get; }

        /// <summary>Stable ID of the entity that raised this interruption (e.g. product ID, contract ID).</summary>
        public string SourceEntityId { get; }

        /// <summary>Human-readable description of the interruption for display or logging.</summary>
        public string Message { get; }

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new InterruptionRequest.
        /// </summary>
        /// <param name="type">Category of the interruption.</param>
        /// <param name="sourceEntityId">Stable ID of the source entity.</param>
        /// <param name="message">Human-readable description.</param>
        public InterruptionRequest(InterruptionType type, string sourceEntityId, string message)
        {
            Type           = type;
            SourceEntityId = sourceEntityId;
            Message        = message;
        }
    }
}
