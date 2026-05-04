using System;
using Project.Core.Runtime;

namespace Project.Application
{
    /// <summary>
    /// Lightweight Application-layer POCO that acts as the typed session facade
    /// for screen controllers. Holds a reference to the live GameSessionState.
    ///
    /// Constructed by GameCompositionRoot at the end of BindSession() and handed
    /// to UIShellController via RebindSession(). Screen controllers receive it
    /// through their factory closures.
    ///
    /// Subsequent Phase 6 plans will add typed use case and service properties
    /// as each screen is wired (Rule 12 — Context Growth Rule).
    ///
    /// No MonoBehaviour. No UI Toolkit references. No UnityEngine dependency.
    /// Defined in Plan 6A-0.
    /// </summary>
    public sealed class GameSessionContext
    {
        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// The live session. Always non-null after construction.
        /// </summary>
        public GameSessionState CurrentSession { get; }

        // ─── Constructor ──────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new GameSessionContext wrapping the given session.
        /// </summary>
        /// <param name="currentSession">The active game session. Must not be null.</param>
        public GameSessionContext(GameSessionState currentSession)
        {
            CurrentSession = currentSession ?? throw new ArgumentNullException(nameof(currentSession));
        }
    }
}
