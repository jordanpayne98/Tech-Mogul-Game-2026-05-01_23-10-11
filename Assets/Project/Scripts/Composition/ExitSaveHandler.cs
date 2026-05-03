using Project.Application.UseCases.SaveLoad;
using Project.Core.Debugging;
using UnityEngine;

namespace Project.Composition
{
    /// <summary>
    /// Thin MonoBehaviour that triggers an autosave when the application is about to quit.
    /// Wired by GameBootstrapper via SetCoordinator() after construction.
    /// No Update() loop. No Start(). OnApplicationQuit() only.
    /// </summary>
    public sealed class ExitSaveHandler : MonoBehaviour
    {
        private AutosaveCoordinator _coordinator;

        // ─── Wiring ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by GameBootstrapper after AutosaveCoordinator is constructed and initialized.
        /// </summary>
        /// <param name="coordinator">The active autosave coordinator.</param>
        public void SetCoordinator(AutosaveCoordinator coordinator)
        {
            _coordinator = coordinator;
        }

        // ─── Unity lifecycle ──────────────────────────────────────────────────────

        private void OnApplicationQuit()
        {
            if (_coordinator == null)
            {
                DebugLogger.LogWarning(DebugCategory.SaveLoad,
                    "[ExitSaveHandler] OnApplicationQuit called but coordinator is null — autosave skipped.");
                return;
            }

            DebugLogger.Log(DebugCategory.SaveLoad,
                "[ExitSaveHandler] Application quitting — requesting exit autosave.");

            _coordinator.RequestPreActionAutosave("on_exit");
        }
    }
}
