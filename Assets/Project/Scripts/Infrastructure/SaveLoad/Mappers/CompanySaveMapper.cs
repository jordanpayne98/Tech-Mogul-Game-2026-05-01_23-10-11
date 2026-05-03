using System;
using Project.Core.Definitions.Company;
using Project.Core.Runtime.Company;
using Project.Core.Runtime.Time;
using Project.Core.SaveData.Company;

namespace Project.Infrastructure.SaveLoad.Mappers
{
    /// <summary>
    /// Maps Company domain runtime types to and from their save data equivalents.
    /// All methods are static — this mapper holds no state.
    /// GameDateTime fields convert via TotalElapsedHours / GameDateTime.FromTotalHours().
    /// Enum fields convert via ToString() / Enum.Parse().
    /// </summary>
    public static class CompanySaveMapper
    {
        // ─── CompanyProfile ───────────────────────────────────────────────────────

        public static CompanySaveData ToSaveData(CompanyProfile profile)
        {
            return new CompanySaveData
            {
                Id                          = profile.Id,
                Name                        = profile.Name,
                FounderId                   = profile.FounderId,
                LogoIconId                  = profile.LogoIconId,
                BrandColourHex              = profile.BrandColourHex,
                Focus                       = profile.Focus.ToString(),
                Location                    = profile.Location,
                FoundedDateTotalElapsedHours = profile.FoundedDate.TotalElapsedHours
            };
        }

        public static CompanyProfile FromSaveData(CompanySaveData data)
        {
            return new CompanyProfile(
                id:             data.Id,
                name:           data.Name,
                founderId:      data.FounderId,
                logoIconId:     data.LogoIconId,
                brandColourHex: data.BrandColourHex,
                focus:          Enum.Parse<CompanyFocus>(data.Focus),
                location:       data.Location,
                foundedDate:    GameDateTime.FromTotalHours(data.FoundedDateTotalElapsedHours)
            );
        }

        // ─── FounderProfile ───────────────────────────────────────────────────────

        public static FounderSaveData ToSaveData(FounderProfile profile)
        {
            return new FounderSaveData
            {
                Id         = profile.Id,
                Name       = profile.Name,
                Background = profile.Background.ToString()
            };
        }

        public static FounderProfile FromSaveData(FounderSaveData data)
        {
            return new FounderProfile(
                id:         data.Id,
                name:       data.Name,
                background: Enum.Parse<FounderBackground>(data.Background)
            );
        }

        // ─── CompanyRuntimeState ──────────────────────────────────────────────────

        public static CompanyStateSaveData ToSaveData(CompanyRuntimeState state)
        {
            return new CompanyStateSaveData
            {
                CompanyId           = state.CompanyId,
                Reputation          = state.Reputation,
                EmployeeIds         = state.EmployeeIds,
                TeamIds             = state.TeamIds,
                ProductIds          = state.ProductIds,
                ContractIds         = state.ContractIds,
                ResearchProjectIds  = state.ResearchProjectIds,
                ReportIds           = state.ReportIds,
                MarketPositionIds   = state.MarketPositionIds
            };
        }

        public static CompanyRuntimeState FromSaveData(CompanyStateSaveData data)
        {
            return new CompanyRuntimeState
            {
                CompanyId           = data.CompanyId,
                Reputation          = data.Reputation,
                EmployeeIds         = data.EmployeeIds,
                TeamIds             = data.TeamIds,
                ProductIds          = data.ProductIds,
                ContractIds         = data.ContractIds,
                ResearchProjectIds  = data.ResearchProjectIds,
                ReportIds           = data.ReportIds,
                MarketPositionIds   = data.MarketPositionIds
            };
        }
    }
}
