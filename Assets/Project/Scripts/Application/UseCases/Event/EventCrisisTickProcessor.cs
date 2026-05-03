using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Event;
using Project.Core.Events.Event;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Event;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Event
{
    /// <summary>
    /// Application-layer tick processor that coordinates event evaluation, effect application,
    /// and event publishing for the three MVP game events (Plan 2M, GDD_15).
    ///
    /// Responsibility breakdown:
    ///   - Checks game day and per-event/global cooldowns before evaluating events
    ///   - Delegates evaluation to EventCrisisService (pure stateless Core service)
    ///   - Applies returned effects to runtime state (this is the Application layer's job)
    ///   - Publishes CrisisTriggeredEvent per fired event instance
    ///   - Returns InterruptionRequests for events with InterruptsContinue = true
    ///
    /// Processing rules:
    ///   - Threshold events (team morale crisis, hardware defect spike) run every day boundary
    ///   - Global cooldown does NOT gate threshold events (urgent conditions always surface)
    ///   - Per-event cooldown DOES apply to threshold events using composite key {EventId}:{TargetEntityId}
    ///   - Random events (market shock) check every EventCheckIntervalDays days
    ///   - Global cooldown DOES gate random events
    ///
    /// Basis point effect application:
    ///   Demand:      TotalDemand += TotalDemand * ValueBasisPoints / 10000 (clamped to MinDemandPerCategory)
    ///   Morale:      Morale += ValueBasisPoints / 100 (clamped to 0–100)
    ///   Defect rate: DefectRateBasisPoints += ValueBasisPoints (clamped to 0–10000)
    ///
    /// Defined in Plan 2M, GDD_15.
    /// </summary>
    public sealed class EventCrisisTickProcessor : ITickProcessor
    {
        private readonly IEventCrisisService _eventCrisisService;
        private readonly IEventCrisisTuning  _tuning;
        private readonly IMarketTuning       _marketTuning;
        private readonly IEventBus           _eventBus;
        private readonly GameSessionState    _sessionState;
        private readonly System.Random       _random;

        // ─── ITickProcessor ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public string ProcessorName => "EventCrisisTickProcessor";

        /// <inheritdoc/>
        public int ProcessingOrder => 1000;

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new EventCrisisTickProcessor.
        /// </summary>
        /// <param name="eventCrisisService">Stateless service for event eligibility evaluation.</param>
        /// <param name="tuning">Event/crisis tuning parameters.</param>
        /// <param name="marketTuning">Market tuning for demand floor clamping.</param>
        /// <param name="eventBus">Event bus for publishing CrisisTriggeredEvent.</param>
        /// <param name="sessionState">The session state to read from and write to.</param>
        /// <param name="random">Seeded random instance shared with other systems (deterministic).</param>
        public EventCrisisTickProcessor(
            IEventCrisisService eventCrisisService,
            IEventCrisisTuning  tuning,
            IMarketTuning       marketTuning,
            IEventBus           eventBus,
            GameSessionState    sessionState,
            System.Random       random)
        {
            _eventCrisisService = eventCrisisService ?? throw new ArgumentNullException(nameof(eventCrisisService));
            _tuning             = tuning             ?? throw new ArgumentNullException(nameof(tuning));
            _marketTuning       = marketTuning       ?? throw new ArgumentNullException(nameof(marketTuning));
            _eventBus           = eventBus           ?? throw new ArgumentNullException(nameof(eventBus));
            _sessionState       = sessionState       ?? throw new ArgumentNullException(nameof(sessionState));
            _random             = random             ?? throw new ArgumentNullException(nameof(random));
        }

        // ─── ITickProcessor ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            // Only process on day boundaries — events are checked at most once per day.
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            GameDateTime currentDate = context.CurrentDate;
            int currentGameDay      = ComputeGameDay(currentDate);

            var interruptions = new List<InterruptionRequest>();

            // ── Threshold events (daily, no global cooldown) ───────────────────────

            ProcessTeamMoraleCrises(currentDate, currentGameDay, interruptions);
            ProcessHardwareDefectSpikes(currentDate, currentGameDay, interruptions);

            // ── Random event check (periodic, subject to global cooldown) ─────────

            if (ShouldRunRandomEventCheck(currentDate))
            {
                _sessionState.LastEventCheckDate = currentDate;
                ProcessMarketShock(currentDate, currentGameDay, interruptions);
            }

            if (interruptions.Count > 0)
            {
                return TickResult.Succeeded(interruptions);
            }

            return TickResult.Succeeded();
        }

        // ─── Threshold event processors ───────────────────────────────────────────

        private void ProcessTeamMoraleCrises(
            GameDateTime             currentDate,
            int                      currentGameDay,
            List<InterruptionRequest> interruptions)
        {
            GameEventDefinition definition = GameEventCatalog.GetById("event.team_morale_crisis");

            if (definition == null || currentGameDay < definition.EarliestGameDay)
            {
                return;
            }

            if (_sessionState.TeamStates == null || _sessionState.TeamStates.Count == 0)
            {
                return;
            }

            List<GameEventEffect> effects = _eventCrisisService.EvaluateTeamMoraleCrises(
                _sessionState.TeamStates, _tuning);

            foreach (GameEventEffect effect in effects)
            {
                // Per-entity cooldown key: "{EventDefinitionId}:{TargetEntityId}"
                string cooldownKey = $"{definition.EventId}:{effect.TargetEntityId}";

                if (!IsCooldownClear(cooldownKey, definition.CooldownDays, currentDate))
                {
                    continue;
                }

                // Apply morale penalty: ValueBasisPoints / 100 morale points.
                var team = _sessionState.TeamStates
                    .Find(t => t.TeamId == effect.TargetEntityId);

                if (team != null)
                {
                    int moraleDelta = effect.ValueBasisPoints / 100;
                    team.Morale = Math.Clamp(team.Morale + moraleDelta, 0, 100);

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[EventCrisisTickProcessor] Applied morale penalty to team {team.TeamId}. " +
                        $"Delta: {moraleDelta}, New morale: {team.Morale}.");
                }

                // Record and publish.
                string instanceId = Guid.NewGuid().ToString();
                RecordEventFired(instanceId, definition.EventId, currentDate, new List<GameEventEffect> { effect });
                UpdateCooldowns(cooldownKey, currentDate, definition.InterruptionType == null);

                string description = $"Team morale has reached a critical level. {effect.Description}";

                _eventBus.Publish(new CrisisTriggeredEvent(
                    instanceId:        instanceId,
                    eventDefinitionId: definition.EventId,
                    description:       description,
                    category:          definition.Category,
                    severity:          definition.Severity,
                    interruptionType:  definition.InterruptionType,
                    appliedEffects:    new List<GameEventEffect> { effect }));

                if (definition.InterruptsContinue && definition.InterruptionType.HasValue)
                {
                    interruptions.Add(new InterruptionRequest(
                        type:           definition.InterruptionType.Value,
                        sourceEntityId: effect.TargetEntityId,
                        message:        description));
                }
            }
        }

        private void ProcessHardwareDefectSpikes(
            GameDateTime             currentDate,
            int                      currentGameDay,
            List<InterruptionRequest> interruptions)
        {
            GameEventDefinition definition = GameEventCatalog.GetById("event.hardware_defect_spike");

            if (definition == null || currentGameDay < definition.EarliestGameDay)
            {
                return;
            }

            if (_sessionState.HardwareMetrics == null || _sessionState.HardwareMetrics.Count == 0)
            {
                return;
            }

            List<GameEventEffect> effects = _eventCrisisService.EvaluateHardwareDefectSpikes(
                _sessionState.HardwareMetrics, _tuning);

            foreach (GameEventEffect effect in effects)
            {
                // Per-entity cooldown key: "{EventDefinitionId}:{TargetEntityId}"
                string cooldownKey = $"{definition.EventId}:{effect.TargetEntityId}";

                if (!IsCooldownClear(cooldownKey, definition.CooldownDays, currentDate))
                {
                    continue;
                }

                // Apply defect rate increase: direct basis point addition.
                var metrics = _sessionState.HardwareMetrics
                    .Find(m => m.ProductId == effect.TargetEntityId);

                if (metrics != null)
                {
                    metrics.DefectRateBasisPoints = Math.Clamp(
                        metrics.DefectRateBasisPoints + effect.ValueBasisPoints,
                        0,
                        10000);

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[EventCrisisTickProcessor] Applied defect spike to product {metrics.ProductId}. " +
                        $"Increase: {effect.ValueBasisPoints} bps, New rate: {metrics.DefectRateBasisPoints} bps.");
                }

                // Record and publish.
                string instanceId = Guid.NewGuid().ToString();
                RecordEventFired(instanceId, definition.EventId, currentDate, new List<GameEventEffect> { effect });
                UpdateCooldowns(cooldownKey, currentDate, definition.InterruptionType == null);

                string description = $"Hardware defect rate has exceeded safe levels. {effect.Description}";

                _eventBus.Publish(new CrisisTriggeredEvent(
                    instanceId:        instanceId,
                    eventDefinitionId: definition.EventId,
                    description:       description,
                    category:          definition.Category,
                    severity:          definition.Severity,
                    interruptionType:  definition.InterruptionType,
                    appliedEffects:    new List<GameEventEffect> { effect }));

                if (definition.InterruptsContinue && definition.InterruptionType.HasValue)
                {
                    interruptions.Add(new InterruptionRequest(
                        type:           definition.InterruptionType.Value,
                        sourceEntityId: effect.TargetEntityId,
                        message:        description));
                }
            }
        }

        // ─── Random event processor ───────────────────────────────────────────────

        private void ProcessMarketShock(
            GameDateTime             currentDate,
            int                      currentGameDay,
            List<InterruptionRequest> interruptions)
        {
            GameEventDefinition definition = GameEventCatalog.GetById("event.market_shock");

            if (definition == null || currentGameDay < definition.EarliestGameDay)
            {
                return;
            }

            // Check global cooldown for random events.
            if (!IsGlobalCooldownClear(definition.CooldownDays, currentDate))
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    "[EventCrisisTickProcessor] Market shock skipped: global cooldown active.");
                return;
            }

            // Check per-event cooldown (no entity target for random events).
            if (!IsCooldownClear(definition.EventId, definition.CooldownDays, currentDate))
            {
                return;
            }

            if (_sessionState.MarketCategoryStates == null || _sessionState.MarketCategoryStates.Count == 0)
            {
                return;
            }

            GameEventEffect effect = _eventCrisisService.EvaluateMarketShock(
                _sessionState.MarketCategoryStates, _random, _tuning);

            if (effect == null)
            {
                return;
            }

            // Apply demand shift: TotalDemand += TotalDemand * ValueBasisPoints / 10000
            var categoryState = _sessionState.MarketCategoryStates
                .Find(c => c.Id == effect.TargetEntityId);

            if (categoryState != null)
            {
                int shift        = categoryState.TotalDemand * effect.ValueBasisPoints / 10000;
                int newDemand    = categoryState.TotalDemand + shift;
                int clampedFloor = _marketTuning.MinDemandPerCategory;
                categoryState.TotalDemand = Math.Max(newDemand, clampedFloor);

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[EventCrisisTickProcessor] Market shock applied to category {categoryState.CategoryType}. " +
                    $"Shift: {shift}, New demand: {categoryState.TotalDemand}.");
            }

            // Record and publish.
            string instanceId = Guid.NewGuid().ToString();
            RecordEventFired(instanceId, definition.EventId, currentDate, new List<GameEventEffect> { effect });
            UpdateCooldowns(definition.EventId, currentDate, isGlobalEvent: true);

            string description = $"A sudden market shock has altered demand in the {categoryState?.CategoryType} category. " +
                                  effect.Description;

            _eventBus.Publish(new CrisisTriggeredEvent(
                instanceId:        instanceId,
                eventDefinitionId: definition.EventId,
                description:       description,
                category:          definition.Category,
                severity:          definition.Severity,
                interruptionType:  definition.InterruptionType,
                appliedEffects:    new List<GameEventEffect> { effect }));

            if (definition.InterruptsContinue && definition.InterruptionType.HasValue)
            {
                interruptions.Add(new InterruptionRequest(
                    type:           definition.InterruptionType.Value,
                    sourceEntityId: effect.TargetEntityId ?? string.Empty,
                    message:        description));
            }
        }

        // ─── Cooldown helpers ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if enough days have elapsed since the last event check to run another.
        /// </summary>
        private bool ShouldRunRandomEventCheck(GameDateTime currentDate)
        {
            if (_sessionState.LastEventCheckDate == null)
            {
                return true;
            }

            int daysSinceCheck = ComputeDaysBetween(_sessionState.LastEventCheckDate, currentDate);
            return daysSinceCheck >= _tuning.EventCheckIntervalDays;
        }

        /// <summary>
        /// Returns true if the per-event cooldown has elapsed for the given composite key.
        /// </summary>
        private bool IsCooldownClear(string cooldownKey, int cooldownDays, GameDateTime currentDate)
        {
            if (!_sessionState.LastEventDates.TryGetValue(cooldownKey, out GameDateTime lastDate))
            {
                return true;
            }

            return ComputeDaysBetween(lastDate, currentDate) >= cooldownDays;
        }

        /// <summary>
        /// Returns true if the global cooldown (minimum days between any two random events) has elapsed.
        /// </summary>
        private bool IsGlobalCooldownClear(int eventCooldownDays, GameDateTime currentDate)
        {
            if (_sessionState.LastGlobalEventDate == null)
            {
                return true;
            }

            // Use the larger of the event-specific cooldown and the global tuning cooldown.
            int cooldown = Math.Max(eventCooldownDays, _tuning.GlobalEventCooldownDays);
            return ComputeDaysBetween(_sessionState.LastGlobalEventDate, currentDate) >= cooldown;
        }

        /// <summary>
        /// Updates the per-event cooldown date and optionally the global cooldown date.
        /// </summary>
        private void UpdateCooldowns(string cooldownKey, GameDateTime currentDate, bool isGlobalEvent)
        {
            _sessionState.LastEventDates[cooldownKey] = currentDate;

            if (isGlobalEvent)
            {
                _sessionState.LastGlobalEventDate = currentDate;
            }
        }

        // ─── State helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a GameEventRuntimeState and adds it to the session event history.
        /// </summary>
        private void RecordEventFired(
            string                instanceId,
            string                eventDefinitionId,
            GameDateTime          firedDate,
            List<GameEventEffect> appliedEffects)
        {
            var runtimeState = new GameEventRuntimeState
            {
                InstanceId        = instanceId,
                EventDefinitionId = eventDefinitionId,
                FiredDate         = firedDate,
                AppliedEffects    = appliedEffects,
                Resolved          = true   // Auto-resolve for MVP
            };

            _sessionState.EventHistory.Add(runtimeState);
        }

        /// <summary>
        /// Returns the number of elapsed game days from the current session start.
        /// Session start is defined as GameDateTime.DefaultStart.
        /// </summary>
        private static int ComputeGameDay(GameDateTime currentDate)
        {
            // Game day is total elapsed days from the epoch.
            // Using TotalElapsedHours / HoursPerDay for a simple approximation.
            return currentDate.TotalElapsedHours / GameDateTime.HoursPerDay;
        }

        /// <summary>
        /// Returns the number of whole days between two GameDateTimes.
        /// </summary>
        private static int ComputeDaysBetween(GameDateTime from, GameDateTime to)
        {
            int hoursDiff = to.TotalElapsedHours - from.TotalElapsedHours;
            return hoursDiff / GameDateTime.HoursPerDay;
        }
    }
}
