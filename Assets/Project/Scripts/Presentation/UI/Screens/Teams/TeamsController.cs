using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Teams management screen.
    /// Builds [Placeholder] static ViewModels with 3–5 teams and wires click callbacks.
    /// Handles team row selection and Create Team placeholder action.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamsController
    {
        private readonly TeamsView       _view;
        private readonly IScreenRouter   _screenRouter;
        private readonly IModalRouter    _modalRouter;

        // ─── Constructor ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public TeamsController(
            TeamsView     view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnTeamRowClicked    += HandleTeamRowClicked;
            _view.OnCreateTeamClicked += HandleCreateTeamClicked;
        }

        // ─── Public API ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            TeamsViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                "TeamsController: initialized with [Placeholder] static ViewModel.");
        }

        // ─── Event handlers ───────────────────────────────────────────────────────────

        private void HandleTeamRowClicked(string teamId)
        {
            if (string.IsNullOrEmpty(teamId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"TeamsController: team row clicked — teamId='{teamId}'. " +
                "[Placeholder] Detail drawer wiring deferred to Phase 6+.");

            // Phase 5: placeholder — detail drawer navigation wired in Phase 6+.
        }

        private void HandleCreateTeamClicked()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "TeamsController: Create Team clicked. " +
                "[Placeholder] Create Team modal wiring deferred to Phase 6+.");

            // Phase 5: placeholder — Create Team modal wired in Phase 6+.
        }

        // ─── Private — ViewModel builder ──────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] TeamsViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// Includes 5 teams: one overloaded, one with role gaps, and three in normal state.
        /// </summary>
        private static TeamsViewModel BuildPlaceholderViewModel()
        {
            IReadOnlyList<TeamRowViewModel>         rows  = BuildPlaceholderRows();
            IReadOnlyList<TeamSummaryCardViewModel> cards = BuildPlaceholderSummaryCards(rows);

            return new TeamsViewModel(
                screenTitle:    "Teams",
                screenSubtitle: "[Placeholder] Manage team composition, workload, and assignments.",
                isLoading:      false,
                hasError:       false,
                errorMessage:   string.Empty,
                emptyStateTitle: "No Teams Yet",
                emptyStateBody:  "Create your first team to start organising work.",
                summaryCards:   cards,
                rows:           rows,
                selectedTeamId: string.Empty,
                hasNoTeams:     false,
                canCreateTeam:  false);  // Locked false in Phase 5 per implementation document Section 14.
        }

        // ─── Private — row data ───────────────────────────────────────────────────────

        private static IReadOnlyList<TeamRowViewModel> BuildPlaceholderRows()
        {
            return new List<TeamRowViewModel>
            {
                // ── Core Software — normal, assigned ─────────────────────────────────
                new TeamRowViewModel(
                    id:               "team.core_software",
                    teamName:         "Core Software",
                    function:         "Core Software",
                    members:          "6 members",
                    lead:             "Alex Mercer",
                    currentAssignment: "Project Alpha",
                    capacity:         "5 / 6 slots",
                    workload:         "High",
                    morale:           "78%",
                    cohesion:         "82%",
                    roleGaps:         "None",
                    status:           "At Capacity",
                    semanticState:    "normal",
                    isClickable:      true),

                // ── QA & Reliability — overloaded warning ─────────────────────────────
                new TeamRowViewModel(
                    id:               "team.qa_reliability",
                    teamName:         "QA & Reliability",
                    function:         "QA & Reliability",
                    members:          "3 members",
                    lead:             "Sam Torres",
                    currentAssignment: "Project Alpha",
                    capacity:         "3 / 3 slots",
                    workload:         "Critical",
                    morale:           "61%",
                    cohesion:         "70%",
                    roleGaps:         "1 gap",
                    status:           "Overloaded",
                    semanticState:    "danger",
                    isClickable:      true),

                // ── Marketing — role gaps present ─────────────────────────────────────
                new TeamRowViewModel(
                    id:               "team.marketing",
                    teamName:         "Marketing",
                    function:         "Marketing",
                    members:          "2 members",
                    lead:             "—",
                    currentAssignment: "Brand Launch",
                    capacity:         "2 / 4 slots",
                    workload:         "Moderate",
                    morale:           "72%",
                    cohesion:         "65%",
                    roleGaps:         "2 gaps",
                    status:           "Missing Lead",
                    semanticState:    "warning",
                    isClickable:      true),

                // ── Infrastructure — available ────────────────────────────────────────
                new TeamRowViewModel(
                    id:               "team.infrastructure",
                    teamName:         "Infrastructure",
                    function:         "Infrastructure",
                    members:          "4 members",
                    lead:             "Jordan Kim",
                    currentAssignment: "—",
                    capacity:         "2 / 4 slots",
                    workload:         "Low",
                    morale:           "85%",
                    cohesion:         "88%",
                    roleGaps:         "None",
                    status:           "Available",
                    semanticState:    "success",
                    isClickable:      true),

                // ── Research — normal ─────────────────────────────────────────────────
                new TeamRowViewModel(
                    id:               "team.research",
                    teamName:         "Research",
                    function:         "Research",
                    members:          "3 members",
                    lead:             "Morgan Lee",
                    currentAssignment: "Tech Roadmap Study",
                    capacity:         "3 / 4 slots",
                    workload:         "Moderate",
                    morale:           "80%",
                    cohesion:         "75%",
                    roleGaps:         "None",
                    status:           "Active",
                    semanticState:    "normal",
                    isClickable:      true),
            };
        }

        // ─── Private — summary card data ──────────────────────────────────────────────

        private static IReadOnlyList<TeamSummaryCardViewModel> BuildPlaceholderSummaryCards(
            IReadOnlyList<TeamRowViewModel> rows)
        {
            // Derive counts from placeholder rows so the cards reflect the table data.
            int totalTeams  = rows?.Count ?? 0;
            int available   = 0;
            int overloaded  = 0;
            int roleGapTeams = 0;

            if (rows != null)
            {
                foreach (TeamRowViewModel row in rows)
                {
                    if (row.SemanticState == "success")   available++;
                    if (row.SemanticState == "danger")    overloaded++;
                    if (row.RoleGaps != "None")           roleGapTeams++;
                }
            }

            return new List<TeamSummaryCardViewModel>
            {
                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.TotalTeams,
                    label:        "Total Teams",
                    value:        totalTeams.ToString(),
                    semanticState: "normal"),

                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.Available,
                    label:        "Available Teams",
                    value:        available.ToString(),
                    semanticState: available > 0 ? "success" : "muted"),

                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.Overloaded,
                    label:        "Overloaded Teams",
                    value:        overloaded.ToString(),
                    semanticState: overloaded > 0 ? "danger" : "normal"),

                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.AvgMorale,
                    label:        "Avg Morale",
                    value:        "75%",   // [Placeholder] — derived from static data
                    semanticState: "normal"),

                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.AvgCohesion,
                    label:        "Avg Cohesion",
                    value:        "76%",   // [Placeholder] — derived from static data
                    semanticState: "normal"),

                new TeamSummaryCardViewModel(
                    id:           TeamSummaryCardIds.RoleGaps,
                    label:        "Open Role Gaps",
                    value:        roleGapTeams.ToString(),
                    semanticState: roleGapTeams > 0 ? "warning" : "normal"),
            };
        }
    }
}
