using System.Collections.Generic;
using Project.Core.Runtime.Product;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for product-domain types.
    /// Checks required IDs, money non-negative, MarketShareBasisPoints 0–10000,
    /// ProgressPercent 0–100, score values 0–100, basis point ranges, and enum validity.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class ProductDataValidator :
        IValidator<ProductProfile>,
        IValidator<ProductBudgetProfile>,
        IValidator<ProductRuntimeState>,
        IValidator<SoftwareRuntimeMetrics>,
        IValidator<HardwareRuntimeMetrics>
    {
        // -------------------------------------------------------------------------
        // ProductProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ProductProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ProductProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ProductProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ProductProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var familyIssue = ValidationRules.EnumDefined(target.Family, "Family", ref_);
            if (familyIssue.HasValue) issues.Add(familyIssue.Value);

            var categoryIssue = ValidationRules.EnumDefined(target.Category, "Category", ref_);
            if (categoryIssue.HasValue) issues.Add(categoryIssue.Value);

            var revenueModelIssue = ValidationRules.EnumDefined(target.RevenueModel, "RevenueModel", ref_);
            if (revenueModelIssue.HasValue) issues.Add(revenueModelIssue.Value);

            // Price must be non-negative.
            var priceIssue = ValidationRules.NonNegative(target.PriceMinorUnits, "PriceMinorUnits", ref_);
            if (priceIssue.HasValue) issues.Add(priceIssue.Value);

            // QualityTarget: 0–100.
            var qualityIssue = ValidationRules.Range(target.QualityTarget, 0, 100, "QualityTarget", ref_);
            if (qualityIssue.HasValue) issues.Add(qualityIssue.Value);

            // Market/customer segment reference IDs — validate stable format if present.
            if (!string.IsNullOrWhiteSpace(target.TargetMarketSegmentId))
            {
                var segIssue = ValidationRules.ValidateStableIdFormat(target.TargetMarketSegmentId, "TargetMarketSegmentId", ref_);
                if (segIssue.HasValue) issues.Add(segIssue.Value);
            }

            if (!string.IsNullOrWhiteSpace(target.CustomerSegmentId))
            {
                var custIssue = ValidationRules.ValidateStableIdFormat(target.CustomerSegmentId, "CustomerSegmentId", ref_);
                if (custIssue.HasValue) issues.Add(custIssue.Value);
            }

            // SupportedPlatformIds must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.SupportedPlatformIds, "SupportedPlatformIds", ref_));

            // Date fields must not be null.
            if (target.CreatedDate == null)
            {
                issues.Add(new ValidationIssue(ValidationSeverity.Error, "required.missing", ref_, "CreatedDate", $"{ref_}.CreatedDate is null."));
            }
            if (target.TargetReleaseDate == null)
            {
                issues.Add(new ValidationIssue(ValidationSeverity.Error, "required.missing", ref_, "TargetReleaseDate", $"{ref_}.TargetReleaseDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // ProductBudgetProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ProductBudgetProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ProductBudgetProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ProductBudgetProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ProductBudgetProfile", target.ProductId);

            var idIssue = ValidationRules.RequiredString(target.ProductId, "ProductId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var devBudgetIssue = ValidationRules.NonNegative(target.DevelopmentBudgetMinorUnits, "DevelopmentBudgetMinorUnits", ref_);
            if (devBudgetIssue.HasValue) issues.Add(devBudgetIssue.Value);

            var preLaunchIssue = ValidationRules.NonNegative(target.PreLaunchMarketingMonthlyBudgetMinorUnits, "PreLaunchMarketingMonthlyBudgetMinorUnits", ref_);
            if (preLaunchIssue.HasValue) issues.Add(preLaunchIssue.Value);

            var postLaunchMarketingIssue = ValidationRules.NonNegative(target.PostLaunchMarketingMonthlyBudgetMinorUnits, "PostLaunchMarketingMonthlyBudgetMinorUnits", ref_);
            if (postLaunchMarketingIssue.HasValue) issues.Add(postLaunchMarketingIssue.Value);

            var postLaunchSupportIssue = ValidationRules.NonNegative(target.PostLaunchSupportMonthlyBudgetMinorUnits, "PostLaunchSupportMonthlyBudgetMinorUnits", ref_);
            if (postLaunchSupportIssue.HasValue) issues.Add(postLaunchSupportIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // ProductRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(ProductRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("ProductRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "ProductRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("ProductRuntimeState", target.ProductId);

            var idIssue = ValidationRules.RequiredString(target.ProductId, "ProductId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var statusIssue = ValidationRules.EnumDefined(target.Status, "Status", ref_);
            if (statusIssue.HasValue) issues.Add(statusIssue.Value);

            // ProgressPercent: 0–100.
            var progressIssue = ValidationRules.Range(target.ProgressPercent, 0, 100, "ProgressPercent", ref_);
            if (progressIssue.HasValue) issues.Add(progressIssue.Value);

            // Revenue fields must be non-negative.
            var totalRevenueIssue = ValidationRules.NonNegative(target.TotalRevenueMinorUnits, "TotalRevenueMinorUnits", ref_);
            if (totalRevenueIssue.HasValue) issues.Add(totalRevenueIssue.Value);

            var monthRevenueIssue = ValidationRules.NonNegative(target.CurrentMonthRevenueMinorUnits, "CurrentMonthRevenueMinorUnits", ref_);
            if (monthRevenueIssue.HasValue) issues.Add(monthRevenueIssue.Value);

            // Review scores: 0–100.
            var reviewScoreIssue = ValidationRules.Range(target.ReviewScore, 0, 100, "ReviewScore", ref_);
            if (reviewScoreIssue.HasValue) issues.Add(reviewScoreIssue.Value);

            var recentReviewIssue = ValidationRules.Range(target.RecentReviewScore, 0, 100, "RecentReviewScore", ref_);
            if (recentReviewIssue.HasValue) issues.Add(recentReviewIssue.Value);

            // MarketShareBasisPoints: 0–10000.
            var marketShareIssue = ValidationRules.Range(target.MarketShareBasisPoints, 0, 10000, "MarketShareBasisPoints", ref_);
            if (marketShareIssue.HasValue) issues.Add(marketShareIssue.Value);

            // Per-dimension score values: 0–100 each.
            if (target.ScoreValues != null)
            {
                foreach (var kvp in target.ScoreValues)
                {
                    var scoreIssue = ValidationRules.Range(kvp.Value, 0, 100, $"ScoreValues[{kvp.Key}]", ref_);
                    if (scoreIssue.HasValue) issues.Add(scoreIssue.Value);
                }
            }

            issues.AddRange(ValidationRules.NoNullItems(target.AssignedTeamIds,         "AssignedTeamIds",         ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.MonthlyRevenueHistoryIds, "MonthlyRevenueHistoryIds", ref_));

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // SoftwareRuntimeMetrics
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(SoftwareRuntimeMetrics target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("SoftwareRuntimeMetrics");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "SoftwareRuntimeMetrics instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("SoftwareRuntimeMetrics", target.ProductId);

            var idIssue = ValidationRules.RequiredString(target.ProductId, "ProductId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            // Basis point fields: 0–10000.
            var churnIssue = ValidationRules.Range(target.ChurnBasisPoints, 0, 10000, "ChurnBasisPoints", ref_);
            if (churnIssue.HasValue) issues.Add(churnIssue.Value);

            var uptimeIssue = ValidationRules.Range(target.UptimeBasisPoints, 0, 10000, "UptimeBasisPoints", ref_);
            if (uptimeIssue.HasValue) issues.Add(uptimeIssue.Value);

            // FeatureSatisfaction: 0–100.
            var featSatIssue = ValidationRules.Range(target.FeatureSatisfaction, 0, 100, "FeatureSatisfaction", ref_);
            if (featSatIssue.HasValue) issues.Add(featSatIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // HardwareRuntimeMetrics
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(HardwareRuntimeMetrics target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("HardwareRuntimeMetrics");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "HardwareRuntimeMetrics instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("HardwareRuntimeMetrics", target.ProductId);

            var idIssue = ValidationRules.RequiredString(target.ProductId, "ProductId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var bomTierIssue = ValidationRules.EnumDefined(target.BOMTier, "BOMTier", ref_);
            if (bomTierIssue.HasValue) issues.Add(bomTierIssue.Value);

            // Manufacturing cost and margin: non-negative.
            var mfgCostIssue = ValidationRules.NonNegative(target.ManufacturingCostPerUnitMinorUnits, "ManufacturingCostPerUnitMinorUnits", ref_);
            if (mfgCostIssue.HasValue) issues.Add(mfgCostIssue.Value);

            // UnitMarginMinorUnits can be negative (loss-leading hardware), so not validated here.

            // Warranty cost: non-negative.
            var warrantyIssue = ValidationRules.NonNegative(target.WarrantyCostThisMonthMinorUnits, "WarrantyCostThisMonthMinorUnits", ref_);
            if (warrantyIssue.HasValue) issues.Add(warrantyIssue.Value);

            // Basis point fields: 0–10000.
            var defectIssue = ValidationRules.Range(target.DefectRateBasisPoints, 0, 10000, "DefectRateBasisPoints", ref_);
            if (defectIssue.HasValue) issues.Add(defectIssue.Value);

            var returnIssue = ValidationRules.Range(target.ReturnRateBasisPoints, 0, 10000, "ReturnRateBasisPoints", ref_);
            if (returnIssue.HasValue) issues.Add(returnIssue.Value);

            return ValidationResult.WithIssues(issues);
        }
    }
}
