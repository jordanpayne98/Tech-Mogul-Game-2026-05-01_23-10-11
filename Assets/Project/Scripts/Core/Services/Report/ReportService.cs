using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Event;
using Project.Core.Definitions.Product;
using Project.Core.Definitions.Report;
using Project.Core.Events.Report;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.Report;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Market;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Report;
using Project.Core.Runtime.Research;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Report
{
    /// <summary>
    /// Stateless service that constructs ReportProfile instances and delivers them to InboxRuntimeState.
    /// Each method receives all required state as parameters and returns a GenerateReportResult.
    /// Report IDs are GUIDs. InboxRuntimeState is mutated in-place (report added, counts updated).
    /// ReportGeneratedEvent is published after each successful report generation.
    /// Defined in Plan 2L, GDD_14, GDD_18.
    /// </summary>
    public sealed class ReportService : IReportService
    {
        private static readonly string[] MonthNames =
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        private readonly IEventBus     _eventBus;
        private readonly IFinanceTuning _financeTuning;

        public ReportService(IEventBus eventBus, IFinanceTuning financeTuning)
        {
            _eventBus      = eventBus      ?? throw new ArgumentNullException(nameof(eventBus));
            _financeTuning = financeTuning ?? throw new ArgumentNullException(nameof(financeTuning));
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public GenerateReportResult GenerateMonthlyFinanceReport(
            GameDateTime          date,
            MonthlyFinanceSummary summary,
            FinanceRuntimeState   financeState,
            InboxRuntimeState     inbox)
        {
            if (date == null)         return GenerateReportResult.Failed("date is null.");
            if (summary == null)      return GenerateReportResult.Failed("summary is null.");
            if (financeState == null) return GenerateReportResult.Failed("financeState is null.");
            if (inbox == null)        return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildMonthlyFinanceReport(reportId, date, summary, financeState);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated MonthlyFinanceReport. ReportId: {reportId}, Priority: {profile.Priority}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateOfferResponseReport(
            GameDateTime     date,
            CandidateProfile candidate,
            OfferStatus      response,
            InboxRuntimeState inbox)
        {
            if (date == null)      return GenerateReportResult.Failed("date is null.");
            if (candidate == null) return GenerateReportResult.Failed("candidate is null.");
            if (inbox == null)     return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildOfferResponseReport(reportId, date, candidate, response);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated OfferResponseReport. ReportId: {reportId}, Candidate: {candidate.Name}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateContractCompletionReport(
            GameDateTime         date,
            ContractRuntimeState contract,
            ContractOutcome      outcome,
            InboxRuntimeState    inbox)
        {
            if (date == null)     return GenerateReportResult.Failed("date is null.");
            if (contract == null) return GenerateReportResult.Failed("contract is null.");
            if (inbox == null)    return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildContractCompletionReport(reportId, date, contract, outcome);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated ContractCompletionReport. ReportId: {reportId}, ContractId: {contract.ContractId}, Outcome: {outcome}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateProductProgressReport(
            GameDateTime        date,
            ProductRuntimeState product,
            ProductStatus       completedPhase,
            ProductStatus       nextPhase,
            InboxRuntimeState   inbox)
        {
            if (date == null)    return GenerateReportResult.Failed("date is null.");
            if (product == null) return GenerateReportResult.Failed("product is null.");
            if (inbox == null)   return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildProductProgressReport(reportId, date, product, completedPhase, nextPhase);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated ProductProgressReport. ReportId: {reportId}, ProductId: {product.ProductId}, Phase: {completedPhase} -> {nextPhase}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateProductLaunchReport(
            GameDateTime        date,
            ProductRuntimeState product,
            InboxRuntimeState   inbox)
        {
            if (date == null)    return GenerateReportResult.Failed("date is null.");
            if (product == null) return GenerateReportResult.Failed("product is null.");
            if (inbox == null)   return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildProductLaunchReport(reportId, date, product);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated ProductLaunchReport. ReportId: {reportId}, ProductId: {product.ProductId}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateMonthlyProductPerformanceReport(
            GameDateTime           date,
            ProductRuntimeState    product,
            SoftwareRuntimeMetrics softwareMetrics,
            HardwareRuntimeMetrics hardwareMetrics,
            InboxRuntimeState      inbox)
        {
            if (date == null)    return GenerateReportResult.Failed("date is null.");
            if (product == null) return GenerateReportResult.Failed("product is null.");
            if (inbox == null)   return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildMonthlyProductPerformanceReport(
                reportId, date, product, softwareMetrics, hardwareMetrics);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated MonthlyProductPerformanceReport. ReportId: {reportId}, ProductId: {product.ProductId}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateMarketTrendReport(
            GameDateTime                     date,
            List<MarketCategoryRuntimeState> marketStates,
            InboxRuntimeState                inbox)
        {
            if (date == null)         return GenerateReportResult.Failed("date is null.");
            if (marketStates == null) return GenerateReportResult.Failed("marketStates is null.");
            if (inbox == null)        return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildMarketTrendReport(reportId, date, marketStates);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated MarketTrendReport. ReportId: {reportId}, Categories: {marketStates.Count}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateCompetitorLaunchReport(
            GameDateTime                  date,
            CompetitorRuntimeState        competitor,
            CompetitorProductRuntimeState product,
            InboxRuntimeState             inbox)
        {
            if (date == null)       return GenerateReportResult.Failed("date is null.");
            if (competitor == null) return GenerateReportResult.Failed("competitor is null.");
            if (product == null)    return GenerateReportResult.Failed("product is null.");
            if (inbox == null)      return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildCompetitorLaunchReport(reportId, date, competitor, product);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated CompetitorLaunchReport. ReportId: {reportId}, CompetitorId: {competitor.CompetitorId}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateResearchCompletionReport(
            GameDateTime                date,
            ResearchProjectRuntimeState project,
            List<string>                unlockedCapabilityIds,
            InboxRuntimeState           inbox)
        {
            if (date == null)                  return GenerateReportResult.Failed("date is null.");
            if (project == null)               return GenerateReportResult.Failed("project is null.");
            if (unlockedCapabilityIds == null)  return GenerateReportResult.Failed("unlockedCapabilityIds is null.");
            if (inbox == null)                 return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildResearchCompletionReport(reportId, date, project, unlockedCapabilityIds);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated ResearchCompletionReport. ReportId: {reportId}, ProjectId: {project.ProjectId}");

            return GenerateReportResult.Succeeded(reportId);
        }

        /// <inheritdoc/>
        public GenerateReportResult GenerateCrisisReport(
            GameDateTime          date,
            string                eventDefinitionId,
            string                description,
            GameEventCategory     category,
            GameEventSeverity     severity,
            List<GameEventEffect> effects,
            InboxRuntimeState     inbox)
        {
            if (date == null)              return GenerateReportResult.Failed("date is null.");
            if (eventDefinitionId == null) return GenerateReportResult.Failed("eventDefinitionId is null.");
            if (effects == null)           return GenerateReportResult.Failed("effects is null.");
            if (inbox == null)             return GenerateReportResult.Failed("inbox is null.");

            string reportId = Guid.NewGuid().ToString();
            ReportProfile profile = BuildCrisisReport(reportId, date, eventDefinitionId, description, category, severity, effects);
            DeliverReport(profile, inbox);

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ReportService] Generated CrisisReport. ReportId: {reportId}, EventDefinitionId: {eventDefinitionId}, Severity: {severity}");

            return GenerateReportResult.Succeeded(reportId);
        }

        // ── Private builders ──────────────────────────────────────────────────────

        private ReportProfile BuildMonthlyFinanceReport(
            string                reportId,
            GameDateTime          date,
            MonthlyFinanceSummary summary,
            FinanceRuntimeState   financeState)
        {
            ReportPriority priority;

            if (financeState.RunwayMonths <= _financeTuning.CriticalRunwayThresholdMonths)
            {
                priority = ReportPriority.Critical;
            }
            else if (financeState.RunwayMonths <= _financeTuning.LowRunwayThresholdMonths)
            {
                priority = ReportPriority.Important;
            }
            else
            {
                priority = ReportPriority.Routine;
            }

            string monthName = GetMonthName(date.Month);
            long   netMinor  = summary.NetProfitLossMinorUnits;
            string netSign   = netMinor >= 0 ? "+" : string.Empty;

            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Revenue",        Value = FormatMinorUnits(summary.TotalRevenueMinorUnits) },
                new ReportKeyValue { Label = "Expenses",       Value = FormatMinorUnits(summary.TotalExpensesMinorUnits) },
                new ReportKeyValue { Label = "Net",            Value = $"{netSign}{FormatMinorUnits(netMinor)}" },
                new ReportKeyValue { Label = "Cash",           Value = FormatMinorUnits(summary.CashAtEndMinorUnits) },
                new ReportKeyValue { Label = "Runway Months",  Value = financeState.RunwayMonths.ToString() }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.MonthlyFinance,
                Category          = ReportCategory.Finance,
                Priority          = priority,
                Title             = $"Monthly Finance Report — {monthName} {date.Year}",
                Summary           = $"Financial summary for {monthName} {date.Year}. Net: {netSign}{FormatMinorUnits(netMinor)}. Runway: {financeState.RunwayMonths} months.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = new List<ReportEntityReference>(),
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildOfferResponseReport(
            string           reportId,
            GameDateTime     date,
            CandidateProfile candidate,
            OfferStatus      response)
        {
            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Role",            Value = candidate.Role.ToString() },
                new ReportKeyValue { Label = "Offered Salary",  Value = FormatMinorUnits(candidate.SalaryExpectationMinorUnits) },
                new ReportKeyValue { Label = "Response",        Value = response.ToString() }
            };

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = candidate.Id, EntityType = "candidate" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.OfferResponse,
                Category          = ReportCategory.Hiring,
                Priority          = ReportPriority.Routine,
                Title             = $"Offer Response — {candidate.Name}",
                Summary           = $"{candidate.Name} has {response} your offer for the {candidate.Role} role.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildContractCompletionReport(
            string               reportId,
            GameDateTime         date,
            ContractRuntimeState contract,
            ContractOutcome      outcome)
        {
            ReportPriority priority = outcome == ContractOutcome.Failed
                ? ReportPriority.Important
                : ReportPriority.Routine;

            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Outcome",       Value = outcome.ToString() },
                new ReportKeyValue { Label = "Quality Score", Value = contract.QualityScore.ToString() },
                new ReportKeyValue { Label = "Payment",       Value = FormatMinorUnits(contract.PaymentDueMinorUnits) }
            };

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = contract.ContractId, EntityType = "contract" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.ContractCompletion,
                Category          = ReportCategory.Contract,
                Priority          = priority,
                Title             = $"Contract Completed — {contract.ContractId}",
                Summary           = $"Contract {contract.ContractId} has concluded with outcome: {outcome}. Quality score: {contract.QualityScore}.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildProductProgressReport(
            string              reportId,
            GameDateTime        date,
            ProductRuntimeState product,
            ProductStatus       completedPhase,
            ProductStatus       nextPhase)
        {
            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Completed Phase", Value = completedPhase.ToString() },
                new ReportKeyValue { Label = "Next Phase",      Value = nextPhase.ToString() }
            };

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = product.ProductId, EntityType = "product" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.ProductProgress,
                Category          = ReportCategory.Product,
                Priority          = ReportPriority.Routine,
                Title             = $"Product Phase Complete — {product.ProductId}",
                Summary           = $"Product {product.ProductId} has completed the {completedPhase} phase and moved to {nextPhase}.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildProductLaunchReport(
            string              reportId,
            GameDateTime        date,
            ProductRuntimeState product)
        {
            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Product ID", Value = product.ProductId }
            };

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = product.ProductId, EntityType = "product" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.ProductLaunch,
                Category          = ReportCategory.Product,
                Priority          = ReportPriority.Important,
                Title             = $"Product Launched — {product.ProductId}",
                Summary           = $"Product {product.ProductId} has been successfully launched to the market.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildMonthlyProductPerformanceReport(
            string                 reportId,
            GameDateTime           date,
            ProductRuntimeState    product,
            SoftwareRuntimeMetrics softwareMetrics,
            HardwareRuntimeMetrics hardwareMetrics)
        {
            string monthName = GetMonthName(date.Month);

            var keyValues = new List<ReportKeyValue>();

            // Software metrics
            if (softwareMetrics != null)
            {
                keyValues.Add(new ReportKeyValue { Label = "Active Users",   Value = product.ActiveUsers.ToString() });
                keyValues.Add(new ReportKeyValue { Label = "New Users",      Value = softwareMetrics.NewUsersThisMonth.ToString() });
                int churnedUsers = ComputeChurnedUsers(product.ActiveUsers, softwareMetrics.ChurnBasisPoints);
                keyValues.Add(new ReportKeyValue { Label = "Churned Users",  Value = churnedUsers.ToString() });
                keyValues.Add(new ReportKeyValue { Label = "Revenue",        Value = FormatMinorUnits(product.CurrentMonthRevenueMinorUnits) });
            }

            // Hardware metrics
            if (hardwareMetrics != null)
            {
                keyValues.Add(new ReportKeyValue { Label = "Units Sold",       Value = product.UnitsSoldThisMonth.ToString() });
                keyValues.Add(new ReportKeyValue { Label = "Stock Remaining",  Value = hardwareMetrics.CurrentInventory.ToString() });
                keyValues.Add(new ReportKeyValue { Label = "Revenue",          Value = FormatMinorUnits(product.CurrentMonthRevenueMinorUnits) });
            }

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = product.ProductId, EntityType = "product" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.SalesUser,
                Category          = ReportCategory.Product,
                Priority          = ReportPriority.Routine,
                Title             = $"Product Performance — {product.ProductId} — {monthName} {date.Year}",
                Summary           = $"Monthly performance summary for product {product.ProductId} in {monthName} {date.Year}.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildMarketTrendReport(
            string                           reportId,
            GameDateTime                     date,
            List<MarketCategoryRuntimeState> marketStates)
        {
            string monthName = GetMonthName(date.Month);

            var keyValues = new List<ReportKeyValue>();

            foreach (var category in marketStates)
            {
                string growthDirection = category.GrowthRateBasisPoints >= 0 ? "Growing" : "Shrinking";
                keyValues.Add(new ReportKeyValue
                {
                    Label = category.CategoryType.ToString(),
                    Value = $"Demand: {category.TotalDemand} | {growthDirection}"
                });
            }

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.MarketTrend,
                Category          = ReportCategory.Market,
                Priority          = ReportPriority.Routine,
                Title             = $"Market Trends — {monthName} {date.Year}",
                Summary           = $"Market trend summary for {monthName} {date.Year} across {marketStates.Count} categories.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = new List<ReportEntityReference>(),
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildCompetitorLaunchReport(
            string                        reportId,
            GameDateTime                  date,
            CompetitorRuntimeState        competitor,
            CompetitorProductRuntimeState product)
        {
            var keyValues = new List<ReportKeyValue>
            {
                new ReportKeyValue { Label = "Product Name",     Value = product.Name },
                new ReportKeyValue { Label = "Market Category",  Value = product.Category.ToString() },
                new ReportKeyValue { Label = "Price",            Value = FormatMinorUnits(product.PriceMinorUnits) }
            };

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = competitor.CompetitorId,  EntityType = "competitor" },
                new ReportEntityReference { EntityId = product.Id,               EntityType = "competitor_product" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.CompetitorLaunch,
                Category          = ReportCategory.Competitor,
                Priority          = ReportPriority.Important,
                Title             = $"Competitor Launch — {competitor.CompetitorId}",
                Summary           = $"Competitor {competitor.CompetitorId} has launched a new product: {product.Name} in the {product.Category} market.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildResearchCompletionReport(
            string                      reportId,
            GameDateTime                date,
            ResearchProjectRuntimeState project,
            List<string>                unlockedCapabilityIds)
        {
            var keyValues = new List<ReportKeyValue>();

            if (unlockedCapabilityIds.Count > 0)
            {
                keyValues.Add(new ReportKeyValue
                {
                    Label = "Unlocked Capabilities",
                    Value = string.Join(", ", unlockedCapabilityIds)
                });
            }
            else
            {
                keyValues.Add(new ReportKeyValue
                {
                    Label = "Unlocked Capabilities",
                    Value = "None"
                });
            }

            var relatedEntities = new List<ReportEntityReference>
            {
                new ReportEntityReference { EntityId = project.ProjectId, EntityType = "research_project" }
            };

            return new ReportProfile
            {
                Id                = reportId,
                Type              = ReportType.ResearchCompletion,
                Category          = ReportCategory.Research,
                Priority          = ReportPriority.Important,
                Title             = $"Research Complete — {project.ProjectId}",
                Summary           = $"Research project {project.ProjectId} has been completed. {unlockedCapabilityIds.Count} capabilities unlocked.",
                Date              = date,
                KeyValues         = keyValues,
                RelatedEntities   = relatedEntities,
                RequiresDecision  = false,
                AvailableActionIds = new List<string>()
            };
        }

        private ReportProfile BuildCrisisReport(
            string                reportId,
            GameDateTime          date,
            string                eventDefinitionId,
            string                description,
            GameEventCategory     category,
            GameEventSeverity     severity,
            List<GameEventEffect> effects)
        {
            // Look up the event title from the catalog; fall back to the definition ID.
            GameEventDefinition definition = GameEventCatalog.GetById(eventDefinitionId);
            string title = definition != null ? definition.Title : eventDefinitionId;

            // Map GameEventCategory → ReportCategory.
            ReportCategory reportCategory = category switch
            {
                GameEventCategory.Market        => ReportCategory.Market,
                GameEventCategory.Team          => ReportCategory.Team,
                GameEventCategory.Product       => ReportCategory.Product,
                GameEventCategory.Employee      => ReportCategory.Employee,
                GameEventCategory.Finance       => ReportCategory.Finance,
                GameEventCategory.Infrastructure => ReportCategory.Infrastructure,
                _                               => ReportCategory.System
            };

            // Map GameEventSeverity → ReportPriority.
            ReportPriority priority = severity switch
            {
                GameEventSeverity.Critical => ReportPriority.Critical,
                GameEventSeverity.Major    => ReportPriority.Important,
                _                          => ReportPriority.Routine   // Moderate, Minor
            };

            // Build key-value pairs from applied effects.
            var keyValues = new List<ReportKeyValue>();
            foreach (GameEventEffect effect in effects)
            {
                keyValues.Add(new ReportKeyValue
                {
                    Label = effect.Type.ToString(),
                    Value = effect.Description
                });
            }

            // Build related entity references from effect targets.
            var relatedEntities = new List<ReportEntityReference>();
            foreach (GameEventEffect effect in effects)
            {
                if (!string.IsNullOrEmpty(effect.TargetEntityId))
                {
                    string entityType = category switch
                    {
                        GameEventCategory.Market   => "market_category",
                        GameEventCategory.Team     => "team",
                        GameEventCategory.Product  => "product",
                        GameEventCategory.Employee => "employee",
                        _                          => "entity"
                    };

                    relatedEntities.Add(new ReportEntityReference
                    {
                        EntityId   = effect.TargetEntityId,
                        EntityType = entityType
                    });
                }
            }

            return new ReportProfile
            {
                Id                 = reportId,
                Type               = ReportType.CrisisEvent,
                Category           = reportCategory,
                Priority           = priority,
                Title              = title,
                Summary            = string.IsNullOrEmpty(description) ? title : description,
                Date               = date,
                KeyValues          = keyValues,
                RelatedEntities    = relatedEntities,
                RequiresDecision   = false,
                AvailableActionIds = new List<string>()
            };
        }

        // ── Inbox delivery ─────────────────────────────────────────────────────────
        private void DeliverReport(ReportProfile profile, InboxRuntimeState inbox)
        {
            inbox.ReportIds              ??= new List<string>();
            inbox.ReportReadStates       ??= new Dictionary<string, ReportReadState>();
            inbox.ReportInboxStates      ??= new Dictionary<string, ReportInboxState>();

            inbox.ReportIds.Add(profile.Id);
            inbox.ReportReadStates[profile.Id]  = ReportReadState.Unread;
            inbox.ReportInboxStates[profile.Id] = ReportInboxState.Active;
            inbox.UnreadCount++;

            if (profile.RequiresDecision)
            {
                inbox.DecisionRequiredCount++;
            }

            _eventBus.Publish(new ReportGeneratedEvent(profile.Id, profile.Type, profile.Priority));
        }

        // ── Private helpers ────────────────────────────────────────────────────────

        private static string GetMonthName(int month)
        {
            int index = Math.Clamp(month, 1, 12) - 1;
            return MonthNames[index];
        }

        /// <summary>
        /// Formats a minor currency unit value as a display string using decimal major units.
        /// Minor units are divided by 100 to derive major units (e.g. pence -> pounds).
        /// </summary>
        private static string FormatMinorUnits(long minorUnits)
        {
            decimal major = minorUnits / 100m;
            return major.ToString("C0", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static int ComputeChurnedUsers(int activeUsers, int churnBasisPoints)
        {
            if (activeUsers <= 0 || churnBasisPoints <= 0) return 0;
            return (int)Math.Round(activeUsers * (churnBasisPoints / 10000.0));
        }
    }
}
