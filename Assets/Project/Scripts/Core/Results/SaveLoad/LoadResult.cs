using Project.Core.Runtime;

namespace Project.Core.Results.SaveLoad
{
    /// <summary>
    /// Result returned by ISaveService.Load().
    /// Contains the fully reconstructed GameSessionState on success.
    /// Use the static factories to construct instances.
    /// </summary>
    public sealed class LoadResult
    {
        /// <summary>True when the load completed without errors and the session is ready to use.</summary>
        public bool Success { get; }

        /// <summary>Human-readable description of the failure reason. Empty when Success is true.</summary>
        public string FailureReason { get; }

        /// <summary>Fully reconstructed session state. Null when Success is false.</summary>
        public GameSessionState Session { get; }

        private LoadResult(bool success, string failureReason, GameSessionState session)
        {
            Success       = success;
            FailureReason = failureReason;
            Session       = session;
        }

        /// <summary>Creates a successful load result containing the reconstructed session.</summary>
        public static LoadResult Succeeded(GameSessionState session)
        {
            return new LoadResult(true, string.Empty, session);
        }

        /// <summary>Creates a failed load result with a description of the failure reason.</summary>
        public static LoadResult Failed(string reason)
        {
            return new LoadResult(false, reason, null);
        }
    }
}
