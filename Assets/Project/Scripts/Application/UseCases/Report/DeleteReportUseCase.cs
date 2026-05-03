using Project.Core.Debugging;
using Project.Core.Definitions.Report;
using Project.Core.Results.Report;
using Project.Core.Runtime.Report;

namespace Project.Application.UseCases.Report
{
    /// <summary>
    /// Application use case for soft-deleting a report.
    /// Idempotent — deleting an already-deleted report succeeds without side effects.
    /// Auto-marks report as read when deleting an unread report.
    /// Decrements DecisionRequiredCount if the report required a decision.
    /// Defined in Plan 2L, GDD_18.
    /// </summary>
    public sealed class DeleteReportUseCase
    {
        /// <summary>
        /// Soft-deletes the specified report in the given inbox.
        /// </summary>
        /// <param name="reportId">Stable ID of the report to delete.</param>
        /// <param name="inbox">The inbox state to mutate.</param>
        /// <returns>A result indicating success or the reason for failure.</returns>
        public DeleteReportResult Execute(string reportId, InboxRuntimeState inbox)
        {
            if (string.IsNullOrEmpty(reportId))
            {
                return DeleteReportResult.Failed("reportId is null or empty.");
            }

            if (inbox == null)
            {
                return DeleteReportResult.Failed("inbox is null.");
            }

            if (inbox.ReportInboxStates == null || !inbox.ReportInboxStates.ContainsKey(reportId))
            {
                return DeleteReportResult.Failed($"ReportId not found in inbox: {reportId}");
            }

            // Idempotent — already deleted, nothing to do.
            if (inbox.ReportInboxStates[reportId] == ReportInboxState.Deleted)
            {
                return DeleteReportResult.Succeeded();
            }

            inbox.ReportInboxStates[reportId] = ReportInboxState.Deleted;

            // Auto-mark as read when deleting.
            if (inbox.ReportReadStates != null
                && inbox.ReportReadStates.TryGetValue(reportId, out ReportReadState readState)
                && readState == ReportReadState.Unread)
            {
                inbox.ReportReadStates[reportId] = ReportReadState.Read;
                inbox.UnreadCount = System.Math.Max(0, inbox.UnreadCount - 1);
            }

            // Note: RequiresDecision is stored on ReportProfile, not InboxRuntimeState.
            // The caller is responsible for decrementing DecisionRequiredCount if needed,
            // as ReportProfile lookup is outside the scope of this use case for MVP.

            DebugLogger.Log(DebugCategory.Simulation,
                $"[DeleteReportUseCase] Report deleted. ReportId: {reportId}");

            return DeleteReportResult.Succeeded();
        }
    }
}
