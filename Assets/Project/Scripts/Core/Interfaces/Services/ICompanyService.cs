using Project.Core.Definitions.Company;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Company;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Time;

namespace Project.Core.Interfaces.Services
{
    /// <summary>
    /// Company domain service interface.
    /// Stateless — all configuration arrives via method parameters.
    /// Does not create cross-domain states (finance, time, recruitment, etc.).
    /// Does not publish events.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Creates an immutable company profile from request data.
        /// Generates the stable ID "company.player".
        /// </summary>
        CompanyProfile CreateCompanyProfile(CreateCompanyRequest request, GameDateTime foundedDate);

        /// <summary>
        /// Creates an immutable founder profile from request data.
        /// Generates the stable ID "founder.player".
        /// </summary>
        FounderProfile CreateFounderProfile(CreateCompanyRequest request);

        /// <summary>
        /// Creates a mutable company runtime state with default reputation plus the founder background bonus.
        /// </summary>
        CompanyRuntimeState InitializeCompanyState(string companyId, FounderBackground background, ICompanyTuning tuning);

        /// <summary>
        /// Maps a CapitalPreset to its starting cash value in minor currency units via tuning.
        /// </summary>
        long ResolveStartingCash(CapitalPreset preset, ICompanyTuning tuning);
    }
}
