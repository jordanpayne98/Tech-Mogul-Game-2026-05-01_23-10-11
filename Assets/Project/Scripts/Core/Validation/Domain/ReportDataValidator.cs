using System.Collections.Generic;
using Project.Core.Runtime.Report;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for report-domain types.
    /// Checks required IDs, required Title/Summary, enum validity, no null items in
    /// RelatedEntities/KeyValues, and stable ID format on AvailableActionIds.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class ReportDataValidator :
        IValidator<ReportProfile>,
        IValidator<InboxRuntimeState>
    {
        // -------------------------------------------------------------------------
        // ReportProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ReportProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ReportProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ReportProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ReportProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var titleIssue = ValidationRules.RequiredString(target.Title, "Title", ref_);
            if (titleIssue.HasValue) issues.Add(titleIssue.Value);

            var summaryIssue = ValidationRules.RequiredString(target.Summary, "Summary", ref_);
            if (summaryIssue.HasValue) issues.Add(summaryIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

            var categoryIssue = ValidationRules.EnumDefined(target.Category, "Category", ref_);
            if (categoryIssue.HasValue) issues.Add(categoryIssue.Value);

            var priorityIssue = ValidationRules.EnumDefined(target.Priority, "Priority", ref_);
            if (priorityIssue.HasValue) issues.Add(priorityIssue.Value);

            // Date must not be null.
            if (target.Date == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "Date",
                    $"{ref_}.Date is null."));
            }

            // RelatedEntities: validate each reference entry's EntityId format where present.
            if (target.RelatedEntities != null)
            {
                for (int i = 0; i < target.RelatedEntities.Count; i++)
                {
                    var refEntry = target.RelatedEntities[i];
                    // EntityId must be present in each reference.
                    if (string.IsNullOrWhiteSpace(refEntry.EntityId))
                    {
                        issues.Add(new ValidationIssue(
                            ValidationSeverity.Error,
                            "required.missing",
                            ref_,
                            $"RelatedEntities[{i}].EntityId",
                            $"{ref_}.RelatedEntities[{i}].EntityId is required but was null or empty."));
                    }

                    if (string.IsNullOrWhiteSpace(refEntry.EntityType))
                    {
                        issues.Add(new ValidationIssue(
                            ValidationSeverity.Error,
                            "required.missing",
                            ref_,
                            $"RelatedEntities[{i}].EntityType",
                            $"{ref_}.RelatedEntities[{i}].EntityType is required but was null or empty."));
                    }
                }
            }

            // KeyValues: validate Label and Value presence for each entry.
            if (target.KeyValues != null)
            {
                for (int i = 0; i < target.KeyValues.Count; i++)
                {
                    var kv = target.KeyValues[i];
                    if (string.IsNullOrWhiteSpace(kv.Label))
                    {
                        issues.Add(new ValidationIssue(
                            ValidationSeverity.Error,
                            "required.missing",
                            ref_,
                            $"KeyValues[{i}].Label",
                            $"{ref_}.KeyValues[{i}].Label is required but was null or empty."));
                    }
                    if (string.IsNullOrWhiteSpace(kv.Value))
                    {
                        issues.Add(new ValidationIssue(
                            ValidationSeverity.Error,
                            "required.missing",
                            ref_,
                            $"KeyValues[{i}].Value",
                            $"{ref_}.KeyValues[{i}].Value is required but was null or empty."));
                    }
                }
            }

            // AvailableActionIds: each entry must follow stable ID format.
            if (target.AvailableActionIds != null)
            {
                for (int i = 0; i < target.AvailableActionIds.Count; i++)
                {
                    string actionId = target.AvailableActionIds[i];
                    var actionIdIssue = ValidationRules.RequiredId(actionId, $"AvailableActionIds[{i}]", ref_);
                    if (actionIdIssue.HasValue) issues.Add(actionIdIssue.Value);
                }
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // InboxRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(InboxRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("InboxRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "InboxRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("InboxRuntimeState", target.CompanyId);

            var companyIdIssue = ValidationRules.RequiredString(target.CompanyId, "CompanyId", ref_);
            if (companyIdIssue.HasValue) issues.Add(companyIdIssue.Value);

            // UnreadCount must be non-negative.
            if (target.UnreadCount < 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    ref_,
                    "UnreadCount",
                    $"{ref_}.UnreadCount value {target.UnreadCount} must be zero or greater."));
            }

            // DecisionRequiredCount must be non-negative.
            if (target.DecisionRequiredCount < 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    ref_,
                    "DecisionRequiredCount",
                    $"{ref_}.DecisionRequiredCount value {target.DecisionRequiredCount} must be zero or greater."));
            }

            // ReportIds must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.ReportIds, "ReportIds", ref_));

            return ValidationResult.WithIssues(issues);
        }
    }
}
