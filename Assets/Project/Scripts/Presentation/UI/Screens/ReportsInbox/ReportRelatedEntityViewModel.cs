namespace Project.Presentation.UI.Screens.ReportsInbox
{
    /// <summary>
    /// Pure display-data class for one related-entity link in the Reports Inbox detail panel.
    /// Immutable after construction. No Unity dependencies.
    /// Used to surface clickable entity references (e.g. a product, a candidate) within a report.
    /// [Placeholder] — Phase 5 uses static data. Live entity navigation wired in Phase 6+.
    /// </summary>
    public sealed class ReportRelatedEntityViewModel
    {
        /// <summary>Stable entity ID for the referenced entity.</summary>
        public string EntityId { get; }

        /// <summary>Display label for the entity link, e.g. "Product Alpha".</summary>
        public string Label { get; }

        /// <summary>Entity type string that describes the kind of entity, e.g. "product", "candidate", "contract".</summary>
        public string EntityType { get; }

        /// <summary>Route ID used to navigate to the entity's detail screen, e.g. "screen.products".</summary>
        public string DrillDownRouteId { get; }

        /// <summary>True when the link responds to click/tap input to trigger drill-down navigation.</summary>
        public bool IsClickable { get; }

        public ReportRelatedEntityViewModel(
            string entityId,
            string label,
            string entityType,
            string drillDownRouteId,
            bool isClickable)
        {
            EntityId         = entityId;
            Label            = label;
            EntityType       = entityType;
            DrillDownRouteId = drillDownRouteId;
            IsClickable      = isClickable;
        }
    }
}
