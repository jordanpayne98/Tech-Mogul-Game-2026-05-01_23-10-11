using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Teams
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Teams screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates summary cards and team table rows from ViewModel data.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class TeamsView
    {
        // ─── Root ─────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from TeamsScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ─────────────────────────────────────────────────────────────

        /// <summary>Fired when a team table row is clicked. Argument is the team's stable ID.</summary>
        public event Action<string> OnTeamRowClicked;

        /// <summary>Fired when the Create Team button (header or empty state) is activated.</summary>
        public event Action OnCreateTeamClicked;

        // ─── State containers ─────────────────────────────────────────────────────────

        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _contentContainer;

        // ─── Header ───────────────────────────────────────────────────────────────────

        private readonly Label         _headerTitle;
        private readonly Label         _headerSubtitle;
        private readonly Button        _createTeamButton;

        // ─── Empty state ──────────────────────────────────────────────────────────────

        private readonly Label  _emptyStateTitle;
        private readonly Label  _emptyStateBody;
        private readonly Button _emptyStateCreateButton;

        // ─── Error state ──────────────────────────────────────────────────────────────

        private readonly Label _errorMessage;

        // ─── Dynamic content containers ───────────────────────────────────────────────

        private readonly VisualElement _summaryCardRow;
        private readonly VisualElement _tableBody;

        // ─── Footer ───────────────────────────────────────────────────────────────────

        private readonly Label _footerSummary;

        // ─── Constructor ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public TeamsView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "TeamsView: root VisualElement is null. View cannot be initialized.");

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
            _createTeamButton = root.Q<Button>("CreateTeamButton");

            LogIfNull(_headerTitle,     "HeaderTitle");
            LogIfNull(_headerSubtitle,  "HeaderSubtitle");
            LogIfNull(_createTeamButton, "CreateTeamButton");

            // ── Empty state elements ──────────────────────────────────────────────────

            _emptyStateTitle        = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody         = root.Q<Label>("EmptyStateBody");
            _emptyStateCreateButton = root.Q<Button>("EmptyStateCreateButton");

            LogIfNull(_emptyStateTitle,        "EmptyStateTitle");
            LogIfNull(_emptyStateBody,         "EmptyStateBody");
            LogIfNull(_emptyStateCreateButton, "EmptyStateCreateButton");

            // ── Error state elements ──────────────────────────────────────────────────

            _errorMessage = root.Q<Label>("ErrorMessage");
            LogIfNull(_errorMessage, "ErrorMessage");

            // ── Dynamic content containers ───────────────────────────────────────────

            _summaryCardRow = QueryElement(root, "SummaryCardRow");
            _tableBody      = QueryElement(root, "TableBody");

            // ── Footer ───────────────────────────────────────────────────────────────

            _footerSummary = root.Q<Label>("FooterSummary");
            LogIfNull(_footerSummary, "FooterSummary");

            // ── Wire static button callbacks ─────────────────────────────────────────

            if (_createTeamButton != null)
            {
                _createTeamButton.clicked += () => OnCreateTeamClicked?.Invoke();
            }

            if (_emptyStateCreateButton != null)
            {
                _emptyStateCreateButton.clicked += () => OnCreateTeamClicked?.Invoke();
            }
        }

        // ─── Public API ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds summary cards and team rows on each call.
        /// </summary>
        public void Bind(TeamsViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "TeamsView.Bind: viewModel is null. Showing error state.");
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

            if (viewModel.HasNoTeams)
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

            // Create Team button — disabled in Phase 5 per the implementation lock
            if (_createTeamButton != null)
            {
                _createTeamButton.SetEnabled(viewModel.CanCreateTeam);
            }

            // Summary cards
            BuildSummaryCards(viewModel.SummaryCards);

            // Team table rows
            BuildTableRows(viewModel.Rows);

            // Footer summary
            UpdateFooterSummary(viewModel.Rows);
        }

        // ─── Private — summary card builder ──────────────────────────────────────────

        private void BuildSummaryCards(IReadOnlyList<TeamSummaryCardViewModel> cards)
        {
            if (_summaryCardRow == null)
            {
                return;
            }

            _summaryCardRow.Clear();

            if (cards == null)
            {
                return;
            }

            foreach (TeamSummaryCardViewModel card in cards)
            {
                VisualElement cardEl = CreateSummaryCard(card);
                _summaryCardRow.Add(cardEl);
            }
        }

        private static VisualElement CreateSummaryCard(TeamSummaryCardViewModel card)
        {
            var container = new VisualElement();
            container.AddToClassList("teams__card");
            ApplySemanticStateClass(container, card.SemanticState);

            var labelEl = new Label(card.Label);
            labelEl.AddToClassList("teams__card__label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(card.Value);
            valueEl.AddToClassList("teams__card__value");
            valueEl.AddToClassList("text-heading");

            container.Add(valueEl);
            container.Add(labelEl);

            return container;
        }

        // ─── Private — table row builder ──────────────────────────────────────────────

        private void BuildTableRows(IReadOnlyList<TeamRowViewModel> rows)
        {
            if (_tableBody == null)
            {
                return;
            }

            _tableBody.Clear();

            if (rows == null || rows.Count == 0)
            {
                var emptyRow = new Label("[Placeholder] No teams to display.");
                emptyRow.AddToClassList("text-body");
                emptyRow.AddToClassList("text-muted");
                _tableBody.Add(emptyRow);
                return;
            }

            foreach (TeamRowViewModel row in rows)
            {
                VisualElement rowEl = CreateTableRow(row);
                _tableBody.Add(rowEl);
            }
        }

        private VisualElement CreateTableRow(TeamRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("teams__row");
            ApplySemanticStateClass(rowEl, row.SemanticState);

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            // ── Columns — order matches TableHeader in TeamsScreen.uxml ──────────────

            rowEl.Add(CreateCell(row.TeamName,          "teams__col--name"));
            rowEl.Add(CreateCell(row.Function,          "teams__col--function"));
            rowEl.Add(CreateCell(row.Members,           "teams__col--members"));
            rowEl.Add(CreateCell(row.Lead,              "teams__col--lead"));
            rowEl.Add(CreateCell(row.CurrentAssignment, "teams__col--assignment"));
            rowEl.Add(CreateCell(row.Capacity,          "teams__col--capacity"));
            rowEl.Add(CreateCell(row.Workload,          "teams__col--workload"));
            rowEl.Add(CreateCell(row.Morale,            "teams__col--morale"));
            rowEl.Add(CreateCell(row.Cohesion,          "teams__col--cohesion"));
            rowEl.Add(CreateCell(row.RoleGaps,          "teams__col--role-gaps"));

            // Status — rendered as a pill
            var statusEl = new Label(row.Status);
            statusEl.AddToClassList("teams__col");
            statusEl.AddToClassList("teams__col--status");
            statusEl.AddToClassList("pill");
            statusEl.AddToClassList($"pill--{MapSemanticStateToPill(row.SemanticState)}");
            rowEl.Add(statusEl);

            // Actions column — placeholder button
            var actionsCell = new VisualElement();
            actionsCell.AddToClassList("teams__col");
            actionsCell.AddToClassList("teams__col--actions");

            var viewBtn = new Button();
            viewBtn.text = "View";
            viewBtn.AddToClassList("base-button");
            viewBtn.AddToClassList("base-button--secondary");
            viewBtn.AddToClassList("base-button--compact");
            actionsCell.Add(viewBtn);

            rowEl.Add(actionsCell);

            // ── Click registration ────────────────────────────────────────────────────

            if (row.IsClickable)
            {
                string teamId = row.Id;
                rowEl.RegisterCallback<ClickEvent>(_ => OnTeamRowClicked?.Invoke(teamId));
                viewBtn.clicked += () => OnTeamRowClicked?.Invoke(teamId);
            }

            return rowEl;
        }

        private static VisualElement CreateCell(string text, string columnClass)
        {
            var cell = new Label(text ?? string.Empty);
            cell.AddToClassList("teams__col");
            cell.AddToClassList(columnClass);
            cell.AddToClassList("text-body");
            return cell;
        }

        // ─── Private — footer ─────────────────────────────────────────────────────────

        private void UpdateFooterSummary(IReadOnlyList<TeamRowViewModel> rows)
        {
            if (_footerSummary == null)
            {
                return;
            }

            int count = rows?.Count ?? 0;
            _footerSummary.text = count == 1
                ? "1 team"
                : $"{count} teams";
        }

        // ─── Private — state helpers ──────────────────────────────────────────────────

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

        private static string MapSemanticStateToPill(string semanticState)
        {
            return semanticState switch
            {
                "danger"  => "danger",
                "warning" => "warning",
                "success" => "success",
                "muted"   => "neutral",
                _         => "info",
            };
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
                    $"TeamsView: element '{name}' not found in UXML root.");
            }
        }
    }
}
