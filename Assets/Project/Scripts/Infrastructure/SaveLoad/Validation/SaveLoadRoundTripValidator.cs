using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Company;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Market;
using Project.Core.Definitions.Product;
using Project.Core.Definitions.Research;
using Project.Core.Definitions.Time;
using Project.Core.Interfaces.Services;
using Project.Core.Results.SaveLoad;
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
using Project.Core.Runtime.Time;

namespace Project.Infrastructure.SaveLoad.Validation
{
    /// <summary>
    /// Permanent debug tool that verifies the full save/load round-trip preserves all data.
    /// Creates a representative GameSessionState, saves it, loads it, and compares every domain.
    /// Cleans up all validation saves after completion. Never throws or crashes bootstrap.
    /// </summary>
    public sealed class SaveLoadRoundTripValidator
    {
        private readonly ISaveSlotManager _slotManager;

        private const string ValidatorName      = "SaveLoad RoundTrip";
        private const string ValidationSaveName = "_validation_round_trip";

        // ─── Constructor ──────────────────────────────────────────────────────────

        public SaveLoadRoundTripValidator(ISaveSlotManager slotManager)
        {
            _slotManager = slotManager ?? throw new ArgumentNullException(nameof(slotManager));
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Executes the full validation sequence: save → load → compare → cleanup.
        /// Logs pass/fail per domain and an overall summary.
        /// </summary>
        public void Run()
        {
            int    passCount  = 0;
            int    totalCount = 0;
            string savedId    = null;

            try
            {
                GameSessionState original = CreateTestSession();

                // ── Save ─────────────────────────────────────────────────────────

                SaveResult saveResult = _slotManager.RequestManualSave(original, ValidationSaveName);

                if (!saveResult.Success)
                {
                    DebugLogger.LogError(DebugCategory.Validation,
                        $"[Validation] {ValidatorName} — ABORTED: Save step failed: {saveResult.FailureReason}");
                    return;
                }

                savedId = saveResult.SaveId;

                // ── Load ─────────────────────────────────────────────────────────

                LoadResult loadResult = _slotManager.LoadSave(savedId);

                if (!loadResult.Success)
                {
                    DebugLogger.LogError(DebugCategory.Validation,
                        $"[Validation] {ValidatorName} — ABORTED: Load step failed: {loadResult.FailureReason}");
                    return;
                }

                GameSessionState loaded = loadResult.Session;

                // ── Compare per domain ────────────────────────────────────────────

                void Check(string domain, bool pass, string mismatch)
                {
                    totalCount++;
                    if (pass)
                    {
                        passCount++;
                        DebugLogger.Log(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — {domain}: PASS");
                    }
                    else
                    {
                        DebugLogger.LogError(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — {domain}: FAIL — {mismatch}");
                    }
                }

                Check("Session Identity", CompareSessionIdentity(original, loaded, out string m0),  m0);
                Check("RandomState",      CompareRandomState(original, loaded, out string m1),      m1);
                Check("Company",          CompareCompany(original, loaded, out string m2),          m2);
                Check("Founder",          CompareFounder(original, loaded, out string m3),          m3);
                Check("Time",             CompareTime(original, loaded, out string m4),             m4);
                Check("Finance",          CompareFinance(original, loaded, out string m5),          m5);
                Check("Recruitment",      CompareRecruitment(original, loaded, out string m6),      m6);
                Check("Inbox",            CompareInbox(original, loaded, out string m7),            m7);
                Check("Employee",         CompareEmployee(original, loaded, out string m8),         m8);
                Check("Teams",            CompareTeams(original, loaded, out string m9),            m9);
                Check("Products",         CompareProducts(original, loaded, out string m10),        m10);
                Check("Contracts",        CompareContracts(original, loaded, out string m11),       m11);
                Check("Market",           CompareMarket(original, loaded, out string m12),          m12);
                Check("Competitors",      CompareCompetitors(original, loaded, out string m13),     m13);
                Check("Research",         CompareResearch(original, loaded, out string m14),        m14);
                Check("Reports",          CompareReports(original, loaded, out string m15),         m15);
                Check("Transactions",     CompareTransactions(original, loaded, out string m16),    m16);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — EXCEPTION during validation: {ex.Message}");
            }
            finally
            {
                // ── Cleanup ───────────────────────────────────────────────────────
                if (!string.IsNullOrEmpty(savedId))
                {
                    try
                    {
                        DeleteResult deleteResult = _slotManager.DeleteSave(savedId);

                        if (!deleteResult.Success)
                        {
                            DebugLogger.LogWarning(DebugCategory.Validation,
                                $"[Validation] {ValidatorName} — Cleanup warning: Could not delete validation save " +
                                $"'{savedId}': {deleteResult.FailureReason}");
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        DebugLogger.LogWarning(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — Cleanup exception: {cleanupEx.Message}");
                    }
                }

                if (totalCount > 0)
                {
                    DebugLogger.Log(DebugCategory.Validation,
                        $"[Validation] {ValidatorName} — {passCount}/{totalCount} domains passed");
                }
            }
        }

        // ─── Test session factory ─────────────────────────────────────────────────

        /// <summary>
        /// Builds a representative session with known values in all domains.
        /// One entry per list — enough to exercise the full serialisation path.
        /// </summary>
        private static GameSessionState CreateTestSession()
        {
            const string companyId    = "company.001";
            const string founderId    = "founder.001";
            const string employeeId   = "employee.001";
            const string teamId       = "team.001";
            const string productId    = "product.001";
            const string contractId   = "contract.001";
            const string competitorId = "competitor.001";
            const string reportId     = "report.001";
            const string researchId   = "research.cloud_auto_scaling";
            const string trendId      = "trend.001";
            const string categoryId   = "mcat.001";
            const string txnId        = "txn.001";
            const string summaryId    = "summary.001";

            var startDate = new GameDateTime(2026, 1, 1, 9);

            var session = new GameSessionState
            {
                SessionId   = "validation-session-001",
                RandomState = new RandomRuntimeState { Seed = 42, CallCount = 10 },

                CompanyProfile = new CompanyProfile(
                    id:             companyId,
                    name:           "Test Corp",
                    founderId:      founderId,
                    logoIconId:     "logo.default",
                    brandColourHex: "#FF5733",
                    focus:          CompanyFocus.DeveloperTools,
                    location:       "London",
                    foundedDate:    startDate),

                FounderProfile = new FounderProfile(
                    id:         founderId,
                    name:       "Test Founder",
                    background: FounderBackground.Engineer),

                CompanyState = new CompanyRuntimeState
                {
                    CompanyId  = companyId,
                    Reputation = 50
                },

                TimeState = new TimeRuntimeState
                {
                    CurrentDate       = startDate,
                    TotalElapsedHours = startDate.TotalElapsedHours,
                    Speed             = TimeSpeed.Speed1x,
                    AdvanceMode       = TimeAdvanceMode.ContinueUntilInterrupt,
                    Filter            = InterruptionFilter.CriticalOnly,
                    IsAdvancing       = false
                },

                FinanceState = new FinanceRuntimeState
                {
                    CompanyId              = companyId,
                    CashMinorUnits         = 5_000_000L,
                    MonthlyPayrollMinorUnits = 100_000L,
                    RunwayMonths           = 50,
                    IsRunwayStable         = true,
                    LastPayrollDate        = startDate
                },

                RecruitmentState = new RecruitmentRuntimeState
                {
                    CandidateIds                 = new List<string> { "candidate.001" },
                    JobPostIds                   = new List<string>(),
                    LastCandidatePoolRefreshDate = startDate
                },

                InboxState = new InboxRuntimeState
                {
                    CompanyId     = companyId,
                    ReportIds     = new List<string> { reportId },
                    ReportReadStates = new Dictionary<string, Core.Definitions.Report.ReportReadState>
                    {
                        { reportId, Core.Definitions.Report.ReportReadState.Unread }
                    },
                    ReportInboxStates = new Dictionary<string, Core.Definitions.Report.ReportInboxState>
                    {
                        { reportId, Core.Definitions.Report.ReportInboxState.Active }
                    },
                    UnreadCount           = 1,
                    DecisionRequiredCount = 0
                },

                ResearchState = new ResearchRuntimeState
                {
                    CompanyId             = companyId,
                    AvailableProjectIds   = new List<string> { researchId },
                    ActiveProjectIds      = new List<string>(),
                    CompletedProjectIds   = new List<string>(),
                    ObsoleteProjectIds    = new List<string>(),
                    UnlockedCapabilityIds = new List<string>()
                }
            };

            // ── Employee ──────────────────────────────────────────────────────────

            session.EmployeeProfiles.Add(new EmployeeProfile
            {
                Id             = employeeId,
                Name           = "Alice Test",
                Traits         = new List<EmployeeTrait>(),
                WorkPreference = WorkPreference.Hybrid,
                HireDate       = startDate,
                PotentialMin   = 60,
                PotentialMax   = 80
            });

            session.EmployeeStates.Add(new EmployeeRuntimeState
            {
                EmployeeId = employeeId,
                Morale     = 75,
                Loyalty    = 80,
                Skills     = new Dictionary<SkillCategory, int>
                {
                    { SkillCategory.Engineering, 70 }
                }
            });

            // ── Teams ─────────────────────────────────────────────────────────────

            session.TeamProfiles.Add(new TeamProfile
            {
                Id          = teamId,
                Name        = "Alpha Team",
                Type        = Core.Definitions.Team.TeamType.Development,
                CreatedDate = startDate
            });

            session.TeamStates.Add(new TeamRuntimeState
            {
                TeamId              = teamId,
                MemberIds           = new List<string> { employeeId },
                Cohesion            = 70,
                Morale              = 80,
                CurrentAssignmentId = null
            });

            // ── Products ──────────────────────────────────────────────────────────

            session.ProductProfiles.Add(new ProductProfile
            {
                Id                   = productId,
                Name                 = "Test Product",
                Family               = ProductFamily.Software,
                Category             = ProductCategory.DevelopmentTool,
                RevenueModel         = RevenueModel.OneTimePurchase,
                PriceMinorUnits      = 9_900L,
                FeatureScope         = 5,
                QualityTarget        = 70,
                CreatedDate          = startDate,
                TargetReleaseDate    = startDate,
                SupportedPlatformIds = new List<string>()
            });

            session.ProductStates.Add(new ProductRuntimeState
            {
                ProductId                = productId,
                Status                   = ProductStatus.InDevelopment,
                ProgressPercent          = 0,
                AssignedTeamIds          = new List<string> { teamId },
                ScoreValues              = new Dictionary<ProductScoreDimension, int>(),
                MonthlyRevenueHistoryIds = new List<string>()
            });

            // ── Contracts ─────────────────────────────────────────────────────────

            session.ContractProfiles.Add(new ContractProfile
            {
                Id                       = contractId,
                ClientName               = "Test Client",
                Type                     = Core.Definitions.Contract.ContractType.InternalBusinessTool,
                Difficulty               = Core.Definitions.Contract.ContractDifficulty.Standard,
                RequiredRoles            = new List<EmployeeRole>(),
                RequiredSkills           = new List<SkillCategory>(),
                PostedDate               = startDate,
                ExpiryDate               = startDate,
                Deadline                 = startDate,
                BasePaymentMinorUnits    = 50_000L,
                ExcellentBonusMinorUnits = 10_000L,
                FailurePaymentMinorUnits = 0L,
                QualityTarget            = 70,
                MilestoneCount           = 3
            });

            session.ContractStates.Add(new ContractRuntimeState
            {
                ContractId          = contractId,
                Status              = Core.Definitions.Contract.ContractStatus.InProgress,
                Outcome             = Core.Definitions.Contract.ContractOutcome.None,
                ProgressPercent     = 0,
                MilestonesCompleted = 0,
                QualityScore        = 0
            });

            // ── Market ────────────────────────────────────────────────────────────

            session.MarketCategoryStates.Add(new MarketCategoryRuntimeState
            {
                Id                    = categoryId,
                CategoryType          = MarketCategoryType.DevelopmentTool,
                TotalDemand           = 10000,
                GrowthRateBasisPoints = 500,
                SegmentDemandWeights  = new Dictionary<CustomerSegment, int>(),
                CustomerPreferences   = new Dictionary<CustomerPreference, int>(),
                CompetitiveIntensity  = 40,
                LeaderProductIds      = new List<string>(),
                ActiveTrendIds        = new List<string>()
            });

            session.TrendStates.Add(new TrendRuntimeState
            {
                Id                 = trendId,
                Type               = TrendType.DeveloperToolingShift,
                InitialStrength    = 70,
                Strength           = 70,
                AffectedCategories = new List<MarketCategoryType> { MarketCategoryType.DevelopmentTool },
                StartDate          = startDate,
                IsActive           = true
            });

            // ── Competitors ───────────────────────────────────────────────────────

            session.CompetitorProfiles.Add(new CompetitorProfile
            {
                Id             = competitorId,
                Name           = "Rival Co",
                Archetype      = CompetitorArchetype.LowCostCompetitor,
                MarketFocus    = new List<MarketCategoryType> { MarketCategoryType.DevelopmentTool },
                RiskAppetite   = 40,
                PricingStyle   = CompetitorPricingStyle.Balanced,
                MarketingStyle = CompetitorMarketingStyle.Balanced
            });

            session.CompetitorStates.Add(new CompetitorRuntimeState
            {
                CompetitorId           = competitorId,
                CashStrength           = 50,
                Reputation             = 40,
                ProductIds             = new List<string>(),
                HiringStrength         = 40,
                ResearchStrength       = 30,
                LaunchCadence          = 30,
                MarketShareBasisPoints = new Dictionary<MarketCategoryType, int>()
            });

            // ── Research ─────────────────────────────────────────────────────────

            session.ResearchProjectStates.Add(new ResearchProjectRuntimeState
            {
                ProjectId       = researchId,
                Status          = ResearchProjectStatus.Available,
                ProgressPercent = 0,
                AssignedTeamId  = null,
                StartDate       = null
            });

            // ── Reports ───────────────────────────────────────────────────────────

            session.ReportProfiles.Add(new ReportProfile
            {
                Id                 = reportId,
                Type               = Core.Definitions.Report.ReportType.MonthlyFinance,
                Category           = Core.Definitions.Report.ReportCategory.Finance,
                Priority           = Core.Definitions.Report.ReportPriority.Routine,
                Title              = "Test Report",
                Summary            = "Test summary.",
                Date               = startDate,
                RelatedEntities    = new List<ReportEntityReference>(),
                KeyValues          = new List<ReportKeyValue>(),
                RequiresDecision   = false,
                AvailableActionIds = new List<string>()
            });

            // ── Transactions ──────────────────────────────────────────────────────

            session.TransactionRecords.Add(new TransactionRecord
            {
                Id               = txnId,
                Type             = Core.Definitions.Finance.TransactionType.Expense,
                ExpenseCategory  = Core.Definitions.Finance.ExpenseCategory.Payroll,
                AmountMinorUnits = 100_000L,
                Description      = "Test transaction",
                Date             = startDate
            });

            session.MonthlyFinanceSummaries.Add(new MonthlyFinanceSummary
            {
                Id                      = summaryId,
                Period                  = new FinancePeriodKey(2026, 1),
                CashAtStartMinorUnits   = 5_000_000L,
                CashAtEndMinorUnits     = 4_900_000L,
                TotalRevenueMinorUnits  = 0L,
                TotalExpensesMinorUnits = 100_000L,
                NetProfitLossMinorUnits = -100_000L,
                RunwayMonths            = 49,
                IsRunwayStable          = true
            });

            // ── Event state ───────────────────────────────────────────────────────

            session.LastGlobalEventDate = startDate;
            session.LastEventCheckDate  = startDate;

            return session;
        }

        // ─── Per-domain comparison methods ────────────────────────────────────────

        private static bool CompareSessionIdentity(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.SessionId != b.SessionId)
            {
                mismatch = $"SessionId mismatch: expected '{a.SessionId}', got '{b.SessionId}'";
                return false;
            }

            return true;
        }

        private static bool CompareRandomState(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.RandomState == null && b.RandomState == null) return true;
            if (a.RandomState == null || b.RandomState == null)
            {
                mismatch = "RandomState null mismatch";
                return false;
            }

            if (a.RandomState.Seed != b.RandomState.Seed)
            {
                mismatch = $"Seed mismatch: expected {a.RandomState.Seed}, got {b.RandomState.Seed}";
                return false;
            }

            if (a.RandomState.CallCount != b.RandomState.CallCount)
            {
                mismatch = $"CallCount mismatch: expected {a.RandomState.CallCount}, got {b.RandomState.CallCount}";
                return false;
            }

            return true;
        }

        private static bool CompareCompany(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.CompanyProfile == null && b.CompanyProfile == null) return true;
            if (a.CompanyProfile == null || b.CompanyProfile == null)
            {
                mismatch = "CompanyProfile null mismatch";
                return false;
            }

            if (a.CompanyProfile.Id != b.CompanyProfile.Id)
            {
                mismatch = $"CompanyProfile.Id mismatch: expected '{a.CompanyProfile.Id}', got '{b.CompanyProfile.Id}'";
                return false;
            }

            if (a.CompanyProfile.Name != b.CompanyProfile.Name)
            {
                mismatch = $"CompanyProfile.Name mismatch: expected '{a.CompanyProfile.Name}', got '{b.CompanyProfile.Name}'";
                return false;
            }

            if (a.CompanyState == null && b.CompanyState == null) return true;
            if (a.CompanyState == null || b.CompanyState == null)
            {
                mismatch = "CompanyState null mismatch";
                return false;
            }

            if (a.CompanyState.Reputation != b.CompanyState.Reputation)
            {
                mismatch = $"CompanyState.Reputation mismatch: expected {a.CompanyState.Reputation}, got {b.CompanyState.Reputation}";
                return false;
            }

            return true;
        }

        private static bool CompareFounder(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.FounderProfile == null && b.FounderProfile == null) return true;
            if (a.FounderProfile == null || b.FounderProfile == null)
            {
                mismatch = "FounderProfile null mismatch";
                return false;
            }

            if (a.FounderProfile.Id != b.FounderProfile.Id)
            {
                mismatch = $"FounderProfile.Id mismatch: expected '{a.FounderProfile.Id}', got '{b.FounderProfile.Id}'";
                return false;
            }

            if (a.FounderProfile.Background != b.FounderProfile.Background)
            {
                mismatch = $"FounderProfile.Background mismatch: expected {a.FounderProfile.Background}, got {b.FounderProfile.Background}";
                return false;
            }

            return true;
        }

        private static bool CompareTime(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.TimeState == null && b.TimeState == null) return true;
            if (a.TimeState == null || b.TimeState == null)
            {
                mismatch = "TimeState null mismatch";
                return false;
            }

            if (a.TimeState.TotalElapsedHours != b.TimeState.TotalElapsedHours)
            {
                mismatch = $"TotalElapsedHours mismatch: expected {a.TimeState.TotalElapsedHours}, got {b.TimeState.TotalElapsedHours}";
                return false;
            }

            if (a.TimeState.Speed != b.TimeState.Speed)
            {
                mismatch = $"Speed mismatch: expected {a.TimeState.Speed}, got {b.TimeState.Speed}";
                return false;
            }

            return true;
        }

