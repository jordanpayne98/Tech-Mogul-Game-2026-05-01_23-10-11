using System;
using System.Collections.Generic;
using Project.Core.Definitions.Finance;
using Project.Core.Runtime.Finance;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Finance;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Finance domain runtime types to and from their save data equivalents.
    /// Covers FinanceRuntimeState, TransactionRecord, and MonthlyFinanceSummary.
    /// All methods are static — this mapper holds no state.
    /// FinancePeriodKey is flattened to PeriodYear/PeriodMonth int fields.
    /// Nullable enums convert to nullable strings. Dictionary enum keys convert to string keys.
    /// </summary>
    public static class FinanceSaveMapper
    {
        // ─── FinanceRuntimeState ──────────────────────────────────────────────────

        public static FinanceSaveData ToSaveData(FinanceRuntimeState state)
        {
            // LastPayrollDate is nullable — default (zero TotalElapsedHours) means no payroll yet.
            int? lastPayrollHours = state.LastPayrollDate != null && state.LastPayrollDate.TotalElapsedHours != 0
                ? state.LastPayrollDate.TotalElapsedHours
                : (int?)null;

            return new FinanceSaveData
            {
                CompanyId                         = state.CompanyId,
                CashMinorUnits                    = state.CashMinorUnits,
                MonthlyPayrollMinorUnits          = state.MonthlyPayrollMinorUnits,
                MonthlyInfrastructureCostMinorUnits = state.MonthlyInfrastructureCostMinorUnits,
                MonthlySupportCostMinorUnits      = state.MonthlySupportCostMinorUnits,
                MonthlyMarketingSpendMinorUnits   = state.MonthlyMarketingSpendMinorUnits,
                MonthlyResearchSpendMinorUnits    = state.MonthlyResearchSpendMinorUnits,
                MonthlyManufacturingCostMinorUnits = state.MonthlyManufacturingCostMinorUnits,
                MonthlyProductRevenueMinorUnits   = state.MonthlyProductRevenueMinorUnits,
                MonthlyContractRevenueMinorUnits  = state.MonthlyContractRevenueMinorUnits,
                MonthlyNetProfitLossMinorUnits    = state.MonthlyNetProfitLossMinorUnits,
                RunwayMonths                      = state.RunwayMonths,
                IsRunwayStable                    = state.IsRunwayStable,
                LastPayrollDateTotalElapsedHours  = lastPayrollHours,
                TransactionIds                    = state.TransactionIds,
                MonthlySummaryIds                 = state.MonthlySummaryIds
            };
        }

        public static FinanceRuntimeState FromSaveData(FinanceSaveData data)
        {
            GameDateTime lastPayrollDate = data.LastPayrollDateTotalElapsedHours.HasValue
                ? GameDateTime.FromTotalHours(data.LastPayrollDateTotalElapsedHours.Value)
                : default;

            var state = new FinanceRuntimeState
            {
                CompanyId                           = data.CompanyId,
                CashMinorUnits                      = data.CashMinorUnits,
                MonthlyPayrollMinorUnits            = data.MonthlyPayrollMinorUnits,
                MonthlyInfrastructureCostMinorUnits = data.MonthlyInfrastructureCostMinorUnits,
                MonthlySupportCostMinorUnits        = data.MonthlySupportCostMinorUnits,
                MonthlyMarketingSpendMinorUnits     = data.MonthlyMarketingSpendMinorUnits,
                MonthlyResearchSpendMinorUnits      = data.MonthlyResearchSpendMinorUnits,
                MonthlyManufacturingCostMinorUnits  = data.MonthlyManufacturingCostMinorUnits,
                MonthlyProductRevenueMinorUnits     = data.MonthlyProductRevenueMinorUnits,
                MonthlyContractRevenueMinorUnits    = data.MonthlyContractRevenueMinorUnits,
                MonthlyNetProfitLossMinorUnits      = data.MonthlyNetProfitLossMinorUnits,
                RunwayMonths                        = data.RunwayMonths,
                IsRunwayStable                      = data.IsRunwayStable,
                LastPayrollDate                     = lastPayrollDate
            };

            state.TransactionIds.AddRange(data.TransactionIds);
            state.MonthlySummaryIds.AddRange(data.MonthlySummaryIds);

            return state;
        }

        // ─── TransactionRecord ────────────────────────────────────────────────────

        public static TransactionSaveData ToSaveData(TransactionRecord record)
        {
            return new TransactionSaveData
            {
                Id                  = record.Id,
                Type                = record.Type.ToString(),
                ExpenseCategory     = record.ExpenseCategory.HasValue ? record.ExpenseCategory.Value.ToString() : null,
                RevenueSource       = record.RevenueSource.HasValue ? record.RevenueSource.Value.ToString() : null,
                AmountMinorUnits    = record.AmountMinorUnits,
                Description         = record.Description,
                RelatedEntityId     = record.RelatedEntityId,
                RelatedEntityType   = record.RelatedEntityType,
                DateTotalElapsedHours = record.Date.TotalElapsedHours
            };
        }

        public static TransactionRecord FromSaveData(TransactionSaveData data)
        {
            ExpenseCategory? expenseCategory = string.IsNullOrEmpty(data.ExpenseCategory)
                ? (ExpenseCategory?)null
                : Enum.Parse<ExpenseCategory>(data.ExpenseCategory);

            RevenueSource? revenueSource = string.IsNullOrEmpty(data.RevenueSource)
                ? (RevenueSource?)null
                : Enum.Parse<RevenueSource>(data.RevenueSource);

            return new TransactionRecord
            {
                Id                = data.Id,
                Type              = Enum.Parse<TransactionType>(data.Type),
                ExpenseCategory   = expenseCategory,
                RevenueSource     = revenueSource,
                AmountMinorUnits  = data.AmountMinorUnits,
                Description       = data.Description,
                RelatedEntityId   = data.RelatedEntityId,
                RelatedEntityType = data.RelatedEntityType,
                Date              = GameDateTime.FromTotalHours(data.DateTotalElapsedHours)
            };
        }

        // ─── MonthlyFinanceSummary ────────────────────────────────────────────────

        public static MonthlyFinanceSummarySaveData ToSaveData(MonthlyFinanceSummary summary)
        {
            var revenueBySource = new Dictionary<string, long>(summary.RevenueBySource.Count);
            foreach (KeyValuePair<RevenueSource, long> pair in summary.RevenueBySource)
            {
                revenueBySource[pair.Key.ToString()] = pair.Value;
            }

            var expensesByCategory = new Dictionary<string, long>(summary.ExpensesByCategory.Count);
            foreach (KeyValuePair<ExpenseCategory, long> pair in summary.ExpensesByCategory)
            {
                expensesByCategory[pair.Key.ToString()] = pair.Value;
            }

            return new MonthlyFinanceSummarySaveData
            {
                Id                      = summary.Id,
                PeriodYear              = summary.Period.Year,
                PeriodMonth             = summary.Period.Month,
                CashAtStartMinorUnits   = summary.CashAtStartMinorUnits,
                CashAtEndMinorUnits     = summary.CashAtEndMinorUnits,
                TotalRevenueMinorUnits  = summary.TotalRevenueMinorUnits,
                TotalExpensesMinorUnits = summary.TotalExpensesMinorUnits,
                NetProfitLossMinorUnits = summary.NetProfitLossMinorUnits,
                RevenueBySource         = revenueBySource,
                ExpensesByCategory      = expensesByCategory,
                RunwayMonths            = summary.RunwayMonths,
                IsRunwayStable          = summary.IsRunwayStable,
                PayrollChangeMinorUnits = summary.PayrollChangeMinorUnits
            };
        }

        public static MonthlyFinanceSummary FromSaveData(MonthlyFinanceSummarySaveData data)
        {
            var revenueBySource = new Dictionary<RevenueSource, long>(data.RevenueBySource.Count);
            foreach (KeyValuePair<string, long> pair in data.RevenueBySource)
            {
                revenueBySource[Enum.Parse<RevenueSource>(pair.Key)] = pair.Value;
            }

            var expensesByCategory = new Dictionary<ExpenseCategory, long>(data.ExpensesByCategory.Count);
            foreach (KeyValuePair<string, long> pair in data.ExpensesByCategory)
            {
                expensesByCategory[Enum.Parse<ExpenseCategory>(pair.Key)] = pair.Value;
            }

            return new MonthlyFinanceSummary
            {
                Id                      = data.Id,
                Period                  = new FinancePeriodKey(data.PeriodYear, data.PeriodMonth),
                CashAtStartMinorUnits   = data.CashAtStartMinorUnits,
                CashAtEndMinorUnits     = data.CashAtEndMinorUnits,
                TotalRevenueMinorUnits  = data.TotalRevenueMinorUnits,
                TotalExpensesMinorUnits = data.TotalExpensesMinorUnits,
                NetProfitLossMinorUnits = data.NetProfitLossMinorUnits,
                RevenueBySource         = revenueBySource,
                ExpensesByCategory      = expensesByCategory,
                RunwayMonths            = data.RunwayMonths,
                IsRunwayStable          = data.IsRunwayStable,
                PayrollChangeMinorUnits = data.PayrollChangeMinorUnits
            };
        }
    }
}
