using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Employee;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Employee
{
    /// <summary>
    /// Stateless concrete implementation of IEmployeeService.
    /// Handles candidate generation, offer evaluation, hire conversion, and daily/monthly processing.
    /// All randomness is provided via System.Random passed in as a parameter; this service never
    /// creates its own Random instance.
    ///
    /// MVP role subset: only the 8 first-playable roles are used in candidate generation.
    /// Remaining roles exist in the enum but are excluded from generation until unlocked.
    ///
    /// Seniority weighting (60/30/10): Junior+Mid / Senior / Lead+Principal.
    /// This is a generation heuristic hardcoded for the prototype.
    ///
    /// RoleFitScore and GrowthScore use [Placeholder] simplified values.
    /// Full evaluation requires job post matching and career path data (future plans).
    /// </summary>
    public sealed class EmployeeService : IEmployeeService
    {
        // ─── MVP playable roles (GDD_04.3) ────────────────────────────────────────

        private static readonly EmployeeRole[] PlayableRoles =
        {
            EmployeeRole.SoftwareEngineer,
            EmployeeRole.HardwareEngineer,
            EmployeeRole.ProductDesigner,
            EmployeeRole.QAEngineer,
            EmployeeRole.MarketingSpecialist,
            EmployeeRole.SupportSpecialist,
            EmployeeRole.InfrastructureEngineer,
            EmployeeRole.ProductManager
        };

        // ─── IEmployeeService ─────────────────────────────────────────────────────

        /// <inheritdoc/>
        public List<CandidateProfile> GenerateCandidates(
            int count,
            int startingCounter,
            IEmployeeTuning tuning,
            GameDateTime currentDate,
            Random random)
        {
            var candidates = new List<CandidateProfile>(count);

            for (int i = 0; i < count; i++)
            {
                string candidateId = $"candidate.{startingCounter + i}";
                EmployeeRole role  = PickRole(random);
                Seniority seniority = PickSeniority(random);

                long salaryMin = GetSalaryMin(seniority, tuning);
                long salaryMax = GetSalaryMax(seniority, tuning);
                long salaryExpectation = NextLongInRange(salaryMin, salaryMax, random);

                SkillCategory primarySkill = GetPrimarySkillForRole(role);
                SkillCategory[] secondarySkills = GetSecondarySkillsForRole(role);

                var visibleSkills = new Dictionary<SkillCategory, int>();
                visibleSkills[primarySkill] = random.Next(tuning.MinSkillValue, tuning.MaxSkillValue + 1);
                foreach (SkillCategory secondary in secondarySkills)
                {
                    if (!visibleSkills.ContainsKey(secondary))
                    {
                        visibleSkills[secondary] = random.Next(tuning.MinSkillValue, tuning.MaxSkillValue + 1);
                    }
                }

                int currentAbilityEstimate = random.Next(tuning.MinSkillValue, tuning.MaxSkillValue + 1);
                int potentialMin           = random.Next(tuning.MinPotential, tuning.MaxPotential + 1);
                int potentialMax           = Math.Min(100, potentialMin + random.Next(0, 20));
                int confidenceLevel        = random.Next(tuning.MinCandidateConfidence, tuning.MaxCandidateConfidence + 1);

                var traits = PickTraits(tuning.TraitsPerCandidate, random);
                var workPreference = (WorkPreference)random.Next(0, Enum.GetValues(typeof(WorkPreference)).Length);

                var profile = new CandidateProfile
                {
                    Id                       = candidateId,
                    Name                     = GeneratePlaceholderName(startingCounter + i, random),
                    Role                     = role,
                    Seniority                = seniority,
                    SalaryExpectationMinorUnits = salaryExpectation,
                    VisibleSkills            = visibleSkills,
                    CurrentAbilityEstimate   = currentAbilityEstimate,
                    PotentialMin             = potentialMin,
                    PotentialMax             = potentialMax,
                    Traits                   = traits,
                    WorkPreference           = workPreference,
                    AvailabilityDate         = currentDate,
                    ConfidenceLevel          = confidenceLevel
                };

                candidates.Add(profile);
            }

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[EmployeeService] Generated {count} candidates. Counter start: {startingCounter}.");

            return candidates;
        }

        /// <inheritdoc/>
        public List<CandidateRuntimeState> CreateCandidateStates(List<CandidateProfile> candidates)
        {
            var states = new List<CandidateRuntimeState>(candidates.Count);

            foreach (CandidateProfile candidate in candidates)
            {
                states.Add(new CandidateRuntimeState
                {
                    CandidateId   = candidate.Id,
                    OfferStatus   = OfferStatus.None,
                    InterestLevel = 50
                });
            }

            return states;
        }

        /// <inheritdoc/>
        public int EvaluateOfferAcceptance(
            CandidateProfile candidate,
            long offeredSalaryMinorUnits,
            int companyReputation,
            IEmployeeTuning tuning)
        {
            // Salary delta: percentage above/below candidate's expectation, clamped to [-50, +50].
            long salaryDeltaRaw = ((offeredSalaryMinorUnits - candidate.SalaryExpectationMinorUnits) * 100L)
                                / candidate.SalaryExpectationMinorUnits;
            int salaryDelta = Clamp((int)salaryDeltaRaw, -50, 50);

            // [Placeholder] Simplified role fit — always moderate.
            int roleFitScore = 50;

            // [Placeholder] Growth score based on seniority only (Junior/Mid prefer growth opportunities).
            int growthScore = (candidate.Seniority == Seniority.Junior || candidate.Seniority == Seniority.Mid)
                ? 50
                : 20;

            int acceptanceScore = tuning.BaseOfferAcceptancePercent
                + (salaryDelta * tuning.SalaryWeightPercent / 100)
                + (companyReputation * tuning.ReputationWeightPercent / 100)
                + (roleFitScore * tuning.RoleFitWeightPercent / 100)
                + (growthScore * tuning.GrowthWeightPercent / 100);

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[EmployeeService] EvaluateOfferAcceptance: candidate={candidate.Id} " +
                $"offeredSalary={offeredSalaryMinorUnits} expectedSalary={candidate.SalaryExpectationMinorUnits} " +
                $"salaryDelta={salaryDelta} reputation={companyReputation} " +
                $"roleFit={roleFitScore} growth={growthScore} score={acceptanceScore}");

            return acceptanceScore;
        }

        /// <inheritdoc/>
        public bool ResolveOffer(int acceptanceScore, Random random)
        {
            int clampedScore = Clamp(acceptanceScore, 5, 95);
            int roll         = random.Next(0, 100);
            return roll < clampedScore;
        }

        /// <inheritdoc/>
        public EmployeeProfile ConvertToEmployeeProfile(
            CandidateProfile candidate,
            GameDateTime hireDate,
            int nextEmployeeCounter)
        {
            string employeeId = $"employee.{nextEmployeeCounter}";

            var profile = new EmployeeProfile
            {
                Id             = employeeId,
                Name           = candidate.Name,
                Traits         = new List<EmployeeTrait>(candidate.Traits),
                WorkPreference = candidate.WorkPreference,
                HireDate       = hireDate,
                PotentialMin   = candidate.PotentialMin,
                PotentialMax   = candidate.PotentialMax
            };

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[EmployeeService] ConvertToEmployeeProfile: candidate={candidate.Id} → employee={employeeId} " +
                $"role={candidate.Role} seniority={candidate.Seniority}");

            return profile;
        }

        /// <inheritdoc/>
        public EmployeeRuntimeState CreateEmployeeState(
            CandidateProfile candidate,
            string employeeId,
            long offeredSalaryMinorUnits,
            IEmployeeTuning tuning,
            Random random)
        {
            var state = new EmployeeRuntimeState
            {
                EmployeeId       = employeeId,
                Role             = candidate.Role,
                Seniority        = candidate.Seniority,
                SalaryMinorUnits = offeredSalaryMinorUnits,
                // True skills are revealed at hire — copy from visible skills.
                Skills           = new Dictionary<SkillCategory, int>(candidate.VisibleSkills),
                CurrentAbility   = candidate.CurrentAbilityEstimate,
                Morale           = tuning.DefaultMorale,
                BurnoutRisk      = 0,
                Loyalty          = tuning.DefaultLoyalty,
                Ambition         = random.Next(tuning.DefaultAmbitionMin, tuning.DefaultAmbitionMax + 1),
                Status           = EmploymentStatus.Active,
                CurrentTeamId    = null
            };

            return state;
        }

        /// <inheritdoc/>
        public void ProcessDailyRecovery(EmployeeRuntimeState employee, IEmployeeTuning tuning)
        {
            if (employee.Status != EmploymentStatus.Active)
            {
                return;
            }

            if (employee.BurnoutRisk > 0)
            {
                employee.BurnoutRisk = Math.Max(0, employee.BurnoutRisk - (int)tuning.BurnoutRecoveryPerRestDay);
            }
        }

        /// <inheritdoc/>
        public void ProcessMonthlyLoyalty(EmployeeRuntimeState employee, IEmployeeTuning tuning)
        {
            if (employee.Status != EmploymentStatus.Active)
            {
                return;
            }

            employee.Loyalty = Math.Min(100, employee.Loyalty + (int)tuning.LoyaltyGrowthPerMonth);
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private static EmployeeRole PickRole(Random random)
        {
            return PlayableRoles[random.Next(0, PlayableRoles.Length)];
        }

        /// <summary>
        /// Weighted seniority selection: Junior+Mid 60%, Senior 30%, Lead+Principal 10%.
        /// Heuristic hardcoded for the prototype. Can be moved to tuning later.
        /// </summary>
        private static Seniority PickSeniority(Random random)
        {
            int roll = random.Next(0, 100);

            if (roll < 60)
            {
                // Junior or Mid with equal probability within the 60% band.
                return random.Next(0, 2) == 0 ? Seniority.Junior : Seniority.Mid;
            }

            if (roll < 90)
            {
                return Seniority.Senior;
            }

            // Lead or Principal with equal probability within the 10% band.
            return random.Next(0, 2) == 0 ? Seniority.Lead : Seniority.Principal;
        }

        private static SkillCategory GetPrimarySkillForRole(EmployeeRole role)
        {
            return role switch
            {
                EmployeeRole.SoftwareEngineer      => SkillCategory.Engineering,
                EmployeeRole.HardwareEngineer      => SkillCategory.Hardware,
                EmployeeRole.ProductDesigner       => SkillCategory.Design,
                EmployeeRole.ProductManager        => SkillCategory.Leadership,
                EmployeeRole.QAEngineer            => SkillCategory.QAReliability,
                EmployeeRole.InfrastructureEngineer => SkillCategory.Infrastructure,
                EmployeeRole.MarketingSpecialist   => SkillCategory.Marketing,
                EmployeeRole.SupportSpecialist     => SkillCategory.Support,
                _ => SkillCategory.Engineering
            };
        }

        private static SkillCategory[] GetSecondarySkillsForRole(EmployeeRole role)
        {
            return role switch
            {
                EmployeeRole.SoftwareEngineer       => new[] { SkillCategory.QAReliability, SkillCategory.Collaboration },
                EmployeeRole.HardwareEngineer       => new[] { SkillCategory.QAReliability, SkillCategory.Operations },
                EmployeeRole.ProductDesigner        => new[] { SkillCategory.Collaboration, SkillCategory.Research },
                EmployeeRole.ProductManager         => new[] { SkillCategory.Collaboration, SkillCategory.Marketing },
                EmployeeRole.QAEngineer             => new[] { SkillCategory.Engineering, SkillCategory.Support },
                EmployeeRole.InfrastructureEngineer => new[] { SkillCategory.Engineering, SkillCategory.Operations },
                EmployeeRole.MarketingSpecialist    => new[] { SkillCategory.Sales, SkillCategory.Collaboration },
                EmployeeRole.SupportSpecialist      => new[] { SkillCategory.Collaboration, SkillCategory.Operations },
                _ => new[] { SkillCategory.Collaboration, SkillCategory.Operations }
            };
        }

        private static long GetSalaryMin(Seniority seniority, IEmployeeTuning tuning)
        {
            return seniority switch
            {
                Seniority.Junior    => tuning.JuniorSalaryMinMonthlyMinorUnits,
                Seniority.Mid       => tuning.MidSalaryMinMonthlyMinorUnits,
                Seniority.Senior    => tuning.SeniorSalaryMinMonthlyMinorUnits,
                Seniority.Lead      => tuning.LeadSalaryMinMonthlyMinorUnits,
                Seniority.Principal => tuning.PrincipalSalaryMinMonthlyMinorUnits,
                _ => tuning.JuniorSalaryMinMonthlyMinorUnits
            };
        }

        private static long GetSalaryMax(Seniority seniority, IEmployeeTuning tuning)
        {
            return seniority switch
            {
                Seniority.Junior    => tuning.JuniorSalaryMaxMonthlyMinorUnits,
                Seniority.Mid       => tuning.MidSalaryMaxMonthlyMinorUnits,
                Seniority.Senior    => tuning.SeniorSalaryMaxMonthlyMinorUnits,
                Seniority.Lead      => tuning.LeadSalaryMaxMonthlyMinorUnits,
                Seniority.Principal => tuning.PrincipalSalaryMaxMonthlyMinorUnits,
                _ => tuning.JuniorSalaryMaxMonthlyMinorUnits
            };
        }

        private static List<EmployeeTrait> PickTraits(int count, Random random)
        {
            var allTraits = (EmployeeTrait[])Enum.GetValues(typeof(EmployeeTrait));
            var chosen = new List<EmployeeTrait>(count);
            var available = new List<EmployeeTrait>(allTraits);

            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int index = random.Next(0, available.Count);
                chosen.Add(available[index]);
                available.RemoveAt(index);
            }

            return chosen;
        }

        /// <summary>
        /// Generates a [Placeholder] display name for a candidate.
        /// Final implementation requires a proper name database.
        /// </summary>
        private static string GeneratePlaceholderName(int counter, Random random)
        {
            // [Placeholder] Minimal name generation until a proper name list is sourced.
            string[] firstNames = { "Alex", "Jordan", "Morgan", "Casey", "Sam", "Taylor", "Riley", "Drew" };
            string[] lastNames  = { "Smith", "Chen", "Park", "Patel", "Johnson", "Lee", "Brown", "Garcia" };
            return $"{firstNames[random.Next(0, firstNames.Length)]} {lastNames[random.Next(0, lastNames.Length)]}";
        }

        private static long NextLongInRange(long min, long max, Random random)
        {
            if (min >= max)
            {
                return min;
            }

            long range    = max - min;
            long sample   = (long)(random.NextDouble() * range);
            return min + sample;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }
    }
}
