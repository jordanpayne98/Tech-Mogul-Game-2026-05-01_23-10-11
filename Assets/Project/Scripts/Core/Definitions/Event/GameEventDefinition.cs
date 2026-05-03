using Project.Core.Definitions.Time;

namespace Project.Core.Definitions.Event
{
    /// <summary>
    /// Immutable data class describing a game event template.
    /// Instances are stored in GameEventCatalog and are never modified at runtime.
    /// A fired event produces a GameEventRuntimeState that references this definition by EventId.
    /// </summary>
    public sealed class GameEventDefinition
    {
        // ─── Identity ─────────────────────────────────────────────────────────────

        /// <summary>Stable ID used for catalog lookups and cooldown tracking. Must never change.</summary>
        public string EventId { get; }

        /// <summary>Display title shown in reports and UI.</summary>
        public string Title { get; }

        /// <summary>
        /// Narrative description template. May contain placeholder tokens such as
        /// {CategoryName} and {Percent} that callers resolve when building a report.
        /// </summary>
        public string DescriptionTemplate { get; }

        // ─── Classification ───────────────────────────────────────────────────────

        /// <summary>Simulation domain this event belongs to.</summary>
        public GameEventCategory Category { get; }

        /// <summary>Urgency level used for report priority mapping.</summary>
        public GameEventSeverity Severity { get; }

        /// <summary>How this event is triggered — probabilistic or threshold-based.</summary>
        public GameEventTriggerType TriggerType { get; }

        /// <summary>The kind of state mutation this event's effects apply.</summary>
        public GameEventEffectType EffectType { get; }

        // ─── Interruption ─────────────────────────────────────────────────────────

        /// <summary>
        /// True if firing this event should pause Continue time advancement.
        /// All 3 MVP events interrupt Continue.
        /// </summary>
        public bool InterruptsContinue { get; }

        /// <summary>
        /// The interruption category to raise when this event fires.
        /// Null if InterruptsContinue is false.
        /// </summary>
        public InterruptionType? InterruptionType { get; }

        // ─── Scheduling ───────────────────────────────────────────────────────────

        /// <summary>Minimum game days that must elapse before this specific event can fire again.</summary>
        public int CooldownDays { get; }

        /// <summary>
        /// The earliest game day (0-indexed from session start) on which this event may fire.
        /// Prevents early-game spam before backing systems are populated.
        /// </summary>
        public int EarliestGameDay { get; }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates an immutable game event definition.
        /// </summary>
        public GameEventDefinition(
            string eventId,
            string title,
            string descriptionTemplate,
            GameEventCategory category,
            GameEventSeverity severity,
            GameEventTriggerType triggerType,
            GameEventEffectType effectType,
            bool interruptsContinue,
            InterruptionType? interruptionType,
            int cooldownDays,
            int earliestGameDay)
        {
            EventId             = eventId;
            Title               = title;
            DescriptionTemplate = descriptionTemplate;
            Category            = category;
            Severity            = severity;
            TriggerType         = triggerType;
            EffectType          = effectType;
            InterruptsContinue  = interruptsContinue;
            InterruptionType    = interruptionType;
            CooldownDays        = cooldownDays;
            EarliestGameDay     = earliestGameDay;
        }
    }
}
