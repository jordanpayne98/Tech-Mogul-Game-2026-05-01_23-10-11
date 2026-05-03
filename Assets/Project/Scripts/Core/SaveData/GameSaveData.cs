using System.Collections.Generic;
using Project.Core.SaveData.Company;
using Project.Core.SaveData.Contract;
using Project.Core.SaveData.Employee;
using Project.Core.SaveData.Finance;
using Project.Core.SaveData.Market;
using Project.Core.SaveData.Product;
using Project.Core.SaveData.Report;
using Project.Core.SaveData.Research;
using Project.Core.SaveData.Team;
using Project.Core.SaveData.Time;

namespace Project.Core.SaveData
{
    /// <summary>
    /// Root container for all save file data.
    /// Mirrors the layout of <c>GameSessionState</c> using serialization-safe types only.
    /// No Unity dependencies. No runtime logic. No validation.
    /// Validation and mapping are handled by Plan 3B.
    /// </summary>
    public sealed class GameSaveData
    {
        // ─── Schema version ───────────────────────────────────────────────────────

        /// <summary>Current schema version. Increment when the save format changes in a breaking way.</summary>
        public const int CurrentSaveVersion = 1;

        // ─── Save metadata ────────────────────────────────────────────────────────

        public int SaveVersion;
        public string SaveId;
        public string SaveName;

        /// <summary>UTC timestamp as ISO 8601 string (e.g. "2025-01-15T12:00:00Z").</summary>
        public string SavedAtUtcIso8601;

        /// <summary>
        /// Serialized save type. Valid values: "Manual", "Autosave", "QuickSave".
        /// Stored as string because the SaveType enum is created in Plan 3D.
        /// </summary>
        public string SaveType;

        // ─── RNG state ────────────────────────────────────────────────────────────

        public RandomStateSaveData RandomState;

        // ─── Core identity ────────────────────────────────────────────────────────

        public CompanySaveData Company;
        public FounderSaveData Founder;

        // ─── Company state ────────────────────────────────────────────────────────

        public CompanyStateSaveData CompanyState;

        // ─── Time state ───────────────────────────────────────────────────────────

        public TimeSaveData Time;

        // ─── Finance state ────────────────────────────────────────────────────────

        public FinanceSaveData Finance;

        // ─── Recruitment state ────────────────────────────────────────────────────

        public RecruitmentSaveData Recruitment;

        // ─── Inbox state ──────────────────────────────────────────────────────────

        public InboxSaveData Inbox;

        // ─── Research state ───────────────────────────────────────────────────────

        public ResearchSaveData Research;

        // ─── Employee lists ───────────────────────────────────────────────────────

        public List<EmployeeSaveData> Employees;
        public List<EmployeeStateSaveData> EmployeeStates;

        // ─── Candidate lists ──────────────────────────────────────────────────────

        public List<CandidateSaveData> Candidates;
        public List<CandidateStateSaveData> CandidateStates;

        // ─── Team lists ───────────────────────────────────────────────────────────

        public List<TeamSaveData> Teams;
        public List<TeamStateSaveData> TeamStates;
        public List<AssignmentSaveData> Assignments;

        // ─── Product lists ────────────────────────────────────────────────────────

        public List<ProductSaveData> Products;
        public List<ProductStateSaveData> ProductStates;
        public List<SoftwareMetricsSaveData> SoftwareMetrics;
        public List<HardwareMetricsSaveData> HardwareMetrics;
        public List<ProductBudgetSaveData> ProductBudgets;

        // ─── Contract lists ───────────────────────────────────────────────────────

        public List<ContractSaveData> Contracts;
        public List<ContractStateSaveData> ContractStates;

        // ─── Market lists ─────────────────────────────────────────────────────────

        public List<CompetitorSaveData> Competitors;
        public List<CompetitorStateSaveData> CompetitorStates;
        public List<MarketCategorySaveData> MarketCategories;
        public List<TrendSaveData> Trends;

        // ─── Report lists ─────────────────────────────────────────────────────────

        public List<ReportSaveData> Reports;

        // ─── Research lists ───────────────────────────────────────────────────────

        public List<ResearchProjectSaveData> ResearchProjects;

        // ─── Job post lists ───────────────────────────────────────────────────────

        public List<JobPostSaveData> JobPosts;
        public List<JobPostStateSaveData> JobPostStates;

        // ─── Finance record lists ─────────────────────────────────────────────────

        public List<TransactionSaveData> Transactions;
        public List<MonthlyFinanceSummarySaveData> MonthlyFinanceSummaries;
    }
}
