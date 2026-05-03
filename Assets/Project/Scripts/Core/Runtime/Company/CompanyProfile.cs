using Project.Core.Definitions.Company;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Company
{
    /// <summary>
    /// Immutable player-created company identity data. Created once at game setup and read-only at runtime.
    /// This is a Profile (per-save player data), not a Definition (static design data).
    /// </summary>
    public sealed class CompanyProfile
    {
        /// <summary>Stable ID for persistent references across save/load.</summary>
        public string Id { get; }

        /// <summary>Display name of the company as entered by the player.</summary>
        public string Name { get; }

        /// <summary>Stable ID referencing the associated FounderProfile.</summary>
        public string FounderId { get; }

        /// <summary>Stable ID referencing the selected logo icon asset.</summary>
        public string LogoIconId { get; }

        /// <summary>Hex colour string for the company brand colour (e.g. "#FF5733").</summary>
        public string BrandColourHex { get; }

        /// <summary>
        /// The market focus identity of the company. Flavour only — must not affect gameplay mechanically.
        /// </summary>
        public CompanyFocus Focus { get; }

        /// <summary>Display name of the company's location as entered or selected by the player.</summary>
        public string Location { get; }

        /// <summary>The in-game date on which the company was founded.</summary>
        public GameDateTime FoundedDate { get; }

        public CompanyProfile(
            string id,
            string name,
            string founderId,
            string logoIconId,
            string brandColourHex,
            CompanyFocus focus,
            string location,
            GameDateTime foundedDate)
        {
            Id = id;
            Name = name;
            FounderId = founderId;
            LogoIconId = logoIconId;
            BrandColourHex = brandColourHex;
            Focus = focus;
            Location = location;
            FoundedDate = foundedDate;
        }
    }
}
