using System.Collections.Generic;
using Project.Application;
using Project.Core.Debugging;
using Project.Presentation.UI.Components;
using Project.Presentation.UI.Controllers;
using Project.Presentation.UI.Modals;
using Project.Presentation.UI.Routing;
using Project.Presentation.UI.Screens.CompanyCreation;
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

        // ─── Private references ──────────────────────────────────────────────────────

        private SidebarView           _sidebarView;
        private TopBarView            _topBarView;
        private VisualElement         _shellRoot;
        private VisualElement         _wizardContainer;
        private RightDrawerRouter     _drawerRouterImpl;
        private CompanyCreationView   _companyCreationView;

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

        // ─── Private helpers ─────────────────────────────────────────────────────────

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
