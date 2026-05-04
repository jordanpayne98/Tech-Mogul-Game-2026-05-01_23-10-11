using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Company screen.
    /// Builds a [Placeholder] static ViewModel, wires click callbacks to ScreenRouter/ModalRouter,
    /// and calls View.Bind() to display data.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyController
    {
        private readonly CompanyView   _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view click events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public CompanyController(
            CompanyView   view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnMilestoneClicked    += HandleMilestoneClicked;
            _view.OnEditIdentityClicked += HandleEditIdentityClicked;
            _view.OnDrillDownClicked    += HandleDrillDownClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            CompanyViewModel viewModel = BuildPlaceholderViewModel();
            _view.Bind(viewModel);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleMilestoneClicked(string milestoneId)
        {
            if (string.IsNullOrEmpty(milestoneId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"CompanyController: milestone clicked — id '{milestoneId}'. " +
                "[Placeholder] Detail modal not yet implemented.");

            // [Placeholder] Route to a milestone detail modal in Phase 6+.
            _modalRouter.OpenModal(ModalIds.Detail);
        }

        private void HandleEditIdentityClicked()
        {
            DebugLogger.Log(DebugCategory.Navigation,
                "CompanyController: Edit Identity clicked. [Placeholder] Drawer not yet implemented.");

            // [Placeholder] Open an edit identity drawer in Phase 6+.
            _screenRouter.OpenScreen(ScreenIds.Company);
        }

        private void HandleDrillDownClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"CompanyController: drill-down clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] CompanyViewModel for Phase 5.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static CompanyViewModel BuildPlaceholderViewModel()
        {
            CompanyIdentityViewModel       identity       = BuildPlaceholderIdentity();
            CompanyStatusCardViewModel     status         = BuildPlaceholderStatus();
            CompanyFounderSummaryViewModel founderSummary = BuildPlaceholderFounderSummary();
            IReadOnlyList<CompanyMilestoneRowViewModel> milestones = BuildPlaceholderMilestones();

            return new CompanyViewModel(
                screenTitle:          "Company Profile",
                screenSubtitle:       "TechCorp Alpha [Placeholder]",
                isLoading:            false,
                hasError:             false,
                errorMessage:         string.Empty,
                emptyStateTitle:      "No Company Data",
                emptyStateBody:       "Start a new company to see your company profile.",
                identity:             identity,
                status:               status,
                founderSummary:       founderSummary,
                milestones:           milestones,
                hasMilestones:        milestones.Count > 0,
                hasMissingProfileData: false,
                hasUnknownReputation: false);
        }

        private static CompanyIdentityViewModel BuildPlaceholderIdentity()
        {
            return new CompanyIdentityViewModel(
                companyName:       "TechCorp Alpha",
                companyId:         "company.techcorp_alpha",
                foundedDate:       "[Placeholder] Q1 2024",
                headquarters:      "[Placeholder] San Francisco, CA",
                logoIconClass:     "icon-company-default",
                companyColourClass: "company-colour--blue",
                startingFocus:     "[Placeholder] Consumer Software",
                currentFocus:      "[Placeholder] Enterprise SaaS",
                isEditable:        true,
                editRouteId:       ScreenIds.Company);
        }

        private static CompanyStatusCardViewModel BuildPlaceholderStatus()
        {
            return new CompanyStatusCardViewModel(
                reputationSummary:        "[Placeholder] Emerging Player",
                companyStage:             "[Placeholder] Seed Stage",
                healthState:              "[Placeholder] Stable",
                semanticState:            "normal",
                majorMilestonesCount:     "3",
                activeProductCount:       "2",
                employeeCount:            "8",
                productsDrillDownRouteId: ScreenIds.Products,
                employeesDrillDownRouteId: ScreenIds.Employees);
        }

        private static CompanyFounderSummaryViewModel BuildPlaceholderFounderSummary()
        {
            return new CompanyFounderSummaryViewModel(
                founderName:       "[Placeholder] Jordan Rivera",
                founderTitle:      "CEO & Co-founder",
                founderBackground: "[Placeholder] Former engineer at Meridian Labs",
                foundingTeamMembers: new List<string>
                {
                    "[Placeholder] Alex Chen — CTO",
                    "[Placeholder] Sam Torres — COO",
                },
                hasFoundingTeam:  true,
                drillDownRouteId: ScreenIds.Employees,
                isClickable:      true);
        }

        private static IReadOnlyList<CompanyMilestoneRowViewModel> BuildPlaceholderMilestones()
        {
            return new List<CompanyMilestoneRowViewModel>
            {
                new CompanyMilestoneRowViewModel(
                    id:              "milestone.founded",
                    title:           "[Placeholder] Company Founded",
                    date:            "[Placeholder] Day 1",
                    description:     "TechCorp Alpha officially incorporated.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),

                new CompanyMilestoneRowViewModel(
                    id:              "milestone.first_hire",
                    title:           "[Placeholder] First Hire",
                    date:            "[Placeholder] Day 7",
                    description:     "First employee joined the founding team.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),

                new CompanyMilestoneRowViewModel(
                    id:              "milestone.first_team",
                    title:           "[Placeholder] First Team Assembled",
                    date:            "[Placeholder] Day 14",
                    description:     "Core founding team of 3 people formed.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),

                new CompanyMilestoneRowViewModel(
                    id:              "milestone.first_contract",
                    title:           "[Placeholder] First Contract Signed",
                    date:            "[Placeholder] Day 21",
                    description:     "First external client contract signed.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),

                new CompanyMilestoneRowViewModel(
                    id:              "milestone.first_product",
                    title:           "[Placeholder] First Product Shipped",
                    date:            "[Placeholder] Day 30",
                    description:     "Alpha App v1.0 shipped to first customers.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),

                new CompanyMilestoneRowViewModel(
                    id:              "milestone.first_report",
                    title:           "[Placeholder] First Monthly Report",
                    date:            "[Placeholder] Day 30",
                    description:     "First monthly financial and team report generated.",
                    semanticState:   "normal",
                    isClickable:     false,
                    drillDownRouteId: string.Empty),
            };
        }
    }
}
