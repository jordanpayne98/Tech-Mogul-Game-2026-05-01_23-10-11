using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Research
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Research screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates track tabs, available project cards, locked project cards,
    /// and the assigned research panel.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ResearchView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from ResearchScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a track tab is selected. Argument is the track's stable ID.</summary>
        public event Action<string> OnTrackSelected;

        /// <summary>Fired when a project card or row is clicked. Argument is the project's stable ID.</summary>
        public event Action<string> OnProjectClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;

        // ─── Track list ──────────────────────────────────────────────────────────────

        private readonly ScrollView _trackList;

        // ─── Project lists ───────────────────────────────────────────────────────────

        private readonly VisualElement _availableList;
        private readonly VisualElement _lockedList;
        private readonly VisualElement _noAvailableState;

        // ─── Assigned research ───────────────────────────────────────────────────────

        private readonly VisualElement _assignedContent;
        private readonly VisualElement _noAssignedState;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Active track tracking ───────────────────────────────────────────────────

        private string _activeTrackId;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public ResearchView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "ResearchView: root VisualElement is null. View cannot be initialized.");

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

            // ── Track list ───────────────────────────────────────────────────────────

            _trackList = root.Q<ScrollView>("TrackList");
            LogIfNull(_trackList, "TrackList");

            // ── Project lists ────────────────────────────────────────────────────────

            _availableList    = QueryElement(root, "AvailableList");
            _lockedList       = QueryElement(root, "LockedList");
            _noAvailableState = QueryElement(root, "NoAvailableState");

            // ── Assigned research ─────────────────────────────────────────────────────

            _assignedContent  = QueryElement(root, "AssignedContent");
            _noAssignedState  = QueryElement(root, "NoAssignedState");

            // ── State text ───────────────────────────────────────────────────────────

            _errorMessage   = root.Q<Label>("ErrorMessage");
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
        /// Clears and rebuilds all dynamic track tabs and project lists on each call.
        /// </summary>
        public void Bind(ResearchViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "ResearchView.Bind: viewModel is null. Showing error state.");
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

            bool hasTracks = viewModel.Tracks != null && viewModel.Tracks.Count > 0;

            if (!hasTracks)
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

            _activeTrackId = viewModel.ActiveTrackId;

            // Track list
            BuildTrackList(viewModel.Tracks, viewModel.ActiveTrackId);

            // Available projects
            BuildAvailableList(viewModel.AvailableProjects);

            // Locked projects
            BuildLockedList(viewModel.LockedProjects);

            // Assigned research
            BuildAssignedPanel(viewModel.AssignedResearch);
        }

        // ─── Private — track list builder ────────────────────────────────────────────

        private void BuildTrackList(IReadOnlyList<ResearchTrackViewModel> tracks, string activeTrackId)
        {
            if (_trackList == null)
            {
                return;
            }

            _trackList.Clear();

            if (tracks == null)
            {
                return;
            }

            foreach (ResearchTrackViewModel track in tracks)
            {
                VisualElement trackBtn = CreateTrackButton(track, activeTrackId);
                _trackList.Add(trackBtn);
            }
        }

        private VisualElement CreateTrackButton(ResearchTrackViewModel track, string activeTrackId)
        {
            var container = new VisualElement();
            container.AddToClassList("research__track-btn");

            bool isActive = track.Id == activeTrackId;
            if (isActive)
            {
                container.AddToClassList("is-active");
            }

            ApplySemanticStateClass(container, track.SemanticState);

            var nameLabel = new Label(track.Name);
            nameLabel.AddToClassList("research__track-btn__name");
            nameLabel.AddToClassList("text-body");

            var countLabel = new Label(track.ProjectCount);
            countLabel.AddToClassList("research__track-btn__count");
            countLabel.AddToClassList("text-caption");

            container.Add(nameLabel);
            container.Add(countLabel);

            // Capture the stable ID for the closure.
            string trackId = track.Id;
            container.RegisterCallback<ClickEvent>(_ => OnTrackSelected?.Invoke(trackId));

            return container;
        }

        // ─── Private — available list builder ────────────────────────────────────────

        private void BuildAvailableList(IReadOnlyList<ResearchProjectRowViewModel> projects)
        {
            if (_availableList == null)
            {
                return;
            }

            _availableList.Clear();

            bool hasProjects = projects != null && projects.Count > 0;

            SetVisible(_noAvailableState, !hasProjects);

            if (!hasProjects)
            {
                return;
            }

            foreach (ResearchProjectRowViewModel project in projects)
            {
                VisualElement card = CreateProjectCard(project);
                _availableList.Add(card);
            }
        }

        // ─── Private — locked list builder ───────────────────────────────────────────

        private void BuildLockedList(IReadOnlyList<ResearchProjectRowViewModel> projects)
        {
            if (_lockedList == null)
            {
                return;
            }

            _lockedList.Clear();

            if (projects == null || projects.Count == 0)
            {
                return;
            }

            foreach (ResearchProjectRowViewModel project in projects)
            {
                VisualElement card = CreateProjectCard(project);
                _lockedList.Add(card);
            }
        }

        // ─── Private — project card builder ──────────────────────────────────────────

        private VisualElement CreateProjectCard(ResearchProjectRowViewModel project)
        {
            var card = new VisualElement();
            card.AddToClassList("research__card");
            ApplySemanticStateClass(card, project.SemanticState);

            if (project.IsLocked)
            {
                card.AddToClassList("is-disabled");
            }

            if (project.IsClickable)
            {
                card.AddToClassList("is-clickable");
            }

            // ── Card header row ──────────────────────────────────────────────────────

            var cardHeader = new VisualElement();
            cardHeader.AddToClassList("research__card__header");

            var nameLabel = new Label(project.Name);
            nameLabel.AddToClassList("research__card__name");
            nameLabel.AddToClassList("text-heading");

            var statusLabel = new Label(project.Status);
            statusLabel.AddToClassList("research__card__status");
            statusLabel.AddToClassList("text-caption");
            ApplySemanticStateClass(statusLabel, project.SemanticState);

            cardHeader.Add(nameLabel);
            cardHeader.Add(statusLabel);
            card.Add(cardHeader);

            // ── Key metadata row ─────────────────────────────────────────────────────

            var metaRow = new VisualElement();
            metaRow.AddToClassList("research__card__meta");

            metaRow.Add(CreateMetaChip("Track", project.Track));
            metaRow.Add(CreateMetaChip("Skill", project.RequiredSkill));
            metaRow.Add(CreateMetaChip("Duration", project.Duration));
            metaRow.Add(CreateMetaChip("Cost", project.Cost));
            metaRow.Add(CreateMetaChip("Risk", project.RiskLevel));

            card.Add(metaRow);

            // ── Unlocks row ──────────────────────────────────────────────────────────

            if (!string.IsNullOrEmpty(project.Unlocks))
            {
                var unlocksRow = new VisualElement();
                unlocksRow.AddToClassList("research__card__unlocks");

                var unlocksLabel = new Label($"Unlocks: {project.Unlocks}");
                unlocksLabel.AddToClassList("text-caption");
                unlocksRow.Add(unlocksLabel);
                card.Add(unlocksRow);
            }

            // ── Prerequisites row (locked projects only, per Section 14 lock) ─────────

            if (project.IsLocked && !string.IsNullOrEmpty(project.Prerequisites))
            {
                var prereqRow = new VisualElement();
                prereqRow.AddToClassList("research__card__prereqs");
                prereqRow.AddToClassList("has-warning");

                var prereqLabel = new Label($"Requires: {project.Prerequisites}");
                prereqLabel.AddToClassList("text-caption");
                prereqRow.Add(prereqLabel);
                card.Add(prereqRow);
            }

            // ── Assigned team / completion estimate row ───────────────────────────────

            bool hasTeam     = !string.IsNullOrEmpty(project.AssignedTeam);
            bool hasEstimate = !string.IsNullOrEmpty(project.CompletionEstimate);

            if (hasTeam || hasEstimate)
            {
                var assignRow = new VisualElement();
                assignRow.AddToClassList("research__card__assignment");

                if (hasTeam)
                {
                    var teamLabel = new Label($"Team: {project.AssignedTeam}");
                    teamLabel.AddToClassList("text-caption");
                    assignRow.Add(teamLabel);
                }

                if (hasEstimate)
                {
                    var estimateLabel = new Label($"Est. completion: {project.CompletionEstimate}");
                    estimateLabel.AddToClassList("text-caption");
                    assignRow.Add(estimateLabel);
                }

                card.Add(assignRow);
            }

            // ── Click registration ───────────────────────────────────────────────────

            if (project.IsClickable)
            {
                string projectId = project.Id;
                card.RegisterCallback<ClickEvent>(_ => OnProjectClicked?.Invoke(projectId));
            }

            return card;
        }

        private static VisualElement CreateMetaChip(string label, string value)
        {
            var chip = new VisualElement();
            chip.AddToClassList("research__card__meta-chip");

            var labelEl = new Label(label);
            labelEl.AddToClassList("research__card__meta-chip__label");
            labelEl.AddToClassList("text-label");

            var valueEl = new Label(value);
            valueEl.AddToClassList("research__card__meta-chip__value");
            valueEl.AddToClassList("text-caption");

            chip.Add(labelEl);
            chip.Add(valueEl);
            return chip;
        }

        // ─── Private — assigned research panel builder ────────────────────────────────

        private void BuildAssignedPanel(AssignedResearchViewModel assigned)
        {
            if (_assignedContent == null)
            {
                return;
            }

            _assignedContent.Clear();

            if (assigned == null || !assigned.HasAssignedResearch)
            {
                SetVisible(_noAssignedState, true);
                return;
            }

            SetVisible(_noAssignedState, false);
            ApplySemanticStateClass(_assignedContent, assigned.SemanticState);

            // Project name
            var projectNameLabel = new Label(assigned.ProjectName);
            projectNameLabel.AddToClassList("research__assigned__project-name");
            projectNameLabel.AddToClassList("text-heading");
            _assignedContent.Add(projectNameLabel);

            // Progress
            var progressRow = new VisualElement();
            progressRow.AddToClassList("research__assigned__progress-row");

            var progressLabel = new Label($"Progress: {assigned.Progress}");
            progressLabel.AddToClassList("text-body");
            progressRow.Add(progressLabel);

            _assignedContent.Add(progressRow);

            // Team and completion estimate
            var detailRow = new VisualElement();
            detailRow.AddToClassList("research__assigned__detail-row");

            if (!string.IsNullOrEmpty(assigned.AssignedTeam))
            {
                var teamLabel = new Label($"Team: {assigned.AssignedTeam}");
                teamLabel.AddToClassList("text-caption");
                detailRow.Add(teamLabel);
            }

            if (!string.IsNullOrEmpty(assigned.CompletionEstimate))
            {
                var estimateLabel = new Label($"Est. completion: {assigned.CompletionEstimate}");
                estimateLabel.AddToClassList("text-caption");
                detailRow.Add(estimateLabel);
            }

            _assignedContent.Add(detailRow);

            // [Placeholder] Assign Team button — Phase 6+ wiring
            var assignTeamBtn = new Button();
            assignTeamBtn.text = "[Placeholder] Assign Team";
            assignTeamBtn.AddToClassList("research__assigned__assign-btn");
            assignTeamBtn.AddToClassList("base-button");
            assignTeamBtn.AddToClassList("base-button--secondary");
            assignTeamBtn.SetEnabled(false);
            _assignedContent.Add(assignTeamBtn);
        }

        // ─── Private — state helpers ──────────────────────────────────────────────────

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

        // ─── Private — element helpers ────────────────────────────────────────────────

        private static VisualElement QueryElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            LogIfNull(element, name);
            return element;
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
            if (element == null || string.IsNullOrEmpty(semanticState))
            {
                return;
            }

            element.AddToClassList($"is-{semanticState}");
        }

        private static void LogIfNull(object element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"ResearchView: element '{name}' not found in UXML. " +
                    "This section will be silently skipped during Bind.");
            }
        }
    }
}
