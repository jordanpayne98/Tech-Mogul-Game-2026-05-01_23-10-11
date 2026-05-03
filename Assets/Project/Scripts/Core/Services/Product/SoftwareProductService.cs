using System;
using System.Collections.Generic;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Definitions.Team;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Product;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Product
{
    /// <summary>
    /// Software-specific product service.
    /// Implements IProductService for software product lifecycle management.
    /// Also exposes additional software-only methods for metrics and post-launch simulation.
    /// Stateless — receives all required state as parameters, holds no references.
    /// Does not publish events.
    /// </summary>
    public sealed class SoftwareProductService : IProductService
    {
        // ─── IProductService Implementation ──────────────────────────────────────

        /// <inheritdoc/>
        public ProductProfile CreateProductProfile(CreateProductRequest request, GameDateTime currentDate)
        {
            var profile = new ProductProfile
            {
                Id                                     = Guid.NewGuid().ToString("N"),
                Name                                   = request.Name,
                Family                                 = DeriveFamily(request.Category),
                Category                               = request.Category,
                TargetMarketSegmentId                  = request.TargetMarketSegmentId,
                CustomerSegmentId                      = request.CustomerSegmentId,
                RevenueModel                           = request.RevenueModel,
                PriceMinorUnits                        = request.PriceMinorUnits,
                FeatureScope                           = request.FeatureScope,
                QualityTarget                          = request.QualityTarget,
                CreatedDate                            = currentDate,
                TargetReleaseDate                      = request.TargetReleaseDate,
                SupportedPlatformIds                   = request.SupportedPlatformIds ?? new List<string>(),
                // Software products always require post-launch support resourcing for MVP.
                RequiresSupport                        = true
            };

            DebugLogger.Log(DebugCategory.Simulation,
                $"[SoftwareProductService] Product profile created. Id: {profile.Id}, Name: {profile.Name}, Category: {profile.Category}");

            return profile;
        }

        /// <inheritdoc/>
        public ProductRuntimeState CreateProductRuntimeState(string productId)
        {
            var state = new ProductRuntimeState
            {
                ProductId                   = productId,
                Status                      = ProductStatus.InConcept,
                ProgressPercent             = 0,
                AssignedTeamIds             = new List<string>(),
                LaunchDate                  = default,
                TotalRevenueMinorUnits      = 0,
                CurrentMonthRevenueMinorUnits = 0,
                UnitsSoldTotal              = 0,
                UnitsSoldThisMonth          = 0,
                ActiveUsers                 = 0,
                ReviewScore                 = 0,
                RecentReviewScore           = 0,
                MarketShareBasisPoints      = 0,
                MonthlyRevenueHistoryIds    = new List<string>(),
                ScoreValues                 = new Dictionary<ProductScoreDimension, int>()
            };

            // Initialize all score dimensions to 0.
            foreach (ProductScoreDimension dimension in Enum.GetValues(typeof(ProductScoreDimension)))
            {
                state.ScoreValues[dimension] = 0;
            }

            return state;
        }

        /// <inheritdoc/>
        public ProductBudgetProfile CreateProductBudgetProfile(string productId, CreateProductRequest request)
        {
            return new ProductBudgetProfile
            {
                ProductId                                 = productId,
                DevelopmentBudgetMinorUnits               = request.DevelopmentBudgetMinorUnits,
                PreLaunchMarketingMonthlyBudgetMinorUnits = request.PreLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchMarketingMonthlyBudgetMinorUnits = request.PostLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchSupportMonthlyBudgetMinorUnits  = request.PostLaunchSupportMonthlyBudgetMinorUnits
            };
        }

        /// <inheritdoc/>
        /// <remarks>
        /// formula.product.phase_duration:
        ///   required = baseHours * (1.0 + (featureScope / 100.0) * scopeMult) * (1.0 + (qualityTarget / 100.0) * qualMult)
        ///   result = Math.Max(1, (int)required)
        /// Returns 0 for non-work phases.
        /// </remarks>
        public int ComputePhaseRequiredPoints(ProductStatus phase, int featureScope, int qualityTarget, IProductTuning tuning, ProductFamily family)
        {
            int baseHours;
            switch (phase)
            {
                case ProductStatus.InConcept:
                    baseHours = tuning.ConceptPhaseBaseHours;
                    break;
                case ProductStatus.InDevelopment:
                    baseHours = tuning.DevelopmentPhaseBaseHours;
                    break;
                case ProductStatus.InQA:
                    baseHours = tuning.QAPhaseBaseHours;
                    break;
                default:
                    // No work required for non-work phases (ReadyForLaunch, Launched, Sunset, etc.).
                    return 0;
            }

            double required = baseHours
                * (1.0 + (featureScope / 100.0) * tuning.FeatureScopeComplexityMultiplier)
                * (1.0 + (qualityTarget / 100.0) * tuning.QualityTargetComplexityMultiplier);

            return Math.Max(1, (int)required);
        }

        /// <inheritdoc/>
        public ProductStatus? GetNextPhase(ProductStatus currentPhase, ProductFamily family)
        {
            // Software lifecycle: InConcept → InDevelopment → InQA → ReadyForLaunch (gate, no auto-transition after).
            switch (currentPhase)
            {
                case ProductStatus.InConcept:
                    return ProductStatus.InDevelopment;
                case ProductStatus.InDevelopment:
                    return ProductStatus.InQA;
                case ProductStatus.InQA:
                    return ProductStatus.ReadyForLaunch;
                default:
                    return null;
            }
        }

        /// <inheritdoc/>
        public AssignmentType GetAssignmentTypeForPhase(ProductStatus phase, ProductFamily family)
        {
            switch (phase)
            {
                case ProductStatus.InConcept:
                    return AssignmentType.ProductConcept;
                case ProductStatus.InDevelopment:
                    return AssignmentType.SoftwareDevelopment;
                case ProductStatus.InQA:
                    return AssignmentType.QATesting;
                case ProductStatus.ReadyForLaunch:
                    return AssignmentType.LaunchPreparation;
                case ProductStatus.Launched:
                case ProductStatus.Supported:
                    return AssignmentType.PostLaunchSupport;
                default:
                    return AssignmentType.SoftwareDevelopment;
            }
        }

        /// <inheritdoc/>
        public bool IsWorkPhase(ProductStatus status)
        {
            return status == ProductStatus.InConcept
                || status == ProductStatus.InDevelopment
                || status == ProductStatus.InQA;
        }

        /// <inheritdoc/>
        public bool ValidateLaunch(ProductRuntimeState productState, out string failureReason)
        {
            if (productState.Status != ProductStatus.ReadyForLaunch)
            {
                failureReason = "Product must be in ReadyForLaunch status to launch";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        /// <inheritdoc/>
        public bool ValidateSunset(ProductRuntimeState productState, out string failureReason)
        {
            if (productState.Status != ProductStatus.Launched
                && productState.Status != ProductStatus.Supported)
            {
                failureReason = "Product must be Launched or Supported to sunset";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        /// <inheritdoc/>
        public ProductFamily DeriveFamily(ProductCategory category)
        {
            return category == ProductCategory.HardwarePlatform
                ? ProductFamily.Hardware
                : ProductFamily.Software;
        }

        // ─── Software-Specific Methods ────────────────────────────────────────────

        /// <summary>
        /// Creates a zeroed SoftwareRuntimeMetrics object for the given product ID.
        /// All numeric fields are initialized to 0.
        /// </summary>
        public SoftwareRuntimeMetrics CreateSoftwareMetrics(string productId)
        {
            return new SoftwareRuntimeMetrics
            {
                ProductId           = productId,
                NewUsersThisMonth   = 0,
                ChurnBasisPoints    = 0,
                InfrastructureLoad  = 0,
                UptimeBasisPoints   = 0,
                BugCount            = 0,
                SecurityRisk        = 0,
                SupportTickets      = 0,
                FeatureSatisfaction = 0
            };
        }

        /// <summary>
        /// Computes the review score for a launched software product.
        /// formula.product.review_score:
        ///   qualityAchieved = qualityTarget  [Placeholder — quality = target for MVP]
        ///   qualityBonus = (qualityAchieved - 50) / 5
        ///   bugPenalty = (int)(metrics.BugCount * tuning.BugImpactOnReviewScore)
        ///   featureSatisfactionBonus = (metrics.FeatureSatisfaction - 50) / 10
        ///   reviewScore = BaseReviewScore + qualityBonus - bugPenalty + featureSatisfactionBonus
        ///   return Math.Clamp(reviewScore, MinReviewScore, MaxReviewScore)
        /// </summary>
        public int ComputeReviewScore(
            ProductRuntimeState productState,
            SoftwareRuntimeMetrics metrics,
            int qualityTarget,
            IProductTuning tuning)
        {
            // [Placeholder] Quality achieved equals quality target for MVP.
            int qualityAchieved          = qualityTarget;
            int qualityBonus             = (qualityAchieved - 50) / 5;
            int bugPenalty               = (int)(metrics.BugCount * tuning.BugImpactOnReviewScore);
            int featureSatisfactionBonus = (metrics.FeatureSatisfaction - 50) / 10;

            int rawScore = tuning.BaseReviewScore + qualityBonus - bugPenalty + featureSatisfactionBonus;
            return Math.Clamp(rawScore, tuning.MinReviewScore, tuning.MaxReviewScore);
        }

        /// <summary>
        /// Computes the initial active user count at product launch.
        /// formula.product.initial_users:
        ///   marketingFactor = budget.PreLaunchMarketingMonthlyBudgetMinorUnits > 0
        ///     ? 1.0f + ((float)budget.PreLaunchMarketing / tuning.MarketingBudgetReference) * tuning.UserGrowthMarketingMultiplier
        ///     : 1.0f
        ///   reviewFactor = reviewScore / 50f
        ///   initialUsers = (int)(tuning.InitialActiveUsersOnLaunch * marketingFactor * reviewFactor)
        ///   return Math.Max(1, initialUsers)
        /// </summary>
        public int ComputeInitialActiveUsers(
            ProductBudgetProfile budget,
            int reviewScore,
            IProductTuning tuning)
        {
            float marketingFactor;
            long marketingRef = Math.Max(1L, tuning.MarketingBudgetReferenceMinorUnits);

            if (budget.PreLaunchMarketingMonthlyBudgetMinorUnits > 0)
            {
                marketingFactor = 1.0f
                    + ((float)budget.PreLaunchMarketingMonthlyBudgetMinorUnits / marketingRef)
                    * tuning.UserGrowthMarketingMultiplier;
            }
            else
            {
                marketingFactor = 1.0f;
            }

            float reviewFactor   = reviewScore / 50f;
            int   initialUsers   = (int)(tuning.InitialActiveUsersOnLaunch * marketingFactor * reviewFactor);
            return Math.Max(1, initialUsers);
        }

        /// <summary>
        /// [Placeholder] Derives feature satisfaction from feature scope.
        /// formula.product.feature_satisfaction:
        ///   satisfaction = BaseFeatureSatisfaction + (int)(featureScope * scopeMult / 100 * 50)
        ///   return Math.Clamp(satisfaction, 0, 100)
        /// </summary>
        public int ComputeFeatureSatisfaction(int featureScope, IProductTuning tuning)
        {
            int satisfaction = tuning.BaseFeatureSatisfaction
                + (int)(featureScope * tuning.FeatureSatisfactionScopeMultiplier / 100f * 50);

            return Math.Clamp(satisfaction, 0, 100);
        }

        /// <summary>
        /// Processes daily post-launch metrics for a software product.
        /// Calculates user growth, churn, bug generation, support tickets,
        /// infrastructure load, and uptime. Only runs for Launched or Supported products.
        /// </summary>
        public void UpdateDailyMetrics(
            ProductRuntimeState productState,
            SoftwareRuntimeMetrics metrics,
            ProductBudgetProfile budget,
            IProductTuning tuning,
            Random random)
        {
            if (productState.Status != ProductStatus.Launched
                && productState.Status != ProductStatus.Supported)
            {
                return;
            }

            long marketingRef = Math.Max(1L, tuning.MarketingBudgetReferenceMinorUnits);

            // formula.product.user_growth (daily fraction of monthly)
            float marketingMultiplier = 1.0f
                + ((float)budget.PostLaunchMarketingMonthlyBudgetMinorUnits / marketingRef)
                * tuning.UserGrowthMarketingMultiplier;
            float reviewFactor       = productState.ReviewScore / 50f;
            int   monthlyNewUsers    = (int)(tuning.BaseNewUsersPerMonth * marketingMultiplier * reviewFactor);
            int   dailyNewUsers      = Math.Max(0, monthlyNewUsers / 30);

            // formula.product.user_churn (daily fraction of monthly basis points)
            int qualityScore = productState.ScoreValues.ContainsKey(ProductScoreDimension.Quality)
                ? productState.ScoreValues[ProductScoreDimension.Quality]
                : productState.ReviewScore;

            int churnBP = tuning.DefaultChurnBasisPoints
                - (int)(qualityScore * tuning.ChurnReductionPerQualityPoint);
            churnBP = Math.Clamp(churnBP, tuning.MinChurnBasisPoints, tuning.MaxChurnBasisPoints);

            int dailyChurnUsers = productState.ActiveUsers * churnBP / 10000 / 30;

            // Apply user changes.
            metrics.NewUsersThisMonth += dailyNewUsers;
            productState.ActiveUsers   = Math.Max(0, productState.ActiveUsers + dailyNewUsers - dailyChurnUsers);
            metrics.ChurnBasisPoints   = churnBP;

            // [Placeholder] Bug generation — scaled by active users.
            int maxBugs  = Math.Max(1, tuning.MaxBugsPerDayBase * productState.ActiveUsers
                / Math.Max(1, tuning.InitialActiveUsersOnLaunch * 10));
            int newBugs  = random.Next(0, maxBugs + 1);
            metrics.BugCount += newBugs;

            // [Placeholder] Support tickets — proportional to active users.
            int dailyTickets = productState.ActiveUsers
                * tuning.BaseSupportTicketsPerHundredUsers / 100 / 30;
            metrics.SupportTickets += Math.Max(0, dailyTickets);

            // [Placeholder] Infrastructure load.
            metrics.InfrastructureLoad = Math.Min(100, productState.ActiveUsers / 10);

            // [Placeholder] Uptime — degrades under high infrastructure load.
            metrics.UptimeBasisPoints = metrics.InfrastructureLoad < 80 ? 9950 : 9500;

            DebugLogger.Log(DebugCategory.Simulation,
                $"[SoftwareProductService] Daily metrics updated. ProductId: {productState.ProductId}, "
                + $"ActiveUsers: {productState.ActiveUsers}, DailyNew: {dailyNewUsers}, "
                + $"DailyChurn: {dailyChurnUsers}, Bugs: {metrics.BugCount}");
        }

        /// <summary>
        /// Resets month-scoped counters on a product and its software metrics.
        /// Called at the start of each new simulation month.
        /// </summary>
        public void ResetMonthlyCounters(ProductRuntimeState productState, SoftwareRuntimeMetrics metrics)
        {
            productState.UnitsSoldThisMonth              = 0;
            productState.CurrentMonthRevenueMinorUnits   = 0;
            metrics.NewUsersThisMonth                    = 0;
        }
    }
}
