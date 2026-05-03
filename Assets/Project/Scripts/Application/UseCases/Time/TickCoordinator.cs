using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Interfaces;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Time
{
    /// <summary>
    /// Domain-agnostic tick coordinator. Holds an ordered list of ITickProcessor instances
    /// and calls each one per clock tick in ascending ProcessingOrder.
    /// Lives in Application — depends on Core interfaces only, no UnityEngine dependency.
    /// </summary>
    public sealed class TickCoordinator
    {
        private readonly IEventBus _eventBus;
        private readonly List<ITickProcessor> _processors = new();

        private bool _sortDirty    = false;
        private bool _firstTickLog = true;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        /// <summary>
        /// Creates a new TickCoordinator.
        /// </summary>
        /// <param name="eventBus">Event bus for future tick-level event publication.</param>
        public TickCoordinator(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        // -------------------------------------------------------------------------
        // Registration
        // -------------------------------------------------------------------------

        /// <summary>
        /// Registers a tick processor. Duplicate ProcessorName is rejected with an error.
        /// Marks the sort order as dirty; processors are sorted on the next ProcessTick call.
        /// </summary>
        /// <param name="processor">The processor to register.</param>
        public void Register(ITickProcessor processor)
        {
            if (processor == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TickCoordinator] Register called with null processor.");
                return;
            }

            foreach (ITickProcessor existing in _processors)
            {
                if (existing.ProcessorName == processor.ProcessorName)
                {
                    DebugLogger.LogError(DebugCategory.Simulation,
                        $"[TickCoordinator] Duplicate processor registration rejected: '{processor.ProcessorName}'.");
                    return;
                }
            }

            _processors.Add(processor);
            _sortDirty  = true;
            _firstTickLog = true; // Re-log processor list on next tick after any registration.

            DebugLogger.Log(DebugCategory.Simulation,
                $"[TickCoordinator] Registered processor: '{processor.ProcessorName}' (order {processor.ProcessingOrder}).");
        }

        // -------------------------------------------------------------------------
        // Tick processing
        // -------------------------------------------------------------------------

        /// <summary>
        /// Calls each registered processor in ProcessingOrder for the given tick context.
        /// Aggregates all interruptions from all processors into a single result.
        /// </summary>
        /// <param name="context">Immutable tick metadata. Must not be null.</param>
        public TickCoordinatorResult ProcessTick(TickContext context)
        {
            if (context == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TickCoordinator] ProcessTick called with null context.");
                return TickCoordinatorResult.Failed("TickContext is null.");
            }

            EnsureSorted();
            LogProcessorListOnFirstTick();

            List<InterruptionRequest> allInterruptions = null;
            int executedCount = 0;

            foreach (ITickProcessor processor in _processors)
            {
                TickResult result = processor.ProcessTick(context);
                executedCount++;

                if (!result.Success)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[TickCoordinator] Processor '{processor.ProcessorName}' failed: {result.FailureReason}");
                }

                if (result.Interruptions != null && result.Interruptions.Count > 0)
                {
                    allInterruptions ??= new List<InterruptionRequest>();
                    allInterruptions.AddRange(result.Interruptions);
                }
            }

            if (allInterruptions != null && allInterruptions.Count > 0)
            {
                return TickCoordinatorResult.Succeeded(executedCount, allInterruptions);
            }

            return TickCoordinatorResult.Succeeded(executedCount);
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private void EnsureSorted()
        {
            if (!_sortDirty)
            {
                return;
            }

            _processors.Sort((a, b) => a.ProcessingOrder.CompareTo(b.ProcessingOrder));
            _sortDirty = false;
        }

        private void LogProcessorListOnFirstTick()
        {
            if (!_firstTickLog)
            {
                return;
            }

            _firstTickLog = false;

            if (_processors.Count == 0)
            {
                DebugLogger.Log(DebugCategory.Simulation,
                    "[TickCoordinator] First tick — no processors registered. Time advances but no domain processing occurs.");
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append($"[TickCoordinator] First tick — {_processors.Count} processor(s) in order:");

            foreach (ITickProcessor processor in _processors)
            {
                sb.Append($" [{processor.ProcessingOrder}:{processor.ProcessorName}]");
            }

            DebugLogger.Log(DebugCategory.Simulation, sb.ToString());
        }
    }
}
