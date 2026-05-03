using System.Collections.Generic;
using Project.Core.Runtime.Market;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for market-domain types.
    /// Checks required IDs, GrowthRateBasisPoints 0–10000, MarketShareBasisPoints 0–10000,
    /// CashStrength/Reputation 0–100, and enum validity.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class MarketDataValidator :
        IValidator<MarketCategoryRuntimeState>,
        IValidator<CompetitorProfile>,
        IValidator<CompetitorRuntimeState>,
        IValidator<CompetitorProductRuntimeState>,
        IValidator<TrendRuntimeState>
    {
        // -------------------------------------------------------------------------
        // MarketCategoryRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(MarketCategoryRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("MarketCategoryRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "MarketCategoryRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("MarketCategoryRuntimeState", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var categoryTypeIssue = ValidationRules.EnumDefined(target.CategoryType, "CategoryType", ref_);
            if (categoryTypeIssue.HasValue) issues.Add(categoryTypeIssue.Value);

            // GrowthRateBasisPoints: 0–10000.
            var growthIssue = ValidationRules.Range(target.GrowthRateBasisPoints, 0, 10000, "GrowthRateBasisPoints", ref_);
            if (growthIssue.HasValue) issues.Add(growthIssue.Value);

            // 0–100 threshold fields.
            var competitiveIntensityIssue = ValidationRules.Range(target.CompetitiveIntensity, 0, 100, "CompetitiveIntensity", ref_);
            if (competitiveIntensityIssue.HasValue) issues.Add(competitiveIntensityIssue.Value);

            var techExpIssue = ValidationRules.Range(target.TechnologyExpectation, 0, 100, "TechnologyExpectation", ref_);
            if (techExpIssue.HasValue) issues.Add(techExpIssue.Value);

            var priceSensIssue = ValidationRules.Range(target.PriceSensitivity, 0, 100, "PriceSensitivity", ref_);
            if (priceSensIssue.HasValue) issues.Add(priceSensIssue.Value);

            var marketingSensIssue = ValidationRules.Range(target.MarketingSensitivity, 0, 100, "MarketingSensitivity", ref_);
            if (marketingSensIssue.HasValue) issues.Add(marketingSensIssue.Value);

            var supportExpIssue = ValidationRules.Range(target.SupportExpectation, 0, 100, "SupportExpectation", ref_);
            if (supportExpIssue.HasValue) issues.Add(supportExpIssue.Value);

            // Lists must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.LeaderProductIds, "LeaderProductIds", ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.ActiveTrendIds,   "ActiveTrendIds",   ref_));

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CompetitorProfile
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CompetitorProfile target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CompetitorProfile");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CompetitorProfile instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CompetitorProfile", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var nameIssue = ValidationRules.RequiredString(target.Name, "Name", ref_);
            if (nameIssue.HasValue) issues.Add(nameIssue.Value);

            var archetypeIssue = ValidationRules.EnumDefined(target.Archetype, "Archetype", ref_);
            if (archetypeIssue.HasValue) issues.Add(archetypeIssue.Value);

            var pricingStyleIssue = ValidationRules.EnumDefined(target.PricingStyle, "PricingStyle", ref_);
            if (pricingStyleIssue.HasValue) issues.Add(pricingStyleIssue.Value);

            var marketingStyleIssue = ValidationRules.EnumDefined(target.MarketingStyle, "MarketingStyle", ref_);
            if (marketingStyleIssue.HasValue) issues.Add(marketingStyleIssue.Value);

            // RiskAppetite: 0–100.
            var riskIssue = ValidationRules.Range(target.RiskAppetite, 0, 100, "RiskAppetite", ref_);
            if (riskIssue.HasValue) issues.Add(riskIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CompetitorRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CompetitorRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CompetitorRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CompetitorRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("CompetitorRuntimeState", target.CompetitorId);

            var idIssue = ValidationRules.RequiredString(target.CompetitorId, "CompetitorId", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            // 0–100 strength indicators.
            var cashIssue = ValidationRules.Range(target.CashStrength, 0, 100, "CashStrength", ref_);
            if (cashIssue.HasValue) issues.Add(cashIssue.Value);

            var repIssue = ValidationRules.Range(target.Reputation, 0, 100, "Reputation", ref_);
            if (repIssue.HasValue) issues.Add(repIssue.Value);

            var hiringIssue = ValidationRules.Range(target.HiringStrength, 0, 100, "HiringStrength", ref_);
            if (hiringIssue.HasValue) issues.Add(hiringIssue.Value);

            var researchIssue = ValidationRules.Range(target.ResearchStrength, 0, 100, "ResearchStrength", ref_);
            if (researchIssue.HasValue) issues.Add(researchIssue.Value);

            var cadenceIssue = ValidationRules.Range(target.LaunchCadence, 0, 100, "LaunchCadence", ref_);
            if (cadenceIssue.HasValue) issues.Add(cadenceIssue.Value);

            // MarketShareBasisPoints per category: 0–10000 each.
            if (target.MarketShareBasisPoints != null)
            {
                foreach (var kvp in target.MarketShareBasisPoints)
                {
                    var shareIssue = ValidationRules.Range(kvp.Value, 0, 10000, $"MarketShareBasisPoints[{kvp.Key}]", ref_);
                    if (shareIssue.HasValue) issues.Add(shareIssue.Value);
                }
            }

            issues.AddRange(ValidationRules.NoNullItems(target.ProductIds, "ProductIds", ref_));

            // LastLaunchDate: nullable, no structural constraint. Value is accepted when present.

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // CompetitorProductRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(CompetitorProductRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("CompetitorProductRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "CompetitorProductRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_   = new ValidationEntityReference("CompetitorProductRuntimeState", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var competitorIdIssue = ValidationRules.RequiredString(target.CompetitorId, "CompetitorId", ref_);
            if (competitorIdIssue.HasValue) issues.Add(competitorIdIssue.Value);

            var categoryIssue = ValidationRules.EnumDefined(target.Category, "Category", ref_);
            if (categoryIssue.HasValue) issues.Add(categoryIssue.Value);

            // PriceMinorUnits must be > 0.
            if (target.PriceMinorUnits <= 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "value.out_of_range",
                    ref_,
                    "PriceMinorUnits",
                    $"{ref_}.PriceMinorUnits must be > 0. Actual: {target.PriceMinorUnits}."));
            }

            var qualityIssue = ValidationRules.Range(target.QualityScore, 0, 100, "QualityScore", ref_);
            if (qualityIssue.HasValue) issues.Add(qualityIssue.Value);

            var marketingIssue = ValidationRules.Range(target.MarketingStrength, 0, 100, "MarketingStrength", ref_);
            if (marketingIssue.HasValue) issues.Add(marketingIssue.Value);

            var shareIssue = ValidationRules.Range(target.MarketShareBasisPoints, 0, 10000, "MarketShareBasisPoints", ref_);
            if (shareIssue.HasValue) issues.Add(shareIssue.Value);

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // TrendRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(TrendRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("TrendRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "TrendRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("TrendRuntimeState", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

            // InitialStrength must be >= 0.
            var initialStrengthIssue = ValidationRules.Range(target.InitialStrength, 0, int.MaxValue, "InitialStrength", ref_);
            if (initialStrengthIssue.HasValue) issues.Add(initialStrengthIssue.Value);

            // Strength: 0–InitialStrength. Clamp upper to InitialStrength.
            var strengthIssue = ValidationRules.Range(target.Strength, 0, 100, "Strength", ref_);
            if (strengthIssue.HasValue) issues.Add(strengthIssue.Value);

            // StartDate must not be null.
            if (target.StartDate == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "StartDate",
                    $"{ref_}.StartDate is null."));
            }

            return ValidationResult.WithIssues(issues);
        }
    }
}
