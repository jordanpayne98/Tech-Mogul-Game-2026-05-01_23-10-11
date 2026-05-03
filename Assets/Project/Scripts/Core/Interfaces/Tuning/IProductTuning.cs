namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the product lifecycle system.
    /// Consumed by ProductService, ProductPhaseTickProcessor, SoftwareMetricsTickProcessor,
    /// HardwareMetricsService, HardwareMetricsTickProcessor, LaunchProductUseCase,
    /// and related use cases without depending on Infrastructure.
    /// All prototype values are [Placeholder] — update as GDD_06/GDD_07/GDD_08 values are confirmed.
    /// </summary>
    public interface IProductTuning
    {
        // ─── Software Phase Duration ──────────────────────────────────────────────

        /// <summary>Base work hours required to complete the Concept phase. Prototype: 80.</summary>
        int ConceptPhaseBaseHours { get; }

        /// <summary>Base work hours required to complete the Development phase. Prototype: 240.</summary>
        int DevelopmentPhaseBaseHours { get; }

        /// <summary>Base work hours required to complete the QA phase. Prototype: 120.</summary>
        int QAPhaseBaseHours { get; }

        /// <summary>
        /// Base work hours reserved for a Launch Prep phase. Not used by Software MVP.
        /// Reserved for future use (e.g. Hardware launch preparation). Prototype: 40.
        /// </summary>
        int LaunchPrepPhaseBaseHours { get; }

        /// <summary>
        /// Multiplier applied to phase duration based on feature scope (0–100).
        /// Higher feature scope increases required work. Prototype: 1.5.
        /// </summary>
        float FeatureScopeComplexityMultiplier { get; }

        /// <summary>
        /// Multiplier applied to phase duration based on quality target (0–100).
        /// Higher quality target increases required work. Prototype: 1.0.
        /// </summary>
        float QualityTargetComplexityMultiplier { get; }

        // ─── Hardware Phase Duration ──────────────────────────────────────────────

        /// <summary>Base work hours required to complete the hardware Concept phase. Prototype: 60.</summary>
        int HardwareConceptPhaseBaseHours { get; }

        /// <summary>Base work hours required to complete the hardware Prototype phase. Prototype: 200.</summary>
        int HardwarePrototypePhaseBaseHours { get; }

        /// <summary>Base work hours required to complete the hardware Manufacturing Prep phase. Prototype: 160.</summary>
        int HardwareManufacturingPrepPhaseBaseHours { get; }

        // ─── Hardware Manufacturing ───────────────────────────────────────────────

        /// <summary>
        /// Base manufacturing cost per unit in minor currency units (e.g. cents).
        /// Stored as major units in the Inspector and returned as minor units (x100). Prototype: 10,000 (£100).
        /// </summary>
        long BaseManufacturingCostPerUnitMinorUnits { get; }

        /// <summary>
        /// Base defect rate expressed in basis points (0–10000). 100 = 1%. Prototype: 200 (2%).
        /// </summary>
        int BaseDefectRateBasisPoints { get; }

        /// <summary>
        /// Base return rate expressed in basis points (0–10000). 100 = 1%. Prototype: 100 (1%).
        /// </summary>
        int BaseReturnRateBasisPoints { get; }

        /// <summary>
        /// Warranty cost incurred per defective unit in minor currency units.
        /// Stored as major units in the Inspector and returned as minor units (x100). Prototype: 5,000 (£50).
        /// </summary>
        long WarrantyCostPerDefectMinorUnits { get; }

        /// <summary>
        /// Default component availability score (0–100). Higher = better supply chain health. Prototype: 80.
        /// </summary>
        int DefaultComponentAvailability { get; }

        // ─── Hardware Sales ───────────────────────────────────────────────────────

        /// <summary>Base number of hardware units sold per month before review and inventory modifiers. Prototype: 100.</summary>
        int BaseHardwareUnitsSoldPerMonth { get; }

        /// <summary>
        /// Multiplier applied to monthly hardware sales based on the product review score.
        /// Prototype: 1.0f. [Placeholder]
        /// </summary>
        float HardwareReviewScoreSalesMultiplier { get; }

        /// <summary>
        /// Multiplier applied to hardware sales based on current inventory availability.
        /// Prototype: 1.0f. [Placeholder]
        /// </summary>
        float HardwareInventoryAvailabilityMultiplier { get; }

        // ─── Hardware Review Score ────────────────────────────────────────────────

        /// <summary>
        /// Points deducted from hardware review score per 1% defect rate.
        /// Applied as: (int)(DefectRateBP / 100 * DefectImpactOnReviewScore). Prototype: 1.0f.
        /// </summary>
        float DefectImpactOnReviewScore { get; }

        /// <summary>
        /// Points deducted from hardware review score per 1% return rate.
        /// Applied as: (int)(ReturnRateBP / 100 * ReturnImpactOnReviewScore). Prototype: 0.5f.
        /// </summary>
        float ReturnImpactOnReviewScore { get; }

        // ─── BOM Cost Multipliers ─────────────────────────────────────────────────

        /// <summary>Cost multiplier for Budget BOM tier. Prototype: 0.6f.</summary>
        float BOMBudgetCostMultiplier { get; }

        /// <summary>Cost multiplier for Standard BOM tier. Prototype: 1.0f.</summary>
        float BOMStandardCostMultiplier { get; }

        /// <summary>Cost multiplier for Premium BOM tier. Prototype: 1.5f.</summary>
        float BOMPremiumCostMultiplier { get; }

        /// <summary>Cost multiplier for Experimental BOM tier. Prototype: 2.5f.</summary>
        float BOMExperimentalCostMultiplier { get; }

        // ─── BOM Defect Multipliers ───────────────────────────────────────────────

        /// <summary>Defect rate multiplier for Budget BOM tier. Prototype: 1.5f.</summary>
        float BOMBudgetDefectMultiplier { get; }

        /// <summary>Defect rate multiplier for Standard BOM tier. Prototype: 1.0f.</summary>
        float BOMStandardDefectMultiplier { get; }

        /// <summary>Defect rate multiplier for Premium BOM tier. Prototype: 0.7f.</summary>
        float BOMPremiumDefectMultiplier { get; }

        /// <summary>Defect rate multiplier for Experimental BOM tier. Prototype: 2.0f.</summary>
        float BOMExperimentalDefectMultiplier { get; }

        // ─── BOM Quality Bonuses ──────────────────────────────────────────────────

        /// <summary>Quality score bonus applied at launch for Budget BOM tier. Prototype: -10.</summary>
        int BOMBudgetQualityBonus { get; }

        /// <summary>Quality score bonus applied at launch for Standard BOM tier. Prototype: 0.</summary>
        int BOMStandardQualityBonus { get; }

        /// <summary>Quality score bonus applied at launch for Premium BOM tier. Prototype: 10.</summary>
        int BOMPremiumQualityBonus { get; }

        /// <summary>Quality score bonus applied at launch for Experimental BOM tier. Prototype: 20.</summary>
        int BOMExperimentalQualityBonus { get; }

        // ─── Review Score ─────────────────────────────────────────────────────────

        /// <summary>Starting review score before quality, bug, and feature satisfaction modifiers. Prototype: 50.</summary>
        int BaseReviewScore { get; }

        /// <summary>
        /// Points deducted from the review score per outstanding bug.
        /// Applied as: (int)(BugCount * BugImpactOnReviewScore). Prototype: 2.0.
        /// </summary>
        float BugImpactOnReviewScore { get; }

        /// <summary>Minimum allowed review score after clamping. Prototype: 1.</summary>
        int MinReviewScore { get; }

        /// <summary>Maximum allowed review score after clamping. Prototype: 100.</summary>
        int MaxReviewScore { get; }

        // ─── User Metrics ─────────────────────────────────────────────────────────

        /// <summary>
        /// Default monthly churn expressed in basis points (0–10000).
        /// 100 basis points = 1% monthly churn. Prototype: 500 (5%).
        /// </summary>
        int DefaultChurnBasisPoints { get; }

        /// <summary>
        /// Basis points of churn removed per point of quality score.
        /// Applied as: DefaultChurn - (qualityScore * ChurnReductionPerQualityPoint). Prototype: 5.0.
        /// </summary>
        float ChurnReductionPerQualityPoint { get; }

        /// <summary>Base number of new users acquired per month before marketing and review modifiers. Prototype: 50.</summary>
        int BaseNewUsersPerMonth { get; }

        /// <summary>
        /// Multiplier applied to new user growth based on marketing budget relative to reference.
        /// Applied as: 1.0 + (budget / reference) * multiplier. Prototype: 1.0.
        /// </summary>
        float UserGrowthMarketingMultiplier { get; }

        /// <summary>Number of active users at launch before marketing and review score scaling. Prototype: 100.</summary>
        int InitialActiveUsersOnLaunch { get; }

        /// <summary>
        /// Reference marketing budget in minor currency units (e.g. cents) used to normalize marketing impact.
        /// Stored as major units in the Inspector and returned as minor units (x100). Prototype: 500,000 (£5,000/month).
        /// </summary>
        long MarketingBudgetReferenceMinorUnits { get; }

        /// <summary>
        /// Floor for monthly churn in basis points. Churn cannot fall below this value.
        /// Prototype: 50 (0.5%).
        /// </summary>
        int MinChurnBasisPoints { get; }

        /// <summary>
        /// Ceiling for monthly churn in basis points. Churn cannot exceed this value.
        /// Prototype: 5000 (50%).
        /// </summary>
        int MaxChurnBasisPoints { get; }

        // ─── Misc ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Maximum bugs generated per day at baseline user count.
        /// Scaled by active users relative to InitialActiveUsersOnLaunch. Prototype: 3.
        /// </summary>
        int MaxBugsPerDayBase { get; }

        /// <summary>
        /// [Placeholder] Multiplier applied to feature scope when deriving feature satisfaction.
        /// Prototype: 1.0.
        /// </summary>
        float FeatureSatisfactionScopeMultiplier { get; }

        /// <summary>[Placeholder] Baseline feature satisfaction before scope modifier. Prototype: 50.</summary>
        int BaseFeatureSatisfaction { get; }

        /// <summary>
        /// [Placeholder] Base number of support tickets generated per 100 active users per month.
        /// Prototype: 5.
        /// </summary>
        int BaseSupportTicketsPerHundredUsers { get; }
    }
}
