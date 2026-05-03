using Project.Core.Definitions.Report;
using Project.Core.Runtime.Time;

namespace Project.Core.Requests.Report
{
    /// <summary>
    /// Request to generate and deliver a report to the player inbox.
    /// RelatedEntityId and RelatedEntityType are nullable — omit when the report
    /// has no specific entity reference (e.g. a system-level monthly summary).
    /// </summary>
    public sealed class GenerateReportRequest
    {
        /// <summary>The specific type of report to generate.</summary>
        public ReportType Type;

        /// <summary>The simulation domain category for the generated report.</summary>
        public ReportCategory Category;

        /// <summary>The urgency priority for the generated report.</summary>
        public ReportPriority Priority;

        /// <summary>Stable ID of the primary related entity. Null if not applicable.</summary>
        public string RelatedEntityId;

        /// <summary>
        /// Type label of the primary related entity (e.g. "product", "employee").
        /// Null if RelatedEntityId is null.
        /// </summary>
        public string RelatedEntityType;

        /// <summary>The in-game date on which this report is being generated.</summary>
        public GameDateTime Date;
    }
}
