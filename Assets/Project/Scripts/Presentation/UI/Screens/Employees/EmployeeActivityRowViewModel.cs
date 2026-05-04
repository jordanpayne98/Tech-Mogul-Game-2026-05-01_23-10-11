namespace Project.Presentation.UI.Screens.Employees
{
    /// <summary>
    /// Pure display-data class for a single row in the employee activity history list
    /// inside the employee profile modal.
    /// Immutable after construction. No Unity dependencies.
    /// SemanticState drives USS state classes in the View: "normal", "warning", "positive".
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class EmployeeActivityRowViewModel
    {
        /// <summary>Pre-formatted date string, e.g. "Mar 2024".</summary>
        public string Date { get; }

        /// <summary>Activity description text, e.g. "Promoted to Senior Engineer".</summary>
        public string Description { get; }

        /// <summary>Category label, e.g. "Promotion", "Assignment", "Warning".</summary>
        public string Category { get; }

        /// <summary>Semantic state string: "normal", "warning", or "positive". Drives USS state class.</summary>
        public string SemanticState { get; }

        public EmployeeActivityRowViewModel(
            string date,
            string description,
            string category,
            string semanticState)
        {
            Date          = date;
            Description   = description;
            Category      = category;
            SemanticState = semanticState;
        }
    }
}
