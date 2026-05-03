using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Contract
{
    /// <summary>
    /// Stateless concrete implementation of <see cref="IContractService"/>.
    /// Handles procedural contract generation, board management, progress derivation,
    /// milestone tracking, quality scoring, deadline enforcement, outcome determination,
    /// payout computation, and reputation change computation.
    ///
    /// Stateless — holds no state. All state is received as parameters.
    /// Does not publish events. Does not mutate FinanceRuntimeState or CompanyRuntimeState.
    ///
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public sealed class ContractService : IContractService
    {
        // ── Placeholder client names used during generation ────────────────────────
        // [Placeholder] Replace with a GDD-defined client name system when available.

        private static readonly string[] PlaceholderClientNames =
        {
            "Acme Corp",
            "GlobalTech",
            "Nexus Ltd",
            "Pinnacle Systems",
            "Vantage Digital",
            "Meridian Group",
            "Axion Solutions",
            "CoreBridge Inc",
            "Stratos Partners",
            "Luminary Works"
        };

        // ── Generation ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public ContractProfile GenerateContract(
            IContractTuning tuning,
            GameDateTime    currentDate,
            Random          random)
        {
            // 1. Stable GUID ID
            string id = Guid.NewGuid().ToString("N");

            // 2. Random ContractType
            var allTypes = (ContractType[])Enum.GetValues(typeof(ContractType));
            var type     = allTypes[random.Next(allTypes.Length)];

            // 3. Random ContractDifficulty
            var allDifficulties = (ContractDifficulty[])Enum.GetValues(typeof(ContractDifficulty));
            var difficulty       = allDifficulties[random.Next(allDifficulties.Length)];

            // 4. Client name [Placeholder]
            string clientName = PlaceholderClientNames[random.Next(PlaceholderClientNames.Length)];

            // 5. Required roles — pick distinct values
            var allRoles       = (EmployeeRole[])Enum.GetValues(typeof(EmployeeRole));
            int roleCount      = random.Next(tuning.ContractMinRequiredRoles, tuning.ContractMaxRequiredRoles + 1);
            var requiredRoles  = PickDistinct(allRoles, roleCount, random);

            // 6. Required skills — pick distinct values
            var allSkills       = (SkillCategory[])Enum.GetValues(typeof(SkillCategory));
            int skillCount      = random.Next(tuning.ContractMinRequiredSkills, tuning.ContractMaxRequiredSkills + 1);
            var requiredSkills  = PickDistinct(allSkills, skillCount, random);

            // 7. Base payment in minor units
            // ContractBasePaymentMin/Max are already minor units from IContractTuning.
            // We work in major units for the random roll to avoid long-range Next issues,
            // then convert. Both are stored as minor units by the property, so convert back.
            long minMinor = tuning.ContractBasePaymentMinMinorUnits;
            long maxMinor = tuning.ContractBasePaymentMaxMinorUnits;
            // Roll in major units to stay within int range, then convert.
            int  minMajor = (int)(minMinor / 100L);
            int  maxMajor = (int)(maxMinor / 100L);
            long baseMajor = random.Next(minMajor, maxMajor + 1);
            long basePaymentMinorUnits = ApplyDifficultyPaymentMultiplier(
                baseMajor * 100L, difficulty, tuning);

            // Round to nearest 100 minor units (nearest £1)
            basePaymentMinorUnits = ((basePaymentMinorUnits + 50L) / 100L) * 100L;

            // 8. Excellent bonus
            long excellentBonusMinorUnits = basePaymentMinorUnits * tuning.ContractExcellentBonusPercent / 100L;

            // 9. Failure payment
            long failurePaymentMinorUnits = basePaymentMinorUnits * tuning.ContractFailurePaymentPercent / 100L;

            // 10. Deadline
            float deadlineMultiplier = GetDeadlineMultiplier(difficulty, tuning);
            int   deadlineDays       = (int)(tuning.ContractBaseDeadlineDays * deadlineMultiplier);
            deadlineDays             = Math.Max(1, deadlineDays);
            GameDateTime deadline    = currentDate.AddHours(deadlineDays * GameDateTime.HoursPerDay);

            // 11. Milestones
            int milestoneCount = random.Next(tuning.ContractMinMilestones, tuning.ContractMaxMilestones + 1);

            // 12. Quality target
            int qualityTarget = random.Next(tuning.ContractMinQualityTarget, tuning.ContractMaxQualityTarget + 1);

            // 13. Dates
            GameDateTime postedDate = currentDate;
            GameDateTime expiryDate = currentDate.AddHours(tuning.ContractExpiryDays * GameDateTime.HoursPerDay);

            return new ContractProfile
            {
                Id                       = id,
                ClientName               = clientName,
                Type                     = type,
                Difficulty               = difficulty,
                RequiredRoles            = requiredRoles,
                RequiredSkills           = requiredSkills,
                PostedDate               = postedDate,
                ExpiryDate               = expiryDate,
                Deadline                 = deadline,
                BasePaymentMinorUnits    = basePaymentMinorUnits,
                ExcellentBonusMinorUnits = excellentBonusMinorUnits,
                FailurePaymentMinorUnits = failurePaymentMinorUnits,
                QualityTarget            = qualityTarget,
                MilestoneCount           = milestoneCount
            };
        }

        /// <inheritdoc/>
        public List<ContractProfile> GenerateContracts(
            int             count,
            IContractTuning tuning,
            GameDateTime    currentDate,
            Random          random)
        {
            var profiles = new List<ContractProfile>(count);

            for (int i = 0; i < count; i++)
            {
                profiles.Add(GenerateContract(tuning, currentDate, random));
            }

            return profiles;
        }

        /// <inheritdoc/>
        public ContractRuntimeState CreateContractRuntimeState(string contractId)
        {
            return new ContractRuntimeState
            {
                ContractId              = contractId,
                Status                  = ContractStatus.Available,
                Outcome                 = ContractOutcome.None,
                AssignedTeamId          = null,
                ProgressPercent         = 0,
                MilestonesCompleted     = 0,
                QualityScore            = 0,
                AcceptedDate            = null,
                CompletedDate           = null,
                PaymentDueMinorUnits    = 0
            };
        }

        // ── Board ─────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public List<string> GetExpiredContractIds(
            List<ContractProfile>      profiles,
            List<ContractRuntimeState> states,
            GameDateTime               currentDate)
        {
            var expired = new List<string>();

            foreach (var state in states)
            {
                if (state.Status != ContractStatus.Available)
                {
                    continue;
                }

                var profile = profiles.FirstOrDefault(p => p.Id == state.ContractId);

                if (profile == null)
                {
                    continue;
                }

                // Expired when current date is strictly after the expiry date.
                if (currentDate.TotalElapsedHours > profile.ExpiryDate.TotalElapsedHours)
                {
                    expired.Add(state.ContractId);
                }
            }

            return expired;
        }

        // ── Progress ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ComputeContractRequiredPoints(ContractProfile profile, IContractTuning tuning)
        {
            float multiplier = GetWorkMultiplier(profile.Difficulty, tuning);
            float required   = tuning.ContractBaseWorkHours * multiplier;

            return Math.Max(1, (int)required);
        }

        /// <inheritdoc/>
        public int DeriveProgressPercent(int rawProgressPoints, int requiredPoints)
        {
            if (requiredPoints <= 0)
            {
                return 100;
            }

            return Math.Clamp(rawProgressPoints * 100 / requiredPoints, 0, 100);
        }

        // ── Milestones ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ComputeCompletedMilestones(int progressPercent, int totalMilestones)
        {
            if (totalMilestones <= 0)
            {
                return 0;
            }

            int completed = 0;

            for (int n = 1; n <= totalMilestones; n++)
            {
                int threshold = n * 100 / totalMilestones;

                if (progressPercent >= threshold)
                {
                    completed = n;
                }
                else
                {
                    break;
                }
            }

            return completed;
        }

        // ── Quality ───────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public int ComputeQualityScore(
            ContractProfile            profile,
            List<EmployeeRuntimeState> teamMembers,
            int                        elapsedDays,
            int                        deadlineDays,
            IContractTuning            tuning)
        {
            float roleCoverage = ComputeRoleCoverage(teamMembers, profile.RequiredRoles);
            float skillFit     = ComputeSkillFit(teamMembers, profile.RequiredSkills);
            float timeRatio    = ComputeTimeRatio(elapsedDays, deadlineDays);

            int totalWeight = tuning.ContractQualityRoleCoverageWeight
                            + tuning.ContractQualitySkillFitWeight
                            + tuning.ContractQualityTimeRatioWeight;

            if (totalWeight <= 0)
            {
                return 0;
            }

            int qualityScore = (int)Math.Floor(
                (roleCoverage * tuning.ContractQualityRoleCoverageWeight
               + skillFit     * tuning.ContractQualitySkillFitWeight
               + timeRatio    * tuning.ContractQualityTimeRatioWeight)
               / totalWeight);

            return Math.Clamp(qualityScore, 0, 100);
        }

        /// <inheritdoc/>
        public float ComputeRoleCoverage(
            List<EmployeeRuntimeState> teamMembers,
            List<EmployeeRole>         requiredRoles)
        {
            if (requiredRoles == null || requiredRoles.Count == 0)
            {
                return 100f;
            }

            if (teamMembers == null || teamMembers.Count == 0)
            {
                return 0f;
            }

            int coveredCount = 0;

            foreach (var role in requiredRoles)
            {
                bool isCovered = teamMembers.Any(m => m.Role == role);

                if (isCovered)
                {
                    coveredCount++;
                }
            }

            return (float)coveredCount / requiredRoles.Count * 100f;
        }

        /// <inheritdoc/>
        public float ComputeSkillFit(
            List<EmployeeRuntimeState> teamMembers,
            List<SkillCategory>        requiredSkills)
        {
            if (requiredSkills == null || requiredSkills.Count == 0)
            {
                return 100f;
            }

            if (teamMembers == null || teamMembers.Count == 0)
            {
                return 0f;
            }

            float totalHighestScore = 0f;

            foreach (var skill in requiredSkills)
            {
                int highestScore = 0;

                foreach (var member in teamMembers)
                {
                    if (member.Skills != null && member.Skills.TryGetValue(skill, out int score))
                    {
                        if (score > highestScore)
                        {
                            highestScore = score;
                        }
                    }
                }

                totalHighestScore += highestScore;
            }

            return totalHighestScore / requiredSkills.Count;
        }

        /// <inheritdoc/>
        public float ComputeTimeRatio(int elapsedDays, int deadlineDays)
        {
            if (deadlineDays <= 0)
            {
                return 0f;
            }

            return Math.Clamp(100f - (elapsedDays * 100f / deadlineDays), 0f, 100f);
        }

        // ── Outcome ───────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public ContractOutcome DetermineOutcome(int qualityScore, IContractTuning tuning)
        {
            if (qualityScore >= tuning.QualityBonusThreshold)
            {
                return ContractOutcome.Excellent;
            }

            if (qualityScore >= tuning.ContractMinQualityForAccepted)
            {
                return ContractOutcome.Accepted;
            }

            return ContractOutcome.Failed;
        }

        /// <inheritdoc/>
        public bool IsDeadlineExceeded(ContractProfile profile, GameDateTime currentDate)
        {
            return currentDate.TotalElapsedHours > profile.Deadline.TotalElapsedHours;
        }

        // ── Payout ────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public long ComputePaymentDue(
            ContractProfile profile,
            ContractOutcome outcome,
            IContractTuning tuning)
        {
            switch (outcome)
            {
                case ContractOutcome.Failed:
                    return profile.FailurePaymentMinorUnits;

                case ContractOutcome.Accepted:
                    return profile.BasePaymentMinorUnits;

                case ContractOutcome.Excellent:
                    return profile.BasePaymentMinorUnits + profile.ExcellentBonusMinorUnits;

                default:
                    return 0L;
            }
        }

        // ── Reputation ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public float ComputeReputationChange(ContractOutcome outcome, IContractTuning tuning)
        {
            switch (outcome)
            {
                case ContractOutcome.Excellent:
                    return tuning.ReputationGainPerContract * 1.5f;

                case ContractOutcome.Accepted:
                    return tuning.ReputationGainPerContract;

                case ContractOutcome.Failed:
                    return -tuning.ReputationLossPerFailedContract;

                default:
                    return 0f;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private static float GetWorkMultiplier(ContractDifficulty difficulty, IContractTuning tuning)
        {
            switch (difficulty)
            {
                case ContractDifficulty.Standard: return tuning.ContractStandardWorkMultiplier;
                case ContractDifficulty.Hard:     return tuning.ContractHardWorkMultiplier;
                case ContractDifficulty.Expert:   return tuning.ContractExpertWorkMultiplier;
                default:                          return 1.0f; // Easy
            }
        }

        private static float GetDeadlineMultiplier(ContractDifficulty difficulty, IContractTuning tuning)
        {
            switch (difficulty)
            {
                case ContractDifficulty.Standard: return tuning.ContractStandardDeadlineMultiplier;
                case ContractDifficulty.Hard:     return tuning.ContractHardDeadlineMultiplier;
                case ContractDifficulty.Expert:   return tuning.ContractExpertDeadlineMultiplier;
                default:                          return 1.0f; // Easy
            }
        }

        private static long ApplyDifficultyPaymentMultiplier(
            long               baseMinorUnits,
            ContractDifficulty difficulty,
            IContractTuning    tuning)
        {
            float multiplier;

            switch (difficulty)
            {
                case ContractDifficulty.Standard: multiplier = tuning.ContractStandardPaymentMultiplier; break;
                case ContractDifficulty.Hard:     multiplier = tuning.ContractHardPaymentMultiplier;     break;
                case ContractDifficulty.Expert:   multiplier = tuning.ContractExpertPaymentMultiplier;   break;
                default:                          multiplier = 1.0f;                                     break;
            }

            return (long)(baseMinorUnits * multiplier);
        }

        private static List<T> PickDistinct<T>(T[] source, int count, Random random)
        {
            // Guard against requesting more than available
            count = Math.Min(count, source.Length);

            var pool   = new List<T>(source);
            var result = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                int index = random.Next(pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }
    }
}
