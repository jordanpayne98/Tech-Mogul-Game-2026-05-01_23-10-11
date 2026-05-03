namespace Project.Presentation.UI.Controllers
{
    /// <summary>
    /// Immutable data record for a single wizard step configuration.
    /// Used by WizardShellView to build the stepper chip row.
    ///
    /// State values match the USS modifier class names applied to step chips:
    ///   "complete" | "active" | "available" | "locked" | "warning" | "error"
    /// </summary>
    public sealed class WizardStepData
    {
        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>Player-visible step label rendered below the step chip.</summary>
        public string Label { get; }

        /// <summary>
        /// Step visual state. Must be one of:
        /// "complete", "active", "available", "locked", "warning", "error".
        /// </summary>
        public string State { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <param name="label">Player-visible step label.</param>
        /// <param name="state">
        /// Visual state string: "complete" | "active" | "available" | "locked" | "warning" | "error".
        /// </param>
        public WizardStepData(string label, string state)
        {
            Label = label ?? string.Empty;
            State = state ?? "available";
        }
    }
}
