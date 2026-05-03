using Project.Core.Debugging;
using Project.Core.Definitions.Report;
using Project.Core.Results.Report;
using Project.Core.Runtime.Report;

namespace Project.Application.UseCases.Report
{
    /// <summary>
    /// Application use case for archiving a report.
    /// Idempotent — archiving an already-archived report succeeds without side effects.
    /// Cannot archive a deleted report.
    /// Auto-marks report as read when archiving an unread report.
    /// Decrements DecisionRequiredCount if the report required a decision.
    /// Defined in Plan 2L, GDD_18.
    /// </summary>
    public sealed class ArchiveReportUseCase
    {
        /// <summary>
        /// Archives the specified report in the given inbox.
        /// </summary>
        /// <param name="reportId">Stable ID of the report to archive.</param>
        /// <param name="inbox">The inbox state to mutate.</param>
        /// <returns>A result indicating success or the reason for failure.</returns>
        public ArchiveReportResult Execute(string reportId, InboxRuntimeState inbox)
        {
            if (string.IsNullOrEmpty(reportId))
            {
                return ArchiveReportResult.Failed("reportId is null or empty.");
            }

            if (inbox == null)
            {
                return ArchiveReportResult.Failed("inbox is null.");
            }

            if (inbox.ReportInboxStates == null || !inbox.ReportInboxStates.ContainsKey(reportId))
            {
                return ArchiveReportResult.Failed($"ReportId not found in inbox: {reportId}");
            }

            ReportInboxState currentState = inbox.ReportInboxStates[reportId];

            // Cannot archive a deleted report.
            if (currentState == ReportInboxState.Deleted)
            {
                return ArchiveReportResult.Failed($"Cannot archive a deleted report. ReportId: {reportId}");
            }

            // Idempotent — already archived, nothing to do.
            if (currentState == ReportInboxState.Archived)
            {
                return ArchiveReportResult.Succeeded();
            }

            inbox.ReportInboxStates[reportId] = ReportInboxState.Archived;

            // Auto-mark as read when archiving.
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
                $"[ArchiveReportUseCase] Report archived. ReportId: {reportId}");

            return ArchiveReportResult.Succeeded();
        }
    }
}
