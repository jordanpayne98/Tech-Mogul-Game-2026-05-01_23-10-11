using Project.Core.Definitions.Employee;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Mutable runtime state for a job post.
    /// Linked to the corresponding <see cref="JobPostProfile"/> via <see cref="JobPostId"/>.
    /// </summary>
    public sealed class JobPostRuntimeState
    {
        /// <summary>Stable ID matching the linked <see cref="JobPostProfile.Id"/>.</summary>
        public string JobPostId;

        /// <summary>Current lifecycle status of the job post.</summary>
        public JobPostStatus Status;
    }
}
