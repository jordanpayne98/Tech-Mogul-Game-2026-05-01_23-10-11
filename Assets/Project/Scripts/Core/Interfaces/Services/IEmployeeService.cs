using System.Collections.Generic;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Domain service interface for all employee and recruitment operations.
    /// Implementations must be stateless. System.Random is passed in as a parameter;
    /// the service must never create its own random instance.
    /// Does not publish events and does not access GameSessionState directly.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Generates a list of randomized candidate profiles.
        /// </summary>
        /// <param name="count">Number of candidates to generate.</param>
        /// <param name="startingCounter">Counter offset used to generate stable IDs sequentially.</param>
        /// <param name="tuning">Employee tuning values used for generation ranges.</param>
        /// <param name="currentDate">Current in-game date used for availability dates.</param>
        /// <param name="random">Shared random instance. Must not be replaced by the service.</param>
        List<CandidateProfile> GenerateCandidates(
            int count,
            int startingCounter,
            IEmployeeTuning tuning,
            GameDateTime currentDate,
            System.Random random);

        /// <summary>
        /// Creates default mutable runtime states for a list of generated candidate profiles.
        /// </summary>
        List<CandidateRuntimeState> CreateCandidateStates(List<CandidateProfile> candidates);

        /// <summary>
        /// Computes an offer acceptance score (0–100 before clamping) using the acceptance formula.
        /// </summary>
        /// <param name="candidate">The candidate profile receiving the offer.</param>
        /// <param name="offeredSalaryMinorUnits">The offered monthly salary in minor currency units.</param>
        /// <param name="companyReputation">Current company reputation score (0–100).</param>
        /// <param name="tuning">Employee tuning values for formula weights.</param>
        int EvaluateOfferAcceptance(
            CandidateProfile candidate,
            long offeredSalaryMinorUnits,
            int companyReputation,
            IEmployeeTuning tuning);

        /// <summary>
        /// Rolls against the clamped acceptance score to determine whether the candidate accepts.
        /// </summary>
        /// <param name="acceptanceScore">Raw score from EvaluateOfferAcceptance (may be outside 0–100).</param>
        /// <param name="random">Shared random instance. Must not be replaced by the service.</param>
        /// <returns>True if the offer is accepted; false if rejected.</returns>
        bool ResolveOffer(int acceptanceScore, System.Random random);

        /// <summary>
        /// Creates an EmployeeProfile from an accepted candidate at the moment of hire.
        /// </summary>
        /// <param name="candidate">The candidate being converted.</param>
        /// <param name="hireDate">The in-game date the hire occurred.</param>
        /// <param name="nextEmployeeCounter">Counter used to generate the stable employee ID.</param>
        EmployeeProfile ConvertToEmployeeProfile(
            CandidateProfile candidate,
            GameDateTime hireDate,
            int nextEmployeeCounter);

        /// <summary>
        /// Creates the initial mutable runtime state for a newly hired employee.
        /// </summary>
        /// <param name="candidate">The source candidate profile.</param>
        /// <param name="employeeId">The stable ID assigned to the employee.</param>
        /// <param name="offeredSalaryMinorUnits">The agreed monthly salary in minor currency units.</param>
        /// <param name="tuning">Employee tuning values for default morale/loyalty/ambition ranges.</param>
        /// <param name="random">Shared random instance for randomized default values.</param>
        EmployeeRuntimeState CreateEmployeeState(
            CandidateProfile candidate,
            string employeeId,
            long offeredSalaryMinorUnits,
            IEmployeeTuning tuning,
            System.Random random);

        /// <summary>
        /// Applies daily burnout recovery to an active employee who is not overworked.
        /// </summary>
        void ProcessDailyRecovery(EmployeeRuntimeState employee, IEmployeeTuning tuning);

        /// <summary>
        /// Applies monthly loyalty growth to an active employee.
        /// </summary>
        void ProcessMonthlyLoyalty(EmployeeRuntimeState employee, IEmployeeTuning tuning);
    }
}
