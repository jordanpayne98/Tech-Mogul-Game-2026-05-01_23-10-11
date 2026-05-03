using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 1 — Company Identity.
    /// Renders: Company Name, Industry Focus choice cards, Headquarters dropdown,
    /// Company Colour placeholder, Logo placeholder.
    /// </summary>
    public sealed class CompanyStepView
    {
        // ─── Industry focus options ──────────────────────────────────────────────────

        private static readonly string[] IndustryOptions =
        {
            "Consumer Software",
            "Enterprise SaaS",
            "Developer Tools",
            "Games & Entertainment",
            "Hardware Devices",
            "Cloud Infrastructure",
            "Security",
            "AI & Automation",
            "Platform Ecosystems"
        };

        // ─── Headquarters options ────────────────────────────────────────────────────

        private static readonly List<string> HeadquarterOptions = new List<string>
        {
            "London, UK",
            "Manchester, UK",
            "Birmingham, UK",
            "Edinburgh, UK",
            "Remote-first"
        };

        // ─── Internal elements ───────────────────────────────────────────────────────

        private readonly TextField           _companyNameField;
        private readonly DropdownField       _hqDropdown;
        private readonly List<VisualElement> _industryCards = new List<VisualElement>();

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when any field value changes.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public CompanyStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            // Step heading
            Root.Add(BuildStepHeading("Company Identity",
                "Define your company's name, industry focus, and headquarters."));

            // Form grid
            var formGrid = new VisualElement();
            formGrid.AddToClassList("company-creation__form-grid");
            Root.Add(formGrid);

            // ── Company Name ─────────────────────────────────────────────────────────
            var nameField = BuildFieldWrapper("Company Name");

            _companyNameField = new TextField();
            _companyNameField.maxLength = 32;
            _companyNameField.AddToClassList("search-input");
            _companyNameField.RegisterValueChangedCallback(e =>
                OnFieldChanged?.Invoke("CompanyName", e.newValue));
            nameField.Add(_companyNameField);

            formGrid.Add(nameField);

            // ── Industry Focus ───────────────────────────────────────────────────────
            var industrySection = BuildFieldWrapper("Industry Focus");

            var industryGrid = new VisualElement();
            industryGrid.AddToClassList("company-creation__choice-grid");

            foreach (string option in IndustryOptions)
            {
                string captured = option;
                var card = BuildChoiceCard(option, string.Empty);

                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetIndustrySelection(captured);
                    OnFieldChanged?.Invoke("IndustryFocus", captured);
                });

                _industryCards.Add(card);
                industryGrid.Add(card);
            }

            industrySection.Add(industryGrid);
            formGrid.Add(industrySection);

            // ── Headquarters ─────────────────────────────────────────────────────────
            var hqField = BuildFieldWrapper("Headquarters");

            _hqDropdown = new DropdownField(HeadquarterOptions, 0);
            _hqDropdown.AddToClassList("dropdown-field");
            _hqDropdown.RegisterValueChangedCallback(e =>
                OnFieldChanged?.Invoke("Headquarters", e.newValue));
            hqField.Add(_hqDropdown);

            formGrid.Add(hqField);

            // ── Company Colour (placeholder) ─────────────────────────────────────────
            var colourField = BuildFieldWrapper("Company Colour");
            var colourPlaceholder = new Label("[Placeholder] Colour picker — Phase 5");
            colourPlaceholder.AddToClassList("text-small");
            colourField.Add(colourPlaceholder);
            formGrid.Add(colourField);

            // ── Logo / Icon (placeholder) ────────────────────────────────────────────
            var logoField = BuildFieldWrapper("Logo / Icon");
            var logoPlaceholder = new Label("[Placeholder] Logo selector — Phase 5");
            logoPlaceholder.AddToClassList("text-small");
            logoField.Add(logoPlaceholder);
            formGrid.Add(logoField);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        /// <summary>Populates field values from the ViewModel without triggering callbacks.</summary>
        public void Bind(CompanyCreationViewModel vm)
        {
            // Set company name without triggering the change callback.
            if (_companyNameField.value != vm.CompanyName)
            {
                _companyNameField.SetValueWithoutNotify(vm.CompanyName);
            }

            // Sync industry cards.
            foreach (VisualElement card in _industryCards)
            {
                var label = card.Q<Label>(className: "wizard-choice-card__title");
                bool isSelected = label != null && label.text == vm.IndustryFocus;
                SetCardSelected(card, isSelected);
            }

            // Sync HQ dropdown.
            if (!string.IsNullOrEmpty(vm.Headquarters) && _hqDropdown.value != vm.Headquarters)
            {
                _hqDropdown.SetValueWithoutNotify(vm.Headquarters);
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private void SetIndustrySelection(string selected)
        {
            foreach (VisualElement card in _industryCards)
            {
                var label = card.Q<Label>(className: "wizard-choice-card__title");
                SetCardSelected(card, label != null && label.text == selected);
            }
        }

        private static void SetCardSelected(VisualElement card, bool selected)
        {
            if (selected)
            {
                card.AddToClassList("is-selected");
            }
            else
            {
                card.RemoveFromClassList("is-selected");
            }
        }

        private static VisualElement BuildChoiceCard(string title, string description)
        {
            var card = new VisualElement();
            card.AddToClassList("wizard-choice-card");

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("wizard-choice-card__title");
            titleLabel.AddToClassList("text-small");
            card.Add(titleLabel);

            if (!string.IsNullOrEmpty(description))
            {
                var descLabel = new Label(description);
                descLabel.AddToClassList("wizard-choice-card__description");
                descLabel.AddToClassList("text-small");
                card.Add(descLabel);
            }

            return card;
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

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("wizard-step-heading__title");
            titleLabel.AddToClassList("text-heading");
            heading.Add(titleLabel);

            var subtitleLabel = new Label(subtitle);
            subtitleLabel.AddToClassList("wizard-step-heading__subtitle");
            subtitleLabel.AddToClassList("text-body");
            heading.Add(subtitleLabel);

            return heading;
        }
    }
}
