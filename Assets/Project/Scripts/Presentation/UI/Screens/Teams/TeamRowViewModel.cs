namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Pure display-data class for a single row in the Teams management table.
    /// Immutable after construction. No Unity dependencies.
    /// All string fields are pre-formatted and display-ready.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success", "muted".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamRowViewModel
    {
        // ── Identity ─────────────────────────────────────────────────────────

        /// <summary>Stable team identifier used for selection and detail navigation.</summary>
        public string Id { get; }

        /// <summary>Display name of the team, e.g. "Core Software".</summary>
        public string TeamName { get; }

        /// <summary>Function label, e.g. "Core Software", "QA &amp; Reliability", "Marketing".</summary>
        public string Function { get; }

        // ── Composition ──────────────────────────────────────────────────────

        /// <summary>Pre-formatted member count string, e.g. "5 members".</summary>
        public string Members { get; }

        /// <summary>Display name of the team lead. Empty string if no lead is assigned.</summary>
        public string Lead { get; }

        // ── Assignment and capacity ──────────────────────────────────────────

        /// <summary>Display name of the current assignment, e.g. "Project Alpha". Empty string if unassigned.</summary>
        public string CurrentAssignment { get; }

        /// <summary>Pre-formatted capacity string, e.g. "4 / 5 slots" or "80%".</summary>
        public string Capacity { get; }

        // ── Health metrics ───────────────────────────────────────────────────

        /// <summary>Pre-formatted workload string, e.g. "High" or "75%". Reflects aggregate team workload.</summary>
        public string Workload { get; }

        /// <summary>Pre-formatted morale string, e.g. "82%" or "Good".</summary>
        public string Morale { get; }

        /// <summary>Pre-formatted cohesion string, e.g. "70%" or "Fair".</summary>
        public string Cohesion { get; }

        // ── Gap summary ──────────────────────────────────────────────────────

        /// <summary>Pre-formatted role gap count or summary, e.g. "2 gaps" or "None".</summary>
        public string RoleGaps { get; }

        // ── Status ───────────────────────────────────────────────────────────

        /// <summary>Human-readable team status label, e.g. "Available", "Overloaded", "At Capacity".</summary>
        public string Status { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when clicking this row opens the team detail drawer.</summary>
        public bool IsClickable { get; }

        public TeamRowViewModel(
            string id,
            string teamName,
            string function,
            string members,
            string lead,
            string currentAssignment,
            string capacity,
            string workload,
            string morale,
            string cohesion,
            string roleGaps,
            string status,
            string semanticState,
            bool isClickable)
        {
            Id                = id;
            TeamName          = teamName;
            Function          = function;
            Members           = members;
            Lead              = lead;
            CurrentAssignment = currentAssignment;
            Capacity          = capacity;
            Workload          = workload;
            Morale            = morale;
            Cohesion          = cohesion;
            RoleGaps          = roleGaps;
            Status            = status;
            SemanticState     = semanticState;
            IsClickable       = isClickable;
        }
    }
}
