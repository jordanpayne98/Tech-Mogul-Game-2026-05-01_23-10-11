using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Contracts board screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates tab buttons and contract table rows.
    /// Exposes tab selection, contract row click, and filters button events.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractsView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from ContractsScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a tab button is clicked. Argument is the stable tab ID.</summary>
        public event Action<string> OnTabSelected;

        /// <summary>Fired when a contract row is clicked. Argument is the contract stable ID.</summary>
        public event Action<string> OnContractRowClicked;

        /// <summary>Fired when the Filters button is clicked to open the right filter drawer.</summary>
        public event Action OnFiltersButtonClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Tabs ────────────────────────────────────────────────────────────────────

        private readonly VisualElement _tabBar;

        // ─── Toolbar ─────────────────────────────────────────────────────────────────

        private readonly DropdownField _typeDropdown;
        private readonly DropdownField _difficultyDropdown;
        private readonly TextField     _searchField;
        private readonly Button        _filtersButton;

        // ─── Table ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _tableBody;

        // ─── Footer ──────────────────────────────────────────────────────────────────

        private readonly Label _resultCount;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Tab display name map ────────────────────────────────────────────────────

        private static readonly Dictionary<string, string> TabDisplayNames = new Dictionary<string, string>
        {
            { "tab.available",     "Available"     },
            { "tab.active",        "Active"        },
            { "tab.completed",     "Completed"     },
            { "tab.failed_expired","Failed / Expired" },
        };

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public ContractsView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "ContractsView: root VisualElement is null. View cannot be initialized.");

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

            // ── Tabs ─────────────────────────────────────────────────────────────────

            _tabBar = QueryElement(root, "TabBar");

            // ── Toolbar ──────────────────────────────────────────────────────────────

            _typeDropdown       = root.Q<DropdownField>("TypeDropdown");
            _difficultyDropdown = root.Q<DropdownField>("DifficultyDropdown");
            _searchField        = root.Q<TextField>("SearchField");
            _filtersButton      = root.Q<Button>("FiltersButton");

            LogIfNull(_typeDropdown,       "TypeDropdown");
            LogIfNull(_difficultyDropdown, "DifficultyDropdown");
            LogIfNull(_searchField,        "SearchField");
            LogIfNull(_filtersButton,      "FiltersButton");

            // ── Table ─────────────────────────────────────────────────────────────────

            _tableBody = QueryElement(root, "TableBody");

            // ── Footer ────────────────────────────────────────────────────────────────

            _resultCount = root.Q<Label>("ResultCount");
            LogIfNull(_resultCount, "ResultCount");

            // ── Error / empty labels ──────────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");

            // ── Wire static events ────────────────────────────────────────────────────

            if (_filtersButton != null)
            {
                _filtersButton.clicked += () => OnFiltersButtonClicked?.Invoke();
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic tab and row lists on each call.
        /// </summary>
        public void Bind(ContractsViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "ContractsView.Bind: viewModel is null. Showing error state.");
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

            bool hasRows = viewModel.Rows != null && viewModel.Rows.Count > 0;

            bool emptyTab = viewModel.ActiveTabId == "tab.available"
                ? viewModel.HasNoAvailableContracts
                : viewModel.ActiveTabId == "tab.active"
                    ? viewModel.HasNoActiveContracts
                    : !hasRows;

            if (!hasRows)
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, true);   // keep header/tabs visible
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

                // Still rebuild tabs so the user can switch tabs.
                BuildTabBar(viewModel);
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

            // Tabs
            BuildTabBar(viewModel);

            // Table rows
            BuildTableBody(viewModel);

            // Footer result count
            if (_resultCount != null)
            {
                int count = viewModel.Rows?.Count ?? 0;
                _resultCount.text = count == 1 ? "1 contract" : $"{count} contracts";
            }
        }

        // ─── Private — tab bar ───────────────────────────────────────────────────────

        private void BuildTabBar(ContractsViewModel viewModel)
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

            foreach (string tabId in viewModel.VisibleTabs)
            {
                VisualElement tabButton = CreateTabButton(tabId, viewModel.ActiveTabId);
                _tabBar.Add(tabButton);
            }
        }

        private VisualElement CreateTabButton(string tabId, string activeTabId)
        {
            string label = TabDisplayNames.TryGetValue(tabId, out string display)
                ? display
                : tabId;

            var button = new Button();
            button.text = label;
            button.AddToClassList("contracts__tab-button");
            button.AddToClassList("text-label");

            if (tabId == activeTabId)
            {
                button.AddToClassList("is-active");
            }

            string capturedTabId = tabId;
            button.clicked += () => OnTabSelected?.Invoke(capturedTabId);

            return button;
        }

        // ─── Private — table body ────────────────────────────────────────────────────

        private void BuildTableBody(ContractsViewModel viewModel)
        {
            if (_tableBody == null)
            {
                return;
            }

            _tableBody.Clear();

            if (viewModel.Rows == null)
            {
                return;
            }

            foreach (ContractRowViewModel row in viewModel.Rows)
            {
                VisualElement rowEl = CreateContractRow(row, viewModel.SelectedContractId);
                _tableBody.Add(rowEl);
            }
        }

        private VisualElement CreateContractRow(ContractRowViewModel row, string selectedContractId)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("contracts__row");

            // Semantic state class
            ApplySemanticStateClass(rowEl, row.SemanticState);

            // Selection
            if (row.Id == selectedContractId)
            {
                rowEl.AddToClassList("is-selected");
            }

            // Status-specific classes
            if (row.Status == "Completed")
            {
                rowEl.AddToClassList("contracts__row--completed");
            }
            else if (row.Status == "Failed" || row.Status == "Expired")
            {
                rowEl.AddToClassList("contracts__row--failed");
            }

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            // ── Cells ────────────────────────────────────────────────────────────────

            rowEl.Add(CreateCell(row.Client,        "contracts__col--client"));
            rowEl.Add(CreateCell(row.ContractType,  "contracts__col--type"));
            rowEl.Add(CreateCell(row.RequiredSkills,"contracts__col--skills"));
            rowEl.Add(CreateCell(row.Difficulty,    "contracts__col--difficulty"));
            rowEl.Add(CreateCell(row.Deadline,      "contracts__col--deadline"));
            rowEl.Add(CreateCell(row.Payment,       "contracts__col--payment"));
            rowEl.Add(CreateProgressCell(row));
            rowEl.Add(CreateCell(row.AssignedTeam,  "contracts__col--team"));
            rowEl.Add(CreateCell(row.QualityTarget, "contracts__col--quality"));
            rowEl.Add(CreateStatusCell(row));
            rowEl.Add(CreateActionsCell(row));

            // ── Row click ────────────────────────────────────────────────────────────

            if (row.IsClickable)
            {
                string contractId = row.Id;
                rowEl.RegisterCallback<ClickEvent>(_ => OnContractRowClicked?.Invoke(contractId));
            }

            return rowEl;
        }

        private static VisualElement CreateCell(string text, string columnClass)
        {
            var cell = new VisualElement();
            cell.AddToClassList("contracts__row-cell");
            cell.AddToClassList(columnClass);

            var label = new Label(text ?? string.Empty);
            label.AddToClassList("contracts__cell-label");
            label.AddToClassList("text-body");
            cell.Add(label);

            return cell;
        }

        private static VisualElement CreateProgressCell(ContractRowViewModel row)
        {
            var cell = new VisualElement();
            cell.AddToClassList("contracts__row-cell");
            cell.AddToClassList("contracts__col--progress");

            if (row.Progress == "—" || string.IsNullOrEmpty(row.Progress))
            {
                var label = new Label("—");
                label.AddToClassList("contracts__cell-label");
                label.AddToClassList("text-body");
                label.AddToClassList("text-muted");
                cell.Add(label);
            }
            else
            {
                // Progress bar container
                var barBg = new VisualElement();
                barBg.AddToClassList("contracts__progress-bar");

                var barFill = new VisualElement();
                barFill.AddToClassList("contracts__progress-bar__fill");

                // Apply width via inline style — progress bar fill width is a runtime data-driven value,
                // not a design constant, so inline style is acceptable here.
                if (float.TryParse(
                        row.Progress.Replace("%", string.Empty).Trim(),
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out float pct))
                {
                    barFill.style.width = new StyleLength(new Length(pct, LengthUnit.Percent));
                }

                barBg.Add(barFill);

                var progressLabel = new Label(row.Progress);
                progressLabel.AddToClassList("contracts__cell-label");
                progressLabel.AddToClassList("text-small");

                cell.Add(barBg);
                cell.Add(progressLabel);
            }

            return cell;
        }

        private static VisualElement CreateStatusCell(ContractRowViewModel row)
        {
            var cell = new VisualElement();
            cell.AddToClassList("contracts__row-cell");
            cell.AddToClassList("contracts__col--status");

            var pill = new Label(row.Status ?? string.Empty);
            pill.AddToClassList("pill");
            pill.AddToClassList("contracts__status-pill");
            ApplySemanticStateClass(pill, row.SemanticState);
            cell.Add(pill);

            return cell;
        }

        private static VisualElement CreateActionsCell(ContractRowViewModel row)
        {
            var cell = new VisualElement();
            cell.AddToClassList("contracts__row-cell");
            cell.AddToClassList("contracts__col--actions");

            // [Placeholder] Actions buttons — Phase 6+ will wire Accept/Assign commands.
            var viewButton = new Label("[View]");
            viewButton.AddToClassList("contracts__action-link");
            viewButton.AddToClassList("text-small");
            cell.Add(viewButton);

            return cell;
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

        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
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
                    $"ContractsView: element '{name}' not found in UXML root.");
            }
        }
    }
}
