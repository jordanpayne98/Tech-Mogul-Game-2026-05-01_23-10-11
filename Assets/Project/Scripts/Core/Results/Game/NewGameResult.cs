using Project.Core.Runtime;

namespace Project.Core.Results.Game
{
    /// <summary>
    /// Result returned by NewGameOrchestrator.Execute.
    /// Contains the fully initialised GameSessionState on success.
    /// Use static factory methods: Succeeded(session) or Failed(reason).
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class NewGameResult
    {
        public bool             Success       { get; }
        public string           FailureReason { get; }

        /// <summary>The initialised session. Null on failure.</summary>
        public GameSessionState Session       { get; }

        private NewGameResult(bool success, string failureReason, GameSessionState session)
        {
            Success       = success;
            FailureReason = failureReason;
            Session       = session;
        }

        public static NewGameResult Succeeded(GameSessionState session)
            => new NewGameResult(true, string.Empty, session);

        public static NewGameResult Failed(string reason)
            => new NewGameResult(false, reason, null);
    }
}
