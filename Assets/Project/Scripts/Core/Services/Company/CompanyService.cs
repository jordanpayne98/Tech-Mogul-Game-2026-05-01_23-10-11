using Project.Core.Debugging;
using Project.Core.Definitions.Company;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Company;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Company
{
    /// <summary>
    /// Concrete company domain service.
    /// Stateless — no constructor dependencies. All configuration arrives via method parameters.
    /// Generates stable IDs "company.player" and "founder.player" for the single-player company and founder.
    /// </summary>
    public sealed class CompanyService : ICompanyService
    {
        private const string CompanyStableId = "company.player";
        private const string FounderStableId = "founder.player";

        // ─── ICompanyService ──────────────────────────────────────────────────────

        /// <inheritdoc/>
        public CompanyProfile CreateCompanyProfile(CreateCompanyRequest request, GameDateTime foundedDate)
        {
            if (string.IsNullOrWhiteSpace(request?.CompanyName))
            {
                DebugLogger.LogError(DebugCategory.Simulation, "CreateCompanyProfile: CompanyName is null or whitespace.");
                return null;
            }

            var profile = new CompanyProfile(
                CompanyStableId,
                request.CompanyName,
                FounderStableId,
                request.LogoIconId,
                request.BrandColourHex,
                request.Focus,
                request.Location,
                foundedDate);

            DebugLogger.Log(DebugCategory.Simulation,
                $"CompanyProfile created: id='{CompanyStableId}', name='{request.CompanyName}'");

            return profile;
        }

        /// <inheritdoc/>
        public FounderProfile CreateFounderProfile(CreateCompanyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.FounderName))
            {
                DebugLogger.LogError(DebugCategory.Simulation, "CreateFounderProfile: FounderName is null or whitespace.");
                return null;
            }

            var profile = new FounderProfile(FounderStableId, request.FounderName, request.Background);

            DebugLogger.Log(DebugCategory.Simulation,
                $"FounderProfile created: id='{FounderStableId}', name='{request.FounderName}', background='{request.Background}'");

            return profile;
        }

        /// <inheritdoc/>
        public CompanyRuntimeState InitializeCompanyState(string companyId, FounderBackground background, ICompanyTuning tuning)
        {
            int reputation = tuning.DefaultReputation + GetFounderReputationBonus(background, tuning);

            var state = new CompanyRuntimeState
            {
                CompanyId  = companyId,
                Reputation = reputation
            };

            DebugLogger.Log(DebugCategory.Simulation,
                $"CompanyRuntimeState initialized: companyId='{companyId}', reputation={reputation} (base={tuning.DefaultReputation}, bonus={GetFounderReputationBonus(background, tuning)})");

            return state;
        }

        /// <inheritdoc/>
        public long ResolveStartingCash(CapitalPreset preset, ICompanyTuning tuning)
        {
            long cash = preset switch
            {
                CapitalPreset.Garage       => tuning.GarageCashMinorUnits,
                CapitalPreset.Bootstrapped => tuning.BootstrappedCashMinorUnits,
                CapitalPreset.SeedFunded   => tuning.SeedFundedCashMinorUnits,
                CapitalPreset.VentureStart => tuning.VentureStartCashMinorUnits,
                CapitalPreset.Sandbox      => tuning.SandboxDefaultCashMinorUnits,
                _                          => tuning.BootstrappedCashMinorUnits
            };

            DebugLogger.Log(DebugCategory.Simulation,
                $"ResolveStartingCash: preset='{preset}', cash={cash} minor units");

            return cash;
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private static int GetFounderReputationBonus(FounderBackground background, ICompanyTuning tuning)
        {
            return background switch
            {
                FounderBackground.Engineer            => tuning.EngineerReputationBonus,
                FounderBackground.ProductDesigner     => tuning.ProductDesignerReputationBonus,
                FounderBackground.SalesFounder        => tuning.SalesFounderReputationBonus,
                FounderBackground.HardwareSpecialist  => tuning.HardwareSpecialistReputationBonus,
                FounderBackground.ResearchFounder     => tuning.ResearchFounderReputationBonus,
                FounderBackground.SerialFounder       => tuning.SerialFounderReputationBonus,
                FounderBackground.BootstrappedFounder => tuning.BootstrappedFounderReputationBonus,
                _                                     => 0
            };
        }
    }
}
