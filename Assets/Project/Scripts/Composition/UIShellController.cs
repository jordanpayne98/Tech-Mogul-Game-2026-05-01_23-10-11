using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using Project.Presentation.UI.Components;
using Project.Presentation.UI.Controllers;
using Project.Presentation.UI.Modals;
using Project.Presentation.UI.Routing;
using Project.Presentation.UI.Screens.Calendar;
using Project.Presentation.UI.Screens.Company;
using Project.Presentation.UI.Screens.CompanyCreation;
using Project.Presentation.UI.Screens.Competitors;
using Project.Presentation.UI.Screens.Employees;
using Project.Presentation.UI.Screens.Contracts;
using Project.Presentation.UI.Screens.FounderPortal;
using Project.Presentation.UI.Screens.Finance;
using Project.Presentation.UI.Screens.Market;
using Project.Presentation.UI.Screens.Products;
using Project.Presentation.UI.Screens.ProductDetail;
using Project.Presentation.UI.Screens.RecruitmentHub;
using Project.Presentation.UI.Screens.Research;
using Project.Presentation.UI.Screens.ReportsInbox;
using Project.Presentation.UI.Screens.Settings;
using Project.Presentation.UI.Screens.Teams;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Composition
{
    /// <summary>
    /// Composition MonoBehaviour that bootstraps the UI shell.
    /// Lives on the UIShell GameObject alongside the UIDocument.
    ///
    /// Responsibilities:
    ///   - Query named host elements from the shell UXML.
    ///   - Create ScreenRouter, ModalRouter, and RightDrawerRouter.
    ///   - Register modal factories for all known modal IDs.
    ///   - Register drawer factories for all known drawer IDs.
    ///   - Create SidebarView and TopBarView.
    ///   - Provide ShowWizard / HideWizard for wizard shell show/hide.
    ///   - Navigate to the default screen on startup.
    ///
    /// Initialized by GameBootstrapper.InitializePhase4UI() after Phase 3.
    /// Must not contain core project rules.
    /// </summary>
    public sealed class UIShellController : MonoBehaviour
    {
        // ─── Exposed router references ───────────────────────────────────────────────

        /// <summary>Screen router — expose for other Composition code.</summary>
        public IScreenRouter ScreenRouter { get; private set; }

        /// <summary>Modal router — expose for other Composition code.</summary>
        public IModalRouter ModalRouter { get; private set; }

        /// <summary>Right drawer router — expose for other Composition code.</summary>
        public IRightDrawerRouter DrawerRouter { get; private set; }

        // ─── Serialized screen assets ────────────────────────────────────────────────

        [SerializeField] private VisualTreeAsset _founderPortalUxml;
        [SerializeField] private StyleSheet      _founderPortalUss;

        [SerializeField] private VisualTreeAsset _contractsUxml;
        [SerializeField] private StyleSheet      _contractsUss;

        [SerializeField] private VisualTreeAsset _financeUxml;
        [SerializeField] private StyleSheet      _financeUss;

        [SerializeField] private VisualTreeAsset _employeesUxml;
        [SerializeField] private StyleSheet      _employeesUss;

        [SerializeField] private VisualTreeAsset _companyUxml;
        [SerializeField] private StyleSheet      _companyUss;

        [SerializeField] private VisualTreeAsset _teamsUxml;
        [SerializeField] private StyleSheet      _teamsUss;

        [SerializeField] private VisualTreeAsset _recruitmentHubUxml;
        [SerializeField] private StyleSheet      _recruitmentHubUss;

        [SerializeField] private VisualTreeAsset _marketUxml;
        [SerializeField] private StyleSheet      _marketUss;

        [SerializeField] private VisualTreeAsset _competitorsUxml;
        [SerializeField] private StyleSheet      _competitorsUss;

        [SerializeField] private VisualTreeAsset _calendarUxml;
        [SerializeField] private StyleSheet      _calendarUss;

        [SerializeField] private VisualTreeAsset _productDetailUxml;
        [SerializeField] private StyleSheet      _productDetailUss;


        [SerializeField] private VisualTreeAsset _productsUxml;
        [SerializeField] private StyleSheet      _productsUss;

        [SerializeField] private VisualTreeAsset _researchUxml;
        [SerializeField] private StyleSheet      _researchUss;

        [SerializeField] private VisualTreeAsset _reportsInboxUxml;
        [SerializeField] private StyleSheet      _reportsInboxUss;

        [SerializeField] private VisualTreeAsset _settingsUxml;
        [SerializeField] private StyleSheet      _settingsUss;

        // ─── Private references ──────────────────────────────────────────────────────

        private SidebarView           _sidebarView;
        private TopBarView            _topBarView;
        private VisualElement         _shellRoot;
        private VisualElement         _wizardContainer;
        private RightDrawerRouter     _drawerRouterImpl;
        private CompanyCreationView   _companyCreationView;
        private GameSessionContext    _sessionContext;

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets up the full shell: routers, views, and default navigation.
        /// Called once by GameBootstrapper after Phase 3 is complete.
        /// </summary>
        public void Initialize(UIDocument uiDocument)
        {
            if (uiDocument == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "UIShellController.Initialize: UIDocument is null. Shell cannot be initialized.", this);
                return;
            }

            VisualElement root = uiDocument.rootVisualElement;
            if (root == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "UIShellController.Initialize: rootVisualElement is null. Shell cannot be initialized.", this);
                return;
            }

            _shellRoot = root;

            // ── Step 0: Ensure rootVisualElement and TemplateContainer fill the viewport ─
            // UIShellLayoutFixer handles this in OnEnable(), but component execution order
            // may cause it to run before UIDocument clones the UXML tree. Apply here as a
            // guaranteed fallback since the tree is always populated at this point.

            root.style.flexGrow = 1;
            for (int i = 0; i < root.childCount; i++)
            {
                root[i].style.flexGrow = 1;
            }

            // ── Step 1: Query host elements ──────────────────────────────────────────

            VisualElement screenHost       = root.Q("ScreenHost");
            VisualElement sidebarRoot      = root.Q("SidebarNavigation");
            VisualElement topBarRoot       = root.Q("TopStatusBar");
            VisualElement modalHost        = root.Q("ModalHost");
            VisualElement rightDrawerHost  = root.Q("RightDrawerHost");

            LogMissingHost(screenHost,      "ScreenHost");
            LogMissingHost(sidebarRoot,     "SidebarNavigation");
            LogMissingHost(topBarRoot,      "TopStatusBar");
            LogMissingHost(modalHost,       "ModalHost");
            LogMissingHost(rightDrawerHost, "RightDrawerHost");

            if (screenHost == null || sidebarRoot == null || topBarRoot == null)
            {
                DebugLogger.LogError(DebugCategory.Bootstrap,
                    "UIShellController.Initialize: one or more required shell elements are missing. " +
                    "Navigation will not function. Check MainShell.uxml element names.", this);
                return;
            }

            // ── Step 2: Create routers ───────────────────────────────────────────────

            var screenRouter = new ScreenRouter(screenHost);
            var modalRouter  = new ModalRouter(modalHost ?? new VisualElement());

            _drawerRouterImpl = new RightDrawerRouter(rightDrawerHost ?? new VisualElement());

            ScreenRouter = screenRouter;
            ModalRouter  = modalRouter;
            DrawerRouter = _drawerRouterImpl;

            DebugLogger.Log(DebugCategory.Bootstrap,
                "UIShellController: routers created.", this);

            // ── Step 2b: Register screen factories ──────────────────────────────────

            RegisterScreenFactories(screenRouter);

            DebugLogger.Log(DebugCategory.Bootstrap,
                "UIShellController: screen factories registered.", this);

            // ── Step 3: Register modal factories ────────────────────────────────────

            RegisterModalFactories(modalRouter);

            DebugLogger.Log(DebugCategory.Bootstrap,
                "UIShellController: modal factories registered.", this);

            // ── Step 4: Register drawer factories ───────────────────────────────────

            RegisterDrawerFactories(_drawerRouterImpl);

            DebugLogger.Log(DebugCategory.Bootstrap,
                "UIShellController: drawer factories registered.", this);

            // ── Step 5: Create views ─────────────────────────────────────────────────

            _sidebarView = new SidebarView(sidebarRoot, screenRouter);
            _topBarView  = new TopBarView(topBarRoot, screenRouter, modalRouter, _drawerRouterImpl);

            DebugLogger.Log(DebugCategory.Bootstrap,
                "UIShellController: SidebarView and TopBarView created.", this);

            // ── Step 6: Navigate to default screen ───────────────────────────────────

            screenRouter.OpenScreen(ScreenIds.Portal);

            DebugLogger.Log(DebugCategory.Bootstrap,
                $"UIShellController: initialized. Default screen: '{ScreenIds.Portal}'.", this);
        }

        /// <summary>
        /// Shows the wizard shell for the given wizard type.
        /// When wizardType is "company_creation", builds the real Company Creation wizard.
        /// Other wizard types fall through to a 3-step placeholder layout.
        /// </summary>
        public void ShowWizard(string wizardType)
        {
            if (_shellRoot == null)
            {
                DebugLogger.LogError(DebugCategory.UI,
                    "UIShellController.ShowWizard: shell root is null. Cannot show wizard.", this);
                return;
            }

            // Hide the existing shell content.
            VisualElement shellElement = _shellRoot.Q("Root");
            if (shellElement != null)
            {
                shellElement.AddToClassList("hidden");
            }

            if (wizardType == "company_creation")
            {
                // ── Real Company Creation wizard ─────────────────────────────────────
                var controller = new CompanyCreationController(
                    onCloseRequested:   HideWizard,
                    onConfirmRequested: OnCompanyCreationConfirmed);

                _companyCreationView = new CompanyCreationView(controller);

                // Load and apply the screen-specific USS.
                var uss = Resources.Load<StyleSheet>("USS/CompanyCreationScreen");
                if (uss != null)
                {
                    _companyCreationView.Root.styleSheets.Add(uss);
                }
                else
                {
                    DebugLogger.LogWarning(DebugCategory.UI,
                        "UIShellController.ShowWizard: CompanyCreationScreen.uss not found in Resources. " +
                        "Ensure the USS is placed in a Resources folder or referenced via PanelSettings.", this);
                }

                _wizardContainer = _companyCreationView.Root;
                _shellRoot.Add(_wizardContainer);

                // Initialize fires the first ViewModel event and renders the first step.
                controller.Initialize();

                DebugLogger.Log(DebugCategory.UI,
                    "UIShellController.ShowWizard: CompanyCreationView shown.", this);
            }
            else
            {
                // ── Fallback: Phase 4 placeholder wizard ─────────────────────────────
                var wizard = new WizardShellView();

                wizard.SetHeader("NEW GAME", "Company Setup", 1, 3);

                var steps = new List<WizardStepData>
                {
                    new WizardStepData("Placeholder Step A", "active"),
                    new WizardStepData("Placeholder Step B", "available"),
                    new WizardStepData("Placeholder Step C", "locked")
                };
                wizard.SetSteps(steps);

                var mainPlaceholder = new VisualElement();
                mainPlaceholder.AddToClassList("wizard-step-heading");

                var mainHeading = new Label($"[Placeholder] {wizardType}");
                mainHeading.AddToClassList("wizard-step-heading__title");
                mainHeading.AddToClassList("text-heading");
                mainPlaceholder.Add(mainHeading);

                var mainSubtitle = new Label("This wizard will be implemented in a later phase.");
                mainSubtitle.AddToClassList("wizard-step-heading__subtitle");
                mainSubtitle.AddToClassList("text-body");
                mainPlaceholder.Add(mainSubtitle);

                wizard.SetMainContent(mainPlaceholder);

                var previewPlaceholder = new VisualElement();
                var previewTitle = new Label("Preview");
                previewTitle.AddToClassList("text-heading");
                previewPlaceholder.Add(previewTitle);

                var previewMessage = new Label("Select options to see a preview.");
                previewMessage.AddToClassList("text-body");
                previewPlaceholder.Add(previewMessage);

                wizard.SetPreviewContent(previewPlaceholder);
                wizard.SetContinueEnabled(false);
                wizard.OnCancelRequested = HideWizard;

                _wizardContainer = wizard.Root;
                _shellRoot.Add(_wizardContainer);

                DebugLogger.Log(DebugCategory.UI,
                    $"UIShellController.ShowWizard: wizard type '{wizardType}' shown. [Placeholder] 3-step layout preview.", this);
            }
        }

        /// <summary>
        /// Hides the wizard and restores the main shell.
        /// Disposes any active CompanyCreationView to unsubscribe event handlers.
        /// </summary>
        public void HideWizard()
        {
            if (_shellRoot == null)
            {
                return;
            }

            // Dispose the Company Creation view if active, to unsubscribe events.
            if (_companyCreationView != null)
            {
                _companyCreationView.Dispose();
                _companyCreationView = null;
            }

            // Remove wizard container from root.
            if (_wizardContainer != null && _wizardContainer.parent != null)
            {
                _wizardContainer.RemoveFromHierarchy();
                _wizardContainer = null;
            }

            // Restore the main shell element.
            VisualElement shellElement = _shellRoot.Q("Root");
            if (shellElement != null)
            {
                shellElement.RemoveFromClassList("hidden");
            }

            DebugLogger.Log(DebugCategory.UI,
                "UIShellController.HideWizard: wizard removed, shell restored.", this);
        }

        /// <summary>
        /// Updates the session context reference so that factory closures can access
        /// the live session when they create screen controllers.
        /// Called by GameBootstrapper after GameCompositionRoot.BindSession() completes.
        /// </summary>
        /// <param name="context">The newly built session context. Must not be null.</param>
        public void RebindSession(GameSessionContext context)
        {
            _sessionContext = context;
            DebugLogger.Log(DebugCategory.Bootstrap,
                "[UIShellController] Rebound to GameSessionContext.", this);
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Registers screen factories for all screens with real implementations.
        /// Unregistered screen IDs will fall back to PlaceholderScreenView automatically.
        /// Phase 5A registers the Founder Portal screen factory.
        /// Phase 5B-3 registers the Company screen factory.
        /// Phase 5C-3 registers the Employees screen factory.
        /// Phase 5D-3 registers the Recruitment Hub screen factory.
        /// Phase 5E-3 registers the Teams screen factory.
        /// Phase 5H-3 registers the Contracts screen factory.
        /// Phase 5N-3 registers the Calendar screen factory.
        /// Phase 5J-3 registers the Market screen factory.
        /// Phase 5M-3 registers the Reports / Inbox screen factory.
        /// Phase 5O-3 registers the Settings screen factory.
        /// </summary>
        private void RegisterScreenFactories(ScreenRouter screenRouter)
        {
            // ── screen.portal — Founder Portal ───────────────────────────────────────

            if (_founderPortalUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _founderPortalUxml is not assigned. " +
                    "screen.portal will fall back to PlaceholderScreenView. " +
                    "Assign FounderPortalScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset founderPortalUxml = _founderPortalUxml;
                StyleSheet      founderPortalUss  = _founderPortalUss;
                IScreenRouter   iScreenRouter     = ScreenRouter;
                IModalRouter    iModalRouter      = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Portal, () =>
                {
                    VisualElement root = founderPortalUxml.Instantiate();

                    if (founderPortalUss != null)
                    {
                        root.styleSheets.Add(founderPortalUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _founderPortalUss is not assigned. " +
                            "FounderPortalScreen.uss will not be applied. " +
                            "Assign FounderPortalScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new FounderPortalView(root);
                    var controller = new FounderPortalController(view, iScreenRouter, iModalRouter);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.teams — Teams Management ──────────────────────────────────────

            if (_teamsUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _teamsUxml is not assigned. " +
                    "screen.teams will fall back to PlaceholderScreenView. " +
                    "Assign TeamsScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset teamsUxml    = _teamsUxml;
                StyleSheet      teamsUss     = _teamsUss;
                IScreenRouter   iScreenRouter = ScreenRouter;
                IModalRouter    iModalRouter  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Teams, () =>
                {
                    VisualElement root = teamsUxml.Instantiate();

                    if (teamsUss != null)
                    {
                        root.styleSheets.Add(teamsUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _teamsUss is not assigned. " +
                            "TeamsScreen.uss will not be applied. " +
                            "Assign TeamsScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new TeamsView(root);
                    var controller = new TeamsController(view, iScreenRouter, iModalRouter);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.recruitment — Recruitment Hub ─────────────────────────────────

            if (_recruitmentHubUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _recruitmentHubUxml is not assigned. " +
                    "screen.recruitment will fall back to PlaceholderScreenView. " +
                    "Assign RecruitmentHubScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset recruitmentHubUxml = _recruitmentHubUxml;
                StyleSheet      recruitmentHubUss  = _recruitmentHubUss;
                IScreenRouter   iScreenRouterRec   = ScreenRouter;
                IModalRouter    iModalRouterRec    = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Recruitment, () =>
                {
                    VisualElement root = recruitmentHubUxml.Instantiate();

                    if (recruitmentHubUss != null)
                    {
                        root.styleSheets.Add(recruitmentHubUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _recruitmentHubUss is not assigned. " +
                            "RecruitmentHubScreen.uss will not be applied. " +
                            "Assign RecruitmentHubScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new RecruitmentHubView(root);
                    var controller = new RecruitmentHubController(view, iScreenRouterRec, iModalRouterRec);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.finance — Finance Overview ────────────────────────────────────

            if (_financeUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _financeUxml is not assigned. " +
                    "screen.finance will fall back to PlaceholderScreenView. " +
                    "Assign FinanceScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset financeUxml      = _financeUxml;
                StyleSheet      financeUss       = _financeUss;
                IScreenRouter   iScreenRouterFin = ScreenRouter;

                screenRouter.RegisterScreen(ScreenIds.Finance, () =>
                {
                    VisualElement root = financeUxml.Instantiate();

                    if (financeUss != null)
                    {
                        root.styleSheets.Add(financeUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _financeUss is not assigned. " +
                            "FinanceScreen.uss will not be applied. " +
                            "Assign FinanceScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new FinanceView(root);
                    var controller = new FinanceController(view, iScreenRouterFin);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.competitors — Competitors ─────────────────────────────────────

            if (_competitorsUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _competitorsUxml is not assigned. " +
                    "screen.competitors will fall back to PlaceholderScreenView. " +
                    "Assign CompetitorsScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset competitorsUxml    = _competitorsUxml;
                StyleSheet      competitorsUss     = _competitorsUss;
                IScreenRouter   iScreenRouterComp  = ScreenRouter;

                screenRouter.RegisterScreen(ScreenIds.Competitors, () =>
                {
                    VisualElement root = competitorsUxml.Instantiate();

                    if (competitorsUss != null)
                    {
                        root.styleSheets.Add(competitorsUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _competitorsUss is not assigned. " +
                            "CompetitorsScreen.uss will not be applied. " +
                            "Assign CompetitorsScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new CompetitorsView(root);
                    var controller = new CompetitorsController(view, iScreenRouterComp);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.product_detail — Product Detail ───────────────────────────────

            if (_productDetailUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _productDetailUxml is not assigned. " +
                    "screen.product_detail will fall back to PlaceholderScreenView. " +
                    "Assign ProductDetailScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset productDetailUxml = _productDetailUxml;
                StyleSheet      productDetailUss  = _productDetailUss;
                IScreenRouter   iScreenRouterPd   = ScreenRouter;

                screenRouter.RegisterScreen(ScreenIds.ProductDetail, () =>
                {
                    VisualElement root = productDetailUxml.Instantiate();

                    if (productDetailUss != null)
                    {
                        root.styleSheets.Add(productDetailUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _productDetailUss is not assigned. " +
                            "ProductDetailScreen.uss will not be applied. " +
                            "Assign ProductDetailScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new ProductDetailView(root);
                    var controller = new ProductDetailController(view, iScreenRouterPd);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.market — Market Overview ──────────────────────────────────────

            if (_marketUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _marketUxml is not assigned. " +
                    "screen.market will fall back to PlaceholderScreenView. " +
                    "Assign MarketScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset marketUxml       = _marketUxml;
                StyleSheet      marketUss        = _marketUss;
                IScreenRouter   iScreenRouterMkt = ScreenRouter;

                screenRouter.RegisterScreen(ScreenIds.Market, () =>
                {
                    VisualElement root = marketUxml.Instantiate();

                    if (marketUss != null)
                    {
                        root.styleSheets.Add(marketUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _marketUss is not assigned. " +
                            "MarketScreen.uss will not be applied. " +
                            "Assign MarketScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new MarketView(root);
                    var controller = new MarketController(view, iScreenRouterMkt);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.calendar — Calendar ───────────────────────────────────────────

            if (_calendarUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _calendarUxml is not assigned. " +
                    "screen.calendar will fall back to PlaceholderScreenView. " +
                    "Assign CalendarScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset calendarUxml     = _calendarUxml;
                StyleSheet      calendarUss      = _calendarUss;
                IScreenRouter   iScreenRouterCal = ScreenRouter;
                IModalRouter    iModalRouterCal  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Calendar, () =>
                {
                    VisualElement root = calendarUxml.Instantiate();

                    if (calendarUss != null)
                    {
                        root.styleSheets.Add(calendarUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _calendarUss is not assigned. " +
                            "CalendarScreen.uss will not be applied. " +
                            "Assign CalendarScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new CalendarView(root);
                    var controller = new CalendarController(view, iScreenRouterCal, iModalRouterCal);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.research — Research ───────────────────────────────────────────

            if (_researchUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _researchUxml is not assigned. " +
                    "screen.research will fall back to PlaceholderScreenView. " +
                    "Assign ResearchScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset researchUxml     = _researchUxml;
                StyleSheet      researchUss      = _researchUss;
                IScreenRouter   iScreenRouterRes = ScreenRouter;
                IModalRouter    iModalRouterRes  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Research, () =>
                {
                    VisualElement root = researchUxml.Instantiate();

                    if (researchUss != null)
                    {
                        root.styleSheets.Add(researchUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _researchUss is not assigned. " +
                            "ResearchScreen.uss will not be applied. " +
                            "Assign ResearchScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new ResearchView(root);
                    var controller = new ResearchController(view, iScreenRouterRes, iModalRouterRes);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.products — Products Portfolio ─────────────────────────────────

            if (_productsUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _productsUxml is not assigned. " +
                    "screen.products will fall back to PlaceholderScreenView. " +
                    "Assign ProductsScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset productsUxml     = _productsUxml;
                StyleSheet      productsUss      = _productsUss;
                IScreenRouter   iScreenRouterPro = ScreenRouter;
                IModalRouter    iModalRouterPro  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Products, () =>
                {
                    VisualElement root = productsUxml.Instantiate();

                    if (productsUss != null)
                    {
                        root.styleSheets.Add(productsUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _productsUss is not assigned. " +
                            "ProductsScreen.uss will not be applied. " +
                            "Assign ProductsScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new ProductsView(root);
                    var controller = new ProductsController(view, iScreenRouterPro, iModalRouterPro);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.reports — Reports / Inbox ─────────────────────────────────────

            if (_reportsInboxUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _reportsInboxUxml is not assigned. " +
                    "screen.reports will fall back to PlaceholderScreenView. " +
                    "Assign ReportsInboxScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset reportsInboxUxml = _reportsInboxUxml;
                StyleSheet      reportsInboxUss  = _reportsInboxUss;
                IScreenRouter   iScreenRouterRep = ScreenRouter;

                screenRouter.RegisterScreen(ScreenIds.Reports, () =>
                {
                    VisualElement root = reportsInboxUxml.Instantiate();

                    if (reportsInboxUss != null)
                    {
                        root.styleSheets.Add(reportsInboxUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _reportsInboxUss is not assigned. " +
                            "ReportsInboxScreen.uss will not be applied. " +
                            "Assign ReportsInboxScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new ReportsInboxView(root);
                    var controller = new ReportsInboxController(view, iScreenRouterRep);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.company — Company Profile ─────────────────────────────────────

            if (_companyUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _companyUxml is not assigned. " +
                    "screen.company will fall back to PlaceholderScreenView. " +
                    "Assign CompanyScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset companyUxml     = _companyUxml;
                StyleSheet      companyUss      = _companyUss;
                IScreenRouter   iScreenRouterCo = ScreenRouter;
                IModalRouter    iModalRouterCo  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Company, () =>
                {
                    VisualElement root = companyUxml.Instantiate();

                    if (companyUss != null)
                    {
                        root.styleSheets.Add(companyUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _companyUss is not assigned. " +
                            "CompanyScreen.uss will not be applied. " +
                            "Assign CompanyScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new CompanyView(root);
                    var controller = new CompanyController(view, iScreenRouterCo, iModalRouterCo);
                    controller.Initialize();

                    return view.Root;
                });
            }

            // ── screen.settings — Settings ───────────────────────────────────────────

            if (_settingsUxml == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    "UIShellController: _settingsUxml is not assigned. " +
                    "screen.settings will fall back to PlaceholderScreenView. " +
                    "Assign SettingsScreen.uxml to UIShellController on the UIShell GameObject.", this);
            }
            else
            {
                // Capture references for the factory closure.
                VisualTreeAsset settingsUxml     = _settingsUxml;
                StyleSheet      settingsUss      = _settingsUss;
                IScreenRouter   iScreenRouterSet = ScreenRouter;
                IModalRouter    iModalRouterSet  = ModalRouter;

                screenRouter.RegisterScreen(ScreenIds.Settings, () =>
                {
                    VisualElement root = settingsUxml.Instantiate();

                    if (settingsUss != null)
                    {
                        root.styleSheets.Add(settingsUss);
                    }
                    else
                    {
                        DebugLogger.LogWarning(DebugCategory.Bootstrap,
                            "UIShellController: _settingsUss is not assigned. " +
                            "SettingsScreen.uss will not be applied. " +
                            "Assign SettingsScreen.uss to UIShellController on the UIShell GameObject.", this);
                    }

                    var view       = new SettingsView(root);
                    var controller = new SettingsController(view, iScreenRouterSet, iModalRouterSet);
                    controller.Initialize();

                    return view.Root;
                });
            }
        }

        /// <summary>
        /// Registers drawer factories for all known stable drawer IDs.
        /// Phase 4 registers a placeholder filter drawer.
        /// Phase 5+ will replace factories with fully wired implementations.
        /// </summary>
        private void RegisterDrawerFactories(RightDrawerRouter drawerRouter)
        {
            IRightDrawerRouter iRouter = drawerRouter;

            // ── drawer.filter ────────────────────────────────────────────────────────
            drawerRouter.RegisterDrawer(DrawerIds.Filter, context =>
            {
                var filterDrawer = new FilterDrawerFrameView();
                filterDrawer.InnerView.OnCloseRequested = () => iRouter.CloseDrawer();
                return filterDrawer.Root;
            });
        }

        /// <summary>
        /// Registers modal factories for all known stable modal IDs.
        /// Phase 4 registers placeholder content for most modals.
        /// Phase 5+ will replace factories with fully wired implementations.
        /// </summary>
        private void RegisterModalFactories(ModalRouter modalRouter)
        {
            // Capture IModalRouter reference for use in factory lambdas.
            IModalRouter iRouter = modalRouter;

            // ── modal.confirm ────────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Confirm, context =>
                ConfirmModalView.Create(
                    title:     "Confirm Action",
                    message:   "Are you sure you want to proceed? This action cannot be undone.",
                    onConfirm: () => iRouter.CloseModal(),
                    onCancel:  () => iRouter.CloseModal(),
                    variant:   "confirm"));

            // ── modal.info ───────────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Info, context =>
            {
                var frame = new ModalFrameView("Info", "modal-frame--standard");
                frame.OnCloseRequested = () => iRouter.CloseModal();
                var msg = new Label("[Placeholder] Info modal content.");
                msg.AddToClassList("confirm-modal__message");
                frame.Body.Add(msg);
                return frame.Root;
            });

            // ── modal.warning ────────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Warning, context =>
                ConfirmModalView.Create(
                    title:     "Warning",
                    message:   "[Placeholder] Warning modal content.",
                    onConfirm: () => iRouter.CloseModal(),
                    onCancel:  () => iRouter.CloseModal(),
                    variant:   "warning"));

            // ── modal.error ──────────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Error, context =>
                ConfirmModalView.Create(
                    title:     "Error",
                    message:   "[Placeholder] An error occurred.",
                    onConfirm: () => iRouter.CloseModal(),
                    onCancel:  () => iRouter.CloseModal(),
                    variant:   "error"));

            // ── modal.detail ─────────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Detail, context =>
            {
                var detail = new DetailModalFrameView(
                    name:      "[Placeholder] Entity Name",
                    subtitle:  "[Placeholder] Subtitle",
                    sizeClass: "modal-frame--large");

                detail.OnCloseRequested = () => iRouter.CloseModal();
                detail.SetAvatar("??");
                detail.SetStatus("[Placeholder] Status", "pill--neutral");

                var tabContent = new VisualElement();
                var tabMsg = new Label("[Placeholder] Tab content will be implemented in Phase 5.");
                tabMsg.AddToClassList("confirm-modal__message");
                tabContent.Add(tabMsg);
                detail.AddTab("tab.overview", "Overview", tabContent);

                return detail.Root;
            });

            // ── modal.save_game ──────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.SaveGame, context =>
                CreatePlaceholderSystemModal("Save Game", iRouter));

            // ── modal.load_game ──────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.LoadGame, context =>
                CreatePlaceholderSystemModal("Load Game", iRouter));

            // ── modal.new_game ───────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.NewGame, context =>
                CreatePlaceholderSystemModal("New Game", iRouter));

            // ── modal.settings ───────────────────────────────────────────────────────
            modalRouter.RegisterModal(ModalIds.Settings, context =>
                CreatePlaceholderSystemModal("Settings", iRouter));
        }

        /// <summary>Creates a generic placeholder modal for system modals not yet implemented.</summary>
        private static VisualElement CreatePlaceholderSystemModal(string title, IModalRouter modalRouter)
        {
            var frame = new ModalFrameView(title, "modal-frame--standard");
            frame.OnCloseRequested = () => modalRouter.CloseModal();

            var message = new Label("[Placeholder] This modal will be implemented later.");
            message.AddToClassList("confirm-modal__message");
            frame.Body.Add(message);

            var closeBtn = new UnityEngine.UIElements.Button(() => modalRouter.CloseModal());
            closeBtn.text = "Close";
            frame.Footer.Add(closeBtn);

            return frame.Root;
        }

        private void LogMissingHost(VisualElement element, string name)
        {
            if (element == null)
            {
                DebugLogger.LogWarning(DebugCategory.Bootstrap,
                    $"UIShellController: host element '{name}' not found in shell UXML.", this);
            }
        }

        /// <summary>
        /// [Placeholder] Called when the player confirms company creation.
        /// Logs confirmation and hides the wizard. Core simulation wiring is a later phase.
        /// </summary>
        private void OnCompanyCreationConfirmed()
        {
            DebugLogger.Log(DebugCategory.UI,
                "[Placeholder] Company created. Core simulation wiring will be implemented in a later phase.", this);
            HideWizard();
        }
    }
}
