namespace Project.Core.Validation
{
    /// <summary>
    /// Contract for external validators. Validators are kept separate from data models.
    /// Each domain type has a corresponding IValidator implementation.
    /// </summary>
    /// <typeparam name="T">The data type this validator validates.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates the target instance and returns a result describing any issues found.
        /// Returns ValidationResult.Valid() when no issues exist.
        /// </summary>
        ValidationResult Validate(T target);
    }
}
