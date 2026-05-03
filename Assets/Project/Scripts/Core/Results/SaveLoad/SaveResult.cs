namespace Project.Core.Results.SaveLoad
{
    /// <summary>
    /// Result returned by ISaveService.Save().
    /// Use the static factories to construct instances.
    /// </summary>
    public sealed class SaveResult
    {
        /// <summary>True when the save completed without errors.</summary>
        public bool Success { get; }

        /// <summary>Human-readable description of the failure reason. Empty when Success is true.</summary>
        public string FailureReason { get; }

        /// <summary>Stable ID of the save file that was written. Empty when Success is false.</summary>
        public string SaveId { get; }

        private SaveResult(bool success, string failureReason, string saveId)
        {
            Success       = success;
            FailureReason = failureReason;
            SaveId        = saveId;
        }

        /// <summary>Creates a successful save result with the stable ID of the written file.</summary>
        public static SaveResult Succeeded(string saveId)
        {
            return new SaveResult(true, string.Empty, saveId);
        }

        /// <summary>Creates a failed save result with a description of the failure reason.</summary>
        public static SaveResult Failed(string reason)
        {
            return new SaveResult(false, reason, string.Empty);
        }
    }
}
