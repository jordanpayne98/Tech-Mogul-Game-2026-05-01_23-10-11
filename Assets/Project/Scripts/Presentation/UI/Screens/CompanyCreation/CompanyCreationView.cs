using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Presentation.UI.Screens.CompanyCreation.Steps;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Top-level view that orchestrates the shell, step content views, and summary panel.
    ///
    /// Responsibilities:
    ///   - Creates CompanyCreationShellView and all 7 step views (lazily).
    ///   - Creates CompanyCreationSummaryPanelView.
    ///   - Subscribes to controller.OnViewModelChanged and re-renders.
    ///   - Wires all shell callbacks to controller methods.
    ///   - Routes step field changes to the appropriate controller method.
    ///
    /// Pattern: MainMenuView — pure C# class, constructor wires everything,
    ///          Bind applies ViewModel data.
    /// </summary>
    public sealed class CompanyCreationView : IDisposable
    {
        // ─── Dependencies ────────────────────────────────────────────────────────────

        private readonly CompanyCreationController    _controller;
        private readonly CompanyCreationShellView     _shell;
        private readonly CompanyCreationSummaryPanelView _summaryPanel;

        // ─── Lazy step views ─────────────────────────────────────────────────────────

        private CompanyStepView         _companyStep;
        private BackgroundStepView      _backgroundStep;
        private FoundersStepView        _foundersStep;
        private FounderDetailsStepView  _founderDetailsStep;
        private TeamBudgetStepView      _teamBudgetStep;
        private SandboxSetupStepView    _sandboxStep;
        private ReviewStepView          _reviewStep;

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>The full-screen root element. Inject this into the UIDocument root.</summary>
        public VisualElement Root => _shell.Root;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the shell and wires all controller callbacks.
        /// Call controller.Initialize() after construction to fire the first ViewModel event.
        /// </summary>
        public CompanyCreationView(CompanyCreationController controller)
        {
            _controller   = controller   ?? throw new ArgumentNullException(nameof(controller));
            _shell        = new CompanyCreationShellView();
            _summaryPanel = new CompanyCreationSummaryPanelView();

            // Wire shell callbacks once — these do not change after construction.
            _shell.SetHeader("Company Creation", "Set up your company before starting the simulation.");
            _shell.SetCloseCallback(() => _controller.RequestClose());
            _shell.SetStepClickCallback(stepId => _controller.GoToStep(stepId));

            // Subscribe to ViewModel changes.
            _controller.OnViewModelChanged += Bind;
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        /// <summary>Updates all visual state from the ViewModel. Called on every controller change.</summary>
        public void Bind(CompanyCreationViewModel vm)
        {
            if (vm == null)
            {
                DebugLogger.LogWarning(DebugCategory.UI,
                    "CompanyCreationView.Bind: received null ViewModel.");
                return;
            }

            // Stepper
            _shell.SetStepsWithCallback(vm.Steps, vm.CurrentStep);

            // Active step content (lazy create + swap)
            VisualElement stepContent = GetOrCreateStepView(vm.CurrentStep);
            BindStepView(vm.CurrentStep, vm);
            _shell.SetMainContent(stepContent);

            // Summary panel
            _summaryPanel.Bind(vm.Summary);
            _shell.SetSummaryContent(_summaryPanel.Root);

            // Footer
            bool isOnReview = vm.CurrentStep == CompanyCreationStepId.Review;

            _shell.SetBackButton("Back", vm.CanGoBack, () => _controller.GoBack());

            if (isOnReview)
            {
                _shell.SetNextButton(vm.NextButtonText, vm.CanConfirm, () => _controller.RequestConfirm());
            }
            else
            {
                _shell.SetNextButton(vm.NextButtonText, vm.CanGoNext, () => _controller.GoNext());
            }

            // Footer text: show validation message if present, otherwise helper text.
            if (!string.IsNullOrEmpty(vm.FooterValidationMessage))
            {
                _shell.SetFooterValidationMessage(vm.FooterValidationMessage);
            }
            else
            {
                _shell.SetFooterHelperText(vm.FooterHelperText);
            }
        }

        // ─── IDisposable ─────────────────────────────────────────────────────────────

        public void Dispose()
        {
            _controller.OnViewModelChanged -= Bind;
        }

        // ─── Private — lazy step view creation ──────────────────────────────────────

        private VisualElement GetOrCreateStepView(CompanyCreationStepId stepId)
        {
            switch (stepId)
            {
                case CompanyCreationStepId.Company:
                    if (_companyStep == null)
                    {
                        _companyStep = new CompanyStepView();
                        _companyStep.OnFieldChanged = (field, value) => _controller.UpdateField(field, value);
                    }
                    return _companyStep.Root;

                case CompanyCreationStepId.Background:
                    if (_backgroundStep == null)
                    {
                        _backgroundStep = new BackgroundStepView();
                        _backgroundStep.OnFieldChanged = (field, value) => _controller.UpdateField(field, value);
                    }
                    return _backgroundStep.Root;

                case CompanyCreationStepId.Founders:
                    if (_foundersStep == null)
                    {
                        _foundersStep = new FoundersStepView();
                        _foundersStep.OnFieldChanged = (field, value) => _controller.UpdateField(field, value);
                    }
                    return _foundersStep.Root;

                case CompanyCreationStepId.FounderDetails:
                    if (_founderDetailsStep == null)
                    {
                        _founderDetailsStep = new FounderDetailsStepView();
                        _founderDetailsStep.OnFounderFieldChanged = (index, field, value) =>
                            _controller.UpdateFounderField(index, field, value);
                    }
                    return _founderDetailsStep.Root;

                case CompanyCreationStepId.TeamBudget:
                    if (_teamBudgetStep == null)
                    {
                        _teamBudgetStep = new TeamBudgetStepView();
                        _teamBudgetStep.OnFieldChanged = (field, value) => _controller.UpdateField(field, value);
                    }
                    return _teamBudgetStep.Root;

                case CompanyCreationStepId.SandboxSetup:
                    if (_sandboxStep == null)
                    {
                        _sandboxStep = new SandboxSetupStepView();
                        _sandboxStep.OnFieldChanged = (field, value) => _controller.UpdateField(field, value);
                    }
                    return _sandboxStep.Root;

                case CompanyCreationStepId.Review:
                    if (_reviewStep == null)
                    {
                        _reviewStep = new ReviewStepView();
                        _reviewStep.OnEditStepRequested = stepId2 => _controller.GoToStep(stepId2);
                    }
                    return _reviewStep.Root;

                default:
                    DebugLogger.LogError(DebugCategory.UI,
                        $"CompanyCreationView: unknown step id '{stepId}'.");
                    return new VisualElement();
            }
        }

        private void BindStepView(CompanyCreationStepId stepId, CompanyCreationViewModel vm)
        {
            switch (stepId)
            {
                case CompanyCreationStepId.Company:
                    _companyStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.Background:
                    _backgroundStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.Founders:
                    _foundersStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.FounderDetails:
                    _founderDetailsStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.TeamBudget:
                    _teamBudgetStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.SandboxSetup:
                    _sandboxStep?.Bind(vm);
                    break;
                case CompanyCreationStepId.Review:
                    _reviewStep?.Bind(vm);
                    break;
            }
        }
    }
}
