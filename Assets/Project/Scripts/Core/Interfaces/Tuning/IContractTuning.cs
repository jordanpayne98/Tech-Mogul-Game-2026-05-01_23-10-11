namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Tuning interface for the contract system.
    /// Covers board management, procedural generation parameters, quality scoring weights,
    /// outcome thresholds, and payout/reputation modifiers.
    /// Implemented by TuningConfig in Infrastructure.
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public interface IContractTuning
    {
        // ── Board Management ──────────────────────────────────────────────────────

        /// <summary>Number of contracts placed on the board when a new session is initialized.</summary>
        int InitialContractBoardSize { get; }

        /// <summary>Number of new contracts added to the board on each monthly refresh.</summary>
        int MonthlyContractRefreshCount { get; }

        /// <summary>Number of days an Available contract remains on the board before expiring.</summary>
        int ContractExpiryDays { get; }

        // ── Generation — Payment ──────────────────────────────────────────────────

        /// <summary>Minimum base payment for Easy contracts in minor currency units.</summary>
        long ContractBasePaymentMinMinorUnits { get; }

        /// <summary>Maximum base payment for Easy contracts in minor currency units.</summary>
        long ContractBasePaymentMaxMinorUnits { get; }

        /// <summary>Payment multiplier applied to Standard difficulty contracts.</summary>
        float ContractStandardPaymentMultiplier { get; }

        /// <summary>Payment multiplier applied to Hard difficulty contracts.</summary>
        float ContractHardPaymentMultiplier { get; }

        /// <summary>Payment multiplier applied to Expert difficulty contracts.</summary>
        float ContractExpertPaymentMultiplier { get; }

        /// <summary>Bonus percentage added on top of base payment for an Excellent outcome.</summary>
        int ContractExcellentBonusPercent { get; }

        /// <summary>Percentage of base payment received as partial payment on a Failed outcome.</summary>
        int ContractFailurePaymentPercent { get; }

        // ── Generation — Deadline ─────────────────────────────────────────────────

        /// <summary>Base deadline duration in days for Easy contracts.</summary>
        int ContractBaseDeadlineDays { get; }

        /// <summary>Deadline multiplier applied to Standard difficulty contracts.</summary>
        float ContractStandardDeadlineMultiplier { get; }

        /// <summary>Deadline multiplier applied to Hard difficulty contracts.</summary>
        float ContractHardDeadlineMultiplier { get; }

        /// <summary>Deadline multiplier applied to Expert difficulty contracts.</summary>
        float ContractExpertDeadlineMultiplier { get; }

        // ── Generation — Milestones ───────────────────────────────────────────────

        /// <summary>Minimum number of milestones on a generated contract.</summary>
        int ContractMinMilestones { get; }

        /// <summary>Maximum number of milestones on a generated contract.</summary>
        int ContractMaxMilestones { get; }

        // ── Generation — Requirements ─────────────────────────────────────────────

        /// <summary>Minimum number of required employee roles on a generated contract.</summary>
        int ContractMinRequiredRoles { get; }

        /// <summary>Maximum number of required employee roles on a generated contract.</summary>
        int ContractMaxRequiredRoles { get; }

        /// <summary>Minimum number of required skill categories on a generated contract.</summary>
        int ContractMinRequiredSkills { get; }

        /// <summary>Maximum number of required skill categories on a generated contract.</summary>
        int ContractMaxRequiredSkills { get; }

        // ── Generation — Work ─────────────────────────────────────────────────────

        /// <summary>Base work hours required to complete an Easy contract.</summary>
        int ContractBaseWorkHours { get; }

        /// <summary>Work multiplier applied to Standard difficulty contracts.</summary>
        float ContractStandardWorkMultiplier { get; }

        /// <summary>Work multiplier applied to Hard difficulty contracts.</summary>
        float ContractHardWorkMultiplier { get; }

        /// <summary>Work multiplier applied to Expert difficulty contracts.</summary>
        float ContractExpertWorkMultiplier { get; }

        // ── Generation — Quality Target ───────────────────────────────────────────

        /// <summary>Minimum quality target (0–100) assigned to a generated contract.</summary>
        int ContractMinQualityTarget { get; }

        /// <summary>Maximum quality target (0–100) assigned to a generated contract.</summary>
        int ContractMaxQualityTarget { get; }

        // ── Quality Scoring ───────────────────────────────────────────────────────

        /// <summary>Weight of role coverage component in quality score formula (see formula.contract.quality_score).</summary>
        int ContractQualityRoleCoverageWeight { get; }

        /// <summary>Weight of skill fit component in quality score formula.</summary>
        int ContractQualitySkillFitWeight { get; }

        /// <summary>Weight of time ratio component in quality score formula.</summary>
        int ContractQualityTimeRatioWeight { get; }

        // ── Outcome Thresholds ────────────────────────────────────────────────────

        /// <summary>Quality score threshold for an Excellent outcome. qualityScore >= this → Excellent.</summary>
        int QualityBonusThreshold { get; }

        /// <summary>Minimum quality score for an Accepted outcome. qualityScore >= this → Accepted, else Failed.</summary>
        int ContractMinQualityForAccepted { get; }

        // ── Payout / Reputation ───────────────────────────────────────────────────

        /// <summary>[Placeholder] Late delivery penalty as a percentage. Deferred — set to 0.</summary>
        int LateDeliveryPenaltyPercent { get; }

        /// <summary>Reputation points gained on a successful (Accepted or Excellent) contract.</summary>
        float ReputationGainPerContract { get; }

        /// <summary>Reputation points lost on a Failed contract.</summary>
        float ReputationLossPerFailedContract { get; }
    }
}
