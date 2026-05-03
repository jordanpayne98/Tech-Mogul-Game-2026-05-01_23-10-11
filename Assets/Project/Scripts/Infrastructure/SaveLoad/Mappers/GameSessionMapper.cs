using System;
using System.Collections.Generic;
using Project.Core.Definitions.SaveLoad;
using Project.Core.Runtime;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Team;
using Project.Core.SaveData;
using Project.Core.SaveData.Report;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Root mapper orchestrating all domain mappers.
    /// Converts GameSessionState to GameSaveData and back.
    /// All methods are static — this mapper holds no state.
    /// Note: GameSaveData does not persist EventHistory, LastEventDates,
    /// CompetitorProductStates, or ContractBoardState — these are omitted per Plan 3A save schema.
    /// </summary>
    public static class GameSessionMapper
    {
        /// <summary>
        /// Converts a live GameSessionState into a fully populated GameSaveData ready for serialization.
        /// </summary>
        public static GameSaveData ToSaveData(
            GameSessionState session,
            string saveId,
            string saveName,
            SaveType saveType)
        {
            var data = new GameSaveData
            {
                // ── Metadata ──────────────────────────────────────────────────────
                SaveVersion       = GameSaveData.CurrentSaveVersion,
                SaveId            = saveId,
                SaveName          = saveName,
                SavedAtUtcIso8601 = DateTime.UtcNow.ToString("o"),
                SaveType          = saveType.ToString(),

                // ── RNG ───────────────────────────────────────────────────────────
                RandomState = RandomStateSaveMapper.ToSaveData(session.RandomState),

                // ── Core identity ─────────────────────────────────────────────────
                Company     = CompanySaveMapper.ToSaveData(session.CompanyProfile),
                Founder     = CompanySaveMapper.ToSaveData(session.FounderProfile),
                CompanyState = CompanySaveMapper.ToSaveData(session.CompanyState),

                // ── Time ──────────────────────────────────────────────────────────
                Time = TimeSaveMapper.ToSaveData(session.TimeState),

                // ── Finance ───────────────────────────────────────────────────────
                Finance = FinanceSaveMapper.ToSaveData(session.FinanceState),

                // ── Recruitment ───────────────────────────────────────────────────
                Recruitment = EmployeeSaveMapper.ToSaveData(session.RecruitmentState),

                // ── Inbox ─────────────────────────────────────────────────────────
                Inbox = ReportSaveMapper.ToSaveData(session.InboxState),

                // ── Research ──────────────────────────────────────────────────────
                Research = ResearchSaveMapper.ToSaveData(session.ResearchState)
            };

            // ── Employee lists ────────────────────────────────────────────────────
            data.Employees = MapList(session.EmployeeProfiles, EmployeeSaveMapper.ToSaveData);
            data.EmployeeStates = MapList(session.EmployeeStates, EmployeeSaveMapper.ToSaveData);

            // ── Candidate lists ───────────────────────────────────────────────────
            data.Candidates = MapList(session.CandidateProfiles, EmployeeSaveMapper.ToSaveData);
            data.CandidateStates = MapList(session.CandidateStates, EmployeeSaveMapper.ToSaveData);

            // ── Team lists ────────────────────────────────────────────────────────
            data.Teams = MapList(session.TeamProfiles, TeamSaveMapper.ToSaveData);
            data.TeamStates = MapList(session.TeamStates, TeamSaveMapper.ToSaveData);
            data.Assignments = MapList(session.AssignmentStates, TeamSaveMapper.ToSaveData);

            // ── Product lists ─────────────────────────────────────────────────────
            data.Products = MapList(session.ProductProfiles, ProductSaveMapper.ToSaveData);
            data.ProductStates = MapList(session.ProductStates, ProductSaveMapper.ToSaveData);
            data.SoftwareMetrics = MapList(session.SoftwareMetrics, ProductSaveMapper.ToSaveData);
            data.HardwareMetrics = MapList(session.HardwareMetrics, ProductSaveMapper.ToSaveData);
            data.ProductBudgets = MapList(session.ProductBudgets, ProductSaveMapper.ToSaveData);

            // ── Contract lists ────────────────────────────────────────────────────
            data.Contracts = MapList(session.ContractProfiles, ContractSaveMapper.ToSaveData);
            data.ContractStates = MapList(session.ContractStates, ContractSaveMapper.ToSaveData);

            // ── Market lists ──────────────────────────────────────────────────────
            data.Competitors = MapList(session.CompetitorProfiles, MarketSaveMapper.ToSaveData);
            data.CompetitorStates = MapList(session.CompetitorStates, MarketSaveMapper.ToSaveData);
            data.MarketCategories = MapList(session.MarketCategoryStates, MarketSaveMapper.ToSaveData);
            data.Trends = MapList(session.TrendStates, MarketSaveMapper.ToSaveData);

            // ── Report lists ──────────────────────────────────────────────────────
            // Reports carry read/inbox state denormalized from InboxRuntimeState.
            data.Reports = new List<ReportSaveData>(session.ReportProfiles.Count);
            foreach (ReportProfile report in session.ReportProfiles)
            {
                data.Reports.Add(ReportSaveMapper.ToSaveData(report, session.InboxState));
            }

            // ── Research project lists ────────────────────────────────────────────
            data.ResearchProjects = MapList(session.ResearchProjectStates, ResearchSaveMapper.ToSaveData);

            // ── Job post lists ────────────────────────────────────────────────────
            data.JobPosts = MapList(session.JobPostProfiles, EmployeeSaveMapper.ToSaveData);
            data.JobPostStates = MapList(session.JobPostStates, EmployeeSaveMapper.ToSaveData);

            // ── Finance record lists ──────────────────────────────────────────────
            data.Transactions = MapList(session.TransactionRecords, FinanceSaveMapper.ToSaveData);
            data.MonthlyFinanceSummaries = MapList(session.MonthlyFinanceSummaries, FinanceSaveMapper.ToSaveData);

            return data;
        }

        /// <summary>
        /// Reconstructs a GameSessionState from a deserialized GameSaveData.
        /// </summary>
        public static GameSessionState FromSaveData(GameSaveData data)
        {
            var session = new GameSessionState
            {
                // ── Session identity ──────────────────────────────────────────────
                // SessionId is derived from SaveId on load — preserves the stable reference.
                SessionId = data.SaveId,

                // ── RNG ───────────────────────────────────────────────────────────
                RandomState = RandomStateSaveMapper.FromSaveData(data.RandomState),

                // ── Core identity ─────────────────────────────────────────────────
                CompanyProfile = CompanySaveMapper.FromSaveData(data.Company),
                FounderProfile = CompanySaveMapper.FromSaveData(data.Founder),
                CompanyState   = CompanySaveMapper.FromSaveData(data.CompanyState),

                // ── Time ──────────────────────────────────────────────────────────
                TimeState = TimeSaveMapper.FromSaveData(data.Time),

                // ── Finance ───────────────────────────────────────────────────────
                FinanceState = FinanceSaveMapper.FromSaveData(data.Finance),

                // ── Recruitment ───────────────────────────────────────────────────
                RecruitmentState = EmployeeSaveMapper.FromSaveData(data.Recruitment),

                // ── Inbox ─────────────────────────────────────────────────────────
                InboxState = ReportSaveMapper.FromSaveData(data.Inbox, data.Reports),

                // ── Research ──────────────────────────────────────────────────────
                ResearchState = ResearchSaveMapper.FromSaveData(data.Research)
            };

            // ── Employee lists ────────────────────────────────────────────────────
            session.EmployeeProfiles = MapList(data.Employees, EmployeeSaveMapper.FromSaveData);
            session.EmployeeStates = MapList(data.EmployeeStates, EmployeeSaveMapper.FromSaveData);

            // ── Candidate lists ───────────────────────────────────────────────────
            session.CandidateProfiles = MapList(data.Candidates, EmployeeSaveMapper.FromSaveData);
            session.CandidateStates = MapList(data.CandidateStates, EmployeeSaveMapper.FromSaveData);

            // ── Team lists ────────────────────────────────────────────────────────
            session.TeamProfiles = MapList(data.Teams, TeamSaveMapper.FromSaveData);
            session.TeamStates = MapList(data.TeamStates, TeamSaveMapper.FromSaveData);
            session.AssignmentStates = MapList(data.Assignments, TeamSaveMapper.FromSaveData);

            // ── Product lists ─────────────────────────────────────────────────────
            session.ProductProfiles = MapList(data.Products, ProductSaveMapper.FromSaveData);
            session.ProductStates = MapList(data.ProductStates, ProductSaveMapper.FromSaveData);
            session.SoftwareMetrics = MapList(data.SoftwareMetrics, ProductSaveMapper.FromSaveData);
            session.HardwareMetrics = MapList(data.HardwareMetrics, ProductSaveMapper.FromSaveData);
            session.ProductBudgets = MapList(data.ProductBudgets, ProductSaveMapper.FromSaveData);

            // ── Contract lists ────────────────────────────────────────────────────
            session.ContractProfiles = MapList(data.Contracts, ContractSaveMapper.FromSaveData);
            session.ContractStates = MapList(data.ContractStates, ContractSaveMapper.FromSaveData);

            // ── Market lists ──────────────────────────────────────────────────────
            session.CompetitorProfiles = MapList(data.Competitors, MarketSaveMapper.FromSaveData);
            session.CompetitorStates = MapList(data.CompetitorStates, MarketSaveMapper.FromSaveData);
            session.MarketCategoryStates = MapList(data.MarketCategories, MarketSaveMapper.FromSaveData);
            session.TrendStates = MapList(data.Trends, MarketSaveMapper.FromSaveData);

            // ── Report lists ──────────────────────────────────────────────────────
            session.ReportProfiles = MapList(data.Reports, ReportSaveMapper.FromSaveData);

            // ── Research project lists ────────────────────────────────────────────
            session.ResearchProjectStates = MapList(data.ResearchProjects, ResearchSaveMapper.FromSaveData);

            // ── Job post lists ────────────────────────────────────────────────────
            session.JobPostProfiles = MapList(data.JobPosts, EmployeeSaveMapper.FromSaveData);
            session.JobPostStates = MapList(data.JobPostStates, EmployeeSaveMapper.FromSaveData);

            // ── Finance record lists ──────────────────────────────────────────────
            session.TransactionRecords = MapList(data.Transactions, FinanceSaveMapper.FromSaveData);
            session.MonthlyFinanceSummaries = MapList(data.MonthlyFinanceSummaries, FinanceSaveMapper.FromSaveData);

            return session;
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private static List<TOut> MapList<TIn, TOut>(List<TIn> source, Func<TIn, TOut> mapper)
        {
            if (source == null)
            {
                return new List<TOut>();
            }

            var result = new List<TOut>(source.Count);
            foreach (TIn item in source)
            {
                result.Add(mapper(item));
            }

            return result;
        }
    }
}
