using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 2 — Company Background.
    /// Renders 6 large choice cards: title, description, tags, strengths, risks, difficulty.
    /// </summary>
    public sealed class BackgroundStepView
    {
        // ─── Background option data ──────────────────────────────────────────────────

        private readonly struct BackgroundOption
        {
            public readonly string Id;
            public readonly string Title;
            public readonly string Description;
            public readonly string Tags;
            public readonly string Strengths;
            public readonly string Risks;
            public readonly string Difficulty;

            public BackgroundOption(string id, string title, string description, string tags,
                string strengths, string risks, string difficulty)
            {
                Id = id; Title = title; Description = description;
                Tags = tags; Strengths = strengths; Risks = risks; Difficulty = difficulty;
            }
        }

        private static readonly BackgroundOption[] Options =
        {
            new BackgroundOption("design_studio",       "Design Studio",
                "A creative agency known for polished products and strong brand identity.",
                "Design · Creative · Branding",
                "+ Strong visual identity\n+ Premium brand trust",
                "- Engineering hiring is harder\n- Engineering runway is shorter",
                "Medium"),

            new BackgroundOption("enterprise_consultancy", "Enterprise Consultancy",
                "A professional services firm with deep client relationships.",
                "B2B · Consulting · Enterprise",
                "+ Existing client relationships\n+ Revenue from day one",
                "- Product-led growth is harder\n- Engineering culture must be built",
                "Hard"),

            new BackgroundOption("game_studio",         "Game Studio",
                "A game development studio pivoting to apply technical craft to a new domain.",
                "Games · Technical · Creative",
                "+ Strong technical talent pool\n+ Community-led marketing",
                "- Longer revenue cycles\n- Niche audience risks",
                "Medium"),

            new BackgroundOption("growth_agency",       "Growth Agency",
                "A performance marketing agency with proven distribution experience.",
                "Marketing · Distribution · SaaS",
                "+ Distribution advantage\n+ GTM expertise",
                "- Product depth takes longer\n- Engineering may lag",
                "Easy"),

            new BackgroundOption("hardware_startup",    "Hardware Startup",
                "A hardware engineering team bringing a physical product background.",
                "Hardware · IoT · Engineering",
                "+ Deep technical credibility\n+ IP opportunities",
                "- Software scaling is slower\n- Supply chain complexity",
                "Very Hard"),

            new BackgroundOption("software_startup",    "Software Startup",
                "A focused software team ready to build and ship fast.",
                "Software · Agile · Product",
                "+ Fast iteration\n+ Lean operations",
                "- Competitive landscape is crowded\n- Early revenue harder",
                "Easy"),
        };

        // ─── Internal references ─────────────────────────────────────────────────────

        private readonly List<VisualElement> _cards = new List<VisualElement>();

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when a card is selected.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public BackgroundStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Company Background",
                "Choose the founding background that shaped your team and approach."));

            var grid = new VisualElement();
            grid.AddToClassList("company-creation__choice-grid");
            Root.Add(grid);

            foreach (BackgroundOption option in Options)
            {
                string capturedId = option.Id;
                string capturedTitle = option.Title;

                var card = BuildCard(option);
                card.RegisterCallback<ClickEvent>(_ =>
                {
                    SetSelection(capturedId);
                    OnFieldChanged?.Invoke("BackgroundId", capturedId);
                });

                _cards.Add(card);
                grid.Add(card);
            }
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            foreach (VisualElement card in _cards)
            {
                bool isSelected = card.userData is string id && id == vm.BackgroundId;
                SetCardSelected(card, isSelected);
            }
        }

        // ─── Private helpers ─────────────────────────────────────────────────────────

        private void SetSelection(string selectedId)
        {
            foreach (VisualElement card in _cards)
            {
                bool isSelected = card.userData is string id && id == selectedId;
                SetCardSelected(card, isSelected);
            }
        }

        private static void SetCardSelected(VisualElement card, bool selected)
        {
            if (selected) { card.AddToClassList("is-selected"); }
            else          { card.RemoveFromClassList("is-selected"); }
        }

        private static VisualElement BuildCard(BackgroundOption option)
        {
            var card = new VisualElement();
            card.AddToClassList("wizard-choice-card");
            card.userData = option.Id;

            var title = new Label(option.Title);
            title.AddToClassList("wizard-choice-card__title");
            card.Add(title);

            var desc = new Label(option.Description);
            desc.AddToClassList("wizard-choice-card__description");
            desc.AddToClassList("text-small");
            card.Add(desc);

            var tags = new Label(option.Tags);
            tags.AddToClassList("wizard-choice-card__tags");
            tags.AddToClassList("text-tiny");
            card.Add(tags);

            var strengths = new Label(option.Strengths);
            strengths.AddToClassList("wizard-choice-card__advantages");
            strengths.AddToClassList("text-small");
            card.Add(strengths);

            var risks = new Label(option.Risks);
            risks.AddToClassList("wizard-choice-card__tradeoffs");
            risks.AddToClassList("text-small");
            card.Add(risks);

            var difficulty = new Label($"Difficulty: {option.Difficulty}");
            difficulty.AddToClassList("text-tiny");
            card.Add(difficulty);

            return card;
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
