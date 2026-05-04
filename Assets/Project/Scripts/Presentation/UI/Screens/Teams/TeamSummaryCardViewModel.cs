namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Stable IDs for the Teams screen summary cards shown in the top row.
    /// </summary>
    public static class TeamSummaryCardIds
    {
        public const string TotalTeams   = "summary.total_teams";
        public const string Available    = "summary.available";
        public const string Overloaded   = "summary.overloaded";
        public const string AvgMorale    = "summary.avg_morale";
        public const string AvgCohesion  = "summary.avg_cohesion";
        public const string RoleGaps     = "summary.role_gaps";
    }

    /// <summary>
    /// Pure display-data class for a single summary card in the Teams screen top row.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success", "muted".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamSummaryCardViewModel
    {
        /// <summary>Stable ID matching one of <see cref="TeamSummaryCardIds"/>, e.g. "summary.total_teams".</summary>
        public string Id { get; }

        /// <summary>Display label shown beneath the value, e.g. "Total Teams".</summary>
        public string Label { get; }

        /// <summary>Pre-formatted display value, e.g. "6" or "78%".</summary>
        public string Value { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        public TeamSummaryCardViewModel(
            string id,
            string label,
            string value,
            string semanticState)
        {
            Id            = id;
            Label         = label;
            Value         = value;
            SemanticState = semanticState;
        }
    }
}
