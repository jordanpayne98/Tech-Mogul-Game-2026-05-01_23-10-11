using System.Collections.Generic;
using System.Linq;

namespace Project.Core.Validation
{
    /// <summary>
    /// Aggregated result of a validation pass.
    /// IsValid is true when no Error-severity issues are present.
    /// HasWarnings is true when at least one Warning-severity issue is present.
    /// Use the static factories Valid() and WithIssues() to construct instances.
    /// Use Merge() to combine results from multiple validators.
    /// </summary>
    public sealed class ValidationResult
    {
        // -------------------------------------------------------------------------
        // Properties
        // -------------------------------------------------------------------------

        /// <summary>All issues found during this validation pass.</summary>
        public List<ValidationIssue> Issues { get; }

        /// <summary>
        /// True when no Error-severity issues exist.
        /// Warnings alone do not make a result invalid.
        /// </summary>
        public bool IsValid => !Issues.Any(i => i.Severity == ValidationSeverity.Error);

        /// <summary>True when at least one Warning-severity issue exists.</summary>
        public bool HasWarnings => Issues.Any(i => i.Severity == ValidationSeverity.Warning);

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        private ValidationResult(List<ValidationIssue> issues)
        {
            Issues = issues ?? new List<ValidationIssue>();
        }

        // -------------------------------------------------------------------------
        // Static factories
        // -------------------------------------------------------------------------

        /// <summary>Creates a result with no issues. Always valid.</summary>
        public static ValidationResult Valid()
        {
            return new ValidationResult(new List<ValidationIssue>());
        }

        /// <summary>Creates a result containing the provided issues.</summary>
        public static ValidationResult WithIssues(IEnumerable<ValidationIssue> issues)
        {
            return new ValidationResult(issues != null
                ? new List<ValidationIssue>(issues)
                : new List<ValidationIssue>());
        }

        // -------------------------------------------------------------------------
        // Instance methods
        // -------------------------------------------------------------------------

        /// <summary>
        /// Combines this result with another, returning a new result containing all issues from both.
        /// Neither source result is modified.
        /// </summary>
        public ValidationResult Merge(ValidationResult other)
        {
            var combined = new List<ValidationIssue>(Issues);

            if (other != null)
            {
                combined.AddRange(other.Issues);
            }

            return new ValidationResult(combined);
        }
    }
}
