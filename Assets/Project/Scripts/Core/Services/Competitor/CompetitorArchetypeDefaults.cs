using Project.Core.Definitions.Market;

namespace Project.Core.Services.Competitor
{
    /// <summary>
    /// Provides archetype-driven initialization defaults for AI competitors.
    /// All values are [Placeholder] — can be moved to data-driven tuning later.
    /// Defined in Plan 2J, GDD_10.5.
    /// </summary>
    public static class CompetitorArchetypeDefaults
    {
        /// <summary>
        /// Returns a tuple of initialization defaults for the given archetype.
        /// Fields: (CashStrength, Reputation, HiringStrength, ResearchStrength, LaunchCadence, RiskAppetite)
        /// All values use a 0–100 scale.
        /// </summary>
        public static (int CashStrength, int Reputation, int HiringStrength, int ResearchStrength, int LaunchCadence, int RiskAppetite)
            GetDefaults(CompetitorArchetype archetype)
        {
            switch (archetype)
            {
                case CompetitorArchetype.IncumbentGiant:       return (90, 80, 70, 60, 30, 20);
                case CompetitorArchetype.AggressiveStartup:    return (40, 30, 50, 40, 80, 80);
                case CompetitorArchetype.ResearchLab:          return (50, 60, 40, 90, 40, 50);
                case CompetitorArchetype.HardwareManufacturer: return (70, 60, 50, 50, 30, 30);
                case CompetitorArchetype.EnterpriseSpecialist: return (60, 70, 60, 40, 40, 30);
                case CompetitorArchetype.ConsumerBrand:        return (60, 80, 50, 30, 50, 40);
                case CompetitorArchetype.LowCostCompetitor:    return (30, 30, 30, 20, 60, 50);
                case CompetitorArchetype.PlatformHolder:       return (80, 90, 60, 70, 30, 30);
                default:                                       return (50, 50, 50, 50, 50, 50);
            }
        }
    }
}
