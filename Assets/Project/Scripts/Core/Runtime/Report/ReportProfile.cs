using System.Collections.Generic;
using Project.Core.Definitions.Report;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Report
{
    /// <summary>
    /// Immutable per-save data describing a generated report delivered to the player inbox.
    /// ReportProfile is the static record of what the report contains.
    /// Mutable inbox state (read state, archive state) is tracked separately in InboxRuntimeState.
    /// </summary>
    public sealed class ReportProfile
    {
        /// <summary>Stable unique ID for this report instance.</summary>
        public string Id;

        /// <summary>Specific report type identifying the simulation event that triggered this report.</summary>
        public ReportType Type;

        /// <summary>Simulation domain this report belongs to.</summary>
        public ReportCategory Category;

        /// <summary>Urgency level used for inbox sorting and player attention routing.</summary>
        public ReportPriority Priority;

        /// <summary>Short display title shown in inbox list views.</summary>
        public string Title;

        /// <summary>Longer narrative summary of the report content shown in the detail view.</summary>
        public string Summary;

        /// <summary>The in-game date when this report was generated.</summary>
        public GameDateTime Date;

        /// <summary>
        /// Simulation entities related to this report (e.g. the product, employee, or team referenced).
        /// May be empty if the report has no specific entity references.
        /// </summary>
        public List<ReportEntityReference> RelatedEntities;

        /// <summary>
        /// Structured key/value data points embedded in the report (e.g. metrics, stats).
        /// May be empty if the report has no quantitative data to surface.
        /// </summary>
        public List<ReportKeyValue> KeyValues;

        /// <summary>
        /// True if this report requires the player to choose an action before it can be dismissed.
        /// When true, AvailableActionIds must contain at least one entry.
        /// </summary>
        public bool RequiresDecision;

        /// <summary>
        /// Stable IDs of actions the player may take in response to this report.
        /// Empty when RequiresDecision is false.
        /// </summary>
        public List<string> AvailableActionIds;
    }
}
