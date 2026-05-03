using Project.Core.Debugging;
using Project.Core.Definitions.Time;
using Project.Core.Interfaces.Services;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Time
{
    /// <summary>
    /// Concrete implementation of ITimeService.
    /// Stateless service — receives TimeRuntimeState as a parameter and returns results.
    /// All state mutation is explicit; this class owns no persistent state.
    /// Logs via DebugLogger with DebugCategory.Simulation.
    /// No UnityEngine dependency.
    /// </summary>
    public sealed class TimeService : ITimeService
    {
        // -------------------------------------------------------------------------
        // ITimeService
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public TickContext AdvanceOneHour(TimeRuntimeState state)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TimeService] AdvanceOneHour called with null state. Returning null.");
                return null;
            }

            GameDateTime previousDate = state.CurrentDate;
            GameDateTime newDate      = previousDate.AddHours(1);

            state.CurrentDate       = newDate;
            state.TotalElapsedHours = newDate.TotalElapsedHours;

            TickContext context = new TickContext(previousDate, newDate, 1);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[TimeService] Advanced: {previousDate} → {newDate}" +
                $" | Day:{context.IsDayBoundary} Week:{context.IsWeekBoundary}" +
                $" Month:{context.IsMonthBoundary} Year:{context.IsYearBoundary}");

            return context;
        }

        /// <inheritdoc/>
        public void SetSpeed(TimeRuntimeState state, TimeSpeed speed)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TimeService] SetSpeed called with null state.");
                return;
            }

            if (speed == TimeSpeed.Paused)
            {
                DebugLogger.LogWarning(DebugCategory.Simulation,
                    "[TimeService] SetSpeed called with Paused during active advancing. " +
                    "This will halt time if IsAdvancing is checked externally.");
            }

            state.Speed = speed;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[TimeService] Speed set to {speed}.");
        }

        /// <inheritdoc/>
        public void SetAdvanceMode(TimeRuntimeState state, TimeAdvanceMode mode)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TimeService] SetAdvanceMode called with null state.");
                return;
            }

            state.AdvanceMode = mode;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[TimeService] AdvanceMode set to {mode}.");
        }

        /// <inheritdoc/>
        public void SetInterruptionFilter(TimeRuntimeState state, InterruptionFilter filter)
        {
            if (state == null)
            {
                DebugLogger.LogError(DebugCategory.Simulation,
                    "[TimeService] SetInterruptionFilter called with null state.");
                return;
            }

            state.Filter = filter;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[TimeService] InterruptionFilter set to {filter}.");
        }
    }
}
