using System.Collections.Generic;
using Project.Core.Definitions.Time;
using Project.Core.Runtime.Time;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for time-domain types.
    /// Checks field ranges, enum validity, and required reference presence.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class TimeDataValidator :
        IValidator<GameDateTime>,
        IValidator<TimeRuntimeState>
    {
        // -------------------------------------------------------------------------
        // GameDateTime
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(GameDateTime target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("GameDateTime");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "GameDateTime instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("GameDateTime");

            // Year must be greater than 0.
            if (target.Year <= 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.out_of_bounds",
                    ref_,
                    "Year",
                    $"GameDateTime.Year value {target.Year} must be greater than 0."));
            }

            var monthIssue = ValidationRules.Range(target.Month, 1, 12, "Month", ref_);
            if (monthIssue.HasValue) issues.Add(monthIssue.Value);

            var dayIssue = ValidationRules.Range(target.Day, 1, 30, "Day", ref_);
            if (dayIssue.HasValue) issues.Add(dayIssue.Value);

            var hourIssue = ValidationRules.Range(target.Hour, 0, 23, "Hour", ref_);
            if (hourIssue.HasValue) issues.Add(hourIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // TimeRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(TimeRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("TimeRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "TimeRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("TimeRuntimeState");

            // Validate CurrentDate.
            if (target.CurrentDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "CurrentDate",
                    "TimeRuntimeState.CurrentDate is null."));
            }
            else
            {
                issues.AddRange(Validate(target.CurrentDate).Issues);
            }

            // Validate enum fields.
            var speedIssue = ValidationRules.EnumDefined(target.Speed, "Speed", ref_);
            if (speedIssue.HasValue) issues.Add(speedIssue.Value);

            var modeIssue = ValidationRules.EnumDefined(target.AdvanceMode, "AdvanceMode", ref_);
            if (modeIssue.HasValue) issues.Add(modeIssue.Value);

            var filterIssue = ValidationRules.EnumDefined(target.Filter, "Filter", ref_);
            if (filterIssue.HasValue) issues.Add(filterIssue.Value);

            return ValidationResult.WithIssues(issues);
        }
    }
}
