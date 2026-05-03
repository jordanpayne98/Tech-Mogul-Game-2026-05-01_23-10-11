namespace Project.Core.Validation
{
    /// <summary>
    /// Severity level of a validation issue.
    /// IsValid is determined by the absence of Error-severity issues.
    /// Warnings alone do not make a result invalid.
    /// </summary>
    public enum ValidationSeverity
    {
        /// <summary>Structural problem that makes the data unusable or unsafe to process.</summary>
        Error,

        /// <summary>Potential problem that should be reviewed but does not block processing.</summary>
        Warning,

        /// <summary>Informational notice with no action required.</summary>
        Info
    }
}
