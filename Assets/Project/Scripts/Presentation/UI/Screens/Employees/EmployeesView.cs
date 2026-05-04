using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Employees roster screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates tab buttons, table header, and employee rows.
    /// Exposes click/interaction events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeesView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from EmployeesScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a tab button is clicked. Argument is the stable tab ID.</summary>
        public event Action<string> OnTabSelected;

        /// <summary>Fired when an employee row is clicked. Argument is the employee's stable ID.</summary>
        public event Action<string> OnEmployeeRowClicked;

        /// <summary>Fired when the Filters toolbar button is clicked.</summary>
        public event Action OnFiltersButtonClicked;

        /// <summary>Fired when the search field value changes. Argument is the current search text.</summary>
        public event Action<string> OnSearchChanged;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _filteredEmptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Tab bar ─────────────────────────────────────────────────────────────────

        private readonly VisualElement _tabBar;

        // ─── Toolbar ─────────────────────────────────────────────────────────────────

        private readonly TextField    _searchField;
        private readonly Button       _filtersButton;
        private readonly Label        _filtersBadge;

        // ─── Table ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _tableHeader;
        private readonly VisualElement _tableBody;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;
        private readonly Label _filteredEmptyMessage;

        // ─── Tracking ────────────────────────────────────────────────────────────────

        private string _selectedEmployeeId = string.Empty;

        // ─── Column definitions ──────────────────────────────────────────────────────

        private static readonly string[] ColumnHeaders = new[]
        {
            "Name", "Role", "Seniority", "Team", "Assignment",
            "Salary", "Morale", "Burnout Risk", "Ability", "Potential",
            "Status", "Start Date", "Actions"
        };

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public EmployeesView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "EmployeesView: root VisualElement is null. View cannot be initialized.");

                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _contentContainer   = QueryElement(root, "ContentContainer");
            _loadingState       = QueryElement(root, "LoadingState");
            _errorState         = QueryElement(root, "ErrorState");
            _emptyState         = QueryElement(root, "EmptyState");
            _filteredEmptyState = QueryElement(root, "FilteredEmptyState");

            // ── Header ───────────────────────────────────────────────────────────────

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Tab bar ──────────────────────────────────────────────────────────────

            _tabBar = QueryElement(root, "TabBar");

            // ── Toolbar ──────────────────────────────────────────────────────────────

            _searchField   = root.Q<TextField>("SearchField");
            _filtersButton = root.Q<Button>("FiltersButton");
            _filtersBadge  = root.Q<Label>("FiltersBadge");

            LogIfNull(_searchField,   "SearchField");
            LogIfNull(_filtersButton, "FiltersButton");

            // ── Table ────────────────────────────────────────────────────────────────

            _tableHeader = QueryElement(root, "TableHeader");
            _tableBody   = QueryElement(root, "TableBody");

            // ── Text labels ──────────────────────────────────────────────────────────

            _errorMessage        = root.Q<Label>("ErrorMessage");
            _emptyStateTitle     = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody      = root.Q<Label>("EmptyStateBody");
            _filteredEmptyMessage = root.Q<Label>("FilteredEmptyMessage");

            LogIfNull(_errorMessage,         "ErrorMessage");
            LogIfNull(_emptyStateTitle,      "EmptyStateTitle");
            LogIfNull(_emptyStateBody,       "EmptyStateBody");

            // ── Wire toolbar callbacks ────────────────────────────────────────────────

            if (_filtersButton != null)
            {
                _filtersButton.RegisterCallback<ClickEvent>(_ => OnFiltersButtonClicked?.Invoke());
            }

            if (_searchField != null)
            {
                _searchField.RegisterValueChangedCallback(evt => OnSearchChanged?.Invoke(evt.newValue));
            }

            // ── Build static table header ─────────────────────────────────────────────

            BuildTableHeader();
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds the tab bar and table rows on each call.
        /// </summary>
        public void Bind(EmployeesViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "EmployeesView.Bind: viewModel is null. Showing error state.");
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
                SetVisible(_filteredEmptyState, false);
                return;
            }

            // ── Error state ──────────────────────────────────────────────────────────

            if (viewModel.HasError)
            {
                ShowError(viewModel.ErrorMessage);
                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_errorState,       false);
            SetVisible(_contentContainer, true);

            // Header
            if (_headerTitle != null)
            {
                _headerTitle.text = viewModel.ScreenTitle;
            }

            if (_headerSubtitle != null)
            {
                _headerSubtitle.text = viewModel.ScreenSubtitle;
            }

            // Tabs
            BuildTabBar(viewModel);

            // Search field
            if (_searchField != null && _searchField.value != viewModel.SearchText)
            {
                // Set without triggering the callback to avoid re-entrant Bind cycles.
                _searchField.SetValueWithoutNotify(viewModel.SearchText ?? string.Empty);
            }

            // Filters badge
            UpdateFiltersBadge(viewModel.FilterState);

            // Track selected row ID
            _selectedEmployeeId = viewModel.SelectedEmployeeId ?? string.Empty;

            // ── Empty state (no employees at all) ────────────────────────────────────

            if (viewModel.HasNoEmployees)
            {
                SetVisible(_emptyState, true);
                SetVisible(_filteredEmptyState, false);
                SetVisible(_tableHeader, false);
                SetVisible(_tableBody,   false);

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

            SetVisible(_emptyState, false);

            // ── Filtered empty state ──────────────────────────────────────────────────

            bool hasRows = viewModel.Rows != null && viewModel.Rows.Count > 0;

            if (!hasRows)
            {
                SetVisible(_filteredEmptyState, true);
                SetVisible(_tableHeader,        false);
                SetVisible(_tableBody,          false);

                if (_filteredEmptyMessage != null)
                {
                    _filteredEmptyMessage.text = "No employees match the current filters or search.";
                }

                return;
            }

            SetVisible(_filteredEmptyState, false);
            SetVisible(_tableHeader,        true);
            SetVisible(_tableBody,          true);

            // ── Table rows ───────────────────────────────────────────────────────────

            BuildTableRows(viewModel.Rows, viewModel.SelectedEmployeeId);
        }

        // ─── Private — tab bar builder ────────────────────────────────────────────────

        private void BuildTabBar(EmployeesViewModel viewModel)
        {
            if (_tabBar == null)
            {
                return;
            }

            _tabBar.Clear();

            if (viewModel.VisibleTabs == null)
            {
                return;
            }

            // Map tab IDs to display labels.
            foreach (string tabId in viewModel.VisibleTabs)
            {
                string label = ResolveTabLabel(tabId);

                var tabButton = new Button();
                tabButton.text = label;
                tabButton.AddToClassList("employees__tab-button");

                bool isActive = tabId == viewModel.ActiveTabId;
                if (isActive)
                {
                    tabButton.AddToClassList("is-active");
                }

                string capturedTabId = tabId;
                tabButton.RegisterCallback<ClickEvent>(_ => OnTabSelected?.Invoke(capturedTabId));

                _tabBar.Add(tabButton);
            }
        }

        // ─── Private — table builders ─────────────────────────────────────────────────

        private void BuildTableHeader()
        {
            if (_tableHeader == null)
            {
                return;
            }

            _tableHeader.Clear();

            foreach (string colHeader in ColumnHeaders)
            {
                var colEl = new Label(colHeader);
                colEl.AddToClassList("employees__table-header-cell");
                colEl.AddToClassList("text-label");
                _tableHeader.Add(colEl);
            }
        }

        private void BuildTableRows(IReadOnlyList<EmployeeRowViewModel> rows, string selectedId)
        {
            if (_tableBody == null)
            {
                return;
            }

            _tableBody.Clear();

            if (rows == null)
            {
                return;
            }

            foreach (EmployeeRowViewModel row in rows)
            {
                VisualElement rowEl = CreateTableRow(row, selectedId);
                _tableBody.Add(rowEl);
            }
        }

        private VisualElement CreateTableRow(EmployeeRowViewModel row, string selectedId)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("employees__row");

            // Semantic state class — "at-risk", "former", or none for "normal".
            ApplySemanticStateClass(rowEl, row.SemanticState);

            // Selection highlight.
            if (row.Id == selectedId)
            {
                rowEl.AddToClassList("is-selected");
            }

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            rowEl.Add(CreateCell(row.Name,             "employees__row__name",       "text-body"));
            rowEl.Add(CreateCell(row.Role,             "employees__row__role",       "text-body"));
            rowEl.Add(CreateCell(row.Seniority,        "employees__row__seniority",  "text-caption"));
            rowEl.Add(CreateCell(row.Team,             "employees__row__team",       "text-body"));
            rowEl.Add(CreateCell(row.CurrentAssignment,"employees__row__assignment", "text-caption"));
            rowEl.Add(CreateCell(row.Salary,           "employees__row__salary",     "text-body"));
            rowEl.Add(CreateCell(row.Morale,           "employees__row__morale",     "text-body"));
            rowEl.Add(CreateCell(row.BurnoutRisk,      "employees__row__burnout",    "text-caption"));
            rowEl.Add(CreateCell(row.Ability,          "employees__row__ability",    "text-body"));
            rowEl.Add(CreateCell(row.PotentialRange,   "employees__row__potential",  "text-caption"));
            rowEl.Add(CreateCell(row.Status,           "employees__row__status",     "text-caption"));
            rowEl.Add(CreateCell(row.StartDate,        "employees__row__start-date", "text-caption"));

            // Actions cell — view profile button.
            var actionsCell = new VisualElement();
            actionsCell.AddToClassList("employees__row__actions");

            if (row.IsClickable)
            {
                var viewBtn = new Button();
                viewBtn.text = "View";
                viewBtn.AddToClassList("employees__row__view-button");
                viewBtn.AddToClassList("btn-secondary");
                actionsCell.Add(viewBtn);

                string capturedId = row.Id;
                rowEl.RegisterCallback<ClickEvent>(_ => OnEmployeeRowClicked?.Invoke(capturedId));
                viewBtn.RegisterCallback<ClickEvent>(evt =>
                {
                    // Stop propagation to avoid double-firing from the row-level handler.
                    evt.StopPropagation();
                    OnEmployeeRowClicked?.Invoke(capturedId);
                });
            }
            else
            {
                var disabledLabel = new Label("—");
                disabledLabel.AddToClassList("text-caption");
                disabledLabel.AddToClassList("text-muted");
                actionsCell.Add(disabledLabel);
            }

            rowEl.Add(actionsCell);

            return rowEl;
        }

        private static VisualElement CreateCell(string text, string cellClass, string textClass)
        {
            var cell = new VisualElement();
            cell.AddToClassList("employees__row__cell");
            cell.AddToClassList(cellClass);

            var label = new Label(text ?? string.Empty);
            label.AddToClassList(textClass);
            cell.Add(label);

            return cell;
        }

        // ─── Private — filter badge ───────────────────────────────────────────────────

        private void UpdateFiltersBadge(EmployeeFilterViewModel filterState)
        {
            if (_filtersBadge == null)
            {
                return;
            }

            if (filterState == null || !filterState.HasActiveFilters)
            {
                _filtersBadge.AddToClassList("is-hidden");
                return;
            }

            _filtersBadge.RemoveFromClassList("is-hidden");
            _filtersBadge.text = filterState.ActiveFilterCount.ToString();
        }

        // ─── Private — state helpers ──────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,       false);
            SetVisible(_contentContainer,   false);
            SetVisible(_emptyState,         false);
            SetVisible(_filteredEmptyState, false);
            SetVisible(_errorState,         true);

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

        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        // ─── Private — label helpers ──────────────────────────────────────────────────

        private static string ResolveTabLabel(string tabId)
        {
            switch (tabId)
            {
                case EmployeesTabIds.All:        return "All Employees";
                case EmployeesTabIds.ByTeam:     return "By Team";
                case EmployeesTabIds.Unassigned: return "Unassigned";
                case EmployeesTabIds.AtRisk:     return "At Risk";
                case EmployeesTabIds.Former:     return "Former / Archived";
                default:                         return tabId;
            }
        }

        // ─── Private — query helpers ──────────────────────────────────────────────────

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
                    $"EmployeesView: element '{name}' not found in UXML root.");
            }
        }
    }
}
