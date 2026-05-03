using System.Collections.Generic;
using Project.Core.Requests.Time;

namespace Project.Core.Results.Time
{
    /// <summary>
    /// Result returned by each ITickProcessor after processing a single tick.
    /// Interruptions, if any, are evaluated by ContinueOrchestrator after all processors
    /// have run for the tick. Use Succeeded() or Failed(string) static factories.
    /// </summary>
    public sealed class TickResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the processor completed its tick work successfully.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// Interruptions raised during this tick. Empty if none.
        /// Evaluated against the active InterruptionFilter by ContinueOrchestrator.
        /// </summary>
        public List<InterruptionRequest> Interruptions { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private TickResult(bool success, string failureReason, List<InterruptionRequest> interruptions)
        {
            Success       = success;
            FailureReason = failureReason;
            Interruptions = interruptions ?? new List<InterruptionRequest>();
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>Creates a successful result with no interruptions.</summary>
        public static TickResult Succeeded()
        {
            return new TickResult(true, string.Empty, null);
        }

        /// <summary>Creates a successful result with the specified interruptions.</summary>
        /// <param name="interruptions">Interruptions raised during the tick.</param>
        public static TickResult Succeeded(List<InterruptionRequest> interruptions)
        {
            return new TickResult(true, string.Empty, interruptions);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        /// <param name="reason">Human-readable explanation of why the tick failed.</param>
        public static TickResult Failed(string reason)
        {
            return new TickResult(false, reason, null);
        }
    }
}
