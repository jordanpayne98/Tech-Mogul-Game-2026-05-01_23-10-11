using Project.Core.Interfaces.Tuning;

namespace Project.Core.Interfaces
{
    /// <summary>
    /// Interface allowing Core and Application layers to read tuning values
    /// without depending on Infrastructure.
    /// Concrete implementation lives in Project.Infrastructure.Tuning.TuningConfig.
    /// ITuningConfig acts as the aggregate root extending all domain tuning interfaces.
    /// Each domain service depends only on the specific tuning interface it requires.
    /// </summary>
    public interface ITuningConfig :
        ITimeTuning,
        ICompanyTuning,
        ITeamTuning,
        IEmployeeTuning,
        IProductTuning,
        IContractTuning,
        IFinanceTuning,
        IEventCrisisTuning,
        IResearchTuning,
        IMarketTuning,
        ICompetitorTuning,
        ISaveTuning
    {
        // General
        float DefaultActionDelaySeconds { get; }

        // Validation
        int MaxGeneratedItems { get; }

        // Timing
        float UIRefreshIntervalSeconds { get; }

        // SaveLoad
        float AutosaveIntervalSeconds { get; }
        int MaxSaveSlots { get; }
    }
}
