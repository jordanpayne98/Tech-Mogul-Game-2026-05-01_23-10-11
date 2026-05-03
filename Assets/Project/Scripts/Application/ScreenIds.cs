namespace Project.Application
{
    /// <summary>
    /// Stable string constants for all screen IDs.
    /// IDs must not change without data migration — use these constants everywhere,
    /// never inline the raw strings at call sites.
    /// </summary>
    public static class ScreenIds
    {
        public const string Portal           = "screen.portal";
        public const string Company          = "screen.company";
        public const string Employees        = "screen.employees";
        public const string Recruitment      = "screen.recruitment";
        public const string Teams            = "screen.teams";
        public const string Products         = "screen.products";
        public const string ProductDetail    = "screen.product_detail";
        public const string Contracts        = "screen.contracts";
        public const string Research         = "screen.research";
        public const string Infrastructure   = "screen.infrastructure";
        public const string Market           = "screen.market";
        public const string Competitors      = "screen.competitors";
        public const string Finance          = "screen.finance";
        public const string Reports          = "screen.reports";
        public const string ReportsFinance   = "screen.reports_finance";
        public const string Calendar         = "screen.calendar";
        public const string Settings         = "screen.settings";
        public const string CompanyCreation  = "screen.company_creation";
        public const string MainMenu         = "screen.main_menu";
    }
}
