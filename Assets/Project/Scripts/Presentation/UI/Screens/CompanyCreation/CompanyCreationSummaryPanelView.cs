using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Builds and updates the right-side summary panel content.
    /// Renders summary rows, warning chips, and an empty state when nothing is selected yet.
    /// </summary>
    public sealed class CompanyCreationSummaryPanelView
    {
        // ─── Public API ──────────────────────────────────────────────────────────────

        public VisualElement Root { get; }

        // ─── Private elements ────────────────────────────────────────────────────────

        private readonly Label         _titleLabel;
        private readonly VisualElement _rowsContainer;
        private readonly VisualElement _warningsContainer;
        private readonly Label         _emptyStateLabel;

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public CompanyCreationSummaryPanelView()
        {
            Root = new VisualElement();
            Root.AddToClassList("company-creation__summary-content");

            _titleLabel = new Label("Company Summary");
            _titleLabel.AddToClassList("text-heading");
            Root.Add(_titleLabel);

            _rowsContainer = new VisualElement();
            _rowsContainer.AddToClassList("company-creation__summary-rows");
            Root.Add(_rowsContainer);

            _warningsContainer = new VisualElement();
            _warningsContainer.AddToClassList("company-creation__summary-warnings");
            Root.Add(_warningsContainer);

            _emptyStateLabel = new Label("Select options to build your company summary.");
            _emptyStateLabel.AddToClassList("text-small");
            Root.Add(_emptyStateLabel);
        }

        // ─── Bind ────────────────────────────────────────────────────────────────────

        /// <summary>Rebuilds the summary rows, warnings, and empty state from the ViewModel.</summary>
        public void Bind(CompanyCreationSummaryViewModel vm)
        {
            if (vm == null) { return; }

            _titleLabel.text = vm.Title;

            // ── Rows ──────────────────────────────────────────────────────────────────
            _rowsContainer.Clear();

            foreach (CompanyCreationSummaryRowViewModel row in vm.Rows)
            {
                var rowEl = new VisualElement();
                rowEl.AddToClassList("company-creation__summary-row");

                var labelEl = new Label(row.Label);
                labelEl.AddToClassList("company-creation__summary-label");
                rowEl.Add(labelEl);

                var valueEl = new Label(row.Value);
                valueEl.AddToClassList("company-creation__summary-value");

                if (!row.IsSet)
                {
                    valueEl.AddToClassList("is-unset");
                }

                rowEl.Add(valueEl);
                _rowsContainer.Add(rowEl);
            }

            // ── Warnings ──────────────────────────────────────────────────────────────
            _warningsContainer.Clear();

            foreach (CompanyCreationWarningViewModel warning in vm.Warnings)
            {
                var chip = new Label(warning.Message);
                chip.AddToClassList("company-creation__warning-row");

                string severityClass = warning.Severity == "danger"
                    ? "wizard-validation--error"
                    : "wizard-validation--warning";

                chip.AddToClassList(severityClass);
                _warningsContainer.Add(chip);
            }

            // ── Empty state ───────────────────────────────────────────────────────────
            _emptyStateLabel.style.display = vm.HasSelections ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
