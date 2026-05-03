namespace Project.Core.Results.Employee
{
    /// <summary>
    /// Result of a <see cref="Project.Core.Requests.Employee.HireCandidateRequest"/>.
    /// Use the static factory methods <see cref="Succeeded"/> and <see cref="Failed"/> to construct instances.
    /// </summary>
    public sealed class HireCandidateResult
    {
        /// <summary>True if the hire offer was successfully processed.</summary>
        public bool Success { get; }

        /// <summary>Human-readable reason for failure. Empty string when <see cref="Success"/> is true.</summary>
        public string FailureReason { get; }

        /// <summary>
        /// Stable ID of the newly created employee record.
        /// Null or empty when <see cref="Success"/> is false.
        /// </summary>
        public string EmployeeId { get; }

        private HireCandidateResult(bool success, string failureReason, string employeeId)
        {
            Success = success;
            FailureReason = failureReason;
            EmployeeId = employeeId;
        }

        /// <summary>Creates a successful result with the new employee's stable ID.</summary>
        public static HireCandidateResult Succeeded(string employeeId)
        {
            return new HireCandidateResult(true, string.Empty, employeeId);
        }

        /// <summary>Creates a failure result with a descriptive reason.</summary>
        public static HireCandidateResult Failed(string reason)
        {
            return new HireCandidateResult(false, reason, string.Empty);
        }
    }
}
