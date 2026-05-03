using System;
using Project.Core.Debugging;
using Project.Core.Events.Event;
using Project.Core.Events.Time;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;

namespace Project.Application.UseCases.SaveLoad
{
    /// <summary>
    /// Coordinates autosave triggers from both event-driven and explicit paths.
    /// Subscribes to domain events via IEventBus and exposes an explicit trigger method
    /// for use cases and the application quit hook.
    ///
    /// Autosave failure must never crash the simulation — all handlers catch and log exceptions.
    /// Not a MonoBehaviour. Call Initialize() after construction and Dispose() on teardown.
    /// </summary>
    public sealed class AutosaveCoordinator : IDisposable
    {
        private readonly ISaveSlotManager          _slotManager;
        private readonly IEventBus                 _eventBus;
        private readonly Func<GameSessionState>    _sessionProvider;

        private bool _initialized;
        private bool _disposed;

        // ─── Event handlers stored as fields so they can be unsubscribed ──────────

        private readonly Action<TimeAdvancedEvent>   _onTimeAdvanced;
        private readonly Action<CrisisResolvedEvent> _onCrisisResolved;

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new AutosaveCoordinator.
        /// </summary>
        /// <param name="slotManager">Save slot manager for all save operations.</param>
        /// <param name="eventBus">Event bus for subscribing to domain events.</param>
        /// <param name="sessionProvider">Lambda returning the current GameSessionState at call time.</param>
        public AutosaveCoordinator(
            ISaveSlotManager       slotManager,
            IEventBus              eventBus,
            Func<GameSessionState> sessionProvider)
        {
            _slotManager     = slotManager     ?? throw new ArgumentNullException(nameof(slotManager));
            _eventBus        = eventBus         ?? throw new ArgumentNullException(nameof(eventBus));
            _sessionProvider = sessionProvider  ?? throw new ArgumentNullException(nameof(sessionProvider));

            // Store delegates as fields for clean unsubscription.
            _onTimeAdvanced   = HandleTimeAdvanced;
            _onCrisisResolved = HandleCrisisResolved;
        }

        // ─── Lifecycle ────────────────────────────────────────────────────────────

        /// <summary>
        /// Subscribes to domain events that trigger autosaves.
        /// Must be called once after construction.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                DebugLogger.LogWarning(DebugCategory.SaveLoad,
                    "[AutosaveCoordinator] Initialize() called more than once. Ignoring.");
                return;
            }

            _eventBus.Subscribe<TimeAdvancedEvent>(_onTimeAdvanced);
            _eventBus.Subscribe<CrisisResolvedEvent>(_onCrisisResolved);

            _initialized = true;

            DebugLogger.Log(DebugCategory.SaveLoad,
                "[AutosaveCoordinator] Initialized — subscribed to TimeAdvancedEvent and CrisisResolvedEvent.");
        }

        /// <summary>
        /// Unsubscribes from all domain events. Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _eventBus.Unsubscribe<TimeAdvancedEvent>(_onTimeAdvanced);
            _eventBus.Unsubscribe<CrisisResolvedEvent>(_onCrisisResolved);

            _disposed = true;

            DebugLogger.Log(DebugCategory.SaveLoad,
                "[AutosaveCoordinator] Disposed — unsubscribed from all events.");
        }

        // ─── Explicit trigger ─────────────────────────────────────────────────────

        /// <summary>
        /// Explicit pre-action trigger. Called directly by use cases and the quit hook.
        /// Obtains the current session via the session provider and delegates to ISaveSlotManager.
        /// </summary>
        /// <param name="reason">Human-readable reason for this autosave (e.g. "before_product_launch", "on_exit").</param>
        public SaveResult RequestPreActionAutosave(string reason)
        {
            return TriggerAutosave(reason);
        }

        // ─── Event handlers ───────────────────────────────────────────────────────

        /// <summary>
        /// Handles TimeAdvancedEvent. Triggers an autosave when a monthly boundary is crossed.
        /// Monthly boundary = PreviousDate.Month differs from NewDate.Month.
        /// </summary>
        private void HandleTimeAdvanced(TimeAdvancedEvent evt)
        {
            try
            {
                if (evt == null)
                {
                    return;
                }

                bool monthBoundaryCrossed = evt.PreviousDate.Month != evt.NewDate.Month;

                if (!monthBoundaryCrossed)
                {
                    return;
                }

                DebugLogger.Log(DebugCategory.SaveLoad,
                    $"[AutosaveCoordinator] Monthly boundary crossed " +
                    $"(Month {evt.PreviousDate.Month} → {evt.NewDate.Month}). Triggering autosave.");

                TriggerAutosave("monthly_boundary");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[AutosaveCoordinator] Exception in HandleTimeAdvanced: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles CrisisResolvedEvent. Triggers an autosave after the crisis has been resolved.
        /// </summary>
        private void HandleCrisisResolved(CrisisResolvedEvent evt)
        {
            try
            {
                if (evt == null)
                {
                    return;
                }

                DebugLogger.Log(DebugCategory.SaveLoad,
                    $"[AutosaveCoordinator] Crisis resolved (InstanceId: {evt.InstanceId}). Triggering autosave.");

                TriggerAutosave("crisis_resolved");
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[AutosaveCoordinator] Exception in HandleCrisisResolved: {ex.Message}");
            }
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Obtains the current session from the provider and delegates to ISaveSlotManager.RequestAutosave().
        /// Returns a failed result if the session provider returns null.
        /// Wraps the full operation in a try-catch to ensure autosave failure cannot crash the simulation.
        /// </summary>
        private SaveResult TriggerAutosave(string reason)
        {
            try
            {
                GameSessionState session = _sessionProvider();

                if (session == null)
                {
                    const string errorMsg = "Session provider returned null — autosave skipped.";
                    DebugLogger.LogError(DebugCategory.SaveLoad,
                        $"[AutosaveCoordinator] {errorMsg}");
                    return SaveResult.Failed(errorMsg);
                }

                SaveResult result = _slotManager.RequestAutosave(session, reason);

                if (!result.Success)
                {
                    DebugLogger.LogWarning(DebugCategory.SaveLoad,
                        $"[AutosaveCoordinator] Autosave failed (reason: {reason}): {result.FailureReason}");
                }

                return result;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Autosave threw an exception (reason: {reason}): {ex.Message}";
                DebugLogger.LogError(DebugCategory.SaveLoad,
                    $"[AutosaveCoordinator] {errorMsg}");
                return SaveResult.Failed(errorMsg);
            }
        }
    }
}
