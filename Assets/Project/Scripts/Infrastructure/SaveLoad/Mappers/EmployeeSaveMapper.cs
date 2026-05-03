using System;
using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Employee;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Employee domain runtime types to and from their save data equivalents.
    /// Covers EmployeeProfile, EmployeeRuntimeState, CandidateProfile, CandidateRuntimeState,
    /// JobPostProfile, JobPostRuntimeState, and RecruitmentRuntimeState.
    /// All methods are static — this mapper holds no state.
    /// Dictionary{SkillCategory, int} converts to Dictionary{string, int} using enum member names.
    /// List{EmployeeTrait} converts to List{string} using enum member names.
    /// </summary>
    public static class EmployeeSaveMapper
    {
        // ─── EmployeeProfile ──────────────────────────────────────────────────────

        public static EmployeeSaveData ToSaveData(EmployeeProfile profile)
        {
            var traits = new List<string>(profile.Traits.Count);
            foreach (EmployeeTrait trait in profile.Traits)
            {
                traits.Add(trait.ToString());
            }

            return new EmployeeSaveData
            {
                Id                        = profile.Id,
                Name                      = profile.Name,
                Traits                    = traits,
                WorkPreference            = profile.WorkPreference.ToString(),
                HireDateTotalElapsedHours = profile.HireDate.TotalElapsedHours,
                PotentialMin              = profile.PotentialMin,
                PotentialMax              = profile.PotentialMax
            };
        }

        public static EmployeeProfile FromSaveData(EmployeeSaveData data)
        {
            var traits = new List<EmployeeTrait>(data.Traits.Count);
            foreach (string traitStr in data.Traits)
            {
                traits.Add(Enum.Parse<EmployeeTrait>(traitStr));
            }

            return new EmployeeProfile
            {
                Id             = data.Id,
                Name           = data.Name,
                Traits         = traits,
                WorkPreference = Enum.Parse<WorkPreference>(data.WorkPreference),
                HireDate       = GameDateTime.FromTotalHours(data.HireDateTotalElapsedHours),
                PotentialMin   = data.PotentialMin,
                PotentialMax   = data.PotentialMax
            };
        }

        // ─── EmployeeRuntimeState ─────────────────────────────────────────────────

        public static EmployeeStateSaveData ToSaveData(EmployeeRuntimeState state)
        {
            var skills = new Dictionary<string, int>(state.Skills.Count);
            foreach (KeyValuePair<SkillCategory, int> pair in state.Skills)
            {
                skills[pair.Key.ToString()] = pair.Value;
            }

            return new EmployeeStateSaveData
            {
                EmployeeId    = state.EmployeeId,
                Role          = state.Role.ToString(),
                Seniority     = state.Seniority.ToString(),
                SalaryMinorUnits = state.SalaryMinorUnits,
                Skills        = skills,
                CurrentAbility = state.CurrentAbility,
                Morale        = state.Morale,
                BurnoutRisk   = state.BurnoutRisk,
                Loyalty       = state.Loyalty,
                Ambition      = state.Ambition,
                CurrentTeamId = state.CurrentTeamId,
                Status        = state.Status.ToString()
            };
        }

        public static EmployeeRuntimeState FromSaveData(EmployeeStateSaveData data)
        {
            var skills = new Dictionary<SkillCategory, int>(data.Skills.Count);
            foreach (KeyValuePair<string, int> pair in data.Skills)
            {
                skills[Enum.Parse<SkillCategory>(pair.Key)] = pair.Value;
            }

            return new EmployeeRuntimeState
            {
                EmployeeId     = data.EmployeeId,
                Role           = Enum.Parse<EmployeeRole>(data.Role),
                Seniority      = Enum.Parse<Seniority>(data.Seniority),
                SalaryMinorUnits = data.SalaryMinorUnits,
                Skills         = skills,
                CurrentAbility = data.CurrentAbility,
                Morale         = data.Morale,
                BurnoutRisk    = data.BurnoutRisk,
                Loyalty        = data.Loyalty,
                Ambition       = data.Ambition,
                CurrentTeamId  = data.CurrentTeamId,
                Status         = Enum.Parse<EmploymentStatus>(data.Status)
            };
        }

        // ─── CandidateProfile ─────────────────────────────────────────────────────

        public static CandidateSaveData ToSaveData(CandidateProfile profile)
        {
            var visibleSkills = new Dictionary<string, int>(profile.VisibleSkills.Count);
            foreach (KeyValuePair<SkillCategory, int> pair in profile.VisibleSkills)
            {
                visibleSkills[pair.Key.ToString()] = pair.Value;
            }

            var traits = new List<string>(profile.Traits.Count);
            foreach (EmployeeTrait trait in profile.Traits)
            {
                traits.Add(trait.ToString());
            }

            return new CandidateSaveData
            {
                Id                                    = profile.Id,
                Name                                  = profile.Name,
                Role                                  = profile.Role.ToString(),
                Seniority                             = profile.Seniority.ToString(),
                SalaryExpectationMinorUnits           = profile.SalaryExpectationMinorUnits,
                VisibleSkills                         = visibleSkills,
                CurrentAbilityEstimate                = profile.CurrentAbilityEstimate,
                PotentialMin                          = profile.PotentialMin,
                PotentialMax                          = profile.PotentialMax,
                Traits                                = traits,
                WorkPreference                        = profile.WorkPreference.ToString(),
                AvailabilityDateTotalElapsedHours     = profile.AvailabilityDate.TotalElapsedHours,
                ConfidenceLevel                       = profile.ConfidenceLevel
            };
        }

        public static CandidateProfile FromSaveData(CandidateSaveData data)
        {
            var visibleSkills = new Dictionary<SkillCategory, int>(data.VisibleSkills.Count);
            foreach (KeyValuePair<string, int> pair in data.VisibleSkills)
            {
                visibleSkills[Enum.Parse<SkillCategory>(pair.Key)] = pair.Value;
            }

            var traits = new List<EmployeeTrait>(data.Traits.Count);
            foreach (string traitStr in data.Traits)
            {
                traits.Add(Enum.Parse<EmployeeTrait>(traitStr));
            }

            return new CandidateProfile
            {
                Id                          = data.Id,
                Name                        = data.Name,
                Role                        = Enum.Parse<EmployeeRole>(data.Role),
                Seniority                   = Enum.Parse<Seniority>(data.Seniority),
                SalaryExpectationMinorUnits = data.SalaryExpectationMinorUnits,
                VisibleSkills               = visibleSkills,
                CurrentAbilityEstimate      = data.CurrentAbilityEstimate,
                PotentialMin                = data.PotentialMin,
                PotentialMax                = data.PotentialMax,
                Traits                      = traits,
                WorkPreference              = Enum.Parse<WorkPreference>(data.WorkPreference),
                AvailabilityDate            = GameDateTime.FromTotalHours(data.AvailabilityDateTotalElapsedHours),
                ConfidenceLevel             = data.ConfidenceLevel
            };
        }

        // ─── CandidateRuntimeState ────────────────────────────────────────────────

        public static CandidateStateSaveData ToSaveData(CandidateRuntimeState state)
        {
            return new CandidateStateSaveData
            {
                CandidateId   = state.CandidateId,
                OfferStatus   = state.OfferStatus.ToString(),
                InterestLevel = state.InterestLevel
            };
        }

        public static CandidateRuntimeState FromSaveData(CandidateStateSaveData data)
        {
            return new CandidateRuntimeState
            {
                CandidateId   = data.CandidateId,
                OfferStatus   = Enum.Parse<OfferStatus>(data.OfferStatus),
                InterestLevel = data.InterestLevel
            };
        }

        // ─── JobPostProfile ───────────────────────────────────────────────────────

        public static JobPostSaveData ToSaveData(JobPostProfile profile)
        {
            var requiredSkills = new List<string>(profile.RequiredSkills.Count);
            foreach (SkillCategory skill in profile.RequiredSkills)
            {
                requiredSkills.Add(skill.ToString());
            }

            var preferredSkills = new List<string>(profile.PreferredSkills.Count);
            foreach (SkillCategory skill in profile.PreferredSkills)
            {
                preferredSkills.Add(skill.ToString());
            }

            return new JobPostSaveData
            {
                Id                             = profile.Id,
                Role                           = profile.Role.ToString(),
                Seniority                      = profile.Seniority.ToString(),
                SalaryRangeMinMinorUnits       = profile.SalaryRangeMinMinorUnits,
                SalaryRangeMaxMinorUnits       = profile.SalaryRangeMaxMinorUnits,
                RequiredSkills                 = requiredSkills,
                PreferredSkills                = preferredSkills,
                WorkPreference                 = profile.WorkPreference.ToString(),
                CompanyPitch                   = profile.CompanyPitch,
                HiringBudgetMinorUnits         = profile.HiringBudgetMinorUnits,
                CreatedDateTotalElapsedHours   = profile.CreatedDate.TotalElapsedHours
            };
        }

        public static JobPostProfile FromSaveData(JobPostSaveData data)
        {
            var requiredSkills = new List<SkillCategory>(data.RequiredSkills.Count);
            foreach (string skillStr in data.RequiredSkills)
            {
                requiredSkills.Add(Enum.Parse<SkillCategory>(skillStr));
            }

            var preferredSkills = new List<SkillCategory>(data.PreferredSkills.Count);
            foreach (string skillStr in data.PreferredSkills)
            {
                preferredSkills.Add(Enum.Parse<SkillCategory>(skillStr));
            }

            return new JobPostProfile
            {
                Id                       = data.Id,
                Role                     = Enum.Parse<EmployeeRole>(data.Role),
                Seniority                = Enum.Parse<Seniority>(data.Seniority),
                SalaryRangeMinMinorUnits = data.SalaryRangeMinMinorUnits,
                SalaryRangeMaxMinorUnits = data.SalaryRangeMaxMinorUnits,
                RequiredSkills           = requiredSkills,
                PreferredSkills          = preferredSkills,
                WorkPreference           = Enum.Parse<WorkPreference>(data.WorkPreference),
                CompanyPitch             = data.CompanyPitch,
                HiringBudgetMinorUnits   = data.HiringBudgetMinorUnits,
                CreatedDate              = GameDateTime.FromTotalHours(data.CreatedDateTotalElapsedHours)
            };
        }

        // ─── JobPostRuntimeState ──────────────────────────────────────────────────

        public static JobPostStateSaveData ToSaveData(JobPostRuntimeState state)
        {
            return new JobPostStateSaveData
            {
                JobPostId = state.JobPostId,
                Status    = state.Status.ToString()
            };
        }

        public static JobPostRuntimeState FromSaveData(JobPostStateSaveData data)
        {
            return new JobPostRuntimeState
            {
                JobPostId = data.JobPostId,
                Status    = Enum.Parse<JobPostStatus>(data.Status)
            };
        }

        // ─── RecruitmentRuntimeState ──────────────────────────────────────────────

        public static RecruitmentSaveData ToSaveData(RecruitmentRuntimeState state)
        {
            return new RecruitmentSaveData
            {
                CandidateIds                              = state.CandidateIds,
                JobPostIds                                = state.JobPostIds,
                LastCandidatePoolRefreshDateTotalElapsedHours = state.LastCandidatePoolRefreshDate.TotalElapsedHours
            };
        }

        public static RecruitmentRuntimeState FromSaveData(RecruitmentSaveData data)
        {
            return new RecruitmentRuntimeState
            {
                CandidateIds                  = data.CandidateIds,
                JobPostIds                    = data.JobPostIds,
                LastCandidatePoolRefreshDate  = GameDateTime.FromTotalHours(data.LastCandidatePoolRefreshDateTotalElapsedHours)
            };
        }
    }
}
