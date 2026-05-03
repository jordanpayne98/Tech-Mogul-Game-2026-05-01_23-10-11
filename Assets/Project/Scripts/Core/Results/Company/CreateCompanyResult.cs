using Project.Core.Runtime;

namespace Project.Core.Results.Company
{
    /// <summary>
    /// Result of a CreateCompanyRequest. Contains the fully initialized session state on success.
    /// </summary>
    public sealed class CreateCompanyResult
    {
        public bool Success { get; }
        public string FailureReason { get; }

        /// <summary>Stable ID of the newly created company. Empty string on failure.</summary>
        public string CompanyId { get; }

        /// <summary>Stable ID of the newly created founder. Empty string on failure.</summary>
        public string FounderId { get; }

        /// <summary>
        /// The fully assembled session state for the new game.
        /// Null on failure.
        /// </summary>
        public GameSessionState SessionState { get; }

        private CreateCompanyResult(bool success, string failureReason, string companyId, string founderId, GameSessionState sessionState)
        {
            Success       = success;
            FailureReason = failureReason;
            CompanyId     = companyId;
            FounderId     = founderId;
            SessionState  = sessionState;
        }

        public static CreateCompanyResult Succeeded(string companyId, string founderId, GameSessionState sessionState)
        {
            return new CreateCompanyResult(true, string.Empty, companyId, founderId, sessionState);
        }

        public static CreateCompanyResult Failed(string reason)
        {
            return new CreateCompanyResult(false, reason, string.Empty, string.Empty, null);
        }
    }
}
