using Project.Application.UseCases.Finance;
using Project.Core.Interfaces;
using Project.Core.Results.Time;
using Project.Core.Runtime.Time;

namespace Project.Application.TickProcessors
{
    /// <summary>
    /// Thin monthly tick processor that delegates to ProcessMonthlyFinanceUseCase.
    /// Runs at Order 700, after SoftwareMetricsTickProcessor (350) and HardwareMetricsTickProcessor (400).
    /// Contains no business logic — delegates entirely to the use case.
    ///
    /// Placed in Application rather than Core/Services because it delegates to an Application use case.
    /// This is an approved exception to the pattern of other tick processors living in Core.
    ///
    /// On a month-boundary tick:
    ///   - SoftwareMetricsTickProcessor (350) skips monthly counter accumulation
    ///   - HardwareMetricsTickProcessor (400) skips monthly counter accumulation
    ///   - FinanceTickProcessor (700) snapshots, processes, resets, and publishes
    ///
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class FinanceTickProcessor : ITickProcessor
    {
        private readonly ProcessMonthlyFinanceUseCase _useCase;

        /// <inheritdoc/>
        public string ProcessorName => "FinanceTickProcessor";

        /// <inheritdoc/>
        public int ProcessingOrder => 700;

        /// <summary>
        /// Creates a new FinanceTickProcessor.
        /// </summary>
        /// <param name="useCase">The monthly finance use case to delegate to.</param>
        public FinanceTickProcessor(ProcessMonthlyFinanceUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            if (!context.IsMonthBoundary)
            {
                return TickResult.Succeeded();
            }

            return _useCase.Execute(context);
        }
    }
}
