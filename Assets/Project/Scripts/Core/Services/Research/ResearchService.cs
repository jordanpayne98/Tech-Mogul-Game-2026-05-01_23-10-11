using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Definitions.Research;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Research;
using Project.Core.Runtime;
using Project.Core.Runtime.Research;

namespace Project.Core.Services.Research
{
    /// <summary>
    /// Concrete implementation of IResearchService.
    /// Stateless — all mutable state is received as parameters.
    /// Does not publish events. Uses ResearchProjectCatalog for definition lookups.
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public sealed class ResearchService : IResearchService
    {
        // ── Progress ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ComputeRequiredWorkUnits(ResearchProjectDefinition definition, IResearchTuning tuning)
        {
            // Step 1: base work = days * work units per day
            int requiredWorkUnits = definition.EstimatedDurationDays * tuning.BaseResearchWorkUnitsPerDay;

            // Step 2: combined multiplier from expertise and tooling
            float multiplier = tuning.LeadExpertiseMultiplier * tuning.ToolingMultiplier;

            int adjustedRequired;
            if (multiplier <= 0f)
            {
                // Guard: if multiplier is zero or negative, apply no reduction.
                adjustedRequired = requiredWorkUnits;
            }
            else
            {
                // Higher multiplier → fewer required work units → faster research.
                adjustedRequired = (int)Math.Ceiling(requiredWorkUnits / multiplier);
            }

            // Step 3: ensure at least 1 work unit is always required.
            return Math.Max(1, adjustedRequired);
        }

        /// <inheritdoc/>
        public int DeriveProgressPercent(int rawProgressPoints, int requiredWorkUnits)
        {
            // Guard division by zero — treat 0-required as already complete.
            if (requiredWorkUnits <= 0)
            {
                return 100;
            }

            return Math.Clamp(rawProgressPoints * 100 / requiredWorkUnits, 0, 100);
        }

        // ── Prerequisites ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public bool ArePrerequisitesMet(ResearchProjectDefinition definition, ResearchRuntimeState researchState)
        {
            if (definition.PrerequisiteIds == null || definition.PrerequisiteIds.Count == 0)
            {
                return true;
            }

            foreach (string prerequisiteId in definition.PrerequisiteIds)
            {
                if (!researchState.CompletedProjectIds.Contains(prerequisiteId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public List<string> EvaluateNewlyAvailableProjects(ResearchRuntimeState researchState)
        {
            var newlyAvailable = new List<string>();

            foreach (var definition in ResearchProjectCatalog.GetAllProjects())
            {
                // Skip projects already in a terminal or active state.
                if (researchState.AvailableProjectIds.Contains(definition.Id)
                    || researchState.ActiveProjectIds.Contains(definition.Id)
                    || researchState.CompletedProjectIds.Contains(definition.Id)
                    || researchState.ObsoleteProjectIds.Contains(definition.Id))
                {
                    continue;
                }

                // Only unlock if all prerequisites are now met.
                if (ArePrerequisitesMet(definition, researchState))
                {
                    newlyAvailable.Add(definition.Id);
                }
            }

            return newlyAvailable;
        }

        // ── Validation ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public bool ValidateStartResearch(
            StartResearchRequest request,
            ResearchRuntimeState researchState,
            GameSessionState sessionState,
            long projectCostMinorUnits,
            out string failureReason)
        {
            // 1. Project definition must exist.
            var definition = ResearchProjectCatalog.GetProject(request.ResearchProjectId);
            if (definition == null)
            {
                failureReason = "Research project not found.";
                return false;
            }

            // 2. Project must be in Available state.
            if (!researchState.AvailableProjectIds.Contains(request.ResearchProjectId))
            {
                failureReason = "Research project is not available.";
                return false;
            }

            // 3. Team must exist.
            var teamState = sessionState.TeamStates
                .FirstOrDefault(t => t.TeamId == request.TeamId);

            if (teamState == null)
            {
                failureReason = "Team not found.";
                return false;
            }

            // 4. Team must have no current assignment.
            if (teamState.CurrentAssignmentId != null)
            {
                failureReason = "Team already has an active assignment.";
                return false;
            }

            // 5. Company must have sufficient cash.
            if (sessionState.FinanceState.CashMinorUnits < projectCostMinorUnits)
            {
                failureReason = "Insufficient funds.";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        // ── Completion ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public List<string> GrantUnlocks(ResearchProjectDefinition definition, ResearchRuntimeState researchState)
        {
            var newlyGranted = new List<string>();

            if (definition.Unlocks == null)
            {
                return newlyGranted;
            }

            foreach (var unlock in definition.Unlocks)
            {
                // Skip capabilities that have already been unlocked (no duplicates).
                if (researchState.UnlockedCapabilityIds.Contains(unlock.TargetId))
                {
                    continue;
                }

                researchState.UnlockedCapabilityIds.Add(unlock.TargetId);
                newlyGranted.Add(unlock.TargetId);
            }

            return newlyGranted;
        }
    }
}
