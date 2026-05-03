namespace Project.Core.Validation
{
    /// <summary>
    /// Identifies the entity being validated.
    /// Used in ValidationIssue to pinpoint which record produced an issue.
    /// EntityId is nullable — some validations apply to the system rather than a specific record.
    /// </summary>
    public struct ValidationEntityReference
    {
        /// <summary>Category or type name of the entity (e.g. "CompanyProfile", "EmployeeProfile").</summary>
        public string EntityType;

        /// <summary>
        /// Stable ID of the specific entity instance, if applicable.
        /// Null when the issue relates to the collection or system rather than a single record.
        /// </summary>
        public string EntityId;

        public ValidationEntityReference(string entityType, string entityId = null)
        {
            EntityType = entityType;
            EntityId   = entityId;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return EntityId != null
                ? $"{EntityType}[{EntityId}]"
                : EntityType;
        }
    }
}
