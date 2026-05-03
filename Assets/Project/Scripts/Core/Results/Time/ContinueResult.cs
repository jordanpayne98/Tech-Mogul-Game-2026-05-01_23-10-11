using System.Collections.Generic;
using Project.Core.Requests.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Results.Time
{
    /// <summary>
    /// Result returned from a complete Continue time advancement cycle.
    /// Use static factory methods: Succeeded(...) or Failed(string reason).
    /// </summary>
    public sealed class ContinueResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>True if the Continue cycle completed without a critical error.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string on success.</summary>
        public string FailureReason { get; }

        /// <summary>The game date at which the cycle stopped.</summary>
        public GameDateTime StoppedAtDate { get; }

        /// <summary>Total hours advanced during this Continue session.</summary>
        public int TotalHoursAdvanced { get; }

        /// <summary>
        /// All interruptions that matched the active filter and caused the cycle to stop.
        /// Empty if the cycle stopped manually or hit the safety cap.
        /// </summary>
        public List<InterruptionRequest> Interruptions { get; }

        // -------------------------------------------------------------------------
        // Private constructor
        // -------------------------------------------------------------------------

        private ContinueResult(
            bool success,
            string failureReason,
            GameDateTime stoppedAtDate,
            int totalHoursAdvanced,
            List<InterruptionRequest> interruptions)
        {
            Success            = success;
            FailureReason      = failureReason;
            StoppedAtDate      = stoppedAtDate;
            TotalHoursAdvanced = totalHoursAdvanced;
            Interruptions      = interruptions ?? new List<InterruptionRequest>();
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>Creates a successful result with no interruptions.</summary>
        /// <param name="stoppedAt">The game date at which the cycle stopped.</param>
        /// <param name="hoursAdvanced">Total hours advanced.</param>
        public static ContinueResult Succeeded(GameDateTime stoppedAt, int hoursAdvanced)
        {
            return new ContinueResult(true, string.Empty, stoppedAt, hoursAdvanced, null);
        }

        /// <summary>Creates a successful result with matched interruptions.</summary>
        /// <param name="stoppedAt">The game date at which the cycle stopped.</param>
        /// <param name="hoursAdvanced">Total hours advanced.</param>
        /// <param name="interruptions">Interruptions that caused the stop.</param>
        public static ContinueResult Succeeded(
            GameDateTime stoppedAt,
            int hoursAdvanced,
            List<InterruptionRequest> interruptions)
        {
            return new ContinueResult(true, string.Empty, stoppedAt, hoursAdvanced, interruptions);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        /// <param name="reason">Human-readable explanation of why the cycle failed.</param>
        public static ContinueResult Failed(string reason)
        {
            return new ContinueResult(false, reason, null, 0, null);
        }
    }
}
