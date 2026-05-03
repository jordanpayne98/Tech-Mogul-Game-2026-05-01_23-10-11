using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Dedicated full-screen shell for the Company Creation wizard.
    ///
    /// Layout:
    ///   company-creation-root (flex-column, fills screen)
    ///   ├── company-creation__background   (absolute-fill background layer)
    ///   ├── company-creation__header       (title-block + close button)
    ///   │   ├── company-creation__title-block
    ///   │   │   ├── company-creation__title
    ///   │   │   └── company-creation__subtitle
    ///   │   └── company-creation__close-button
    ///   ├── company-creation__content      (flex-row body)
    ///   │   ├── company-creation__stepper-panel  (vertical stepper, 280px)
    ///   │   ├── company-creation__main-panel     (ScrollView, flex-grow)
    ///   │   └── company-creation__summary-panel  (ScrollView, 500px)
    ///   └── company-creation__footer       (back | helper/validation | next)
    ///
    /// All UI built programmatically following WizardShellView pattern.
    /// </summary>
    public sealed class CompanyCreationShellView
    {
        // ─── USS class constants ─────────────────────────────────────────────────────

        private const string RootClass           = "company-creation-root";
        private const string BackgroundClass     = "company-creation__background";
        private const string HeaderClass         = "company-creation__header";
        private const string TitleBlockClass     = "company-creation__title-block";
        private const string TitleClass          = "company-creation__title";
        private const string SubtitleClass       = "company-creation__subtitle";
        private const string CloseButtonClass    = "company-creation__close-button";
        private const string ContentClass        = "company-creation__content";
        private const string StepperPanelClass   = "company-creation__stepper-panel";
        private const string StepperListClass    = "company-creation__stepper-list";
        private const string StepperItemClass    = "company-creation__stepper-item";
        private const string StepperNumberClass  = "company-creation__stepper-number";
        private const string StepperLabelClass   = "company-creation__stepper-label";
        private const string StepperConnClass    = "company-creation__stepper-connector";
        private const string MainPanelClass      = "company-creation__main-panel";
        private const string SummaryPanelClass   = "company-creation__summary-panel";
        private const string FooterClass         = "company-creation__footer";
        private const string FooterLeftClass     = "company-creation__footer-left";
        private const string FooterCentreClass   = "company-creation__footer-centre";
        private const string FooterRightClass    = "company-creation__footer-right";

        // ─── Queried element references ──────────────────────────────────────────────

        private readonly Label         _titleLabel;
        private readonly Label         _subtitleLabel;
        private readonly Button        _closeButton;
        private readonly VisualElement _stepperList;
        private readonly VisualElement _mainContent;
        private readonly VisualElement _summaryContent;
        private readonly VisualElement _footerLeft;
        private readonly Label         _footerCentreLabel;
        private readonly VisualElement _footerRight;

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>The full-screen root element. Add this to the UI tree to display the wizard.</summary>
        public VisualElement Root { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public CompanyCreationShellView()
        {
            Root = new VisualElement();
            Root.AddToClassList(RootClass);

            // ── Background ──────────────────────────────────────────────────────────
            var background = new VisualElement();
            background.AddToClassList(BackgroundClass);
            Root.Add(background);

            // ── Header ──────────────────────────────────────────────────────────────
            var header = new VisualElement();
            header.AddToClassList(HeaderClass);

            var titleBlock = new VisualElement();
            titleBlock.AddToClassList(TitleBlockClass);

            _titleLabel = new Label("Company Creation");
            _titleLabel.AddToClassList(TitleClass);
            titleBlock.Add(_titleLabel);

            _subtitleLabel = new Label(string.Empty);
            _subtitleLabel.AddToClassList(SubtitleClass);
            titleBlock.Add(_subtitleLabel);

            header.Add(titleBlock);

            _closeButton = new Button();
            _closeButton.text = "✕";
            _closeButton.AddToClassList(CloseButtonClass);
            _closeButton.AddToClassList("base-button");
            _closeButton.AddToClassList("base-button--secondary");
            header.Add(_closeButton);

            Root.Add(header);

            // ── Content (3 columns) ─────────────────────────────────────────────────
            var content = new VisualElement();
            content.AddToClassList(ContentClass);

            // Left: Stepper panel
            var stepperPanel = new VisualElement();
            stepperPanel.AddToClassList(StepperPanelClass);

            _stepperList = new VisualElement();
            _stepperList.AddToClassList(StepperListClass);
            stepperPanel.Add(_stepperList);

            content.Add(stepperPanel);

            // Centre: Main content ScrollView
            var mainScroll = new ScrollView(ScrollViewMode.Vertical);
            mainScroll.AddToClassList(MainPanelClass);
            _mainContent = mainScroll.contentContainer;
            content.Add(mainScroll);

            // Right: Summary panel ScrollView
            var summaryScroll = new ScrollView(ScrollViewMode.Vertical);
            summaryScroll.AddToClassList(SummaryPanelClass);
            _summaryContent = summaryScroll.contentContainer;
            content.Add(summaryScroll);

            Root.Add(content);

            // ── Footer ──────────────────────────────────────────────────────────────
            var footer = new VisualElement();
            footer.AddToClassList(FooterClass);

            _footerLeft = new VisualElement();
            _footerLeft.AddToClassList(FooterLeftClass);
            footer.Add(_footerLeft);

            var footerCentre = new VisualElement();
            footerCentre.AddToClassList(FooterCentreClass);

            _footerCentreLabel = new Label(string.Empty);
            _footerCentreLabel.AddToClassList("text-small");
            footerCentre.Add(_footerCentreLabel);
            footer.Add(footerCentre);

            _footerRight = new VisualElement();
            _footerRight.AddToClassList(FooterRightClass);
            footer.Add(_footerRight);

            Root.Add(footer);
        }

        // ─── Public API methods ──────────────────────────────────────────────────────

        /// <summary>Sets the header title and subtitle labels.</summary>
        public void SetHeader(string title, string subtitle)
        {
            _titleLabel.text    = title    ?? string.Empty;
            _subtitleLabel.text = subtitle ?? string.Empty;
        }

        /// <summary>Wires the close button click callback.</summary>
        public void SetCloseCallback(Action callback)
        {
            _closeButton.clicked -= callback;
            _closeButton.clicked += callback;
        }

        /// <summary>Rebuilds the vertical stepper from ViewModel data.</summary>
        public void SetSteps(List<CompanyCreationStepViewModel> steps, CompanyCreationStepId activeStep)
        {
            _stepperList.Clear();

            if (steps == null || steps.Count == 0)
            {
                return;
            }

            for (int i = 0; i < steps.Count; i++)
            {
                CompanyCreationStepViewModel step = steps[i];

                var item = new VisualElement();
                item.AddToClassList(StepperItemClass);
                ApplyStepStateClass(item, step.State);

                // Number circle
                var numberEl = new Label(step.StepNumber.ToString());
                numberEl.AddToClassList(StepperNumberClass);
                item.Add(numberEl);

                // Label
                var labelEl = new Label(step.DisplayName);
                labelEl.AddToClassList(StepperLabelClass);
                item.Add(labelEl);

                _stepperList.Add(item);

                // Connector line between items (not after the last)
                if (i < steps.Count - 1)
                {
                    var connector = new VisualElement();
                    connector.AddToClassList(StepperConnClass);

                    if (step.State == CompanyCreationStepState.Complete)
                    {
                        connector.AddToClassList("is-complete");
                    }

                    _stepperList.Add(connector);
                }
            }
        }

        /// <summary>Wires a callback invoked when the player clicks a stepper item.</summary>
        public void SetStepClickCallback(Action<CompanyCreationStepId> callback)
        {
            // Re-register on each SetSteps call is handled through RegisterCallback with the item's userData.
            // This method stores the delegate so item construction can reference it.
            // For simplicity, register callbacks directly on items after SetSteps.
            // Call this before SetSteps to provide the delegate; items capture it at build time.
            // Since SetSteps rebuilds items, we need to re-register; callers should call this first.
            // Implementation: store delegate and wire in SetSteps.
            _stepClickCallback = callback;
        }

        // Internal storage for step click callback used during SetSteps rebuild.
        private Action<CompanyCreationStepId> _stepClickCallback;

        /// <summary>
        /// Rebuilds the vertical stepper with click callbacks wired.
        /// SetStepClickCallback must be called before this if click navigation is needed.
        /// </summary>
        public void SetStepsWithCallback(List<CompanyCreationStepViewModel> steps, CompanyCreationStepId activeStep)
        {
            _stepperList.Clear();

            if (steps == null || steps.Count == 0)
            {
                return;
            }

            for (int i = 0; i < steps.Count; i++)
            {
                CompanyCreationStepViewModel step = steps[i];

                var item = new VisualElement();
                item.AddToClassList(StepperItemClass);
                ApplyStepStateClass(item, step.State);

                // Register click only if the step is clickable.
                if (step.IsClickable && _stepClickCallback != null)
                {
                    CompanyCreationStepId capturedId = step.StepId;
                    item.RegisterCallback<ClickEvent>(_ => _stepClickCallback?.Invoke(capturedId));
                }

                // Number circle
                var numberEl = new Label(step.StepNumber.ToString());
                numberEl.AddToClassList(StepperNumberClass);
                item.Add(numberEl);

                // Label
                var labelEl = new Label(step.DisplayName);
                labelEl.AddToClassList(StepperLabelClass);
                item.Add(labelEl);

                _stepperList.Add(item);

                // Connector between items (not after last)
                if (i < steps.Count - 1)
                {
                    var connector = new VisualElement();
                    connector.AddToClassList(StepperConnClass);

                    if (step.State == CompanyCreationStepState.Complete)
                    {
                        connector.AddToClassList("is-complete");
                    }

                    _stepperList.Add(connector);
                }
            }
        }

        /// <summary>Replaces the centre main-panel content.</summary>
        public void SetMainContent(VisualElement content)
        {
            _mainContent.Clear();
            if (content != null)
            {
                _mainContent.Add(content);
            }
        }

        /// <summary>Replaces the right summary-panel content.</summary>
        public void SetSummaryContent(VisualElement content)
        {
            _summaryContent.Clear();
            if (content != null)
            {
                _summaryContent.Add(content);
            }
        }

        /// <summary>Configures the footer back button.</summary>
        public void SetBackButton(string text, bool enabled, Action callback)
        {
            _footerLeft.Clear();

            var btn = new Button();
            btn.text = text ?? "Back";
            btn.AddToClassList("base-button");
            btn.AddToClassList("base-button--secondary");

            if (!enabled)
            {
                btn.SetEnabled(false);
                btn.AddToClassList("is-disabled");
            }
            else if (callback != null)
            {
                btn.clicked += callback;
            }

            _footerLeft.Add(btn);
        }

        /// <summary>Configures the footer next / confirm button.</summary>
        public void SetNextButton(string text, bool enabled, Action callback)
        {
            _footerRight.Clear();

            var btn = new Button();
            btn.text = text ?? "Next";
            btn.AddToClassList("base-button");
            btn.AddToClassList("base-button--primary");

            if (!enabled)
            {
                btn.SetEnabled(false);
                btn.AddToClassList("is-disabled");
            }
            else if (callback != null)
            {
                btn.clicked += callback;
            }

            _footerRight.Add(btn);
        }

        /// <summary>Sets the footer centre helper text. Cleared by SetFooterValidationMessage.</summary>
        public void SetFooterHelperText(string text)
        {
            _footerCentreLabel.text = text ?? string.Empty;
            _footerCentreLabel.RemoveFromClassList("wizard-validation--error");
            _footerCentreLabel.RemoveFromClassList("wizard-validation--warning");
        }

        /// <summary>Sets footer validation message (overrides helper text). Shown in error style.</summary>
        public void SetFooterValidationMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            _footerCentreLabel.text = message;
            _footerCentreLabel.RemoveFromClassList("wizard-validation--warning");
            _footerCentreLabel.AddToClassList("wizard-validation--error");
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private static void ApplyStepStateClass(VisualElement element, CompanyCreationStepState state)
        {
            element.RemoveFromClassList("is-active");
            element.RemoveFromClassList("is-complete");
            element.RemoveFromClassList("is-available");
            element.RemoveFromClassList("is-future");
            element.RemoveFromClassList("is-invalid");
            element.RemoveFromClassList("is-disabled");

            string cls = state switch
            {
                CompanyCreationStepState.Active    => "is-active",
                CompanyCreationStepState.Complete  => "is-complete",
                CompanyCreationStepState.Available => "is-available",
                CompanyCreationStepState.Future    => "is-future",
                CompanyCreationStepState.Invalid   => "is-invalid",
                CompanyCreationStepState.Disabled  => "is-disabled",
                _                                  => string.Empty
            };

            if (!string.IsNullOrEmpty(cls))
            {
                element.AddToClassList(cls);
            }
        }
    }
}
