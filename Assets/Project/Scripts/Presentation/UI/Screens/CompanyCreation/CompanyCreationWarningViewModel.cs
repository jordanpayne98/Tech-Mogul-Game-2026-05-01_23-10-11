namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Display-data for a single warning chip in the summary panel.
    /// </summary>
    public sealed class CompanyCreationWarningViewModel
    {
        /// <summary>Warning message text.</summary>
        public string Message { get; }

        /// <summary>"warning" or "danger" — maps to USS variant class.</summary>
        public string Severity { get; }

        public CompanyCreationWarningViewModel(string message, string severity)
        {
            Message  = message  ?? string.Empty;
            Severity = severity ?? "warning";
        }
    }
}
