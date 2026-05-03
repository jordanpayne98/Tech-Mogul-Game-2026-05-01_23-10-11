using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Time;
using Project.Core.Events.Time;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Time
{
    /// <summary>
    /// Orchestrates the Continue time advancement loop.
    /// Coordinates TimeRuntimeState mutation, tick processing, interruption evaluation,
    /// and event publication. Lives in Application — no UnityEngine dependency.
    /// </summary>
    public sealed class ContinueOrchestrator
    {
        private readonly ITimeService   _timeService;
        private readonly TickCoordinator _coordinator;
        private readonly ITimeTuning    _tuning;
        private readonly IEventBus      _eventBus;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new ContinueOrchestrator.
        /// </summary>
        /// <param name="timeService">Time service for clock advancement.</param>
        /// <param name="coordinator">Tick coordinator for domain processing.</param>
        /// <param name="tuning">Time tuning interface providing safety cap.</param>
        /// <param name="eventBus">Event bus for cross-domain notifications.</param>
        public ContinueOrchestrator(
            ITimeService   timeService,
            TickCoordinator coordinator,
            ITimeTuning    tuning,
            IEventBus      eventBus)
        {
            _timeService = timeService;
            _coordinator = coordinator;
            _tuning      = tuning;
            _eventBus    = eventBus;
        }

        // -------------------------------------------------------------------------
        // Public API
        // -------------------------------------------------------------------------

        /// <summary>
        /// Executes the Continue loop. Advances time tick by tick until an interruption
        /// matches the active filter, a manual stop is requested, or the safety cap is reached.
        /// </summary>
        /// <param name="request">Settings for this Continue session.</param>
        /// <param name="state">Mutable runtime time state.</param>
        public ContinueResult Execute(StartContinueRequest request, TimeRuntimeState state)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[ContinueOrchestrator] Execute called with null state.");
                return ContinueResult.Failed("State is null.");
            }

            if (state.IsAdvancing)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ContinueOrchestrator] Execute called while already advancing.");
                return ContinueResult.Failed("Already advancing.");
            }

            if (request.Speed == TimeSpeed.Paused)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[ContinueOrchestrator] Execute called with Speed.Paused. Continue requires an active speed.");
                return ContinueResult.Failed("Speed is Paused.");
            }

            // Apply request settings to state.
            _timeService.SetSpeed(state, request.Speed);
            _timeService.SetAdvanceMode(state, request.Mode);
            _timeService.SetInterruptionFilter(state, request.Filter);

            GameDateTime startDate = state.CurrentDate;
            state.IsAdvancing = true;

            _eventBus.Publish(new ContinueStartedEvent(startDate, request.Speed));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContinueOrchestrator] Continue started at {startDate} | Speed:{request.Speed} | Filter:{request.Filter}");

            int hoursAdvanced                          = 0;
            List<InterruptionRequest> matchedInterruptions = new();
            int safetyCap                              = _tuning.MaxTicksPerContinue;

            while (state.IsAdvancing && hoursAdvanced < safetyCap)
            {
                TickContext context = _timeService.AdvanceOneHour(state);

                if (context == null)
                {
                    DebugLogger.LogError(DebugCategory.Simulation,
                        "[ContinueOrchestrator] AdvanceOneHour returned null. Stopping Continue.");
                    break;
                }

                hoursAdvanced++;

                TickCoordinatorResult tickResult = _coordinator.ProcessTick(context);

                // Evaluate interruptions from this tick against the active filter.
                if (tickResult.Interruptions != null && tickResult.Interruptions.Count > 0)
                {
                    foreach (InterruptionRequest interruption in tickResult.Interruptions)
                    {
                        if (ShouldInterrupt(interruption, state.Filter))
                        {
                            matchedInterruptions.Add(interruption);
                        }
                    }
                }

                // Stop if any interruptions matched.
                if (matchedInterruptions.Count > 0)
                {
                    state.IsAdvancing = false;
                }
            }

            state.IsAdvancing = false;

            // Determine stop reason for the event.
            InterruptionType? stopReason    = null;
            string            sourceEntityId = string.Empty;

            if (matchedInterruptions.Count > 0)
            {
                stopReason     = matchedInterruptions[0].Type;
                sourceEntityId = matchedInterruptions[0].SourceEntityId;
            }

            _eventBus.Publish(new ContinueStoppedEvent(state.CurrentDate, stopReason, sourceEntityId));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContinueOrchestrator] Continue stopped at {state.CurrentDate} " +
                $"| Hours advanced: {hoursAdvanced} | Interruptions: {matchedInterruptions.Count}");

            if (matchedInterruptions.Count > 0)
            {
                return ContinueResult.Succeeded(state.CurrentDate, hoursAdvanced, matchedInterruptions);
            }

            return ContinueResult.Succeeded(state.CurrentDate, hoursAdvanced);
        }

        /// <summary>
        /// Signals a manual stop for an active Continue session by clearing IsAdvancing on the state.
        /// The loop will exit on its next iteration.
        /// </summary>
        /// <param name="request">Manual stop request (used for logging).</param>
        /// <param name="state">Mutable runtime time state.</param>
        public void Stop(StopContinueRequest request, TimeRuntimeState state)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[ContinueOrchestrator] Stop called with null state.");
                return;
            }

            state.IsAdvancing = false;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContinueOrchestrator] Manual stop requested. Reason: {request?.Reason ?? "none"}");
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Evaluates whether a given InterruptionRequest matches the active InterruptionFilter.
        /// </summary>
        private static bool ShouldInterrupt(InterruptionRequest request, InterruptionFilter filter)
        {
            return filter switch
            {
                InterruptionFilter.CriticalOnly         => IsCritical(request.Type),
                InterruptionFilter.ImportantAndCritical => IsCritical(request.Type) || IsImportant(request.Type),
                InterruptionFilter.All                  => true,
                InterruptionFilter.Custom               => true, // Full custom logic deferred to a later phase.
                _                                       => false
            };
        }

        /// <summary>
        /// Returns true for interruption types classified as Critical.
        /// Critical: immediate threats requiring player attention.
        /// </summary>
        private static bool IsCritical(InterruptionType type)
        {
            return type is InterruptionType.LowRunway
                       or InterruptionType.InfrastructureIncident
                       or InterruptionType.MajorDefect
                       or InterruptionType.TeamMoraleCrisis;
        }

        /// <summary>
        /// Returns true for interruption types classified as Important (but not Critical).
        /// Important: milestone or deadline events that warrant player review.
        /// </summary>
        private static bool IsImportant(InterruptionType type)
        {
            return type is InterruptionType.ProductPhaseComplete
                       or InterruptionType.ProductReadyForLaunch
                       or InterruptionType.ContractDeadline
                       or InterruptionType.MonthlyFinanceReport
                       or InterruptionType.CompetitorLaunch
                       or InterruptionType.ResearchComplete
                       or InterruptionType.ProductLaunchDay;
        }
    }
}
