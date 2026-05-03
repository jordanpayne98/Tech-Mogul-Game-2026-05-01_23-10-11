namespace Project.Core.Requests.Time
{
    /// <summary>
    /// Request to manually stop an active Continue time advancement cycle.
    /// Consumed by ContinueUseCase → ContinueOrchestrator.Stop.
    /// No logic is executed inside this class.
    /// </summary>
    public sealed class StopContinueRequest
    {
        /// <summary>Human-readable reason for the manual stop (for logging purposes).</summary>
        public string Reason { get; }

        /// <summary>
        /// Creates a new StopContinueRequest.
        /// </summary>
        /// <param name="reason">Human-readable reason for stopping.</param>
        public StopContinueRequest(string reason)
        {
            Reason = reason;
        }
    }
}
