namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Display data for one customer preference entry in the market category detail drawer.
    /// Represents what a specific customer segment values in this market category.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state classes applied by the View (e.g. "aligned", "misaligned", "neutral").
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    ///
    /// Supported customer segments: Consumers, Small businesses, Enterprises, Developers,
    /// Gamers, Creators, Education, Government, Hardware enthusiasts, Budget buyers, Premium buyers.
    /// </summary>
    public sealed class CustomerPreferenceViewModel
    {
        /// <summary>Display name of the customer segment (e.g. "Enterprises", "Gamers").</summary>
        public string SegmentName { get; }

        /// <summary>Human-readable description of what this segment values in this category.</summary>
        public string PreferenceText { get; }

        /// <summary>
        /// Semantic visual state string used to apply USS state classes on the View.
        /// Examples: "aligned", "misaligned", "neutral".
        /// Must not be a raw colour or pixel value.
        /// </summary>
        public string SemanticState { get; }

        public CustomerPreferenceViewModel(
            string segmentName,
            string preferenceText,
            string semanticState)
        {
            SegmentName    = segmentName;
            PreferenceText = preferenceText;
            SemanticState  = semanticState;
        }
    }
}
