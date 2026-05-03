namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Tuning interface for the finance system.
    /// Covers runway thresholds, hardware retailer margin, and recurring expense placeholders.
    /// Implemented by TuningConfig in Infrastructure.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public interface IFinanceTuning
    {
        // ── Runway ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Months of runway at or below which a low runway warning is published (LowRunwayWarningEvent).
        /// Does not interrupt Continue.
        /// </summary>
        int LowRunwayThresholdMonths { get; }

        /// <summary>
        /// Months of runway at or below which a critical runway interruption fires (InterruptionType.CriticalRunway).
        /// Pauses Continue and prompts the player.
        /// </summary>
        int CriticalRunwayThresholdMonths { get; }

        // ── Hardware ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Percentage of hardware gross revenue retained by the retailer.
        /// Net revenue = grossRevenue * (1 - HardwareRetailerMarginPercent / 100).
        /// Valid range: 0–100.
        /// </summary>
        float HardwareRetailerMarginPercent { get; }

        // ── Recurring Expense Placeholders ────────────────────────────────────────

        /// <summary>
        /// [Placeholder] Monthly infrastructure cost per launched product in minor currency units.
        /// Applied as: launchedProductCount * BaseInfrastructureCostPerProductMinorUnits.
        /// </summary>
        long BaseInfrastructureCostPerProductMinorUnits { get; }

        /// <summary>
        /// [Placeholder] Monthly manufacturing overhead cost per active hardware product in minor currency units.
        /// Applied as: activeHardwareProductCount * BaseManufacturingMonthlyCostMinorUnits.
        /// Does not include hardware COGS (UnitsSoldThisMonth * ManufacturingCostPerUnitMinorUnits).
        /// </summary>
        long BaseManufacturingMonthlyCostMinorUnits { get; }

        /// <summary>
        /// [Placeholder] Monthly research overhead cost per active research project in minor currency units.
        /// Applied as: activeResearchProjectCount * BaseResearchMonthlyCostMinorUnits.
        /// </summary>
        long BaseResearchMonthlyCostMinorUnits { get; }
    }
}
