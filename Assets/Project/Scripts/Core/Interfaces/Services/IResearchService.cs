using System.Collections.Generic;
using Project.Core.Definitions.Research;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Research;
using Project.Core.Runtime;
using Project.Core.Runtime.Research;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Research domain service interface.
    /// Covers required work computation, progress derivation, prerequisite evaluation,
    /// start validation, and unlock granting.
    /// Stateless — all state is passed in as parameters. Does not publish events.
    /// Uses ResearchProjectCatalog for definition lookups.
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public interface IResearchService
    {
        // ── Progress ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the total abstract work units required to complete a research project.
        /// Applies formula.research.required_work:
        ///   requiredWork = EstimatedDurationDays * BaseResearchWorkUnitsPerDay
        ///   adjusted     = Ceiling(requiredWork / (LeadExpertiseMultiplier * ToolingMultiplier))
        ///   final        = Max(1, adjusted)
        /// If the combined multiplier is 0 or negative, the unadjusted value is used.
        /// Result is always at least 1.
        /// </summary>
        int ComputeRequiredWorkUnits(ResearchProjectDefinition definition, IResearchTuning tuning);

        /// <summary>
        /// Derives the completion percentage from raw progress and required work.
        /// Applies formula.research.progress_percent:
        ///   percent = Clamp(rawProgressPoints * 100 / requiredWorkUnits, 0, 100)
        /// Guards division by zero — returns 100 if requiredWorkUnits is 0 or less.
        /// </summary>
        int DeriveProgressPercent(int rawProgressPoints, int requiredWorkUnits);

        // ── Prerequisites ─────────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if all prerequisite project IDs for the given definition
        /// are present in <see cref="ResearchRuntimeState.CompletedProjectIds"/>.
        /// Returns true unconditionally if the definition has no prerequisites.
        /// </summary>
        bool ArePrerequisitesMet(ResearchProjectDefinition definition, ResearchRuntimeState researchState);

        /// <summary>
        /// Scans the full catalog and returns stable IDs of projects that are currently Locked
        /// but have all prerequisites met. Excludes projects that are already Available,
        /// InProgress, Completed, or Obsolete (i.e. any ID already in AvailableProjectIds,
        /// ActiveProjectIds, CompletedProjectIds, or ObsoleteProjectIds).
        /// </summary>
        List<string> EvaluateNewlyAvailableProjects(ResearchRuntimeState researchState);

        // ── Validation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Validates that a StartResearchRequest can be executed.
        /// Returns true on success. Returns false and sets failureReason on failure.
        /// Checks: project exists, project is Available, team exists,
        ///         team has no current assignment, sufficient cash.
        /// </summary>
        bool ValidateStartResearch(
            StartResearchRequest request,
            ResearchRuntimeState researchState,
            GameSessionState sessionState,
            long projectCostMinorUnits,
            out string failureReason);

        // ── Completion ────────────────────────────────────────────────────────────

        /// <summary>
        /// Grants all unlocks defined in the project definition.
        /// Adds each Unlock.TargetId to <see cref="ResearchRuntimeState.UnlockedCapabilityIds"/>,
        /// skipping any that are already present.
        /// Returns the list of newly granted capability IDs (excludes duplicates).
        /// </summary>
        List<string> GrantUnlocks(ResearchProjectDefinition definition, ResearchRuntimeState researchState);
    }
}
