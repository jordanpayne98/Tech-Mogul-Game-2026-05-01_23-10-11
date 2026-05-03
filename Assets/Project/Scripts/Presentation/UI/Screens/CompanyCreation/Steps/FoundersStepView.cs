using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation.Steps
{
    /// <summary>
    /// Step 3 — Founder Setup.
    /// Renders two large choice cards: Solo Founder and Co-Founders,
    /// each showing advantages and trade-offs.
    /// </summary>
    public sealed class FoundersStepView
    {
        // ─── Internal references ─────────────────────────────────────────────────────

        private readonly List<VisualElement> _cards = new List<VisualElement>();

        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        /// <summary>Callback(fieldName, value) fired when a card is selected.</summary>
        public Action<string, string> OnFieldChanged { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public FoundersStepView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation-step");

            Root.Add(BuildStepHeading("Founder Setup",
                "Choose how many founders are starting this company."));

            var cardRow = new VisualElement();
            cardRow.AddToClassList("company-creation__choice-grid");
            Root.Add(cardRow);

            // Solo Founder
            var soloCard = BuildFounderCard(
                id:          "solo",
                title:       "Solo Founder",
                description: "You are the sole founder driving the company.",
                advantages:  new[] { "Full control and decision-making speed", "Simpler equity structure", "Lean operation from day one" },
                tradeoffs:   new[] { "No co-founder to share the load", "Broader skill gaps to fill", "Risk concentrated on one person" });

            soloCard.RegisterCallback<ClickEvent>(_ =>
            {
                SetSelection("solo");
                OnFieldChanged?.Invoke("FounderSetupType", "solo");
            });

            _cards.Add(soloCard);
            cardRow.Add(soloCard);

            // Co-Founders
            var coCard = BuildFounderCard(
                id:          "co-founders",
                title:       "Co-Founders",
                description: "Two founders working together to build the company.",
                advantages:  new[] { "Complementary skills", "Shared resilience under pressure", "More credibility with investors" },
                tradeoffs:   new[] { "Equity split required", "Decision-making alignment takes effort", "Potential for co-founder conflict" });

            coCard.RegisterCallback<ClickEvent>(_ =>
            {
                SetSelection("co-founders");
                OnFieldChanged?.Invoke("FounderSetupType", "co-founders");
            });

            _cards.Add(coCard);
            cardRow.Add(coCard);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        public void Bind(CompanyCreationViewModel vm)
        {
            foreach (VisualElement card in _cards)
            {
                bool isSelected = card.userData is string id && id == vm.FounderSetupType;
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

        private static VisualElement BuildFounderCard(string id, string title, string description,
            string[] advantages, string[] tradeoffs)
        {
            var card = new VisualElement();
            card.AddToClassList("wizard-choice-card");
            card.userData = id;

            var titleLabel = new Label(title);
            titleLabel.AddToClassList("wizard-choice-card__title");
            card.Add(titleLabel);

            var descLabel = new Label(description);
            descLabel.AddToClassList("wizard-choice-card__description");
            descLabel.AddToClassList("text-small");
            card.Add(descLabel);

            // Advantages
            var advantagesEl = new VisualElement();
            advantagesEl.AddToClassList("wizard-choice-card__advantages");
            foreach (string adv in advantages)
            {
                var l = new Label($"+ {adv}");
                l.AddToClassList("text-small");
                advantagesEl.Add(l);
            }

            card.Add(advantagesEl);

            // Trade-offs
            var tradeoffsEl = new VisualElement();
            tradeoffsEl.AddToClassList("wizard-choice-card__tradeoffs");
            foreach (string tradeoff in tradeoffs)
            {
                var l = new Label($"- {tradeoff}");
                l.AddToClassList("text-small");
                tradeoffsEl.Add(l);
            }

            card.Add(tradeoffsEl);

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
