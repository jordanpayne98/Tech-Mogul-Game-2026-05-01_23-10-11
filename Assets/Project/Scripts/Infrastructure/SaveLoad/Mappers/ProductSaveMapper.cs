using System;
using System.Collections.Generic;
using Project.Core.Definitions.Product;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Product;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Product domain runtime types to and from their save data equivalents.
    /// Covers ProductProfile, ProductRuntimeState, SoftwareRuntimeMetrics,
    /// HardwareRuntimeMetrics, and ProductBudgetProfile.
    /// All methods are static — this mapper holds no state.
    /// Dictionary{ProductScoreDimension, int} converts to Dictionary{string, int}.
    /// Nullable GameDateTime fields convert to nullable int via TotalElapsedHours.
    /// </summary>
    public static class ProductSaveMapper
    {
        // ─── ProductProfile ───────────────────────────────────────────────────────

        public static ProductSaveData ToSaveData(ProductProfile profile)
        {
            return new ProductSaveData
            {
                Id                             = profile.Id,
                Name                           = profile.Name,
                Family                         = profile.Family.ToString(),
                Category                       = profile.Category.ToString(),
                TargetMarketSegmentId          = profile.TargetMarketSegmentId,
                CustomerSegmentId              = profile.CustomerSegmentId,
                RevenueModel                   = profile.RevenueModel.ToString(),
                PriceMinorUnits                = profile.PriceMinorUnits,
                FeatureScope                   = profile.FeatureScope,
                QualityTarget                  = profile.QualityTarget,
                CreatedDateTotalElapsedHours   = profile.CreatedDate.TotalElapsedHours,
                TargetReleaseDateTotalElapsedHours = profile.TargetReleaseDate.TotalElapsedHours,
                SupportedPlatformIds           = profile.SupportedPlatformIds,
                RequiresSupport                = profile.RequiresSupport
            };
        }

        public static ProductProfile FromSaveData(ProductSaveData data)
        {
            return new ProductProfile
            {
                Id                    = data.Id,
                Name                  = data.Name,
                Family                = Enum.Parse<ProductFamily>(data.Family),
                Category              = Enum.Parse<ProductCategory>(data.Category),
                TargetMarketSegmentId = data.TargetMarketSegmentId,
                CustomerSegmentId     = data.CustomerSegmentId,
                RevenueModel          = Enum.Parse<RevenueModel>(data.RevenueModel),
                PriceMinorUnits       = data.PriceMinorUnits,
                FeatureScope          = data.FeatureScope,
                QualityTarget         = data.QualityTarget,
                CreatedDate           = GameDateTime.FromTotalHours(data.CreatedDateTotalElapsedHours),
                TargetReleaseDate     = GameDateTime.FromTotalHours(data.TargetReleaseDateTotalElapsedHours),
                SupportedPlatformIds  = data.SupportedPlatformIds,
                RequiresSupport       = data.RequiresSupport
            };
        }

        // ─── ProductRuntimeState ──────────────────────────────────────────────────

        public static ProductStateSaveData ToSaveData(ProductRuntimeState state)
        {
            var scoreValues = new Dictionary<string, int>(state.ScoreValues.Count);
            foreach (KeyValuePair<ProductScoreDimension, int> pair in state.ScoreValues)
            {
                scoreValues[pair.Key.ToString()] = pair.Value;
            }

            // LaunchDate is nullable — if TotalElapsedHours is 0, treat as not yet launched.
            int? launchDateHours = state.LaunchDate.TotalElapsedHours == 0
                ? (int?)null
                : state.LaunchDate.TotalElapsedHours;

            return new ProductStateSaveData
            {
                ProductId                     = state.ProductId,
                Status                        = state.Status.ToString(),
                ProgressPercent               = state.ProgressPercent,
                AssignedTeamIds               = state.AssignedTeamIds,
                LaunchDateTotalElapsedHours   = launchDateHours,
                TotalRevenueMinorUnits        = state.TotalRevenueMinorUnits,
                CurrentMonthRevenueMinorUnits = state.CurrentMonthRevenueMinorUnits,
                UnitsSoldTotal                = state.UnitsSoldTotal,
                UnitsSoldThisMonth            = state.UnitsSoldThisMonth,
                ActiveUsers                   = state.ActiveUsers,
                ReviewScore                   = state.ReviewScore,
                RecentReviewScore             = state.RecentReviewScore,
                MarketShareBasisPoints        = state.MarketShareBasisPoints,
                ScoreValues                   = scoreValues,
                MonthlyRevenueHistoryIds      = state.MonthlyRevenueHistoryIds
            };
        }

        public static ProductRuntimeState FromSaveData(ProductStateSaveData data)
        {
            var scoreValues = new Dictionary<ProductScoreDimension, int>(data.ScoreValues.Count);
            foreach (KeyValuePair<string, int> pair in data.ScoreValues)
            {
                scoreValues[Enum.Parse<ProductScoreDimension>(pair.Key)] = pair.Value;
            }

            GameDateTime launchDate = data.LaunchDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.LaunchDateTotalElapsedHours.Value)
                : default;

            return new ProductRuntimeState
            {
                ProductId                     = data.ProductId,
                Status                        = Enum.Parse<ProductStatus>(data.Status),
                ProgressPercent               = data.ProgressPercent,
                AssignedTeamIds               = data.AssignedTeamIds,
                LaunchDate                    = launchDate,
                TotalRevenueMinorUnits        = data.TotalRevenueMinorUnits,
                CurrentMonthRevenueMinorUnits = data.CurrentMonthRevenueMinorUnits,
                UnitsSoldTotal                = data.UnitsSoldTotal,
                UnitsSoldThisMonth            = data.UnitsSoldThisMonth,
                ActiveUsers                   = data.ActiveUsers,
                ReviewScore                   = data.ReviewScore,
                RecentReviewScore             = data.RecentReviewScore,
                MarketShareBasisPoints        = data.MarketShareBasisPoints,
                ScoreValues                   = scoreValues,
                MonthlyRevenueHistoryIds      = data.MonthlyRevenueHistoryIds
            };
        }

        // ─── SoftwareRuntimeMetrics ───────────────────────────────────────────────

        public static SoftwareMetricsSaveData ToSaveData(SoftwareRuntimeMetrics metrics)
        {
            return new SoftwareMetricsSaveData
            {
                ProductId           = metrics.ProductId,
                NewUsersThisMonth   = metrics.NewUsersThisMonth,
                ChurnBasisPoints    = metrics.ChurnBasisPoints,
                InfrastructureLoad  = metrics.InfrastructureLoad,
                UptimeBasisPoints   = metrics.UptimeBasisPoints,
                BugCount            = metrics.BugCount,
                SecurityRisk        = metrics.SecurityRisk,
                SupportTickets      = metrics.SupportTickets,
                FeatureSatisfaction = metrics.FeatureSatisfaction
            };
        }

        public static SoftwareRuntimeMetrics FromSaveData(SoftwareMetricsSaveData data)
        {
            return new SoftwareRuntimeMetrics
            {
                ProductId           = data.ProductId,
                NewUsersThisMonth   = data.NewUsersThisMonth,
                ChurnBasisPoints    = data.ChurnBasisPoints,
                InfrastructureLoad  = data.InfrastructureLoad,
                UptimeBasisPoints   = data.UptimeBasisPoints,
                BugCount            = data.BugCount,
                SecurityRisk        = data.SecurityRisk,
                SupportTickets      = data.SupportTickets,
                FeatureSatisfaction = data.FeatureSatisfaction
            };
        }

        // ─── HardwareRuntimeMetrics ───────────────────────────────────────────────

        public static HardwareMetricsSaveData ToSaveData(HardwareRuntimeMetrics metrics)
        {
            return new HardwareMetricsSaveData
            {
                ProductId                        = metrics.ProductId,
                BOMTier                          = metrics.BOMTier.ToString(),
                ManufacturingCostPerUnitMinorUnits = metrics.ManufacturingCostPerUnitMinorUnits,
                UnitMarginMinorUnits             = metrics.UnitMarginMinorUnits,
                DefectRateBasisPoints            = metrics.DefectRateBasisPoints,
                WarrantyCostThisMonthMinorUnits  = metrics.WarrantyCostThisMonthMinorUnits,
                LaunchStock                      = metrics.LaunchStock,
                CurrentInventory                 = metrics.CurrentInventory,
                ComponentAvailability            = metrics.ComponentAvailability,
                ReturnRateBasisPoints            = metrics.ReturnRateBasisPoints
            };
        }

        public static HardwareRuntimeMetrics FromSaveData(HardwareMetricsSaveData data)
        {
            return new HardwareRuntimeMetrics
            {
                ProductId                          = data.ProductId,
                BOMTier                            = Enum.Parse<BOMTier>(data.BOMTier),
                ManufacturingCostPerUnitMinorUnits = data.ManufacturingCostPerUnitMinorUnits,
                UnitMarginMinorUnits               = data.UnitMarginMinorUnits,
                DefectRateBasisPoints              = data.DefectRateBasisPoints,
                WarrantyCostThisMonthMinorUnits    = data.WarrantyCostThisMonthMinorUnits,
                LaunchStock                        = data.LaunchStock,
                CurrentInventory                   = data.CurrentInventory,
                ComponentAvailability              = data.ComponentAvailability,
                ReturnRateBasisPoints              = data.ReturnRateBasisPoints
            };
        }

        // ─── ProductBudgetProfile ─────────────────────────────────────────────────

        public static ProductBudgetSaveData ToSaveData(ProductBudgetProfile budget)
        {
            return new ProductBudgetSaveData
            {
                ProductId                                = budget.ProductId,
                DevelopmentBudgetMinorUnits              = budget.DevelopmentBudgetMinorUnits,
                PreLaunchMarketingMonthlyBudgetMinorUnits = budget.PreLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchMarketingMonthlyBudgetMinorUnits = budget.PostLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchSupportMonthlyBudgetMinorUnits = budget.PostLaunchSupportMonthlyBudgetMinorUnits
            };
        }

        public static ProductBudgetProfile FromSaveData(ProductBudgetSaveData data)
        {
            return new ProductBudgetProfile
            {
                ProductId                                 = data.ProductId,
                DevelopmentBudgetMinorUnits               = data.DevelopmentBudgetMinorUnits,
                PreLaunchMarketingMonthlyBudgetMinorUnits = data.PreLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchMarketingMonthlyBudgetMinorUnits = data.PostLaunchMarketingMonthlyBudgetMinorUnits,
                PostLaunchSupportMonthlyBudgetMinorUnits  = data.PostLaunchSupportMonthlyBudgetMinorUnits
            };
        }
    }
}
