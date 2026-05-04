using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Employees roster screen.
    /// Builds a [Placeholder] static ViewModel with 8-10 demo employees, wires tab/search/filter callbacks,
    /// and routes employee row clicks to the modal router and filter button clicks to the drawer router.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeesController
    {
        private readonly EmployeesView     _view;
        private readonly IScreenRouter     _screenRouter;
        private readonly IModalRouter      _modalRouter;
        private readonly IRightDrawerRouter _drawerRouter;

        // ── Internal mutable display state ───────────────────────────────────────────

        private string _activeTabId   = EmployeesTabIds.All;
        private string _searchText    = string.Empty;
        private string _selectedId    = string.Empty;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public EmployeesController(
            EmployeesView      view,
            IScreenRouter      screenRouter,
            IModalRouter       modalRouter,
            IRightDrawerRouter drawerRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;
            _drawerRouter = drawerRouter;

            _view.OnTabSelected         += HandleTabSelected;
            _view.OnEmployeeRowClicked  += HandleEmployeeRowClicked;
            _view.OnFiltersButtonClicked += HandleFiltersButtonClicked;
            _view.OnSearchChanged       += HandleSearchChanged;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            DebugLogger.Log(DebugCategory.UI,
                "EmployeesController.Initialize: binding placeholder ViewModel.");

            EmployeesViewModel viewModel = BuildViewModel();
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTabSelected(string tabId)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"EmployeesController: tab selected — '{tabId}'.");

            _activeTabId = tabId;
            _selectedId  = string.Empty;

            EmployeesViewModel viewModel = BuildViewModel();
            _view.Bind(viewModel);
        }

        private void HandleEmployeeRowClicked(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"EmployeesController: employee row clicked — '{employeeId}'. Opening profile modal.");

            _selectedId = employeeId;

            // Open the employee profile modal. Phase 5 uses the generic detail modal as a placeholder.
            // Phase 6+ will register modal.employee_profile with a fully wired EmployeeProfileView.
            _modalRouter.OpenModal(ModalIds.Detail);

            // Refresh selection highlight.
            EmployeesViewModel viewModel = BuildViewModel();
            _view.Bind(viewModel);
        }

        private void HandleFiltersButtonClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "EmployeesController: filters button clicked. Opening filter drawer.");

            _drawerRouter.OpenDrawer(DrawerIds.Filter);
        }

        private void HandleSearchChanged(string searchText)
        {
            _searchText = searchText ?? string.Empty;

            DebugLogger.Log(DebugCategory.UI,
                $"EmployeesController: search changed — '{_searchText}'.");

            EmployeesViewModel viewModel = BuildViewModel();
            _view.Bind(viewModel);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] EmployeesViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// Applies tab and search filtering on the static placeholder list.
        /// </summary>
        private EmployeesViewModel BuildViewModel()
        {
            IReadOnlyList<EmployeeRowViewModel> allRows = BuildAllPlaceholderRows();
            IReadOnlyList<EmployeeRowViewModel> filteredRows = ApplyTabFilter(allRows, _activeTabId);
            filteredRows = ApplySearchFilter(filteredRows, _searchText);

            return new EmployeesViewModel(
                screenTitle:        "Employees",
                screenSubtitle:     "Manage your team, roles, and development. [Placeholder]",
                isLoading:          false,
                hasError:           false,
                errorMessage:       string.Empty,
                emptyStateTitle:    "No Employees Yet",
                emptyStateBody:     "Start hiring to build your team.",
                visibleTabs:        BuildVisibleTabs(),
                activeTabId:        _activeTabId,
                searchText:         _searchText,
                filterState:        BuildDefaultFilterState(),
                rows:               filteredRows,
                selectedEmployeeId: _selectedId,
                hasNoEmployees:     false,
                recruitmentRouteId: ScreenIds.Recruitment);
        }

        private static IReadOnlyList<string> BuildVisibleTabs()
        {
            return new List<string>
            {
                EmployeesTabIds.All,
                EmployeesTabIds.ByTeam,
                EmployeesTabIds.Unassigned,
                EmployeesTabIds.AtRisk,
                EmployeesTabIds.Former
            };
        }

        private static EmployeeFilterViewModel BuildDefaultFilterState()
        {
            return new EmployeeFilterViewModel(
                teamFilter:           string.Empty,
                roleFilter:           string.Empty,
                seniorityFilter:      string.Empty,
                statusFilter:         string.Empty,
                moraleRangeFilter:    string.Empty,
                salaryRangeFilter:    string.Empty,
                burnoutRiskFilter:    string.Empty,
                assignmentStateFilter: string.Empty,
                hasActiveFilters:     false,
                activeFilterCount:    0);
        }

        /// <summary>
        /// Builds 9 static [Placeholder] employee rows across different roles, seniorities,
        /// teams, morale levels, and statuses. Includes 2 at-risk and 1 former employee.
        /// </summary>
        private static IReadOnlyList<EmployeeRowViewModel> BuildAllPlaceholderRows()
        {
            return new List<EmployeeRowViewModel>
            {
                // ── Active employees ─────────────────────────────────────────────────

                new EmployeeRowViewModel(
                    id:                "employee.001",
                    name:              "Alex Rivera",
                    role:              "Software Engineer",
                    seniority:         "Senior",
                    team:              "Core Platform",
                    currentAssignment: "Project Alpha",
                    salary:            "$95,000 / yr",
                    morale:            "82",
                    burnoutRisk:       "Low",
                    ability:           "78",
                    potentialRange:    "80–90",
                    status:            "Active",
                    startDate:         "Mar 2023",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                new EmployeeRowViewModel(
                    id:                "employee.002",
                    name:              "Jordan Lee",
                    role:              "UI Designer",
                    seniority:         "Mid",
                    team:              "Product Design",
                    currentAssignment: "Project Alpha",
                    salary:            "$74,000 / yr",
                    morale:            "76",
                    burnoutRisk:       "Low",
                    ability:           "65",
                    potentialRange:    "70–80",
                    status:            "Active",
                    startDate:         "Jun 2023",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                new EmployeeRowViewModel(
                    id:                "employee.003",
                    name:              "Sam Okafor",
                    role:              "QA Engineer",
                    seniority:         "Junior",
                    team:              "Quality",
                    currentAssignment: "Project Alpha",
                    salary:            "$58,000 / yr",
                    morale:            "69",
                    burnoutRisk:       "Medium",
                    ability:           "52",
                    potentialRange:    "60–75",
                    status:            "Active",
                    startDate:         "Jan 2024",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                new EmployeeRowViewModel(
                    id:                "employee.004",
                    name:              "Morgan Chen",
                    role:              "Marketing Specialist",
                    seniority:         "Mid",
                    team:              "Growth",
                    currentAssignment: "Beta Launch Campaign",
                    salary:            "$68,000 / yr",
                    morale:            "80",
                    burnoutRisk:       "Low",
                    ability:           "63",
                    potentialRange:    "65–78",
                    status:            "Active",
                    startDate:         "Sep 2023",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                new EmployeeRowViewModel(
                    id:                "employee.005",
                    name:              "Riley Park",
                    role:              "Software Engineer",
                    seniority:         "Lead",
                    team:              "Core Platform",
                    currentAssignment: "Infrastructure Upgrade",
                    salary:            "$112,000 / yr",
                    morale:            "74",
                    burnoutRisk:       "Medium",
                    ability:           "88",
                    potentialRange:    "85–95",
                    status:            "Active",
                    startDate:         "Nov 2022",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                // ── At-risk employees ────────────────────────────────────────────────

                new EmployeeRowViewModel(
                    id:                "employee.006",
                    name:              "Casey Martinez",
                    role:              "Software Engineer",
                    seniority:         "Mid",
                    team:              "Core Platform",
                    currentAssignment: "Project Alpha",
                    salary:            "$82,000 / yr",
                    morale:            "38",
                    burnoutRisk:       "High",
                    ability:           "70",
                    potentialRange:    "72–82",
                    status:            "Active",
                    startDate:         "Apr 2023",
                    semanticState:     "at-risk",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                new EmployeeRowViewModel(
                    id:                "employee.007",
                    name:              "Dakota Webb",
                    role:              "QA Engineer",
                    seniority:         "Senior",
                    team:              "Quality",
                    currentAssignment: "—",
                    salary:            "$78,000 / yr",
                    morale:            "31",
                    burnoutRisk:       "High",
                    ability:           "66",
                    potentialRange:    "68–78",
                    status:            "Active",
                    startDate:         "Aug 2022",
                    semanticState:     "at-risk",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                // ── Unassigned employee ──────────────────────────────────────────────

                new EmployeeRowViewModel(
                    id:                "employee.008",
                    name:              "Taylor Brooks",
                    role:              "UX Researcher",
                    seniority:         "Junior",
                    team:              "Product Design",
                    currentAssignment: "—",
                    salary:            "$55,000 / yr",
                    morale:            "71",
                    burnoutRisk:       "Low",
                    ability:           "48",
                    potentialRange:    "55–70",
                    status:            "Active",
                    startDate:         "Feb 2024",
                    semanticState:     "normal",
                    isClickable:       true,
                    drillDownRouteId:  ModalIds.Detail),

                // ── Former employee ──────────────────────────────────────────────────

                new EmployeeRowViewModel(
                    id:                "employee.009",
                    name:              "Quinn Torres",
                    role:              "Marketing Lead",
                    seniority:         "Senior",
                    team:              "Growth",
                    currentAssignment: "—",
                    salary:            "—",
                    morale:            "—",
                    burnoutRisk:       "—",
                    ability:           "72",
                    potentialRange:    "70–80",
                    status:            "Former",
                    startDate:         "Jan 2023",
                    semanticState:     "former",
                    isClickable:       false,
                    drillDownRouteId:  string.Empty),
            };
        }

        // ─── Private — filter helpers ─────────────────────────────────────────────────

        private static IReadOnlyList<EmployeeRowViewModel> ApplyTabFilter(
            IReadOnlyList<EmployeeRowViewModel> rows, string tabId)
        {
            if (rows == null)
            {
                return new List<EmployeeRowViewModel>();
            }

            var result = new List<EmployeeRowViewModel>();

            foreach (EmployeeRowViewModel row in rows)
            {
                if (RowMatchesTab(row, tabId))
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private static bool RowMatchesTab(EmployeeRowViewModel row, string tabId)
        {
            switch (tabId)
            {
                case EmployeesTabIds.All:
                    return row.SemanticState != "former";

                case EmployeesTabIds.ByTeam:
                    return row.SemanticState != "former";

                case EmployeesTabIds.Unassigned:
                    return row.CurrentAssignment == "—" && row.SemanticState != "former";

                case EmployeesTabIds.AtRisk:
                    return row.SemanticState == "at-risk";

                case EmployeesTabIds.Former:
                    return row.SemanticState == "former";

                default:
                    return true;
            }
        }

        private static IReadOnlyList<EmployeeRowViewModel> ApplySearchFilter(
            IReadOnlyList<EmployeeRowViewModel> rows, string searchText)
        {
            if (rows == null)
            {
                return new List<EmployeeRowViewModel>();
            }

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return rows;
            }

            string lower = searchText.ToLowerInvariant();
            var result = new List<EmployeeRowViewModel>();

            foreach (EmployeeRowViewModel row in rows)
            {
                bool matches = row.Name.ToLowerInvariant().Contains(lower)
                            || row.Role.ToLowerInvariant().Contains(lower)
                            || row.Team.ToLowerInvariant().Contains(lower)
                            || row.Status.ToLowerInvariant().Contains(lower);

                if (matches)
                {
                    result.Add(row);
                }
            }

            return result;
        }
    }
}
