using System.Collections.Generic;
using Project.Core.Runtime.Team;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for team-domain types.
    /// Checks required IDs, 0–100 stat ranges, ProgressPercent range, enum validity,
    /// and null-item checks on member ID lists.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class TeamDataValidator :
        IValidator<TeamProfile>,
        IValidator<TeamRuntimeState>,
        IValidator<AssignmentRuntimeState>
    {
        // -------------------------------------------------------------------------
        // TeamProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(TeamProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("TeamProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "TeamProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("TeamProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

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
        // TeamRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(TeamRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("TeamRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "TeamRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("TeamRuntimeState", target.TeamId);

            var teamIdIssue = ValidationRules.RequiredString(target.TeamId, "TeamId", ref_);
            if (teamIdIssue.HasValue) issues.Add(teamIdIssue.Value);

            // 0–100 stats.
            var cohesionIssue = ValidationRules.Range(target.Cohesion, 0, 100, "Cohesion", ref_);
            if (cohesionIssue.HasValue) issues.Add(cohesionIssue.Value);

            var moraleIssue = ValidationRules.Range(target.Morale, 0, 100, "Morale", ref_);
            if (moraleIssue.HasValue) issues.Add(moraleIssue.Value);

            var workloadIssue = ValidationRules.Range(target.Workload, 0, 100, "Workload", ref_);
            if (workloadIssue.HasValue) issues.Add(workloadIssue.Value);

            // MemberIds must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.MemberIds, "MemberIds", ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.AssignmentHistoryIds, "AssignmentHistoryIds", ref_));

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // AssignmentRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(AssignmentRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("AssignmentRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "AssignmentRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("AssignmentRuntimeState", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

            var targetTypeIssue = ValidationRules.EnumDefined(target.TargetType, "TargetType", ref_);
            if (targetTypeIssue.HasValue) issues.Add(targetTypeIssue.Value);

            // TargetId is a stable reference — must be present.
            var targetIdIssue = ValidationRules.RequiredString(target.TargetId, "TargetId", ref_);
            if (targetIdIssue.HasValue) issues.Add(targetIdIssue.Value);

            // TeamId must be present.
            var teamIdIssue = ValidationRules.RequiredString(target.TeamId, "TeamId", ref_);
            if (teamIdIssue.HasValue) issues.Add(teamIdIssue.Value);

            // ProgressPercent: 0–100.
            var progressIssue = ValidationRules.Range(target.ProgressPercent, 0, 100, "ProgressPercent", ref_);
            if (progressIssue.HasValue) issues.Add(progressIssue.Value);

            // StartDate must not be null.
            if (target.StartDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "StartDate",
                    $"{ref_}.StartDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }
    }
}
