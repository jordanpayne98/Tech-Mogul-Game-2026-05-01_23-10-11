using Project.Core.Debugging;
using Project.Core.Definitions.Employee;
using Project.Core.Interfaces;
using Project.Core.Requests.Employee;
using Project.Core.Results.Employee;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;

namespace Project.Application.UseCases.Employee
{
    /// <summary>
    /// Application-layer use case for creating a new job post.
    /// Job posts are stored data only — no active candidate filtering occurs in MVP.
    /// Candidate matching is deferred to a future plan.
    /// </summary>
    public sealed class PostJobUseCase
    {
        private readonly IEventBus _eventBus;

        public PostJobUseCase(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// Validates the request, creates the job post, and registers it in session state.
        /// </summary>
        /// <param name="request">The job post creation request.</param>
        /// <param name="sessionState">Active session state to update.</param>
        /// <param name="nextJobPostCounter">Counter used to generate the stable job post ID.</param>
        public PostJobResult Execute(PostJobRequest request, GameSessionState sessionState, int nextJobPostCounter)
        {
            if (request == null)
            {
                return PostJobResult.Failed("Request is null.");
            }

            if (request.SalaryRangeMinMinorUnits <= 0)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    "[PostJobUseCase] Failed: salary range minimum must be positive.");
                return PostJobResult.Failed("Salary range minimum must be positive.");
            }

            if (request.SalaryRangeMinMinorUnits > request.SalaryRangeMaxMinorUnits)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    "[PostJobUseCase] Failed: salary range minimum is greater than maximum.");
                return PostJobResult.Failed("Salary range minimum must not exceed the maximum.");
            }

            string jobPostId = $"jobpost.{nextJobPostCounter}";

            var profile = new JobPostProfile
            {
                Id                       = jobPostId,
                Role                     = request.Role,
                Seniority                = request.Seniority,
                SalaryRangeMinMinorUnits = request.SalaryRangeMinMinorUnits,
                SalaryRangeMaxMinorUnits = request.SalaryRangeMaxMinorUnits,
                RequiredSkills           = request.RequiredSkills,
                PreferredSkills          = request.PreferredSkills,
                WorkPreference           = request.WorkPreference,
                CompanyPitch             = request.CompanyPitch,
                HiringBudgetMinorUnits   = request.HiringBudgetMinorUnits,
                CreatedDate              = sessionState.TimeState.CurrentDate
            };

            var state = new JobPostRuntimeState
            {
                JobPostId = jobPostId,
                Status    = JobPostStatus.Open
            };

            sessionState.JobPostProfiles.Add(profile);
            sessionState.JobPostStates.Add(state);
            sessionState.RecruitmentState.JobPostIds.Add(jobPostId);

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[PostJobUseCase] Job post created: {jobPostId} role={request.Role} " +
                $"seniority={request.Seniority} salaryRange=[{request.SalaryRangeMinMinorUnits}–{request.SalaryRangeMaxMinorUnits}]");

            return PostJobResult.Succeeded(jobPostId);
        }
    }
}
