using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;

namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Pure C# class (not MonoBehaviour) that drives the Reports / Inbox screen.
    /// Builds a [Placeholder] static ViewModel with 10–15 reports across categories,
    /// handles category selection, report selection, action callbacks, and related entity navigation.
    /// Must not own core rules, save/load, or persistent state.
    /// [Placeholder] — Phase 5 uses static demo data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ReportsInboxController
    {
        private readonly ReportsInboxView _view;
        private readonly IScreenRouter    _screenRouter;

        // ── Selection state ──────────────────────────────────────────────────────────

        private string _activeCategoryId = ReportCategoryIds.All;
        private string _selectedReportId = string.Empty;

        // ── Placeholder report data store ────────────────────────────────────────────

        private readonly IReadOnlyList<ReportRowViewModel>     _allReports;
        private readonly IReadOnlyList<ReportCategoryViewModel> _categories;
        private readonly Dictionary<string, ReportDetailViewModel> _detailsByReportId;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Stores references, pre-builds placeholder data, and subscribes to view events.
        /// Call Initialize() after construction to bind the first ViewModel.
        /// </summary>
        public ReportsInboxController(ReportsInboxView view, IScreenRouter screenRouter)
        {
            _view         = view;
            _screenRouter = screenRouter;

            _allReports        = BuildPlaceholderReports();
            _categories        = BuildPlaceholderCategories();
            _detailsByReportId = BuildPlaceholderDetails();

            _view.OnCategorySelected    += HandleCategorySelected;
            _view.OnReportSelected      += HandleReportSelected;
            _view.OnReportActionClicked += HandleReportActionClicked;
            _view.OnRelatedEntityClicked += HandleRelatedEntityClicked;
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the [Placeholder] ViewModel and binds it to the View.
        /// Phase 5 shows static demo data only. Core simulation wiring is deferred to Phase 6+.
        /// Auto-selects the first report for detail preview.
        /// </summary>
        public void Initialize()
        {
            // Auto-select the first visible report for the detail panel.
            IReadOnlyList<ReportRowViewModel> filteredReports = FilterReports(_activeCategoryId);

            if (filteredReports.Count > 0 && string.IsNullOrEmpty(_selectedReportId))
            {
                _selectedReportId = filteredReports[0].Id;
            }

            RefreshView();
        }

        // ─── Event handlers ──────────────────────────────────────────────────────────

        private void HandleCategorySelected(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ReportsInboxController: category selected — '{categoryId}'.");

            _activeCategoryId = categoryId;

            // Auto-select first report in new category if current selection is not visible.
            IReadOnlyList<ReportRowViewModel> filtered = FilterReports(_activeCategoryId);
            bool selectedIsVisible = false;

            foreach (ReportRowViewModel row in filtered)
            {
                if (row.Id == _selectedReportId)
                {
                    selectedIsVisible = true;
                    break;
                }
            }

            if (!selectedIsVisible)
            {
                _selectedReportId = filtered.Count > 0 ? filtered[0].Id : string.Empty;
            }

            RefreshView();
        }

        private void HandleReportSelected(string reportId)
        {
            if (string.IsNullOrEmpty(reportId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ReportsInboxController: report selected — '{reportId}'.");

            _selectedReportId = reportId;
            RefreshView();
        }

        private void HandleReportActionClicked(string actionId)
        {
            if (string.IsNullOrEmpty(actionId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.UI,
                $"ReportsInboxController: action clicked — '{actionId}' on report '{_selectedReportId}'. " +
                $"[Placeholder] Archive/delete/pin actions update placeholder UI state only in Phase 5.");

            // [Placeholder] Phase 5 — actions update placeholder UI state only.
            // Phase 6+ will route these through Application/Core command handlers.
            RefreshView();
        }

        private void HandleRelatedEntityClicked(string routeId)
        {
            if (string.IsNullOrEmpty(routeId))
            {
                return;
            }

            DebugLogger.Log(DebugCategory.Navigation,
                $"ReportsInboxController: related entity clicked — navigating to '{routeId}'.");

            _screenRouter.OpenScreen(routeId);
        }

        // ─── Private — view refresh ───────────────────────────────────────────────────

        private void RefreshView()
        {
            IReadOnlyList<ReportRowViewModel> filteredReports = FilterReports(_activeCategoryId);

            _detailsByReportId.TryGetValue(_selectedReportId, out ReportDetailViewModel selectedDetail);

            int unreadCount    = CountUnread(_allReports);
            int decisionCount  = CountDecisionRequired(_allReports);
            bool hasEmptyInbox = _allReports.Count == 0;
            bool hasFiltered   = _allReports.Count > 0 && filteredReports.Count == 0;

            var viewModel = new ReportsInboxViewModel(
                screenTitle:          "Reports / Inbox",
                screenSubtitle:       "[Placeholder] Company reports and decision inbox — Month 3",
                isLoading:            false,
                hasError:             false,
                errorMessage:         string.Empty,
                emptyStateTitle:      "No Reports",
                emptyStateBody:       "Your inbox is empty. Reports will appear here as your company generates activity.",
                searchText:           string.Empty,
                categories:           _categories,
                activeCategoryId:     _activeCategoryId,
                reports:              filteredReports,
                selectedReportId:     _selectedReportId,
                selectedReportDetail: selectedDetail,
                hasEmptyInbox:        hasEmptyInbox,
                hasFilteredEmpty:     hasFiltered,
                unreadCount:          unreadCount,
                decisionRequiredCount: decisionCount);

            _view.Bind(viewModel);
        }

        // ─── Private — filtering helpers ─────────────────────────────────────────────

        private IReadOnlyList<ReportRowViewModel> FilterReports(string categoryId)
        {
            if (categoryId == ReportCategoryIds.All)
            {
                return _allReports;
            }

            var result = new List<ReportRowViewModel>();

            foreach (ReportRowViewModel row in _allReports)
            {
                if (MatchesCategory(row, categoryId))
                {
                    result.Add(row);
                }
            }

            return result;
        }

        private static bool MatchesCategory(ReportRowViewModel row, string categoryId)
        {
            return categoryId switch
            {
                ReportCategoryIds.RequiresDecision => row.RequiresDecision,
                ReportCategoryIds.Finance          => row.Category == "Finance",
                ReportCategoryIds.Products         => row.Category == "Products",
                ReportCategoryIds.Employees        => row.Category == "Employees",
                ReportCategoryIds.Teams            => row.Category == "Teams",
                ReportCategoryIds.Hiring           => row.Category == "Hiring",
                ReportCategoryIds.Market           => row.Category == "Market",
                ReportCategoryIds.Competitors      => row.Category == "Competitors",
                ReportCategoryIds.Infrastructure   => row.Category == "Infrastructure",
                ReportCategoryIds.Support          => row.Category == "Support",
                ReportCategoryIds.Contracts        => row.Category == "Contracts",
                ReportCategoryIds.Research         => row.Category == "Research",
                ReportCategoryIds.Archived         => row.IsArchived,
                _                                  => false
            };
        }

        private static int CountUnread(IReadOnlyList<ReportRowViewModel> reports)
        {
            int count = 0;

            foreach (ReportRowViewModel row in reports)
            {
                if (row.IsUnread && !row.IsArchived)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountDecisionRequired(IReadOnlyList<ReportRowViewModel> reports)
        {
            int count = 0;

            foreach (ReportRowViewModel row in reports)
            {
                if (row.RequiresDecision)
                {
                    count++;
                }
            }

            return count;
        }

        // ─── Private — placeholder data builders ──────────────────────────────────────

        private static IReadOnlyList<ReportCategoryViewModel> BuildPlaceholderCategories()
        {
            // 14 categories per GDD Section 6.
            return new List<ReportCategoryViewModel>
            {
                new ReportCategoryViewModel(ReportCategoryIds.All,              "All",                "12", "3",  "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.RequiresDecision, "Requires Decision",  "2",  "2",  "warning"),
                new ReportCategoryViewModel(ReportCategoryIds.Finance,          "Finance",            "2",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Products,         "Products",           "3",  "1",  "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Employees,        "Employees",          "1",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Teams,            "Teams",              "0",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Hiring,           "Hiring",             "1",  "1",  "warning"),
                new ReportCategoryViewModel(ReportCategoryIds.Market,           "Market",             "1",  "1",  "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Competitors,      "Competitors",        "1",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Infrastructure,   "Infrastructure",     "1",  "1",  "danger"),
                new ReportCategoryViewModel(ReportCategoryIds.Support,          "Support",            "1",  "",   "warning"),
                new ReportCategoryViewModel(ReportCategoryIds.Contracts,        "Contracts",          "1",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Research,         "Research",           "0",  "",   "normal"),
                new ReportCategoryViewModel(ReportCategoryIds.Archived,         "Archived",           "3",  "",   "normal"),
            };
        }

        private static IReadOnlyList<ReportRowViewModel> BuildPlaceholderReports()
        {
            // 12 active + 3 archived = 15 placeholder reports across GDD MVP types.
            // [Placeholder] Phase 5 static data only.
            return new List<ReportRowViewModel>
            {
                // Finance
                new ReportRowViewModel(
                    id:               "report.finance.month_2",
                    title:            "Monthly Finance Summary — Month 2",
                    date:             "Month 2, Week 4",
                    category:         "Finance",
                    summary:          "Revenue was $38,000. Operating costs exceeded projections by 12%. Burn rate increased from last month.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                new ReportRowViewModel(
                    id:               "report.finance.month_3",
                    title:            "Monthly Finance Summary — Month 3",
                    date:             "Month 3, Week 4",
                    category:         "Finance",
                    summary:          "Revenue reached $42,000. Cash runway is now 4.2 months at current burn rate.",
                    isUnread:         true,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                // Products
                new ReportRowViewModel(
                    id:               "report.product.alpha_launch",
                    title:            "Product Launch — Alpha v1.0",
                    date:             "Month 3, Week 2",
                    category:         "Products",
                    summary:          "Alpha v1.0 shipped with 3 critical bugs open. Initial user ratings average 3.1 stars.",
                    isUnread:         false,
                    requiresDecision: true,
                    isArchived:       false,
                    semanticState:    "warning",
                    isClickable:      true),

                new ReportRowViewModel(
                    id:               "report.product.alpha_milestone",
                    title:            "Product Milestone — Alpha Backend Complete",
                    date:             "Month 2, Week 3",
                    category:         "Products",
                    summary:          "Backend development milestone reached. Frontend integration is next.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "success",
                    isClickable:      true),

                new ReportRowViewModel(
                    id:               "report.product.alpha_monthly_perf",
                    title:            "Monthly Product Performance — Alpha v1.0",
                    date:             "Month 3, Week 4",
                    category:         "Products",
                    summary:          "Day 30 retention: 22%. Daily active users are growing at 3% week-over-week.",
                    isUnread:         true,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                // Hiring
                new ReportRowViewModel(
                    id:               "report.hiring.offer_response",
                    title:            "Candidate Offer Response — Jordan Lee",
                    date:             "Month 3, Week 3",
                    category:         "Hiring",
                    summary:          "Candidate Jordan Lee has accepted the offer. Start date is Month 4, Week 1.",
                    isUnread:         true,
                    requiresDecision: true,
                    isArchived:       false,
                    semanticState:    "warning",
                    isClickable:      true),

                // Market
                new ReportRowViewModel(
                    id:               "report.market.trend_q3",
                    title:            "Market Trend Report — Q3 Consumer Demand Shift",
                    date:             "Month 3, Week 1",
                    category:         "Market",
                    summary:          "Demand for mobile productivity tools grew 18% quarter-over-quarter. Desktop tools declined by 6%.",
                    isUnread:         true,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                // Competitors
                new ReportRowViewModel(
                    id:               "report.competitors.nova_launch",
                    title:            "Competitor Launch — NovaTech releases TaskFlow 2.0",
                    date:             "Month 3, Week 2",
                    category:         "Competitors",
                    summary:          "NovaTech launched TaskFlow 2.0 with AI-assisted scheduling. Media coverage was moderate.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                // Infrastructure
                new ReportRowViewModel(
                    id:               "report.infrastructure.server_warning",
                    title:            "Infrastructure Warning — Production Server Latency",
                    date:             "Month 3, Week 4",
                    category:         "Infrastructure",
                    summary:          "Average API response time increased to 840ms. 99th-percentile spikes reached 3.2s during peak hours.",
                    isUnread:         true,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "danger",
                    isClickable:      true),

                // Support
                new ReportRowViewModel(
                    id:               "report.support.bug_spike",
                    title:            "Support Report — Bug Report Spike",
                    date:             "Month 3, Week 3",
                    category:         "Support",
                    summary:          "Support ticket volume increased by 40% this week. Primary complaints relate to export feature failures.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "warning",
                    isClickable:      true),

                // Contracts
                new ReportRowViewModel(
                    id:               "report.contracts.meridian_complete",
                    title:            "Contract Completion — Meridian Corp Pilot",
                    date:             "Month 3, Week 1",
                    category:         "Contracts",
                    summary:          "Meridian Corp pilot contract delivered on time. Satisfaction score: 4.2/5.0.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "success",
                    isClickable:      true),

                // Employees
                new ReportRowViewModel(
                    id:               "report.employees.performance_q3",
                    title:            "Employee Performance Summary — Q3",
                    date:             "Month 3, Week 4",
                    category:         "Employees",
                    summary:          "Team output increased 8% vs Q2. One underperforming individual contributor identified.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       false,
                    semanticState:    "normal",
                    isClickable:      true),

                // Archived reports
                new ReportRowViewModel(
                    id:               "report.finance.month_1",
                    title:            "Monthly Finance Summary — Month 1",
                    date:             "Month 1, Week 4",
                    category:         "Finance",
                    summary:          "First month revenue: $12,000. Burn rate was $18,000.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       true,
                    semanticState:    "normal",
                    isClickable:      true),

                new ReportRowViewModel(
                    id:               "report.product.alpha_kickoff",
                    title:            "Product Milestone — Alpha Kickoff",
                    date:             "Month 1, Week 1",
                    category:         "Products",
                    summary:          "Alpha project officially started. Team of 3 assigned.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       true,
                    semanticState:    "normal",
                    isClickable:      true),

                new ReportRowViewModel(
                    id:               "report.contracts.pilot_signed",
                    title:            "Contract Signed — Meridian Corp Pilot",
                    date:             "Month 1, Week 2",
                    category:         "Contracts",
                    summary:          "Meridian Corp pilot agreement signed. Delivery target: Month 3.",
                    isUnread:         false,
                    requiresDecision: false,
                    isArchived:       true,
                    semanticState:    "normal",
                    isClickable:      true),
            };
        }

        private static Dictionary<string, ReportDetailViewModel> BuildPlaceholderDetails()
        {
            var details = new Dictionary<string, ReportDetailViewModel>();

            // ── Finance Month 2 ──────────────────────────────────────────────────────
            details["report.finance.month_2"] = new ReportDetailViewModel(
                reportId:        "report.finance.month_2",
                title:           "Monthly Finance Summary — Month 2",
                date:            "Month 2, Week 4",
                category:        "Finance",
                summary:         "Month 2 closed with $38,000 in revenue against $32,000 projected. " +
                                 "Operating costs came in at $44,800, exceeding budget by 12%. " +
                                 "The increase was concentrated in infrastructure and contractor fees.",
                keyNumbers:      new List<string>
                {
                    "Revenue: $38,000",
                    "Operating Costs: $44,800",
                    "Net Loss: −$6,800",
                    "Cash Runway: 5.1 months",
                    "Burn Rate: $6,800/month"
                },
                whatChanged:     new List<string>
                {
                    "Revenue increased $6,000 vs Month 1.",
                    "Infrastructure costs rose $3,200 due to additional server provisioning.",
                    "Contractor fees increased $1,600 vs Month 1."
                },
                causeIndicators: new List<string>
                {
                    "Server provisioning was triggered by Alpha backend milestone completing ahead of schedule.",
                    "Contractor hours increased to meet Alpha integration deadline."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.finance",   "Finance Overview",    "screen",       ScreenIds.Finance,        true),
                    new ReportRelatedEntityViewModel("entity.screen.products",  "Product Alpha",       "product",      ScreenIds.Products,       true),
                    new ReportRelatedEntityViewModel("entity.screen.infra",     "Infrastructure",      "screen",       ScreenIds.Infrastructure, true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Finance Month 3 ──────────────────────────────────────────────────────
            details["report.finance.month_3"] = new ReportDetailViewModel(
                reportId:        "report.finance.month_3",
                title:           "Monthly Finance Summary — Month 3",
                date:            "Month 3, Week 4",
                category:        "Finance",
                summary:         "Month 3 revenue reached $42,000. Operating costs held at $48,200. " +
                                 "Cash runway has declined to 4.2 months at current burn rate of $6,200/month.",
                keyNumbers:      new List<string>
                {
                    "Revenue: $42,000",
                    "Operating Costs: $48,200",
                    "Net Loss: −$6,200",
                    "Cash Runway: 4.2 months",
                    "Burn Rate: $6,200/month"
                },
                whatChanged:     new List<string>
                {
                    "Revenue grew $4,000 vs Month 2.",
                    "Contract revenue contribution: $12,000 (Meridian Corp pilot).",
                    "Cash runway shortened by 0.9 months vs Month 2."
                },
                causeIndicators: new List<string>
                {
                    "Meridian Corp contract completion added $12,000 to revenue.",
                    "Burn rate reduction is small; costs remain above revenue pace."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.finance",    "Finance Overview", "screen",   ScreenIds.Finance,   true),
                    new ReportRelatedEntityViewModel("entity.screen.contracts",  "Meridian Corp",    "contract", ScreenIds.Contracts, true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Product Alpha Launch ─────────────────────────────────────────────────
            details["report.product.alpha_launch"] = new ReportDetailViewModel(
                reportId:        "report.product.alpha_launch",
                title:           "Product Launch — Alpha v1.0",
                date:            "Month 3, Week 2",
                category:        "Products",
                summary:         "Alpha v1.0 launched to 450 early access users. Three critical bugs were open at launch. " +
                                 "Initial user ratings average 3.1 out of 5 stars. " +
                                 "Day-1 crash rate was 4.2%, above the 2.0% target.",
                keyNumbers:      new List<string>
                {
                    "Launch Users: 450",
                    "Day-1 Crash Rate: 4.2% (target: 2.0%)",
                    "Average Rating: 3.1 / 5.0",
                    "Critical Bugs Open at Launch: 3",
                    "Day-7 Retention: 34%"
                },
                whatChanged:     new List<string>
                {
                    "Product moved from Beta to v1.0 production release.",
                    "3 critical bugs promoted from backlog to P0 priority.",
                    "Support ticket volume increased 80% within 24 hours of launch."
                },
                causeIndicators: new List<string>
                {
                    "Crash rate spike traced to a data migration issue in the onboarding flow.",
                    "Rating drop correlated with export feature failures reported in support."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0",      "product", ScreenIds.Products,        true),
                    new ReportRelatedEntityViewModel("entity.screen.support",  "Support Reports",  "screen",  ScreenIds.Reports,         true),
                    new ReportRelatedEntityViewModel("entity.screen.teams",    "Engineering Team", "team",    ScreenIds.Teams,           true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: true,
                semanticState:   "warning");

            // ── Product Alpha Monthly Perf ────────────────────────────────────────────
            details["report.product.alpha_monthly_perf"] = new ReportDetailViewModel(
                reportId:        "report.product.alpha_monthly_perf",
                title:           "Monthly Product Performance — Alpha v1.0",
                date:            "Month 3, Week 4",
                category:        "Products",
                summary:         "Alpha v1.0 has 612 registered users after 30 days. Day-30 retention is 22%. " +
                                 "Daily active users grew 3% week-over-week. Average session duration: 8.4 minutes.",
                keyNumbers:      new List<string>
                {
                    "Registered Users: 612",
                    "Day-30 Retention: 22%",
                    "DAU Growth: +3% week-over-week",
                    "Avg Session Duration: 8.4 min",
                    "Active Critical Bugs Resolved: 2 of 3"
                },
                whatChanged:     new List<string>
                {
                    "User base grew from 450 to 612.",
                    "2 of 3 critical bugs resolved; 1 remains open (export issue).",
                    "Rating improved from 3.1 to 3.6 stars after bug fixes."
                },
                causeIndicators: new List<string>
                {
                    "Rating improvement correlates with the crash fix deployed in Week 3.",
                    "Retention below benchmark — export bug remains a likely factor."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0",   "product", ScreenIds.Products,   true),
                    new ReportRelatedEntityViewModel("entity.screen.support",  "Support",      "screen",  ScreenIds.Reports,    true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Hiring Offer Response ─────────────────────────────────────────────────
            details["report.hiring.offer_response"] = new ReportDetailViewModel(
                reportId:        "report.hiring.offer_response",
                title:           "Candidate Offer Response — Jordan Lee",
                date:            "Month 3, Week 3",
                category:        "Hiring",
                summary:         "Jordan Lee (Senior Backend Engineer) has accepted the offer. " +
                                 "Negotiated start date: Month 4, Week 1. " +
                                 "Offer package: $95,000 base + standard equity allocation.",
                keyNumbers:      new List<string>
                {
                    "Role: Senior Backend Engineer",
                    "Base Salary: $95,000/year",
                    "Start Date: Month 4, Week 1",
                    "Time to Hire: 6 weeks from first contact"
                },
                whatChanged:     new List<string>
                {
                    "Headcount will increase by 1 from Month 4.",
                    "Monthly payroll will increase by approximately $7,917."
                },
                causeIndicators: new List<string>
                {
                    "Hiring was initiated in response to Alpha backend milestone velocity concerns.",
                    "Candidate accepted after second negotiation round on start date."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.recruitment", "Recruitment",   "screen",   ScreenIds.Recruitment, true),
                    new ReportRelatedEntityViewModel("entity.screen.employees",   "Employees",     "screen",   ScreenIds.Employees,   true),
                    new ReportRelatedEntityViewModel("entity.screen.finance",     "Finance",       "screen",   ScreenIds.Finance,     true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: true,
                semanticState:   "warning");

            // ── Market Trend ─────────────────────────────────────────────────────────
            details["report.market.trend_q3"] = new ReportDetailViewModel(
                reportId:        "report.market.trend_q3",
                title:           "Market Trend Report — Q3 Consumer Demand Shift",
                date:            "Month 3, Week 1",
                category:        "Market",
                summary:         "Q3 data shows an 18% rise in demand for mobile productivity tools. " +
                                 "Desktop tools declined 6%. Cloud-native collaboration features saw the largest growth segment.",
                keyNumbers:      new List<string>
                {
                    "Mobile Productivity Demand Growth: +18% QoQ",
                    "Desktop Tool Demand: −6% QoQ",
                    "Cloud Collaboration Growth: +24% QoQ",
                    "Top Feature Request: Offline mode (43% of surveyed users)"
                },
                whatChanged:     new List<string>
                {
                    "Mobile demand category grew significantly above prior quarter baseline.",
                    "Desktop-first product positioning is seeing reduced market tailwind."
                },
                causeIndicators: new List<string>
                {
                    "Remote work trends are sustaining mobile-first demand patterns.",
                    "Enterprise buyers increasingly prefer cloud-native deployment models."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.market",      "Market Overview",  "screen", ScreenIds.Market,      true),
                    new ReportRelatedEntityViewModel("entity.screen.competitors", "Competitors",      "screen", ScreenIds.Competitors, true),
                    new ReportRelatedEntityViewModel("entity.screen.products",    "Products",         "screen", ScreenIds.Products,    true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Competitor Launch ─────────────────────────────────────────────────────
            details["report.competitors.nova_launch"] = new ReportDetailViewModel(
                reportId:        "report.competitors.nova_launch",
                title:           "Competitor Launch — NovaTech releases TaskFlow 2.0",
                date:            "Month 3, Week 2",
                category:        "Competitors",
                summary:         "NovaTech launched TaskFlow 2.0 with AI-assisted scheduling. " +
                                 "Media coverage was moderate. Launch pricing is $12/user/month — below our current $15 tier.",
                keyNumbers:      new List<string>
                {
                    "Competitor: NovaTech",
                    "Product: TaskFlow 2.0",
                    "Launch Price: $12/user/month",
                    "Our Price (equivalent tier): $15/user/month",
                    "Media Coverage Score: 3/10 (moderate)"
                },
                whatChanged:     new List<string>
                {
                    "NovaTech added AI scheduling as a differentiating feature.",
                    "Price gap between TaskFlow 2.0 and our product widened."
                },
                causeIndicators: new List<string>
                {
                    "NovaTech has been in stealth development for 6+ months based on prior reports.",
                    "Pricing is likely a launch promotion strategy rather than their long-term tier structure."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.competitors", "Competitors",   "screen", ScreenIds.Competitors, true),
                    new ReportRelatedEntityViewModel("entity.screen.market",      "Market",        "screen", ScreenIds.Market,      true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Infrastructure Warning ────────────────────────────────────────────────
            details["report.infrastructure.server_warning"] = new ReportDetailViewModel(
                reportId:        "report.infrastructure.server_warning",
                title:           "Infrastructure Warning — Production Server Latency",
                date:            "Month 3, Week 4",
                category:        "Infrastructure",
                summary:         "Average API response time increased to 840ms over the past 7 days. " +
                                 "99th-percentile spikes reached 3.2 seconds during peak evening hours. " +
                                 "This is above the 500ms SLA threshold.",
                keyNumbers:      new List<string>
                {
                    "Avg API Response Time: 840ms (SLA: 500ms)",
                    "99th Percentile Spike: 3.2s",
                    "Affected Time Windows: 18:00–22:00 daily",
                    "Current Server Tier: Standard — 2 instances",
                    "Estimated Upscale Cost: +$800/month"
                },
                whatChanged:     new List<string>
                {
                    "Response time increased 68% vs last 30-day baseline.",
                    "Peak-hour error rate increased from 0.3% to 1.8%."
                },
                causeIndicators: new List<string>
                {
                    "User growth since Alpha launch has increased concurrent session load.",
                    "No code deployments in the affected window — load-only cause suspected."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.infra",    "Infrastructure", "screen",  ScreenIds.Infrastructure, true),
                    new ReportRelatedEntityViewModel("entity.screen.finance",  "Finance",        "screen",  ScreenIds.Finance,        true),
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0",     "product", ScreenIds.Products,       true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "danger");

            // ── Support Bug Spike ─────────────────────────────────────────────────────
            details["report.support.bug_spike"] = new ReportDetailViewModel(
                reportId:        "report.support.bug_spike",
                title:           "Support Report — Bug Report Spike",
                date:            "Month 3, Week 3",
                category:        "Support",
                summary:         "Support ticket volume rose 40% week-over-week. " +
                                 "Export feature failures account for 62% of new tickets. " +
                                 "Average resolution time increased from 18 hours to 31 hours.",
                keyNumbers:      new List<string>
                {
                    "Ticket Volume: +40% week-over-week",
                    "Export Feature Tickets: 62% of total",
                    "Avg Resolution Time: 31h (prev: 18h)",
                    "Open Critical Tickets: 1",
                    "Escalated Tickets: 4"
                },
                whatChanged:     new List<string>
                {
                    "Total open tickets grew from 38 to 53.",
                    "Export-related tickets appeared after the v1.0 launch push.",
                    "Customer satisfaction score dipped to 3.3/5.0."
                },
                causeIndicators: new List<string>
                {
                    "Export failure bug was identified post-launch but not resolved before launch.",
                    "Support team headcount has not scaled with user growth."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0", "product", ScreenIds.Products,   true),
                    new ReportRelatedEntityViewModel("entity.screen.teams",    "Support Team", "team",  ScreenIds.Teams,      true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "warning");

            // ── Contract Completion ───────────────────────────────────────────────────
            details["report.contracts.meridian_complete"] = new ReportDetailViewModel(
                reportId:        "report.contracts.meridian_complete",
                title:           "Contract Completion — Meridian Corp Pilot",
                date:            "Month 3, Week 1",
                category:        "Contracts",
                summary:         "Meridian Corp pilot delivered on time. Final satisfaction score: 4.2/5.0. " +
                                 "Contract value: $12,000. Renewal discussion is now open.",
                keyNumbers:      new List<string>
                {
                    "Contract Value: $12,000",
                    "Satisfaction Score: 4.2 / 5.0",
                    "Delivery Date: On Time",
                    "Renewal Probability: High (flagged by account contact)"
                },
                whatChanged:     new List<string>
                {
                    "Contract revenue recognised in Month 3 ($12,000).",
                    "Renewal conversation initiated by Meridian Corp account manager."
                },
                causeIndicators: new List<string>
                {
                    "On-time delivery driven by dedicated contract team allocation in Month 2.",
                    "Satisfaction score slightly below 5/5 due to minor onboarding friction noted in feedback."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.contracts", "Contracts",         "screen",   ScreenIds.Contracts, true),
                    new ReportRelatedEntityViewModel("entity.screen.finance",   "Finance",           "screen",   ScreenIds.Finance,   true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "success");

            // ── Employee Performance ──────────────────────────────────────────────────
            details["report.employees.performance_q3"] = new ReportDetailViewModel(
                reportId:        "report.employees.performance_q3",
                title:           "Employee Performance Summary — Q3",
                date:            "Month 3, Week 4",
                category:        "Employees",
                summary:         "Overall team output increased 8% vs Q2. " +
                                 "One individual contributor in the backend team was flagged with below-target output. " +
                                 "Team morale survey average: 3.8/5.0.",
                keyNumbers:      new List<string>
                {
                    "Team Output Change: +8% vs Q2",
                    "Morale Survey Average: 3.8 / 5.0",
                    "Underperforming Contributors: 1",
                    "Headcount at End of Quarter: 7"
                },
                whatChanged:     new List<string>
                {
                    "Backend team output velocity increased after Alpha milestone.",
                    "One engineer flagged for performance below team baseline."
                },
                causeIndicators: new List<string>
                {
                    "Output increase attributed to cleared blockers post-Alpha milestone.",
                    "Underperformance flag raised by team lead; specific causes not yet investigated."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.employees", "Employees", "screen", ScreenIds.Employees, true),
                    new ReportRelatedEntityViewModel("entity.screen.teams",     "Teams",     "screen", ScreenIds.Teams,     true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Product Alpha Milestone ───────────────────────────────────────────────
            details["report.product.alpha_milestone"] = new ReportDetailViewModel(
                reportId:        "report.product.alpha_milestone",
                title:           "Product Milestone — Alpha Backend Complete",
                date:            "Month 2, Week 3",
                category:        "Products",
                summary:         "The Alpha backend development milestone was reached 4 days ahead of schedule. " +
                                 "Frontend integration sprint is now unblocked and can begin immediately.",
                keyNumbers:      new List<string>
                {
                    "Milestone: Alpha Backend Complete",
                    "Completed: 4 days ahead of schedule",
                    "Next Milestone: Frontend Integration",
                    "Estimated Frontend Sprint Duration: 3 weeks"
                },
                whatChanged:     new List<string>
                {
                    "Backend API coverage reached 100% of planned v1.0 endpoints.",
                    "Frontend integration is now unblocked."
                },
                causeIndicators: new List<string>
                {
                    "Additional contractor hours in Month 2 contributed to ahead-of-schedule delivery.",
                    "No major blockers encountered in the final backend sprint."
                },
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0",       "product", ScreenIds.Products, true),
                    new ReportRelatedEntityViewModel("entity.screen.teams",    "Engineering Team",  "team",    ScreenIds.Teams,    true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "success");

            // ── Archived: Finance Month 1 ────────────────────────────────────────────
            details["report.finance.month_1"] = new ReportDetailViewModel(
                reportId:        "report.finance.month_1",
                title:           "Monthly Finance Summary — Month 1",
                date:            "Month 1, Week 4",
                category:        "Finance",
                summary:         "[Archived] Month 1 finance summary. Revenue: $12,000. Burn: $18,000.",
                keyNumbers:      new List<string> { "Revenue: $12,000", "Burn: $18,000", "Net Loss: −$6,000" },
                whatChanged:     new List<string>(),
                causeIndicators: new List<string>(),
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.finance", "Finance", "screen", ScreenIds.Finance, true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Archived: Product Alpha Kickoff ──────────────────────────────────────
            details["report.product.alpha_kickoff"] = new ReportDetailViewModel(
                reportId:        "report.product.alpha_kickoff",
                title:           "Product Milestone — Alpha Kickoff",
                date:            "Month 1, Week 1",
                category:        "Products",
                summary:         "[Archived] Alpha project kickoff report. Team of 3 assigned to build the initial prototype.",
                keyNumbers:      new List<string> { "Team Size: 3", "Milestone: Kickoff" },
                whatChanged:     new List<string>(),
                causeIndicators: new List<string>(),
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.products", "Alpha v1.0", "product", ScreenIds.Products, true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            // ── Archived: Contract Signed ────────────────────────────────────────────
            details["report.contracts.pilot_signed"] = new ReportDetailViewModel(
                reportId:        "report.contracts.pilot_signed",
                title:           "Contract Signed — Meridian Corp Pilot",
                date:            "Month 1, Week 2",
                category:        "Contracts",
                summary:         "[Archived] Meridian Corp pilot agreement signed. Delivery target: Month 3.",
                keyNumbers:      new List<string> { "Contract Value: $12,000", "Target Delivery: Month 3" },
                whatChanged:     new List<string>(),
                causeIndicators: new List<string>(),
                relatedEntities: new List<ReportRelatedEntityViewModel>
                {
                    new ReportRelatedEntityViewModel("entity.screen.contracts", "Contracts", "screen", ScreenIds.Contracts, true),
                },
                actions:         BuildStandardActions(),
                requiresDecision: false,
                semanticState:   "normal");

            return details;
        }

        private static List<ReportActionViewModel> BuildStandardActions()
        {
            return new List<ReportActionViewModel>
            {
                new ReportActionViewModel(ReportActionIds.Archive,      "Archive",     "normal",  true),
                new ReportActionViewModel(ReportActionIds.Pin,          "Pin",         "normal",  true),
                new ReportActionViewModel(ReportActionIds.MarkUnread,   "Mark Unread", "normal",  true),
                new ReportActionViewModel(ReportActionIds.Delete,       "Delete",      "danger",  true),
            };
        }
    }
}
