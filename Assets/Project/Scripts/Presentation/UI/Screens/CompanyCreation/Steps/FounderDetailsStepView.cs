using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 4 — Founder Details.
    /// Renders: name, age, nationality, location fields; founder background and skill profile
    /// choice cards; co-founder tab buttons; avatar placeholder.
    /// </summary>
    public sealed class FounderDetailsStepView
    {
        // ─── Options ─────────────────────────────────────────────────────────────────

        private static readonly List<string> NationalityOptions = new List<string>
        {
            "British", "American", "Canadian", "Australian", "German", "French",
            "Swedish", "Dutch", "Spanish", "Italian", "Indian", "Chinese", "Japanese",
            "Brazilian", "South African", "Other"
        };

        private static readonly List<string> LocationOptions = new List<string>
        {
            "London, UK", "Manchester, UK", "Birmingham, UK", "Edinburgh, UK",
            "New York, USA", "San Francisco, USA", "Berlin, Germany", "Amsterdam, Netherlands",
            "Paris, France", "Stockholm, Sweden", "Sydney, Australia", "Remote"
        };

        private static readonly string[] BackgroundOptions =
        {
            "Engineer", "Product Designer", "Sales Founder", "Hardware Specialist",
            "Research Founder", "Serial Founder", "Bootstrapped Founder"
        };

        private static readonly string[] SkillProfileOptions =
        {
            "Technical", "Product", "Commercial", "Design", "Hardware", "Research", "Operations"
        };

        // ─── Internal state ──────────────────────────────────────────────────────────

        private int _activeFounderIndex;

        // Per-founder element references [founderIndex]
        private readonly TextField[]           _firstNameFields    = new TextField[2];
        private readonly TextField[]           _lastNameFields     = new TextField[2];
        private readonly DropdownField[]       _ageDropdowns       = new DropdownField[2];
        private readonly DropdownField[]       _nationalityDrops   = new DropdownField[2];
        private readonly DropdownField[]       _locationDrops      = new DropdownField[2];
        private readonly List<VisualElement>[] _backgroundCards    = { new List<VisualElement>(), new List<VisualElement>() };
        private readonly List<VisualElement>[] _skillCards         = { new List<VisualElement>(), new List<VisualElement>() };

        private readonly VisualElement[] _founderForms = new VisualElement[2];
        private readonly Button[]        _tabButtons   = new Button[2];

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when a non-founder field changes.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        /// <summary>Callback(founderIndex, fieldName, value) fired for founder-specific fields.</summary>
        public Action<int, string, string> OnFounderFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public FounderDetailsStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Founder Details",
                "Complete the profile for each founder."));

            // Tab row — shown/hidden based on co-founder selection
            var tabRow = new VisualElement();
            tabRow.AddToClassList("company-creation__tab-row");

            for (int i = 0; i < 2; i++)
            {
                int captured = i;
                var tabBtn = new Button(() => SetActiveFounder(captured));
                tabBtn.text = i == 0 ? "Founder 1" : "Founder 2";
                tabBtn.AddToClassList("base-button");
                tabBtn.AddToClassList("base-button--secondary");
                _tabButtons[i] = tabBtn;
                tabRow.Add(tabBtn);
            }

            Root.Add(tabRow);

            // Build a form panel per founder
            for (int fi = 0; fi < 2; fi++)
            {
                _founderForms[fi] = BuildFounderForm(fi);
                Root.Add(_founderForms[fi]);
            }

            // Default: show Founder 0, hide Founder 1
            SetActiveFounder(0);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            // Show tab row only for co-founders
            bool isCoFounders = vm.FounderSetupType == "co-founders";
            _tabButtons[1].parent?.SetEnabled(isCoFounders);

            for (int i = 0; i < vm.FounderProfiles.Count && i < 2; i++)
            {
                FounderProfileViewModel profile = vm.FounderProfiles[i];

                BindFounderForm(i, profile);
            }
        }

        // ─── Private form builder ─────────────────────────────────────────────────────

        private VisualElement BuildFounderForm(int founderIndex)
        {
            var form = new VisualElement();
            form.AddToClassList("company-creation__form-grid");

            // ── First Name ────────────────────────────────────────────────────────────
            var firstNameWrapper = BuildFieldWrapper("First Name");
            var firstNameField = new TextField();
            firstNameField.AddToClassList("search-input");
            firstNameField.RegisterValueChangedCallback(e =>
                OnFounderFieldChanged?.Invoke(founderIndex, "FirstName", e.newValue));
            _firstNameFields[founderIndex] = firstNameField;
            firstNameWrapper.Add(firstNameField);
            form.Add(firstNameWrapper);

            // ── Last Name ─────────────────────────────────────────────────────────────
            var lastNameWrapper = BuildFieldWrapper("Last Name");
            var lastNameField = new TextField();
            lastNameField.AddToClassList("search-input");
            lastNameField.RegisterValueChangedCallback(e =>
                OnFounderFieldChanged?.Invoke(founderIndex, "LastName", e.newValue));
            _lastNameFields[founderIndex] = lastNameField;
            lastNameWrapper.Add(lastNameField);
            form.Add(lastNameWrapper);

            // ── Age ───────────────────────────────────────────────────────────────────
            var ageWrapper = BuildFieldWrapper("Age");
            var ageChoices = new List<string>();
            for (int a = 18; a <= 80; a++) { ageChoices.Add(a.ToString()); }

            var ageDrop = new DropdownField(ageChoices, 0);
            ageDrop.AddToClassList("dropdown-field");
            ageDrop.RegisterValueChangedCallback(e =>
                OnFounderFieldChanged?.Invoke(founderIndex, "Age", e.newValue));
            _ageDropdowns[founderIndex] = ageDrop;
            ageWrapper.Add(ageDrop);
            form.Add(ageWrapper);

            // ── Nationality ───────────────────────────────────────────────────────────
            var natWrapper = BuildFieldWrapper("Nationality");
            var natDrop = new DropdownField(NationalityOptions, 0);
            natDrop.AddToClassList("dropdown-field");
            natDrop.RegisterValueChangedCallback(e =>
                OnFounderFieldChanged?.Invoke(founderIndex, "Nationality", e.newValue));
            _nationalityDrops[founderIndex] = natDrop;
            natWrapper.Add(natDrop);
            form.Add(natWrapper);

            // ── Location ──────────────────────────────────────────────────────────────
            var locWrapper = BuildFieldWrapper("Location");
            var locDrop = new DropdownField(LocationOptions, 0);
            locDrop.AddToClassList("dropdown-field");
            locDrop.RegisterValueChangedCallback(e =>
                OnFounderFieldChanged?.Invoke(founderIndex, "Location", e.newValue));
            _locationDrops[founderIndex] = locDrop;
            locWrapper.Add(locDrop);
            form.Add(locWrapper);

            // ── Founder Background choice cards ───────────────────────────────────────
            var bgWrapper = BuildFieldWrapper("Founder Background");
            var bgGrid = new VisualElement();
            bgGrid.AddToClassList("company-creation__choice-grid");

            foreach (string bg in BackgroundOptions)
            {
                string captured = bg;
                int capturedIndex = founderIndex;

                var card = BuildSmallChoiceCard(bg);
                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetCardGroupSelection(_backgroundCards[capturedIndex], captured);
                    OnFounderFieldChanged?.Invoke(capturedIndex, "Background", captured);
                });

                _backgroundCards[founderIndex].Add(card);
                bgGrid.Add(card);
            }

            bgWrapper.Add(bgGrid);
            form.Add(bgWrapper);

            // ── Primary Skill Profile choice cards ────────────────────────────────────
            var skillWrapper = BuildFieldWrapper("Primary Skill Profile");
            var skillGrid = new VisualElement();
            skillGrid.AddToClassList("company-creation__choice-grid");

            foreach (string skill in SkillProfileOptions)
            {
                string captured = skill;
                int capturedIndex = founderIndex;

                var card = BuildSmallChoiceCard(skill);
                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetCardGroupSelection(_skillCards[capturedIndex], captured);
                    OnFounderFieldChanged?.Invoke(capturedIndex, "SkillProfile", captured);
                });

                _skillCards[founderIndex].Add(card);
                skillGrid.Add(card);
            }

            skillWrapper.Add(skillGrid);
            form.Add(skillWrapper);

            // ── Avatar placeholder ────────────────────────────────────────────────────
            var avatarWrapper = BuildFieldWrapper("Avatar");
            var avatar = new VisualElement();
            avatar.AddToClassList("company-creation__avatar-placeholder");
            var initials = new Label("?");
            initials.AddToClassList("text-heading");
            avatar.Add(initials);
            avatarWrapper.Add(avatar);
            form.Add(avatarWrapper);

            return form;
        }

        private void SetActiveFounder(int index)
        {
            _activeFounderIndex = index;

            for (int i = 0; i < 2; i++)
            {
                bool isActive = i == index;
                _founderForms[i].style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;

                if (_tabButtons[i] != null)
                {
                    if (isActive) { _tabButtons[i].AddToClassList("is-active"); }
                    else          { _tabButtons[i].RemoveFromClassList("is-active"); }
                }
            }
        }

        private void BindFounderForm(int index, FounderProfileViewModel profile)
        {
            if (_firstNameFields[index] != null)
            {
                _firstNameFields[index].SetValueWithoutNotify(profile.FirstName);
            }

            if (_lastNameFields[index] != null)
            {
                _lastNameFields[index].SetValueWithoutNotify(profile.LastName);
            }

            if (!string.IsNullOrEmpty(profile.Background))
            {
                SetCardGroupSelectionByLabel(_backgroundCards[index], profile.Background);
            }

            if (!string.IsNullOrEmpty(profile.PrimarySkillProfile))
            {
                SetCardGroupSelectionByLabel(_skillCards[index], profile.PrimarySkillProfile);
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────────

        private static void SetCardGroupSelection(List<VisualElement> cards, string selectedTitle)
        {
            foreach (VisualElement card in cards)
            {
                var lbl = card.Q<Label>(className: "wizard-choice-card__title");
                bool isSelected = lbl != null && lbl.text == selectedTitle;
                if (isSelected) { card.AddToClassList("is-selected"); }
                else            { card.RemoveFromClassList("is-selected"); }
            }
        }

        private static void SetCardGroupSelectionByLabel(List<VisualElement> cards, string label)
        {
            SetCardGroupSelection(cards, label);
        }

        private static VisualElement BuildSmallChoiceCard(string title)
        {
            var card = new VisualElement();
            card.AddToClassList("wizard-choice-card");

            var lbl = new Label(title);
            lbl.AddToClassList("wizard-choice-card__title");
            lbl.AddToClassList("text-small");
            card.Add(lbl);

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