        private static bool CompareFinance(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.FinanceState == null && b.FinanceState == null) return true;
            if (a.FinanceState == null || b.FinanceState == null)
            {
                mismatch = "FinanceState null mismatch";
                return false;
            }

            if (a.FinanceState.CashMinorUnits != b.FinanceState.CashMinorUnits)
            {
                mismatch = $"CashMinorUnits mismatch: expected {a.FinanceState.CashMinorUnits}, got {b.FinanceState.CashMinorUnits}";
                return false;
            }

            if (a.FinanceState.RunwayMonths != b.FinanceState.RunwayMonths)
            {
                mismatch = $"RunwayMonths mismatch: expected {a.FinanceState.RunwayMonths}, got {b.FinanceState.RunwayMonths}";
                return false;
            }

            return true;
        }

        private static bool CompareRecruitment(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.RecruitmentState == null && b.RecruitmentState == null) return true;
            if (a.RecruitmentState == null || b.RecruitmentState == null)
            {
                mismatch = "RecruitmentState null mismatch";
                return false;
            }

            if (a.RecruitmentState.CandidateIds.Count != b.RecruitmentState.CandidateIds.Count)
            {
                mismatch = $"CandidateIds count mismatch: expected {a.RecruitmentState.CandidateIds.Count}, got {b.RecruitmentState.CandidateIds.Count}";
                return false;
            }

