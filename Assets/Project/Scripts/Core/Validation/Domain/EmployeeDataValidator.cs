using System.Collections.Generic;
using Project.Core.Runtime.Employee;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for employee-domain types.
    /// Checks required IDs, display name presence, skill values 0–100, salary non-negative,
    /// enum validity, null items in collections, and PotentialMin &lt;= PotentialMax.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class EmployeeDataValidator :
        IValidator<EmployeeProfile>,
        IValidator<CandidateProfile>,
        IValidator<JobPostProfile>,
        IValidator<EmployeeRuntimeState>,
        IValidator<CandidateRuntimeState>,
        IValidator<JobPostRuntimeState>,
        IValidator<RecruitmentRuntimeState>
    {
        // -------------------------------------------------------------------------
        // EmployeeProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(EmployeeProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("EmployeeProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "EmployeeProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("EmployeeProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var prefIssue = ValidationRules.EnumDefined(target.WorkPreference, "WorkPreference", ref_);
            if (prefIssue.HasValue) issues.Add(prefIssue.Value);

            // PotentialMin and PotentialMax: each 0–100, and Min must not exceed Max.
            var potMinIssue = ValidationRules.Range(target.PotentialMin, 0, 100, "PotentialMin", ref_);
            if (potMinIssue.HasValue) issues.Add(potMinIssue.Value);

            var potMaxIssue = ValidationRules.Range(target.PotentialMax, 0, 100, "PotentialMax", ref_);
            if (potMaxIssue.HasValue) issues.Add(potMaxIssue.Value);

            if (target.PotentialMin > target.PotentialMax)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.inverted",
                    ref_,
                    "PotentialMin",
                    $"{ref_}.PotentialMin ({target.PotentialMin}) must not exceed PotentialMax ({target.PotentialMax})."));
            }

            // HireDate must not be null.
            if (target.HireDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "HireDate",
                    $"{ref_}.HireDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CandidateProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CandidateProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CandidateProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CandidateProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CandidateProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var roleIssue = ValidationRules.EnumDefined(target.Role, "Role", ref_);
            if (roleIssue.HasValue) issues.Add(roleIssue.Value);

            var seniorityIssue = ValidationRules.EnumDefined(target.Seniority, "Seniority", ref_);
            if (seniorityIssue.HasValue) issues.Add(seniorityIssue.Value);

            var salaryIssue = ValidationRules.NonNegative(target.SalaryExpectationMinorUnits, "SalaryExpectationMinorUnits", ref_);
            if (salaryIssue.HasValue) issues.Add(salaryIssue.Value);

            // Visible skill values: 0–100 per skill.
            if (target.VisibleSkills != null)
            {
                foreach (var kvp in target.VisibleSkills)
                {
                    var skillIssue = ValidationRules.Range(kvp.Value, 0, 100, $"VisibleSkills[{kvp.Key}]", ref_);
                    if (skillIssue.HasValue) issues.Add(skillIssue.Value);
                }
            }

            var abilityIssue = ValidationRules.Range(target.CurrentAbilityEstimate, 0, 100, "CurrentAbilityEstimate", ref_);
            if (abilityIssue.HasValue) issues.Add(abilityIssue.Value);

            var potMinIssue = ValidationRules.Range(target.PotentialMin, 0, 100, "PotentialMin", ref_);
            if (potMinIssue.HasValue) issues.Add(potMinIssue.Value);

            var potMaxIssue = ValidationRules.Range(target.PotentialMax, 0, 100, "PotentialMax", ref_);
            if (potMaxIssue.HasValue) issues.Add(potMaxIssue.Value);

            if (target.PotentialMin > target.PotentialMax)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.inverted",
                    ref_,
                    "PotentialMin",
                    $"{ref_}.PotentialMin ({target.PotentialMin}) must not exceed PotentialMax ({target.PotentialMax})."));
            }

            var prefIssue = ValidationRules.EnumDefined(target.WorkPreference, "WorkPreference", ref_);
            if (prefIssue.HasValue) issues.Add(prefIssue.Value);

            var confidenceIssue = ValidationRules.Range(target.ConfidenceLevel, 0, 100, "ConfidenceLevel", ref_);
            if (confidenceIssue.HasValue) issues.Add(confidenceIssue.Value);

            // AvailabilityDate must not be null.
            if (target.AvailabilityDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "AvailabilityDate",
                    $"{ref_}.AvailabilityDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // JobPostProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(JobPostProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("JobPostProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "JobPostProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("JobPostProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var roleIssue = ValidationRules.EnumDefined(target.Role, "Role", ref_);
            if (roleIssue.HasValue) issues.Add(roleIssue.Value);

            var seniorityIssue = ValidationRules.EnumDefined(target.Seniority, "Seniority", ref_);
            if (seniorityIssue.HasValue) issues.Add(seniorityIssue.Value);

            var salaryMinIssue = ValidationRules.NonNegative(target.SalaryRangeMinMinorUnits, "SalaryRangeMinMinorUnits", ref_);
            if (salaryMinIssue.HasValue) issues.Add(salaryMinIssue.Value);

            var salaryMaxIssue = ValidationRules.NonNegative(target.SalaryRangeMaxMinorUnits, "SalaryRangeMaxMinorUnits", ref_);
            if (salaryMaxIssue.HasValue) issues.Add(salaryMaxIssue.Value);

            var budgetIssue = ValidationRules.NonNegative(target.HiringBudgetMinorUnits, "HiringBudgetMinorUnits", ref_);
            if (budgetIssue.HasValue) issues.Add(budgetIssue.Value);

            var prefIssue = ValidationRules.EnumDefined(target.WorkPreference, "WorkPreference", ref_);
            if (prefIssue.HasValue) issues.Add(prefIssue.Value);

            // CreatedDate must not be null.
            if (target.CreatedDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "CreatedDate",
                    $"{ref_}.CreatedDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // EmployeeRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(EmployeeRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("EmployeeRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "EmployeeRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("EmployeeRuntimeState", target.EmployeeId);

            var idIssue = ValidationRules.RequiredString(target.EmployeeId, "EmployeeId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var roleIssue = ValidationRules.EnumDefined(target.Role, "Role", ref_);
            if (roleIssue.HasValue) issues.Add(roleIssue.Value);

            var seniorityIssue = ValidationRules.EnumDefined(target.Seniority, "Seniority", ref_);
            if (seniorityIssue.HasValue) issues.Add(seniorityIssue.Value);

            var salaryIssue = ValidationRules.NonNegative(target.SalaryMinorUnits, "SalaryMinorUnits", ref_);
            if (salaryIssue.HasValue) issues.Add(salaryIssue.Value);

            // Skill values: 0–100 per skill category.
            if (target.Skills != null)
            {
                foreach (var kvp in target.Skills)
                {
                    var skillIssue = ValidationRules.Range(kvp.Value, 0, 100, $"Skills[{kvp.Key}]", ref_);
                    if (skillIssue.HasValue) issues.Add(skillIssue.Value);
                }
            }

            // 0–100 stats.
            var abilityIssue   = ValidationRules.Range(target.CurrentAbility, 0, 100, "CurrentAbility", ref_);
            if (abilityIssue.HasValue) issues.Add(abilityIssue.Value);

            var moraleIssue    = ValidationRules.Range(target.Morale, 0, 100, "Morale", ref_);
            if (moraleIssue.HasValue) issues.Add(moraleIssue.Value);

            var burnoutIssue   = ValidationRules.Range(target.BurnoutRisk, 0, 100, "BurnoutRisk", ref_);
            if (burnoutIssue.HasValue) issues.Add(burnoutIssue.Value);

            var loyaltyIssue   = ValidationRules.Range(target.Loyalty, 0, 100, "Loyalty", ref_);
            if (loyaltyIssue.HasValue) issues.Add(loyaltyIssue.Value);

            var ambitionIssue  = ValidationRules.Range(target.Ambition, 0, 100, "Ambition", ref_);
            if (ambitionIssue.HasValue) issues.Add(ambitionIssue.Value);

            var statusIssue = ValidationRules.EnumDefined(target.Status, "Status", ref_);
            if (statusIssue.HasValue) issues.Add(statusIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CandidateRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CandidateRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CandidateRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CandidateRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CandidateRuntimeState", target.CandidateId);

            var idIssue = ValidationRules.RequiredString(target.CandidateId, "CandidateId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var offerStatusIssue = ValidationRules.EnumDefined(target.OfferStatus, "OfferStatus", ref_);
            if (offerStatusIssue.HasValue) issues.Add(offerStatusIssue.Value);

            var interestIssue = ValidationRules.Range(target.InterestLevel, 0, 100, "InterestLevel", ref_);
            if (interestIssue.HasValue) issues.Add(interestIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // JobPostRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(JobPostRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("JobPostRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "JobPostRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("JobPostRuntimeState", target.JobPostId);

            var idIssue = ValidationRules.RequiredString(target.JobPostId, "JobPostId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var statusIssue = ValidationRules.EnumDefined(target.Status, "Status", ref_);
            if (statusIssue.HasValue) issues.Add(statusIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // RecruitmentRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(RecruitmentRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("RecruitmentRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "RecruitmentRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("RecruitmentRuntimeState");

            issues.AddRange(ValidationRules.NoNullItems(target.CandidateIds, "CandidateIds", ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.JobPostIds,   "JobPostIds",   ref_));

            return ValidationResult.WithIssues(issues);
        }
    }
}
