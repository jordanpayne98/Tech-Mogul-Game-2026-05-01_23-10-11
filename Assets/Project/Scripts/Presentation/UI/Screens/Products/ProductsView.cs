using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Products portfolio screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates tab buttons, product table rows, and status chips.
    /// Exposes user-intent events; applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static placeholder ViewModels. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductsView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from ProductsScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── User-intent events ──────────────────────────────────────────────────────

        /// <summary>Fired when a tab button is clicked. Argument is the tab's stable ID.</summary>
        public event Action<string> OnTabSelected;

        /// <summary>Fired when a product table row is clicked. Argument is the product's stable ID.</summary>
        public event Action<string> OnProductRowClicked;

        /// <summary>Fired when the Create Product button is clicked (header or empty-state variant).</summary>
        public event Action OnCreateProductClicked;

        /// <summary>Fired when the Filters button is clicked.</summary>
        public event Action OnFiltersButtonClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _contentContainer;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label  _headerTitle;
        private readonly Label  _headerSubtitle;
        private readonly Button _createProductButton;

        // ─── Empty-state elements ────────────────────────────────────────────────────

        private readonly Label  _emptyStateTitle;
        private readonly Label  _emptyStateBody;
        private readonly Button _createProductButtonEmpty;

        // ─── Error state ─────────────────────────────────────────────────────────────

        private readonly Label _errorMessage;

        // ─── Tabs and toolbar ────────────────────────────────────────────────────────

        private readonly VisualElement  _tabBar;
        private readonly DropdownField  _familyDropdown;
        private readonly TextField      _searchField;
        private readonly Button         _filtersButton;

        // ─── Table ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _tableBody;

        // ─── Footer ──────────────────────────────────────────────────────────────────

        private readonly Label _resultCount;

        // ─── Tab label map ───────────────────────────────────────────────────────────

        private static readonly Dictionary<string, string> TabLabels = new Dictionary<string, string>
        {
            { "tab.all",               "All Products"       },
            { "tab.in_development",    "In Development"     },
            { "tab.ready_for_launch",  "Ready for Launch"   },
            { "tab.launched",          "Launched"           },
            { "tab.supported",         "Supported"          },
            { "tab.cancelled_sunset",  "Cancelled / Sunset" },
        };

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public ProductsView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "ProductsView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
                Root = new VisualElement();
                return;
            }

            Root = root;

            // ── State containers ─────────────────────────────────────────────────────

            _loadingState     = QueryElement(root, "LoadingState");
            _errorState       = QueryElement(root, "ErrorState");
            _emptyState       = QueryElement(root, "EmptyState");
            _contentContainer = QueryElement(root, "ContentContainer");

            // ── Header ───────────────────────────────────────────────────────────────

            _headerTitle    = root.Q<Label>("HeaderTitle");
            _headerSubtitle = root.Q<Label>("HeaderSubtitle");
            _createProductButton = root.Q<Button>("CreateProductButton");

            LogIfNull(_headerTitle,          "HeaderTitle");
            LogIfNull(_headerSubtitle,       "HeaderSubtitle");
            LogIfNull(_createProductButton,  "CreateProductButton");

            // ── Empty-state ──────────────────────────────────────────────────────────

            _emptyStateTitle           = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody            = root.Q<Label>("EmptyStateBody");
            _createProductButtonEmpty  = root.Q<Button>("CreateProductButton_Empty");

            LogIfNull(_emptyStateTitle,          "EmptyStateTitle");
            LogIfNull(_emptyStateBody,           "EmptyStateBody");
            LogIfNull(_createProductButtonEmpty, "CreateProductButton_Empty");

            // ── Error state ──────────────────────────────────────────────────────────

            _errorMessage = root.Q<Label>("ErrorMessage");
            LogIfNull(_errorMessage, "ErrorMessage");

            // ── Tabs and toolbar ─────────────────────────────────────────────────────

            _tabBar         = QueryElement(root, "TabBar");
            _familyDropdown = root.Q<DropdownField>("FamilyDropdown");
            _searchField    = root.Q<TextField>("SearchField");
            _filtersButton  = root.Q<Button>("FiltersButton");

            LogIfNull(_familyDropdown, "FamilyDropdown");
            LogIfNull(_searchField,    "SearchField");
            LogIfNull(_filtersButton,  "FiltersButton");

            // ── Table ────────────────────────────────────────────────────────────────

            _tableBody = QueryElement(root, "TableBody");

            // ── Footer ───────────────────────────────────────────────────────────────

            _resultCount = root.Q<Label>("ResultCount");
            LogIfNull(_resultCount, "ResultCount");

            // ── Wire static callbacks ─────────────────────────────────────────────────

            if (_createProductButton != null)
            {
                _createProductButton.clicked += () => OnCreateProductClicked?.Invoke();
            }

            if (_createProductButtonEmpty != null)
            {
                _createProductButtonEmpty.clicked += () => OnCreateProductClicked?.Invoke();
            }

            if (_filtersButton != null)
            {
                _filtersButton.clicked += () => OnFiltersButtonClicked?.Invoke();
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds tab buttons and table rows on each call.
        /// </summary>
        public void Bind(ProductsViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "ProductsView.Bind: viewModel is null. Showing error state.");
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

            if (viewModel.HasNoProducts)
            {
                SetVisible(_loadingState,     false);
                SetVisible(_contentContainer, false);
                SetVisible(_errorState,       false);
                SetVisible(_emptyState,       true);

                if (_emptyStateTitle != null) { _emptyStateTitle.text = viewModel.EmptyStateTitle; }
                if (_emptyStateBody  != null) { _emptyStateBody.text  = viewModel.EmptyStateBody;  }

                return;
            }

            // ── Normal content state ─────────────────────────────────────────────────

            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, true);
            SetVisible(_errorState,       false);
            SetVisible(_emptyState,       false);

            // ── Header ───────────────────────────────────────────────────────────────

            if (_headerTitle    != null) { _headerTitle.text    = viewModel.ScreenTitle;    }
            if (_headerSubtitle != null) { _headerSubtitle.text = viewModel.ScreenSubtitle; }

            // Disable Create Product button when not permitted.
            if (_createProductButton != null)
            {
                _createProductButton.SetEnabled(viewModel.CanCreateProduct);
            }

            // ── Tabs ─────────────────────────────────────────────────────────────────

            BuildTabs(viewModel.VisibleTabs, viewModel.ActiveTabId);

            // ── Table rows ───────────────────────────────────────────────────────────

            BuildTableRows(viewModel.Rows);

            // ── Footer ───────────────────────────────────────────────────────────────

            if (_resultCount != null)
            {
                int count = viewModel.Rows != null ? viewModel.Rows.Count : 0;
                _resultCount.text = count == 1 ? "1 product" : $"{count} products";
            }
        }

        // ─── Private — Tab building ──────────────────────────────────────────────────

        /// <summary>
        /// Clears the tab bar and rebuilds one button per entry in visibleTabs.
        /// The active tab receives the is-active class.
        /// </summary>
        private void BuildTabs(IReadOnlyList<string> visibleTabs, string activeTabId)
        {
            if (_tabBar == null)
            {
                return;
            }

            _tabBar.Clear();

            if (visibleTabs == null || visibleTabs.Count == 0)
            {
                return;
            }

            foreach (string tabId in visibleTabs)
            {
                string label = TabLabels.TryGetValue(tabId, out string mapped) ? mapped : tabId;
                string capturedTabId = tabId;

                var tabButton = new Button(() => OnTabSelected?.Invoke(capturedTabId));
                tabButton.text = label;
                tabButton.AddToClassList("products__tab-btn");

                if (tabId == activeTabId)
                {
                    tabButton.AddToClassList("is-active");
                }

                _tabBar.Add(tabButton);
            }
        }

        // ─── Private — Table row building ────────────────────────────────────────────

        /// <summary>
        /// Clears the table body and rebuilds one row VisualElement per ProductRowViewModel.
        /// Each row gets semantic state classes and a click callback.
        /// </summary>
        private void BuildTableRows(IReadOnlyList<ProductRowViewModel> rows)
        {
            if (_tableBody == null)
            {
                return;
            }

            _tableBody.Clear();

            if (rows == null || rows.Count == 0)
            {
                var noDataLabel = new Label("No products match the current filter.");
                noDataLabel.AddToClassList("products__no-data-label");
                noDataLabel.AddToClassList("text-body");
                _tableBody.Add(noDataLabel);
                return;
            }

            foreach (ProductRowViewModel row in rows)
            {
                _tableBody.Add(BuildRow(row));
            }
        }

        /// <summary>
        /// Builds a single product table row from the given ViewModel.
        /// </summary>
        private VisualElement BuildRow(ProductRowViewModel row)
        {
            var rowElement = new VisualElement();
            rowElement.AddToClassList("products__row");

            // ── Semantic state class ─────────────────────────────────────────────────

            if (!string.IsNullOrEmpty(row.SemanticState))
            {
                rowElement.AddToClassList($"products__row--{row.SemanticState}");
            }

            // ── Cancelled/sunset muted class ─────────────────────────────────────────

            if (row.Status == "Cancelled" || row.Status == "Sunset")
            {
                rowElement.AddToClassList("products__row--sunset");
            }

            // ── Click callback ───────────────────────────────────────────────────────

            if (row.IsClickable && !string.IsNullOrEmpty(row.Id))
            {
                string capturedId = row.Id;
                rowElement.RegisterCallback<ClickEvent>(_ => OnProductRowClicked?.Invoke(capturedId));
                rowElement.AddToClassList("products__row--clickable");
            }

            // ── Cells ────────────────────────────────────────────────────────────────

            rowElement.Add(BuildCell(row.ProductName,        "products__col--name",          "text-body"));
            rowElement.Add(BuildCell(row.Family,             "products__col--family",         "text-body"));
            rowElement.Add(BuildCell(row.Type,               "products__col--type",           "text-body"));
            rowElement.Add(BuildStatusChip(row.Status,       "products__col--status",         row.SemanticState));
            rowElement.Add(BuildCell(row.AssignedTeam,       "products__col--team",           "text-body"));
            rowElement.Add(BuildCell(row.Phase,              "products__col--phase",          "text-body"));
            rowElement.Add(BuildCell(row.ReleaseTarget,      "products__col--release",        "text-body"));
            rowElement.Add(BuildCell(row.ReviewScore,        "products__col--review",         "text-body"));
            rowElement.Add(BuildCell(row.RecentReviewScore,  "products__col--recent-review",  "text-body"));
            rowElement.Add(BuildCell(row.RevenueThisMonth,   "products__col--revenue",        "text-body"));
            rowElement.Add(BuildCell(row.ActiveUsersOrUnits, "products__col--users",          "text-body"));
            rowElement.Add(BuildCell(row.SupportLoad,        "products__col--support",        "text-body"));

            // ── Actions cell ─────────────────────────────────────────────────────────

            var actionsCell = new VisualElement();
            actionsCell.AddToClassList("products__row-cell");
            actionsCell.AddToClassList("products__col--actions");

            var detailButton = new Button(() =>
            {
                if (!string.IsNullOrEmpty(row.Id))
                {
                    OnProductRowClicked?.Invoke(row.Id);
                }
            });
            detailButton.text = "View";
            detailButton.AddToClassList("products__row-action-btn");

            actionsCell.Add(detailButton);
            rowElement.Add(actionsCell);

            return rowElement;
        }

        /// <summary>Builds a standard text cell with the given column class.</summary>
        private static VisualElement BuildCell(string text, string columnClass, string textClass)
        {
            var cell = new VisualElement();
            cell.AddToClassList("products__row-cell");
            cell.AddToClassList(columnClass);

            var label = new Label(text ?? "—");
            label.AddToClassList(textClass);
            cell.Add(label);

            return cell;
        }

        /// <summary>Builds a status chip cell using semantic state classes instead of raw colours.</summary>
        private static VisualElement BuildStatusChip(string statusText, string columnClass, string semanticState)
        {
            var cell = new VisualElement();
            cell.AddToClassList("products__row-cell");
            cell.AddToClassList(columnClass);

            var chip = new Label(statusText ?? "—");
            chip.AddToClassList("products__status-chip");

            if (!string.IsNullOrEmpty(semanticState))
            {
                chip.AddToClassList($"products__status-chip--{semanticState}");
            }

            cell.Add(chip);
            return cell;
        }

        // ─── Private — State helpers ─────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_errorState,       true);
            SetVisible(_emptyState,       false);

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

        // ─── Private — Query helpers ─────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q(name);

            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"ProductsView: element '{name}' not found in UXML. Display may be incomplete.");
            }

            return element;
        }

        private static void LogIfNull(VisualElement element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"ProductsView: element '{name}' not found in UXML. Display may be incomplete.");
            }
        }
    }
}
