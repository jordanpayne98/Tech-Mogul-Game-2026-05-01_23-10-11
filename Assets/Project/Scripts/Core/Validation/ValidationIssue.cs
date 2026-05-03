namespace Project.Core.Validation
{
    /// <summary>
    /// Describes a single data integrity problem found during validation.
    /// Each issue identifies the severity, the entity it belongs to, the affected field,
    /// a stable issue code for programmatic handling, and a human-readable message.
    /// </summary>
    public struct ValidationIssue
    {
        /// <summary>How serious this issue is. Error = invalid. Warning = review needed. Info = notice only.</summary>
        public ValidationSeverity Severity;

        /// <summary>
        /// Stable dot-separated code identifying the type of issue (e.g. "required.missing", "range.out_of_bounds").
        /// Should not change across versions.
        /// </summary>
        public string IssueCode;

        /// <summary>The entity this issue belongs to.</summary>
        public ValidationEntityReference Entity;

        /// <summary>The name of the field on the entity where the issue was found (e.g. "Id", "Name", "Morale").</summary>
        public string FieldName;

        /// <summary>Human-readable explanation of the issue. Suitable for logs and debug output.</summary>
        public string Message;

        public ValidationIssue(
            ValidationSeverity severity,
            string issueCode,
            ValidationEntityReference entity,
            string fieldName,
            string message)
        {
            Severity  = severity;
            IssueCode = issueCode;
            Entity    = entity;
            FieldName = fieldName;
            Message   = message;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{Severity}] {Entity}.{FieldName} ({IssueCode}): {Message}";
        }
    }
}