            return true;
        }

        private static bool CompareInbox(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.InboxState == null && b.InboxState == null) return true;
            if (a.InboxState == null || b.InboxState == null)
            {
                mismatch = "InboxState null mismatch";
                return false;
            }

            if (a.InboxState.UnreadCount != b.InboxState.UnreadCount)
            {
                mismatch = $"InboxState.UnreadCount mismatch: expected {a.InboxState.UnreadCount}, got {b.InboxState.UnreadCount}";
                return false;
            }

            if (a.InboxState.ReportIds.Count != b.InboxState.ReportIds.Count)
            {
                mismatch = $"InboxState.ReportIds count mismatch: expected {a.InboxState.ReportIds.Count}, got {b.InboxState.ReportIds.Count}";
                return false;
            }

            return true;
        }

        private static bool CompareEmployee(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.EmployeeProfiles.Count != b.EmployeeProfiles.Count)
            {
                mismatch = $"EmployeeProfiles count mismatch: expected {a.EmployeeProfiles.Count}, got {b.EmployeeProfiles.Count}";
                return false;
            }

            if (a.EmployeeStates.Count != b.EmployeeStates.Count)
            {
                mismatch = $"EmployeeStates count mismatch: expected {a.EmployeeStates.Count}, got {b.EmployeeStates.Count}";
                return false;
            }

            if (a.EmployeeStates.Count > 0 && b.EmployeeStates.Count > 0)
            {
                EmployeeRuntimeState origState   = a.EmployeeStates[0];
                EmployeeRuntimeState loadedState = b.EmployeeStates[0];

                if (origState.Morale != loadedState.Morale)
                {
                    mismatch = $"Morale mismatch on employee.001: expected {origState.Morale}, got {loadedState.Morale}";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareTeams(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.TeamProfiles.Count != b.TeamProfiles.Count)
            {
                mismatch = $"TeamProfiles count mismatch: expected {a.TeamProfiles.Count}, got {b.TeamProfiles.Count}";
                return false;
            }

            if (a.TeamStates.Count != b.TeamStates.Count)
            {
                mismatch = $"TeamStates count mismatch: expected {a.TeamStates.Count}, got {b.TeamStates.Count}";
                return false;
            }

            if (a.AssignmentStates.Count != b.AssignmentStates.Count)
            {
                mismatch = $"AssignmentStates count mismatch: expected {a.AssignmentStates.Count}, got {b.AssignmentStates.Count}";
                return false;
            }

            return true;
        }

        private static bool CompareProducts(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.ProductProfiles.Count != b.ProductProfiles.Count)
            {
                mismatch = $"ProductProfiles count mismatch: expected {a.ProductProfiles.Count}, got {b.ProductProfiles.Count}";
                return false;
            }

            if (a.ProductStates.Count != b.ProductStates.Count)
            {
                mismatch = $"ProductStates count mismatch: expected {a.ProductStates.Count}, got {b.ProductStates.Count}";
                return false;
            }

            if (a.SoftwareMetrics.Count != b.SoftwareMetrics.Count)
            {
                mismatch = $"SoftwareMetrics count mismatch: expected {a.SoftwareMetrics.Count}, got {b.SoftwareMetrics.Count}";
                return false;
            }

            if (a.HardwareMetrics.Count != b.HardwareMetrics.Count)
            {
                mismatch = $"HardwareMetrics count mismatch: expected {a.HardwareMetrics.Count}, got {b.HardwareMetrics.Count}";
                return false;
            }

            if (a.ProductBudgets.Count != b.ProductBudgets.Count)
            {
                mismatch = $"ProductBudgets count mismatch: expected {a.ProductBudgets.Count}, got {b.ProductBudgets.Count}";
                return false;
            }

            return true;
        }

        private static bool CompareContracts(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.ContractProfiles.Count != b.ContractProfiles.Count)
            {
                mismatch = $"ContractProfiles count mismatch: expected {a.ContractProfiles.Count}, got {b.ContractProfiles.Count}";
                return false;
            }

            if (a.ContractStates.Count != b.ContractStates.Count)
            {
                mismatch = $"ContractStates count mismatch: expected {a.ContractStates.Count}, got {b.ContractStates.Count}";
                return false;
            }

            return true;
        }

        private static bool CompareMarket(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.MarketCategoryStates.Count != b.MarketCategoryStates.Count)
            {
                mismatch = $"MarketCategoryStates count mismatch: expected {a.MarketCategoryStates.Count}, got {b.MarketCategoryStates.Count}";
                return false;
            }

            if (a.TrendStates.Count != b.TrendStates.Count)
            {
                mismatch = $"TrendStates count mismatch: expected {a.TrendStates.Count}, got {b.TrendStates.Count}";
                return false;
            }

            if (a.MarketCategoryStates.Count > 0 && b.MarketCategoryStates.Count > 0)
            {
                if (a.MarketCategoryStates[0].TotalDemand != b.MarketCategoryStates[0].TotalDemand)
                {
                    mismatch = $"MarketCategoryState[0].TotalDemand mismatch: expected {a.MarketCategoryStates[0].TotalDemand}, got {b.MarketCategoryStates[0].TotalDemand}";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareCompetitors(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.CompetitorProfiles.Count != b.CompetitorProfiles.Count)
            {
                mismatch = $"CompetitorProfiles count mismatch: expected {a.CompetitorProfiles.Count}, got {b.CompetitorProfiles.Count}";
                return false;
            }

            if (a.CompetitorStates.Count != b.CompetitorStates.Count)
            {
                mismatch = $"CompetitorStates count mismatch: expected {a.CompetitorStates.Count}, got {b.CompetitorStates.Count}";
                return false;
            }

            if (a.CompetitorStates.Count > 0 && b.CompetitorStates.Count > 0)
            {
                if (a.CompetitorStates[0].Reputation != b.CompetitorStates[0].Reputation)
                {
                    mismatch = $"CompetitorState[0].Reputation mismatch: expected {a.CompetitorStates[0].Reputation}, got {b.CompetitorStates[0].Reputation}";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareResearch(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.ResearchProjectStates.Count != b.ResearchProjectStates.Count)
            {
                mismatch = $"ResearchProjectStates count mismatch: expected {a.ResearchProjectStates.Count}, got {b.ResearchProjectStates.Count}";
                return false;
            }

            if (a.ResearchState != null && b.ResearchState != null)
            {
                if (a.ResearchState.CompanyId != b.ResearchState.CompanyId)
                {
                    mismatch = $"ResearchState.CompanyId mismatch: expected '{a.ResearchState.CompanyId}', got '{b.ResearchState.CompanyId}'";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareReports(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.ReportProfiles.Count != b.ReportProfiles.Count)
            {
                mismatch = $"ReportProfiles count mismatch: expected {a.ReportProfiles.Count}, got {b.ReportProfiles.Count}";
                return false;
            }

            if (a.ReportProfiles.Count > 0 && b.ReportProfiles.Count > 0)
            {
                if (a.ReportProfiles[0].Title != b.ReportProfiles[0].Title)
                {
                    mismatch = $"ReportProfile[0].Title mismatch: expected '{a.ReportProfiles[0].Title}', got '{b.ReportProfiles[0].Title}'";
                    return false;
                }
            }

            return true;
        }

        private static bool CompareTransactions(GameSessionState a, GameSessionState b, out string mismatch)
        {
            mismatch = string.Empty;

            if (a.TransactionRecords.Count != b.TransactionRecords.Count)
            {
                mismatch = $"TransactionRecords count mismatch: expected {a.TransactionRecords.Count}, got {b.TransactionRecords.Count}";
                return false;
            }

            if (a.MonthlyFinanceSummaries.Count != b.MonthlyFinanceSummaries.Count)
            {
                mismatch = $"MonthlyFinanceSummaries count mismatch: expected {a.MonthlyFinanceSummaries.Count}, got {b.MonthlyFinanceSummaries.Count}";
                return false;
            }

            if (a.TransactionRecords.Count > 0 && b.TransactionRecords.Count > 0)
            {
                if (a.TransactionRecords[0].AmountMinorUnits != b.TransactionRecords[0].AmountMinorUnits)
                {
                    mismatch = $"TransactionRecord[0].AmountMinorUnits mismatch: expected {a.TransactionRecords[0].AmountMinorUnits}, got {b.TransactionRecords[0].AmountMinorUnits}";
                    return false;
                }
            }

            return true;
        }
    }
}
