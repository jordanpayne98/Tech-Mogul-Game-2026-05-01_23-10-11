using System.Collections.Generic;
using Project.Core.Runtime.Contract;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for contract-domain types.
    /// Checks required IDs, money non-negative, QualityTarget/QualityScore 0–100,
    /// ProgressPercent 0–100, MilestoneCount > 0, and enum validity.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class ContractDataValidator :
        IValidator<ContractProfile>,
        IValidator<ContractRuntimeState>
    {
        // -------------------------------------------------------------------------
        // ContractProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ContractProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ContractProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ContractProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ContractProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var clientNameIssue = ValidationRules.RequiredString(target.ClientName, "ClientName", ref_);
            if (clientNameIssue.HasValue) issues.Add(clientNameIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

            var difficultyIssue = ValidationRules.EnumDefined(target.Difficulty, "Difficulty", ref_);
            if (difficultyIssue.HasValue) issues.Add(difficultyIssue.Value);

            // Payment amounts must be non-negative.
            var basePaymentIssue = ValidationRules.NonNegative(target.BasePaymentMinorUnits, "BasePaymentMinorUnits", ref_);
            if (basePaymentIssue.HasValue) issues.Add(basePaymentIssue.Value);

            var bonusIssue = ValidationRules.NonNegative(target.ExcellentBonusMinorUnits, "ExcellentBonusMinorUnits", ref_);
            if (bonusIssue.HasValue) issues.Add(bonusIssue.Value);

            var failurePaymentIssue = ValidationRules.NonNegative(target.FailurePaymentMinorUnits, "FailurePaymentMinorUnits", ref_);
            if (failurePaymentIssue.HasValue) issues.Add(failurePaymentIssue.Value);

            // QualityTarget: 0–100.
            var qualityTargetIssue = ValidationRules.Range(target.QualityTarget, 0, 100, "QualityTarget", ref_);
            if (qualityTargetIssue.HasValue) issues.Add(qualityTargetIssue.Value);

            // MilestoneCount must be greater than 0.
            if (target.MilestoneCount <= 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.out_of_bounds",
                    ref_,
                    "MilestoneCount",
                    $"{ref_}.MilestoneCount value {target.MilestoneCount} must be greater than 0."));
            }

            // Date fields must not be null.
            if (target.PostedDate == null)
            {
                issues.Add(new ValidationIssue(ValidationSeverity.Error, "required.missing", ref_, "PostedDate", $"{ref_}.PostedDate is null."));
            }
            if (target.ExpiryDate == null)
            {
                issues.Add(new ValidationIssue(ValidationSeverity.Error, "required.missing", ref_, "ExpiryDate", $"{ref_}.ExpiryDate is null."));
            }
            if (target.Deadline == null)
            {
                issues.Add(new ValidationIssue(ValidationSeverity.Error, "required.missing", ref_, "Deadline", $"{ref_}.Deadline is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // ContractRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ContractRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ContractRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ContractRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ContractRuntimeState", target.ContractId);

            var idIssue = ValidationRules.RequiredString(target.ContractId, "ContractId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var statusIssue = ValidationRules.EnumDefined(target.Status, "Status", ref_);
            if (statusIssue.HasValue) issues.Add(statusIssue.Value);

            var outcomeIssue = ValidationRules.EnumDefined(target.Outcome, "Outcome", ref_);
            if (outcomeIssue.HasValue) issues.Add(outcomeIssue.Value);

            // ProgressPercent: 0–100.
            var progressIssue = ValidationRules.Range(target.ProgressPercent, 0, 100, "ProgressPercent", ref_);
            if (progressIssue.HasValue) issues.Add(progressIssue.Value);

            // QualityScore: 0–100.
            var qualityScoreIssue = ValidationRules.Range(target.QualityScore, 0, 100, "QualityScore", ref_);
            if (qualityScoreIssue.HasValue) issues.Add(qualityScoreIssue.Value);

            // MilestonesCompleted must be non-negative.
            if (target.MilestonesCompleted < 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    ref_,
                    "MilestonesCompleted",
                    $"{ref_}.MilestonesCompleted value {target.MilestonesCompleted} must be zero or greater."));
            }

            return ValidationResult.WithIssues(issues);
        }
    }
}
