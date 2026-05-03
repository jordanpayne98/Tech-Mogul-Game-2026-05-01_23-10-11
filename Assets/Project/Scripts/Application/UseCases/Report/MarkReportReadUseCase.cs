using Project.Core.Debugging;
using Project.Core.Definitions.Report;
using Project.Core.Results.Report;
using Project.Core.Runtime.Report;

namespace Project.Application.UseCases.Report
{
    /// <summary>
    /// Application use case for marking a report as read.
    /// Idempotent — marking an already-read report succeeds without side effects.
    /// Decrements InboxRuntimeState.UnreadCount when transitioning Unread -> Read.
    /// Defined in Plan 2L, GDD_18.
    /// </summary>
    public sealed class MarkReportReadUseCase
    {
        /// <summary>
        /// Marks the specified report as read in the given inbox.
        /// </summary>
        /// <param name="reportId">Stable ID of the report to mark as read.</param>
        /// <param name="inbox">The inbox state to mutate.</param>
        /// <returns>A result indicating success or the reason for failure.</returns>
        public MarkReportReadResult Execute(string reportId, InboxRuntimeState inbox)
        {
            if (string.IsNullOrEmpty(reportId))
            {
                return MarkReportReadResult.Failed("reportId is null or empty.");
            }

            if (inbox == null)
            {
                return MarkReportReadResult.Failed("inbox is null.");
            }

            if (inbox.ReportReadStates == null || !inbox.ReportReadStates.ContainsKey(reportId))
            {
                return MarkReportReadResult.Failed($"ReportId not found in inbox: {reportId}");
            }

            // Idempotent — already read, nothing to do.
            if (inbox.ReportReadStates[reportId] == ReportReadState.Read)
            {
                return MarkReportReadResult.Succeeded();
            }

            inbox.ReportReadStates[reportId] = ReportReadState.Read;
            inbox.UnreadCount = System.Math.Max(0, inbox.UnreadCount - 1);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[MarkReportReadUseCase] Report marked as read. ReportId: {reportId}. UnreadCount: {inbox.UnreadCount}");

            return MarkReportReadResult.Succeeded();
        }
    }
}
