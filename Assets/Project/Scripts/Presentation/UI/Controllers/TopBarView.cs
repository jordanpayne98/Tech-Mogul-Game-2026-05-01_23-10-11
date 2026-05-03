using Project.Application;
using Project.Core.Debugging;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Controllers
{
    /// <summary>
    /// View controller for the top status bar.
    /// Wires button interactions and speed control segment toggling.
    /// Metric and date values are set via explicit update methods.
    ///
    /// Speed control is visual-only in Phase 4 — no time system integration yet.
    /// The Search button opens the filter drawer via the RightDrawerRouter.
    /// </summary>
    public sealed class TopBarView
    {
        private const string IsActiveClass   = "is-active";
        private const string SegmentClass    = "segmented-control__segment";

        // Queried elements.
        private readonly Label         _cashValue;
        private readonly Label         _netValue;
        private readonly Label         _runwayValue;
        private readonly Label         _dateDisplay;
        private readonly VisualElement _continueButton;
        private readonly VisualElement _settingsButton;
        private readonly VisualElement _alertsButton;
        private readonly VisualElement _searchButton;
        private readonly VisualElement _speedControl;

        private readonly IScreenRouter      _screenRouter;
        private readonly IModalRouter       _modalRouter;
        private readonly IRightDrawerRouter _drawerRouter;

        /// <param name="topBarRoot">The TopStatusBar element from the shell UXML.</param>
        /// <param name="screenRouter">Screen router for navigation requests.</param>
        /// <param name="modalRouter">Modal router for modal requests.</param>
        /// <param name="drawerRouter">Right drawer router — search button opens filter drawer.</param>
        public TopBarView(VisualElement topBarRoot, IScreenRouter screenRouter, IModalRouter modalRouter, IRightDrawerRouter drawerRouter)
        {
            _screenRouter = screenRouter ?? throw new System.ArgumentNullException(nameof(screenRouter));
            _modalRouter  = modalRouter  ?? throw new System.ArgumentNullException(nameof(modalRouter));
            _drawerRouter = drawerRouter ?? throw new System.ArgumentNullException(nameof(drawerRouter));

            if (topBarRoot == null)
            {
                throw new System.ArgumentNullException(nameof(topBarRoot));
            }

            // ── Query elements by name ───────────────────────────────────────────────

            _cashValue      = topBarRoot.Q<Label>("MetricCashValue");
            _netValue       = topBarRoot.Q<Label>("MetricNetValue");
            _runwayValue    = topBarRoot.Q<Label>("MetricRunwayValue");
            _dateDisplay    = topBarRoot.Q<Label>("DateDisplay");
            _continueButton = topBarRoot.Q("ContinueButton");
            _settingsButton = topBarRoot.Q("SettingsButton");
            _alertsButton   = topBarRoot.Q("AlertsButton");
            _searchButton   = topBarRoot.Q("SearchButton");
            _speedControl   = topBarRoot.Q("SpeedControl");

            LogMissingElement(_cashValue,      "MetricCashValue");
            LogMissingElement(_netValue,       "MetricNetValue");
            LogMissingElement(_runwayValue,    "MetricRunwayValue");
            LogMissingElement(_dateDisplay,    "DateDisplay");
            LogMissingElement(_continueButton, "ContinueButton");
            LogMissingElement(_settingsButton, "SettingsButton");
            LogMissingElement(_alertsButton,   "AlertsButton");
            LogMissingElement(_searchButton,   "SearchButton");
            LogMissingElement(_speedControl,   "SpeedControl");

            RegisterCallbacks(topBarRoot);
        }

        // ─── Public API ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Updates the three metric display labels.
        /// Values should already be formatted for display (e.g. "GBP 512K").
        /// </summary>
        public void UpdateMetrics(string cash, string netMonth, string runway)
        {
            if (_cashValue   != null) { _cashValue.text   = cash;     }
            if (_netValue    != null) { _netValue.text    = netMonth; }
            if (_runwayValue != null) { _runwayValue.text = runway;   }
        }

        /// <summary>Updates the date display label.</summary>
        public void UpdateDate(string dateText)
        {
            if (_dateDisplay != null)
            {
                _dateDisplay.text = dateText;
            }
        }

        // ─── Private ────────────────────────────────────────────────────────────────

        private void RegisterCallbacks(VisualElement topBarRoot)
        {
            // Continue button — [Placeholder] no time system yet.
            _continueButton?.RegisterCallback<ClickEvent>(_ =>
            {
                DebugLogger.Log(DebugCategory.UI,
                    "TopBarView: Continue button clicked. [Placeholder] No time system integration yet.");
            });

            // Settings button — navigates to Settings screen.
            _settingsButton?.RegisterCallback<ClickEvent>(_ =>
            {
                DebugLogger.Log(DebugCategory.Navigation,
                    "TopBarView: Settings button clicked — navigating to screen.settings.");
                _screenRouter.OpenScreen(ScreenIds.Settings);
            });

            // Alerts button — [Placeholder] no alert system yet.
            _alertsButton?.RegisterCallback<ClickEvent>(_ =>
            {
                DebugLogger.Log(DebugCategory.UI,
                    "TopBarView: Alerts button clicked. [Placeholder] No alert system integration yet.");
            });

            // Search button — opens the filter drawer.
            _searchButton?.RegisterCallback<ClickEvent>(_ =>
            {
                DebugLogger.Log(DebugCategory.Navigation,
                    "TopBarView: Search button clicked — opening filter drawer.");
                _drawerRouter.OpenDrawer(DrawerIds.Filter);
            });

            // Speed control segments — visual-only toggle in Phase 4.
            if (_speedControl != null)
            {
                UQueryBuilder<VisualElement> segments = _speedControl.Query<VisualElement>(className: SegmentClass);
                segments.ForEach(segment =>
                {
                    segment.RegisterCallback<ClickEvent>(_ =>
                    {
                        // Remove active class from all siblings, then set on clicked.
                        _speedControl
                            .Query<VisualElement>(className: SegmentClass)
                            .ForEach(s => s.RemoveFromClassList(IsActiveClass));

                        segment.AddToClassList(IsActiveClass);

                        // Retrieve display text safely — segments are Labels in the UXML.
                        string segmentText = segment is Label lbl ? lbl.text : segment.name;
                        DebugLogger.Log(DebugCategory.UI,
                            $"TopBarView: Speed segment '{segmentText}' selected. [Placeholder] Visual only.");
                    });
                });
            }
        }

        private static void LogMissingElement(object element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    $"TopBarView: element '{name}' not found in topbar UXML. Some interactions will be unavailable.");
            }
        }
    }
}
