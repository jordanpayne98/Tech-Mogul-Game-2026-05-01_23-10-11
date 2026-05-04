namespace Project.Presentation.UI.Screens.Products
{
    /// <summary>
    /// Display data for one row in the Products portfolio table.
    /// Immutable after construction. No Unity dependencies.
    /// All string fields are pre-formatted and display-ready.
    /// [Placeholder] — Phase 5 uses static row data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ProductRowViewModel
    {
        // ── Identity ──────────────────────────────────────────────────────────

        /// <summary>Stable product ID, e.g. "product.alpha_web_platform".</summary>
        public string Id { get; }

        // ── Core columns ──────────────────────────────────────────────────────

        /// <summary>Display name of the product.</summary>
        public string ProductName { get; }

        /// <summary>Product family label, e.g. "Software" or "Hardware".</summary>
        public string Family { get; }

        /// <summary>
        /// Product type label within the family.
        /// Software: Web Platform, Productivity App, Business SaaS, Developer Tool, Game.
        /// Hardware: Peripheral, Laptop/Desktop Device, Server Device.
        /// </summary>
        public string Type { get; }

        /// <summary>Current lifecycle status label, e.g. "In Development", "Launched", "Sunset".</summary>
        public string Status { get; }

        /// <summary>Display name of the team assigned to this product.</summary>
        public string AssignedTeam { get; }

        /// <summary>Current development or post-launch phase label, e.g. "Pre-Alpha", "Beta", "v1.2".</summary>
        public string Phase { get; }

        /// <summary>Formatted release target, e.g. "Q3 Y2" or "—" when not set.</summary>
        public string ReleaseTarget { get; }

        // ── Scores and metrics ────────────────────────────────────────────────

        /// <summary>All-time aggregate review score formatted for display, e.g. "82" or "—".</summary>
        public string ReviewScore { get; }

        /// <summary>Recent review score (last window) formatted for display, e.g. "74" or "—".</summary>
        public string RecentReviewScore { get; }

        /// <summary>Revenue generated this month, formatted for display, e.g. "$12,400" or "—".</summary>
        public string RevenueThisMonth { get; }

        /// <summary>Active users (software) or units sold (hardware) formatted for display, e.g. "3,200" or "—".</summary>
        public string ActiveUsersOrUnits { get; }

        /// <summary>Support load level formatted for display, e.g. "Low", "Moderate", "Critical" or "—".</summary>
        public string SupportLoad { get; }

        // ── Semantic state and navigation ─────────────────────────────────────

        /// <summary>
        /// Semantic visual state token applied as a USS class suffix, e.g. "warning", "positive", "neutral",
        /// "negative", "muted". Must not be a raw colour value.
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when the row responds to pointer interaction.</summary>
        public bool IsClickable { get; }

        /// <summary>
        /// Stable route ID the row navigates to on click, typically <see cref="Project.Application.ScreenIds.ProductDetail"/>.
        /// Empty string when the row is not clickable.
        /// </summary>
        public string DrillDownRouteId { get; }

        public ProductRowViewModel(
            string id,
            string productName,
            string family,
            string type,
            string status,
            string assignedTeam,
            string phase,
            string releaseTarget,
            string reviewScore,
            string recentReviewScore,
            string revenueThisMonth,
            string activeUsersOrUnits,
            string supportLoad,
            string semanticState,
            bool isClickable,
            string drillDownRouteId)
        {
            Id                 = id;
            ProductName        = productName;
            Family             = family;
            Type               = type;
            Status             = status;
            AssignedTeam       = assignedTeam;
            Phase              = phase;
            ReleaseTarget      = releaseTarget;
            ReviewScore        = reviewScore;
            RecentReviewScore  = recentReviewScore;
            RevenueThisMonth   = revenueThisMonth;
            ActiveUsersOrUnits = activeUsersOrUnits;
            SupportLoad        = supportLoad;
            SemanticState      = semanticState;
            IsClickable        = isClickable;
            DrillDownRouteId   = drillDownRouteId;
        }
    }
}
