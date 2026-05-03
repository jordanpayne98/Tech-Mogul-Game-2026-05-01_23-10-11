using System.Collections.Generic;
using Project.Core.Runtime.Company;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for company-domain types.
    /// Checks required IDs, display name presence, stable ID format, enum validity,
    /// and null-item checks on ID lists.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class CompanyDataValidator :
        IValidator<CompanyProfile>,
        IValidator<FounderProfile>,
        IValidator<CompanyRuntimeState>
    {
        // -------------------------------------------------------------------------
        // CompanyProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CompanyProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CompanyProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CompanyProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CompanyProfile", target.Id);

            // Id must be present and follow stable ID format.
            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            // Name is a player-entered display name — required but no stable ID format.
            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            // FounderId must reference a valid founder.
            var founderIdIssue = ValidationRules.RequiredString(target.FounderId, "FounderId", ref_);
            if (founderIdIssue.HasValue) issues.Add(founderIdIssue.Value);

            // LogoIconId is a reference ID — validate stable format if present.
            if (!string.IsNullOrWhiteSpace(target.LogoIconId))
            {
                var logoFormatIssue = ValidationRules.ValidateStableIdFormat(target.LogoIconId, "LogoIconId", ref_);
                if (logoFormatIssue.HasValue) issues.Add(logoFormatIssue.Value);
            }

            // Focus enum must be defined.
            var focusIssue = ValidationRules.EnumDefined(target.Focus, "Focus", ref_);
            if (focusIssue.HasValue) issues.Add(focusIssue.Value);

            // FoundedDate must not be null.
            if (target.FoundedDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "FoundedDate",
                    $"{ref_}.FoundedDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // FounderProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(FounderProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("FounderProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "FounderProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("FounderProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var bgIssue = ValidationRules.EnumDefined(target.Background, "Background", ref_);
            if (bgIssue.HasValue) issues.Add(bgIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CompanyRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CompanyRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CompanyRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CompanyRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CompanyRuntimeState", target.CompanyId);

            var companyIdIssue = ValidationRules.RequiredString(target.CompanyId, "CompanyId", ref_);
            if (companyIdIssue.HasValue) issues.Add(companyIdIssue.Value);

            // Reputation: 0–100.
            var repIssue = ValidationRules.Range(target.Reputation, 0, 100, "Reputation", ref_);
            if (repIssue.HasValue) issues.Add(repIssue.Value);

            // ID lists must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.EmployeeIds,        "EmployeeIds",        ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.TeamIds,             "TeamIds",             ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ProductIds,          "ProductIds",          ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ContractIds,         "ContractIds",         ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ResearchProjectIds,  "ResearchProjectIds",  ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ReportIds,           "ReportIds",           ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.MarketPositionIds,   "MarketPositionIds",   ref_));

            return ValidationResult.WithIssues(issues);
        }
    }
}
