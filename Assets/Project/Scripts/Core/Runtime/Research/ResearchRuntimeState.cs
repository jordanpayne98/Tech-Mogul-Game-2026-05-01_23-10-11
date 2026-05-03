using System.Collections.Generic;

namespace Project.Core.Runtime.Research
{
    /// <summary>
    /// Mutable runtime state representing the full research portfolio for a company.
    /// Tracks which projects are available, active, completed, and obsolete,
    /// as well as which capabilities have been unlocked.
    /// </summary>
    public sealed class ResearchRuntimeState
    {
        /// <summary>Stable ID of the company this research state belongs to.</summary>
        public string CompanyId;

        /// <summary>
        /// Stable IDs of research projects the company can currently start.
        /// Prerequisites are met; projects are not yet InProgress or Completed.
        /// </summary>
        public List<string> AvailableProjectIds;

        /// <summary>
        /// Stable IDs of research projects currently InProgress.
        /// Each has a corresponding <see cref="ResearchProjectRuntimeState"/>.
        /// </summary>
        public List<string> ActiveProjectIds;

        /// <summary>
        /// Stable IDs of research projects that have reached Completed status.
        /// Their unlocks have been granted.
        /// </summary>
        public List<string> CompletedProjectIds;

        /// <summary>
        /// Stable IDs of research projects that have reached Obsolete status.
        /// These were superseded before or after completion.
        /// </summary>
        public List<string> ObsoleteProjectIds;

        /// <summary>
        /// Stable IDs of capabilities that have been unlocked by completed research projects.
        /// Used by other systems to gate features (e.g. "product_type.smartphone").
        /// </summary>
        public List<string> UnlockedCapabilityIds;
    }
}
