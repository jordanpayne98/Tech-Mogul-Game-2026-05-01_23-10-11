namespace Project.Core.Runtime.Report
{
    /// <summary>
    /// A lightweight value type linking a report to a related simulation entity.
    /// Used to surface references (e.g. a product, employee, or team) within a ReportProfile.
    /// </summary>
    public struct ReportEntityReference
    {
        /// <summary>Stable ID of the related entity.</summary>
        public string EntityId;

        /// <summary>
        /// Type label of the related entity (e.g. "product", "employee", "team").
        /// Used for routing and display; not an enum to remain extensible across GDD phases.
        /// </summary>
        public string EntityType;
    }
}
