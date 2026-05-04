using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Competitors
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Competitors screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates competitor table rows with archetype badges.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompetitorsView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from CompetitorsScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a competitor row is clicked. Argument is the competitor's stable ID.</summary>
        public event Action<string> OnCompetitorRowClicked;

        /// <summary>Fired when the Filters button is clicked.</summary>
        public event Action OnFiltersButtonClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Toolbar ─────────────────────────────────────────────────────────────────

        private readonly DropdownField _archetypeDropdown;
        private readonly DropdownField _marketDropdown;
        private readonly TextField     _searchField;
        private readonly Button        _filtersButton;

        // ─── Table ───────────────────────────────────────────────────────────────────

        private readonly VisualElement _tableBody;

        // ─── Footer ──────────────────────────────────────────────────────────────────

        private readonly Label         _resultCount;
        private readonly VisualElement _paginationControls;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public CompetitorsView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "CompetitorsView: root VisualElement is null. View cannot be initialized.");

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

            // ── Toolbar ──────────────────────────────────────────────────────────────

            _archetypeDropdown = root.Q<DropdownField>("ArchetypeDropdown");
            _marketDropdown    = root.Q<DropdownField>("MarketDropdown");
            _searchField       = root.Q<TextField>("SearchField");
            _filtersButton     = root.Q<Button>("FiltersButton");

            LogIfNull(_archetypeDropdown, "ArchetypeDropdown");
            LogIfNull(_marketDropdown,    "MarketDropdown");
            LogIfNull(_searchField,       "SearchField");
            LogIfNull(_filtersButton,     "FiltersButton");

            // ── Table ────────────────────────────────────────────────────────────────

            _tableBody = QueryElement(root, "TableBody");

            // ── Footer ───────────────────────────────────────────────────────────────

            _resultCount        = root.Q<Label>("ResultCount");
            _paginationControls = QueryElement(root, "PaginationControls");

            // ── State / error text ───────────────────────────────────────────────────

            _errorMessage    = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,    "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");

            // ── Wire toolbar callbacks ────────────────────────────────────────────────

            if (_filtersButton != null)
            {
                _filtersButton.clicked += () => OnFiltersButtonClicked?.Invoke();
            }

            InitDropdowns();
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds the competitor table rows on each call.
        /// </summary>
        public void Bind(CompetitorsViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "CompetitorsView.Bind: viewModel is null. Showing error state.");
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

            if (viewModel.HasNoCompetitors)
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

            // Competitor table
            BuildCompetitorTable(viewModel.Rows);

            // Footer result count
            if (_resultCount != null)
            {
                int count = viewModel.Rows?.Count ?? 0;
                _resultCount.text = $"{count} competitor{(count == 1 ? string.Empty : "s")} shown";
            }
        }

        // ─── Private — table builder ─────────────────────────────────────────────────

        private void BuildCompetitorTable(IReadOnlyList<CompetitorRowViewModel> rows)
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

            foreach (CompetitorRowViewModel row in rows)
            {
                VisualElement rowEl = CreateCompetitorRow(row);
                _tableBody.Add(rowEl);
            }
        }

        private VisualElement CreateCompetitorRow(CompetitorRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("competitors__row");

            ApplySemanticStateClass(rowEl, row.SemanticState);

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            // ── Company Name cell ────────────────────────────────────────────────────

            var nameCell = new Label(row.CompanyName);
            nameCell.AddToClassList("competitors__table-cell");
            nameCell.AddToClassList("competitors__col--name");
            nameCell.AddToClassList("text-body");
            rowEl.Add(nameCell);

            // ── Archetype badge cell ──────────────────────────────────────────────────

            var archetypeCell = new VisualElement();
            archetypeCell.AddToClassList("competitors__table-cell");
            archetypeCell.AddToClassList("competitors__col--archetype");

            var archetypeBadge = new Label(row.Archetype);
            archetypeBadge.AddToClassList("competitors__archetype-badge");
            archetypeBadge.AddToClassList("text-caption");
            ApplyArchetypeBadgeClass(archetypeBadge, row.Archetype);
            archetypeCell.Add(archetypeBadge);
            rowEl.Add(archetypeCell);

            // ── Main Market cell ──────────────────────────────────────────────────────

            var marketCell = new Label(row.MainMarket);
            marketCell.AddToClassList("competitors__table-cell");
            marketCell.AddToClassList("competitors__col--market");
            marketCell.AddToClassList("text-body");
            rowEl.Add(marketCell);

            // ── Reputation cell ───────────────────────────────────────────────────────

            var reputationCell = new Label(row.Reputation);
            reputationCell.AddToClassList("competitors__table-cell");
            reputationCell.AddToClassList("competitors__col--reputation");
            reputationCell.AddToClassList("text-body");
            rowEl.Add(reputationCell);

            // ── Product Count cell ────────────────────────────────────────────────────

            var productCountCell = new Label(row.ProductCount);
            productCountCell.AddToClassList("competitors__table-cell");
            productCountCell.AddToClassList("competitors__col--products");
            productCountCell.AddToClassList("text-body");
            rowEl.Add(productCountCell);

            // ── Recent Launch cell ────────────────────────────────────────────────────

            var launchCell = new Label(row.RecentLaunch);
            launchCell.AddToClassList("competitors__table-cell");
            launchCell.AddToClassList("competitors__col--launch");
            launchCell.AddToClassList("text-caption");
            rowEl.Add(launchCell);

            // ── Market Position cell ──────────────────────────────────────────────────

            var positionCell = new Label(row.MarketPosition);
            positionCell.AddToClassList("competitors__table-cell");
            positionCell.AddToClassList("competitors__col--position");
            positionCell.AddToClassList("text-body");
            rowEl.Add(positionCell);

            // ── Trend cell ────────────────────────────────────────────────────────────

            var trendCell = new Label(row.Trend);
            trendCell.AddToClassList("competitors__table-cell");
            trendCell.AddToClassList("competitors__col--trend");
            trendCell.AddToClassList("text-caption");
            ApplyTrendClass(trendCell, row.Trend);
            rowEl.Add(trendCell);

            // ── Actions cell ──────────────────────────────────────────────────────────

            var actionsCell = new VisualElement();
            actionsCell.AddToClassList("competitors__table-cell");
            actionsCell.AddToClassList("competitors__col--actions");

            if (row.IsClickable)
            {
                var viewBtn = new Button();
                viewBtn.text = "View";
                viewBtn.AddToClassList("competitors__row-action-btn");

                string competitorId = row.Id;
                viewBtn.clicked += () => OnCompetitorRowClicked?.Invoke(competitorId);

                actionsCell.Add(viewBtn);
            }

            rowEl.Add(actionsCell);

            // ── Row-level click ───────────────────────────────────────────────────────

            if (row.IsClickable)
            {
                string competitorId = row.Id;
                rowEl.RegisterCallback<ClickEvent>(evt =>
                {
                    // Only fire if the click didn't originate from the action button (already handled).
                    if (evt.target is Button)
                    {
                        return;
                    }

                    OnCompetitorRowClicked?.Invoke(competitorId);
                });
            }

            return rowEl;
        }

        // ─── Private — toolbar initializer ───────────────────────────────────────────

        /// <summary>
        /// Populates the archetype and market dropdowns with their option lists.
        /// Choices are static display labels; filtering logic is driven by CompetitorsController.
        /// </summary>
        private void InitDropdowns()
        {
            if (_archetypeDropdown != null)
            {
                _archetypeDropdown.choices = new List<string>
                {
                    "All Archetypes",
                    "Incumbent Giant",
                    "Aggressive Startup",
                    "Research Lab",
                    "Hardware Manufacturer",
                    "Enterprise Specialist",
                    "Consumer Brand",
                    "Low-Cost Competitor",
                    "Platform Holder",
                };
                _archetypeDropdown.index = 0;
            }

            if (_marketDropdown != null)
            {
                _marketDropdown.choices = new List<string>
                {
                    "All Markets",
                    "Enterprise Software",
                    "Consumer Applications",
                    "Cloud Infrastructure",
                    "Hardware / Devices",
                    "Platform / Ecosystem",
                    "Research & Development",
                };
                _marketDropdown.index = 0;
            }
        }

        // ─── Private — semantic CSS helpers ──────────────────────────────────────────

        /// <summary>Applies the matching archetype badge USS modifier class based on the archetype label.</summary>
        private static void ApplyArchetypeBadgeClass(VisualElement element, string archetype)
        {
            if (string.IsNullOrEmpty(archetype))
            {
                return;
            }

            // Normalize to a CSS-safe suffix.
            string suffix = archetype.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("/", "-")
                .Replace("&", "and");

            element.AddToClassList($"competitors__archetype-badge--{suffix}");
        }

        /// <summary>Applies the matching trend USS modifier class based on the trend label.</summary>
        private static void ApplyTrendClass(VisualElement element, string trend)
        {
            if (string.IsNullOrEmpty(trend))
            {
                return;
            }

            switch (trend.ToLowerInvariant())
            {
                case "growing":
                    element.AddToClassList("competitors__trend--growing");
                    break;
                case "stable":
                    element.AddToClassList("competitors__trend--stable");
                    break;
                case "declining":
                    element.AddToClassList("competitors__trend--declining");
                    break;
            }
        }

        /// <summary>
        /// Applies the semantic state USS modifier to the target element.
        /// Valid states: neutral, warning, dominant, unknown, success, info.
        /// </summary>
        private static void ApplySemanticStateClass(VisualElement element, string semanticState)
        {
            if (string.IsNullOrEmpty(semanticState) || semanticState == "neutral")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        // ─── Private — state helpers ─────────────────────────────────────────────────

        private void ShowError(string message)
        {
            SetVisible(_loadingState,     false);
            SetVisible(_contentContainer, false);
            SetVisible(_errorState,       true);
            SetVisible(_emptyState,       false);

            if (_errorMessage != null)
            {
                _errorMessage.text = message;
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

        // ─── Private — safe query helpers ────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string elementName)
        {
            VisualElement element = root.Q(elementName);

            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"CompetitorsView: '{elementName}' not found in UXML. " +
                    "Verify element name in CompetitorsScreen.uxml.");
            }

            return element;
        }

        private static void LogIfNull(VisualElement element, string elementName)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"CompetitorsView: '{elementName}' not found in UXML. " +
                    "Verify element name in CompetitorsScreen.uxml.");
            }
        }
    }
}
