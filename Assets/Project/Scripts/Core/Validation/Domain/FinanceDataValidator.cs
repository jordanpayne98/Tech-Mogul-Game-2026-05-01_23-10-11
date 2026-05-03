using System.Collections.Generic;
using Project.Core.Runtime.Finance;

namespace Project.Core.Validation.Domain
{
    /// <summary>
    /// Validates structural data integrity for finance-domain types.
    /// Checks required IDs, CashMinorUnits (may be negative — debt), expense/revenue
    /// non-negative where appropriate, RunwayMonths non-negative, enum validity,
    /// and FinancePeriodKey month 1–12.
    /// Does not validate gameplay rules or cross-entity existence.
    /// </summary>
    public sealed class FinanceDataValidator :
        IValidator<FinanceRuntimeState>,
        IValidator<TransactionRecord>,
        IValidator<MonthlyFinanceSummary>
    {
        // -------------------------------------------------------------------------
        // FinanceRuntimeState
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(FinanceRuntimeState target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("FinanceRuntimeState");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "FinanceRuntimeState instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("FinanceRuntimeState", target.CompanyId);

            var companyIdIssue = ValidationRules.RequiredString(target.CompanyId, "CompanyId", ref_);
            if (companyIdIssue.HasValue) issues.Add(companyIdIssue.Value);

            // CashMinorUnits may be negative (debt) — intentionally not validated for sign.

            // Monthly cost projections must be non-negative.
            var payrollIssue = ValidationRules.NonNegative(target.MonthlyPayrollMinorUnits, "MonthlyPayrollMinorUnits", ref_);
            if (payrollIssue.HasValue) issues.Add(payrollIssue.Value);

            var infraIssue = ValidationRules.NonNegative(target.MonthlyInfrastructureCostMinorUnits, "MonthlyInfrastructureCostMinorUnits", ref_);
            if (infraIssue.HasValue) issues.Add(infraIssue.Value);

            var supportIssue = ValidationRules.NonNegative(target.MonthlySupportCostMinorUnits, "MonthlySupportCostMinorUnits", ref_);
            if (supportIssue.HasValue) issues.Add(supportIssue.Value);

            var marketingIssue = ValidationRules.NonNegative(target.MonthlyMarketingSpendMinorUnits, "MonthlyMarketingSpendMinorUnits", ref_);
            if (marketingIssue.HasValue) issues.Add(marketingIssue.Value);

            var researchIssue = ValidationRules.NonNegative(target.MonthlyResearchSpendMinorUnits, "MonthlyResearchSpendMinorUnits", ref_);
            if (researchIssue.HasValue) issues.Add(researchIssue.Value);

            var mfgIssue = ValidationRules.NonNegative(target.MonthlyManufacturingCostMinorUnits, "MonthlyManufacturingCostMinorUnits", ref_);
            if (mfgIssue.HasValue) issues.Add(mfgIssue.Value);

            // Revenue projections must be non-negative.
            var productRevenueIssue = ValidationRules.NonNegative(target.MonthlyProductRevenueMinorUnits, "MonthlyProductRevenueMinorUnits", ref_);
            if (productRevenueIssue.HasValue) issues.Add(productRevenueIssue.Value);

            var contractRevenueIssue = ValidationRules.NonNegative(target.MonthlyContractRevenueMinorUnits, "MonthlyContractRevenueMinorUnits", ref_);
            if (contractRevenueIssue.HasValue) issues.Add(contractRevenueIssue.Value);

            // RunwayMonths must be non-negative (0 = no runway).
            if (target.RunwayMonths < 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    ref_,
                    "RunwayMonths",
                    $"{ref_}.RunwayMonths value {target.RunwayMonths} must be zero or greater."));
            }

            // ID lists must not contain null items.
            issues.AddRange(ValidationRules.NoNullItems(target.TransactionIds,  "TransactionIds",  ref_));
            issues.AddRange(ValidationRules.NoNullItems(target.MonthlySummaryIds, "MonthlySummaryIds", ref_));

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // TransactionRecord
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(TransactionRecord target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("TransactionRecord");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "TransactionRecord instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("TransactionRecord", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            var typeIssue = ValidationRules.EnumDefined(target.Type, "Type", ref_);
            if (typeIssue.HasValue) issues.Add(typeIssue.Value);

            // Amount is always positive for a transaction record.
            var amountIssue = ValidationRules.NonNegative(target.AmountMinorUnits, "AmountMinorUnits", ref_);
            if (amountIssue.HasValue) issues.Add(amountIssue.Value);

            // Description must be present.
            var descIssue = ValidationRules.RequiredString(target.Description, "Description", ref_);
            if (descIssue.HasValue) issues.Add(descIssue.Value);

            // Date must not be null.
            if (target.Date == null)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "required.missing",
                    ref_,
                    "Date",
                    $"{ref_}.Date is null."));
            }

            return ValidationResult.WithIssues(issues);
        }

        // -------------------------------------------------------------------------
        // MonthlyFinanceSummary
        // -------------------------------------------------------------------------

        /// <inheritdoc/>
        public ValidationResult Validate(MonthlyFinanceSummary target)
        {
            if (target == null)
            {
                var entity = new ValidationEntityReference("MonthlyFinanceSummary");
                return ValidationResult.WithIssues(new[]
                {
                    new ValidationIssue(
                        ValidationSeverity.Error,
                        "required.missing",
                        entity,
                        "target",
                        "MonthlyFinanceSummary instance is null.")
                });
            }

            var issues = new List<ValidationIssue>();
            var ref_ = new ValidationEntityReference("MonthlyFinanceSummary", target.Id);

            var idIssue = ValidationRules.RequiredId(target.Id, "Id", ref_);
            if (idIssue.HasValue) issues.Add(idIssue.Value);

            // FinancePeriodKey month: 1–12.
            var monthIssue = ValidationRules.Range(target.Period.Month, 1, 12, "Period.Month", ref_);
            if (monthIssue.HasValue) issues.Add(monthIssue.Value);

            // Year must be greater than 0.
            if (target.Period.Year <= 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.out_of_bounds",
                    ref_,
                    "Period.Year",
                    $"{ref_}.Period.Year value {target.Period.Year} must be greater than 0."));
            }

            // Total revenue and expenses must be non-negative.
            var revenueIssue = ValidationRules.NonNegative(target.TotalRevenueMinorUnits, "TotalRevenueMinorUnits", ref_);
            if (revenueIssue.HasValue) issues.Add(revenueIssue.Value);

            var expensesIssue = ValidationRules.NonNegative(target.TotalExpensesMinorUnits, "TotalExpensesMinorUnits", ref_);
            if (expensesIssue.HasValue) issues.Add(expensesIssue.Value);

            // RunwayMonths must be non-negative.
            if (target.RunwayMonths < 0)
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Error,
                    "range.negative_not_allowed",
                    ref_,
                    "RunwayMonths",
                    $"{ref_}.RunwayMonths value {target.RunwayMonths} must be zero or greater."));
            }

            return ValidationResult.WithIssues(issues);
        }
    }
}
