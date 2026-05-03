using Project.Core.Definitions.Company;

namespace Project.Core.Runtime.Company
{
    /// <summary>
    /// Immutable player-created founder identity data. Created once at game setup and read-only at runtime.
    /// This is a Profile (per-save player data), not a Definition (static design data).
    /// </summary>
    public sealed class FounderProfile
    {
        /// <summary>Stable ID for persistent references across save/load.</summary>
        public string Id { get; }

        /// <summary>Display name of the founder as entered by the player.</summary>
        public string Name { get; }

        /// <summary>The professional background of the founder selected during game setup.</summary>
        public FounderBackground Background { get; }

        public FounderProfile(string id, string name, FounderBackground background)
        {
            Id = id;
            Name = name;
            Background = background;
        }
    }
}
