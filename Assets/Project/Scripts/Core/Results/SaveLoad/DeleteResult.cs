namespace Project.Core.Results.SaveLoad
{
    /// <summary>
    /// Result returned by ISaveService.Delete().
    /// Use the static factories to construct instances.
    /// </summary>
    public sealed class DeleteResult
    {
        /// <summary>True when the delete completed without errors.</summary>
        public bool Success { get; }

        /// <summary>Human-readable description of the failure reason. Empty when Success is true.</summary>
        public string FailureReason { get; }

        private DeleteResult(bool success, string failureReason)
        {
            Success       = success;
            FailureReason = failureReason;
        }

        /// <summary>Creates a successful delete result.</summary>
        public static DeleteResult Succeeded()
        {
            return new DeleteResult(true, string.Empty);
        }

        /// <summary>Creates a failed delete result with a description of the failure reason.</summary>
        public static DeleteResult Failed(string reason)
        {
            return new DeleteResult(false, reason);
        }
    }
}
