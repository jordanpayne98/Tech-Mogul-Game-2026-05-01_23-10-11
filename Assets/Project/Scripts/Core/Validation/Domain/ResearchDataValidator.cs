using System.Collections.Generic;
using Project.Core.Definitions.Research;
using Project.Core.Runtime.Research;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for research-domain types.
    /// Checks required IDs, EstimatedDurationDays &gt; 0, CostMinorUnits non-negative,
    /// RiskLevel/ObsolescenceRisk 0–100, ProgressPercent 0–100, enum validity,
    /// and no null items in PrerequisiteIds/Unlocks.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class ResearchDataValidator :
        IValidator<ResearchProjectDefinition>,
        IValidator<ResearchProjectRuntimeState>,
        IValidator<ResearchRuntimeState>
    {
        // -------------------------------------------------------------------------
        // ResearchProjectDefinition
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ResearchProjectDefinition target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ResearchProjectDefinition");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ResearchProjectDefinition instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ResearchProjectDefinition", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var trackIssue = ValidationRules.EnumDefined(target.Track, "Track", ref_);
            if (trackIssue.HasValue) issues.Add(trackIssue.Value);

            // EstimatedDurationDays must be > 0.
            if (target.EstimatedDurationDays <= 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.out_of_bounds",
                    ref_,
                    "EstimatedDurationDays",
                    $"{ref_}.EstimatedDurationDays value {target.EstimatedDurationDays} must be greater than 0."));
            }

            // CostMinorUnits must be non-negative.
            var costIssue = ValidationRules.NonNegative(target.CostMinorUnits, "CostMinorUnits", ref_);
            if (costIssue.HasValue) issues.Add(costIssue.Value);

            // Risk values: 0–100.
            var riskIssue = ValidationRules.Range(target.RiskLevel, 0, 100, "RiskLevel", ref_);
            if (riskIssue.HasValue) issues.Add(riskIssue.Value);

            var obsolescenceIssue = ValidationRules.Range(target.ObsolescenceRisk, 0, 100, "ObsolescenceRisk", ref_);
            if (obsolescenceIssue.HasValue) issues.Add(obsolescenceIssue.Value);

            // PrerequisiteIds must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.PrerequisiteIds, "PrerequisiteIds", ref_));

            // Unlocks: validate each entry's TargetId and Type.
            if (target.Unlocks != null)
            {
                for (int i = 0; i < target.Unlocks.Count; i++)
                {
                    ResearchUnlock unlock = target.Unlocks[i];

                    var unlockTypeIssue = ValidationRules.EnumDefined(unlock.Type, $"Unlocks[{i}].Type", ref_);
                    if (unlockTypeIssue.HasValue) issues.Add(unlockTypeIssue.Value);

                    var targetIdIssue = ValidationRules.RequiredId(unlock.TargetId, $"Unlocks[{i}].TargetId", ref_);
                    if (targetIdIssue.HasValue) issues.Add(targetIdIssue.Value);
                }
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // ResearchProjectRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ResearchProjectRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ResearchProjectRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ResearchProjectRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ResearchProjectRuntimeState", target.ProjectId);

            var idIssue = ValidationRules.RequiredString(target.ProjectId, "ProjectId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var statusIssue = ValidationRules.EnumDefined(target.Status, "Status", ref_);
            if (statusIssue.HasValue) issues.Add(statusIssue.Value);

            // ProgressPercent: 0–100.
            var progressIssue = ValidationRules.Range(target.ProgressPercent, 0, 100, "ProgressPercent", ref_);
            if (progressIssue.HasValue) issues.Add(progressIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // ResearchRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ResearchRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ResearchRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ResearchRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ResearchRuntimeState", target.CompanyId);

            var companyIdIssue = ValidationRules.RequiredString(target.CompanyId, "CompanyId", ref_);
            if (companyIdIssue.HasValue) issues.Add(companyIdIssue.Value);

            // All project ID lists must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.AvailableProjectIds,    "AvailableProjectIds",    ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ActiveProjectIds,        "ActiveProjectIds",        ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.CompletedProjectIds,     "CompletedProjectIds",     ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ObsoleteProjectIds,      "ObsoleteProjectIds",      ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.UnlockedCapabilityIds,   "UnlockedCapabilityIds",   ref_));

            return ValidationResult.WithIssues(issues);
        }
    }
}
