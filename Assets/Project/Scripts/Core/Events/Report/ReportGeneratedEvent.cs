using Project.Core.Definitions.Report;

namespace Project.Core.Events.Report
{
    /// <summary>
    /// Published after ReportService successfully creates a report and updates inbox state.
    /// Consumers may use this to update UI badge counts or surface notifications.
    /// </summary>
    public sealed class ReportGeneratedEvent
    {
        /// <summary>Stable ID of the newly generated report.</summary>
        public string ReportId { get; }

        /// <summary>The specific type of report that was generated.</summary>
        public ReportType Type { get; }

        /// <summary>The urgency level of the generated report.</summary>
        public ReportPriority Priority { get; }

        public ReportGeneratedEvent(string reportId, ReportType type, ReportPriority priority)
        {
            ReportId = reportId;
            Type     = type;
            Priority = priority;
        }
    }
}
