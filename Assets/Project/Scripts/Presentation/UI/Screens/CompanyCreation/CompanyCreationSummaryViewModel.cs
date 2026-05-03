using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Display-data for the right-side summary panel.
    /// </summary>
    public sealed class CompanyCreationSummaryViewModel
    {
        /// <summary>"Company Summary" or "Founder Summary" depending on active step context.</summary>
        public string Title { get; }

        /// <summary>Ordered list of label/value rows to render.</summary>
        public List<CompanyCreationSummaryRowViewModel> Rows { get; }

        /// <summary>Ordered list of warning chips to render below the rows.</summary>
        public List<CompanyCreationWarningViewModel> Warnings { get; }

        /// <summary>False when no selections have been made yet — triggers empty state text.</summary>
        public bool HasSelections { get; }

        public CompanyCreationSummaryViewModel(
            string                                      title,
            List<CompanyCreationSummaryRowViewModel>    rows,
            List<CompanyCreationWarningViewModel>       warnings,
            bool                                        hasSelections)
        {
            Title         = title         ?? string.Empty;
            Rows          = rows          ?? new List<CompanyCreationSummaryRowViewModel>();
            Warnings      = warnings      ?? new List<CompanyCreationWarningViewModel>();
            HasSelections = hasSelections;
        }
    }
}
