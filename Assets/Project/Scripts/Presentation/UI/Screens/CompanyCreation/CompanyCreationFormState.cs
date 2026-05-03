using UnityEngine;

namespace Project.Presentation.UI.Screens.CompanyCreation
{
    /// <summary>
    /// Internal mutable state holder for all Company Creation wizard form fields.
    /// This is the controller's source of truth — ViewModels are projected from this.
    /// Not a ViewModel; never exposed directly to the view layer.
    /// </summary>
    internal sealed class CompanyCreationFormState
    {
        // ─── Company identity ────────────────────────────────────────────────────────

        public string CompanyName   { get; set; }
        public string IndustryFocus { get; set; }
        public string Headquarters  { get; set; }
        public string CompanyColour { get; set; }
        public string LogoId        { get; set; }

        // ─── Background ──────────────────────────────────────────────────────────────

        public string BackgroundId { get; set; }

        // ─── Founders ────────────────────────────────────────────────────────────────

        public string FounderSetupType { get; set; }

        // ─── Founder profile 0 ──────────────────────────────────────────────────────

        public string Founder0FirstName    { get; set; }
        public string Founder0LastName     { get; set; }
        public int    Founder0Age          { get; set; }
        public string Founder0Nationality  { get; set; }
        public string Founder0Location     { get; set; }
        public string Founder0Background   { get; set; }
        public string Founder0SkillProfile { get; set; }

        // ─── Founder profile 1 (co-founder) ─────────────────────────────────────────

        public string Founder1FirstName    { get; set; }
        public string Founder1LastName     { get; set; }
        public int    Founder1Age          { get; set; }
        public string Founder1Nationality  { get; set; }
        public string Founder1Location     { get; set; }
        public string Founder1Background   { get; set; }
        public string Founder1SkillProfile { get; set; }

        // ─── Team & budget ───────────────────────────────────────────────────────────

        public string StartingCashPreset { get; set; }
        public string StartingTeamChoice { get; set; }

        // ─── Sandbox settings ────────────────────────────────────────────────────────

        public string MarketSize          { get; set; }
        public string CompetitorDensity   { get; set; }
        public string TechnologyPace      { get; set; }
        public string EconomicVolatility  { get; set; }
        public string HiringDifficulty    { get; set; }
        public string FailureMode         { get; set; }
        public string MarketSeed          { get; set; }

        // ─── Constructor ─────────────────────────────────────────────────────────────

        public CompanyCreationFormState()
        {
            Reset();
        }

        // ─── Public API ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Resets all fields to their default values.
        /// Called on controller Initialize and whenever the screen is closed/reopened.
        /// </summary>
        public void Reset()
        {
            CompanyName   = string.Empty;
            IndustryFocus = null;
            Headquarters  = null;
            CompanyColour = string.Empty;
            LogoId        = string.Empty;

            BackgroundId = null;

            FounderSetupType = null;

            Founder0FirstName    = string.Empty;
            Founder0LastName     = string.Empty;
            Founder0Age          = 0;
            Founder0Nationality  = null;
            Founder0Location     = null;
            Founder0Background   = null;
            Founder0SkillProfile = null;

            Founder1FirstName    = string.Empty;
            Founder1LastName     = string.Empty;
            Founder1Age          = 0;
            Founder1Nationality  = null;
            Founder1Location     = null;
            Founder1Background   = null;
            Founder1SkillProfile = null;

            StartingCashPreset = "standard";
            StartingTeamChoice = null;

            MarketSize         = "Standard";
            CompetitorDensity  = "Standard";
            TechnologyPace     = "Standard";
            EconomicVolatility = "Standard";
            HiringDifficulty   = "Standard";
            FailureMode        = "Standard";
            MarketSeed         = Random.Range(100000, 999999).ToString();
        }
    }
}
