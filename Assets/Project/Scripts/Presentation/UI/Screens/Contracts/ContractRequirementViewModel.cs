namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Display data for one contract requirement entry in the Contract Detail modal.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and passed to ContractDetailViewModel.
    /// SemanticState drives USS state class toggling in the View ("normal", "met", "unmet", "warning").
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractRequirementViewModel
    {
        /// <summary>Human-readable label for this requirement, e.g. "Minimum Quality Score".</summary>
        public string Label { get; }

        /// <summary>Display value for this requirement, e.g. "75" or "Senior Engineer x2".</summary>
        public string Value { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "met", "unmet", "warning".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when the player's current capabilities satisfy this requirement.</summary>
        public bool IsMet { get; }

        public ContractRequirementViewModel(
            string label,
            string value,
            string semanticState,
            bool isMet)
        {
            Label         = label;
            Value         = value;
            SemanticState = semanticState;
            IsMet         = isMet;
        }
    }
}
