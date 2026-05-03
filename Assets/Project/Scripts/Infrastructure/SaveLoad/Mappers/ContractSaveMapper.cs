using System;
using System.Collections.Generic;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Contract;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Contract domain runtime types to and from their save data equivalents.
    /// Covers ContractProfile and ContractRuntimeState.
    /// All methods are static — this mapper holds no state.
    /// Nullable GameDateTime fields convert to nullable int via TotalElapsedHours.
    /// </summary>
    public static class ContractSaveMapper
    {
        // ─── ContractProfile ──────────────────────────────────────────────────────

        public static ContractSaveData ToSaveData(ContractProfile profile)
        {
            var requiredRoles = new List<string>(profile.RequiredRoles.Count);
            foreach (EmployeeRole role in profile.RequiredRoles)
            {
                requiredRoles.Add(role.ToString());
            }

            var requiredSkills = new List<string>(profile.RequiredSkills.Count);
            foreach (SkillCategory skill in profile.RequiredSkills)
            {
                requiredSkills.Add(skill.ToString());
            }

            return new ContractSaveData
            {
                Id                       = profile.Id,
                ClientName               = profile.ClientName,
                Type                     = profile.Type.ToString(),
                Difficulty               = profile.Difficulty.ToString(),
                RequiredRoles            = requiredRoles,
                RequiredSkills           = requiredSkills,
                PostedDateTotalElapsedHours  = profile.PostedDate.TotalElapsedHours,
                ExpiryDateTotalElapsedHours  = profile.ExpiryDate.TotalElapsedHours,
                DeadlineTotalElapsedHours    = profile.Deadline.TotalElapsedHours,
                BasePaymentMinorUnits        = profile.BasePaymentMinorUnits,
                ExcellentBonusMinorUnits     = profile.ExcellentBonusMinorUnits,
                FailurePaymentMinorUnits     = profile.FailurePaymentMinorUnits,
                QualityTarget                = profile.QualityTarget,
                MilestoneCount               = profile.MilestoneCount
            };
        }

        public static ContractProfile FromSaveData(ContractSaveData data)
        {
            var requiredRoles = new List<EmployeeRole>(data.RequiredRoles.Count);
            foreach (string roleStr in data.RequiredRoles)
            {
                requiredRoles.Add(Enum.Parse<EmployeeRole>(roleStr));
            }

            var requiredSkills = new List<SkillCategory>(data.RequiredSkills.Count);
            foreach (string skillStr in data.RequiredSkills)
            {
                requiredSkills.Add(Enum.Parse<SkillCategory>(skillStr));
            }

            return new ContractProfile
            {
                Id                    = data.Id,
                ClientName            = data.ClientName,
                Type                  = Enum.Parse<ContractType>(data.Type),
                Difficulty            = Enum.Parse<ContractDifficulty>(data.Difficulty),
                RequiredRoles         = requiredRoles,
                RequiredSkills        = requiredSkills,
                PostedDate            = GameDateTime.FromTotalHours(data.PostedDateTotalElapsedHours),
                ExpiryDate            = GameDateTime.FromTotalHours(data.ExpiryDateTotalElapsedHours),
                Deadline              = GameDateTime.FromTotalHours(data.DeadlineTotalElapsedHours),
                BasePaymentMinorUnits     = data.BasePaymentMinorUnits,
                ExcellentBonusMinorUnits  = data.ExcellentBonusMinorUnits,
                FailurePaymentMinorUnits  = data.FailurePaymentMinorUnits,
                QualityTarget             = data.QualityTarget,
                MilestoneCount            = data.MilestoneCount
            };
        }

        // ─── ContractRuntimeState ─────────────────────────────────────────────────

        public static ContractStateSaveData ToSaveData(ContractRuntimeState state)
        {
            int? acceptedHours = state.AcceptedDate.TotalElapsedHours == 0
                ? (int?)null
                : state.AcceptedDate.TotalElapsedHours;

            int? completedHours = state.CompletedDate.TotalElapsedHours == 0
                ? (int?)null
                : state.CompletedDate.TotalElapsedHours;

            return new ContractStateSaveData
            {
                ContractId                     = state.ContractId,
                Status                         = state.Status.ToString(),
                Outcome                        = state.Outcome.ToString(),
                AssignedTeamId                 = state.AssignedTeamId,
                ProgressPercent                = state.ProgressPercent,
                MilestonesCompleted            = state.MilestonesCompleted,
                QualityScore                   = state.QualityScore,
                AcceptedDateTotalElapsedHours  = acceptedHours,
                CompletedDateTotalElapsedHours = completedHours,
                PaymentDueMinorUnits           = state.PaymentDueMinorUnits
            };
        }

        public static ContractRuntimeState FromSaveData(ContractStateSaveData data)
        {
            GameDateTime acceptedDate = data.AcceptedDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.AcceptedDateTotalElapsedHours.Value)
                : default;

            GameDateTime completedDate = data.CompletedDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.CompletedDateTotalElapsedHours.Value)
                : default;

            return new ContractRuntimeState
            {
                ContractId          = data.ContractId,
                Status              = Enum.Parse<ContractStatus>(data.Status),
                Outcome             = Enum.Parse<ContractOutcome>(data.Outcome),
                AssignedTeamId      = data.AssignedTeamId,
                ProgressPercent     = data.ProgressPercent,
                MilestonesCompleted = data.MilestonesCompleted,
                QualityScore        = data.QualityScore,
                AcceptedDate        = acceptedDate,
                CompletedDate       = completedDate,
                PaymentDueMinorUnits = data.PaymentDueMinorUnits
            };
        }
    }
}
