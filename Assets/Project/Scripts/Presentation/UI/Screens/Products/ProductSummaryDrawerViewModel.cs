namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Display data for the product quick-summary drawer shown when a product row is selected.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ProductsController and passed to ProductsView.
    /// [Placeholder] — Phase 5 uses static drawer data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductSummaryDrawerViewModel
    {
        // ── Identity ──────────────────────────────────────────────────────────

        /// <summary>Stable product ID of the product being summarised, e.g. "product.alpha_web_platform".</summary>
        public string ProductId { get; }

        // ── Summary fields ────────────────────────────────────────────────────

        /// <summary>Display name of the product.</summary>
        public string ProductName { get; }

        /// <summary>Product type label, e.g. "Web Platform" or "Peripheral".</summary>
        public string Type { get; }

        /// <summary>Product family label, e.g. "Software" or "Hardware".</summary>
        public string Family { get; }

        /// <summary>Current lifecycle status label, e.g. "In Development".</summary>
        public string Status { get; }

        /// <summary>Current development or post-launch phase label.</summary>
        public string Phase { get; }

        /// <summary>Display name of the team assigned to this product.</summary>
        public string AssignedTeam { get; }

        /// <summary>All-time aggregate review score formatted for display, e.g. "82" or "—".</summary>
        public string ReviewScore { get; }

        /// <summary>Revenue generated this month formatted for display, e.g. "$12,400" or "—".</summary>
        public string Revenue { get; }

        /// <summary>Support load level formatted for display, e.g. "Low", "Moderate", "Critical" or "—".</summary>
        public string SupportLoad { get; }

        // ── Semantic state and navigation ─────────────────────────────────────

        /// <summary>
        /// Semantic visual state token applied as a USS class suffix, e.g. "warning", "positive", "neutral",
        /// "negative", "muted". Must not be a raw colour value.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>
        /// Stable route ID used by the "View Full Detail" button, pointing to
        /// <see cref="Project.Application.ScreenIds.ProductDetail"/>.
        /// </summary>
        public string DetailRouteId { get; }

        public ProductSummaryDrawerViewModel(
            string productId,
            string productName,
            string type,
            string family,
            string status,
            string phase,
            string assignedTeam,
            string reviewScore,
            string revenue,
            string supportLoad,
            string semanticState,
            string detailRouteId)
        {
            ProductId    = productId;
            ProductName  = productName;
            Type         = type;
            Family       = family;
            Status       = status;
            Phase        = phase;
            AssignedTeam = assignedTeam;
            ReviewScore  = reviewScore;
            Revenue      = revenue;
            SupportLoad  = supportLoad;
            SemanticState = semanticState;
            DetailRouteId = detailRouteId;
        }
    }
}
