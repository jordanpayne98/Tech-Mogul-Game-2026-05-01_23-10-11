using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Project.Core.Validation
{
    /// <summary>
    /// Static helper methods for common structural data validation checks.
    /// All methods return a ValidationIssue? — null means the check passed.
    /// Use these inside IValidator implementations to build consistent ValidationResult instances.
    ///
    /// Standard field ranges (for reference):
    ///   Percentage / stat values : 0–100 (inclusive)
    ///   Basis points             : 0–10000 (inclusive, where 10000 = 100%)
    ///   Money minor units        : usually 0+ (except CashMinorUnits which may be negative — debt)
    ///   Month                    : 1–12 (inclusive)
    ///   Day                      : 1–30 (inclusive, fixed 30-day calendar)
    ///   Hour                     : 0–23 (inclusive)
    /// </summary>
    public static class ValidationRules
    {
        // Stable ID format: lowercase alphanumeric words separated by dots and/or underscores.
        // Valid examples: "research.cloud_auto_scaling", "screen.main_menu", "product.os_v1"
        private static readonly Regex StableIdPattern = new Regex(
            @"^[a-z][a-z0-9_]*(\.[a-z][a-z0-9_]*)*$",
            RegexOptions.Compiled);

        // -------------------------------------------------------------------------
        // String checks
        // -------------------------------------------------------------------------

        /// <summary>
        /// Checks that a string field is non-null, non-empty, and non-whitespace.
        /// Returns an Error issue if the check fails; null otherwise.
        /// </summary>
        public static ValidationIssue? RequiredString(
            string value,
            string fieldName,
            ValidationEntityReference entity)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    entity,
                    fieldName,
                    $"{entity}.{fieldName} is required but was null, empty, or whitespace.");
            }

            return null;
        }

        /// <summary>
        /// Checks that an ID field is non-null, non-empty, non-whitespace, and conforms to stable ID format
        /// (lowercase dot-separated semantic segments, e.g. "screen.main_menu").
        /// Returns an Error issue if the check fails; null otherwise.
        /// </summary>
        public static ValidationIssue? RequiredId(
            string value,
            string fieldName,
            ValidationEntityReference entity)
        {
            ValidationIssue? missing = RequiredString(value, fieldName, entity);
            if (missing.HasValue)
            {
                return missing;
            }

            return ValidateStableIdFormat(value, fieldName, entity);
        }

        /// <summary>
        /// Checks that a string value conforms to stable ID format: lowercase dot-separated semantic segments.
        /// Segments may contain lowercase letters, digits, and underscores.
        /// Example valid values: "research.cloud_auto_scaling", "screen.main_menu", "product.os_v1"
        /// Returns a Warning issue if the check fails; null otherwise.
        /// </summary>
        public static ValidationIssue? ValidateStableIdFormat(
            string value,
            string fieldName,
            ValidationEntityReference entity)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null; // RequiredString should catch this first.
            }

            if (!StableIdPattern.IsMatch(value))
            {
                return new ValidationIssue(
                    ValidationSeverity.Warning,
                    "id.format_invalid",
                    entity,
                    fieldName,
                    $"{entity}.{fieldName} value '{value}' does not match stable ID format (lowercase.dot_separated.semantic).");
            }

            return null;
        }

        // -------------------------------------------------------------------------
        // Numeric range checks
        // -------------------------------------------------------------------------

        /// <summary>
        /// Checks that an integer value falls within an inclusive range [min, max].
        /// Returns an Error issue if the check fails; null otherwise.
        ///
        /// Common ranges:
        ///   Percentage/stat : Range(value, 0, 100, ...)
        ///   Basis points    : Range(value, 0, 10000, ...)
        ///   Month           : Range(value, 1, 12, ...)
        ///   Day             : Range(value, 1, 30, ...)
        ///   Hour            : Range(value, 0, 23, ...)
        /// </summary>
        public static ValidationIssue? Range(
            int value,
            int min,
            int max,
            string fieldName,
            ValidationEntityReference entity)
        {
            if (value < min || value > max)
            {
                return new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.out_of_bounds",
                    entity,
                    fieldName,
                    $"{entity}.{fieldName} value {value} is outside the allowed range [{min}, {max}].");
            }

            return null;
        }

        /// <summary>
        /// Checks that a long value is zero or greater.
        /// Returns an Error issue if the check fails; null otherwise.
        ///
        /// Use for money minor units and other non-negative quantities.
        /// Note: CashMinorUnits on FinanceRuntimeState may be negative (debt) — do not use this rule for it.
        /// </summary>
        public static ValidationIssue? NonNegative(
            long value,
            string fieldName,
            ValidationEntityReference entity)
        {
            if (value < 0L)
            {
                return new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    entity,
                    fieldName,
                    $"{entity}.{fieldName} value {value} must be zero or greater.");
            }

            return null;
        }

        // -------------------------------------------------------------------------
        // Collection checks
        // -------------------------------------------------------------------------

        /// <summary>
        /// Checks that no entry in the list is null.
        /// Returns one Error issue per null entry found; returns an empty list if the collection itself is null.
        /// </summary>
        public static IEnumerable<ValidationIssue> NoNullItems<T>(
            List<T> list,
            string fieldName,
            ValidationEntityReference entity) where T : class
        {
            if (list == null)
            {
                yield break;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    yield return new ValidationIssue(
                        ValidationSeverity.Error,
                        "collection.null_item",
                        entity,
                        fieldName,
                        $"{entity}.{fieldName}[{i}] is null.");
                }
            }
        }

        /// <summary>
        /// Checks that a list of string IDs contains no duplicates (case-sensitive).
        /// Returns Error issues for each duplicate group found.
        /// Null or empty IDs within the list are not checked here; use NoNullItems + RequiredString for those.
        /// </summary>
        public static IEnumerable<ValidationIssue> NoDuplicateIds(
            List<string> ids,
            string fieldName,
            ValidationEntityReference entity)
        {
            if (ids == null || ids.Count == 0)
            {
                yield break;
            }

            var seen    = new HashSet<string>(StringComparer.Ordinal);
            var reported = new HashSet<string>(StringComparer.Ordinal);

            foreach (string id in ids)
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                if (!seen.Add(id) && reported.Add(id))
                {
                    yield return new ValidationIssue(
                        ValidationSeverity.Error,
                        "collection.duplicate_id",
                        entity,
                        fieldName,
                        $"{entity}.{fieldName} contains duplicate ID '{id}'.");
                }
            }
        }

        // -------------------------------------------------------------------------
        // Enum check
        // -------------------------------------------------------------------------

        /// <summary>
        /// Checks that an enum value is defined in the enum type using Enum.IsDefined.
        /// Returns an Error issue if the value is not defined; null otherwise.
        /// </summary>
        public static ValidationIssue? EnumDefined<TEnum>(
            TEnum value,
            string fieldName,
            ValidationEntityReference entity) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                return new ValidationIssue(
                    ValidationSeverity.Error,
                    "enum.undefined_value",
                    entity,
                    fieldName,
                    $"{entity}.{fieldName} value '{value}' is not a defined member of {typeof(TEnum).Name}.");
            }

            return null;
        }
    }
}
