using Project.Core.Definitions.Research;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Research
{
    /// <summary>
    /// Mutable runtime state for a single research project in progress.
    /// One instance exists per project the company has interacted with.
    /// References <see cref="Project.Core.Definitions.Research.ResearchProjectDefinition"/> by stable ID.
    /// </summary>
    public sealed class ResearchProjectRuntimeState
    {
        /// <summary>
        /// Stable ID of the <see cref="Project.Core.Definitions.Research.ResearchProjectDefinition"/> this state tracks.
        /// </summary>
        public string ProjectId;

        /// <summary>Current lifecycle status of the project.</summary>
        public ResearchProjectStatus Status;

        /// <summary>
        /// Completion progress as a percentage in the range 0–100.
        /// Only meaningful when <see cref="Status"/> is <see cref="ResearchProjectStatus.InProgress"/>.
        /// </summary>
        public int ProgressPercent;

        /// <summary>
        /// Stable ID of the team currently assigned to this project.
        /// Null when no team is assigned.
        /// </summary>
        public string AssignedTeamId;

        /// <summary>
        /// The game date on which work began.
        /// Null when the project has not yet started.
        /// </summary>
        public GameDateTime StartDate;

        /// <summary>
        /// The game date on which the project reached Completed or Obsolete status.
        /// Null while still in progress.
        /// </summary>
        public GameDateTime CompletedDate;

        /// <summary>
        /// Projected game date of completion based on current team and progress.
        /// Null when not started or when estimation is unavailable.
        /// </summary>
        public GameDateTime EstimatedCompletionDate;
    }
}
