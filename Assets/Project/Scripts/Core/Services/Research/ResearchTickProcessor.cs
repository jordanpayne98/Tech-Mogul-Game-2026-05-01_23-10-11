using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Time;
using Project.Core.Definitions.Team;
using Project.Core.Definitions.Research;
using Project.Core.Events.Research;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Research
{
    /// <summary>
    /// Daily tick processor for research progress derivation and project completion.
    /// Runs at ProcessingOrder 600, after TeamProgressTickProcessor (Order 100) has written
    /// RawProgressPoints to all AssignmentRuntimeState entries.
    ///
    /// On each day boundary, iterates all active research projects, derives ProgressPercent
    /// from accumulated RawProgressPoints, and fires completion when a project reaches 100%.
    ///
    /// On completion: grants unlocks, updates portfolio state lists, evaluates newly available
    /// projects, and emits an InterruptionRequest (ResearchCompleted) and ResearchCompletedEvent.
    /// Does NOT unassign teams on completion — the player must manually unassign via UnassignTeamUseCase.
    ///
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public sealed class ResearchTickProcessor : ITickProcessor
    {
        private readonly IResearchService _researchService;
        private readonly IResearchTuning  _tuning;
        private readonly IEventBus        _eventBus;
        private readonly GameSessionState _sessionState;

        public string ProcessorName   => "ResearchTickProcessor";
        public int    ProcessingOrder => 600;

        public ResearchTickProcessor(
            IResearchService researchService,
            IResearchTuning  tuning,
            IEventBus        eventBus,
            GameSessionState sessionState)
        {
            _researchService = researchService;
            _tuning          = tuning;
            _eventBus        = eventBus;
            _sessionState    = sessionState;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            // Research progress is updated once per in-game day.
            if (!context.IsDayBoundary)
            {
                return TickResult.Succeeded();
            }

            var interruptions = new List<InterruptionRequest>();

            // Iterate a copy of ActiveProjectIds so we can mutate the list on completion.
            var activeProjectIds = new List<string>(_sessionState.ResearchState.ActiveProjectIds);

            foreach (string projectId in activeProjectIds)
            {
                // Resolve the definition from the static catalog.
                var definition = ResearchProjectCatalog.GetProject(projectId);
                if (definition == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ResearchTickProcessor] Definition not found for active project. ProjectId: {projectId}. Skipping.");
                    continue;
                }

                // Resolve the runtime state for this project.
                var projectState = _sessionState.ResearchProjectStates
                    .FirstOrDefault(s => s.ProjectId == projectId);

                if (projectState == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ResearchTickProcessor] Runtime state not found for active project. ProjectId: {projectId}. Skipping.");
                    continue;
                }

                // Find the assignment targeting this research project.
                var assignment = _sessionState.AssignmentStates
                    .FirstOrDefault(a => a.TargetType == AssignmentTargetType.ResearchProject
                                      && a.TargetId   == projectId);

                if (assignment == null)
                {
                    // No team assigned — no progress this tick.
                    continue;
                }

                // Compute how much total work is required for this project.
                int requiredWorkUnits = _researchService.ComputeRequiredWorkUnits(definition, _tuning);

                // Derive progress percent from accumulated raw progress points.
                int progressPercent = _researchService.DeriveProgressPercent(
                    assignment.RawProgressPoints,
                    requiredWorkUnits);

                projectState.ProgressPercent = progressPercent;

                // Check for completion.
                if (progressPercent < 100)
                {
                    continue;
                }

                // ── Completion flow ────────────────────────────────────────────────

                // Mark project as completed.
                projectState.Status        = ResearchProjectStatus.Completed;
                projectState.CompletedDate = context.CurrentDate;

                // Grant unlocks and collect newly unlocked capability IDs.
                var newlyGranted = _researchService.GrantUnlocks(
                    definition,
                    _sessionState.ResearchState);

                // Update portfolio state lists.
                _sessionState.ResearchState.ActiveProjectIds.Remove(projectId);
                _sessionState.ResearchState.CompletedProjectIds.Add(projectId);

                // Evaluate any projects that become available due to this completion.
                var newlyAvailable = _researchService.EvaluateNewlyAvailableProjects(
                    _sessionState.ResearchState);

                foreach (string newProjectId in newlyAvailable)
                {
                    _sessionState.ResearchState.AvailableProjectIds.Add(newProjectId);
                }

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[ResearchTickProcessor] Research project completed. ProjectId: {projectId}, "
                    + $"UnlocksGranted: {newlyGranted.Count}, NewlyAvailable: {newlyAvailable.Count}, "
                    + $"Date: {context.CurrentDate}");

                // Publish domain event.
                _eventBus.Publish(new ResearchCompletedEvent(projectId, newlyGranted));

                // Raise a Continue interruption so the player is informed.
                interruptions.Add(new InterruptionRequest(
                    InterruptionType.ResearchCompleted,
                    projectId,
                    $"Research project completed: {definition.Name}"));
            }

            return interruptions.Count > 0
                ? TickResult.Succeeded(interruptions)
                : TickResult.Succeeded();
        }
    }
}
