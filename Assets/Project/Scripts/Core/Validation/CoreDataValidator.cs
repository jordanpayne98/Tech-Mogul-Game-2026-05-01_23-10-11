using System.Collections.Generic;
using Project.Core.Definitions.Research;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Team;
using Project.Core.Runtime.Time;
using Project.Core.Validation.Domain;

namespace Project.Core.Validation
{
    /// <summary>
    /// Top-level aggregator that delegates to all domain-specific validators.
    /// Provides per-type Validate methods and a ValidateAll method for batch validation.
    ///
    /// Does not validate cross-entity existence (e.g. does not check whether team member IDs
    /// refer to existing employees). Shape-only structural validation only.
    /// </summary>
    public sealed class CoreDataValidator
    {
        // -------------------------------------------------------------------------
        // Domain validators
        // -------------------------------------------------------------------------

        private readonly TimeDataValidator     _timeValidator;
        private readonly CompanyDataValidator  _companyValidator;
        private readonly EmployeeDataValidator _employeeValidator;
        private readonly TeamDataValidator     _teamValidator;
        private readonly ProductDataValidator  _productValidator;
        private readonly ContractDataValidator _contractValidator;
        private readonly MarketDataValidator   _marketValidator;
        private readonly FinanceDataValidator  _financeValidator;
        private readonly ReportDataValidator   _reportValidator;
        private readonly ResearchDataValidator _researchValidator;

        // -------------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------------

        public CoreDataValidator()
        {
            _timeValidator     = new TimeDataValidator();
            _companyValidator  = new CompanyDataValidator();
            _employeeValidator = new EmployeeDataValidator();
            _teamValidator     = new TeamDataValidator();
            _productValidator  = new ProductDataValidator();
            _contractValidator = new ContractDataValidator();
            _marketValidator   = new MarketDataValidator();
            _financeValidator  = new FinanceDataValidator();
            _reportValidator   = new ReportDataValidator();
            _researchValidator = new ResearchDataValidator();
        }

        // -------------------------------------------------------------------------
        // Time
        // -------------------------------------------------------------------------

        public ValidationResult Validate(GameDateTime target)    => _timeValidator.Validate(target);
        public ValidationResult Validate(TimeRuntimeState target) => _timeValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Company
        // -------------------------------------------------------------------------

        public ValidationResult Validate(CompanyProfile target)      => _companyValidator.Validate(target);
        public ValidationResult Validate(FounderProfile target)      => _companyValidator.Validate(target);
        public ValidationResult Validate(CompanyRuntimeState target) => _companyValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Employee
        // -------------------------------------------------------------------------

        public ValidationResult Validate(EmployeeProfile target)         => _employeeValidator.Validate(target);
        public ValidationResult Validate(CandidateProfile target)        => _employeeValidator.Validate(target);
        public ValidationResult Validate(JobPostProfile target)          => _employeeValidator.Validate(target);
        public ValidationResult Validate(EmployeeRuntimeState target)    => _employeeValidator.Validate(target);
        public ValidationResult Validate(CandidateRuntimeState target)   => _employeeValidator.Validate(target);
        public ValidationResult Validate(JobPostRuntimeState target)     => _employeeValidator.Validate(target);
        public ValidationResult Validate(RecruitmentRuntimeState target) => _employeeValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Team
        // -------------------------------------------------------------------------

        public ValidationResult Validate(TeamProfile target)            => _teamValidator.Validate(target);
        public ValidationResult Validate(TeamRuntimeState target)       => _teamValidator.Validate(target);
        public ValidationResult Validate(AssignmentRuntimeState target) => _teamValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Product
        // -------------------------------------------------------------------------

        public ValidationResult Validate(ProductProfile target)         => _productValidator.Validate(target);
        public ValidationResult Validate(ProductBudgetProfile target)   => _productValidator.Validate(target);
        public ValidationResult Validate(ProductRuntimeState target)    => _productValidator.Validate(target);
        public ValidationResult Validate(SoftwareRuntimeMetrics target) => _productValidator.Validate(target);
        public ValidationResult Validate(HardwareRuntimeMetrics target) => _productValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Contract
        // -------------------------------------------------------------------------

        public ValidationResult Validate(ContractProfile target)      => _contractValidator.Validate(target);
        public ValidationResult Validate(ContractRuntimeState target) => _contractValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Market
        // -------------------------------------------------------------------------

        public ValidationResult Validate(MarketCategoryRuntimeState target) => _marketValidator.Validate(target);
        public ValidationResult Validate(CompetitorProfile target)          => _marketValidator.Validate(target);
        public ValidationResult Validate(CompetitorRuntimeState target)     => _marketValidator.Validate(target);
        public ValidationResult Validate(TrendRuntimeState target)          => _marketValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Finance
        // -------------------------------------------------------------------------

        public ValidationResult Validate(FinanceRuntimeState target)    => _financeValidator.Validate(target);
        public ValidationResult Validate(TransactionRecord target)      => _financeValidator.Validate(target);
        public ValidationResult Validate(MonthlyFinanceSummary target)  => _financeValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Report
        // -------------------------------------------------------------------------

        public ValidationResult Validate(ReportProfile target)      => _reportValidator.Validate(target);
        public ValidationResult Validate(InboxRuntimeState target)  => _reportValidator.Validate(target);

        // -------------------------------------------------------------------------
        // Research
        // -------------------------------------------------------------------------

        public ValidationResult Validate(ResearchProjectDefinition target)   => _researchValidator.Validate(target);
        public ValidationResult Validate(ResearchProjectRuntimeState target) => _researchValidator.Validate(target);
        public ValidationResult Validate(ResearchRuntimeState target)        => _researchValidator.Validate(target);

