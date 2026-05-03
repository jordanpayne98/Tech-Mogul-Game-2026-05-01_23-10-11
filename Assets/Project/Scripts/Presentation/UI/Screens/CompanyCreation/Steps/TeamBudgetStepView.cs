using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 5 — Team and Budget.
    /// Renders: Starting Cash Preset cards, Starting Team placeholder cards,
    /// Budget Allocation placeholder section.
    /// </summary>
    public sealed class TeamBudgetStepView
    {
        // ─── Cash preset data ────────────────────────────────────────────────────────

        private readonly struct CashPreset
        {
            public readonly string Id;
            public readonly string Label;
            public readonly string Amount;
            public readonly string Description;

            public CashPreset(string id, string label, string amount, string description)
            {
                Id = id; Label = label; Amount = amount; Description = description;
            }
        }

        private static readonly CashPreset[] CashPresets =
        {
            new CashPreset("lean",      "Lean Start",       "£35,000", "Minimal runway. Every hire counts."),
            new CashPreset("standard",  "Standard Startup", "£50,000", "Balanced starting position."),
            new CashPreset("supported", "Supported Start",  "£75,000", "More headroom for early mistakes."),
            new CashPreset("sandbox",   "Sandbox Custom",   "Custom",  "[Placeholder] Custom amount selector."),
        };

        // ─── Starting team options ────────────────────────────────────────────────────

        private static readonly string[] TeamOptions =
        {
            "No Starting Team",
            "Small Founding Team [Placeholder]",
            "Custom Starting Team [Placeholder]"
        };

        // ─── Internal references ─────────────────────────────────────────────────────

        private readonly List<VisualElement> _cashCards = new List<VisualElement>();
        private readonly List<VisualElement> _teamCards = new List<VisualElement>();

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when a field changes.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public TeamBudgetStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Team & Budget",
                "Set your starting cash and initial team configuration."));

            // ── Starting Cash Presets ─────────────────────────────────────────────────
            var cashSection = BuildFieldWrapper("Starting Cash");
            var cashGrid = new VisualElement();
            cashGrid.AddToClassList("company-creation__choice-grid");

            foreach (CashPreset preset in CashPresets)
            {
                string capturedId = preset.Id;

                var card = new VisualElement();
                card.AddToClassList("wizard-choice-card");
                card.userData = capturedId;

                var titleLabel = new Label(preset.Label);
                titleLabel.AddToClassList("wizard-choice-card__title");
                card.Add(titleLabel);

                var amountLabel = new Label(preset.Amount);
                amountLabel.AddToClassList("wizard-choice-card__description");
                card.Add(amountLabel);

                var descLabel = new Label(preset.Description);
                descLabel.AddToClassList("text-small");
                card.Add(descLabel);

                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetCashSelection(capturedId);
                    OnFieldChanged?.Invoke("StartingCashPreset", capturedId);
                });

                _cashCards.Add(card);
                cashGrid.Add(card);
            }

            cashSection.Add(cashGrid);
            Root.Add(cashSection);

            // ── Starting Team ─────────────────────────────────────────────────────────
            var teamSection = BuildFieldWrapper("Starting Team [Placeholder]");
            var teamGrid = new VisualElement();
            teamGrid.AddToClassList("company-creation__choice-grid");

            foreach (string teamOption in TeamOptions)
            {
                string captured = teamOption;

                var card = new VisualElement();
                card.AddToClassList("wizard-choice-card");
                card.userData = captured;

                var titleLabel = new Label(teamOption);
                titleLabel.AddToClassList("wizard-choice-card__title");
                card.Add(titleLabel);

                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetTeamSelection(captured);
                    OnFieldChanged?.Invoke("StartingTeamChoice", captured);
                });

                _teamCards.Add(card);
                teamGrid.Add(card);
            }

            teamSection.Add(teamGrid);
            Root.Add(teamSection);

            // ── Budget Allocation (placeholder) ───────────────────────────────────────
            var budgetSection = BuildFieldWrapper("Budget Allocation [Placeholder]");
            var budgetPlaceholder = new Label(
                "[Placeholder] Budget allocation breakdown will be implemented in a later phase.");
            budgetPlaceholder.AddToClassList("text-small");
            budgetSection.Add(budgetPlaceholder);
            Root.Add(budgetSection);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            foreach (VisualElement card in _cashCards)
            {
                bool isSelected = card.userData is string id && id == vm.StartingCashPreset;
                if (isSelected) { card.AddToClassList("is-selected"); }
                else            { card.RemoveFromClassList("is-selected"); }
            }

            foreach (VisualElement card in _teamCards)
            {
                bool isSelected = card.userData is string id && id == vm.StartingTeamChoice;
                if (isSelected) { card.AddToClassList("is-selected"); }
                else            { card.RemoveFromClassList("is-selected"); }
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private void SetCashSelection(string selectedId)
        {
            foreach (VisualElement card in _cashCards)
            {
                bool isSelected = card.userData is string id && id == selectedId;
                if (isSelected) { card.AddToClassList("is-selected"); }
                else            { card.RemoveFromClassList("is-selected"); }
            }
        }

        private void SetTeamSelection(string selectedId)
        {
            foreach (VisualElement card in _teamCards)
            {
                bool isSelected = card.userData is string id && id == selectedId;
                if (isSelected) { card.AddToClassList("is-selected"); }
                else            { card.RemoveFromClassList("is-selected"); }
            }
        }

        private static VisualElement BuildFieldWrapper(string labelText)
        {
            var wrapper = new VisualElement();
            wrapper.AddToClassList("company-creation__field");

            var label = new Label(labelText);
            label.AddToClassList("company-creation__field-label");
            wrapper.Add(label);

            return wrapper;
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
