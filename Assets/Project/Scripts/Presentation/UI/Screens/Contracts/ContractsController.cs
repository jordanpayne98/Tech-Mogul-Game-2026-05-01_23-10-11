using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Contracts board screen.
    /// Builds a [Placeholder] static ViewModel with 5-8 contracts across all tabs,
    /// wires tab selection and row click callbacks, and calls View.Bind() to display data.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractsController
    {
        private readonly ContractsView _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        // ─── State ───────────────────────────────────────────────────────────────────

        /// <summary>Currently active tab ID. Defaults to "tab.available" on initialization.</summary>
        private string _activeTabId = "tab.available";

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public ContractsController(
            ContractsView view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnTabSelected          += HandleTabSelected;
            _view.OnContractRowClicked   += HandleContractRowClicked;
            _view.OnFiltersButtonClicked += HandleFiltersButtonClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            ContractsViewModel viewModel = BuildPlaceholderViewModel(_activeTabId);
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                $"ContractsController: initialized with tab '{_activeTabId}'. [Placeholder]");
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTabSelected(string tabId)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ContractsController: tab selected — '{tabId}'.");

            _activeTabId = tabId;

            ContractsViewModel viewModel = BuildPlaceholderViewModel(_activeTabId);
            _view.Bind(viewModel);
        }

        private void HandleContractRowClicked(string contractId)
        {
            if (string.IsNullOrEmpty(contractId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"ContractsController: contract row clicked — '{contractId}'. " +
                "Opening detail modal. [Placeholder]");

            // Phase 5: opens the generic detail modal with a placeholder message.
            // Phase 6+ will replace this with a ContractDetailModal wired to a real ContractDetailViewModel.
            _modalRouter.OpenModal(ModalIds.Detail);
        }

        private void HandleFiltersButtonClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "ContractsController: Filters button clicked. [Placeholder] Filter drawer not yet wired.");

            // [Placeholder] Filter drawer wiring is deferred to a later phase.
            // The drawer router call will be added here when the filter drawer factory is registered.
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] ContractsViewModel for Phase 5.
        /// Contains 3 available, 2 active (with progress), 1 completed, and 1 failed/expired contract.
        /// All values are hard-coded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static ContractsViewModel BuildPlaceholderViewModel(string activeTabId)
        {
            IReadOnlyList<string> visibleTabs = new List<string>
            {
                "tab.available",
                "tab.active",
                "tab.completed",
                "tab.failed_expired",
            };

            IReadOnlyList<ContractRowViewModel> allRows = BuildAllPlaceholderRows();
            IReadOnlyList<ContractRowViewModel> rows    = FilterRowsForTab(allRows, activeTabId);

            bool hasNoAvailable = FilterRowsForTab(allRows, "tab.available").Count == 0;
            bool hasNoActive    = FilterRowsForTab(allRows, "tab.active").Count    == 0;

            string emptyTitle = GetEmptyStateTitle(activeTabId);
            string emptyBody  = GetEmptyStateBody(activeTabId);

            return new ContractsViewModel(
                screenTitle:             "Contracts",
                screenSubtitle:          "Manage available, active, and completed contracts. [Placeholder]",
                isLoading:               false,
                hasError:                false,
                errorMessage:            string.Empty,
                emptyStateTitle:         emptyTitle,
                emptyStateBody:          emptyBody,
                visibleTabs:             visibleTabs,
                activeTabId:             activeTabId,
                searchText:              string.Empty,
                filterState:             BuildEmptyFilterState(),
                rows:                    rows,
                selectedContractId:      string.Empty,
                hasNoAvailableContracts: hasNoAvailable,
                hasNoActiveContracts:    hasNoActive);
        }

        /// <summary>
        /// Builds the full set of 7 [Placeholder] contract rows spanning all tabs.
        /// — 3 available, 2 active (with progress), 1 completed, 1 failed/expired.
        /// </summary>
        private static IReadOnlyList<ContractRowViewModel> BuildAllPlaceholderRows()
        {
            return new List<ContractRowViewModel>
            {
                // ── Available contracts ──────────────────────────────────────────────

                new ContractRowViewModel(
                    id:            "contract.avail.001",
                    client:        "Apex Ventures",
                    contractType:  "Software Development",
                    requiredSkills:"Backend, QA",
                    difficulty:    "Standard",
                    deadline:      "Q3 Y2",
                    payment:       "$120,000",
                    progress:      "—",
                    assignedTeam:  "—",
                    qualityTarget: "75% or higher",
                    status:        "Available",
                    semanticState: "normal",
                    isClickable:   true),

                new ContractRowViewModel(
                    id:            "contract.avail.002",
                    client:        "NovaTech Solutions",
                    contractType:  "Infrastructure Upgrade",
                    requiredSkills:"DevOps, Systems",
                    difficulty:    "Complex",
                    deadline:      "Q2 Y2",
                    payment:       "$200,000",
                    progress:      "—",
                    assignedTeam:  "—",
                    qualityTarget: "80% or higher",
                    status:        "Available",
                    semanticState: "normal",
                    isClickable:   true),

                new ContractRowViewModel(
                    id:            "contract.avail.003",
                    client:        "Meridian Group",
                    contractType:  "Data Analysis",
                    requiredSkills:"Data, Analytics",
                    difficulty:    "Expert",
                    deadline:      "Q1 Y3",
                    payment:       "$350,000",
                    progress:      "—",
                    assignedTeam:  "—",
                    qualityTarget: "90% or higher",
                    status:        "Available",
                    semanticState: "warning",
                    isClickable:   true),

                // ── Active contracts (with progress) ─────────────────────────────────

                new ContractRowViewModel(
                    id:            "contract.active.001",
                    client:        "Quantum Labs",
                    contractType:  "Software Development",
                    requiredSkills:"Frontend, Backend",
                    difficulty:    "Standard",
                    deadline:      "Q2 Y2",
                    payment:       "$95,000",
                    progress:      "60%",
                    assignedTeam:  "Alpha Squad",
                    qualityTarget: "75% or higher",
                    status:        "In Progress",
                    semanticState: "normal",
                    isClickable:   true),

                new ContractRowViewModel(
                    id:            "contract.active.002",
                    client:        "BrightPath Corp",
                    contractType:  "QA & Testing",
                    requiredSkills:"QA, Testing",
                    difficulty:    "Standard",
                    deadline:      "Q2 Y2",
                    payment:       "$65,000",
                    progress:      "35%",
                    assignedTeam:  "Beta Team",
                    qualityTarget: "80% or higher",
                    status:        "In Progress",
                    semanticState: "warning",
                    isClickable:   true),

                // ── Completed contract ───────────────────────────────────────────────

                new ContractRowViewModel(
                    id:            "contract.completed.001",
                    client:        "Stellar Dynamics",
                    contractType:  "Software Development",
                    requiredSkills:"Backend, QA",
                    difficulty:    "Standard",
                    deadline:      "Q1 Y2",
                    payment:       "$110,000",
                    progress:      "100%",
                    assignedTeam:  "Alpha Squad",
                    qualityTarget: "75% or higher",
                    status:        "Completed",
                    semanticState: "success",
                    isClickable:   true),

                // ── Failed / Expired contract ────────────────────────────────────────

                new ContractRowViewModel(
                    id:            "contract.failed.001",
                    client:        "Ironclad Partners",
                    contractType:  "Infrastructure Upgrade",
                    requiredSkills:"DevOps",
                    difficulty:    "Complex",
                    deadline:      "Q4 Y1",
                    payment:       "$180,000",
                    progress:      "40%",
                    assignedTeam:  "—",
                    qualityTarget: "80% or higher",
                    status:        "Expired",
                    semanticState: "danger",
                    isClickable:   true),
            };
        }

        /// <summary>
        /// Filters the full row list to only those matching the active tab.
        /// </summary>
        private static IReadOnlyList<ContractRowViewModel> FilterRowsForTab(
            IReadOnlyList<ContractRowViewModel> all,
            string tabId)
        {
            var result = new List<ContractRowViewModel>();

            foreach (ContractRowViewModel row in all)
            {
                bool include = tabId switch
                {
                    "tab.available"     => row.Status == "Available",
                    "tab.active"        => row.Status == "In Progress",
                    "tab.completed"     => row.Status == "Completed",
                    "tab.failed_expired"=> row.Status == "Expired" || row.Status == "Failed",
                    _                   => false,
                };

                if (include)
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private static ContractFilterViewModel BuildEmptyFilterState()
        {
            return new ContractFilterViewModel(
                typeFilter:                  string.Empty,
                difficultyFilter:            string.Empty,
                deadlineFilter:              string.Empty,
                paymentFilter:               string.Empty,
                requiredRolesFilter:         string.Empty,
                clientSegmentFilter:         string.Empty,
                reputationRequirementFilter: string.Empty,
                hasActiveFilters:            false,
                activeFilterCount:           0);
        }

        private static string GetEmptyStateTitle(string tabId)
        {
            return tabId switch
            {
                "tab.available"      => "No Available Contracts",
                "tab.active"         => "No Active Contracts",
                "tab.completed"      => "No Completed Contracts",
                "tab.failed_expired" => "No Failed or Expired Contracts",
                _                    => "No Contracts",
            };
        }

        private static string GetEmptyStateBody(string tabId)
        {
            return tabId switch
            {
                "tab.available"      => "There are no contracts available at this time.",
                "tab.active"         => "You have no active contracts in progress.",
                "tab.completed"      => "No contracts have been completed yet.",
                "tab.failed_expired" => "No contracts have failed or expired.",
                _                    => "No contract data available.",
            };
        }
    }
}
