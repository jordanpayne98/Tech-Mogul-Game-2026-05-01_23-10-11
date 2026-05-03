using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 7 — Review Company Setup.
    /// Renders one review section card per preceding wizard step,
    /// showing key values with an "Edit" link per section.
    /// Incomplete sections show a warning marker.
    /// </summary>
    public sealed class ReviewStepView
    {
        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) — not used in review but satisfies view interface.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        /// <summary>Callback invoked when the player clicks Edit on a review section.</summary>
        public Action<CompanyCreationStepId> OnEditStepRequested { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public ReviewStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Review Company Setup",
                "Review all sections before creating your company. Click Edit to make changes."));

            // Sections are rebuilt on every Bind call.
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            // Clear previous sections (all children after the heading).
            while (Root.childCount > 1)
            {
                Root.RemoveAt(1);
            }

            Root.Add(BuildSection(
                title:   "Company Identity",
                stepId:  CompanyCreationStepId.Company,
                rows:    new[]
                {
                    ("Company Name", string.IsNullOrWhiteSpace(vm.CompanyName) ? "Not set" : vm.CompanyName),
                    ("Industry",     string.IsNullOrWhiteSpace(vm.IndustryFocus) ? "Not set" : vm.IndustryFocus),
                    ("Headquarters", string.IsNullOrWhiteSpace(vm.Headquarters) ? "Not set" : vm.Headquarters),
                },
                isValid: !string.IsNullOrWhiteSpace(vm.CompanyName)
                      && !string.IsNullOrWhiteSpace(vm.IndustryFocus)
                      && !string.IsNullOrWhiteSpace(vm.Headquarters)));

            Root.Add(BuildSection(
                title:   "Company Background",
                stepId:  CompanyCreationStepId.Background,
                rows:    new[]
                {
                    ("Background", string.IsNullOrWhiteSpace(vm.BackgroundId) ? "Not set" : vm.BackgroundId),
                },
                isValid: !string.IsNullOrWhiteSpace(vm.BackgroundId)));

            Root.Add(BuildSection(
                title:   "Founder Setup",
                stepId:  CompanyCreationStepId.Founders,
                rows:    new[]
                {
                    ("Setup Type", string.IsNullOrWhiteSpace(vm.FounderSetupType) ? "Not set" : vm.FounderSetupType),
                },
                isValid: !string.IsNullOrWhiteSpace(vm.FounderSetupType)));

            // Founder Details
            bool founderDetailsValid = vm.FounderProfiles.Count > 0 && vm.FounderProfiles[0].IsComplete;
            var founderRows = new List<(string, string)>();

            for (int i = 0; i < vm.FounderProfiles.Count; i++)
            {
                FounderProfileViewModel profile = vm.FounderProfiles[i];
                string label = vm.FounderProfiles.Count > 1 ? $"Founder {i + 1}" : "Founder";
                string name  = string.IsNullOrWhiteSpace(profile.FirstName) && string.IsNullOrWhiteSpace(profile.LastName)
                    ? "Not set"
                    : $"{profile.FirstName} {profile.LastName}".Trim();

                founderRows.Add((label, name));
                founderRows.Add(($"{label} Background", string.IsNullOrWhiteSpace(profile.Background) ? "Not set" : profile.Background));
            }

            Root.Add(BuildSection(
                title:   "Founder Details",
                stepId:  CompanyCreationStepId.FounderDetails,
                rows:    founderRows.ToArray(),
                isValid: founderDetailsValid));

            Root.Add(BuildSection(
                title:   "Team & Budget",
                stepId:  CompanyCreationStepId.TeamBudget,
                rows:    new[]
                {
                    ("Starting Cash", vm.StartingCashAmount),
                    ("Starting Team", string.IsNullOrWhiteSpace(vm.StartingTeamChoice) ? "None" : vm.StartingTeamChoice),
                },
                isValid: !string.IsNullOrWhiteSpace(vm.StartingCashPreset)));

            SandboxSettingsViewModel sandbox = vm.SandboxSettings;
            Root.Add(BuildSection(
                title:   "Sandbox Setup",
                stepId:  CompanyCreationStepId.SandboxSetup,
                rows:    new[]
                {
                    ("Market Size",         sandbox?.MarketSize          ?? "Standard"),
                    ("Competitor Density",  sandbox?.CompetitorDensity   ?? "Standard"),
                    ("Technology Pace",     sandbox?.TechnologyPace      ?? "Standard"),
                    ("Economic Volatility", sandbox?.EconomicVolatility  ?? "Standard"),
                    ("Hiring Difficulty",   sandbox?.HiringDifficulty    ?? "Standard"),
                    ("Failure Mode",        sandbox?.FailureMode         ?? "Standard"),
                    ("Market Seed",         sandbox?.MarketSeed          ?? "-"),
                },
                isValid: true));
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private VisualElement BuildSection(string title, CompanyCreationStepId stepId,
            (string Label, string Value)[] rows, bool isValid)
        {
            var section = new VisualElement();
            section.AddToClassList("company-creation__review-section");

            if (!isValid)
            {
                section.AddToClassList("has-warning");
            }

            // Header
            var sectionHeader = new VisualElement();
            sectionHeader.AddToClassList("company-creation__review-section__header");

            var titleEl = new Label(title);
            titleEl.AddToClassList("text-body");

            if (!isValid)
            {
                titleEl.text = $"⚠ {title}";
            }

            sectionHeader.Add(titleEl);

            // Edit link
            CompanyCreationStepId capturedStep = stepId;
            var editLink = new Label("Edit");
            editLink.AddToClassList("company-creation__review-section__edit-link");
            editLink.RegisterCallback<ClickEvent>(_ => OnEditStepRequested?.Invoke(capturedStep));
            sectionHeader.Add(editLink);

            section.Add(sectionHeader);

            // Rows
            foreach (var (label, value) in rows)
            {
                var row = new VisualElement();
                row.AddToClassList("company-creation__summary-row");

                var labelEl = new Label(label);
                labelEl.AddToClassList("company-creation__summary-label");
                row.Add(labelEl);

                var valueEl = new Label(value);
                valueEl.AddToClassList("company-creation__summary-value");

                bool isSet = value != "Not set" && value != "None" && !string.IsNullOrWhiteSpace(value);
                if (!isSet)
                {
                    valueEl.AddToClassList("is-unset");
                }

                row.Add(valueEl);
                section.Add(row);
            }

            return section;
        }

        private static VisualElement BuildStepHeading(string title, string subtitle)
        {
            var heading = new VisualElement();
            heading.AddToClassList("wizard-step-heading");

            var t = new Label(title);
            t.AddToClassList("wizard-step-heading__title");
            t.AddToClassList("text-heading");
            heading.Add(t);

            var s = new Label(subtitle);
            s.AddToClassList("wizard-step-heading__subtitle");
            s.AddToClassList("text-body");
            heading.Add(s);

            return heading;
        }
    }
}
