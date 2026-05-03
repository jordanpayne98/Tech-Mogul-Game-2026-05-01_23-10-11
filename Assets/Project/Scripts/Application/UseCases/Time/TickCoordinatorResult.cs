using System.Collections.Generic;
using Project.Core.Requests.Time;

namespace Project.Application.UseCases.Time
{
    /// <summary>
    /// Aggregated result from all ITickProcessor executions for a single tick.
    /// Distinct from TickResult (per-processor) — this is the combined output
    /// across all registered processors for one clock step.
    /// Use static factory methods: Succeeded(...) or Failed(string reason).
    /// </summary>
    public sealed class TickCoordinatorResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if all processors completed without failure.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// All interruptions raised by all processors during this tick.
        /// Empty if none were raised.
        /// </summary>
        public List<InterruptionRequest> Interruptions { get; }

        /// <summary>Number of processors that executed during this tick.</summary>
        public int ProcessorsExecuted { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private TickCoordinatorResult(
            bool success,
            string failureReason,
            int processorsExecuted,
            List<InterruptionRequest> interruptions)
        {
            Success            = success;
            FailureReason      = failureReason;
            ProcessorsExecuted = processorsExecuted;
            Interruptions      = interruptions ?? new List<InterruptionRequest>();
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>Creates a successful result with no interruptions.</summary>
        /// <param name="processorsExecuted">Number of processors that ran.</param>
        public static TickCoordinatorResult Succeeded(int processorsExecuted)
        {
            return new TickCoordinatorResult(true, string.Empty, processorsExecuted, null);
        }

        /// <summary>Creates a successful result with aggregated interruptions.</summary>
        /// <param name="processorsExecuted">Number of processors that ran.</param>
        /// <param name="interruptions">All interruptions raised across all processors.</param>
        public static TickCoordinatorResult Succeeded(
            int processorsExecuted,
            List<InterruptionRequest> interruptions)
        {
            return new TickCoordinatorResult(true, string.Empty, processorsExecuted, interruptions);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        /// <param name="reason">Human-readable explanation of the failure.</param>
        public static TickCoordinatorResult Failed(string reason)
        {
            return new TickCoordinatorResult(false, reason, 0, null);
        }
    }
}
