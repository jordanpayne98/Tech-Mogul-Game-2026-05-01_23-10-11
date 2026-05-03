using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Definitions.Finance;
using Project.Core.Definitions.Product;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Runtime.Employee;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Product;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Finance
{
    /// <summary>
    /// Stateless finance calculation service.
    /// Implements payroll, software and hardware revenue, recurring expense placeholders,
    /// runway calculation, and transaction/summary helper methods.
    /// Does not publish events. Does not mutate any state — returns computed values and new record instances.
    /// Defined in Plan 2H, GDD_11.
    /// </summary>
    public sealed class FinanceService : IFinanceService
    {
        // ─── Payroll ──────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public long ComputePayroll(List<EmployeeRuntimeState> activeEmployees)
        {
            if (activeEmployees == null || activeEmployees.Count == 0)
            {
                return 0L;
            }

            long total = 0L;

            foreach (var employee in activeEmployees)
            {
                total += employee.SalaryMinorUnits;
            }

            return total;
        }

        // ─── Software Revenue ─────────────────────────────────────────────────────

        /// <inheritdoc/>
        public long ComputeSoftwareProductRevenue(ProductProfile profile, ProductRuntimeState productState)
        {
            if (productState.Status != ProductStatus.Launched
                && productState.Status != ProductStatus.Supported)
            {
                return 0L;
            }

            switch (profile.RevenueModel)
            {
                case RevenueModel.OneTimePurchase:
                    return productState.UnitsSoldThisMonth * profile.PriceMinorUnits;

                case RevenueModel.Subscription:
                    return (long)productState.ActiveUsers * profile.PriceMinorUnits;

                case RevenueModel.Freemium:
                    // Non-functional in Plan 2H. Returns 0 until Freemium support is explicitly added.
                    return 0L;

                default:
                    return 0L;
            }
        }

        // ─── Hardware Revenue ─────────────────────────────────────────────────────

        /// <inheritdoc/>
        public long ComputeHardwareNetRevenue(ProductProfile profile, ProductRuntimeState productState, IFinanceTuning tuning)
        {
            if (productState.Status != ProductStatus.Launched
                && productState.Status != ProductStatus.Supported)
            {
                return 0L;
            }

            long grossRevenue   = productState.UnitsSoldThisMonth * profile.PriceMinorUnits;
            long retailerMargin = (long)(grossRevenue * tuning.HardwareRetailerMarginPercent / 100.0);

            return grossRevenue - retailerMargin;
        }

        /// <inheritdoc/>
        public long ComputeHardwareCOGS(ProductRuntimeState productState, HardwareRuntimeMetrics metrics)
        {
            return productState.UnitsSoldThisMonth * metrics.ManufacturingCostPerUnitMinorUnits;
        }

        // ─── Recurring Expenses ───────────────────────────────────────────────────

        /// <inheritdoc/>
        public long ComputeInfrastructureExpense(int launchedProductCount, IFinanceTuning tuning)
        {
            // [Placeholder] See formula.finance.infrastructure_expense
            return launchedProductCount * tuning.BaseInfrastructureCostPerProductMinorUnits;
        }

        /// <inheritdoc/>
        public long ComputeMarketingExpense(List<ProductBudgetProfile> budgets, List<ProductRuntimeState> productStates, List<ProductProfile> profiles)
        {
            if (budgets == null || budgets.Count == 0)
            {
                return 0L;
            }

            long total = 0L;

            foreach (var budget in budgets)
            {
                var productState = productStates?.FirstOrDefault(s => s.ProductId == budget.ProductId);

                if (productState == null)
                {
                    continue;
                }

                if (productState.Status == ProductStatus.Launched
                    || productState.Status == ProductStatus.Supported)
                {
                    total += budget.PostLaunchMarketingMonthlyBudgetMinorUnits;
                }
                else if (productState.Status == ProductStatus.InConcept
                         || productState.Status == ProductStatus.InPrototype
                         || productState.Status == ProductStatus.InDevelopment
                         || productState.Status == ProductStatus.InQA
                         || productState.Status == ProductStatus.ReadyForLaunch
                         || productState.Status == ProductStatus.InManufacturingPrep)
                {
                    total += budget.PreLaunchMarketingMonthlyBudgetMinorUnits;
                }
            }

            return total;
        }

        /// <inheritdoc/>
        public long ComputeSupportExpense(List<ProductBudgetProfile> budgets, List<ProductProfile> profiles, List<ProductRuntimeState> productStates)
        {
            if (budgets == null || budgets.Count == 0)
            {
                return 0L;
            }

            long total = 0L;

            foreach (var budget in budgets)
            {
                var productState = productStates?.FirstOrDefault(s => s.ProductId == budget.ProductId);

                if (productState == null)
                {
                    continue;
                }

                if (productState.Status != ProductStatus.Launched
                    && productState.Status != ProductStatus.Supported)
                {
                    continue;
                }

                var profile = profiles?.FirstOrDefault(p => p.Id == budget.ProductId);

                if (profile == null || !profile.RequiresSupport)
                {
                    continue;
                }

                total += budget.PostLaunchSupportMonthlyBudgetMinorUnits;
            }

            return total;
        }

        /// <inheritdoc/>
        public long ComputeManufacturingExpense(int activeHardwareProductCount, IFinanceTuning tuning)
        {
            // [Placeholder] See formula.finance.manufacturing_overhead_expense
            return activeHardwareProductCount * tuning.BaseManufacturingMonthlyCostMinorUnits;
        }

        /// <inheritdoc/>
        public long ComputeResearchExpense(int activeResearchProjectCount, IFinanceTuning tuning)
        {
            // [Placeholder] See formula.finance.research_expense
            return activeResearchProjectCount * tuning.BaseResearchMonthlyCostMinorUnits;
        }

        // ─── Runway ───────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public (int runwayMonths, bool isStable) ComputeRunway(long cashMinorUnits, long totalMonthlyExpenses, long totalMonthlyRevenue)
        {
            long monthlyNetBurn = totalMonthlyExpenses - totalMonthlyRevenue;

            if (monthlyNetBurn <= 0L)
            {
                // Company is profitable or breaking even — runway is effectively infinite.
                return (int.MaxValue, true);
            }

            // Negative cash = 0 runway.
            if (cashMinorUnits <= 0L)
            {
                return (0, false);
            }

            int runwayMonths = (int)(cashMinorUnits / monthlyNetBurn);

            return (runwayMonths, false);
        }

        // ─── Transaction Helpers ──────────────────────────────────────────────────

        /// <inheritdoc/>
        public TransactionRecord CreateExpenseTransaction(
            ExpenseCategory category,
            long amountMinorUnits,
            string description,
            GameDateTime date,
            string relatedEntityId,
            string relatedEntityType)
        {
            return new TransactionRecord
            {
                Id                = Guid.NewGuid().ToString("N"),
                Type              = TransactionType.Expense,
                ExpenseCategory   = category,
                RevenueSource     = null,
                AmountMinorUnits  = amountMinorUnits,
                Description       = description,
                Date              = date,
                RelatedEntityId   = relatedEntityId,
                RelatedEntityType = relatedEntityType
            };
        }

        /// <inheritdoc/>
        public TransactionRecord CreateIncomeTransaction(
            RevenueSource source,
            long amountMinorUnits,
            string description,
            GameDateTime date,
            string relatedEntityId,
            string relatedEntityType)
        {
            return new TransactionRecord
            {
                Id                = Guid.NewGuid().ToString("N"),
                Type              = TransactionType.Income,
                RevenueSource     = source,
                ExpenseCategory   = null,
                AmountMinorUnits  = amountMinorUnits,
                Description       = description,
                Date              = date,
                RelatedEntityId   = relatedEntityId,
                RelatedEntityType = relatedEntityType
            };
        }

        // ─── Summary Helper ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public MonthlyFinanceSummary CreateMonthlySummary(
            FinancePeriodKey period,
            long cashAtStart,
            long cashAtEnd,
            Dictionary<RevenueSource, long> revenueBySource,
            Dictionary<ExpenseCategory, long> expensesByCategory,
            int runwayMonths,
            bool isRunwayStable,
            long payrollChange)
        {
            long totalRevenue  = 0L;
            long totalExpenses = 0L;

            if (revenueBySource != null)
            {
                foreach (var kvp in revenueBySource)
                {
                    totalRevenue += kvp.Value;
                }
            }

            if (expensesByCategory != null)
            {
                foreach (var kvp in expensesByCategory)
                {
                    totalExpenses += kvp.Value;
                }
            }

            return new MonthlyFinanceSummary
            {
                Id                      = Guid.NewGuid().ToString("N"),
                Period                  = period,
                CashAtStartMinorUnits   = cashAtStart,
                CashAtEndMinorUnits     = cashAtEnd,
                TotalRevenueMinorUnits  = totalRevenue,
                TotalExpensesMinorUnits = totalExpenses,
                NetProfitLossMinorUnits = totalRevenue - totalExpenses,
                RevenueBySource         = revenueBySource ?? new Dictionary<RevenueSource, long>(),
                ExpensesByCategory      = expensesByCategory ?? new Dictionary<ExpenseCategory, long>(),
                RunwayMonths            = runwayMonths,
                IsRunwayStable          = isRunwayStable,
                PayrollChangeMinorUnits = payrollChange
            };
        }
    }
}
