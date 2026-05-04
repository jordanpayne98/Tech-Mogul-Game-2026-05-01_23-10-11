using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.Calendar
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Calendar screen.
    /// Builds a [Placeholder] static ViewModel with a month of events, wires click callbacks
    /// to ScreenRouter/ModalRouter, handles view mode toggling, and calls View.Bind().
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CalendarController
    {
        private readonly CalendarView  _view;
        private readonly IScreenRouter _screenRouter;
        private readonly IModalRouter  _modalRouter;

        private string _activeViewMode = "month";

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references and subscribes to all view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public CalendarController(
            CalendarView  view,
            IScreenRouter screenRouter,
            IModalRouter  modalRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;
            _modalRouter  = modalRouter;

            _view.OnEventClicked     += HandleEventClicked;
            _view.OnViewModeChanged  += HandleViewModeChanged;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// </summary>
        public void Initialize()
        {
            CalendarScreenViewModel viewModel = BuildPlaceholderViewModel(_activeViewMode);
            _view.Bind(viewModel);

            DebugLogger.Log(DebugCategory.UI,
                "CalendarController.Initialize: [Placeholder] ViewModel bound to CalendarView.");
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleEventClicked(string routeTarget)
        {
            if (string.IsNullOrEmpty(routeTarget))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"CalendarController: event clicked — route target '{routeTarget}'.");

            // Route to screen or open detail modal based on route prefix.
            if (routeTarget.StartsWith("modal."))
            {
                _modalRouter.OpenModal(routeTarget);
            }
            else if (routeTarget.StartsWith("screen."))
            {
                _screenRouter.OpenScreen(routeTarget);
            }
        }

        private void HandleViewModeChanged(string viewMode)
        {
            if (string.IsNullOrEmpty(viewMode) || viewMode == _activeViewMode)
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"CalendarController: view mode changed to '{viewMode}'.");

            _activeViewMode = viewMode;

            CalendarScreenViewModel viewModel = BuildPlaceholderViewModel(_activeViewMode);
            _view.Bind(viewModel);
        }

        // ─── Private — ViewModel builder ─────────────────────────────────────────────

        /// <summary>
        /// Builds a static [Placeholder] CalendarScreenViewModel for Phase 5.
        /// Generates 30 day cells for June 2027 with 12 events across all supported types.
        /// Generates 5 upcoming events for the upcoming events list.
        /// All values are hardcoded demo data — replace with service calls in Phase 6+.
        /// </summary>
        private static CalendarScreenViewModel BuildPlaceholderViewModel(string activeViewMode)
        {
            IReadOnlyList<CalendarEventViewModel> allEvents = BuildPlaceholderEvents();
            IReadOnlyList<CalendarDayViewModel>   days      = BuildPlaceholderDays(allEvents);
            IReadOnlyList<UpcomingEventRowViewModel> upcoming = BuildPlaceholderUpcomingEvents();

            var filterState = new CalendarFilterViewModel(
                selectedEventTypes: new List<string>(),
                hasActiveFilters:   false,
                activeFilterCount:  0);

            return new CalendarScreenViewModel(
                screenTitle:        "Calendar",
                screenSubtitle:     "[Placeholder] June 2027",
                isLoading:          false,
                hasError:           false,
                errorMessage:       string.Empty,
                emptyStateTitle:    "No Events",
                emptyStateBody:     "No events found for the current view or filter selection.",
                currentDateDisplay: "June 2027",
                activeViewMode:     activeViewMode,
                searchText:         string.Empty,
                days:               days,
                upcomingEvents:     upcoming,
                filterState:        filterState,
                hasNoEvents:        false,
                hasFilteredEmpty:   false);
        }

        /// <summary>
        /// Builds [Placeholder] event definitions covering all supported event types.
        /// 12 events spread across June 2027.
        /// </summary>
        private static IReadOnlyList<CalendarEventViewModel> BuildPlaceholderEvents()
        {
            return new List<CalendarEventViewModel>
            {
                // finance.payroll
                new CalendarEventViewModel(
                    id:              "event.payroll.2027_06",
                    title:           "[Placeholder] Monthly Payroll Run",
                    dateTime:        "01 Jun 2027",
                    category:        "finance.payroll",
                    priority:        "high",
                    relatedEntityId: string.Empty,
                    requiresDecision: false,
                    summary:         "[Placeholder] Monthly payroll processing. Ensure sufficient funds.",
                    routeTarget:     ScreenIds.Finance,
                    semanticState:   "default",
                    isClickable:     true),

                // contract.deadline
                new CalendarEventViewModel(
                    id:              "event.contract.deadline.2027_06_03",
                    title:           "[Placeholder] Acme Corp Contract Deadline",
                    dateTime:        "03 Jun 2027",
                    category:        "contract.deadline",
                    priority:        "critical",
                    relatedEntityId: "contract.acme_corp",
                    requiresDecision: true,
                    summary:         "[Placeholder] Final deadline for Acme Corp contract renewal. Decision required.",
                    routeTarget:     ScreenIds.Contracts,
                    semanticState:   "urgent",
                    isClickable:     true),

                // hiring.candidate_response
                new CalendarEventViewModel(
                    id:              "event.hiring.candidate.2027_06_05",
                    title:           "[Placeholder] Senior Developer — Response Due",
                    dateTime:        "05 Jun 2027",
                    category:        "hiring.candidate_response",
                    priority:        "high",
                    relatedEntityId: "candidate.dev_senior_001",
                    requiresDecision: true,
                    summary:         "[Placeholder] Candidate response expected for Senior Developer offer.",
                    routeTarget:     ScreenIds.Recruitment,
                    semanticState:   "default",
                    isClickable:     true),

                // product.target_release
                new CalendarEventViewModel(
                    id:              "event.product.release.alpha.2027_06_08",
                    title:           "[Placeholder] Alpha App — Target Release",
                    dateTime:        "08 Jun 2027",
                    category:        "product.target_release",
                    priority:        "high",
                    relatedEntityId: "product.alpha_app",
                    requiresDecision: false,
                    summary:         "[Placeholder] Planned release date for Alpha App v1.0.",
                    routeTarget:     ScreenIds.Products,
                    semanticState:   "default",
                    isClickable:     true),

                // research.completion
                new CalendarEventViewModel(
                    id:              "event.research.completion.2027_06_10",
                    title:           "[Placeholder] AI Module Research Complete",
                    dateTime:        "10 Jun 2027",
                    category:        "research.completion",
                    priority:        "normal",
                    relatedEntityId: "research.ai_module",
                    requiresDecision: false,
                    summary:         "[Placeholder] Estimated completion date for AI Module research stream.",
                    routeTarget:     ScreenIds.Research,
                    semanticState:   "default",
                    isClickable:     true),

                // market.competitor_report
                new CalendarEventViewModel(
                    id:              "event.market.competitor.2027_06_12",
                    title:           "[Placeholder] Monthly Competitor Report",
                    dateTime:        "12 Jun 2027",
                    category:        "market.competitor_report",
                    priority:        "normal",
                    relatedEntityId: string.Empty,
                    requiresDecision: false,
                    summary:         "[Placeholder] Monthly competitor activity and market share report.",
                    routeTarget:     ScreenIds.Competitors,
                    semanticState:   "default",
                    isClickable:     true),

                // finance.monthly_report
                new CalendarEventViewModel(
                    id:              "event.finance.report.2027_06_15",
                    title:           "[Placeholder] Monthly Finance Report",
                    dateTime:        "15 Jun 2027",
                    category:        "finance.monthly_report",
                    priority:        "normal",
                    relatedEntityId: string.Empty,
                    requiresDecision: false,
                    summary:         "[Placeholder] End-of-period financial summary and runway projection.",
                    routeTarget:     ScreenIds.Finance,
                    semanticState:   "default",
                    isClickable:     true),

                // infrastructure.support_warning
                new CalendarEventViewModel(
                    id:              "event.infra.warning.2027_06_18",
                    title:           "[Placeholder] Infrastructure Capacity Warning",
                    dateTime:        "18 Jun 2027",
                    category:        "infrastructure.support_warning",
                    priority:        "high",
                    relatedEntityId: "infrastructure.server_cluster_01",
                    requiresDecision: true,
                    summary:         "[Placeholder] Server capacity projected to reach 90% threshold. Upgrade recommended.",
                    routeTarget:     ScreenIds.Infrastructure,
                    semanticState:   "urgent",
                    isClickable:     true),

                // product.report
                new CalendarEventViewModel(
                    id:              "event.product.report.alpha.2027_06_20",
                    title:           "[Placeholder] Alpha App — Milestone Report",
                    dateTime:        "20 Jun 2027",
                    category:        "product.report",
                    priority:        "normal",
                    relatedEntityId: "product.alpha_app",
                    requiresDecision: false,
                    summary:         "[Placeholder] Development milestone progress report for Alpha App.",
                    routeTarget:     ScreenIds.Products,
                    semanticState:   "default",
                    isClickable:     true),

                // contract.deadline (second)
                new CalendarEventViewModel(
                    id:              "event.contract.deadline.2027_06_22",
                    title:           "[Placeholder] Beta Service Contract Deadline",
                    dateTime:        "22 Jun 2027",
                    category:        "contract.deadline",
                    priority:        "high",
                    relatedEntityId: "contract.beta_service",
                    requiresDecision: true,
                    summary:         "[Placeholder] Contract expiry for Beta Service provider. Renewal decision required.",
                    routeTarget:     ScreenIds.Contracts,
                    semanticState:   "urgent",
                    isClickable:     true),

                // product.launch_day
                new CalendarEventViewModel(
                    id:              "event.product.launch.alpha.2027_06_25",
                    title:           "[Placeholder] Alpha App — Launch Day",
                    dateTime:        "25 Jun 2027",
                    category:        "product.launch_day",
                    priority:        "critical",
                    relatedEntityId: "product.alpha_app",
                    requiresDecision: false,
                    summary:         "[Placeholder] Alpha App public launch. Coordinate marketing and support.",
                    routeTarget:     ScreenIds.Products,
                    semanticState:   "default",
                    isClickable:     true),

                // finance.payroll (end of month)
                new CalendarEventViewModel(
                    id:              "event.payroll.2027_06_30",
                    title:           "[Placeholder] End-of-Month Payroll Confirmation",
                    dateTime:        "30 Jun 2027",
                    category:        "finance.payroll",
                    priority:        "high",
                    relatedEntityId: string.Empty,
                    requiresDecision: false,
                    summary:         "[Placeholder] End-of-month payroll finalisation and fund disbursement.",
                    routeTarget:     ScreenIds.Finance,
                    semanticState:   "default",
                    isClickable:     true),
            };
        }

        /// <summary>
        /// Builds 30 day cells for June 2027. Days without events have empty event lists.
        /// Day 15 is marked as "today" for the [Placeholder] data.
        /// Days 1 and 30 have events assigned. Adjacent month overflow cells are not generated in Phase 5.
        /// </summary>
        private static IReadOnlyList<CalendarDayViewModel> BuildPlaceholderDays(
            IReadOnlyList<CalendarEventViewModel> allEvents)
        {
            // Map day number to events for quick lookup.
            var eventsByDay = new Dictionary<int, List<CalendarEventViewModel>>();

            // Assign events to days based on their date strings (format: "DD Jun 2027").
            foreach (CalendarEventViewModel ev in allEvents)
            {
                if (TryParseDayNumber(ev.DateTime, out int day))
                {
                    if (!eventsByDay.ContainsKey(day))
                    {
                        eventsByDay[day] = new List<CalendarEventViewModel>();
                    }

                    eventsByDay[day].Add(ev);
                }
            }

            var days = new List<CalendarDayViewModel>();

            for (int d = 1; d <= 30; d++)
            {
                bool isToday = (d == 15); // [Placeholder] — Day 15 is "today".
                IReadOnlyList<CalendarEventViewModel> dayEvents =
                    eventsByDay.TryGetValue(d, out List<CalendarEventViewModel> list)
                        ? list
                        : new List<CalendarEventViewModel>();

                string semanticState = isToday ? "today" : "normal";

                days.Add(new CalendarDayViewModel(
                    dayLabel:       d.ToString(),
                    isToday:        isToday,
                    isCurrentMonth: true,
                    events:         dayEvents,
                    semanticState:  semanticState));
            }

            return days;
        }

        /// <summary>
        /// Parses "DD Mon YYYY" format event date strings to extract the day number.
        /// Returns false if parsing fails.
        /// </summary>
        private static bool TryParseDayNumber(string dateTime, out int day)
        {
            day = 0;

            if (string.IsNullOrEmpty(dateTime))
            {
                return false;
            }

            string[] parts = dateTime.Split(' ');

            if (parts.Length < 1)
            {
                return false;
            }

            return int.TryParse(parts[0], out day);
        }

        /// <summary>
        /// Builds the [Placeholder] upcoming events list — 5 events drawn from the full event set.
        /// </summary>
        private static IReadOnlyList<UpcomingEventRowViewModel> BuildPlaceholderUpcomingEvents()
        {
            return new List<UpcomingEventRowViewModel>
            {
                new UpcomingEventRowViewModel(
                    id:              "event.contract.deadline.2027_06_03",
                    title:           "[Placeholder] Acme Corp Contract Deadline",
                    date:            "03 Jun 2027",
                    category:        "contract.deadline",
                    priority:        "critical",
                    requiresDecision: true,
                    semanticState:   "urgent",
                    isClickable:     true,
                    routeTarget:     ScreenIds.Contracts),

                new UpcomingEventRowViewModel(
                    id:              "event.hiring.candidate.2027_06_05",
                    title:           "[Placeholder] Senior Developer — Response Due",
                    date:            "05 Jun 2027",
                    category:        "hiring.candidate_response",
                    priority:        "high",
                    requiresDecision: true,
                    semanticState:   "default",
                    isClickable:     true,
                    routeTarget:     ScreenIds.Recruitment),

                new UpcomingEventRowViewModel(
                    id:              "event.product.release.alpha.2027_06_08",
                    title:           "[Placeholder] Alpha App — Target Release",
                    date:            "08 Jun 2027",
                    category:        "product.target_release",
                    priority:        "high",
                    requiresDecision: false,
                    semanticState:   "default",
                    isClickable:     true,
                    routeTarget:     ScreenIds.Products),

                new UpcomingEventRowViewModel(
                    id:              "event.infra.warning.2027_06_18",
                    title:           "[Placeholder] Infrastructure Capacity Warning",
                    date:            "18 Jun 2027",
                    category:        "infrastructure.support_warning",
                    priority:        "high",
                    requiresDecision: true,
                    semanticState:   "urgent",
                    isClickable:     true,
                    routeTarget:     ScreenIds.Infrastructure),

                new UpcomingEventRowViewModel(
                    id:              "event.product.launch.alpha.2027_06_25",
                    title:           "[Placeholder] Alpha App — Launch Day",
                    date:            "25 Jun 2027",
                    category:        "product.launch_day",
                    priority:        "critical",
                    requiresDecision: false,
                    semanticState:   "default",
                    isClickable:     true,
                    routeTarget:     ScreenIds.Products),
            };
        }
    }
}
