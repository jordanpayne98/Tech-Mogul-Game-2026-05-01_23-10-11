using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Team;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Team;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Team
{
    /// <summary>
    /// Concrete implementation of ITeamService.
    /// Stateless domain service — all mutable state is received as parameters.
    /// Does not publish events. Event publication is the responsibility of use cases.
    ///
    /// Uses TeamAssignmentRequirementRules for role and skill category mappings.
    /// </summary>
    public sealed class TeamService : ITeamService
    {
        // ─── CreateTeamProfile ────────────────────────────────────────────────────

        public TeamProfile CreateTeamProfile(string name, TeamType type, GameDateTime createdDate)
        {
            string guid = Guid.NewGuid().ToString("N");

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[TeamService] Creating team profile. Name='{name}' Type={type} Id={guid}");

            return new TeamProfile
            {
                Id          = guid,
                Name        = name,
                Type        = type,
                CreatedDate = createdDate
            };
        }

        // ─── CreateTeamState ──────────────────────────────────────────────────────

        public TeamRuntimeState CreateTeamState(string teamId, List<string> memberIds, ITeamTuning tuning)
        {
            return new TeamRuntimeState
            {
                TeamId              = teamId,
                MemberIds           = new List<string>(memberIds),
                Cohesion            = tuning.DefaultCohesion,
                Morale              = tuning.DefaultMorale,
                Workload            = 0,
                CurrentAssignmentId = null
            };
        }

        // ─── ValidateTeamCreation ─────────────────────────────────────────────────

        public bool ValidateTeamCreation(
            CreateTeamRequest request,
            GameSessionState sessionState,
            ITeamTuning tuning,
            out string failureReason)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                failureReason = "Team name is required";
                return false;
            }

            if (request.MemberIds == null || request.MemberIds.Count < tuning.MinTeamSize)
            {
                failureReason = $"Team requires at least {tuning.MinTeamSize} member(s)";
                return false;
            }

            if (request.MemberIds.Count > tuning.MaxTeamSize)
            {
                failureReason = $"Team exceeds maximum size of {tuning.MaxTeamSize}";
                return false;
            }

            // Validate each requested member ID
            foreach (string memberId in request.MemberIds)
            {
                EmployeeRuntimeState employeeState = sessionState.EmployeeStates
                    .Find(e => e.EmployeeId == memberId);

                if (employeeState == null)
                {
                    failureReason = $"Employee '{memberId}' does not exist";
                    return false;
                }

                if (employeeState.Status != EmploymentStatus.Active)
                {
                    failureReason = $"Employee '{memberId}' is not active";
                    return false;
                }

                if (employeeState.CurrentTeamId != null)
                {
                    failureReason = $"Employee '{memberId}' is already assigned to another team";
                    return false;
                }
            }

            failureReason = string.Empty;
            return true;
        }

        // ─── ValidateAssignment ───────────────────────────────────────────────────

        public bool ValidateAssignment(
            AssignTeamRequest request,
            GameSessionState sessionState,
            out string failureReason)
        {
            TeamRuntimeState teamState = sessionState.TeamStates
                .Find(t => t.TeamId == request.TeamId);

            if (teamState == null)
            {
                failureReason = "Team not found";
                return false;
            }

            if (teamState.CurrentAssignmentId != null)
            {
                failureReason = "Team already has an active assignment. Unassign first.";
                return false;
            }

            if (teamState.MemberIds.Count == 0)
            {
                failureReason = "Team has no members";
                return false;
            }

            if (string.IsNullOrEmpty(request.TargetId))
            {
                failureReason = "Target entity ID is required";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        // ─── CreateAssignment ─────────────────────────────────────────────────────

        public AssignmentRuntimeState CreateAssignment(AssignTeamRequest request, GameDateTime startDate)
        {
            string guid = Guid.NewGuid().ToString("N");

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[TeamService] Creating assignment. TeamId={request.TeamId} Type={request.Type} " +
                $"TargetId={request.TargetId} AssignmentId={guid}");

            return new AssignmentRuntimeState
            {
                Id                      = guid,
                Type                    = request.Type,
                TargetType              = request.TargetType,
                TargetId                = request.TargetId,
                TeamId                  = request.TeamId,
                ProgressPercent         = 0,
                RawProgressPoints       = 0,
                StartDate               = startDate,
                EstimatedCompletionDate = null
            };
        }

        // ─── AddMember ────────────────────────────────────────────────────────────

        public void AddMember(TeamRuntimeState teamState, string employeeId, ITeamTuning tuning)
        {
            teamState.MemberIds.Add(employeeId);
            teamState.Cohesion = Math.Max(0, teamState.Cohesion - tuning.CohesionMemberChangePenalty);
        }

        // ─── RemoveMember ─────────────────────────────────────────────────────────

        public void RemoveMember(TeamRuntimeState teamState, string employeeId, ITeamTuning tuning)
        {
            teamState.MemberIds.Remove(employeeId);
            teamState.Cohesion = Math.Max(0, teamState.Cohesion - tuning.CohesionMemberChangePenalty);
        }

        // ─── ResolveTeamLead ──────────────────────────────────────────────────────

        public EmployeeRuntimeState ResolveTeamLead(TeamRuntimeState teamState, List<EmployeeRuntimeState> members)
        {
            if (members == null || members.Count == 0)
            {
                return null;
            }

            // Filter to Lead+ seniority members only.
            List<EmployeeRuntimeState> leads = members
                .Where(m => m.Seniority >= Seniority.Lead)
                .OrderByDescending(m => m.Seniority)
                .ThenByDescending(m => m.Skills.ContainsKey(SkillCategory.Leadership)
                    ? m.Skills[SkillCategory.Leadership]
                    : 0)
                .ToList();

            return leads.Count > 0 ? leads[0] : null;
        }

        // ─── ComputeRoleCoverage ──────────────────────────────────────────────────

        public float ComputeRoleCoverage(List<EmployeeRuntimeState> members, AssignmentType assignmentType)
        {
            List<EmployeeRole> requiredRoles = TeamAssignmentRequirementRules.GetRequiredRoles(assignmentType);

            if (requiredRoles.Count == 0)
            {
                return 1.0f;
            }

            int coveredCount = 0;
            foreach (EmployeeRole role in requiredRoles)
            {
                bool covered = members.Any(m => m.Role == role);
                if (covered)
                {
                    coveredCount++;
                }
            }

            float coverage = (float)coveredCount / requiredRoles.Count;
            return Math.Clamp(coverage, 0.5f, 1.5f);
        }

        // ─── ComputeSkillFit ──────────────────────────────────────────────────────

        public float ComputeSkillFit(List<EmployeeRuntimeState> members, AssignmentType assignmentType)
        {
            List<SkillCategory> requiredSkills = TeamAssignmentRequirementRules.GetRequiredSkills(assignmentType);

            if (requiredSkills.Count == 0)
            {
                return 1.0f;
            }

            float skillTotal = 0f;

            foreach (SkillCategory skill in requiredSkills)
            {
                float skillAverage = 0f;

                if (members.Count > 0)
                {
                    int memberTotal = 0;
                    foreach (EmployeeRuntimeState member in members)
                    {
                        memberTotal += member.Skills.ContainsKey(skill) ? member.Skills[skill] : 0;
                    }

                    skillAverage = (float)memberTotal / members.Count;
                }

                skillTotal += skillAverage;
            }

            float overallAverage = skillTotal / requiredSkills.Count;
            float fit = overallAverage / 50f;
            return Math.Clamp(fit, 0.5f, 1.5f);
        }

        // ─── ComputeCohesionMultiplier ────────────────────────────────────────────

        public float ComputeCohesionMultiplier(TeamRuntimeState teamState)
        {
            float multiplier = teamState.Cohesion / 100f;
            return Math.Clamp(multiplier, 0.5f, 1.0f);
        }

        // ─── ComputeMoraleMultiplier ──────────────────────────────────────────────

        public float ComputeMoraleMultiplier(TeamRuntimeState teamState)
        {
            float multiplier = teamState.Morale / 100f;
            return Math.Clamp(multiplier, 0.5f, 1.0f);
        }

        // ─── ComputeLeadBonus ─────────────────────────────────────────────────────

        public float ComputeLeadBonus(TeamRuntimeState teamState, List<EmployeeRuntimeState> members, ITeamTuning tuning)
        {
            EmployeeRuntimeState lead = ResolveTeamLead(teamState, members);
            return lead != null ? tuning.LeadCoordinationBonusMultiplier : 1.0f;
        }

        // ─── ComputeDailyProgress ─────────────────────────────────────────────────

        public int ComputeDailyProgress(
            TeamRuntimeState teamState,
            List<EmployeeRuntimeState> members,
            AssignmentType assignmentType,
            ITeamTuning tuning)
        {
            float roleCoverage = ComputeRoleCoverage(members, assignmentType);
            float skillFit     = ComputeSkillFit(members, assignmentType);
            float cohesion     = ComputeCohesionMultiplier(teamState);
            float morale       = ComputeMoraleMultiplier(teamState);
            float leadBonus    = ComputeLeadBonus(teamState, members, tuning);

            // [Placeholder] — no tooling system implemented yet.
            const float Tooling = 1.0f;

            // [Placeholder] — overload impossible in MVP binary workload model.
            const float WorkloadMod = 1.0f;

            // [Placeholder] — complexity defined per target entity in plans 2E–2G.
            const float Complexity = 1.0f;

            float raw    = tuning.BaseTeamCapacity * roleCoverage * skillFit * cohesion * morale * leadBonus * Tooling * WorkloadMod * Complexity;
            int   result = Math.Max(tuning.MinDailyProgress, (int)raw);

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[TeamService] ComputeDailyProgress TeamId={teamState.TeamId} " +
                $"roleCoverage={roleCoverage:F2} skillFit={skillFit:F2} cohesion={cohesion:F2} " +
                $"morale={morale:F2} leadBonus={leadBonus:F2} raw={raw:F2} result={result}");

            return result;
        }

        // ─── UpdateCohesion ───────────────────────────────────────────────────────

        public void UpdateCohesion(TeamRuntimeState teamState, ITeamTuning tuning)
        {
            // Caller is responsible for ensuring this is only called when the team has an active assignment.
            teamState.Cohesion = Math.Min(100, teamState.Cohesion + (int)tuning.CohesionGrowthRatePerDay);
        }

        // ─── ComputeWorkload ──────────────────────────────────────────────────────

        public int ComputeWorkload(TeamRuntimeState teamState)
        {
            // Binary MVP model: assigned = 100, unassigned = 0.
            // Overload (above OverloadThresholdPercent = 120) is impossible in this model.
            return teamState.CurrentAssignmentId != null ? 100 : 0;
        }

        // ─── UpdateTeamMorale ─────────────────────────────────────────────────────

        public void UpdateTeamMorale(TeamRuntimeState teamState, List<EmployeeRuntimeState> members)
        {
            if (members == null || members.Count == 0)
            {
                return;
            }

            int moraleSum = 0;
            foreach (EmployeeRuntimeState member in members)
            {
                moraleSum += member.Morale;
            }

            int avgMorale = moraleSum / members.Count;
            teamState.Morale = Math.Clamp(avgMorale, 0, 100);
        }
    }
}
