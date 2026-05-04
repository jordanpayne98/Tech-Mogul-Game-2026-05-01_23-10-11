namespace Project.Presentation.UI.Screens.Company
{
    /// <summary>
    /// Pure display-data class for the company identity card on the Company screen.
    /// Immutable after construction. No Unity dependencies.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class CompanyIdentityViewModel
    {
        /// <summary>Display name of the company, e.g. "Apex Systems".</summary>
        public string CompanyName { get; }

        /// <summary>Stable company identifier, e.g. "company.apex_systems".</summary>
        public string CompanyId { get; }

        /// <summary>Human-readable founding date string, e.g. "March 2024".</summary>
        public string FoundedDate { get; }

        /// <summary>Headquarters location display string, e.g. "San Francisco, CA".</summary>
        public string Headquarters { get; }

        /// <summary>USS icon class used to render the company logo glyph.</summary>
        public string LogoIconClass { get; }

        /// <summary>USS colour modifier class applied to the company identity panel accent.</summary>
        public string CompanyColourClass { get; }

        /// <summary>Starting focus area display label, e.g. "Consumer Software".</summary>
        public string StartingFocus { get; }

        /// <summary>
        /// Current focus area display label, e.g. "Enterprise SaaS".
        /// Company focus is identity/preference display only — no mechanical bonus is implied.
        /// </summary>
        public string CurrentFocus { get; }

        /// <summary>True when the player may edit company identity details.</summary>
        public bool IsEditable { get; }

        /// <summary>Route ID for the company edit screen. Only relevant when IsEditable is true.</summary>
        public string EditRouteId { get; }

        public CompanyIdentityViewModel(
            string companyName,
            string companyId,
            string foundedDate,
            string headquarters,
            string logoIconClass,
            string companyColourClass,
            string startingFocus,
            string currentFocus,
            bool isEditable,
            string editRouteId)
        {
            CompanyName       = companyName;
            CompanyId         = companyId;
            FoundedDate       = foundedDate;
            Headquarters      = headquarters;
            LogoIconClass     = logoIconClass;
            CompanyColourClass = companyColourClass;
            StartingFocus     = startingFocus;
            CurrentFocus      = currentFocus;
            IsEditable        = isEditable;
            EditRouteId       = editRouteId;
        }
    }
}
