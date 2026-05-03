using System;
using Project.Core.Definitions.Team;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Team;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Team domain runtime types to and from their save data equivalents.
    /// Covers TeamProfile, TeamRuntimeState, and AssignmentRuntimeState.
    /// All methods are static — this mapper holds no state.
    /// Nullable GameDateTime fields convert to nullable int via TotalElapsedHours.
    /// </summary>
    public static class TeamSaveMapper
    {
        // ─── TeamProfile ──────────────────────────────────────────────────────────

        public static TeamSaveData ToSaveData(TeamProfile profile)
        {
            return new TeamSaveData
            {
                Id                          = profile.Id,
                Name                        = profile.Name,
                Type                        = profile.Type.ToString(),
                CreatedDateTotalElapsedHours = profile.CreatedDate.TotalElapsedHours
            };
        }

        public static TeamProfile FromSaveData(TeamSaveData data)
        {
            return new TeamProfile
            {
                Id          = data.Id,
                Name        = data.Name,
                Type        = Enum.Parse<TeamType>(data.Type),
                CreatedDate = GameDateTime.FromTotalHours(data.CreatedDateTotalElapsedHours)
            };
        }

        // ─── TeamRuntimeState ─────────────────────────────────────────────────────

        public static TeamStateSaveData ToSaveData(TeamRuntimeState state)
        {
            return new TeamStateSaveData
            {
                TeamId                = state.TeamId,
                MemberIds             = state.MemberIds,
                Cohesion              = state.Cohesion,
                Morale                = state.Morale,
                Workload              = state.Workload,
                CurrentAssignmentId   = state.CurrentAssignmentId,
                AssignmentHistoryIds  = state.AssignmentHistoryIds
            };
        }

        public static TeamRuntimeState FromSaveData(TeamStateSaveData data)
        {
            return new TeamRuntimeState
            {
                TeamId               = data.TeamId,
                MemberIds            = data.MemberIds,
                Cohesion             = data.Cohesion,
                Morale               = data.Morale,
                Workload             = data.Workload,
                CurrentAssignmentId  = data.CurrentAssignmentId,
                AssignmentHistoryIds = data.AssignmentHistoryIds
            };
        }

        // ─── AssignmentRuntimeState ───────────────────────────────────────────────

        public static AssignmentSaveData ToSaveData(AssignmentRuntimeState state)
        {
            // EstimatedCompletionDate is nullable (default GameDateTime == zero TotalElapsedHours).
            // The plan stores it as int? — null when not yet estimated (default/zero value).
            int? estimatedHours = state.EstimatedCompletionDate.TotalElapsedHours == 0
                ? (int?)null
                : state.EstimatedCompletionDate.TotalElapsedHours;

            return new AssignmentSaveData
            {
                Id                                       = state.Id,
                Type                                     = state.Type.ToString(),
                TargetType                               = state.TargetType.ToString(),
                TargetId                                 = state.TargetId,
                TeamId                                   = state.TeamId,
                ProgressPercent                          = state.ProgressPercent,
                RawProgressPoints                        = state.RawProgressPoints,
                StartDateTotalElapsedHours               = state.StartDate.TotalElapsedHours,
                EstimatedCompletionDateTotalElapsedHours = estimatedHours
            };
        }

        public static AssignmentRuntimeState FromSaveData(AssignmentSaveData data)
        {
            GameDateTime estimatedCompletion = data.EstimatedCompletionDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.EstimatedCompletionDateTotalElapsedHours.Value)
                : default;

            return new AssignmentRuntimeState
            {
                Id                      = data.Id,
                Type                    = Enum.Parse<AssignmentType>(data.Type),
                TargetType              = Enum.Parse<AssignmentTargetType>(data.TargetType),
                TargetId                = data.TargetId,
                TeamId                  = data.TeamId,
                ProgressPercent         = data.ProgressPercent,
                RawProgressPoints       = data.RawProgressPoints,
                StartDate               = GameDateTime.FromTotalHours(data.StartDateTotalElapsedHours),
                EstimatedCompletionDate = estimatedCompletion
            };
        }
    }
}
