using System;
using System.Collections.Generic;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Domain service interface for the contract system.
    /// Covers procedural generation, board management, progress derivation,
    /// milestone tracking, quality scoring, deadline enforcement, outcome determination,
    /// payout computation, and reputation change computation.
    ///
    /// Stateless — all state is received as parameters and returned as values.
    /// Does not publish events. Does not mutate FinanceRuntimeState or CompanyRuntimeState.
    ///
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public interface IContractService
    {
        // ── Generation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Generates a single contract profile with a stable GUID ID, randomized type, difficulty,
        /// requirements, payment, deadline, milestones, and quality target.
        /// </summary>
        ContractProfile GenerateContract(
            IContractTuning tuning,
            GameDateTime    currentDate,
            Random          random);

        /// <summary>
        /// Generates multiple contract profiles in a single call.
        /// </summary>
        List<ContractProfile> GenerateContracts(
            int             count,
            IContractTuning tuning,
            GameDateTime    currentDate,
            Random          random);

        /// <summary>
        /// Creates a fresh ContractRuntimeState for a given contract ID.
        /// Status = Available, all progress zeroed, PaymentDueMinorUnits = 0.
        /// </summary>
        ContractRuntimeState CreateContractRuntimeState(string contractId);

        // ── Board ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the stable IDs of Available contracts whose ExpiryDate is before currentDate.
        /// </summary>
        List<string> GetExpiredContractIds(
            List<ContractProfile>      profiles,
            List<ContractRuntimeState> states,
            GameDateTime               currentDate);

        // ── Progress ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the total raw progress points required to complete the contract.
        /// Applies formula.contract.work_required using difficulty multipliers from tuning.
        /// Returns at least 1.
        /// </summary>
        int ComputeContractRequiredPoints(ContractProfile profile, IContractTuning tuning);

        /// <summary>
        /// Derives progress percentage from raw progress points and required points.
        /// Result is clamped [0, 100]. Guards against division by zero.
        /// </summary>
        int DeriveProgressPercent(int rawProgressPoints, int requiredPoints);

        // ── Milestones ────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns how many milestones have been completed at the given progress level.
        /// Milestone N (1-based) triggers when progressPercent >= (N * 100 / totalMilestones).
        /// </summary>
        int ComputeCompletedMilestones(int progressPercent, int totalMilestones);

        // ── Quality ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes a quality score (0–100) for the contract at this point in time.
        /// Uses formula.contract.quality_score: weighted sum of role coverage, skill fit, time ratio.
        /// </summary>
        int ComputeQualityScore(
            ContractProfile           profile,
            List<EmployeeRuntimeState> teamMembers,
            int                        elapsedDays,
            int                        deadlineDays,
            IContractTuning            tuning);

        /// <summary>
        /// Computes role coverage as a value in [0, 100].
        /// Returns 100 when requiredRoles is empty (no requirements = fully covered).
        /// </summary>
        float ComputeRoleCoverage(
            List<EmployeeRuntimeState> teamMembers,
            List<EmployeeRole>         requiredRoles);

        /// <summary>
        /// Computes average skill fit as a value in [0, 100].
        /// For each required skill category, finds the highest score among team members.
        /// Returns 100 when requiredSkills is empty.
        /// </summary>
        float ComputeSkillFit(
            List<EmployeeRuntimeState> teamMembers,
            List<SkillCategory>        requiredSkills);

        /// <summary>
        /// Computes the time ratio component as a value in [0, 100].
        /// Higher = more time remaining = better. Clamped [0, 100]. Guards against division by zero.
        /// Formula: 100 - (elapsedDays * 100 / deadlineDays)
        /// </summary>
        float ComputeTimeRatio(int elapsedDays, int deadlineDays);

        // ── Outcome ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Determines the terminal outcome based on quality score and tuning thresholds.
        /// Excellent if qualityScore >= tuning.QualityBonusThreshold,
        /// Accepted if qualityScore >= tuning.ContractMinQualityForAccepted, else Failed.
        /// </summary>
        ContractOutcome DetermineOutcome(int qualityScore, IContractTuning tuning);

        /// <summary>
        /// Returns true when currentDate is strictly after the contract deadline.
        /// </summary>
        bool IsDeadlineExceeded(ContractProfile profile, GameDateTime currentDate);

        // ── Payout ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the payment due in minor currency units based on the outcome.
        /// Applies formula.contract.payout.
        /// Does NOT mutate FinanceRuntimeState — that is Plan 2H's responsibility.
        /// </summary>
        long ComputePaymentDue(ContractProfile profile, ContractOutcome outcome, IContractTuning tuning);

        // ── Reputation ────────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the reputation change for the given outcome.
        /// Applies formula.contract.reputation_change.
        /// Does NOT mutate CompanyRuntimeState.Reputation — that is Plan 2H's responsibility.
        /// </summary>
        float ComputeReputationChange(ContractOutcome outcome, IContractTuning tuning);
    }
}
