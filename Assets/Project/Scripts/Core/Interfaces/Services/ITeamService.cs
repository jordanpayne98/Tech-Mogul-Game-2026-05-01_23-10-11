using System.Collections.Generic;
using Project.Core.Definitions.Team;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Team;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Domain service interface for team management.
    /// Owns creation, membership changes, assignment validation,
    /// progress factor calculation, cohesion updates, and workload calculation.
    ///
    /// Stateless — all mutable state is passed in as parameters. Does not publish events.
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// Creates a new TeamProfile with a GUID stable ID.
        /// </summary>
        TeamProfile CreateTeamProfile(string name, TeamType type, GameDateTime createdDate);

        /// <summary>
        /// Creates the initial TeamRuntimeState for a newly formed team.
        /// Sets default cohesion, morale from tuning; workload = 0; CurrentAssignmentId = null.
        /// </summary>
        TeamRuntimeState CreateTeamState(string teamId, List<string> memberIds, ITeamTuning tuning);

        /// <summary>
        /// Validates that a CreateTeamRequest can be executed given the current session state.
        /// Returns true on success. Returns false and sets failureReason on failure.
        /// </summary>
        bool ValidateTeamCreation(
            CreateTeamRequest request,
            GameSessionState sessionState,
            ITeamTuning tuning,
            out string failureReason);

        /// <summary>
        /// Validates that an AssignTeamRequest can be executed given the current session state.
        /// Returns true on success. Returns false and sets failureReason on failure.
        /// </summary>
        bool ValidateAssignment(
            AssignTeamRequest request,
            GameSessionState sessionState,
            out string failureReason);

        /// <summary>
        /// Creates a new AssignmentRuntimeState with a GUID stable ID.
        /// RawProgressPoints = 0, ProgressPercent = 0, EstimatedCompletionDate = null.
        /// </summary>
        AssignmentRuntimeState CreateAssignment(AssignTeamRequest request, GameDateTime startDate);

        /// <summary>
        /// Adds a member to the team and applies the cohesion member-change penalty.
        /// </summary>
        void AddMember(TeamRuntimeState teamState, string employeeId, ITeamTuning tuning);

        /// <summary>
        /// Removes a member from the team and applies the cohesion member-change penalty.
        /// </summary>
        void RemoveMember(TeamRuntimeState teamState, string employeeId, ITeamTuning tuning);

        /// <summary>
        /// Returns the highest-seniority member (Lead+ preferred).
        /// Leadership skill breaks ties. Returns null if the team is empty.
        /// </summary>
        EmployeeRuntimeState ResolveTeamLead(TeamRuntimeState teamState, List<EmployeeRuntimeState> members);

        /// <summary>
        /// Fraction of required roles covered by at least one team member, clamped [0.5, 1.5].
        /// Returns 1.0 if no required roles are defined for the assignment type.
        /// </summary>
        float ComputeRoleCoverage(List<EmployeeRuntimeState> members, AssignmentType assignmentType);

        /// <summary>
        /// Average team skill score in required skill categories / 50, clamped [0.5, 1.5].
        /// Returns 1.0 if no required skills are defined for the assignment type.
        /// </summary>
        float ComputeSkillFit(List<EmployeeRuntimeState> members, AssignmentType assignmentType);

        /// <summary>
        /// Team cohesion / 100, clamped [0.5, 1.0].
        /// </summary>
        float ComputeCohesionMultiplier(TeamRuntimeState teamState);

        /// <summary>
        /// Team morale / 100, clamped [0.5, 1.0].
        /// </summary>
        float ComputeMoraleMultiplier(TeamRuntimeState teamState);

        /// <summary>
        /// Returns tuning.LeadCoordinationBonusMultiplier if a Lead+ seniority member exists, else 1.0f.
        /// </summary>
        float ComputeLeadBonus(TeamRuntimeState teamState, List<EmployeeRuntimeState> members, ITeamTuning tuning);

        /// <summary>
        /// Computes the full daily progress formula result.
        /// See Formula Registry: formula.team.daily_progress.
        /// </summary>
        int ComputeDailyProgress(
            TeamRuntimeState teamState,
            List<EmployeeRuntimeState> members,
            AssignmentType assignmentType,
            ITeamTuning tuning);

        /// <summary>
        /// Applies daily cohesion growth. Call only when the team has an active assignment.
        /// </summary>
        void UpdateCohesion(TeamRuntimeState teamState, ITeamTuning tuning);

        /// <summary>
        /// Returns 100 if the team has an active assignment, else 0.
        /// Binary workload model for MVP. Overload threshold is 120 (impossible in MVP).
        /// </summary>
        int ComputeWorkload(TeamRuntimeState teamState);

        /// <summary>
        /// Recalculates team morale as the integer average of all member morale values.
        /// No-op if members is empty.
        /// </summary>
        void UpdateTeamMorale(TeamRuntimeState teamState, List<EmployeeRuntimeState> members);
    }
}
