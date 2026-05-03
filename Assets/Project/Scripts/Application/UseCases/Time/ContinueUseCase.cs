using Project.Core.Debugging;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Time
{
    /// <summary>
    /// Thin entry point for starting and stopping the Continue time advancement cycle.
    /// Validates inputs and delegates to ContinueOrchestrator.
    /// Lives in Application — no UnityEngine dependency.
    /// </summary>
    public sealed class ContinueUseCase
    {
        private readonly ContinueOrchestrator _orchestrator;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new ContinueUseCase.
        /// </summary>
        /// <param name="orchestrator">Orchestrator that drives the Continue loop.</param>
        public ContinueUseCase(ContinueOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Validates the request and state, then starts the Continue time advancement cycle.
        /// </summary>
        /// <param name="request">Settings for this Continue session. Must not be null.</param>
        /// <param name="state">Mutable runtime time state. Must not be null.</param>
        public ContinueResult Start(StartContinueRequest request, TimeRuntimeState state)
        {
            if (request == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[ContinueUseCase] Start called with null request.");
                return ContinueResult.Failed("Request is null.");
            }

            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[ContinueUseCase] Start called with null state.");
                return ContinueResult.Failed("State is null.");
            }

            return _orchestrator.Execute(request, state);
        }

        /// <summary>
        /// Signals a manual stop for an active Continue session.
        /// </summary>
        /// <param name="request">Manual stop request. Must not be null.</param>
        /// <param name="state">Mutable runtime time state. Must not be null.</param>
        public void Stop(StopContinueRequest request, TimeRuntimeState state)
        {
            if (request == null)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ContinueUseCase] Stop called with null request.");
                return;
            }

            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[ContinueUseCase] Stop called with null state.");
                return;
            }

            _orchestrator.Stop(request, state);
        }
    }
}
