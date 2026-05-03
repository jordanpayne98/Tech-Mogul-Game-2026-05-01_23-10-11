namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the employee and recruitment system.
    /// Consumed by EmployeeService, EmployeeTickProcessor, and related use cases
    /// without depending on Infrastructure.
    /// All salary values are in minor currency units (e.g. pence for GBP).
    /// </summary>
    public interface IEmployeeTuning
    {
        // ─── Morale / Burnout ─────────────────────────────────────────────────────

        /// <summary>Starting morale for a newly hired employee (0–100).</summary>
        int DefaultMorale { get; }

        /// <summary>Starting loyalty for a newly hired employee (0–100).</summary>
        int DefaultLoyalty { get; }

        /// <summary>Morale value below which an employee is considered low-morale.</summary>
        int MoraleLowThreshold { get; }

        /// <summary>Burnout risk value above which an employee is considered at high burnout risk.</summary>
        int BurnoutHighThreshold { get; }

        /// <summary>Morale lost per day when the employee is overworked.</summary>
        float MoraleDecayPerOverworkDay { get; }

        /// <summary>Burnout risk gained per day when the employee is overworked.</summary>
        float BurnoutAccumulationPerOverworkDay { get; }

        /// <summary>Burnout risk recovered per day when the employee is resting (not overworked).</summary>
        float BurnoutRecoveryPerRestDay { get; }

        /// <summary>Loyalty gained per month for active employees.</summary>
        float LoyaltyGrowthPerMonth { get; }

        // ─── Offer Acceptance ─────────────────────────────────────────────────────

        /// <summary>Base probability (0–100) that a candidate accepts any offer before modifiers.</summary>
        int BaseOfferAcceptancePercent { get; }

        /// <summary>Weight applied to the salary delta component of the acceptance formula.</summary>
        int SalaryWeightPercent { get; }

        /// <summary>Weight applied to the company reputation component of the acceptance formula.</summary>
        int ReputationWeightPercent { get; }

        /// <summary>Weight applied to the role-fit score component of the acceptance formula.</summary>
        int RoleFitWeightPercent { get; }

        /// <summary>Weight applied to the growth opportunity component of the acceptance formula.</summary>
        int GrowthWeightPercent { get; }

        // ─── Candidate Generation ─────────────────────────────────────────────────

        /// <summary>Number of candidates generated when recruitment is initialized for a new save.</summary>
        int InitialCandidatePoolSize { get; }

        /// <summary>Number of new candidates added during each monthly pool refresh.</summary>
        int CandidatePoolRefreshCount { get; }

        /// <summary>Minimum skill value assigned during candidate generation (0–100).</summary>
        int MinSkillValue { get; }

        /// <summary>Maximum skill value assigned during candidate generation (0–100).</summary>
        int MaxSkillValue { get; }

        /// <summary>Minimum potential lower bound assigned during candidate generation (0–100).</summary>
        int MinPotential { get; }

        /// <summary>Maximum potential upper bound assigned during candidate generation (0–100).</summary>
        int MaxPotential { get; }

        /// <summary>Minimum confidence level assigned during candidate generation (0–100).</summary>
        int MinCandidateConfidence { get; }

        /// <summary>Maximum confidence level assigned during candidate generation (0–100).</summary>
        int MaxCandidateConfidence { get; }

        /// <summary>Number of personality traits assigned to each generated candidate.</summary>
        int TraitsPerCandidate { get; }

        // ─── Salary Ranges (monthly minor units) ──────────────────────────────────

        /// <summary>Minimum monthly salary expectation for Junior-seniority candidates in minor currency units.</summary>
        long JuniorSalaryMinMonthlyMinorUnits { get; }

        /// <summary>Maximum monthly salary expectation for Junior-seniority candidates in minor currency units.</summary>
        long JuniorSalaryMaxMonthlyMinorUnits { get; }

        /// <summary>Minimum monthly salary expectation for Mid-seniority candidates in minor currency units.</summary>
        long MidSalaryMinMonthlyMinorUnits { get; }

        /// <summary>Maximum monthly salary expectation for Mid-seniority candidates in minor currency units.</summary>
        long MidSalaryMaxMonthlyMinorUnits { get; }

        /// <summary>Minimum monthly salary expectation for Senior-seniority candidates in minor currency units.</summary>
        long SeniorSalaryMinMonthlyMinorUnits { get; }

        /// <summary>Maximum monthly salary expectation for Senior-seniority candidates in minor currency units.</summary>
        long SeniorSalaryMaxMonthlyMinorUnits { get; }

        /// <summary>Minimum monthly salary expectation for Lead-seniority candidates in minor currency units.</summary>
        long LeadSalaryMinMonthlyMinorUnits { get; }

        /// <summary>Maximum monthly salary expectation for Lead-seniority candidates in minor currency units.</summary>
        long LeadSalaryMaxMonthlyMinorUnits { get; }

        /// <summary>Minimum monthly salary expectation for Principal-seniority candidates in minor currency units.</summary>
        long PrincipalSalaryMinMonthlyMinorUnits { get; }

        /// <summary>Maximum monthly salary expectation for Principal-seniority candidates in minor currency units.</summary>
        long PrincipalSalaryMaxMonthlyMinorUnits { get; }

        // ─── Employee Defaults ────────────────────────────────────────────────────

        /// <summary>Minimum ambition value assigned to a newly hired employee (0–100).</summary>
        int DefaultAmbitionMin { get; }

        /// <summary>Maximum ambition value assigned to a newly hired employee (0–100).</summary>
        int DefaultAmbitionMax { get; }
    }
}
