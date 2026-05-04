namespace Project.Presentation.UI.Screens.Market
{
    /// <summary>
    /// Display data for one trend strip item shown at the top of the Market screen.
    /// Immutable after construction. No Unity dependencies.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state classes applied by the View (e.g. "positive", "negative", "neutral").
    /// DrillDownRouteId is empty when no linked report or detail screen exists.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class MarketTrendViewModel
    {
        /// <summary>Stable ID for this trend item (e.g. "trend.ai_adoption_wave").</summary>
        public string Id { get; }

        /// <summary>Short display title for the trend (e.g. "AI Adoption Accelerating").</summary>
        public string Title { get; }

        /// <summary>Brief summary sentence describing the trend impact.</summary>
        public string Summary { get; }

        /// <summary>
        /// Semantic visual state string used to apply USS state classes on the View.
        /// Examples: "positive", "negative", "neutral", "warning".
        /// Must not be a raw colour or pixel value.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>
        /// Stable route ID to open a related report or detail screen when clicked.
        /// Empty string when no drill-down target exists.
        /// </summary>
        public string DrillDownRouteId { get; }

        /// <summary>True when this trend item can be clicked to navigate to a detail view.</summary>
        public bool IsClickable { get; }

        public MarketTrendViewModel(
            string id,
            string title,
            string summary,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Id               = id;
            Title            = title;
            Summary          = summary;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
