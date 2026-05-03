using System.Collections.Generic;

namespace Project.Core.Events.Research
{
    /// <summary>
    /// Published when a research project reaches Completed status.
    /// Consumed by ReportEventHandler to generate a research completion report.
    /// </summary>
    public sealed class ResearchCompletedEvent
    {
        /// <summary>Stable ID of the research project that was completed.</summary>
        public string ProjectId { get; }

        /// <summary>Stable IDs of capabilities unlocked by this research completion.</summary>
        public List<string> UnlockedCapabilityIds { get; }

        public ResearchCompletedEvent(string projectId, List<string> unlockedCapabilityIds)
        {
            ProjectId              = projectId;
            UnlockedCapabilityIds  = unlockedCapabilityIds ?? new List<string>();
        }
    }
}
