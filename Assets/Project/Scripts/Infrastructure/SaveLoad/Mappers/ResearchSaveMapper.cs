using System;
using Project.Core.Definitions.Research;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Research;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Research domain runtime types to and from their save data equivalents.
    /// Covers ResearchRuntimeState and ResearchProjectRuntimeState.
    /// All methods are static — this mapper holds no state.
    /// Nullable GameDateTime fields convert to nullable int via TotalElapsedHours.
    /// </summary>
    public static class ResearchSaveMapper
    {
        // ─── ResearchRuntimeState ─────────────────────────────────────────────────

        public static ResearchSaveData ToSaveData(ResearchRuntimeState state)
        {
            return new ResearchSaveData
            {
                CompanyId              = state.CompanyId,
                AvailableProjectIds    = state.AvailableProjectIds,
                ActiveProjectIds       = state.ActiveProjectIds,
                CompletedProjectIds    = state.CompletedProjectIds,
                ObsoleteProjectIds     = state.ObsoleteProjectIds,
                UnlockedCapabilityIds  = state.UnlockedCapabilityIds
            };
        }

        public static ResearchRuntimeState FromSaveData(ResearchSaveData data)
        {
            return new ResearchRuntimeState
            {
                CompanyId             = data.CompanyId,
                AvailableProjectIds   = data.AvailableProjectIds,
                ActiveProjectIds      = data.ActiveProjectIds,
                CompletedProjectIds   = data.CompletedProjectIds,
                ObsoleteProjectIds    = data.ObsoleteProjectIds,
                UnlockedCapabilityIds = data.UnlockedCapabilityIds
            };
        }

        // ─── ResearchProjectRuntimeState ──────────────────────────────────────────

        public static ResearchProjectSaveData ToSaveData(ResearchProjectRuntimeState state)
        {
            // Nullable GameDateTime fields: default (zero TotalElapsedHours) means not yet set.
            int? startHours = state.StartDate.TotalElapsedHours == 0
                ? (int?)null
                : state.StartDate.TotalElapsedHours;

            int? completedHours = state.CompletedDate.TotalElapsedHours == 0
                ? (int?)null
                : state.CompletedDate.TotalElapsedHours;

            int? estimatedHours = state.EstimatedCompletionDate.TotalElapsedHours == 0
                ? (int?)null
                : state.EstimatedCompletionDate.TotalElapsedHours;

            return new ResearchProjectSaveData
            {
                ProjectId                              = state.ProjectId,
                Status                                 = state.Status.ToString(),
                ProgressPercent                        = state.ProgressPercent,
                AssignedTeamId                         = state.AssignedTeamId,
                StartDateTotalElapsedHours             = startHours,
                CompletedDateTotalElapsedHours         = completedHours,
                EstimatedCompletionDateTotalElapsedHours = estimatedHours
            };
        }

        public static ResearchProjectRuntimeState FromSaveData(ResearchProjectSaveData data)
        {
            GameDateTime startDate = data.StartDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.StartDateTotalElapsedHours.Value)
                : default;

            GameDateTime completedDate = data.CompletedDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.CompletedDateTotalElapsedHours.Value)
                : default;

            GameDateTime estimatedDate = data.EstimatedCompletionDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.EstimatedCompletionDateTotalElapsedHours.Value)
                : default;

            return new ResearchProjectRuntimeState
            {
                ProjectId               = data.ProjectId,
                Status                  = Enum.Parse<ResearchProjectStatus>(data.Status),
                ProgressPercent         = data.ProgressPercent,
                AssignedTeamId          = data.AssignedTeamId,
                StartDate               = startDate,
                CompletedDate           = completedDate,
                EstimatedCompletionDate = estimatedDate
            };
        }
    }
}
