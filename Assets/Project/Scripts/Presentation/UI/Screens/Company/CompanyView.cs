using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Company screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates identity fields, status fields, founder rows, and milestone rows.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from CompanyScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a milestone row is clicked. Argument is the milestone's DrillDownRouteId.</summary>
        public event Action<string> OnMilestoneClicked;

        /// <summary>Fired when the Edit Identity button is clicked.</summary>
        public event Action OnEditIdentityClicked;

        /// <summary>Fired when a drill-down link is activated. Argument is the target route ID.</summary>
        public event Action<string> OnDrillDownClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Identity card ───────────────────────────────────────────────────────────

        private readonly VisualElement _identityFieldList;
        private readonly Button        _editIdentityButton;

        // ─── Status card ─────────────────────────────────────────────────────────────

        private readonly VisualElement _statusCard;
        private readonly VisualElement _statusFieldList;

        // ─── Founder card ────────────────────────────────────────────────────────────

        private readonly VisualElement _founderMemberList;

        // ─── Middle section ──────────────────────────────────────────────────────────

        private readonly VisualElement _focusContent;
        private readonly VisualElement _standingContent;

        // ─── Milestones section ──────────────────────────────────────────────────────

        private readonly VisualElement _milestonesList;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public CompanyView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "CompanyView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
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

            // ── Identity card ─────────────────────────────────────────────────────────

            _identityFieldList  = QueryElement(root, "IdentityFieldList");
            _editIdentityButton = root.Q<Button>("EditIdentityButton");

            LogIfNull(_editIdentityButton, "EditIdentityButton");

            if (_editIdentityButton != null)
            {
                _editIdentityButton.clicked += () => OnEditIdentityClicked?.Invoke();
            }

            // ── Status card ───────────────────────────────────────────────────────────

            _statusCard      = QueryElement(root, "StatusCard");
            _statusFieldList = QueryElement(root, "StatusFieldList");

            // ── Founder card ──────────────────────────────────────────────────────────

            _founderMemberList = QueryElement(root, "FounderMemberList");

            // ── Middle section ────────────────────────────────────────────────────────

            _focusContent    = QueryElement(root, "FocusContent");
            _standingContent = QueryElement(root, "StandingContent");

            // ── Milestones section ────────────────────────────────────────────────────

            _milestonesList = QueryElement(root, "MilestonesList");

            // ── Error / empty text ────────────────────────────────────────────────────

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
        /// Clears and rebuilds all dynamic field/row lists on each call.
        /// </summary>
        public void Bind(CompanyViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "CompanyView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No company data available.");
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

            bool hasContent = viewModel.Identity != null;

            if (!hasContent)
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

            // Identity card
            BuildIdentityFields(viewModel.Identity);

            // Status card
            BuildStatusFields(viewModel.Status);

            // Founder card
            BuildFounderMembers(viewModel.FounderSummary);

            // Focus panel (identity/preference only — no mechanical bonus per GDD Section 14)
            BuildFocusContent(viewModel.Identity);

            // Standing panel
            BuildStandingContent(viewModel.Status);

            // Milestones list
            BuildMilestonesList(viewModel.Milestones);

            // Root warning states
            ApplyRootWarningClasses(viewModel);
        }

        // ─── Private — field builders ─────────────────────────────────────────────────

        private void BuildIdentityFields(CompanyIdentityViewModel identity)
        {
            if (_identityFieldList == null || identity == null)
            {
                return;
            }

            _identityFieldList.Clear();

            AddFieldRow(_identityFieldList, "Company Name",  identity.CompanyName);
            AddFieldRow(_identityFieldList, "Founded",       identity.FoundedDate);
            AddFieldRow(_identityFieldList, "Headquarters",  identity.Headquarters);
            AddFieldRow(_identityFieldList, "Starting Focus", identity.StartingFocus);
            AddFieldRow(_identityFieldList, "Current Focus",  identity.CurrentFocus);
        }

        private void BuildStatusFields(CompanyStatusCardViewModel status)
        {
            if (_statusFieldList == null || status == null)
            {
                return;
            }

            _statusFieldList.Clear();

            // Apply semantic state class to the card container.
            if (_statusCard != null)
            {
                RemoveAllSemanticStateClasses(_statusCard);
                ApplySemanticStateClass(_statusCard, status.SemanticState);
            }

            AddFieldRow(_statusFieldList, "Reputation",       status.ReputationSummary);
            AddFieldRow(_statusFieldList, "Stage",            status.CompanyStage);
            AddFieldRow(_statusFieldList, "Health",           status.HealthState);
            AddFieldRow(_statusFieldList, "Major Milestones", status.MajorMilestonesCount);

            // Products drill-down row
            VisualElement productsRow = CreateDrillDownRow(
                label:   "Active Products",
                value:   status.ActiveProductCount,
                routeId: status.ProductsDrillDownRouteId);
            _statusFieldList.Add(productsRow);

            // Employees drill-down row
            VisualElement employeesRow = CreateDrillDownRow(
                label:   "Employees",
                value:   status.EmployeeCount,
                routeId: status.EmployeesDrillDownRouteId);
            _statusFieldList.Add(employeesRow);
        }

        private void BuildFounderMembers(CompanyFounderSummaryViewModel founderSummary)
        {
            if (_founderMemberList == null || founderSummary == null)
            {
                return;
            }

            _founderMemberList.Clear();

            // Primary founder row
            VisualElement primaryRow = CreateFounderRow(
                name:       founderSummary.FounderName,
                title:      founderSummary.FounderTitle,
                background: founderSummary.FounderBackground,
                routeId:    founderSummary.DrillDownRouteId,
                isClickable: founderSummary.IsClickable);
            _founderMemberList.Add(primaryRow);

            // Additional founding team members
            if (founderSummary.HasFoundingTeam && founderSummary.FoundingTeamMembers != null)
            {
                foreach (string memberName in founderSummary.FoundingTeamMembers)
                {
                    VisualElement memberRow = CreateFounderRow(
                        name:        memberName,
                        title:       string.Empty,
                        background:  string.Empty,
                        routeId:     founderSummary.DrillDownRouteId,
                        isClickable: false);
                    _founderMemberList.Add(memberRow);
                }
            }
        }

        private void BuildFocusContent(CompanyIdentityViewModel identity)
        {
            if (_focusContent == null || identity == null)
            {
                return;
            }

            _focusContent.Clear();

            // Focus is identity/preference display only — no mechanical bonus (GDD Section 14 lock).
            var focusLabel = new Label("Starting Focus: " + identity.StartingFocus);
            focusLabel.AddToClassList("company__focus-item");
            focusLabel.AddToClassList("text-body");

            var currentFocusLabel = new Label("Current Focus: " + identity.CurrentFocus);
            currentFocusLabel.AddToClassList("company__focus-item");
            currentFocusLabel.AddToClassList("text-body");

            var disclaimerLabel = new Label("[Placeholder] Focus reflects company identity only — no mechanical bonus applied.");
            disclaimerLabel.AddToClassList("company__focus-item");
            disclaimerLabel.AddToClassList("text-caption");
            disclaimerLabel.AddToClassList("text-muted");

            _focusContent.Add(focusLabel);
            _focusContent.Add(currentFocusLabel);
            _focusContent.Add(disclaimerLabel);
        }

        private void BuildStandingContent(CompanyStatusCardViewModel status)
        {
            if (_standingContent == null || status == null)
            {
                return;
            }

            _standingContent.Clear();

            AddFieldRow(_standingContent, "Reputation", status.ReputationSummary);
            AddFieldRow(_standingContent, "Stage",      status.CompanyStage);
            AddFieldRow(_standingContent, "Health",     status.HealthState);
        }

        private void BuildMilestonesList(IReadOnlyList<CompanyMilestoneRowViewModel> milestones)
        {
            if (_milestonesList == null)
            {
                return;
            }

            _milestonesList.Clear();

            if (milestones == null || milestones.Count == 0)
            {
                var emptyLabel = new Label("[Placeholder] No milestones recorded yet.");
                emptyLabel.AddToClassList("text-body");
                emptyLabel.AddToClassList("text-muted");
                _milestonesList.Add(emptyLabel);
                return;
            }

            foreach (CompanyMilestoneRowViewModel milestone in milestones)
            {
                VisualElement rowEl = CreateMilestoneRow(milestone);
                _milestonesList.Add(rowEl);
            }
        }

        // ─── Private — element creators ───────────────────────────────────────────────

        private static void AddFieldRow(VisualElement container, string label, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("company__field-row");

            var labelEl = new Label(label);
            labelEl.AddToClassList("company__field-label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(string.IsNullOrEmpty(value) ? "—" : value);
            valueEl.AddToClassList("company__field-value");
            valueEl.AddToClassList("text-body");

            row.Add(labelEl);
            row.Add(valueEl);

            container.Add(row);
        }

        private VisualElement CreateDrillDownRow(string label, string value, string routeId)
        {
            var row = new VisualElement();
            row.AddToClassList("company__field-row");

            if (!string.IsNullOrEmpty(routeId))
            {
                row.AddToClassList("is-clickable");
            }

            var labelEl = new Label(label);
            labelEl.AddToClassList("company__field-label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(string.IsNullOrEmpty(value) ? "—" : value);
            valueEl.AddToClassList("company__field-value");
            valueEl.AddToClassList("text-body");

            row.Add(labelEl);
            row.Add(valueEl);

            if (!string.IsNullOrEmpty(routeId))
            {
                string capturedRouteId = routeId;
                row.RegisterCallback<ClickEvent>(_ => OnDrillDownClicked?.Invoke(capturedRouteId));
            }

            return row;
        }

        private VisualElement CreateFounderRow(
            string name,
            string title,
            string background,
            string routeId,
            bool   isClickable)
        {
            var row = new VisualElement();
            row.AddToClassList("company__founder-row");

            if (isClickable)
            {
                row.AddToClassList("is-clickable");
            }

            var nameEl = new Label(name);
            nameEl.AddToClassList("company__founder-name");
            nameEl.AddToClassList("text-body");

            row.Add(nameEl);

            if (!string.IsNullOrEmpty(title))
            {
                var titleEl = new Label(title);
                titleEl.AddToClassList("company__founder-title");
                titleEl.AddToClassList("text-caption");
                row.Add(titleEl);
            }

            if (!string.IsNullOrEmpty(background))
            {
                var bgEl = new Label(background);
                bgEl.AddToClassList("company__founder-background");
                bgEl.AddToClassList("text-caption");
                bgEl.AddToClassList("text-muted");
                row.Add(bgEl);
            }

            if (isClickable && !string.IsNullOrEmpty(routeId))
            {
                string capturedRouteId = routeId;
                row.RegisterCallback<ClickEvent>(_ => OnDrillDownClicked?.Invoke(capturedRouteId));
            }

            return row;
        }

        private VisualElement CreateMilestoneRow(CompanyMilestoneRowViewModel milestone)
        {
            var row = new VisualElement();
            row.AddToClassList("company__milestone-row");
            ApplySemanticStateClass(row, milestone.SemanticState);

            if (milestone.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            var dateEl = new Label(milestone.Date);
            dateEl.AddToClassList("company__milestone-date");
            dateEl.AddToClassList("text-caption");
            dateEl.AddToClassList("text-muted");

            var titleEl = new Label(milestone.Title);
            titleEl.AddToClassList("company__milestone-title");
            titleEl.AddToClassList("text-body");

            row.Add(dateEl);
            row.Add(titleEl);

            if (!string.IsNullOrEmpty(milestone.Description))
            {
                var descEl = new Label(milestone.Description);
                descEl.AddToClassList("company__milestone-description");
                descEl.AddToClassList("text-caption");
                row.Add(descEl);
            }

            if (milestone.IsClickable)
            {
                string milestoneId = milestone.Id;
                row.RegisterCallback<ClickEvent>(_ => OnMilestoneClicked?.Invoke(milestoneId));
            }

            return row;
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

        private void ApplyRootWarningClasses(CompanyViewModel viewModel)
        {
            if (viewModel.HasMissingProfileData || viewModel.HasUnknownReputation)
            {
                Root.AddToClassList("has-warning");
            }
            else
            {
                Root.RemoveFromClassList("has-warning");
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

        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        private static void RemoveAllSemanticStateClasses(VisualElement element)
        {
            element.RemoveFromClassList("has-warning");
            element.RemoveFromClassList("has-danger");
            element.RemoveFromClassList("has-success");
            element.RemoveFromClassList("has-info");
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
                    $"CompanyView: element '{name}' not found in UXML root.");
            }
        }
    }
}
