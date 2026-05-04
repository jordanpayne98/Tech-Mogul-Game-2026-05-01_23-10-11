namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Display data for a product status chip rendered inside a product row or summary drawer.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static chip data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductStatusChipViewModel
    {
        // ── Display ───────────────────────────────────────────────────────────

        /// <summary>Human-readable label shown inside the chip, e.g. "In Development".</summary>
        public string Label { get; }

        /// <summary>
        /// Semantic visual state token applied as a USS class suffix, e.g. "warning", "positive", "neutral",
        /// "negative". Must not be a raw colour value.
        /// </summary>
        public string SemanticState { get; }

        // ── Navigation ────────────────────────────────────────────────────────

        /// <summary>
        /// Stable route ID the chip navigates to when clicked, typically <see cref="Project.Application.ScreenIds.ProductDetail"/>.
        /// Empty string when the chip is not clickable.
        /// </summary>
        public string DrillDownRouteId { get; }

        /// <summary>True when the chip responds to pointer interaction and navigates on click.</summary>
        public bool IsClickable { get; }

        public ProductStatusChipViewModel(
            string label,
            string semanticState,
            string drillDownRouteId,
            bool isClickable)
        {
            Label            = label;
            SemanticState    = semanticState;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
