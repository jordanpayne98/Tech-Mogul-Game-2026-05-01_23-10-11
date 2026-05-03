using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 6 — Sandbox Setup.
    /// Renders segmented controls for 6 sandbox settings and a market seed row.
    /// All settings default to "Standard".
    /// </summary>
    public sealed class SandboxSetupStepView
    {
        // ─── Setting definitions ─────────────────────────────────────────────────────

        private readonly struct SegmentedSetting
        {
            public readonly string FieldName;
            public readonly string Label;
            public readonly string[] Options;

            public SegmentedSetting(string fieldName, string label, string[] options)
            {
                FieldName = fieldName; Label = label; Options = options;
            }
        }

        private static readonly SegmentedSetting[] Settings =
        {
            new SegmentedSetting("MarketSize",         "Market Size",         new[] { "Small",      "Standard", "Large"    }),
            new SegmentedSetting("CompetitorDensity",  "Competitor Density",  new[] { "Low",        "Standard", "High"     }),
            new SegmentedSetting("TechnologyPace",     "Technology Pace",     new[] { "Slow",       "Standard", "Fast"     }),
            new SegmentedSetting("EconomicVolatility", "Economic Volatility", new[] { "Calm",       "Standard", "Chaotic"  }),
            new SegmentedSetting("HiringDifficulty",   "Hiring Difficulty",   new[] { "Easy",       "Standard", "Hard"     }),
            new SegmentedSetting("FailureMode",        "Failure Mode",        new[] { "Forgiving",  "Standard", "Hardcore", "Sandbox" }),
        };

        // ─── Internal references ─────────────────────────────────────────────────────

        // Maps fieldName -> list of segment buttons for that control.
        private readonly Dictionary<string, List<Button>> _segmentButtons
            = new Dictionary<string, List<Button>>();

        private readonly TextField _seedField;

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when any setting changes.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public SandboxSetupStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Sandbox Setup",
                "Configure the simulation environment for your playthrough."));

            var formGrid = new VisualElement();
            formGrid.AddToClassList("company-creation__form-grid");
            Root.Add(formGrid);

            // ── Segmented controls ────────────────────────────────────────────────────
            foreach (SegmentedSetting setting in Settings)
            {
                string capturedFieldName = setting.FieldName;

                var fieldWrapper = BuildFieldWrapper(setting.Label);

                var segmentControl = new VisualElement();
                segmentControl.AddToClassList("segmented-control");

                var buttons = new List<Button>();
                _segmentButtons[setting.FieldName] = buttons;

                foreach (string option in setting.Options)
                {
                    string capturedOption = option;

                    var segment = new Button(() =>
                    {
                        SetSegmentSelection(capturedFieldName, capturedOption);
                        OnFieldChanged?.Invoke(capturedFieldName, capturedOption);
                    });

                    segment.text = option;
                    segment.AddToClassList("segmented-control__segment");

                    buttons.Add(segment);
                    segmentControl.Add(segment);
                }

                // Default selection = "Standard" (or first option if Standard not in list)
                SetSegmentSelection(setting.FieldName, "Standard");

                fieldWrapper.Add(segmentControl);
                formGrid.Add(fieldWrapper);
            }

            // ── Market Seed ───────────────────────────────────────────────────────────
            var seedWrapper = BuildFieldWrapper("Market Seed");
            var seedRow = new VisualElement();
            seedRow.AddToClassList("company-creation__seed-row");

            _seedField = new TextField();
            _seedField.AddToClassList("search-input");
            _seedField.RegisterValueChangedCallback(e =>
                OnFieldChanged?.Invoke("MarketSeed", e.newValue));
            seedRow.Add(_seedField);

            var randomiseBtn = new Button(() =>
            {
                string newSeed = UnityEngine.Random.Range(100000, 999999).ToString();
                _seedField.SetValueWithoutNotify(newSeed);
                OnFieldChanged?.Invoke("MarketSeed", newSeed);
            });

            randomiseBtn.text = "Randomise";
            randomiseBtn.AddToClassList("base-button");
            randomiseBtn.AddToClassList("base-button--secondary");
            randomiseBtn.AddToClassList("base-button--compact");
            seedRow.Add(randomiseBtn);

            seedWrapper.Add(seedRow);
            formGrid.Add(seedWrapper);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            SandboxSettingsViewModel sandbox = vm.SandboxSettings;
            if (sandbox == null) { return; }

            SetSegmentSelection("MarketSize",         sandbox.MarketSize);
            SetSegmentSelection("CompetitorDensity",  sandbox.CompetitorDensity);
            SetSegmentSelection("TechnologyPace",     sandbox.TechnologyPace);
            SetSegmentSelection("EconomicVolatility", sandbox.EconomicVolatility);
            SetSegmentSelection("HiringDifficulty",   sandbox.HiringDifficulty);
            SetSegmentSelection("FailureMode",        sandbox.FailureMode);

            if (_seedField != null && _seedField.value != sandbox.MarketSeed)
            {
                _seedField.SetValueWithoutNotify(sandbox.MarketSeed);
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private void SetSegmentSelection(string fieldName, string selectedOption)
        {
            if (!_segmentButtons.TryGetValue(fieldName, out List<Button> buttons))
            {
                return;
            }

            foreach (Button btn in buttons)
            {
                if (btn.text == selectedOption)
                {
                    btn.AddToClassList("is-active");
                }
                else
                {
                    btn.RemoveFromClassList("is-active");
                }
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
