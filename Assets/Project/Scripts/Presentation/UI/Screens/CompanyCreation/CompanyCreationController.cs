using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using UnityEngine;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Orchestrates the Company Creation wizard: step navigation, per-step validation,
    /// ViewModel building, summary panel data, and footer state.
    ///
    /// Pure C# class — no MonoBehaviour dependency.
    /// The view subscribes to OnViewModelChanged and re-renders on every change.
    /// </summary>
    public sealed class CompanyCreationController
    {
        // ─── Step definitions ────────────────────────────────────────────────────────

        private static readonly (CompanyCreationStepId Id, int Number, string Name)[] StepDefinitions =
        {
            (CompanyCreationStepId.Company,       1, "Company Identity"),
            (CompanyCreationStepId.Background,    2, "Company Background"),
            (CompanyCreationStepId.Founders,      3, "Founder Setup"),
            (CompanyCreationStepId.FounderDetails,4, "Founder Details"),
            (CompanyCreationStepId.TeamBudget,    5, "Team & Budget"),
            (CompanyCreationStepId.SandboxSetup,  6, "Sandbox Setup"),
            (CompanyCreationStepId.Review,        7, "Review"),
        };

        // ─── Private state ───────────────────────────────────────────────────────────

        private readonly Action                _onCloseRequested;
        private readonly Action                _onConfirmRequested;
        private readonly CompanyCreationFormState _formState;

        private CompanyCreationStepId _currentStep;

        // Tracks which steps have been completed (passed validation and advanced past).
        private readonly HashSet<CompanyCreationStepId> _completedSteps  = new HashSet<CompanyCreationStepId>();
        private readonly HashSet<CompanyCreationStepId> _visitedSteps    = new HashSet<CompanyCreationStepId>();

        // Footer validation message shown after a failed GoNext/RequestConfirm.
        private string _footerValidationMessage = string.Empty;

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>Fired whenever the ViewModel changes. The view must re-render on every call.</summary>
        public event Action<CompanyCreationViewModel> OnViewModelChanged;

        /// <summary>The most recently built ViewModel.</summary>
        public CompanyCreationViewModel CurrentViewModel { get; private set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        /// <param name="onCloseRequested">Invoked when the player requests to close the wizard.</param>
        /// <param name="onConfirmRequested">Invoked when the player confirms company creation.</param>
        public CompanyCreationController(Action onCloseRequested, Action onConfirmRequested)
        {
            _onCloseRequested   = onCloseRequested;
            _onConfirmRequested = onConfirmRequested;
            _formState          = new CompanyCreationFormState();
        }

        // ─── Lifecycle ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Builds the initial ViewModel and fires OnViewModelChanged.
        /// Must be called once after the view has subscribed to the event.
        /// </summary>
        public void Initialize()
        {
            _formState.Reset();
            _currentStep = CompanyCreationStepId.Company;
            _completedSteps.Clear();
            _visitedSteps.Clear();
            _visitedSteps.Add(_currentStep);
            _footerValidationMessage = string.Empty;

            FireViewModelChanged();

            DebugLogger.Log(DebugCategory.UI,
                "CompanyCreationController: initialized at step Company.");
        }

        // ─── Navigation ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Advances to the next step if the current step passes validation.
        /// On failure, sets the footer validation message without advancing.
        /// </summary>
        public void GoNext()
        {
            if (!ValidateStep(_currentStep))
            {
                _footerValidationMessage = GetFirstValidationError(_currentStep);
                FireViewModelChanged();
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"CompanyCreationController.GoNext: step {_currentStep} failed validation. " +
                    $"Message: {_footerValidationMessage}");
                return;
            }

            _completedSteps.Add(_currentStep);
            _footerValidationMessage = string.Empty;

            CompanyCreationStepId next = GetNextStep(_currentStep);
            _currentStep = next;
            _visitedSteps.Add(_currentStep);

            FireViewModelChanged();
            DebugLogger.Log(DebugCategory.Navigation,
                $"CompanyCreationController: advanced to step {_currentStep}.");
        }

        /// <summary>
        /// Returns to the previous step. No-op on Step 1.
        /// </summary>
        public void GoBack()
        {
            CompanyCreationStepId? previous = GetPreviousStep(_currentStep);
            if (previous == null)
            {
                return;
            }

            _footerValidationMessage = string.Empty;
            _currentStep = previous.Value;
            FireViewModelChanged();
            DebugLogger.Log(DebugCategory.Navigation,
                $"CompanyCreationController: went back to step {_currentStep}.");
        }

        /// <summary>
        /// Navigates to the given step if it is clickable.
        /// Ignored (with a log) if the step is Future or Disabled.
        /// </summary>
        public void GoToStep(CompanyCreationStepId stepId)
        {
            if (!IsStepClickable(stepId))
            {
                _footerValidationMessage = "Complete the current step before jumping ahead.";
                FireViewModelChanged();
                DebugLogger.LogWarning(DebugCategory.Navigation,
                    $"CompanyCreationController.GoToStep: step {stepId} is not clickable.");
                return;
            }

            _footerValidationMessage = string.Empty;
            _currentStep = stepId;
            _visitedSteps.Add(_currentStep);
            FireViewModelChanged();
            DebugLogger.Log(DebugCategory.Navigation,
                $"CompanyCreationController: jumped to step {_currentStep}.");
        }

        // ─── Field updates ───────────────────────────────────────────────────────────

        /// <summary>
        /// Updates a named form field and rebuilds the ViewModel.
        /// Field names match the property names in CompanyCreationFormState.
        /// </summary>
        public void UpdateField(string fieldName, string value)
        {
            ApplyField(fieldName, value);
            _footerValidationMessage = string.Empty;
            FireViewModelChanged();
        }

        /// <summary>
        /// Updates a field on a specific founder profile (index 0 or 1) and rebuilds.
        /// </summary>
        public void UpdateFounderField(int founderIndex, string fieldName, string value)
        {
            string prefixedField = $"Founder{founderIndex}{fieldName}";
            ApplyField(prefixedField, value);
            _footerValidationMessage = string.Empty;
            FireViewModelChanged();
        }

        // ─── Close / confirm ─────────────────────────────────────────────────────────

        /// <summary>Invokes the close callback.</summary>
        public void RequestClose()
        {
            DebugLogger.Log(DebugCategory.UI, "CompanyCreationController: close requested.");
            _onCloseRequested?.Invoke();
        }

        /// <summary>
        /// Invokes the confirm callback if all steps are valid.
        /// Sets a footer validation message and does not fire if any step is invalid.
        /// </summary>
        public void RequestConfirm()
        {
            foreach (var def in StepDefinitions)
            {
                if (def.Id == CompanyCreationStepId.Review)
                {
                    continue;
                }

                if (!ValidateStep(def.Id))
                {
                    _footerValidationMessage = $"Please complete step \"{def.Name}\" before creating your company.";
                    FireViewModelChanged();
                    DebugLogger.LogWarning(DebugCategory.Validation,
                        $"CompanyCreationController.RequestConfirm: step {def.Id} is incomplete.");
                    return;
                }
            }

            DebugLogger.Log(DebugCategory.UI, "CompanyCreationController: company confirmed.");
            _onConfirmRequested?.Invoke();
        }

        // ─── ViewModel building ──────────────────────────────────────────────────────

        private void FireViewModelChanged()
        {
            CurrentViewModel = BuildViewModel();
            OnViewModelChanged?.Invoke(CurrentViewModel);
        }

        private CompanyCreationViewModel BuildViewModel()
        {
            bool isOnReview = _currentStep == CompanyCreationStepId.Review;
            bool canConfirm = isOnReview && AllRequiredStepsValid();
            bool canGoNext  = !isOnReview;
            bool canGoBack  = _currentStep != CompanyCreationStepId.Company;

            string nextButtonText = isOnReview ? "Confirm Create Company" : "Next";

            return new CompanyCreationViewModel(
                currentStep:            _currentStep,
                steps:                  BuildStepViewModels(),
                canGoBack:              canGoBack,
                canGoNext:              canGoNext,
                canConfirm:             canConfirm,
                footerHelperText:       BuildFooterText(),
                footerValidationMessage: _footerValidationMessage,
                nextButtonText:         nextButtonText,
                companyName:            _formState.CompanyName,
                industryFocus:          _formState.IndustryFocus   ?? string.Empty,
                headquarters:           _formState.Headquarters     ?? string.Empty,
                companyColour:          _formState.CompanyColour,
                logoId:                 _formState.LogoId,
                backgroundId:           _formState.BackgroundId     ?? string.Empty,
                founderSetupType:       _formState.FounderSetupType ?? string.Empty,
                founderProfiles:        BuildFounderProfiles(),
                startingCashPreset:     _formState.StartingCashPreset,
                startingCashAmount:     GetCashDisplayAmount(_formState.StartingCashPreset),
                startingTeamChoice:     _formState.StartingTeamChoice ?? string.Empty,
                sandboxSettings:        BuildSandboxSettings(),
                summary:                BuildSummary()
            );
        }

        private List<CompanyCreationStepViewModel> BuildStepViewModels()
        {
            var result = new List<CompanyCreationStepViewModel>(StepDefinitions.Length);

            foreach (var def in StepDefinitions)
            {
                CompanyCreationStepState state = GetStepState(def.Id);
                bool isClickable = IsStepClickable(def.Id);
                string validationMsg = (!ValidateStep(def.Id) && _visitedSteps.Contains(def.Id))
                    ? GetFirstValidationError(def.Id)
                    : string.Empty;

                result.Add(new CompanyCreationStepViewModel(
                    stepId:            def.Id,
                    stepNumber:        def.Number,
                    displayName:       def.Name,
                    state:             state,
                    isClickable:       isClickable,
                    validationMessage: validationMsg));
            }

            return result;
        }

        private CompanyCreationStepState GetStepState(CompanyCreationStepId stepId)
        {
            if (stepId == _currentStep)
            {
                return CompanyCreationStepState.Active;
            }

            if (_completedSteps.Contains(stepId))
            {
                // Check if it became invalid since completion.
                return ValidateStep(stepId)
                    ? CompanyCreationStepState.Complete
                    : CompanyCreationStepState.Invalid;
            }

            if (_visitedSteps.Contains(stepId))
            {
                return ValidateStep(stepId)
                    ? CompanyCreationStepState.Complete
                    : CompanyCreationStepState.Invalid;
            }

            // A step is available if all previous steps have been visited.
            if (IsReachable(stepId))
            {
                return CompanyCreationStepState.Available;
            }

            return CompanyCreationStepState.Future;
        }

        private bool IsReachable(CompanyCreationStepId stepId)
        {
            // A step is reachable if all preceding steps have been visited.
            foreach (var def in StepDefinitions)
            {
                if (def.Id == stepId)
                {
                    return true;
                }

                if (!_visitedSteps.Contains(def.Id))
                {
                    return false;
                }
            }

            return false;
        }

        private bool IsStepClickable(CompanyCreationStepId stepId)
        {
            CompanyCreationStepState state = GetStepState(stepId);
            return state == CompanyCreationStepState.Active
                || state == CompanyCreationStepState.Complete
                || state == CompanyCreationStepState.Available
                || state == CompanyCreationStepState.Invalid;
        }

        private CompanyCreationSummaryViewModel BuildSummary()
        {
            bool onFounderSteps = _currentStep == CompanyCreationStepId.Founders
                               || _currentStep == CompanyCreationStepId.FounderDetails;

            string title = onFounderSteps ? "Founder Summary" : "Company Summary";

            var rows = new List<CompanyCreationSummaryRowViewModel>();

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Company Name",
                string.IsNullOrWhiteSpace(_formState.CompanyName) ? "Not set" : _formState.CompanyName,
                !string.IsNullOrWhiteSpace(_formState.CompanyName)));

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Industry",
                _formState.IndustryFocus ?? "Not set",
                _formState.IndustryFocus != null));

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Headquarters",
                _formState.Headquarters ?? "Not set",
                _formState.Headquarters != null));

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Background",
                _formState.BackgroundId ?? "Not set",
                _formState.BackgroundId != null));

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Founder Setup",
                _formState.FounderSetupType ?? "Not set",
                _formState.FounderSetupType != null));

            rows.Add(new CompanyCreationSummaryRowViewModel(
                "Starting Cash",
                GetCashDisplayAmount(_formState.StartingCashPreset),
                !string.IsNullOrWhiteSpace(_formState.StartingCashPreset)));

            bool hasSelections = !string.IsNullOrWhiteSpace(_formState.CompanyName)
                              || _formState.IndustryFocus != null
                              || _formState.Headquarters != null;

            return new CompanyCreationSummaryViewModel(
                title:         title,
                rows:          rows,
                warnings:      BuildWarnings(),
                hasSelections: hasSelections);
        }

        private List<CompanyCreationWarningViewModel> BuildWarnings()
        {
            var warnings = new List<CompanyCreationWarningViewModel>();

            if (_formState.StartingCashPreset == "lean")
            {
                warnings.Add(new CompanyCreationWarningViewModel(
                    "Lean Start gives you less runway. Consider hiring carefully.",
                    "warning"));
            }

            if (_formState.FailureMode == "Hardcore")
            {
                warnings.Add(new CompanyCreationWarningViewModel(
                    "Hardcore mode means company closure on bankruptcy.",
                    "danger"));
            }

            return warnings;
        }

        private string BuildFooterText()
        {
            return _currentStep switch
            {
                CompanyCreationStepId.Company       => "Enter your company name, industry and location.",
                CompanyCreationStepId.Background    => "Choose your company's founding background.",
                CompanyCreationStepId.Founders      => "Choose your founding team structure.",
                CompanyCreationStepId.FounderDetails => "Fill in your founder profile details.",
                CompanyCreationStepId.TeamBudget    => "Set your starting budget and team.",
                CompanyCreationStepId.SandboxSetup  => "Configure the simulation sandbox settings.",
                CompanyCreationStepId.Review        => "Review your setup and confirm to start.",
                _                                   => string.Empty
            };
        }

        // ─── Validation ──────────────────────────────────────────────────────────────

        private bool ValidateStep(CompanyCreationStepId step)
        {
            return step switch
            {
                CompanyCreationStepId.Company =>
                    !string.IsNullOrWhiteSpace(_formState.CompanyName)
                    && _formState.IndustryFocus != null
                    && _formState.Headquarters  != null,

                CompanyCreationStepId.Background =>
                    _formState.BackgroundId != null,

                CompanyCreationStepId.Founders =>
                    _formState.FounderSetupType != null,

                CompanyCreationStepId.FounderDetails =>
                    IsFounderProfileComplete(0),

                CompanyCreationStepId.TeamBudget =>
                    !string.IsNullOrWhiteSpace(_formState.StartingCashPreset),

                CompanyCreationStepId.SandboxSetup =>
                    true,   // Always valid — all fields have defaults.

                CompanyCreationStepId.Review =>
                    AllRequiredStepsValid(),

                _ => false
            };
        }

        private bool IsFounderProfileComplete(int index)
        {
            if (index == 0)
            {
                return !string.IsNullOrWhiteSpace(_formState.Founder0FirstName)
                    && !string.IsNullOrWhiteSpace(_formState.Founder0LastName)
                    && _formState.Founder0Age > 0
                    && _formState.Founder0Nationality  != null
                    && _formState.Founder0Location     != null
                    && _formState.Founder0Background   != null
                    && _formState.Founder0SkillProfile != null;
            }

            return !string.IsNullOrWhiteSpace(_formState.Founder1FirstName)
                && !string.IsNullOrWhiteSpace(_formState.Founder1LastName)
                && _formState.Founder1Age > 0
                && _formState.Founder1Nationality  != null
                && _formState.Founder1Location     != null
                && _formState.Founder1Background   != null
                && _formState.Founder1SkillProfile != null;
        }

        private bool AllRequiredStepsValid()
        {
            foreach (var def in StepDefinitions)
            {
                if (def.Id == CompanyCreationStepId.Review)
                {
                    continue;
                }

                if (!ValidateStep(def.Id))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetFirstValidationError(CompanyCreationStepId step)
        {
            return step switch
            {
                CompanyCreationStepId.Company =>
                    string.IsNullOrWhiteSpace(_formState.CompanyName)
                        ? "Company Name is required."
                        : _formState.IndustryFocus == null
                            ? "Please select an Industry Focus."
                            : "Please select a Headquarters location.",

                CompanyCreationStepId.Background =>
                    "Please select a Company Background.",

                CompanyCreationStepId.Founders =>
                    "Please choose a Founder Setup type.",

                CompanyCreationStepId.FounderDetails =>
                    "Please complete all Founder Details fields.",

                CompanyCreationStepId.TeamBudget =>
                    "Please select a Starting Cash preset.",

                CompanyCreationStepId.Review =>
                    "One or more steps are incomplete. Please review before confirming.",

                _ => "This step is incomplete."
            };
        }

        // ─── Step sequencing ─────────────────────────────────────────────────────────

        private CompanyCreationStepId GetNextStep(CompanyCreationStepId current)
        {
            for (int i = 0; i < StepDefinitions.Length - 1; i++)
            {
                if (StepDefinitions[i].Id == current)
                {
                    return StepDefinitions[i + 1].Id;
                }
            }

            return current;
        }

        private CompanyCreationStepId? GetPreviousStep(CompanyCreationStepId current)
        {
            for (int i = 1; i < StepDefinitions.Length; i++)
            {
                if (StepDefinitions[i].Id == current)
                {
                    return StepDefinitions[i - 1].Id;
                }
            }

            return null;
        }

        // ─── Form field dispatch ─────────────────────────────────────────────────────

        private void ApplyField(string fieldName, string value)
        {
            switch (fieldName)
            {
                case "CompanyName":          _formState.CompanyName        = value;           break;
                case "IndustryFocus":        _formState.IndustryFocus      = value;           break;
                case "Headquarters":         _formState.Headquarters        = value;           break;
                case "CompanyColour":        _formState.CompanyColour       = value;           break;
                case "LogoId":              _formState.LogoId              = value;           break;
                case "BackgroundId":         _formState.BackgroundId        = value;           break;
                case "FounderSetupType":     _formState.FounderSetupType    = value;           break;
                case "StartingCashPreset":   _formState.StartingCashPreset  = value;           break;
                case "StartingTeamChoice":   _formState.StartingTeamChoice  = value;           break;
                case "MarketSize":           _formState.MarketSize           = value;           break;
                case "CompetitorDensity":    _formState.CompetitorDensity    = value;           break;
                case "TechnologyPace":       _formState.TechnologyPace       = value;           break;
                case "EconomicVolatility":   _formState.EconomicVolatility   = value;           break;
                case "HiringDifficulty":     _formState.HiringDifficulty     = value;           break;
                case "FailureMode":          _formState.FailureMode           = value;           break;
                case "MarketSeed":           _formState.MarketSeed            = value;           break;

                case "Founder0FirstName":    _formState.Founder0FirstName    = value;           break;
                case "Founder0LastName":     _formState.Founder0LastName     = value;           break;
                case "Founder0Age":
                    if (int.TryParse(value, out int age0)) { _formState.Founder0Age = age0; }  break;
                case "Founder0Nationality":  _formState.Founder0Nationality  = value;           break;
                case "Founder0Location":     _formState.Founder0Location     = value;           break;
                case "Founder0Background":   _formState.Founder0Background   = value;           break;
                case "Founder0SkillProfile": _formState.Founder0SkillProfile = value;           break;

                case "Founder1FirstName":    _formState.Founder1FirstName    = value;           break;
                case "Founder1LastName":     _formState.Founder1LastName     = value;           break;
                case "Founder1Age":
                    if (int.TryParse(value, out int age1)) { _formState.Founder1Age = age1; }  break;
                case "Founder1Nationality":  _formState.Founder1Nationality  = value;           break;
                case "Founder1Location":     _formState.Founder1Location     = value;           break;
                case "Founder1Background":   _formState.Founder1Background   = value;           break;
                case "Founder1SkillProfile": _formState.Founder1SkillProfile = value;           break;

                default:
                    DebugLogger.LogWarning(DebugCategory.UI,
                        $"CompanyCreationController.ApplyField: unknown field '{fieldName}'.");
                    break;
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────────

        private List<FounderProfileViewModel> BuildFounderProfiles()
        {
            var profiles = new List<FounderProfileViewModel>
            {
                new FounderProfileViewModel(
                    _formState.Founder0FirstName,
                    _formState.Founder0LastName,
                    _formState.Founder0Age,
                    _formState.Founder0Nationality  ?? string.Empty,
                    _formState.Founder0Location     ?? string.Empty,
                    _formState.Founder0Background   ?? string.Empty,
                    _formState.Founder0SkillProfile ?? string.Empty,
                    IsFounderProfileComplete(0))
            };

            if (_formState.FounderSetupType == "co-founders")
            {
                profiles.Add(new FounderProfileViewModel(
                    _formState.Founder1FirstName,
                    _formState.Founder1LastName,
                    _formState.Founder1Age,
                    _formState.Founder1Nationality  ?? string.Empty,
                    _formState.Founder1Location     ?? string.Empty,
                    _formState.Founder1Background   ?? string.Empty,
                    _formState.Founder1SkillProfile ?? string.Empty,
                    IsFounderProfileComplete(1)));
            }

            return profiles;
        }

        private SandboxSettingsViewModel BuildSandboxSettings()
        {
            return new SandboxSettingsViewModel(
                _formState.MarketSize,
                _formState.CompetitorDensity,
                _formState.TechnologyPace,
                _formState.EconomicVolatility,
                _formState.HiringDifficulty,
                _formState.FailureMode,
                _formState.MarketSeed);
        }

        private static string GetCashDisplayAmount(string preset)
        {
            return preset switch
            {
                "lean"      => "£35,000",
                "standard"  => "£50,000",
                "supported" => "£75,000",
                "sandbox"   => "Custom",
                _           => "£50,000"
            };
        }
    }
}
