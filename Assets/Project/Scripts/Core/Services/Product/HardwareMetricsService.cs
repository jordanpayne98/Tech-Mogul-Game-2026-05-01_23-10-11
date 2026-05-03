using System;
using Project.Core.Debugging;
using Project.Core.Definitions.Product;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Product;

namespace Project.Core.Services.Product
{
    /// <summary>
    /// Hardware-specific product metrics service.
    /// Owns hardware-only post-launch metrics: manufacturing economics, defect rates,
    /// warranty costs, inventory levels, and daily unit sales.
    /// Stateless — receives all required state as parameters, holds no references.
    /// Does not publish events. Does not mutate finance state.
    /// Warranty costs are tracked on HardwareRuntimeMetrics only; 2H reads these for finance processing.
    /// </summary>
    public sealed class HardwareMetricsService
    {
        // ─── BOM Tier Helpers ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns the manufacturing cost multiplier for the given BOM tier.
        /// </summary>
        public float GetCostMultiplier(BOMTier tier, IProductTuning tuning)
        {
            switch (tier)
            {
                case BOMTier.Budget:
                    return tuning.BOMBudgetCostMultiplier;
                case BOMTier.Standard:
                    return tuning.BOMStandardCostMultiplier;
                case BOMTier.Premium:
                    return tuning.BOMPremiumCostMultiplier;
                case BOMTier.Experimental:
                    return tuning.BOMExperimentalCostMultiplier;
                default:
                    return tuning.BOMStandardCostMultiplier;
            }
        }

        /// <summary>
        /// Returns the defect rate multiplier for the given BOM tier.
        /// </summary>
        public float GetDefectMultiplier(BOMTier tier, IProductTuning tuning)
        {
            switch (tier)
            {
                case BOMTier.Budget:
                    return tuning.BOMBudgetDefectMultiplier;
                case BOMTier.Standard:
                    return tuning.BOMStandardDefectMultiplier;
                case BOMTier.Premium:
                    return tuning.BOMPremiumDefectMultiplier;
                case BOMTier.Experimental:
                    return tuning.BOMExperimentalDefectMultiplier;
                default:
                    return tuning.BOMStandardDefectMultiplier;
            }
        }

        /// <summary>
        /// Returns the quality score bonus for the given BOM tier.
        /// </summary>
        public int GetQualityBonus(BOMTier tier, IProductTuning tuning)
        {
            switch (tier)
            {
                case BOMTier.Budget:
                    return tuning.BOMBudgetQualityBonus;
                case BOMTier.Standard:
                    return tuning.BOMStandardQualityBonus;
                case BOMTier.Premium:
                    return tuning.BOMPremiumQualityBonus;
                case BOMTier.Experimental:
                    return tuning.BOMExperimentalQualityBonus;
                default:
                    return tuning.BOMStandardQualityBonus;
            }
        }

        // ─── Metrics Creation ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new HardwareRuntimeMetrics with BOM-derived manufacturing cost and defect rate.
        /// formula.product.hardware_manufacturing_cost and formula.product.hardware_defect_rate.
        /// </summary>
        public HardwareRuntimeMetrics CreateHardwareMetrics(
            string productId,
            BOMTier bomTier,
            int launchStockQuantity,
            IProductTuning tuning)
        {
            float costMultiplier   = GetCostMultiplier(bomTier, tuning);
            float defectMultiplier = GetDefectMultiplier(bomTier, tuning);

            var metrics = new HardwareRuntimeMetrics
            {
                ProductId                        = productId,
                BOMTier                          = bomTier,
                ManufacturingCostPerUnitMinorUnits = (long)(tuning.BaseManufacturingCostPerUnitMinorUnits * costMultiplier),
                UnitMarginMinorUnits             = 0,
                DefectRateBasisPoints            = (int)(tuning.BaseDefectRateBasisPoints * defectMultiplier),
                WarrantyCostThisMonthMinorUnits  = 0,
                LaunchStock                      = launchStockQuantity,
                CurrentInventory                 = launchStockQuantity,
                ComponentAvailability            = tuning.DefaultComponentAvailability,
                ReturnRateBasisPoints            = tuning.BaseReturnRateBasisPoints
            };

            DebugLogger.Log(DebugCategory.Simulation,
                $"[HardwareMetricsService] Hardware metrics created. ProductId: {productId}, "
                + $"BOMTier: {bomTier}, CostPerUnit: {metrics.ManufacturingCostPerUnitMinorUnits}, "
                + $"DefectRateBP: {metrics.DefectRateBasisPoints}, LaunchStock: {launchStockQuantity}");

            return metrics;
        }

