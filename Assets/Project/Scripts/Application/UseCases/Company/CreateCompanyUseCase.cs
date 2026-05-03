using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Report;
using Project.Core.Events.Company;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Company;
using Project.Core.Results.Company;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;

namespace Project.Application.UseCases.Company
{
    /// <summary>
    /// Application-layer use case that coordinates full new-game state initialization.
    /// Creates company, founder, and all required empty shell states, then assembles
    /// them into a GameSessionState and publishes CompanyCreatedEvent.
    ///
    /// All validation occurs before any state is created.
    /// Does not populate candidates, contracts, market, or competitors — deferred to Plans 2C, 2G, 2I, 2J.
    /// </summary>
    public sealed class CreateCompanyUseCase
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyTuning _companyTuning;
        private readonly IEventBus _eventBus;

        public CreateCompanyUseCase(ICompanyService companyService, ICompanyTuning companyTuning, IEventBus eventBus)
        {
            _companyService = companyService;
            _companyTuning  = companyTuning;
            _eventBus       = eventBus;
        }

        /// <summary>
        /// Executes the company creation flow.
        /// Returns a failed result if validation fails; returns a succeeded result with a fully populated
        /// GameSessionState on success.
        /// </summary>
        public CreateCompanyResult Execute(CreateCompanyRequest request)
        {
            // ─── 1. Validate request ───────────────────────────────────────────────

            if (request == null)
            {
                return CreateCompanyResult.Failed("Request is null");
            }

            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                return CreateCompanyResult.Failed("Company name is required");
            }

            if (string.IsNullOrWhiteSpace(request.FounderName))
            {
                return CreateCompanyResult.Failed("Founder name is required");
            }

            // ─── 2. Create time state ──────────────────────────────────────────────

            var timeState = TimeRuntimeState.CreateDefault();

            // ─── 3. Create company and founder ─────────────────────────────────────

            var founderProfile = _companyService.CreateFounderProfile(request);
            var companyProfile = _companyService.CreateCompanyProfile(request, timeState.CurrentDate);
            var companyState   = _companyService.InitializeCompanyState(companyProfile.Id, request.Background, _companyTuning);

            // ─── 4. Resolve starting cash ──────────────────────────────────────────

            long startingCash = _companyService.ResolveStartingCash(request.CapitalPreset, _companyTuning);

            // ─── 5. Create finance state ───────────────────────────────────────────

            var financeState = new FinanceRuntimeState
            {
                CompanyId        = companyProfile.Id,
                CashMinorUnits   = startingCash,
                RunwayMonths     = 0,         // Calculated by finance service in Plan 2H.
                IsRunwayStable   = true       // No burn rate at creation — stable by definition.
            };

            // ─── 6. Create empty recruitment state ─────────────────────────────────

            var recruitmentState = new RecruitmentRuntimeState
            {
                CandidateIds                   = new List<string>(),
                JobPostIds                     = new List<string>(),
                LastCandidatePoolRefreshDate   = null
            };

            // ─── 7. Create empty inbox state ───────────────────────────────────────

            var inboxState = new InboxRuntimeState
            {
                CompanyId            = companyProfile.Id,
                ReportIds            = new List<string>(),
                ReportReadStates     = new System.Collections.Generic.Dictionary<string, ReportReadState>(),
                ReportInboxStates    = new System.Collections.Generic.Dictionary<string, ReportInboxState>(),
                UnreadCount          = 0,
                DecisionRequiredCount = 0
            };

            // ─── 8. Create empty research state ────────────────────────────────────

            var researchState = new ResearchRuntimeState
            {
                CompanyId              = companyProfile.Id,
                AvailableProjectIds    = new List<string>(),
                ActiveProjectIds       = new List<string>(),
                CompletedProjectIds    = new List<string>(),
                ObsoleteProjectIds     = new List<string>(),
                UnlockedCapabilityIds  = new List<string>()
            };

            // ─── 9. Assemble GameSessionState ──────────────────────────────────────

            var sessionState = new GameSessionState
            {
                CompanyProfile   = companyProfile,
                FounderProfile   = founderProfile,
                CompanyState     = companyState,
                TimeState        = timeState,
                FinanceState     = financeState,
                RecruitmentState = recruitmentState,
                InboxState       = inboxState,
                ResearchState    = researchState
                // All list properties are initialized to empty lists by GameSessionState constructor.
            };

            // ─── 10. Publish event ─────────────────────────────────────────────────

            _eventBus.Publish(new CompanyCreatedEvent(companyProfile.Id, founderProfile.Id, startingCash));

            // ─── 11. Log ───────────────────────────────────────────────────────────

            DebugLogger.Log(DebugCategory.Simulation,
                $"Company '{companyProfile.Name}' created with {startingCash} minor units");

            // ─── 12. Return ────────────────────────────────────────────────────────

            return CreateCompanyResult.Succeeded(companyProfile.Id, founderProfile.Id, sessionState);
        }
    }
}