        // -------------------------------------------------------------------------
        // ValidateAll — batch validation across all provided domain collections
        // -------------------------------------------------------------------------

        /// <summary>
        /// Validates all provided domain data collections and returns a merged ValidationResult.
        /// Any argument may be null — null collections are safely skipped.
        /// Does not validate cross-entity existence.
        /// </summary>
        public ValidationResult ValidateAll(
            // Time
            GameDateTime currentDate                                = null,
            TimeRuntimeState timeState                              = null,

            // Company
            CompanyProfile companyProfile                           = null,
            FounderProfile founderProfile                           = null,
            CompanyRuntimeState companyState                        = null,

            // Employee
            IEnumerable<EmployeeProfile> employeeProfiles          = null,
            IEnumerable<CandidateProfile> candidateProfiles        = null,
            IEnumerable<JobPostProfile> jobPostProfiles            = null,
            IEnumerable<EmployeeRuntimeState> employeeStates       = null,
            IEnumerable<CandidateRuntimeState> candidateStates     = null,
            IEnumerable<JobPostRuntimeState> jobPostStates         = null,
            RecruitmentRuntimeState recruitmentState               = null,

            // Team
            IEnumerable<TeamProfile> teamProfiles                  = null,
            IEnumerable<TeamRuntimeState> teamStates               = null,
            IEnumerable<AssignmentRuntimeState> assignmentStates   = null,

            // Product
            IEnumerable<ProductProfile> productProfiles            = null,
            IEnumerable<ProductBudgetProfile> productBudgets       = null,
            IEnumerable<ProductRuntimeState> productStates         = null,
            IEnumerable<SoftwareRuntimeMetrics> softwareMetrics    = null,
            IEnumerable<HardwareRuntimeMetrics> hardwareMetrics    = null,

            // Contract
            IEnumerable<ContractProfile> contractProfiles          = null,
            IEnumerable<ContractRuntimeState> contractStates       = null,

            // Market
            IEnumerable<MarketCategoryRuntimeState> marketStates   = null,
            IEnumerable<CompetitorProfile> competitorProfiles      = null,
            IEnumerable<CompetitorRuntimeState> competitorStates   = null,
            IEnumerable<TrendRuntimeState> trendStates             = null,

            // Finance
            FinanceRuntimeState financeState                       = null,
            IEnumerable<TransactionRecord> transactions            = null,
            IEnumerable<MonthlyFinanceSummary> monthlySummaries    = null,

            // Report
            IEnumerable<ReportProfile> reportProfiles              = null,
            InboxRuntimeState inboxState                           = null,

            // Research
            IEnumerable<ResearchProjectDefinition> researchDefs    = null,
            IEnumerable<ResearchProjectRuntimeState> researchProjectStates = null,
            ResearchRuntimeState researchState                     = null)
        {
            ValidationResult result = ValidationResult.Valid();

            // Time.
            if (currentDate  != null) result = result.Merge(Validate(currentDate));
            if (timeState    != null) result = result.Merge(Validate(timeState));

            // Company.
            if (companyProfile  != null) result = result.Merge(Validate(companyProfile));
            if (founderProfile  != null) result = result.Merge(Validate(founderProfile));
            if (companyState    != null) result = result.Merge(Validate(companyState));

            // Employee.
            result = MergeCollection(result, employeeProfiles,   Validate);
            result = MergeCollection(result, candidateProfiles,  Validate);
            result = MergeCollection(result, jobPostProfiles,    Validate);
            result = MergeCollection(result, employeeStates,     Validate);
            result = MergeCollection(result, candidateStates,    Validate);
            result = MergeCollection(result, jobPostStates,      Validate);
            if (recruitmentState != null) result = result.Merge(Validate(recruitmentState));

            // Team.
            result = MergeCollection(result, teamProfiles,      Validate);
            result = MergeCollection(result, teamStates,        Validate);
            result = MergeCollection(result, assignmentStates,  Validate);

            // Product.
            result = MergeCollection(result, productProfiles,   Validate);
            result = MergeCollection(result, productBudgets,    Validate);
            result = MergeCollection(result, productStates,     Validate);
            result = MergeCollection(result, softwareMetrics,   Validate);
            result = MergeCollection(result, hardwareMetrics,   Validate);

            // Contract.
            result = MergeCollection(result, contractProfiles,  Validate);
            result = MergeCollection(result, contractStates,    Validate);

            // Market.
            result = MergeCollection(result, marketStates,      Validate);
            result = MergeCollection(result, competitorProfiles, Validate);
            result = MergeCollection(result, competitorStates,  Validate);
            result = MergeCollection(result, trendStates,       Validate);

            // Finance.
            if (financeState != null) result = result.Merge(Validate(financeState));
            result = MergeCollection(result, transactions,      Validate);
            result = MergeCollection(result, monthlySummaries,  Validate);

            // Report.
            result = MergeCollection(result, reportProfiles,    Validate);
            if (inboxState != null) result = result.Merge(Validate(inboxState));

            // Research.
            result = MergeCollection(result, researchDefs,             Validate);
            result = MergeCollection(result, researchProjectStates,    Validate);
            if (researchState != null) result = result.Merge(Validate(researchState));

            return result;
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private delegate ValidationResult ValidateDelegate<T>(T item);

        private static ValidationResult MergeCollection<T>(
            ValidationResult current,
            IEnumerable<T> collection,
            ValidateDelegate<T> validate)
        {
            if (collection == null)
            {
                return current;
            }

            foreach (T item in collection)
            {
                if (item != null)
                {
                    current = current.Merge(validate(item));
                }
            }

            return current;
        }
    }
}
