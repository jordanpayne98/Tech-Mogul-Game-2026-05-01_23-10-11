using Project.Core.Interfaces;
using Project.Core.Interfaces.Tuning;
using UnityEngine;

namespace Project.Infrastructure.Tuning
{
    /// <summary>
    /// Inspector-editable ScriptableObject containing all centralized tuning values.
    /// Loaded once by the bootstrapper and passed to systems via explicit dependency.
    /// Implements ITuningConfig so Core/Application layers can consume values without
    /// depending on Infrastructure directly.
    ///
    /// All initial values are [Placeholder] — update as GDD systems are implemented.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TuningConfig",
        menuName = "Project/Tuning Config")]
    public sealed class TuningConfig : ScriptableObject, ITuningConfig
    {
        // ─── General ──────────────────────────────────────────────────────────────

        [Header("General")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Default delay in seconds before a generic action is applied.")]
        private float _defaultActionDelaySeconds = 0.25f;

        // ─── Validation ───────────────────────────────────────────────────────────

        [Header("Validation")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum number of items that may be generated in a single operation.")]
        private int _maxGeneratedItems = 100;

        // ─── Timing ───────────────────────────────────────────────────────────────

        [Header("Timing")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] How frequently, in seconds, UI data is refreshed from runtime state.")]
        private float _uiRefreshIntervalSeconds = 0.5f;

        // ─── SaveLoad ─────────────────────────────────────────────────────────────

        [Header("SaveLoad")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Interval in seconds between automatic saves. 0 disables autosave.")]
        private float _autosaveIntervalSeconds = 300f;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of save slots available to the player.")]
        private int _maxSaveSlots = 5;

        // ─── Save ─────────────────────────────────────────────────────────────────

        [Header("Save")]
        [SerializeField,
         Tooltip("[Placeholder] Maximum number of rolling autosave slots. When exceeded, the oldest autosave is deleted before writing a new one. Valid range: 1–20.")]
        private int _maxAutosaveSlots = 5;

        [SerializeField,
         Tooltip("[Placeholder] Maximum number of manual save slots. When reached, RequestManualSave() returns a failure. Player must delete a save to continue. Valid range: 1–100.")]
        private int _maxManualSaveSlots = 20;

        // ─── Time ─────────────────────────────────────────────────────────────────

        [Header("Time")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Hours advanced per simulation tick.")]
        private int _defaultTickHours = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum ticks processed per Continue press. Safety cap.")]
        private int _maxTicksPerContinue = 8640;

        // ─── ITuningConfig Properties ─────────────────────────────────────────────

        public float DefaultActionDelaySeconds => _defaultActionDelaySeconds;
        public int MaxGeneratedItems           => _maxGeneratedItems;
        public float UIRefreshIntervalSeconds  => _uiRefreshIntervalSeconds;
        public float AutosaveIntervalSeconds   => _autosaveIntervalSeconds;
        public int MaxSaveSlots                => _maxSaveSlots;

        // ─── ISaveTuning Properties ───────────────────────────────────────────────

        public int MaxAutosaveSlots   => _maxAutosaveSlots;
        public int MaxManualSaveSlots => _maxManualSaveSlots;

        // ─── Company — Starting Capital ───────────────────────────────────────────

        [Header("Company — Starting Capital")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Garage preset starting cash in major currency units. Converted to minor units (x100) at runtime.")]
        private int _garageCashMajorUnits = 35000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Bootstrapped preset starting cash in major currency units. Converted to minor units (x100) at runtime.")]
        private int _bootstrappedCashMajorUnits = 75000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Seed Funded preset starting cash in major currency units. Converted to minor units (x100) at runtime.")]
        private int _seedFundedCashMajorUnits = 250000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Venture Start preset starting cash in major currency units. Converted to minor units (x100) at runtime.")]
        private int _ventureStartCashMajorUnits = 1000000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Sandbox preset starting cash in major currency units. Converted to minor units (x100) at runtime.")]
        private int _sandboxDefaultCashMajorUnits = 75000;

        // ─── Company — Reputation ─────────────────────────────────────────────────

        [Header("Company — Reputation")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base reputation score applied to all new companies before founder bonus.")]
        private int _defaultReputation = 10;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Engineer founder background.")]
        private int _engineerReputationBonus = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Product Designer founder background.")]
        private int _productDesignerReputationBonus = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Sales Founder background.")]
        private int _salesFounderReputationBonus = 10;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Hardware Specialist founder background.")]
        private int _hardwareSpecialistReputationBonus = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Research Founder background.")]
        private int _researchFounderReputationBonus = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Serial Founder background.")]
        private int _serialFounderReputationBonus = 15;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional reputation bonus for the Bootstrapped Founder background.")]
        private int _bootstrappedFounderReputationBonus = 0;

        // ─── ITimeTuning Properties ───────────────────────────────────────────────

        public int DefaultTickHours     => _defaultTickHours;
        public int MaxTicksPerContinue  => _maxTicksPerContinue;

        // ─── ICompanyTuning Properties ────────────────────────────────────────────

        // Cash values: stored as major units in the Inspector, returned as minor units (x100).
        public long GarageCashMinorUnits              => (long)_garageCashMajorUnits * 100L;
        public long BootstrappedCashMinorUnits        => (long)_bootstrappedCashMajorUnits * 100L;
        public long SeedFundedCashMinorUnits          => (long)_seedFundedCashMajorUnits * 100L;
        public long VentureStartCashMinorUnits        => (long)_ventureStartCashMajorUnits * 100L;
        public long SandboxDefaultCashMinorUnits      => (long)_sandboxDefaultCashMajorUnits * 100L;

        public int DefaultReputation                  => _defaultReputation;
        public int EngineerReputationBonus            => _engineerReputationBonus;
        public int ProductDesignerReputationBonus     => _productDesignerReputationBonus;
        public int SalesFounderReputationBonus        => _salesFounderReputationBonus;
        public int HardwareSpecialistReputationBonus  => _hardwareSpecialistReputationBonus;
        public int ResearchFounderReputationBonus     => _researchFounderReputationBonus;
        public int SerialFounderReputationBonus       => _serialFounderReputationBonus;
        public int BootstrappedFounderReputationBonus => _bootstrappedFounderReputationBonus;

        // ─── Team — General ───────────────────────────────────────────────────────

        [Header("Team — General")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base daily progress points a team produces before any multipliers.")]
        private int _baseTeamCapacity = 10;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Starting cohesion score for newly created teams.")]
        private int _defaultCohesion = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Starting morale score for newly created teams.")]
        private int _defaultMorale = 60;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of members allowed in a single team.")]
        private int _maxTeamSize = 8;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum number of members required to create a team.")]
        private int _minTeamSize = 1;

        // ─── Team — Progress ──────────────────────────────────────────────────────

        [Header("Team — Progress")]
        [SerializeField, Min(1f),
         Tooltip("[Placeholder] Progress multiplier applied when a Lead+ seniority member exists in the team.")]
        private float _leadCoordinationBonusMultiplier = 1.1f;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Floor for daily progress output. Team always produces at least this many points.")]
        private int _minDailyProgress = 1;

        // ─── Team — Cohesion ──────────────────────────────────────────────────────

        [Header("Team — Cohesion")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Cohesion points added per day while the team has an active assignment.")]
        private float _cohesionGrowthRatePerDay = 1.0f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Cohesion points deducted when a member is added to or removed from a team.")]
        private int _cohesionMemberChangePenalty = 5;

        // ─── Team — Workload ──────────────────────────────────────────────────────

        [Header("Team — Workload")]
        [SerializeField, Min(100f),
         Tooltip("[Placeholder] Workload threshold above which an employee is considered overloaded. Overload is impossible in MVP (binary workload 100 < 120).")]
        private float _overloadThresholdPercent = 120f;

        // ─── ITeamTuning Properties ───────────────────────────────────────────────

        public int   BaseTeamCapacity                => _baseTeamCapacity;
        public int   DefaultCohesion                 => _defaultCohesion;
        public int   DefaultMorale                   => _defaultMorale;
        public int   MaxTeamSize                     => _maxTeamSize;
        public int   MinTeamSize                     => _minTeamSize;
        public float LeadCoordinationBonusMultiplier => _leadCoordinationBonusMultiplier;
        public int   MinDailyProgress                => _minDailyProgress;
        public float CohesionGrowthRatePerDay        => _cohesionGrowthRatePerDay;
        public int   CohesionMemberChangePenalty     => _cohesionMemberChangePenalty;
        public float OverloadThresholdPercent        => _overloadThresholdPercent;

        // ─── Employee — Morale/Burnout ────────────────────────────────────────────

        [Header("Employee — Morale/Burnout")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Starting morale for a newly hired employee (0–100).")]
        private int _defaultEmployeeMorale = 70;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Starting loyalty for a newly hired employee (0–100).")]
        private int _defaultLoyalty = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Morale value below which an employee is considered low-morale.")]
        private int _moraleLowThreshold = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Burnout risk value above which an employee is considered at high burnout risk.")]
        private int _burnoutHighThreshold = 70;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Morale lost per day when the employee is overworked.")]
        private float _moraleDecayPerOverworkDay = 2.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Burnout risk gained per day when the employee is overworked.")]
        private float _burnoutAccumulationPerOverworkDay = 3.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Burnout risk recovered per day when the employee is resting (not overworked).")]
        private float _burnoutRecoveryPerRestDay = 1.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Loyalty gained per month for active employees.")]
        private float _loyaltyGrowthPerMonth = 1.0f;

        // ─── Employee — Offer Acceptance ──────────────────────────────────────────

        [Header("Employee — Offer Acceptance")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base probability (0–100) that a candidate accepts any offer before modifiers.")]
        private int _baseOfferAcceptancePercent = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to the salary delta component of the acceptance formula.")]
        private int _salaryWeightPercent = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to the company reputation component of the acceptance formula.")]
        private int _reputationWeightPercent = 25;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to the role-fit score component of the acceptance formula.")]
        private int _roleFitWeightPercent = 25;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to the growth opportunity component of the acceptance formula.")]
        private int _growthWeightPercent = 20;

        // ─── Employee — Candidate Generation ─────────────────────────────────────

        [Header("Employee — Candidate Generation")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of candidates generated when recruitment is initialized for a new save.")]
        private int _initialCandidatePoolSize = 30;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of new candidates added during each monthly pool refresh.")]
        private int _candidatePoolRefreshCount = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum skill value assigned during candidate generation (0–100).")]
        private int _minSkillValue = 10;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum skill value assigned during candidate generation (0–100).")]
        private int _maxSkillValue = 80;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum potential lower bound assigned during candidate generation (0–100).")]
        private int _minPotential = 20;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum potential upper bound assigned during candidate generation (0–100).")]
        private int _maxPotential = 90;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum confidence level assigned during candidate generation (0–100).")]
        private int _minCandidateConfidence = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum confidence level assigned during candidate generation (0–100).")]
        private int _maxCandidateConfidence = 70;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of personality traits assigned to each generated candidate.")]
        private int _traitsPerCandidate = 2;

        // ─── Employee — Salary Ranges ─────────────────────────────────────────────

        [Header("Employee — Salary Ranges")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _juniorSalaryMinMonthlyMajorUnits = 2083;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _juniorSalaryMaxMonthlyMajorUnits = 3333;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _midSalaryMinMonthlyMajorUnits = 3333;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _midSalaryMaxMonthlyMajorUnits = 5417;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _seniorSalaryMinMonthlyMajorUnits = 5417;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _seniorSalaryMaxMonthlyMajorUnits = 7917;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _leadSalaryMinMonthlyMajorUnits = 7083;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _leadSalaryMaxMonthlyMajorUnits = 10000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _principalSalaryMinMonthlyMajorUnits = 9167;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly salary in major currency units. Converted to minor units (x100) at runtime.")]
        private int _principalSalaryMaxMonthlyMajorUnits = 13333;

        // ─── Employee — Defaults ──────────────────────────────────────────────────

        [Header("Employee — Defaults")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum ambition value assigned to a newly hired employee (0–100).")]
        private int _defaultAmbitionMin = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum ambition value assigned to a newly hired employee (0–100).")]
        private int _defaultAmbitionMax = 70;

        // ─── IEmployeeTuning Properties ───────────────────────────────────────────

        // Morale/Burnout
        // ITeamTuning.DefaultMorale uses _defaultMorale (team morale).
        // IEmployeeTuning.DefaultMorale is implemented explicitly to avoid ambiguity.
        int IEmployeeTuning.DefaultMorale                   => _defaultEmployeeMorale;
        public int   DefaultLoyalty                    => _defaultLoyalty;
        public int   MoraleLowThreshold                => _moraleLowThreshold;
        public int   BurnoutHighThreshold              => _burnoutHighThreshold;
        public float MoraleDecayPerOverworkDay         => _moraleDecayPerOverworkDay;
        public float BurnoutAccumulationPerOverworkDay => _burnoutAccumulationPerOverworkDay;
        public float BurnoutRecoveryPerRestDay         => _burnoutRecoveryPerRestDay;
        public float LoyaltyGrowthPerMonth             => _loyaltyGrowthPerMonth;

        // Offer Acceptance
        public int BaseOfferAcceptancePercent => _baseOfferAcceptancePercent;
        public int SalaryWeightPercent        => _salaryWeightPercent;
        public int ReputationWeightPercent    => _reputationWeightPercent;
        public int RoleFitWeightPercent       => _roleFitWeightPercent;
        public int GrowthWeightPercent        => _growthWeightPercent;

        // Candidate Generation
        public int InitialCandidatePoolSize   => _initialCandidatePoolSize;
        public int CandidatePoolRefreshCount  => _candidatePoolRefreshCount;
        public int MinSkillValue              => _minSkillValue;
        public int MaxSkillValue              => _maxSkillValue;
        public int MinPotential               => _minPotential;
        public int MaxPotential               => _maxPotential;
        public int MinCandidateConfidence     => _minCandidateConfidence;
        public int MaxCandidateConfidence     => _maxCandidateConfidence;
        public int TraitsPerCandidate         => _traitsPerCandidate;

        // Salary Ranges — stored as major units, returned as minor units (x100)
        public long JuniorSalaryMinMonthlyMinorUnits    => (long)_juniorSalaryMinMonthlyMajorUnits * 100L;
        public long JuniorSalaryMaxMonthlyMinorUnits    => (long)_juniorSalaryMaxMonthlyMajorUnits * 100L;
        public long MidSalaryMinMonthlyMinorUnits       => (long)_midSalaryMinMonthlyMajorUnits * 100L;
        public long MidSalaryMaxMonthlyMinorUnits       => (long)_midSalaryMaxMonthlyMajorUnits * 100L;
        public long SeniorSalaryMinMonthlyMinorUnits    => (long)_seniorSalaryMinMonthlyMajorUnits * 100L;
        public long SeniorSalaryMaxMonthlyMinorUnits    => (long)_seniorSalaryMaxMonthlyMajorUnits * 100L;
        public long LeadSalaryMinMonthlyMinorUnits      => (long)_leadSalaryMinMonthlyMajorUnits * 100L;
        public long LeadSalaryMaxMonthlyMinorUnits      => (long)_leadSalaryMaxMonthlyMajorUnits * 100L;
        public long PrincipalSalaryMinMonthlyMinorUnits => (long)_principalSalaryMinMonthlyMajorUnits * 100L;
        public long PrincipalSalaryMaxMonthlyMinorUnits => (long)_principalSalaryMaxMonthlyMajorUnits * 100L;

        // Employee Defaults
        public int DefaultAmbitionMin => _defaultAmbitionMin;
        public int DefaultAmbitionMax => _defaultAmbitionMax;

        // ─── Product — Phase Duration ─────────────────────────────────────────────

        [Header("Product — Phase Duration")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the Concept phase.")]
        private int _conceptPhaseBaseHours = 80;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the Development phase.")]
        private int _developmentPhaseBaseHours = 240;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the QA phase.")]
        private int _qaPhaseBaseHours = 120;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours reserved for the Launch Prep phase. Not used by Software MVP.")]
        private int _launchPrepPhaseBaseHours = 40;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to phase duration based on feature scope (0–100). Higher feature scope increases required work.")]
        private float _featureScopeComplexityMultiplier = 1.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to phase duration based on quality target (0–100). Higher quality target increases required work.")]
        private float _qualityTargetComplexityMultiplier = 1.0f;

        // ─── Product — Review Score ───────────────────────────────────────────────

        [Header("Product — Review Score")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Starting review score before quality, bug, and feature satisfaction modifiers.")]
        private int _baseReviewScore = 50;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Points deducted from review score per outstanding bug. Applied as (int)(BugCount * multiplier).")]
        private float _bugImpactOnReviewScore = 2.0f;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum allowed review score after clamping.")]
        private int _minReviewScore = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum allowed review score after clamping.")]
        private int _maxReviewScore = 100;

        // ─── Product — User Metrics ───────────────────────────────────────────────

        [Header("Product — User Metrics")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Default monthly churn in basis points (100 = 1%). Prototype: 500 (5%).")]
        private int _defaultChurnBasisPoints = 500;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Basis points of churn removed per point of quality score.")]
        private float _churnReductionPerQualityPoint = 5.0f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base number of new users acquired per month before marketing and review modifiers.")]
        private int _baseNewUsersPerMonth = 50;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to new user growth based on marketing budget relative to reference.")]
        private float _userGrowthMarketingMultiplier = 1.0f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Number of active users at launch before marketing and review score scaling.")]
        private int _initialActiveUsersOnLaunch = 100;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Reference marketing budget in major currency units (x100 = minor units). Prototype: 5000 (£5,000/month).")]
        private int _marketingBudgetReferenceMajorUnits = 5000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Floor for monthly churn in basis points (100 = 1%). Prototype: 50 (0.5%).")]
        private int _minChurnBasisPoints = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Ceiling for monthly churn in basis points (100 = 1%). Prototype: 5000 (50%).")]
        private int _maxChurnBasisPoints = 5000;

        // ─── Product — Misc ───────────────────────────────────────────────────────

        [Header("Product — Misc")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum bugs generated per day at baseline user count. Scaled by active users at runtime.")]
        private int _maxBugsPerDayBase = 3;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to feature scope when deriving feature satisfaction.")]
        private float _featureSatisfactionScopeMultiplier = 1.0f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Baseline feature satisfaction before scope modifier (0–100).")]
        private int _baseFeatureSatisfaction = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base number of support tickets generated per 100 active users per month.")]
        private int _baseSupportTicketsPerHundredUsers = 5;

        // ─── Product — Hardware Phase Duration ────────────────────────────────────

        [Header("Product — Hardware Phase Duration")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the hardware Concept phase.")]
        private int _hardwareConceptPhaseBaseHours = 60;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the hardware Prototype phase.")]
        private int _hardwarePrototypePhaseBaseHours = 200;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete the hardware Manufacturing Prep phase.")]
        private int _hardwareManufacturingPrepPhaseBaseHours = 160;

        // ─── Product — Hardware Manufacturing ─────────────────────────────────────

        [Header("Product — Hardware Manufacturing")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base manufacturing cost per unit in major currency units (x100 = minor units). Prototype: 100 (£100/unit).")]
        private int _baseManufacturingCostPerUnitMajorUnits = 100;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base defect rate in basis points (100 = 1%). Prototype: 200 (2%).")]
        private int _baseDefectRateBasisPoints = 200;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base return rate in basis points (100 = 1%). Prototype: 100 (1%).")]
        private int _baseReturnRateBasisPoints = 100;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Warranty cost per defective unit in major currency units (x100 = minor units). Prototype: 50 (£50/defect).")]
        private int _warrantyCostPerDefectMajorUnits = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Default component availability score (0–100). Higher = better supply chain health. Prototype: 80.")]
        private int _defaultComponentAvailability = 80;

        // ─── Product — Hardware Sales ─────────────────────────────────────────────

        [Header("Product — Hardware Sales")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Base number of hardware units sold per month before review and inventory modifiers.")]
        private int _baseHardwareUnitsSoldPerMonth = 100;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to monthly hardware sales based on review score.")]
        private float _hardwareReviewScoreSalesMultiplier = 1.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to hardware sales based on current inventory availability.")]
        private float _hardwareInventoryAvailabilityMultiplier = 1.0f;

        // ─── Product — Hardware Review Score ──────────────────────────────────────

        [Header("Product — Hardware Review Score")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Points deducted from hardware review score per 1% defect rate. Applied as (int)(DefectBP / 100 * multiplier).")]
        private float _defectImpactOnReviewScore = 1.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Points deducted from hardware review score per 1% return rate. Applied as (int)(ReturnBP / 100 * multiplier).")]
        private float _returnImpactOnReviewScore = 0.5f;

        // ─── Product — BOM Cost Multipliers ──────────────────────────────────────

        [Header("Product — BOM Cost Multipliers")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Manufacturing cost multiplier for Budget BOM tier.")]
        private float _bomBudgetCostMultiplier = 0.6f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Manufacturing cost multiplier for Standard BOM tier.")]
        private float _bomStandardCostMultiplier = 1.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Manufacturing cost multiplier for Premium BOM tier.")]
        private float _bomPremiumCostMultiplier = 1.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Manufacturing cost multiplier for Experimental BOM tier.")]
        private float _bomExperimentalCostMultiplier = 2.5f;

        // ─── Product — BOM Defect Multipliers ────────────────────────────────────

        [Header("Product — BOM Defect Multipliers")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Defect rate multiplier for Budget BOM tier.")]
        private float _bomBudgetDefectMultiplier = 1.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Defect rate multiplier for Standard BOM tier.")]
        private float _bomStandardDefectMultiplier = 1.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Defect rate multiplier for Premium BOM tier.")]
        private float _bomPremiumDefectMultiplier = 0.7f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Defect rate multiplier for Experimental BOM tier.")]
        private float _bomExperimentalDefectMultiplier = 2.0f;

        // ─── Product — BOM Quality Bonuses ────────────────────────────────────────

        [Header("Product — BOM Quality Bonuses")]
        [SerializeField,
         Tooltip("[Placeholder] Quality score bonus at launch for Budget BOM tier. Negative = penalty.")]
        private int _bomBudgetQualityBonus = -10;

        [SerializeField,
         Tooltip("[Placeholder] Quality score bonus at launch for Standard BOM tier.")]
        private int _bomStandardQualityBonus = 0;

        [SerializeField,
         Tooltip("[Placeholder] Quality score bonus at launch for Premium BOM tier.")]
        private int _bomPremiumQualityBonus = 10;

        [SerializeField,
         Tooltip("[Placeholder] Quality score bonus at launch for Experimental BOM tier.")]
        private int _bomExperimentalQualityBonus = 20;

        // ─── IProductTuning Properties ────────────────────────────────────────────

        // Software Phase Duration
        public int   ConceptPhaseBaseHours              => _conceptPhaseBaseHours;
        public int   DevelopmentPhaseBaseHours          => _developmentPhaseBaseHours;
        public int   QAPhaseBaseHours                   => _qaPhaseBaseHours;
        public int   LaunchPrepPhaseBaseHours           => _launchPrepPhaseBaseHours;
        public float FeatureScopeComplexityMultiplier   => _featureScopeComplexityMultiplier;
        public float QualityTargetComplexityMultiplier  => _qualityTargetComplexityMultiplier;

        // Hardware Phase Duration
        public int HardwareConceptPhaseBaseHours           => _hardwareConceptPhaseBaseHours;
        public int HardwarePrototypePhaseBaseHours         => _hardwarePrototypePhaseBaseHours;
        public int HardwareManufacturingPrepPhaseBaseHours => _hardwareManufacturingPrepPhaseBaseHours;

        // Hardware Manufacturing — stored as major units in Inspector, returned as minor units (x100)
        public long BaseManufacturingCostPerUnitMinorUnits => (long)_baseManufacturingCostPerUnitMajorUnits * 100L;
        public int  BaseDefectRateBasisPoints              => _baseDefectRateBasisPoints;
        public int  BaseReturnRateBasisPoints              => _baseReturnRateBasisPoints;
        public long WarrantyCostPerDefectMinorUnits        => (long)_warrantyCostPerDefectMajorUnits * 100L;
        public int  DefaultComponentAvailability           => _defaultComponentAvailability;

        // Hardware Sales
        public int   BaseHardwareUnitsSoldPerMonth           => _baseHardwareUnitsSoldPerMonth;
        public float HardwareReviewScoreSalesMultiplier      => _hardwareReviewScoreSalesMultiplier;
        public float HardwareInventoryAvailabilityMultiplier => _hardwareInventoryAvailabilityMultiplier;

        // Hardware Review Score
        public float DefectImpactOnReviewScore => _defectImpactOnReviewScore;
        public float ReturnImpactOnReviewScore => _returnImpactOnReviewScore;

        // BOM Cost Multipliers
        public float BOMBudgetCostMultiplier       => _bomBudgetCostMultiplier;
        public float BOMStandardCostMultiplier     => _bomStandardCostMultiplier;
        public float BOMPremiumCostMultiplier      => _bomPremiumCostMultiplier;
        public float BOMExperimentalCostMultiplier => _bomExperimentalCostMultiplier;

        // BOM Defect Multipliers
        public float BOMBudgetDefectMultiplier       => _bomBudgetDefectMultiplier;
        public float BOMStandardDefectMultiplier     => _bomStandardDefectMultiplier;
        public float BOMPremiumDefectMultiplier      => _bomPremiumDefectMultiplier;
        public float BOMExperimentalDefectMultiplier => _bomExperimentalDefectMultiplier;

        // BOM Quality Bonuses
        public int BOMBudgetQualityBonus       => _bomBudgetQualityBonus;
        public int BOMStandardQualityBonus     => _bomStandardQualityBonus;
        public int BOMPremiumQualityBonus      => _bomPremiumQualityBonus;
        public int BOMExperimentalQualityBonus => _bomExperimentalQualityBonus;

        // Review Score
        public int   BaseReviewScore          => _baseReviewScore;
        public float BugImpactOnReviewScore   => _bugImpactOnReviewScore;
        public int   MinReviewScore           => _minReviewScore;
        public int   MaxReviewScore           => _maxReviewScore;

        // User Metrics
        public int   DefaultChurnBasisPoints           => _defaultChurnBasisPoints;
        public float ChurnReductionPerQualityPoint     => _churnReductionPerQualityPoint;
        public int   BaseNewUsersPerMonth              => _baseNewUsersPerMonth;
        public float UserGrowthMarketingMultiplier     => _userGrowthMarketingMultiplier;
        public int   InitialActiveUsersOnLaunch        => _initialActiveUsersOnLaunch;
        // Stored as major units in Inspector, returned as minor units (x100).
        public long  MarketingBudgetReferenceMinorUnits => (long)_marketingBudgetReferenceMajorUnits * 100L;
        public int   MinChurnBasisPoints               => _minChurnBasisPoints;
        public int   MaxChurnBasisPoints               => _maxChurnBasisPoints;

        // Misc
        public int   MaxBugsPerDayBase                    => _maxBugsPerDayBase;
        public float FeatureSatisfactionScopeMultiplier   => _featureSatisfactionScopeMultiplier;
        public int   BaseFeatureSatisfaction               => _baseFeatureSatisfaction;
        public int   BaseSupportTicketsPerHundredUsers     => _baseSupportTicketsPerHundredUsers;

        // ─── Contract — Board ─────────────────────────────────────────────────────

        [Header("Contract — Board")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of contracts placed on the board when a new session begins.")]
        private int _initialContractBoardSize = 5;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of new contracts added to the board on each monthly refresh.")]
        private int _monthlyContractRefreshCount = 3;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Number of days an Available contract remains on the board before expiring.")]
        private int _contractExpiryDays = 30;

        // ─── Contract — Payment Generation ───────────────────────────────────────

        [Header("Contract — Payment Generation")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum base payment for Easy contracts in major currency units (x100 = minor units). Prototype: 500 (£500).")]
        private int _contractBasePaymentMinMajorUnits = 500;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum base payment for Easy contracts in major currency units (x100 = minor units). Prototype: 2000 (£2,000).")]
        private int _contractBasePaymentMaxMajorUnits = 2000;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Payment multiplier applied to Standard difficulty contracts.")]
        private float _contractStandardPaymentMultiplier = 2.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Payment multiplier applied to Hard difficulty contracts.")]
        private float _contractHardPaymentMultiplier = 4.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Payment multiplier applied to Expert difficulty contracts.")]
        private float _contractExpertPaymentMultiplier = 8.0f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Bonus percentage added on top of base payment for an Excellent outcome. Prototype: 25.")]
        private int _contractExcellentBonusPercent = 25;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Percentage of base payment received as partial payment on a Failed outcome. Prototype: 10.")]
        private int _contractFailurePaymentPercent = 10;

        // ─── Contract — Deadline Generation ──────────────────────────────────────

        [Header("Contract — Deadline Generation")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base deadline duration in days for Easy contracts.")]
        private int _contractBaseDeadlineDays = 30;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Deadline multiplier applied to Standard difficulty contracts.")]
        private float _contractStandardDeadlineMultiplier = 1.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Deadline multiplier applied to Hard difficulty contracts.")]
        private float _contractHardDeadlineMultiplier = 2.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Deadline multiplier applied to Expert difficulty contracts.")]
        private float _contractExpertDeadlineMultiplier = 2.5f;

        // ─── Contract — Milestones ────────────────────────────────────────────────

        [Header("Contract — Milestones")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum number of milestones on a generated contract.")]
        private int _contractMinMilestones = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of milestones on a generated contract.")]
        private int _contractMaxMilestones = 5;

        // ─── Contract — Requirements ──────────────────────────────────────────────

        [Header("Contract — Requirements")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum number of required employee roles on a generated contract.")]
        private int _contractMinRequiredRoles = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of required employee roles on a generated contract.")]
        private int _contractMaxRequiredRoles = 3;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum number of required skill categories on a generated contract.")]
        private int _contractMinRequiredSkills = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of required skill categories on a generated contract.")]
        private int _contractMaxRequiredSkills = 3;

        // ─── Contract — Work ──────────────────────────────────────────────────────

        [Header("Contract — Work")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Base work hours required to complete an Easy contract.")]
        private int _contractBaseWorkHours = 80;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Work multiplier applied to Standard difficulty contracts.")]
        private float _contractStandardWorkMultiplier = 2.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Work multiplier applied to Hard difficulty contracts.")]
        private float _contractHardWorkMultiplier = 3.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Work multiplier applied to Expert difficulty contracts.")]
        private float _contractExpertWorkMultiplier = 5.0f;

        // ─── Contract — Quality Target ────────────────────────────────────────────

        [Header("Contract — Quality Target")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum quality target (0–100) assigned to a generated contract.")]
        private int _contractMinQualityTarget = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum quality target (0–100) assigned to a generated contract.")]
        private int _contractMaxQualityTarget = 90;

        // ─── Contract — Quality Scoring ───────────────────────────────────────────

        [Header("Contract — Quality Scoring")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight of role coverage in the quality score formula. See formula.contract.quality_score.")]
        private int _contractQualityRoleCoverageWeight = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight of skill fit in the quality score formula.")]
        private int _contractQualitySkillFitWeight = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight of time ratio in the quality score formula.")]
        private int _contractQualityTimeRatioWeight = 20;

        // ─── Contract — Outcome ───────────────────────────────────────────────────

        [Header("Contract — Outcome")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Quality score at or above which the outcome is Excellent.")]
        private int _qualityBonusThreshold = 80;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum quality score for an Accepted outcome. Below this → Failed.")]
        private int _contractMinQualityForAccepted = 40;

        // ─── Contract — Payout / Reputation ──────────────────────────────────────

        [Header("Contract — Payout / Reputation")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Late delivery penalty as a percentage of base payment. Deferred — 0 for MVP.")]
        private int _lateDeliveryPenaltyPercent = 0;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Reputation points gained on a successful (Accepted or Excellent) contract.")]
        private float _reputationGainPerContract = 5.0f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Reputation points lost on a Failed contract.")]
        private float _reputationLossPerFailedContract = 10.0f;

        // ─── IContractTuning Properties ───────────────────────────────────────────

        // Board
        public int InitialContractBoardSize     => _initialContractBoardSize;
        public int MonthlyContractRefreshCount  => _monthlyContractRefreshCount;
        public int ContractExpiryDays           => _contractExpiryDays;

        // Payment — stored as major units in Inspector, returned as minor units (x100)
        public long  ContractBasePaymentMinMinorUnits       => (long)_contractBasePaymentMinMajorUnits * 100L;
        public long  ContractBasePaymentMaxMinorUnits       => (long)_contractBasePaymentMaxMajorUnits * 100L;
        public float ContractStandardPaymentMultiplier      => _contractStandardPaymentMultiplier;
        public float ContractHardPaymentMultiplier          => _contractHardPaymentMultiplier;
        public float ContractExpertPaymentMultiplier        => _contractExpertPaymentMultiplier;
        public int   ContractExcellentBonusPercent          => _contractExcellentBonusPercent;
        public int   ContractFailurePaymentPercent          => _contractFailurePaymentPercent;

        // Deadline
        public int   ContractBaseDeadlineDays               => _contractBaseDeadlineDays;
        public float ContractStandardDeadlineMultiplier     => _contractStandardDeadlineMultiplier;
        public float ContractHardDeadlineMultiplier         => _contractHardDeadlineMultiplier;
        public float ContractExpertDeadlineMultiplier       => _contractExpertDeadlineMultiplier;

        // Milestones
        public int ContractMinMilestones  => _contractMinMilestones;
        public int ContractMaxMilestones  => _contractMaxMilestones;

        // Requirements
        public int ContractMinRequiredRoles   => _contractMinRequiredRoles;
        public int ContractMaxRequiredRoles   => _contractMaxRequiredRoles;
        public int ContractMinRequiredSkills  => _contractMinRequiredSkills;
        public int ContractMaxRequiredSkills  => _contractMaxRequiredSkills;

        // Work
        public int   ContractBaseWorkHours             => _contractBaseWorkHours;
        public float ContractStandardWorkMultiplier    => _contractStandardWorkMultiplier;
        public float ContractHardWorkMultiplier        => _contractHardWorkMultiplier;
        public float ContractExpertWorkMultiplier      => _contractExpertWorkMultiplier;

        // Quality Target
        public int ContractMinQualityTarget  => _contractMinQualityTarget;
        public int ContractMaxQualityTarget  => _contractMaxQualityTarget;

        // Quality Scoring
        public int ContractQualityRoleCoverageWeight  => _contractQualityRoleCoverageWeight;
        public int ContractQualitySkillFitWeight      => _contractQualitySkillFitWeight;
        public int ContractQualityTimeRatioWeight     => _contractQualityTimeRatioWeight;

        // Outcome
        public int QualityBonusThreshold         => _qualityBonusThreshold;
        public int ContractMinQualityForAccepted  => _contractMinQualityForAccepted;

        // Payout / Reputation
        public int   LateDeliveryPenaltyPercent        => _lateDeliveryPenaltyPercent;
        public float ReputationGainPerContract          => _reputationGainPerContract;
        public float ReputationLossPerFailedContract    => _reputationLossPerFailedContract;

        // ─── Finance — Runway ─────────────────────────────────────────────────────

        [Header("Finance — Runway")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Months of runway at or below which a low runway warning is surfaced. Does not interrupt Continue.")]
        private int _lowRunwayThresholdMonths = 6;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Months of runway at or below which a critical runway interruption fires and pauses Continue.")]
        private int _criticalRunwayThresholdMonths = 2;

        // ─── Finance — Hardware ───────────────────────────────────────────────────

        [Header("Finance — Hardware")]
        [SerializeField, Range(0f, 100f),
         Tooltip("[Placeholder] Percentage of hardware gross revenue retained by the retailer. Prototype: 15%.")]
        private float _hardwareRetailerMarginPercent = 15.0f;

        // ─── Finance — Expense Placeholders ──────────────────────────────────────

        [Header("Finance — Expense Placeholders")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly infrastructure cost per launched product in major currency units (x100 = minor units). Prototype: 500 (£500/month/product).")]
        private int _baseInfrastructureCostPerProductMajorUnits = 500;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Monthly manufacturing overhead cost per active hardware product in major currency units (x100 = minor units). Prototype: 2000 (£2,000/month/product).")]
        private int _baseManufacturingMonthlyCostMajorUnits = 2000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Dormant for MVP — research uses one-time cost only. Monthly research overhead cost per active research project in major currency units (x100 = minor units). Set to 0 to avoid double-charging alongside StartResearchUseCase one-time cost.")]
        private int _baseResearchMonthlyCostMajorUnits = 0;

        // ─── IFinanceTuning Properties ────────────────────────────────────────────

        public int   LowRunwayThresholdMonths      => _lowRunwayThresholdMonths;
        public int   CriticalRunwayThresholdMonths => _criticalRunwayThresholdMonths;
        public float HardwareRetailerMarginPercent => _hardwareRetailerMarginPercent;

        // Stored as major units in Inspector, returned as minor units (x100).
        public long BaseInfrastructureCostPerProductMinorUnits => (long)_baseInfrastructureCostPerProductMajorUnits * 100L;
        public long BaseManufacturingMonthlyCostMinorUnits     => (long)_baseManufacturingMonthlyCostMajorUnits * 100L;
        public long BaseResearchMonthlyCostMinorUnits          => (long)_baseResearchMonthlyCostMajorUnits * 100L;

        // ─── Research ─────────────────────────────────────────────────────────────

        [Header("Research")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] Abstract work units required per estimated day of research. Total required = EstimatedDurationDays * BaseResearchWorkUnitsPerDay, then divided by the combined multiplier. Prototype: 24.")]
        private int _baseResearchWorkUnitsPerDay = 24;

        [SerializeField, Min(0.1f),
         Tooltip("[Placeholder] Reduces required total work when the lead researcher has high expertise. Values > 1.0 speed up research. Applied as a divisor. Prototype: 1.2.")]
        private float _leadExpertiseMultiplier = 1.2f;

        [SerializeField, Min(0.1f),
         Tooltip("[Placeholder] Reduces required total work when tooling is advanced. Values > 1.0 speed up research. Applied as a divisor together with LeadExpertiseMultiplier. Prototype: 1.0.")]
        private float _toolingMultiplier = 1.0f;

        // ─── IResearchTuning Properties ───────────────────────────────────────────

        public int   BaseResearchWorkUnitsPerDay => _baseResearchWorkUnitsPerDay;
        public float LeadExpertiseMultiplier     => _leadExpertiseMultiplier;
        public float ToolingMultiplier           => _toolingMultiplier;

        // ─── Market — Demand ─────────────────────────────────────────────────────

        [Header("Market — Demand")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Default total demand assigned to each market category on initialization.")]
        private int _defaultDemand = 1000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Default annual growth rate for market categories in basis points (200 = 2% annual).")]
        private int _defaultGrowthRateBasisPoints = 200;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier on random volatility in weekly demand shift. Range: 0–1.")]
        private float _demandVolatilityRange = 0.5f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum percentage (positive or negative) random volatility each week.")]
        private int _weeklyDemandAdjustmentPercent = 2;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Floor for market category demand. Demand never drops below this value.")]
        private int _minDemandPerCategory = 100;

        // ─── Market — Trends ─────────────────────────────────────────────────────

        [Header("Market — Trends")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to trend strength when computing demand and preference effects.")]
        private float _trendImpactMultiplier = 1.0f;

        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Probability (0–100) that a new trend spawns each month if MaxActiveTrends is not reached.")]
        private int _trendSpawnChancePerMonthPercent = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum strength (inclusive) assigned to a newly generated trend.")]
        private int _trendMinStrength = 20;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum strength (inclusive) assigned to a newly generated trend.")]
        private int _trendMaxStrength = 80;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum duration in months (inclusive) for a newly generated trend.")]
        private int _trendMinDurationMonths = 3;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum duration in months (inclusive) for a newly generated trend.")]
        private int _trendMaxDurationMonths = 12;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum number of active trends allowed at any time.")]
        private int _maxActiveTrends = 3;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum number of market categories (inclusive) a new trend can affect.")]
        private int _trendMinAffectedCategories = 1;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Maximum number of market categories (inclusive) a new trend can affect.")]
        private int _trendMaxAffectedCategories = 3;

        // ─── Market — Drift ───────────────────────────────────────────────────────

        [Header("Market — Drift")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum preference points gained or lost per monthly drift step. Range: [1, N].")]
        private int _preferenceDriftRange = 5;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Points added to each expectation threshold per month. Clamped to 100.")]
        private int _expectationGrowthPerMonth = 1;

        // ─── Market — Initialization ─────────────────────────────────────────────

        [Header("Market — Initialization")]
        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Initial TechnologyExpectation for all market categories (0–100).")]
        private int _defaultTechnologyExpectation = 50;

        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Initial PriceSensitivity for all market categories (0–100).")]
        private int _defaultPriceSensitivity = 50;

        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Initial MarketingSensitivity for all market categories (0–100).")]
        private int _defaultMarketingSensitivity = 50;

        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Initial SupportExpectation for all market categories (0–100).")]
        private int _defaultSupportExpectation = 50;

        // ─── IMarketTuning Properties ─────────────────────────────────────────────

        // Demand
        public int   DefaultDemand                    => _defaultDemand;
        public int   DefaultGrowthRateBasisPoints     => _defaultGrowthRateBasisPoints;
        public float DemandVolatilityRange            => _demandVolatilityRange;
        public int   WeeklyDemandAdjustmentPercent    => _weeklyDemandAdjustmentPercent;
        public int   MinDemandPerCategory             => _minDemandPerCategory;

        // Trends
        public float TrendImpactMultiplier            => _trendImpactMultiplier;
        public int   TrendSpawnChancePerMonthPercent  => _trendSpawnChancePerMonthPercent;
        public int   TrendMinStrength                 => _trendMinStrength;
        public int   TrendMaxStrength                 => _trendMaxStrength;
        public int   TrendMinDurationMonths           => _trendMinDurationMonths;
        public int   TrendMaxDurationMonths           => _trendMaxDurationMonths;
        public int   MaxActiveTrends                  => _maxActiveTrends;
        public int   TrendMinAffectedCategories       => _trendMinAffectedCategories;
        public int   TrendMaxAffectedCategories       => _trendMaxAffectedCategories;

        // Drift
        public int   PreferenceDriftRange             => _preferenceDriftRange;
        public int   ExpectationGrowthPerMonth        => _expectationGrowthPerMonth;

        // Initialization
        public int   DefaultTechnologyExpectation     => _defaultTechnologyExpectation;
        public int   DefaultPriceSensitivity          => _defaultPriceSensitivity;
        public int   DefaultMarketingSensitivity      => _defaultMarketingSensitivity;
        public int   DefaultSupportExpectation        => _defaultSupportExpectation;

        // ─── Event / Crisis ───────────────────────────────────────────────────────

        [Header("Event — Scheduling")]
        [SerializeField, Min(1),
         Tooltip("[Placeholder] How many game days must elapse between random event checks. Prototype: 7.")]
        private int _eventCheckIntervalDays = 7;

        [SerializeField, Min(1),
         Tooltip("[Placeholder] Minimum game days between any two random events (global cooldown). Prototype: 14.")]
        private int _globalEventCooldownDays = 14;

        [Header("Event — Market Shock")]
        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Base probability (0–100) that a market shock fires per event check. Prototype: 10.")]
        private int _marketShockProbabilityPercent = 10;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum demand shift for a market shock, in basis points. Prototype: 500 (5%).")]
        private int _marketShockMinDemandShiftBasisPoints = 500;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum demand shift for a market shock, in basis points. Prototype: 2000 (20%).")]
        private int _marketShockMaxDemandShiftBasisPoints = 2000;

        [Header("Event — Team Morale Crisis")]
        [SerializeField, Range(0, 100),
         Tooltip("[Placeholder] Team morale at or below which a morale crisis fires (0–100). Prototype: 15.")]
        private int _teamMoraleCrisisThreshold = 15;

        [SerializeField,
         Tooltip("[Placeholder] Additional morale penalty applied in basis points when crisis fires. Prototype: -500 (-5 morale).")]
        private int _moraleCrisisPenaltyBasisPoints = -500;

        [Header("Event — Hardware Defect Spike")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Defect rate in basis points at or above which a defect spike fires. Prototype: 500 (5%).")]
        private int _hardwareDefectSpikeThresholdBasisPoints = 500;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Additional defect rate increase in basis points when spike fires. Prototype: 200 (+2%).")]
        private int _defectSpikeIncreaseBasisPoints = 200;

        // ─── IEventCrisisTuning Properties ───────────────────────────────────────

        public int EventCheckIntervalDays                  => _eventCheckIntervalDays;
        public int GlobalEventCooldownDays                 => _globalEventCooldownDays;
        public int MarketShockProbabilityPercent           => _marketShockProbabilityPercent;
        public int MarketShockMinDemandShiftBasisPoints    => _marketShockMinDemandShiftBasisPoints;
        public int MarketShockMaxDemandShiftBasisPoints    => _marketShockMaxDemandShiftBasisPoints;
        public int TeamMoraleCrisisThreshold               => _teamMoraleCrisisThreshold;
        public int MoraleCrisisPenaltyBasisPoints          => _moraleCrisisPenaltyBasisPoints;
        public int HardwareDefectSpikeThresholdBasisPoints => _hardwareDefectSpikeThresholdBasisPoints;
        public int DefectSpikeIncreaseBasisPoints          => _defectSpikeIncreaseBasisPoints;

        // ─── Competitor — Action Frequency ───────────────────────────────────────

        [Header("Competitor — Action Frequency")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Probability (0–100) that a competitor attempts an action in a given week.")]
        private int _competitorActionChancePerWeekPercent = 25;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Probability (0–100) that a competitor passes the monthly launch chance gate.")]
        private int _competitorLaunchChancePerMonthPercent = 15;

        // ─── Competitor — Price Changes ───────────────────────────────────────────

        [Header("Competitor — Price Changes")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum percentage (positive or negative) by which a competitor may change a product price.")]
        private int _competitorPriceChangeRangePercent = 10;

        // ─── Competitor — Marketing ───────────────────────────────────────────────

        [Header("Competitor — Marketing")]
        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Multiplier applied to competitor marketing effectiveness. Reserved for future use.")]
        private float _competitorMarketingEffectiveness = 1.0f;

        // ─── Competitor — Initialization ──────────────────────────────────────────

        [Header("Competitor — Initialization")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Number of AI competitors generated at session start.")]
        private int _initialCompetitorCount = 4;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Number of products each competitor starts with.")]
        private int _competitorInitialProductCount = 1;

        // ─── Competitor — Launch Quality / Pricing ────────────────────────────────

        [Header("Competitor — Launch Quality / Pricing")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum base quality (inclusive) rolled when a competitor launches a product. Range: 0–100.")]
        private int _competitorBaseQualityMin = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum base quality (inclusive) rolled when a competitor launches a product. Range: 0–100.")]
        private int _competitorBaseQualityMax = 70;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Minimum base launch price in major currency units. Converted to minor units (x100) at runtime. Prototype: 5000 (£5,000).")]
        private int _competitorBasePriceMinMajorUnits = 5000;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Maximum base launch price in major currency units. Converted to minor units (x100) at runtime. Prototype: 50000 (£50,000).")]
        private int _competitorBasePriceMaxMajorUnits = 50000;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Price multiplier applied when a competitor uses the Premium pricing style.")]
        private float _competitorPremiumPriceMultiplier = 1.5f;

        [SerializeField, Min(0f),
         Tooltip("[Placeholder] Price multiplier applied when a competitor uses the LowCost pricing style. AggressiveDiscounting uses this x0.8.")]
        private float _competitorLowCostPriceMultiplier = 0.6f;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Floor price in major currency units. Converted to minor units (x100). Prototype: 1000 (£1,000).")]
        private int _competitorMinPriceMajorUnits = 1000;

        // ─── Competitor — Market Share ─────────────────────────────────────────────

        [Header("Competitor — Market Share")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to quality score in the market share formula.")]
        private int _marketShareQualityWeight = 40;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to price score (100 - pricePercentile) in the market share formula.")]
        private int _marketSharePriceWeight = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Weight applied to marketing value in the market share formula.")]
        private int _marketShareMarketingWeight = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] CompetitiveIntensity points added per active product in a market category. Result clamped to [0, 100].")]
        private int _intensityPerProduct = 10;

        // ─── Competitor — Action Weights ──────────────────────────────────────────

        [Header("Competitor — Action Weights")]
        [SerializeField, Min(0),
         Tooltip("[Placeholder] Relative weight of the LaunchProduct action in the weighted action roll.")]
        private int _competitorLaunchChanceWeight = 50;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Relative weight of the ChangePrice action in the weighted action roll.")]
        private int _competitorPriceChangeChanceWeight = 30;

        [SerializeField, Min(0),
         Tooltip("[Placeholder] Relative weight of the EnterMarket action in the weighted action roll.")]
        private int _competitorExpandChanceWeight = 20;

        // ─── ICompetitorTuning Properties ─────────────────────────────────────────

        public int   CompetitorActionChancePerWeekPercent  => _competitorActionChancePerWeekPercent;
        public int   CompetitorLaunchChancePerMonthPercent => _competitorLaunchChancePerMonthPercent;
        public int   CompetitorPriceChangeRangePercent     => _competitorPriceChangeRangePercent;
        public float CompetitorMarketingEffectiveness      => _competitorMarketingEffectiveness;
        public int   InitialCompetitorCount                => _initialCompetitorCount;
        public int   CompetitorInitialProductCount         => _competitorInitialProductCount;
        public int   CompetitorBaseQualityMin              => _competitorBaseQualityMin;
        public int   CompetitorBaseQualityMax              => _competitorBaseQualityMax;
        public long  CompetitorBasePriceMinMinorUnits      => (long)_competitorBasePriceMinMajorUnits * 100L;
        public long  CompetitorBasePriceMaxMinorUnits      => (long)_competitorBasePriceMaxMajorUnits * 100L;
        public float CompetitorPremiumPriceMultiplier      => _competitorPremiumPriceMultiplier;
        public float CompetitorLowCostPriceMultiplier      => _competitorLowCostPriceMultiplier;
        public long  CompetitorMinPriceMinorUnits          => (long)_competitorMinPriceMajorUnits * 100L;
        public int   MarketShareQualityWeight              => _marketShareQualityWeight;
        public int   MarketSharePriceWeight                => _marketSharePriceWeight;
        public int   MarketShareMarketingWeight            => _marketShareMarketingWeight;
        public int   IntensityPerProduct                   => _intensityPerProduct;
        public int   CompetitorLaunchChanceWeight          => _competitorLaunchChanceWeight;
        public int   CompetitorPriceChangeChanceWeight     => _competitorPriceChangeChanceWeight;
        public int   CompetitorExpandChanceWeight          => _competitorExpandChanceWeight;

        // ─── Validation ───────────────────────────────────────────────────────────

        private void OnValidate()        {
            if (_defaultActionDelaySeconds < 0f)
            {
                _defaultActionDelaySeconds = 0f;
            }

            if (_maxGeneratedItems < 0)
            {
                _maxGeneratedItems = 0;
            }

            if (_uiRefreshIntervalSeconds < 0f)
            {
                _uiRefreshIntervalSeconds = 0f;
            }

            if (_autosaveIntervalSeconds < 0f)
            {
                _autosaveIntervalSeconds = 0f;
            }

            if (_maxSaveSlots < 1)
            {
                _maxSaveSlots = 1;
            }

            if (_defaultTickHours < 1)
            {
                _defaultTickHours = 1;
            }

            if (_maxTicksPerContinue < 1)
            {
                _maxTicksPerContinue = 1;
            }

            // Company — Starting Capital
            if (_garageCashMajorUnits < 0)          { _garageCashMajorUnits = 0; }
            if (_bootstrappedCashMajorUnits < 0)    { _bootstrappedCashMajorUnits = 0; }
            if (_seedFundedCashMajorUnits < 0)      { _seedFundedCashMajorUnits = 0; }
            if (_ventureStartCashMajorUnits < 0)    { _ventureStartCashMajorUnits = 0; }
            if (_sandboxDefaultCashMajorUnits < 0)  { _sandboxDefaultCashMajorUnits = 0; }

            // Company — Reputation
            if (_defaultReputation < 0)                   { _defaultReputation = 0; }
            if (_engineerReputationBonus < 0)             { _engineerReputationBonus = 0; }
            if (_productDesignerReputationBonus < 0)      { _productDesignerReputationBonus = 0; }
            if (_salesFounderReputationBonus < 0)         { _salesFounderReputationBonus = 0; }
            if (_hardwareSpecialistReputationBonus < 0)   { _hardwareSpecialistReputationBonus = 0; }
            if (_researchFounderReputationBonus < 0)      { _researchFounderReputationBonus = 0; }
            if (_serialFounderReputationBonus < 0)        { _serialFounderReputationBonus = 0; }
            if (_bootstrappedFounderReputationBonus < 0)  { _bootstrappedFounderReputationBonus = 0; }

            // Employee — Morale/Burnout
            if (_defaultEmployeeMorale < 0)              { _defaultEmployeeMorale = 0; }
            if (_defaultLoyalty < 0)                     { _defaultLoyalty = 0; }
            if (_moraleLowThreshold < 0)                 { _moraleLowThreshold = 0; }
            if (_burnoutHighThreshold < 0)               { _burnoutHighThreshold = 0; }
            if (_moraleDecayPerOverworkDay < 0f)         { _moraleDecayPerOverworkDay = 0f; }
            if (_burnoutAccumulationPerOverworkDay < 0f) { _burnoutAccumulationPerOverworkDay = 0f; }
            if (_burnoutRecoveryPerRestDay < 0f)         { _burnoutRecoveryPerRestDay = 0f; }
            if (_loyaltyGrowthPerMonth < 0f)             { _loyaltyGrowthPerMonth = 0f; }

            // Employee — Offer Acceptance
            if (_baseOfferAcceptancePercent < 0) { _baseOfferAcceptancePercent = 0; }
            if (_salaryWeightPercent < 0)        { _salaryWeightPercent = 0; }
            if (_reputationWeightPercent < 0)    { _reputationWeightPercent = 0; }
            if (_roleFitWeightPercent < 0)       { _roleFitWeightPercent = 0; }
            if (_growthWeightPercent < 0)        { _growthWeightPercent = 0; }

            // Employee — Candidate Generation
            if (_initialCandidatePoolSize < 1)   { _initialCandidatePoolSize = 1; }
            if (_candidatePoolRefreshCount < 1)  { _candidatePoolRefreshCount = 1; }
            if (_minSkillValue < 0)              { _minSkillValue = 0; }
            if (_maxSkillValue < 0)              { _maxSkillValue = 0; }
            if (_minPotential < 0)               { _minPotential = 0; }
            if (_maxPotential < 0)               { _maxPotential = 0; }
            if (_minCandidateConfidence < 0)     { _minCandidateConfidence = 0; }
            if (_maxCandidateConfidence < 0)     { _maxCandidateConfidence = 0; }
            if (_traitsPerCandidate < 1)         { _traitsPerCandidate = 1; }

            // Employee — Salary Ranges
            if (_juniorSalaryMinMonthlyMajorUnits < 0)    { _juniorSalaryMinMonthlyMajorUnits = 0; }
            if (_juniorSalaryMaxMonthlyMajorUnits < 0)    { _juniorSalaryMaxMonthlyMajorUnits = 0; }
            if (_midSalaryMinMonthlyMajorUnits < 0)       { _midSalaryMinMonthlyMajorUnits = 0; }
            if (_midSalaryMaxMonthlyMajorUnits < 0)       { _midSalaryMaxMonthlyMajorUnits = 0; }
            if (_seniorSalaryMinMonthlyMajorUnits < 0)    { _seniorSalaryMinMonthlyMajorUnits = 0; }
            if (_seniorSalaryMaxMonthlyMajorUnits < 0)    { _seniorSalaryMaxMonthlyMajorUnits = 0; }
            if (_leadSalaryMinMonthlyMajorUnits < 0)      { _leadSalaryMinMonthlyMajorUnits = 0; }
            if (_leadSalaryMaxMonthlyMajorUnits < 0)      { _leadSalaryMaxMonthlyMajorUnits = 0; }
            if (_principalSalaryMinMonthlyMajorUnits < 0) { _principalSalaryMinMonthlyMajorUnits = 0; }
            if (_principalSalaryMaxMonthlyMajorUnits < 0) { _principalSalaryMaxMonthlyMajorUnits = 0; }

            // Employee — Defaults
            if (_defaultAmbitionMin < 0) { _defaultAmbitionMin = 0; }
            if (_defaultAmbitionMax < 0) { _defaultAmbitionMax = 0; }

            // Product — Phase Duration
            if (_conceptPhaseBaseHours < 1)           { _conceptPhaseBaseHours = 1; }
            if (_developmentPhaseBaseHours < 1)       { _developmentPhaseBaseHours = 1; }
            if (_qaPhaseBaseHours < 1)                { _qaPhaseBaseHours = 1; }
            if (_launchPrepPhaseBaseHours < 1)        { _launchPrepPhaseBaseHours = 1; }
            if (_featureScopeComplexityMultiplier < 0f)  { _featureScopeComplexityMultiplier = 0f; }
            if (_qualityTargetComplexityMultiplier < 0f) { _qualityTargetComplexityMultiplier = 0f; }

            // Product — Review Score
            if (_baseReviewScore < 0)       { _baseReviewScore = 0; }
            if (_bugImpactOnReviewScore < 0f) { _bugImpactOnReviewScore = 0f; }
            if (_minReviewScore < 1)        { _minReviewScore = 1; }
            if (_maxReviewScore < 1)        { _maxReviewScore = 1; }
            if (_minReviewScore > _maxReviewScore) { _minReviewScore = _maxReviewScore; }

            // Product — User Metrics
            if (_defaultChurnBasisPoints < 0)       { _defaultChurnBasisPoints = 0; }
            if (_churnReductionPerQualityPoint < 0f) { _churnReductionPerQualityPoint = 0f; }
            if (_baseNewUsersPerMonth < 0)          { _baseNewUsersPerMonth = 0; }
            if (_userGrowthMarketingMultiplier < 0f) { _userGrowthMarketingMultiplier = 0f; }
            if (_initialActiveUsersOnLaunch < 0)    { _initialActiveUsersOnLaunch = 0; }
            if (_marketingBudgetReferenceMajorUnits < 0) { _marketingBudgetReferenceMajorUnits = 0; }
            if (_minChurnBasisPoints < 0)           { _minChurnBasisPoints = 0; }
            if (_maxChurnBasisPoints < 0)           { _maxChurnBasisPoints = 0; }
            if (_minChurnBasisPoints > _maxChurnBasisPoints) { _minChurnBasisPoints = _maxChurnBasisPoints; }

            // Product — Misc
            if (_maxBugsPerDayBase < 0)                 { _maxBugsPerDayBase = 0; }
            if (_featureSatisfactionScopeMultiplier < 0f) { _featureSatisfactionScopeMultiplier = 0f; }
            if (_baseFeatureSatisfaction < 0)           { _baseFeatureSatisfaction = 0; }
            if (_baseSupportTicketsPerHundredUsers < 0) { _baseSupportTicketsPerHundredUsers = 0; }

            // Product — Hardware Phase Duration
            if (_hardwareConceptPhaseBaseHours < 1)              { _hardwareConceptPhaseBaseHours = 1; }
            if (_hardwarePrototypePhaseBaseHours < 1)            { _hardwarePrototypePhaseBaseHours = 1; }
            if (_hardwareManufacturingPrepPhaseBaseHours < 1)    { _hardwareManufacturingPrepPhaseBaseHours = 1; }

            // Product — Hardware Manufacturing
            if (_baseManufacturingCostPerUnitMajorUnits < 0) { _baseManufacturingCostPerUnitMajorUnits = 0; }
            if (_baseDefectRateBasisPoints < 0)              { _baseDefectRateBasisPoints = 0; }
            if (_baseReturnRateBasisPoints < 0)              { _baseReturnRateBasisPoints = 0; }
            if (_warrantyCostPerDefectMajorUnits < 0)        { _warrantyCostPerDefectMajorUnits = 0; }
            if (_defaultComponentAvailability < 0)           { _defaultComponentAvailability = 0; }
            if (_defaultComponentAvailability > 100)         { _defaultComponentAvailability = 100; }

            // Product — Hardware Sales
            if (_baseHardwareUnitsSoldPerMonth < 0)              { _baseHardwareUnitsSoldPerMonth = 0; }
            if (_hardwareReviewScoreSalesMultiplier < 0f)        { _hardwareReviewScoreSalesMultiplier = 0f; }
            if (_hardwareInventoryAvailabilityMultiplier < 0f)   { _hardwareInventoryAvailabilityMultiplier = 0f; }

            // Product — Hardware Review Score
            if (_defectImpactOnReviewScore < 0f) { _defectImpactOnReviewScore = 0f; }
            if (_returnImpactOnReviewScore < 0f) { _returnImpactOnReviewScore = 0f; }

            // Product — BOM Cost Multipliers
            if (_bomBudgetCostMultiplier < 0f)       { _bomBudgetCostMultiplier = 0f; }
            if (_bomStandardCostMultiplier < 0f)     { _bomStandardCostMultiplier = 0f; }
            if (_bomPremiumCostMultiplier < 0f)      { _bomPremiumCostMultiplier = 0f; }
            if (_bomExperimentalCostMultiplier < 0f) { _bomExperimentalCostMultiplier = 0f; }

            // Product — BOM Defect Multipliers
            if (_bomBudgetDefectMultiplier < 0f)       { _bomBudgetDefectMultiplier = 0f; }
            if (_bomStandardDefectMultiplier < 0f)     { _bomStandardDefectMultiplier = 0f; }
            if (_bomPremiumDefectMultiplier < 0f)      { _bomPremiumDefectMultiplier = 0f; }
            if (_bomExperimentalDefectMultiplier < 0f) { _bomExperimentalDefectMultiplier = 0f; }

            // Contract — Board
            if (_initialContractBoardSize < 1)    { _initialContractBoardSize = 1; }
            if (_monthlyContractRefreshCount < 1) { _monthlyContractRefreshCount = 1; }
            if (_contractExpiryDays < 1)          { _contractExpiryDays = 1; }

            // Contract — Payment Generation
            if (_contractBasePaymentMinMajorUnits < 0) { _contractBasePaymentMinMajorUnits = 0; }
            if (_contractBasePaymentMaxMajorUnits < 0) { _contractBasePaymentMaxMajorUnits = 0; }
            if (_contractBasePaymentMinMajorUnits > _contractBasePaymentMaxMajorUnits) { _contractBasePaymentMinMajorUnits = _contractBasePaymentMaxMajorUnits; }
            if (_contractStandardPaymentMultiplier < 0f) { _contractStandardPaymentMultiplier = 0f; }
            if (_contractHardPaymentMultiplier < 0f)     { _contractHardPaymentMultiplier = 0f; }
            if (_contractExpertPaymentMultiplier < 0f)   { _contractExpertPaymentMultiplier = 0f; }
            if (_contractExcellentBonusPercent < 0)  { _contractExcellentBonusPercent = 0; }
            if (_contractFailurePaymentPercent < 0)  { _contractFailurePaymentPercent = 0; }

            // Contract — Deadline Generation
            if (_contractBaseDeadlineDays < 1)              { _contractBaseDeadlineDays = 1; }
            if (_contractStandardDeadlineMultiplier < 0f)   { _contractStandardDeadlineMultiplier = 0f; }
            if (_contractHardDeadlineMultiplier < 0f)       { _contractHardDeadlineMultiplier = 0f; }
            if (_contractExpertDeadlineMultiplier < 0f)     { _contractExpertDeadlineMultiplier = 0f; }

            // Contract — Milestones
            if (_contractMinMilestones < 1) { _contractMinMilestones = 1; }
            if (_contractMaxMilestones < 1) { _contractMaxMilestones = 1; }
            if (_contractMinMilestones > _contractMaxMilestones) { _contractMinMilestones = _contractMaxMilestones; }

            // Contract — Requirements
            if (_contractMinRequiredRoles < 1)  { _contractMinRequiredRoles = 1; }
            if (_contractMaxRequiredRoles < 1)  { _contractMaxRequiredRoles = 1; }
            if (_contractMinRequiredRoles > _contractMaxRequiredRoles) { _contractMinRequiredRoles = _contractMaxRequiredRoles; }
            if (_contractMinRequiredSkills < 1) { _contractMinRequiredSkills = 1; }
            if (_contractMaxRequiredSkills < 1) { _contractMaxRequiredSkills = 1; }
            if (_contractMinRequiredSkills > _contractMaxRequiredSkills) { _contractMinRequiredSkills = _contractMaxRequiredSkills; }

            // Contract — Work
            if (_contractBaseWorkHours < 1)          { _contractBaseWorkHours = 1; }
            if (_contractStandardWorkMultiplier < 0f) { _contractStandardWorkMultiplier = 0f; }
            if (_contractHardWorkMultiplier < 0f)     { _contractHardWorkMultiplier = 0f; }
            if (_contractExpertWorkMultiplier < 0f)   { _contractExpertWorkMultiplier = 0f; }

            // Contract — Quality Target
            if (_contractMinQualityTarget < 0)  { _contractMinQualityTarget = 0; }
            if (_contractMaxQualityTarget < 0)  { _contractMaxQualityTarget = 0; }
            if (_contractMinQualityTarget > _contractMaxQualityTarget) { _contractMinQualityTarget = _contractMaxQualityTarget; }

            // Contract — Quality Scoring
            if (_contractQualityRoleCoverageWeight < 0) { _contractQualityRoleCoverageWeight = 0; }
            if (_contractQualitySkillFitWeight < 0)     { _contractQualitySkillFitWeight = 0; }
            if (_contractQualityTimeRatioWeight < 0)    { _contractQualityTimeRatioWeight = 0; }

            // Contract — Outcome
            if (_qualityBonusThreshold < 0)         { _qualityBonusThreshold = 0; }
            if (_contractMinQualityForAccepted < 0)  { _contractMinQualityForAccepted = 0; }

            // Contract — Payout / Reputation
            if (_lateDeliveryPenaltyPercent < 0)       { _lateDeliveryPenaltyPercent = 0; }
            if (_reputationGainPerContract < 0f)        { _reputationGainPerContract = 0f; }
            if (_reputationLossPerFailedContract < 0f)  { _reputationLossPerFailedContract = 0f; }

            // Finance — Runway
            if (_lowRunwayThresholdMonths < 0)      { _lowRunwayThresholdMonths = 0; }
            if (_criticalRunwayThresholdMonths < 0)  { _criticalRunwayThresholdMonths = 0; }
            if (_criticalRunwayThresholdMonths > _lowRunwayThresholdMonths)
            {
                _criticalRunwayThresholdMonths = _lowRunwayThresholdMonths;
            }

            // Finance — Hardware
            if (_hardwareRetailerMarginPercent < 0f)   { _hardwareRetailerMarginPercent = 0f; }
            if (_hardwareRetailerMarginPercent > 100f)  { _hardwareRetailerMarginPercent = 100f; }

            // Finance — Expense Placeholders
            if (_baseInfrastructureCostPerProductMajorUnits < 0) { _baseInfrastructureCostPerProductMajorUnits = 0; }
            if (_baseManufacturingMonthlyCostMajorUnits < 0)     { _baseManufacturingMonthlyCostMajorUnits = 0; }
            if (_baseResearchMonthlyCostMajorUnits < 0)          { _baseResearchMonthlyCostMajorUnits = 0; }

            // Market — Demand
            if (_defaultDemand < 0)                        { _defaultDemand = 0; }
            if (_defaultGrowthRateBasisPoints < 0)         { _defaultGrowthRateBasisPoints = 0; }
            if (_defaultGrowthRateBasisPoints > 10000)     { _defaultGrowthRateBasisPoints = 10000; }
            if (_demandVolatilityRange < 0f)               { _demandVolatilityRange = 0f; }
            if (_weeklyDemandAdjustmentPercent < 0)        { _weeklyDemandAdjustmentPercent = 0; }
            if (_minDemandPerCategory < 0)                 { _minDemandPerCategory = 0; }

            // Market — Trends
            if (_trendImpactMultiplier < 0f)               { _trendImpactMultiplier = 0f; }
            if (_trendSpawnChancePerMonthPercent < 0)      { _trendSpawnChancePerMonthPercent = 0; }
            if (_trendSpawnChancePerMonthPercent > 100)    { _trendSpawnChancePerMonthPercent = 100; }
            if (_trendMinStrength < 0)                     { _trendMinStrength = 0; }
            if (_trendMaxStrength < 0)                     { _trendMaxStrength = 0; }
            if (_trendMinStrength > _trendMaxStrength)     { _trendMinStrength = _trendMaxStrength; }
            if (_trendMinDurationMonths < 1)               { _trendMinDurationMonths = 1; }
            if (_trendMaxDurationMonths < 1)               { _trendMaxDurationMonths = 1; }
            if (_trendMinDurationMonths > _trendMaxDurationMonths) { _trendMinDurationMonths = _trendMaxDurationMonths; }
            if (_maxActiveTrends < 0)                      { _maxActiveTrends = 0; }
            if (_trendMinAffectedCategories < 1)           { _trendMinAffectedCategories = 1; }
            if (_trendMaxAffectedCategories < 1)           { _trendMaxAffectedCategories = 1; }
            if (_trendMinAffectedCategories > _trendMaxAffectedCategories) { _trendMinAffectedCategories = _trendMaxAffectedCategories; }

            // Market — Drift
            if (_preferenceDriftRange < 0)                 { _preferenceDriftRange = 0; }
            if (_expectationGrowthPerMonth < 0)            { _expectationGrowthPerMonth = 0; }

            // Market — Initialization
            if (_defaultTechnologyExpectation < 0)  { _defaultTechnologyExpectation = 0; }
            if (_defaultTechnologyExpectation > 100) { _defaultTechnologyExpectation = 100; }
            if (_defaultPriceSensitivity < 0)        { _defaultPriceSensitivity = 0; }
            if (_defaultPriceSensitivity > 100)      { _defaultPriceSensitivity = 100; }
            if (_defaultMarketingSensitivity < 0)    { _defaultMarketingSensitivity = 0; }
            if (_defaultMarketingSensitivity > 100)  { _defaultMarketingSensitivity = 100; }
            if (_defaultSupportExpectation < 0)      { _defaultSupportExpectation = 0; }
            if (_defaultSupportExpectation > 100)    { _defaultSupportExpectation = 100; }

            // Research
            if (_baseResearchWorkUnitsPerDay < 1) { _baseResearchWorkUnitsPerDay = 1; }
            if (_leadExpertiseMultiplier < 0.1f)  { _leadExpertiseMultiplier = 0.1f; }
            if (_toolingMultiplier < 0.1f)        { _toolingMultiplier = 0.1f; }

            // Event — Scheduling
            if (_eventCheckIntervalDays < 1)  { _eventCheckIntervalDays = 1; }
            if (_globalEventCooldownDays < 1) { _globalEventCooldownDays = 1; }

            // Event — Market Shock
            if (_marketShockProbabilityPercent < 0)   { _marketShockProbabilityPercent = 0; }
            if (_marketShockProbabilityPercent > 100)  { _marketShockProbabilityPercent = 100; }
            if (_marketShockMinDemandShiftBasisPoints < 0) { _marketShockMinDemandShiftBasisPoints = 0; }
            if (_marketShockMaxDemandShiftBasisPoints < 0) { _marketShockMaxDemandShiftBasisPoints = 0; }
            if (_marketShockMinDemandShiftBasisPoints > _marketShockMaxDemandShiftBasisPoints)
            {
                _marketShockMinDemandShiftBasisPoints = _marketShockMaxDemandShiftBasisPoints;
            }

            // Event — Team Morale Crisis
            if (_teamMoraleCrisisThreshold < 0)  { _teamMoraleCrisisThreshold = 0; }
            if (_teamMoraleCrisisThreshold > 100) { _teamMoraleCrisisThreshold = 100; }

            // Event — Hardware Defect Spike
            if (_hardwareDefectSpikeThresholdBasisPoints < 0) { _hardwareDefectSpikeThresholdBasisPoints = 0; }
            if (_defectSpikeIncreaseBasisPoints < 0)          { _defectSpikeIncreaseBasisPoints = 0; }

            // Competitor — Action Frequency
            if (_competitorActionChancePerWeekPercent < 0)   { _competitorActionChancePerWeekPercent = 0; }
            if (_competitorActionChancePerWeekPercent > 100)  { _competitorActionChancePerWeekPercent = 100; }
            if (_competitorLaunchChancePerMonthPercent < 0)  { _competitorLaunchChancePerMonthPercent = 0; }
            if (_competitorLaunchChancePerMonthPercent > 100) { _competitorLaunchChancePerMonthPercent = 100; }

            // Competitor — Price Changes
            if (_competitorPriceChangeRangePercent < 0) { _competitorPriceChangeRangePercent = 0; }

            // Competitor — Marketing
            if (_competitorMarketingEffectiveness < 0f) { _competitorMarketingEffectiveness = 0f; }

            // Competitor — Initialization
            if (_initialCompetitorCount < 0)       { _initialCompetitorCount = 0; }
            if (_competitorInitialProductCount < 0) { _competitorInitialProductCount = 0; }

            // Competitor — Launch Quality / Pricing
            if (_competitorBaseQualityMin < 0)   { _competitorBaseQualityMin = 0; }
            if (_competitorBaseQualityMin > 100)  { _competitorBaseQualityMin = 100; }
            if (_competitorBaseQualityMax < 0)   { _competitorBaseQualityMax = 0; }
            if (_competitorBaseQualityMax > 100)  { _competitorBaseQualityMax = 100; }
            if (_competitorBaseQualityMin > _competitorBaseQualityMax) { _competitorBaseQualityMin = _competitorBaseQualityMax; }
            if (_competitorBasePriceMinMajorUnits < 0) { _competitorBasePriceMinMajorUnits = 0; }
            if (_competitorBasePriceMaxMajorUnits < 0) { _competitorBasePriceMaxMajorUnits = 0; }
            if (_competitorBasePriceMinMajorUnits > _competitorBasePriceMaxMajorUnits) { _competitorBasePriceMinMajorUnits = _competitorBasePriceMaxMajorUnits; }
            if (_competitorPremiumPriceMultiplier < 0f)  { _competitorPremiumPriceMultiplier = 0f; }
            if (_competitorLowCostPriceMultiplier < 0f)  { _competitorLowCostPriceMultiplier = 0f; }
            if (_competitorMinPriceMajorUnits < 0) { _competitorMinPriceMajorUnits = 0; }

            // Competitor — Market Share
            if (_marketShareQualityWeight < 0)   { _marketShareQualityWeight = 0; }
            if (_marketSharePriceWeight < 0)     { _marketSharePriceWeight = 0; }
            if (_marketShareMarketingWeight < 0) { _marketShareMarketingWeight = 0; }
            if (_intensityPerProduct < 0)        { _intensityPerProduct = 0; }

            // Competitor — Action Weights
            if (_competitorLaunchChanceWeight < 0)      { _competitorLaunchChanceWeight = 0; }
            if (_competitorPriceChangeChanceWeight < 0) { _competitorPriceChangeChanceWeight = 0; }
            if (_competitorExpandChanceWeight < 0)      { _competitorExpandChanceWeight = 0; }

            // Save — Slot Limits
            if (_maxAutosaveSlots < 1)   { _maxAutosaveSlots = 1; }
            if (_maxAutosaveSlots > 20)  { _maxAutosaveSlots = 20; }
            if (_maxManualSaveSlots < 1)  { _maxManualSaveSlots = 1; }
            if (_maxManualSaveSlots > 100) { _maxManualSaveSlots = 100; }
        }
    }
}
