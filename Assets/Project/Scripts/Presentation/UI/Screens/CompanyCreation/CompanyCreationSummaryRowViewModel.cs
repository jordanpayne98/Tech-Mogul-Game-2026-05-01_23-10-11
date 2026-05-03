namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Display-data for a single label/value row in the summary panel.
    /// </summary>
    public sealed class CompanyCreationSummaryRowViewModel
    {
        /// <summary>Field label displayed on the left.</summary>
        public string Label { get; }

        /// <summary>Current value displayed on the right.</summary>
        public string Value { get; }

        /// <summary>True when Value is a real selection; false when it is a placeholder like "Not set".</summary>
        public bool IsSet { get; }

        public CompanyCreationSummaryRowViewModel(string label, string value, bool isSet)
        {
            Label = label ?? string.Empty;
            Value = value ?? string.Empty;
            IsSet = isSet;
        }
    }
}
