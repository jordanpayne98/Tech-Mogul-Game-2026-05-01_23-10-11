using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.RecruitmentHub
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Recruitment Hub screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates tab buttons and candidate table rows with uncertainty chips.
    /// Exposes events for tab selection, candidate row clicks, shortlist toggles, and toolbar actions.
    /// Candidate information uncertainty is honoured: "???" values are rendered as uncertainty chips
    /// per Phase 5D Section 14 lock. Hidden data must not be shown as certain.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class RecruitmentHubView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from RecruitmentHubScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Events ──────────────────────────────────────────────────────────────────

        /// <summary>Fired when a tab button is clicked. Argument is the stable tab ID.</summary>
        public event Action<string> OnTabSelected;

        /// <summary>Fired when a candidate row is clicked. Argument is the stable candidate ID.</summary>
        public event Action<string> OnCandidateRowClicked;

        /// <summary>Fired when the shortlist toggle on a row is activated. Argument is the stable candidate ID.</summary>
        public event Action<string> OnShortlistToggled;

        /// <summary>Fired when the Filters toolbar button is clicked.</summary>
        public event Action OnFiltersButtonClicked;

        /// <summary>Fired when the Create Job Post toolbar button is clicked.</summary>
        public event Action OnCreateJobPostClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Tab strip ───────────────────────────────────────────────────────────────

        private readonly VisualElement _tabList;

        // ─── Table ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _tableBody;

        // ─── Toolbar buttons ─────────────────────────────────────────────────────────

        private readonly Button _filtersButton;
        private readonly Button _createJobPostButton;

        // ─── Footer ──────────────────────────────────────────────────────────────────

        private readonly Label _footerSummary;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public RecruitmentHubView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "RecruitmentHubView: root VisualElement is null. View cannot be initialized.");

                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _contentContainer = QueryElement(root, "ContentContainer");
            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");

            // ── Header ───────────────────────────────────────────────────────────────

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Tab strip ────────────────────────────────────────────────────────────

            _tabList = QueryElement(root, "TabList");

            // ── Table ────────────────────────────────────────────────────────────────

            _tableBody = QueryElement(root, "TableBody");

            // ── Toolbar buttons ──────────────────────────────────────────────────────

            _filtersButton       = root.Q<Button>("FiltersButton");
            _createJobPostButton = root.Q<Button>("CreateJobPostButton");

            LogIfNull(_filtersButton,       "FiltersButton");
            LogIfNull(_createJobPostButton, "CreateJobPostButton");

            if (_filtersButton != null)
            {
                _filtersButton.clicked += () => OnFiltersButtonClicked?.Invoke();
            }

            if (_createJobPostButton != null)
            {
                _createJobPostButton.clicked += () => OnCreateJobPostClicked?.Invoke();
            }

            // ── Footer ───────────────────────────────────────────────────────────────

            _footerSummary = root.Q<Label>("FooterSummary");
            LogIfNull(_footerSummary, "FooterSummary");

            // ── State text ───────────────────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds tab strip and candidate table on each call.
        /// </summary>
        public void Bind(RecruitmentHubViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "RecruitmentHubView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No data available.");
                return;
            }

            // ── Loading state ────────────────────────────────────────────────────────

            if (viewModel.IsLoading)
            {
                SetVisible(_loadingState,     true);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       false);
                return;
            }

            // ── Error state ──────────────────────────────────────────────────────────

            if (viewModel.HasError)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            // ── Empty state ──────────────────────────────────────────────────────────

            if (viewModel.HasNoCandidates &&
                (viewModel.Rows == null || viewModel.Rows.Count == 0))
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       true);

                if (_emptyStateTitle != null)
                {
                    _emptyStateTitle.text = viewModel.EmptyStateTitle;
                }

                if (_emptyStateBody != null)
                {
                    _emptyStateBody.text = viewModel.EmptyStateBody;
                }

                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, true);
            SetVisible(_errorState,       false);
            SetVisible(_emptyState,       false);

            // Header
            if (_headerTitle != null)
            {
                _headerTitle.text = viewModel.ScreenTitle;
            }

            if (_headerSubtitle != null)
            {
                _headerSubtitle.text = viewModel.ScreenSubtitle;
            }

            // Tab strip
            BuildTabStrip(viewModel);

            // Candidate table
            BuildCandidateTable(viewModel);

            // Footer summary
            UpdateFooterSummary(viewModel);
        }

        // ─── Private — tab strip builder ─────────────────────────────────────────────

        private void BuildTabStrip(RecruitmentHubViewModel viewModel)
        {
            if (_tabList == null)
            {
                return;
            }

            _tabList.Clear();

            if (viewModel.VisibleTabs == null)
            {
                return;
            }

            foreach (string tabId in viewModel.VisibleTabs)
            {
                string label = GetTabLabel(tabId);

                var tabBtn = new Button();
                tabBtn.text = label;
                tabBtn.AddToClassList("recruitment-hub__tab-btn");
                tabBtn.AddToClassList("text-small");

                bool isActive = tabId == viewModel.ActiveTabId;

                if (isActive)
                {
                    tabBtn.AddToClassList("is-active");
                }

                string capturedTabId = tabId;
                tabBtn.clicked += () => OnTabSelected?.Invoke(capturedTabId);

                _tabList.Add(tabBtn);
            }
        }

        private static string GetTabLabel(string tabId)
        {
            return tabId switch
            {
                RecruitmentHubTabIds.CandidatePool => "Candidate Pool",
                RecruitmentHubTabIds.Shortlist     => "Shortlist",
                RecruitmentHubTabIds.JobPosts      => "Job Posts",
                RecruitmentHubTabIds.OffersSent    => "Offers Sent",
                RecruitmentHubTabIds.Accepted      => "Accepted",
                RecruitmentHubTabIds.Rejected      => "Rejected",
                _                                  => tabId
            };
        }

        // ─── Private — candidate table builder ───────────────────────────────────────

        private void BuildCandidateTable(RecruitmentHubViewModel viewModel)
        {
            if (_tableBody == null)
            {
                return;
            }

            _tableBody.Clear();

            if (viewModel.Rows == null || viewModel.Rows.Count == 0)
            {
                var emptyRow = new Label("No candidates match the current filter.");
                emptyRow.AddToClassList("recruitment-hub__table-empty");
                emptyRow.AddToClassList("text-body");
                emptyRow.AddToClassList("text-muted");
                _tableBody.Add(emptyRow);
                return;
            }

            foreach (CandidateRowViewModel row in viewModel.Rows)
            {
                VisualElement rowEl = CreateCandidateRow(row, viewModel.SelectedCandidateId);
                _tableBody.Add(rowEl);
            }
        }

        private VisualElement CreateCandidateRow(CandidateRowViewModel row, string selectedCandidateId)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("recruitment-hub__row");

            // Semantic state class
            if (!string.IsNullOrEmpty(row.SemanticState) && row.SemanticState != "normal")
            {
                rowEl.AddToClassList($"is-{row.SemanticState}");
            }

            // Selected state
            if (row.Id == selectedCandidateId)
            {
                rowEl.AddToClassList("is-selected");
            }

            // ── Shortlist indicator strip ────────────────────────────────────────────

            var shortlistStrip = new VisualElement();
            shortlistStrip.AddToClassList("recruitment-hub__row-shortlist-strip");

            if (row.IsShortlisted)
            {
                shortlistStrip.AddToClassList("is-active");
            }

            rowEl.Add(shortlistStrip);

            // ── Name column ──────────────────────────────────────────────────────────

            var nameCell = new VisualElement();
            nameCell.AddToClassList("recruitment-hub__row-cell");
            nameCell.AddToClassList("recruitment-hub__col--name");

            var nameLabel = new Label(row.Name);
            nameLabel.AddToClassList("text-body");
            nameCell.Add(nameLabel);

            rowEl.Add(nameCell);

            // ── Role column ──────────────────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.Role, "recruitment-hub__col--role"));

            // ── Seniority column ─────────────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.Seniority, "recruitment-hub__col--seniority"));

            // ── Salary Expectation column ────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.SalaryExpectation, "recruitment-hub__col--salary"));

            // ── Visible Skills column (uncertainty chips) ────────────────────────────

            var skillsCell = new VisualElement();
            skillsCell.AddToClassList("recruitment-hub__row-cell");
            skillsCell.AddToClassList("recruitment-hub__col--skills");

            BuildSkillChips(skillsCell, row.VisibleSkills);

            rowEl.Add(skillsCell);

            // ── Potential Estimate column (may be "???") ─────────────────────────────

            rowEl.Add(CreateUncertaintyCell(row.PotentialEstimate, "recruitment-hub__col--potential"));

            // ── Availability column ──────────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.Availability, "recruitment-hub__col--availability"));

            // ── Interest column ──────────────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.Interest, "recruitment-hub__col--interest"));

            // ── Offer Status column ──────────────────────────────────────────────────

            rowEl.Add(CreateOfferStatusCell(row.OfferStatus));

            // ── Confidence column ────────────────────────────────────────────────────

            rowEl.Add(CreateTextCell(row.Confidence, "recruitment-hub__col--confidence"));

            // ── Shortlist toggle button ──────────────────────────────────────────────

            var actionsCell = new VisualElement();
            actionsCell.AddToClassList("recruitment-hub__row-cell");
            actionsCell.AddToClassList("recruitment-hub__col--actions");

            var shortlistBtn = new Button();
            shortlistBtn.text = row.IsShortlisted ? "★" : "☆";
            shortlistBtn.AddToClassList("recruitment-hub__shortlist-btn");

            if (row.IsShortlisted)
            {
                shortlistBtn.AddToClassList("is-active");
            }

            string capturedId = row.Id;
            shortlistBtn.clicked += () => OnShortlistToggled?.Invoke(capturedId);

            // Stop row-click propagation from the shortlist button.
            shortlistBtn.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            actionsCell.Add(shortlistBtn);
            rowEl.Add(actionsCell);

            // ── Row click ────────────────────────────────────────────────────────────

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
                rowEl.RegisterCallback<ClickEvent>(_ => OnCandidateRowClicked?.Invoke(capturedId));
            }

            return rowEl;
        }

        private static void BuildSkillChips(VisualElement container, string visibleSkills)
        {
            if (string.IsNullOrEmpty(visibleSkills))
            {
                return;
            }

            string[] skills = visibleSkills.Split(',');

            foreach (string raw in skills)
            {
                string skill = raw.Trim();

                if (string.IsNullOrEmpty(skill))
                {
                    continue;
                }

                var chip = new Label(skill);
                chip.AddToClassList("pill");

                // Uncertainty chip — rendered distinctly per Section 14 lock.
                if (skill == "???")
                {
                    chip.AddToClassList("pill--unknown");
                }
                else
                {
                    chip.AddToClassList("pill--info");
                }

                container.Add(chip);
            }
        }

        private static VisualElement CreateTextCell(string text, string columnClass)
        {
            var cell = new VisualElement();
            cell.AddToClassList("recruitment-hub__row-cell");
            cell.AddToClassList(columnClass);

            var label = new Label(text ?? string.Empty);
            label.AddToClassList("text-body");
            cell.Add(label);

            return cell;
        }

        private static VisualElement CreateUncertaintyCell(string text, string columnClass)
        {
            var cell = new VisualElement();
            cell.AddToClassList("recruitment-hub__row-cell");
            cell.AddToClassList(columnClass);

            var label = new Label(text ?? string.Empty);
            label.AddToClassList("text-body");

            // Uncertainty styling — per Section 14 lock, "???" must be visually distinct.
            if (text == "???")
            {
                label.AddToClassList("text-unknown");
            }

            cell.Add(label);
            return cell;
        }

        private static VisualElement CreateOfferStatusCell(string offerStatus)
        {
            var cell = new VisualElement();
            cell.AddToClassList("recruitment-hub__row-cell");
            cell.AddToClassList("recruitment-hub__col--offer-status");

            string pillClass = offerStatus switch
            {
                "Offer Sent" => "pill--warning",
                "Accepted"   => "pill--success",
                "Rejected"   => "pill--error",
                _            => "pill--neutral"
            };

            var pill = new Label(offerStatus ?? "No Offer");
            pill.AddToClassList("pill");
            pill.AddToClassList(pillClass);

            cell.Add(pill);
            return cell;
        }

        // ─── Private — footer ────────────────────────────────────────────────────────

        private void UpdateFooterSummary(RecruitmentHubViewModel viewModel)
        {
            if (_footerSummary == null)
            {
                return;
            }

            int count = viewModel.Rows?.Count ?? 0;
            _footerSummary.text = $"{count} candidate{(count == 1 ? "" : "s")} shown";
        }

        // ─── Private — state helpers ─────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_emptyState,       false);
            SetVisible(_errorState,       true);

            if (_errorMessage != null)
            {
                _errorMessage.text = message ?? string.Empty;
            }
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            if (element == null)
            {
                return;
            }

            if (visible)
            {
                element.RemoveFromClassList("is-hidden");
            }
            else
            {
                element.AddToClassList("is-hidden");
            }
        }

        // ─── Private — query helpers ─────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            LogIfNull(element, name);
            return element;
        }

        private static void LogIfNull(object element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"RecruitmentHubView: element '{name}' not found in UXML root.");
            }
        }
    }
}
