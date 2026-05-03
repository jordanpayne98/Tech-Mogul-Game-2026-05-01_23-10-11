using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Top-level immutable display-data aggregating all wizard state.
    /// Built by CompanyCreationController and consumed by CompanyCreationView.
    /// </summary>
    public sealed class CompanyCreationViewModel
    {
        // ─── Navigation state ────────────────────────────────────────────────────────

        public CompanyCreationStepId              CurrentStep            { get; }
        public List<CompanyCreationStepViewModel> Steps                  { get; }
        public bool                               CanGoBack              { get; }
        public bool                               CanGoNext              { get; }
        public bool                               CanConfirm             { get; }

        // ─── Footer ──────────────────────────────────────────────────────────────────

        public string FooterHelperText        { get; }
        public string FooterValidationMessage { get; }
        public string NextButtonText          { get; }

        // ─── Company identity ────────────────────────────────────────────────────────

        public string CompanyName     { get; }
        public string IndustryFocus   { get; }
        public string Headquarters    { get; }
        public string CompanyColour   { get; }
        public string LogoId          { get; }

        // ─── Background ──────────────────────────────────────────────────────────────

        public string BackgroundId { get; }

        // ─── Founders ────────────────────────────────────────────────────────────────

        public string                      FounderSetupType  { get; }
        public List<FounderProfileViewModel> FounderProfiles  { get; }

        // ─── Team & budget ───────────────────────────────────────────────────────────

        public string StartingCashPreset  { get; }
        public string StartingCashAmount  { get; }
        public string StartingTeamChoice  { get; }

        // ─── Sandbox settings ────────────────────────────────────────────────────────

        public SandboxSettingsViewModel SandboxSettings { get; }

        // ─── Summary panel ───────────────────────────────────────────────────────────

        public CompanyCreationSummaryViewModel Summary { get; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public CompanyCreationViewModel(
            CompanyCreationStepId              currentStep,
            List<CompanyCreationStepViewModel> steps,
            bool                               canGoBack,
            bool                               canGoNext,
            bool                               canConfirm,
            string                             footerHelperText,
            string                             footerValidationMessage,
            string                             nextButtonText,
            string                             companyName,
            string                             industryFocus,
            string                             headquarters,
            string                             companyColour,
            string                             logoId,
            string                             backgroundId,
            string                             founderSetupType,
            List<FounderProfileViewModel>      founderProfiles,
            string                             startingCashPreset,
            string                             startingCashAmount,
            string                             startingTeamChoice,
            SandboxSettingsViewModel           sandboxSettings,
            CompanyCreationSummaryViewModel    summary)
        {
            CurrentStep            = currentStep;
            Steps                  = steps                  ?? new List<CompanyCreationStepViewModel>();
            CanGoBack              = canGoBack;
            CanGoNext              = canGoNext;
            CanConfirm             = canConfirm;
            FooterHelperText       = footerHelperText       ?? string.Empty;
            FooterValidationMessage = footerValidationMessage ?? string.Empty;
            NextButtonText         = nextButtonText         ?? "Next";
            CompanyName            = companyName            ?? string.Empty;
            IndustryFocus          = industryFocus          ?? string.Empty;
            Headquarters           = headquarters           ?? string.Empty;
            CompanyColour          = companyColour          ?? string.Empty;
            LogoId                 = logoId                 ?? string.Empty;
            BackgroundId           = backgroundId           ?? string.Empty;
            FounderSetupType       = founderSetupType       ?? string.Empty;
            FounderProfiles        = founderProfiles        ?? new List<FounderProfileViewModel>();
            StartingCashPreset     = startingCashPreset     ?? string.Empty;
            StartingCashAmount     = startingCashAmount     ?? string.Empty;
            StartingTeamChoice     = startingTeamChoice     ?? string.Empty;
            SandboxSettings        = sandboxSettings;
            Summary                = summary;
        }
    }
}
