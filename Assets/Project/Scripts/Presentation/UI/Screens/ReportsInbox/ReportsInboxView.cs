using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all visual references for the Reports / Inbox screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates category rail items, report list rows, and the detail panel content.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Layout: three-panel inbox — category rail (left) | report list (centre) | detail panel (right).
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ReportsInboxView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from ReportsInboxScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a category rail item is clicked. Argument is the category's stable ID.</summary>
        public event Action<string> OnCategorySelected;

        /// <summary>Fired when a report row is clicked. Argument is the report's stable ID.</summary>
        public event Action<string> OnReportSelected;

        /// <summary>Fired when a detail panel action button is clicked. Argument is the action ID.</summary>
        public event Action<string> OnReportActionClicked;

        /// <summary>Fired when a related entity link is clicked. Argument is the entity's DrillDownRouteId.</summary>
        public event Action<string> OnRelatedEntityClicked;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;
        private readonly VisualElement _filteredEmptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label         _headerTitle;
        private readonly Label         _headerSubtitle;
        private readonly VisualElement _unreadBadge;
        private readonly Label         _unreadBadgeLabel;

        // ─── Category rail ───────────────────────────────────────────────────────────

        private readonly VisualElement _categoryList;

        // ─── Report list ─────────────────────────────────────────────────────────────

        private readonly ScrollView _reportList;

        // ─── Detail panel ────────────────────────────────────────────────────────────

        private readonly VisualElement _detailEmptyState;
        private readonly ScrollView    _detailContent;
        private readonly Label         _detailTitle;
        private readonly VisualElement _decisionBadge;
        private readonly Label         _detailDate;
        private readonly Label         _detailCategoryLabel;
        private readonly Label         _detailSummaryBody;
        private readonly VisualElement _detailKeyNumbersList;
        private readonly VisualElement _detailWhatChangedList;
        private readonly VisualElement _detailCauseList;
        private readonly VisualElement _detailRelatedList;
        private readonly Button        _actionArchive;
        private readonly Button        _actionPin;
        private readonly Button        _actionUnread;
        private readonly Button        _actionDelete;

        // ─── State/error text ────────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public ReportsInboxView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "ReportsInboxView: root VisualElement is null. View cannot be initialized.");

                // Provide a non-null fallback so callers can safely reference Root without crashing.
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

            _headerTitle      = root.Q<Label>("HeaderTitle");
            _headerSubtitle   = root.Q<Label>("HeaderSubtitle");
            _unreadBadge      = QueryElement(root, "UnreadBadge");
            _unreadBadgeLabel = root.Q<Label>("UnreadBadgeLabel");

            LogIfNull(_headerTitle,    "HeaderTitle");
            LogIfNull(_headerSubtitle, "HeaderSubtitle");

            // ── Category rail ────────────────────────────────────────────────────────

            _categoryList = QueryElement(root, "CategoryList");

            // ── Report list ──────────────────────────────────────────────────────────

            _reportList = root.Q<ScrollView>("ReportList");
            LogIfNull(_reportList, "ReportList");

            // ── Detail panel ─────────────────────────────────────────────────────────

            _detailEmptyState    = QueryElement(root, "DetailEmptyState");
            _detailContent       = root.Q<ScrollView>("DetailContent");
            _detailTitle         = root.Q<Label>("DetailTitle");
            _decisionBadge       = QueryElement(root, "DecisionBadge");
            _detailDate          = root.Q<Label>("DetailDate");
            _detailCategoryLabel = root.Q<Label>("DetailCategoryLabel");
            _detailSummaryBody   = root.Q<Label>("DetailSummaryBody");
            _detailKeyNumbersList  = QueryElement(root, "DetailKeyNumbersList");
            _detailWhatChangedList = QueryElement(root, "DetailWhatChangedList");
            _detailCauseList       = QueryElement(root, "DetailCauseList");
            _detailRelatedList     = QueryElement(root, "DetailRelatedList");

            LogIfNull(_detailContent,       "DetailContent");
            LogIfNull(_detailTitle,         "DetailTitle");
            LogIfNull(_detailDate,          "DetailDate");
            LogIfNull(_detailCategoryLabel, "DetailCategoryLabel");
            LogIfNull(_detailSummaryBody,   "DetailSummaryBody");

            // ── Action buttons ────────────────────────────────────────────────────────

            _actionArchive = root.Q<Button>("ActionArchive");
            _actionPin     = root.Q<Button>("ActionPin");
            _actionUnread  = root.Q<Button>("ActionUnread");
            _actionDelete  = root.Q<Button>("ActionDelete");

            LogIfNull(_actionArchive, "ActionArchive");
            LogIfNull(_actionPin,     "ActionPin");
            LogIfNull(_actionUnread,  "ActionUnread");
            LogIfNull(_actionDelete,  "ActionDelete");

            // ── Wire static action buttons ────────────────────────────────────────────

            WireActionButton(_actionArchive, ReportActionIds.Archive);
            WireActionButton(_actionPin,     ReportActionIds.Pin);
            WireActionButton(_actionUnread,  ReportActionIds.MarkUnread);
            WireActionButton(_actionDelete,  ReportActionIds.Delete);

            // ── State/error text ─────────────────────────────────────────────────────

            _errorMessage   = root.Q<Label>("ErrorMessage");
            _emptyStateTitle = root.Q<Label>("EmptyStateTitle");
            _emptyStateBody  = root.Q<Label>("EmptyStateBody");

            LogIfNull(_errorMessage,   "ErrorMessage");
            LogIfNull(_emptyStateTitle, "EmptyStateTitle");
            LogIfNull(_emptyStateBody,  "EmptyStateBody");
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Applies display data from the ViewModel.
        /// Handles loading, error, empty, and normal content states.
        /// Clears and rebuilds all dynamic lists on each call.
        /// </summary>
        public void Bind(ReportsInboxViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "ReportsInboxView.Bind: viewModel is null. Showing error state.");
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

            // ── Empty inbox state ────────────────────────────────────────────────────

            if (viewModel.HasEmptyInbox)
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

            // Unread badge
            bool showUnreadBadge = viewModel.UnreadCount > 0;
            SetVisible(_unreadBadge, showUnreadBadge);

            if (showUnreadBadge && _unreadBadgeLabel != null)
            {
                _unreadBadgeLabel.text = viewModel.UnreadCount.ToString();
            }

            // Category rail
            BuildCategoryRail(viewModel);

            // Report list
            BuildReportList(viewModel);

            // Filtered empty state
            SetVisible(_filteredEmptyState, viewModel.HasFilteredEmpty);

            // Detail panel
            BindDetailPanel(viewModel.SelectedReportDetail);
        }

        // ─── Private — category rail builder ─────────────────────────────────────────

        private void BuildCategoryRail(ReportsInboxViewModel viewModel)
        {
            if (_categoryList == null)
            {
                return;
            }

            _categoryList.Clear();

            if (viewModel.Categories == null)
            {
                return;
            }

            foreach (ReportCategoryViewModel cat in viewModel.Categories)
            {
                VisualElement item = CreateCategoryItem(cat, viewModel.ActiveCategoryId);
                _categoryList.Add(item);
            }
        }

        private VisualElement CreateCategoryItem(ReportCategoryViewModel cat, string activeCategoryId)
        {
            var row = new VisualElement();
            row.AddToClassList("reports-inbox__category-item");
            ApplySemanticStateClass(row, cat.SemanticState);

            bool isActive = cat.Id == activeCategoryId;

            if (isActive)
            {
                row.AddToClassList("is-active");
                row.AddToClassList("is-selected");
            }

            // Label
            var labelEl = new Label(cat.Label);
            labelEl.AddToClassList("reports-inbox__category-item-label");
            labelEl.AddToClassList("text-body");
            row.Add(labelEl);

            // Unread badge on category row
            if (!string.IsNullOrEmpty(cat.UnreadCount))
            {
                var badge = new Label(cat.UnreadCount);
                badge.AddToClassList("reports-inbox__category-item-badge");
                badge.AddToClassList("text-tiny");
                row.Add(badge);
            }
            else if (!string.IsNullOrEmpty(cat.Count))
            {
                var countEl = new Label(cat.Count);
                countEl.AddToClassList("reports-inbox__category-item-count");
                countEl.AddToClassList("text-tiny");
                row.Add(countEl);
            }

            // Click
            string catId = cat.Id;
            row.RegisterCallback<ClickEvent>(_ => OnCategorySelected?.Invoke(catId));

            return row;
        }

        // ─── Private — report list builder ───────────────────────────────────────────

        private void BuildReportList(ReportsInboxViewModel viewModel)
        {
            if (_reportList == null)
            {
                return;
            }

            _reportList.Clear();

            if (viewModel.Reports == null)
            {
                return;
            }

            foreach (ReportRowViewModel row in viewModel.Reports)
            {
                VisualElement rowEl = CreateReportRow(row, viewModel.SelectedReportId);
                _reportList.Add(rowEl);
            }
        }

        private VisualElement CreateReportRow(ReportRowViewModel report, string selectedReportId)
        {
            var row = new VisualElement();
            row.AddToClassList("reports-inbox__report-row");
            ApplySemanticStateClass(row, report.SemanticState);

            if (report.Id == selectedReportId)
            {
                row.AddToClassList("is-selected");
            }

            if (report.IsUnread)
            {
                row.AddToClassList("is-unread");
            }

            if (report.RequiresDecision)
            {
                row.AddToClassList("is-decision-required");
            }

            if (report.IsArchived)
            {
                row.AddToClassList("is-archived");
            }

            if (report.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            // Row header: title + meta
            var rowHeader = new VisualElement();
            rowHeader.AddToClassList("reports-inbox__report-row-header");

            var titleEl = new Label(report.Title);
            titleEl.AddToClassList("reports-inbox__report-row-title");
            titleEl.AddToClassList("text-body-strong");
            rowHeader.Add(titleEl);

            if (report.RequiresDecision)
            {
                var decisionPill = new Label("Decision");
                decisionPill.AddToClassList("pill");
                decisionPill.AddToClassList("pill--warning");
                decisionPill.AddToClassList("text-tiny");
                rowHeader.Add(decisionPill);
            }

            if (report.IsUnread)
            {
                var unreadDot = new VisualElement();
                unreadDot.AddToClassList("reports-inbox__unread-dot");
                rowHeader.Add(unreadDot);
            }

            row.Add(rowHeader);

            // Row meta: date + category pill
            var rowMeta = new VisualElement();
            rowMeta.AddToClassList("reports-inbox__report-row-meta");

            var dateEl = new Label(report.Date);
            dateEl.AddToClassList("reports-inbox__report-row-date");
            dateEl.AddToClassList("text-small");
            dateEl.AddToClassList("text-muted");
            rowMeta.Add(dateEl);

            var categoryPill = new Label(report.Category);
            categoryPill.AddToClassList("pill");
            categoryPill.AddToClassList("pill--info");
            categoryPill.AddToClassList("text-tiny");
            rowMeta.Add(categoryPill);

            row.Add(rowMeta);

            // Summary teaser
            if (!string.IsNullOrEmpty(report.Summary))
            {
                var summaryEl = new Label(report.Summary);
                summaryEl.AddToClassList("reports-inbox__report-row-summary");
                summaryEl.AddToClassList("text-caption");
                summaryEl.AddToClassList("text-muted");
                row.Add(summaryEl);
            }

            // Click
            if (report.IsClickable)
            {
                string reportId = report.Id;
                row.RegisterCallback<ClickEvent>(_ => OnReportSelected?.Invoke(reportId));
            }

            return row;
        }

        // ─── Private — detail panel builder ──────────────────────────────────────────

        private void BindDetailPanel(ReportDetailViewModel detail)
        {
            if (detail == null)
            {
                // No report selected — show the empty placeholder.
                SetVisible(_detailEmptyState, true);
                SetVisible(_detailContent,    false);
                return;
            }

            SetVisible(_detailEmptyState, false);
            SetVisible(_detailContent,    true);

            // Title
            if (_detailTitle != null)
            {
                _detailTitle.text = detail.Title;
            }

            // Decision badge
            SetVisible(_decisionBadge, detail.RequiresDecision);

            // Date and category
            if (_detailDate != null)
            {
                _detailDate.text = detail.Date;
            }

            if (_detailCategoryLabel != null)
            {
                _detailCategoryLabel.text = detail.Category;
            }

            // Summary
            if (_detailSummaryBody != null)
            {
                _detailSummaryBody.text = detail.Summary;
            }

            // Key numbers
            BuildDetailBulletList(_detailKeyNumbersList, detail.KeyNumbers, "reports-inbox__detail-key-number");

            // What changed
            BuildDetailBulletList(_detailWhatChangedList, detail.WhatChanged, "reports-inbox__detail-change-item");

            // Cause indicators
            BuildDetailBulletList(_detailCauseList, detail.CauseIndicators, "reports-inbox__detail-cause-item");

            // Related entities
            BuildRelatedEntitiesList(detail.RelatedEntities);

            // Semantic state on root detail content
            ApplySemanticStateClass(_detailContent, detail.SemanticState);
        }

        private static void BuildDetailBulletList(VisualElement container, IReadOnlyList<string> items, string itemClass)
        {
            if (container == null)
            {
                return;
            }

            container.Clear();

            if (items == null || items.Count == 0)
            {
                return;
            }

            foreach (string item in items)
            {
                var el = new Label(item);
                el.AddToClassList(itemClass);
                el.AddToClassList("text-body");
                container.Add(el);
            }
        }

        private void BuildRelatedEntitiesList(IReadOnlyList<ReportRelatedEntityViewModel> entities)
        {
            if (_detailRelatedList == null)
            {
                return;
            }

            _detailRelatedList.Clear();

            if (entities == null || entities.Count == 0)
            {
                return;
            }

            foreach (ReportRelatedEntityViewModel entity in entities)
            {
                VisualElement link = CreateRelatedEntityLink(entity);
                _detailRelatedList.Add(link);
            }
        }

        private VisualElement CreateRelatedEntityLink(ReportRelatedEntityViewModel entity)
        {
            var row = new VisualElement();
            row.AddToClassList("reports-inbox__related-entity");

            if (entity.IsClickable)
            {
                row.AddToClassList("is-clickable");
            }

            var typePill = new Label(entity.EntityType);
            typePill.AddToClassList("pill");
            typePill.AddToClassList("pill--info");
            typePill.AddToClassList("text-tiny");
            row.Add(typePill);

            var labelEl = new Label(entity.Label);
            labelEl.AddToClassList("reports-inbox__related-entity-label");
            labelEl.AddToClassList("text-body");
            row.Add(labelEl);

            if (entity.IsClickable)
            {
                string routeId = entity.DrillDownRouteId;
                row.RegisterCallback<ClickEvent>(_ => OnRelatedEntityClicked?.Invoke(routeId));
            }

            return row;
        }

        // ─── Private — action button wiring ──────────────────────────────────────────

        private void WireActionButton(Button button, string actionId)
        {
            if (button == null)
            {
                return;
            }

            button.RegisterCallback<ClickEvent>(_ => OnReportActionClicked?.Invoke(actionId));
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
                    $"ReportsInboxView: element '{name}' not found in UXML root.");
            }
        }
    }
}
