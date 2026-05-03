using System.Collections.Generic;
using Project.Core.Definitions.Report;

namespace Project.Core.Runtime.Report
{
    /// <summary>
    /// Mutable runtime state tracking the player's inbox for a specific company save.
    /// Stores ordered report IDs and per-report read/inbox states keyed by report ID.
    /// ReportProfile data (content, metadata) is stored separately and looked up by ID.
    /// </summary>
    public sealed class InboxRuntimeState
    {
        /// <summary>Stable ID of the company this inbox belongs to.</summary>
        public string CompanyId;

        /// <summary>
        /// Ordered list of report IDs in this inbox.
        /// Ordering reflects delivery sequence (newest last or sorted by caller).
        /// </summary>
        public List<string> ReportIds;

        /// <summary>
        /// Per-report read state, keyed by report ID.
        /// All reports in ReportIds must have a corresponding entry here.
        /// </summary>
        public Dictionary<string, ReportReadState> ReportReadStates;

        /// <summary>
        /// Per-report inbox lifecycle state, keyed by report ID.
        /// All reports in ReportIds must have a corresponding entry here.
        /// </summary>
        public Dictionary<string, ReportInboxState> ReportInboxStates;

        /// <summary>
        /// Cached count of reports with ReportReadState.Unread.
        /// Must be kept in sync whenever ReportReadStates is updated.
        /// </summary>
        public int UnreadCount;

        /// <summary>
        /// Cached count of active reports with RequiresDecision == true.
        /// Must be kept in sync whenever reports are added or resolved.
        /// </summary>
        public int DecisionRequiredCount;
    }
}
