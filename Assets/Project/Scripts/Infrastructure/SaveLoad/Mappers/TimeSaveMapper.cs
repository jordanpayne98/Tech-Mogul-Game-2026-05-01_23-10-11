using System;
using Project.Core.Definitions.Time;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Time;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Time domain runtime types to and from their save data equivalents.
    /// All methods are static — this mapper holds no state.
    /// GameDateTime fields convert via TotalElapsedHours / GameDateTime.FromTotalHours().
    /// Enum fields convert via ToString() / Enum.Parse().
    /// </summary>
    public static class TimeSaveMapper
    {
        public static TimeSaveData ToSaveData(TimeRuntimeState state)
        {
            return new TimeSaveData
            {
                CurrentDateTotalElapsedHours = state.CurrentDate.TotalElapsedHours,
                TotalElapsedHours            = state.TotalElapsedHours,
                Speed                        = state.Speed.ToString(),
                AdvanceMode                  = state.AdvanceMode.ToString(),
                Filter                       = state.Filter.ToString(),
                IsAdvancing                  = state.IsAdvancing
            };
        }

        public static TimeRuntimeState FromSaveData(TimeSaveData data)
        {
            return new TimeRuntimeState
            {
                CurrentDate       = GameDateTime.FromTotalHours(data.CurrentDateTotalElapsedHours),
                TotalElapsedHours = data.TotalElapsedHours,
                Speed             = Enum.Parse<TimeSpeed>(data.Speed),
                AdvanceMode       = Enum.Parse<TimeAdvanceMode>(data.AdvanceMode),
                Filter            = Enum.Parse<InterruptionFilter>(data.Filter),
                IsAdvancing       = data.IsAdvancing
            };
        }
    }
}
