using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that owns all VisualElement references for the Calendar screen.
    /// Queries named VisualElements from the UXML root on construction.
    /// Programmatically generates calendar day cells, event chips, and upcoming event rows.
    /// Exposes click events and applies ViewModel data via Bind().
    /// Handles month/week view mode toggling via semantic class toggles.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 wires static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarView
    {
        // ─── Root ────────────────────────────────────────────────────────────────────

        /// <summary>The root VisualElement cloned from CalendarScreen.uxml.</summary>
        public VisualElement Root { get; }

        // ─── Click events ────────────────────────────────────────────────────────────

        /// <summary>Fired when a calendar event chip or upcoming row is clicked. Argument is the event's RouteTarget.</summary>
        public event Action<string> OnEventClicked;

        /// <summary>Fired when the month/week toggle changes. Argument is the new view mode ID ("month" or "week").</summary>
        public event Action<string> OnViewModeChanged;

        // ─── State containers ────────────────────────────────────────────────────────

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _loadingState;
        private readonly VisualElement _errorState;
        private readonly VisualElement _emptyState;

        // ─── Header ──────────────────────────────────────────────────────────────────

        private readonly Label _headerTitle;
        private readonly Label _headerSubtitle;
        private readonly Label _currentDateDisplay;

        // ─── Toolbar ─────────────────────────────────────────────────────────────────

        private readonly VisualElement _viewModeToggleMonth;
        private readonly VisualElement _viewModeToggleWeek;

        // ─── Calendar grid ───────────────────────────────────────────────────────────

        private readonly VisualElement _calendarGrid;

        // ─── Upcoming events list ────────────────────────────────────────────────────

        private readonly VisualElement _upcomingEventsList;

        // ─── State / error text ──────────────────────────────────────────────────────

        private readonly Label _errorMessage;
        private readonly Label _emptyStateTitle;
        private readonly Label _emptyStateBody;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries all named elements from the UXML root.
        /// Logs a warning for any missing element; missing elements are silently skipped during Bind.
        /// </summary>
        public CalendarView(VisualElement root)
        {
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "CalendarView: root VisualElement is null. View cannot be initialized.");

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

            _headerTitle        = root.Q<Label>("HeaderTitle");
            _headerSubtitle     = root.Q<Label>("HeaderSubtitle");
            _currentDateDisplay = root.Q<Label>("CurrentDateDisplay");

            LogIfNull(_headerTitle,        "HeaderTitle");
            LogIfNull(_headerSubtitle,     "HeaderSubtitle");
            LogIfNull(_currentDateDisplay, "CurrentDateDisplay");

            // ── Toolbar toggle buttons ───────────────────────────────────────────────

            _viewModeToggleMonth = QueryElement(root, "ViewModeToggleMonth");
            _viewModeToggleWeek  = QueryElement(root, "ViewModeToggleWeek");

            if (_viewModeToggleMonth != null)
            {
                _viewModeToggleMonth.RegisterCallback<ClickEvent>(_ => OnViewModeChanged?.Invoke("month"));
            }

            if (_viewModeToggleWeek != null)
            {
                _viewModeToggleWeek.RegisterCallback<ClickEvent>(_ => OnViewModeChanged?.Invoke("week"));
            }

            // ── Dynamic containers ───────────────────────────────────────────────────

            _calendarGrid       = QueryElement(root, "CalendarGrid");
            _upcomingEventsList = QueryElement(root, "UpcomingEventsList");

            // ── Text labels ──────────────────────────────────────────────────────────

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
        /// Clears and rebuilds the calendar grid and upcoming events list on each call.
        /// Applies view mode class toggles for month/week switching.
        /// </summary>
        public void Bind(CalendarScreenViewModel viewModel)
        {
            if (viewModel == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "CalendarView.Bind: viewModel is null. Showing error state.");
                ShowError("[Error] No calendar data available.");
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

            // ── Empty / filtered-empty state ─────────────────────────────────────────

            bool hasContent = (viewModel.Days != null && viewModel.Days.Count > 0)
                           || (viewModel.UpcomingEvents != null && viewModel.UpcomingEvents.Count > 0);

            if (!hasContent || viewModel.HasNoEvents)
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

            if (_currentDateDisplay != null)
            {
                _currentDateDisplay.text = viewModel.CurrentDateDisplay;
            }

            // View mode toggle active state
            ApplyViewModeToggle(viewModel.ActiveViewMode);

            // Calendar grid
            BuildCalendarGrid(viewModel);

            // Upcoming events list
            BuildUpcomingEventsList(viewModel);

            // Root filtered-empty state class
            if (viewModel.HasFilteredEmpty)
            {
                Root.AddToClassList("is-empty");
            }
            else
            {
                Root.RemoveFromClassList("is-empty");
            }
        }

        // ─── Private — view mode ─────────────────────────────────────────────────────

        private void ApplyViewModeToggle(string activeViewMode)
        {
            if (_viewModeToggleMonth != null)
            {
                if (activeViewMode == "month")
                {
                    _viewModeToggleMonth.AddToClassList("is-active");
                }
                else
                {
                    _viewModeToggleMonth.RemoveFromClassList("is-active");
                }
            }

            if (_viewModeToggleWeek != null)
            {
                if (activeViewMode == "week")
                {
                    _viewModeToggleWeek.AddToClassList("is-active");
                }
                else
                {
                    _viewModeToggleWeek.RemoveFromClassList("is-active");
                }
            }

            // Apply view mode class to the root for layout switching via USS.
            Root.RemoveFromClassList("calendar--month-view");
            Root.RemoveFromClassList("calendar--week-view");

            if (activeViewMode == "week")
            {
                Root.AddToClassList("calendar--week-view");
            }
            else
            {
                Root.AddToClassList("calendar--month-view");
            }
        }

        // ─── Private — calendar grid builder ────────────────────────────────────────

        private void BuildCalendarGrid(CalendarScreenViewModel viewModel)
        {
            if (_calendarGrid == null)
            {
                return;
            }

            _calendarGrid.Clear();

            if (viewModel.Days == null)
            {
                return;
            }

            foreach (CalendarDayViewModel day in viewModel.Days)
            {
                VisualElement dayCell = CreateDayCell(day);
                _calendarGrid.Add(dayCell);
            }
        }

        private VisualElement CreateDayCell(CalendarDayViewModel day)
        {
            var cell = new VisualElement();
            cell.AddToClassList("calendar__day-cell");

            ApplySemanticStateClass(cell, day.SemanticState);

            if (day.IsToday)
            {
                cell.AddToClassList("is-today");
            }

            if (!day.IsCurrentMonth)
            {
                cell.AddToClassList("is-overflow");
            }

            var dayLabel = new Label(day.DayLabel);
            dayLabel.AddToClassList("calendar__day-cell__number");
            dayLabel.AddToClassList("text-caption");
            cell.Add(dayLabel);

            if (day.Events != null && day.Events.Count > 0)
            {
                var chipsContainer = new VisualElement();
                chipsContainer.AddToClassList("calendar__day-cell__chips");

                // Show up to 3 event chips per day cell; overflow is indicated by a count label.
                const int maxVisibleChips = 3;
                int chipCount = 0;

                foreach (CalendarEventViewModel ev in day.Events)
                {
                    if (chipCount < maxVisibleChips)
                    {
                        VisualElement chip = CreateEventChip(ev);
                        chipsContainer.Add(chip);
                    }
                    else
                    {
                        int overflow = day.Events.Count - maxVisibleChips;
                        var overflowLabel = new Label($"+{overflow} more");
                        overflowLabel.AddToClassList("calendar__day-cell__overflow");
                        overflowLabel.AddToClassList("text-caption");
                        overflowLabel.AddToClassList("text-muted");
                        chipsContainer.Add(overflowLabel);
                        break;
                    }

                    chipCount++;
                }

                cell.Add(chipsContainer);
            }

            return cell;
        }

        private VisualElement CreateEventChip(CalendarEventViewModel ev)
        {
            var chip = new VisualElement();
            chip.AddToClassList("calendar__event-chip");
            ApplySemanticStateClass(chip, ev.SemanticState);

            if (ev.RequiresDecision)
            {
                chip.AddToClassList("has-decision");
            }

            if (ev.IsClickable)
            {
                chip.AddToClassList("is-clickable");
            }

            var titleEl = new Label(ev.Title);
            titleEl.AddToClassList("calendar__event-chip__title");
            titleEl.AddToClassList("text-caption");
            chip.Add(titleEl);

            if (ev.IsClickable && !string.IsNullOrEmpty(ev.RouteTarget))
            {
                string routeTarget = ev.RouteTarget;
                chip.RegisterCallback<ClickEvent>(_ => OnEventClicked?.Invoke(routeTarget));
            }

            return chip;
        }

        // ─── Private — upcoming events list builder ──────────────────────────────────

        private void BuildUpcomingEventsList(CalendarScreenViewModel viewModel)
        {
            if (_upcomingEventsList == null)
            {
                return;
            }

            _upcomingEventsList.Clear();

            IReadOnlyList<UpcomingEventRowViewModel> events = viewModel.UpcomingEvents;

            if (events == null || events.Count == 0)
            {
                var emptyRow = new Label("No upcoming events.");
                emptyRow.AddToClassList("text-body");
                emptyRow.AddToClassList("text-muted");
                _upcomingEventsList.Add(emptyRow);
                return;
            }

            foreach (UpcomingEventRowViewModel row in events)
            {
                VisualElement rowEl = CreateUpcomingEventRow(row);
                _upcomingEventsList.Add(rowEl);
            }
        }

        private VisualElement CreateUpcomingEventRow(UpcomingEventRowViewModel row)
        {
            var rowEl = new VisualElement();
            rowEl.AddToClassList("calendar__upcoming-row");
            ApplySemanticStateClass(rowEl, row.SemanticState);

            if (row.IsClickable)
            {
                rowEl.AddToClassList("is-clickable");
            }

            if (row.RequiresDecision)
            {
                rowEl.AddToClassList("has-decision");
            }

            // Date column
            var dateEl = new Label(row.Date);
            dateEl.AddToClassList("calendar__upcoming-row__date");
            dateEl.AddToClassList("text-caption");
            dateEl.AddToClassList("text-muted");

            // Title column
            var titleEl = new Label(row.Title);
            titleEl.AddToClassList("calendar__upcoming-row__title");
            titleEl.AddToClassList("text-body");

            // Category pill
            string pillModifier = ResolveCategoryPillModifier(row.Category);
            var categoryEl = new Label(FormatCategoryLabel(row.Category));
            categoryEl.AddToClassList("pill");
            categoryEl.AddToClassList(pillModifier);

            rowEl.Add(dateEl);
            rowEl.Add(titleEl);
            rowEl.Add(categoryEl);

            if (row.RequiresDecision)
            {
                var decisionTag = new Label("Decision Required");
                decisionTag.AddToClassList("pill");
                decisionTag.AddToClassList("pill--warning");
                rowEl.Add(decisionTag);
            }

            if (row.IsClickable && !string.IsNullOrEmpty(row.RouteTarget))
            {
                string routeTarget = row.RouteTarget;
                rowEl.RegisterCallback<ClickEvent>(_ => OnEventClicked?.Invoke(routeTarget));
            }

            return rowEl;
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
            if (string.IsNullOrEmpty(semanticState) || semanticState == "normal" || semanticState == "default")
            {
                return;
            }

            element.AddToClassList($"has-{semanticState}");
        }

        // ─── Private — category helpers ──────────────────────────────────────────────

        /// <summary>Maps event category stable IDs to pill modifier CSS classes.</summary>
        private static string ResolveCategoryPillModifier(string category)
        {
            return category switch
            {
                "finance.payroll"             => "pill--neutral",
                "finance.monthly_report"      => "pill--info",
                "contract.deadline"           => "pill--warning",
                "product.target_release"      => "pill--info",
                "product.launch_day"          => "pill--success",
                "product.report"              => "pill--info",
                "hiring.candidate_response"   => "pill--neutral",
                "research.completion"         => "pill--info",
                "market.competitor_report"    => "pill--neutral",
                "infrastructure.support_warning" => "pill--warning",
                _                             => "pill--neutral"
            };
        }

        /// <summary>Formats a category stable ID into a human-readable short label for pills.</summary>
        private static string FormatCategoryLabel(string category)
        {
            return category switch
            {
                "finance.payroll"             => "Payroll",
                "finance.monthly_report"      => "Finance Report",
                "contract.deadline"           => "Contract",
                "product.target_release"      => "Product Release",
                "product.launch_day"          => "Launch Day",
                "product.report"              => "Product Report",
                "hiring.candidate_response"   => "Hiring",
                "research.completion"         => "Research",
                "market.competitor_report"    => "Market Report",
                "infrastructure.support_warning" => "Infrastructure",
                _                             => category
            };
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
                    $"CalendarView: element '{name}' not found in UXML root.");
            }
        }
    }
}
