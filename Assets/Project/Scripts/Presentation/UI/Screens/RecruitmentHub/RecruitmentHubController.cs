using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Recruitment Hub screen.
    /// Builds a [Placeholder] static ViewModel with 6–8 mixed-role candidates,
    /// wires tab/row/shortlist/toolbar callbacks, and calls View.Bind() to display data.
    ///
    /// Candidate information uncertainty rule (Phase 5D Section 14 lock):
    /// Some candidates have hidden skill slots represented as "???" in VisibleSkills,
    /// and PotentialEstimate is "???" when unrevealed. The Controller must not mark
    /// hidden data as certain — only the ViewModel visibility flags may do so.
    ///
    /// Offer submission is Phase 6+ — CanSendOffer buttons are disabled in the View.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class RecruitmentHubController
    {
        private readonly RecruitmentHubView _view;
        private readonly IScreenRouter      _screenRouter;
        private readonly IModalRouter       _modalRouter;

        // Local shortlist state — UI-only, not persisted. Phase 6+ wires to Application layer.
        private readonly HashSet<string> _shortlistedIds = new HashSet<string>();

        // Active tab state
        private string _activeTabId = RecruitmentHubTabIds.CandidatePool;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public RecruitmentHubController(
            RecruitmentHubView view,
            IScreenRouter      screenRouter,
            IModalRouter       modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnTabSelected         += HandleTabSelected;
            _view.OnCandidateRowClicked += HandleCandidateRowClicked;
            _view.OnShortlistToggled    += HandleShortlistToggled;
            _view.OnFiltersButtonClicked += HandleFiltersButtonClicked;
            _view.OnCreateJobPostClicked += HandleCreateJobPostClicked;

            // Seed placeholder shortlisted IDs.
            _shortlistedIds.Add("candidate.0002");
            _shortlistedIds.Add("candidate.0005");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            RefreshView(selectedCandidateId: string.Empty);
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleTabSelected(string tabId)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"RecruitmentHubController: tab selected — '{tabId}'.");

            _activeTabId = tabId;
            RefreshView(selectedCandidateId: string.Empty);
        }

        private void HandleCandidateRowClicked(string candidateId)
        {
            if (string.IsNullOrEmpty(candidateId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"RecruitmentHubController: candidate row clicked — '{candidateId}'. " +
                "[Placeholder] Candidate Detail modal will open here in Phase 6+.");

            // Phase 5: open the generic detail modal as a placeholder.
            // Phase 6+ will replace with a real CandidateDetailModal factory.
            _modalRouter.OpenModal(ModalIds.Detail);
        }

        private void HandleShortlistToggled(string candidateId)
        {
            if (string.IsNullOrEmpty(candidateId))
            {
                return;
            }

            if (_shortlistedIds.Contains(candidateId))
            {
                _shortlistedIds.Remove(candidateId);

                DebugLogger.Log(DebugCategory.UI,
                    $"RecruitmentHubController: removed '{candidateId}' from shortlist. [Placeholder] local UI state only.");
            }
            else
            {
                _shortlistedIds.Add(candidateId);

                DebugLogger.Log(DebugCategory.UI,
                    $"RecruitmentHubController: added '{candidateId}' to shortlist. [Placeholder] local UI state only.");
            }

            RefreshView(selectedCandidateId: string.Empty);
        }

        private void HandleFiltersButtonClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "RecruitmentHubController: Filters button clicked. [Placeholder] Filter drawer will open here.");

            // Phase 5: open the generic filter drawer placeholder.
            // Phase 6+ will replace with a RecruitmentHub-specific filter drawer.
            _modalRouter.OpenModal(ModalIds.Info);
        }

        private void HandleCreateJobPostClicked()
        {
            DebugLogger.Log(DebugCategory.UI,
                "RecruitmentHubController: Create Job Post clicked. [Placeholder] Job post modal will open here.");

            // Phase 5: open the generic info modal as a placeholder.
            // Phase 6+ will replace with a real JobPostModal factory.
            _modalRouter.OpenModal(ModalIds.Info);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        private void RefreshView(string selectedCandidateId)
        {
            RecruitmentHubViewModel viewModel = BuildPlaceholderViewModel(selectedCandidateId);
            _view.Bind(viewModel);
        }

        private RecruitmentHubViewModel BuildPlaceholderViewModel(string selectedCandidateId)
        {
            IReadOnlyList<CandidateRowViewModel> rows = BuildPlaceholderCandidates();

            return new RecruitmentHubViewModel(
                screenTitle:         "Recruitment Hub",
                screenSubtitle:      "[Placeholder] Candidate pool — Phase 5 static data",
                isLoading:           false,
                hasError:            false,
                errorMessage:        string.Empty,
                emptyStateTitle:     "No Candidates Available",
                emptyStateBody:      "Post a job to begin receiving candidates.",
                visibleTabs:         BuildVisibleTabs(),
                activeTabId:         _activeTabId,
                searchText:          string.Empty,
                filterState:         BuildEmptyFilterState(),
                rows:                rows,
                jobPosts:            new List<JobPostRowViewModel>(),
                selectedCandidateId: selectedCandidateId,
                hasNoCandidates:     false);
        }

        private static IReadOnlyList<string> BuildVisibleTabs()
        {
            return new List<string>
            {
                RecruitmentHubTabIds.CandidatePool,
                RecruitmentHubTabIds.Shortlist,
                RecruitmentHubTabIds.JobPosts,
                RecruitmentHubTabIds.OffersSent,
                RecruitmentHubTabIds.Accepted,
                RecruitmentHubTabIds.Rejected,
            };
        }

        private static CandidateFilterViewModel BuildEmptyFilterState()
        {
            return new CandidateFilterViewModel(
                roleFilter:            string.Empty,
                seniorityFilter:       string.Empty,
                salaryRangeFilter:     string.Empty,
                availabilityFilter:    string.Empty,
                interestFilter:        string.Empty,
                skillTagsFilter:       string.Empty,
                offerStatusFilter:     string.Empty,
                confidenceLevelFilter: string.Empty,
                hasActiveFilters:      false,
                activeFilterCount:     0);
        }

        private IReadOnlyList<CandidateRowViewModel> BuildPlaceholderCandidates()
        {
            // [Placeholder] 8 candidates — mixed roles, seniorities, and visibility levels.
            // Some skills are hidden ("???") per Phase 5D Section 14 lock.
            // candidate.0002 and candidate.0005 are pre-shortlisted.
            // candidate.0007 has an offer pending (offer_sent semantic state).
            // Offer submission is Phase 6+ — CanSendOffer is not exposed in Phase 5.

            return new List<CandidateRowViewModel>
            {
                new CandidateRowViewModel(
                    id:                "candidate.0001",
                    name:              "[Placeholder] Alex Mercer",
                    role:              "Software Engineer",
                    seniority:         "Senior",
                    salaryExpectation: "$135,000 / yr",
                    visibleSkills:     "Python, React, ???",
                    potentialEstimate: "High",
                    availability:      "Immediately",
                    interest:          "High",
                    offerStatus:       "No Offer",
                    confidence:        "Medium",
                    semanticState:     "normal",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0001"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0002",
                    name:              "[Placeholder] Jordan Lee",
                    role:              "Product Designer",
                    seniority:         "Mid",
                    salaryExpectation: "$95,000 / yr",
                    visibleSkills:     "Figma, UI Design",
                    potentialEstimate: "???",
                    availability:      "2 weeks",
                    interest:          "Medium",
                    offerStatus:       "No Offer",
                    confidence:        "Low",
                    semanticState:     "shortlisted",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0002"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0003",
                    name:              "[Placeholder] Sam Rivera",
                    role:              "Data Analyst",
                    seniority:         "Junior",
                    salaryExpectation: "$72,000 / yr",
                    visibleSkills:     "SQL, Tableau, ???, ???",
                    potentialEstimate: "???",
                    availability:      "1 month",
                    interest:          "Low",
                    offerStatus:       "No Offer",
                    confidence:        "Low",
                    semanticState:     "normal",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0003"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0004",
                    name:              "[Placeholder] Casey Wright",
                    role:              "DevOps Engineer",
                    seniority:         "Senior",
                    salaryExpectation: "$145,000 / yr",
                    visibleSkills:     "AWS, Terraform, Docker",
                    potentialEstimate: "High",
                    availability:      "Immediately",
                    interest:          "High",
                    offerStatus:       "No Offer",
                    confidence:        "High",
                    semanticState:     "normal",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0004"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0005",
                    name:              "[Placeholder] Morgan Blake",
                    role:              "Software Engineer",
                    seniority:         "Mid",
                    salaryExpectation: "$105,000 / yr",
                    visibleSkills:     "TypeScript, Node.js, ???",
                    potentialEstimate: "Medium",
                    availability:      "2 weeks",
                    interest:          "Medium",
                    offerStatus:       "No Offer",
                    confidence:        "Medium",
                    semanticState:     "shortlisted",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0005"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0006",
                    name:              "[Placeholder] Taylor Kim",
                    role:              "QA Engineer",
                    seniority:         "Mid",
                    salaryExpectation: "$88,000 / yr",
                    visibleSkills:     "Selenium, ???",
                    potentialEstimate: "???",
                    availability:      "1 month",
                    interest:          "High",
                    offerStatus:       "No Offer",
                    confidence:        "Low",
                    semanticState:     "normal",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0006"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0007",
                    name:              "[Placeholder] Drew Santos",
                    role:              "Product Manager",
                    seniority:         "Senior",
                    salaryExpectation: "$125,000 / yr",
                    visibleSkills:     "Roadmapping, Agile",
                    potentialEstimate: "High",
                    availability:      "1 month",
                    interest:          "Medium",
                    offerStatus:       "Offer Sent",
                    confidence:        "High",
                    semanticState:     "offer_sent",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0007"),
                    isClickable:       true),

                new CandidateRowViewModel(
                    id:                "candidate.0008",
                    name:              "[Placeholder] Avery Collins",
                    role:              "UX Researcher",
                    seniority:         "Junior",
                    salaryExpectation: "$68,000 / yr",
                    visibleSkills:     "User Testing, ???, ???",
                    potentialEstimate: "???",
                    availability:      "Immediately",
                    interest:          "Low",
                    offerStatus:       "No Offer",
                    confidence:        "Low",
                    semanticState:     "normal",
                    isShortlisted:     _shortlistedIds.Contains("candidate.0008"),
                    isClickable:       true),
            };
        }
    }
}
