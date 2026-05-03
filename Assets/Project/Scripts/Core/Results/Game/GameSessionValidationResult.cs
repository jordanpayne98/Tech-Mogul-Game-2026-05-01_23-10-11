using System.Collections.Generic;

namespace Project.Core.Results.Game
{
    /// <summary>
    /// Result returned by GameSessionValidator.Validate.
    /// Contains all structural integrity errors found in the session state.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class GameSessionValidationResult
    {
        public bool         IsValid { get; }
        public List<string> Errors  { get; }

        private GameSessionValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors  = errors ?? new List<string>();
        }

        public static GameSessionValidationResult Valid()
            => new GameSessionValidationResult(true, null);

        public static GameSessionValidationResult Invalid(List<string> errors)
            => new GameSessionValidationResult(false, errors);
    }
}
