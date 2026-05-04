namespace Project.Presentation.UI.Screens.ProductDetail
{
    /// <summary>
    /// Pure display-data class for one row in the History tab of the Product Detail screen.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "danger", "success".
    /// [Placeholder] — static data only in Phase 5.
    /// </summary>
    public sealed class ProductHistoryRowViewModel
    {
        /// <summary>Pre-formatted date string, e.g. "Month 4, Year 1".</summary>
        public string Date { get; }

        /// <summary>Human-readable event description, e.g. "Development phase started".</summary>
        public string Event { get; }

        /// <summary>Category label, e.g. "Lifecycle", "Quality", "Budget".</summary>
        public string Category { get; }

        /// <summary>Semantic state string: "normal", "warning", "danger", or "success".</summary>
        public string SemanticState { get; }

        public ProductHistoryRowViewModel(
            string date,
            string @event,
            string category,
            string semanticState)
        {
            Date          = date;
            Event         = @event;
            Category      = category;
            SemanticState = semanticState;
        }
    }
}
