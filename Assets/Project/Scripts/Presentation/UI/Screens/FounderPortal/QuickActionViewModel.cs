namespace Project.Presentation.UI.Screens.FounderPortal
{
    /// <summary>
    /// Quick action IDs used in the Founder Portal quick action strip.
    /// </summary>
    public static class QuickActionIds
    {
        public const string NewProduct      = "action.new_product";
        public const string HireEmployee    = "action.hire_employee";
        public const string CreateContract  = "action.create_contract";
        public const string ViewReports     = "action.view_reports";
        public const string MarketOverview  = "action.market_overview";
        public const string Settings        = "action.settings";
    }

    /// <summary>
    /// Pure display-data class for one quick action button in the Founder Portal.
    /// Immutable after construction. No Unity dependencies.
    /// </summary>
    public sealed class QuickActionViewModel
    {
        /// <summary>Stable ID, e.g. "action.new_product".</summary>
        public string Id { get; }

        /// <summary>Button label, e.g. "New Product".</summary>
        public string Label { get; }

        /// <summary>USS icon class applied to the icon element, e.g. "icon-product".</summary>
        public string IconClass { get; }

        /// <summary>Target screen or modal route ID to open on activation.</summary>
        public string TargetRouteId { get; }

        /// <summary>Whether the quick action button is interactable.</summary>
        public bool IsEnabled { get; }

        public QuickActionViewModel(
            string id,
            string label,
            string iconClass,
            string targetRouteId,
            bool isEnabled)
        {
            Id            = id;
            Label         = label;
            IconClass     = iconClass;
            TargetRouteId = targetRouteId;
            IsEnabled     = isEnabled;
        }
    }
}
