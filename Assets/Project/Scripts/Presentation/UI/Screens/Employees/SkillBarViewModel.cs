namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure display-data class for a single skill bar in the employee profile modal.
    /// Immutable after construction. No Unity dependencies.
    /// NormalizedValue (0.0–1.0) is intentionally a float to drive bar width rendering.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class SkillBarViewModel
    {
        /// <summary>Display label for the skill, e.g. "Backend Development".</summary>
        public string SkillName { get; }

        /// <summary>Pre-formatted level string, e.g. "68 / 100".</summary>
        public string Level { get; }

        /// <summary>Normalized value in range [0.0, 1.0] used to drive bar width rendering.</summary>
        public float NormalizedValue { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success". Drives USS state class.</summary>
        public string SemanticState { get; }

        public SkillBarViewModel(
            string skillName,
            string level,
            float normalizedValue,
            string semanticState)
        {
            SkillName       = skillName;
            Level           = level;
            NormalizedValue = normalizedValue;
            SemanticState   = semanticState;
        }
    }
}
