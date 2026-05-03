using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Controllers
{
    /// <summary>
    /// Programmatic view controller for the full-screen wizard shell.
    /// Loads WizardShell.uxml, queries its named regions, and provides
    /// a typed API for populating header, stepper, body, and footer.
    ///
    /// Caller pattern:
    ///   var wizard = new WizardShellView(rootContainer);
    ///   wizard.SetHeader("NEW GAME", "Company Setup", 1, 3);
    ///   wizard.SetSteps(steps);
    ///   wizard.SetMainContent(myContent);
    ///   wizard.OnCancelRequested = () => _uiShell.HideWizard();
    ///   rootContainer.Add(wizard.Root);
    /// </summary>
    public sealed class WizardShellView
    {
        // ─── USS class constants ─────────────────────────────────────────────────────

        private const string StepChipClass      = "wizard-step-chip";
        private const string ChipCircleClass    = "wizard-step-chip__circle";
        private const string ChipNumberClass    = "wizard-step-chip__number";
        private const string ChipLabelClass     = "wizard-step-chip__label";
        private const string ConnectorClass     = "wizard-step-chip__connector";
        private const string IsDisabledClass    = "is-disabled";

        // ─── Queried UXML references ─────────────────────────────────────────────────

        private readonly Label         _contextLabel;
        private readonly Label         _titleLabel;
        private readonly Label         _stepCounter;
        private readonly VisualElement _stepper;
        private readonly VisualElement _mainContent;
        private readonly VisualElement _previewContent;
        private readonly VisualElement _footerLeft;
        private readonly VisualElement _footerCentre;
        private readonly VisualElement _footerRight;

        // ─── Built-in footer buttons ─────────────────────────────────────────────────

        private readonly Button _cancelButton;
        private readonly Button _continueButton;

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>The wizard root element. Add this to the UI tree to show the wizard.</summary>
        public VisualElement Root { get; }

        /// <summary>Callback invoked when the built-in Cancel button is clicked.</summary>
        public Action OnCancelRequested { get; set; }

        /// <summary>Callback invoked when the built-in Continue button is clicked.</summary>
        public Action OnContinueRequested { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Loads the WizardShell UXML, queries all named regions, and builds
        /// the default Cancel and Continue buttons in the footer.
        /// </summary>
        public WizardShellView()
        {
            // Build the root element and apply the structural class.
            Root = new VisualElement();
            Root.AddToClassList("wizard-shell");

            // ── HEADER ──────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList("wizard-header");

            var headerLeft = new VisualElement();
            headerLeft.AddToClassList("wizard-header__left");

            _contextLabel = new Label();
            _contextLabel.AddToClassList("wizard-header__context");
            _contextLabel.AddToClassList("text-tiny");
            headerLeft.Add(_contextLabel);

            _titleLabel = new Label();
            _titleLabel.AddToClassList("wizard-header__title");
            _titleLabel.AddToClassList("text-title");
            headerLeft.Add(_titleLabel);

            header.Add(headerLeft);

            _stepCounter = new Label();
            _stepCounter.AddToClassList("wizard-header__step-counter");
            _stepCounter.AddToClassList("text-small");
            header.Add(_stepCounter);

            Root.Add(header);

            // ── STEPPER ─────────────────────────────────────────────────────────────
            _stepper = new VisualElement();
            _stepper.AddToClassList("wizard-stepper");
            Root.Add(_stepper);

            // ── BODY ────────────────────────────────────────────────────────────────
            var body = new VisualElement();
            body.AddToClassList("wizard-body");

            var mainScroll = new ScrollView(ScrollViewMode.Vertical);
            mainScroll.AddToClassList("wizard-body__main");
            _mainContent = mainScroll.contentContainer;
            body.Add(mainScroll);

            var divider = new VisualElement();
            divider.AddToClassList("wizard-body__divider");
            body.Add(divider);

            var previewScroll = new ScrollView(ScrollViewMode.Vertical);
            previewScroll.AddToClassList("wizard-body__preview");
            _previewContent = previewScroll.contentContainer;
            body.Add(previewScroll);

            Root.Add(body);

            // ── FOOTER ──────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList("wizard-footer");

            _footerLeft = new VisualElement();
            _footerLeft.AddToClassList("wizard-footer__left");

            _cancelButton = new Button(() => OnCancelRequested?.Invoke());
            _cancelButton.text = "Cancel";
            _cancelButton.AddToClassList("base-button");
            _cancelButton.AddToClassList("base-button--ghost");
            _footerLeft.Add(_cancelButton);
            footer.Add(_footerLeft);

            _footerCentre = new VisualElement();
            _footerCentre.AddToClassList("wizard-footer__centre");
            footer.Add(_footerCentre);

            _footerRight = new VisualElement();
            _footerRight.AddToClassList("wizard-footer__right");

            _continueButton = new Button(() => OnContinueRequested?.Invoke());
            _continueButton.text = "Continue";
            _continueButton.AddToClassList("base-button");
            _continueButton.AddToClassList("base-button--primary");
            _footerRight.Add(_continueButton);
            footer.Add(_footerRight);

            Root.Add(footer);
        }

        // ─── Public methods ──────────────────────────────────────────────────────────

        /// <summary>
        /// Sets all three header text elements.
        /// </summary>
        /// <param name="contextLabel">Small uppercase label (e.g. "NEW GAME").</param>
        /// <param name="title">Large heading (e.g. "Company Setup").</param>
        /// <param name="currentStep">1-based index of the current step.</param>
        /// <param name="totalSteps">Total number of steps in the wizard.</param>
        public void SetHeader(string contextLabel, string title, int currentStep, int totalSteps)
        {
            _contextLabel.text = contextLabel ?? string.Empty;
            _titleLabel.text   = title        ?? string.Empty;
            _stepCounter.text  = $"Step {currentStep} of {totalSteps}";
        }

        /// <summary>
        /// Rebuilds the stepper row from the provided step data list.
        /// Connector lines are added between adjacent chips.
        /// </summary>
        public void SetSteps(List<WizardStepData> steps)
        {
            _stepper.Clear();

            if (steps == null || steps.Count == 0)
            {
                return;
            }

            for (int i = 0; i < steps.Count; i++)
            {
                WizardStepData stepData = steps[i];
                string stateClass = $"is-{stepData.State}";

                // Add connector before every chip except the first.
                if (i > 0)
                {
                    // Connector state depends on whether the previous step is complete.
                    bool previousComplete = steps[i - 1].State == "complete";
                    string connectorState = previousComplete ? "is-complete" : string.Empty;
                    _stepper.Add(BuildConnector(connectorState));
                }

                _stepper.Add(BuildStepChip(i + 1, stepData.Label, stateClass));
            }
        }

        /// <summary>
        /// Sets the active step, updating stepper chip visual states accordingly.
        /// Only reapplies states from the current steps data — call SetSteps first.
        /// This is a targeted visual update; prefer rebuilding via SetSteps when data changes.
        /// </summary>
        public void SetActiveStep(int stepIndex)
        {
            // Iterate through chip elements and update state classes.
            // Chips are interleaved with connectors in the stepper: chip, connector, chip, ...
            int chipPosition = 0;
            foreach (VisualElement child in _stepper.Children())
            {
                if (!child.ClassListContains(StepChipClass))
                {
                    continue;
                }

                RemoveAllStateClasses(child);

                if (chipPosition < stepIndex)
                {
                    child.AddToClassList("is-complete");
                }
                else if (chipPosition == stepIndex)
                {
                    child.AddToClassList("is-active");
                }
                else
                {
                    child.AddToClassList("is-available");
                }

                chipPosition++;
            }
        }

        /// <summary>Replaces the main content scroll area contents.</summary>
        public void SetMainContent(VisualElement content)
        {
            _mainContent.Clear();
            if (content != null)
            {
                _mainContent.Add(content);
            }
        }

        /// <summary>Replaces the preview panel contents.</summary>
        public void SetPreviewContent(VisualElement content)
        {
            _previewContent.Clear();
            if (content != null)
            {
                _previewContent.Add(content);
            }
        }

        /// <summary>
        /// Replaces the footer action slot contents.
        /// Passing null for a slot leaves it empty.
        /// </summary>
        public void SetFooterActions(VisualElement left, VisualElement centre, VisualElement right)
        {
            _footerLeft.Clear();
            if (left != null)   { _footerLeft.Add(left); }

            _footerCentre.Clear();
            if (centre != null) { _footerCentre.Add(centre); }

            _footerRight.Clear();
            if (right != null)  { _footerRight.Add(right); }
        }

        /// <summary>
        /// Enables or disables the built-in Continue button.
        /// Disabled state adds the is-disabled USS class and removes it when enabled.
        /// </summary>
        public void SetContinueEnabled(bool enabled)
        {
            _continueButton.SetEnabled(enabled);

            if (enabled)
            {
                _continueButton.RemoveFromClassList(IsDisabledClass);
            }
            else
            {
                _continueButton.AddToClassList(IsDisabledClass);
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        /// <summary>Creates a single step chip VisualElement with a number circle and label below.</summary>
        private VisualElement BuildStepChip(int number, string label, string stateClass)
        {
            var chip = new VisualElement();
            chip.AddToClassList(StepChipClass);

            if (!string.IsNullOrEmpty(stateClass))
            {
                chip.AddToClassList(stateClass);
            }

            var circle = new VisualElement();
            circle.AddToClassList(ChipCircleClass);

            var numberLabel = new Label(number.ToString());
            numberLabel.AddToClassList(ChipNumberClass);
            circle.Add(numberLabel);

            chip.Add(circle);

            var stepLabel = new Label(label);
            stepLabel.AddToClassList(ChipLabelClass);
            chip.Add(stepLabel);

            return chip;
        }

        /// <summary>Creates a connector line element between step chips.</summary>
        private VisualElement BuildConnector(string stateClass)
        {
            var connector = new VisualElement();
            connector.AddToClassList(ConnectorClass);

            if (!string.IsNullOrEmpty(stateClass))
            {
                connector.AddToClassList(stateClass);
            }

            return connector;
        }

        /// <summary>Removes all wizard step state classes from a chip element.</summary>
        private static void RemoveAllStateClasses(VisualElement chip)
        {
            chip.RemoveFromClassList("is-active");
            chip.RemoveFromClassList("is-complete");
            chip.RemoveFromClassList("is-available");
            chip.RemoveFromClassList("is-locked");
            chip.RemoveFromClassList("is-warning");
            chip.RemoveFromClassList("is-error");
        }
    }
}
