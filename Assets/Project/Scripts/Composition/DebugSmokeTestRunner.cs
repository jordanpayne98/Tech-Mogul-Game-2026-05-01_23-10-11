#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Project.Core.Debugging;
using Project.Core.Definitions.Company;
using Project.Core.Definitions.Time;
using Project.Core.Requests.Company;
using Project.Core.Requests.Time;
using Project.Core.Results.Game;
using Project.Core.Results.Time;
using UnityEngine;

namespace Project.Composition
{
    /// <summary>
    /// Developer tool for manually verifying the full simulation loop.
    /// Triggers a New Game → Validate → Continue cycle via [ContextMenu] or from GameBootstrapper.
    ///
    /// This is NOT a test framework. It logs results for human inspection only.
    /// Available in Editor and Development Builds only.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class DebugSmokeTestRunner : MonoBehaviour
    {
        [Header("Debug Smoke Test")]
        [SerializeField, Tooltip("Reference to the bootstrapper hosting the composition root.")]
        private GameBootstrapper _bootstrapper;

        // ─── Context Menu Entry Point ─────────────────────────────────────────────

        [ContextMenu("Run Smoke Test")]
        public void RunSmokeTest()
        {
            if (_bootstrapper == null)
            {
                _bootstrapper = FindObjectOfType<GameBootstrapper>();
            }

            if (_bootstrapper == null || _bootstrapper.CompositionRoot == null)
            {
                DebugLogger.LogError(DebugCategory.Testing,
                    "[DebugSmokeTestRunner] No GameBootstrapper or CompositionRoot available. " +
                    "Ensure GameBootstrapper is in the scene and has initialized.");
                return;
            }

            ExecuteSmokeTest(_bootstrapper.CompositionRoot);
        }

        // ─── Internal execution ───────────────────────────────────────────────────

        private static void ExecuteSmokeTest(GameCompositionRoot compositionRoot)
        {
            DebugLogger.Log(DebugCategory.Testing, "[SmokeTest] ─── Smoke test starting... ───");

            // ── 1. New Game ────────────────────────────────────────────────────────

            var companyRequest = new CreateCompanyRequest(
                founderName:   "[Placeholder] Test Founder",
                companyName:   "[Placeholder] Test Company",
                logoIconId:    string.Empty,
                brandColourHex: "#FFFFFF",
                location:      "[Placeholder] Test City",
                background:    FounderBackground.Engineer,
                capitalPreset: CapitalPreset.Bootstrapped,
                focus:         CompanyFocus.ConsumerSoftware,
                difficulty:    SandboxDifficulty.Standard,
                marketSeed:    12345);

            NewGameResult newGameResult = compositionRoot.NewGameOrchestrator.Execute(companyRequest, 12345);

            if (!newGameResult.Success)
            {
                DebugLogger.LogError(DebugCategory.Testing,
                    $"[SmokeTest] New game FAILED: {newGameResult.FailureReason}");
                return;
            }

            DebugLogger.Log(DebugCategory.Testing,
                $"[SmokeTest] New game OK. SessionId: {newGameResult.Session.SessionId} | " +
                $"Date: {newGameResult.Session.TimeState?.CurrentDate}");

            // ── 2. Bind session ────────────────────────────────────────────────────

            compositionRoot.BindSession(newGameResult.Session);

            // ── 3. Validate session ────────────────────────────────────────────────

            var validationResult = compositionRoot.GameSessionValidator.Validate(newGameResult.Session);

            if (!validationResult.IsValid)
            {
                DebugLogger.LogError(DebugCategory.Testing,
                    $"[SmokeTest] Session INVALID after new game — {validationResult.Errors.Count} error(s):");

                foreach (string error in validationResult.Errors)
                {
                    DebugLogger.LogError(DebugCategory.Testing, $"  • {error}");
                }

                return;
            }

            DebugLogger.Log(DebugCategory.Testing, "[SmokeTest] Session valid after new game.");

            // ── 4. Run one Continue cycle ─────────────────────────────────────────

            var continueRequest = new StartContinueRequest(
                TimeSpeed.Speed1x,
                TimeAdvanceMode.ContinueUntilInterrupt,
                InterruptionFilter.CriticalOnly);

            ContinueResult continueResult = compositionRoot.ContinueOrchestrator.Execute(
                continueRequest,
                newGameResult.Session.TimeState);

            if (!continueResult.Success)
            {
                DebugLogger.LogError(DebugCategory.Testing,
                    $"[SmokeTest] Continue FAILED: {continueResult.FailureReason}");
                return;
            }

            DebugLogger.Log(DebugCategory.Testing,
                $"[SmokeTest] Continue OK. " +
                $"Hours advanced: {continueResult.TotalHoursAdvanced} | " +
                $"Final date: {continueResult.StoppedAtDate} | " +
                $"Interruptions: {continueResult.Interruptions?.Count ?? 0}");

            // ── 5. Validate session after Continue ─────────────────────────────────

            var postContinueValidation = compositionRoot.GameSessionValidator.Validate(newGameResult.Session);

            if (!postContinueValidation.IsValid)
            {
                DebugLogger.LogError(DebugCategory.Testing,
                    $"[SmokeTest] Session INVALID after Continue — {postContinueValidation.Errors.Count} error(s):");

                foreach (string error in postContinueValidation.Errors)
                {
                    DebugLogger.LogError(DebugCategory.Testing, $"  • {error}");
                }
            }
            else
            {
                DebugLogger.Log(DebugCategory.Testing, "[SmokeTest] Session valid after Continue.");
            }

            // ── 6. Done ────────────────────────────────────────────────────────────

            DebugLogger.Log(DebugCategory.Testing, "[SmokeTest] ─── Smoke test complete. ───");
        }
    }
}
#endif