        // ─── Review Score ─────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the hardware review score at launch (and monthly thereafter).
        /// formula.product.hardware_review_score:
        ///   qualityBonus = GetQualityBonus(metrics.BOMTier, tuning)
        ///   defectPenalty = (int)(metrics.DefectRateBasisPoints / 100.0 * tuning.DefectImpactOnReviewScore)
        ///   returnPenalty = (int)(metrics.ReturnRateBasisPoints / 100.0 * tuning.ReturnImpactOnReviewScore)
        ///   reviewScore = BaseReviewScore + qualityBonus + (qualityTarget - 50) / 5 - defectPenalty - returnPenalty
        ///   return Math.Clamp(reviewScore, MinReviewScore, MaxReviewScore)
        /// </summary>
        public int ComputeHardwareReviewScore(
            HardwareRuntimeMetrics metrics,
            int qualityTarget,
            IProductTuning tuning)
        {
            int qualityBonus   = GetQualityBonus(metrics.BOMTier, tuning);
            int defectPenalty  = (int)(metrics.DefectRateBasisPoints / 100.0 * tuning.DefectImpactOnReviewScore);
            int returnPenalty  = (int)(metrics.ReturnRateBasisPoints / 100.0 * tuning.ReturnImpactOnReviewScore);
            int reviewScore    = tuning.BaseReviewScore + qualityBonus + (qualityTarget - 50) / 5 - defectPenalty - returnPenalty;

            return Math.Clamp(reviewScore, tuning.MinReviewScore, tuning.MaxReviewScore);
        }

        // ─── Daily Sales ──────────────────────────────────────────────────────────

        /// <summary>
        /// Computes the number of hardware units sold in a single day.
        /// formula.product.hardware_daily_sales:
        ///   if CurrentInventory ≤ 0: return 0
        ///   reviewFactor = productState.ReviewScore / 50f
        ///   monthlyUnits = (int)(BaseHardwareUnitsSoldPerMonth * reviewFactor * HardwareReviewScoreSalesMultiplier)
        ///   dailyUnits = Max(0, monthlyUnits / 30)
        ///   return Min(dailyUnits, CurrentInventory)
        /// [Placeholder] No market demand, competitor comparison, or marketing budget effect.
        /// </summary>
        public int ComputeDailyUnitsSold(
            ProductRuntimeState productState,
            HardwareRuntimeMetrics metrics,
            IProductTuning tuning)
        {
            if (metrics.CurrentInventory <= 0)
            {
                return 0;
            }

            float reviewFactor  = productState.ReviewScore / 50f;
            int   monthlyUnits  = (int)(tuning.BaseHardwareUnitsSoldPerMonth * reviewFactor * tuning.HardwareReviewScoreSalesMultiplier);
            int   dailyUnits    = Math.Max(0, monthlyUnits / 30);
            return Math.Min(dailyUnits, metrics.CurrentInventory);
        }

        // ─── Daily Metrics Update ─────────────────────────────────────────────────

        /// <summary>
        /// Processes daily post-launch hardware metrics.
        /// Updates units sold, inventory, defect generation, warranty costs, and return estimates.
        /// Only runs for Launched or Supported products.
        /// Does not mutate finance state — warranty costs are tracked on HardwareRuntimeMetrics only.
        /// </summary>
        public void UpdateDailyMetrics(
            ProductRuntimeState productState,
            HardwareRuntimeMetrics metrics,
            IProductTuning tuning,
            Random random)
        {
            if (productState.Status != ProductStatus.Launched
                && productState.Status != ProductStatus.Supported)
            {
                return;
            }

            // ── Units sold ────────────────────────────────────────────────────────
            int dailySold = ComputeDailyUnitsSold(productState, metrics, tuning);
            productState.UnitsSoldThisMonth += dailySold;
            productState.UnitsSoldTotal     += dailySold;
            metrics.CurrentInventory        -= dailySold;

            // ── Defect generation (daily fraction of monthly) ─────────────────────
            // [Placeholder formula] — simplified daily accumulation
            int monthlyDefects = (int)(productState.UnitsSoldTotal * metrics.DefectRateBasisPoints / 10000);
            int dailyDefects   = Math.Max(0, monthlyDefects / 30);

            // ── Warranty cost ─────────────────────────────────────────────────────
            metrics.WarrantyCostThisMonthMinorUnits += dailyDefects * tuning.WarrantyCostPerDefectMinorUnits;

            // ── Return processing (daily fraction of monthly) ─────────────────────
            // [Placeholder formula] — returns do not replenish inventory (assumed defective/refurbished)
            // dailyReturns tracked for future use; not applied to inventory.

            DebugLogger.Log(DebugCategory.Simulation,
                $"[HardwareMetricsService] Daily hardware metrics updated. ProductId: {productState.ProductId}, "
                + $"DailySold: {dailySold}, Inventory: {metrics.CurrentInventory}, "
                + $"DailyDefects: {dailyDefects}, WarrantyCost: {metrics.WarrantyCostThisMonthMinorUnits}");
        }

        // ─── Monthly Reset ────────────────────────────────────────────────────────

        /// <summary>
        /// Resets month-scoped counters on a product and its hardware metrics.
        /// Called at the start of each new simulation month.
        /// </summary>
        public void ResetMonthlyCounters(ProductRuntimeState productState, HardwareRuntimeMetrics metrics)
        {
            productState.UnitsSoldThisMonth            = 0;
            productState.CurrentMonthRevenueMinorUnits = 0;
            metrics.WarrantyCostThisMonthMinorUnits    = 0;
        }
    }
}
