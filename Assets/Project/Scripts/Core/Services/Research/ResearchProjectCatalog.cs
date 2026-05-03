using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Research;

namespace Project.Core.Services.Research
{
    /// <summary>
    /// Hardcoded static catalog of all research project definitions.
    /// Serves as the authoritative source for definition lookups at runtime.
    /// All entries are [Placeholder] — values can be tuned or moved to data-driven loading in a future plan.
    /// Not serialized. Not stored on GameSessionState. Pure Core static data.
    /// Defined in Plan 2K, GDD_13.
    /// </summary>
    public static class ResearchProjectCatalog
    {
        // ─── Catalog ──────────────────────────────────────────────────────────────

        private static readonly IReadOnlyList<ResearchProjectDefinition> AllProjects =
            new List<ResearchProjectDefinition>
            {
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.cloud_scaling",
                    Name               = "Cloud Auto-Scaling",
                    Track              = ResearchTrack.CloudInfrastructure,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Infrastructure },
                    EstimatedDurationDays = 30,
                    CostMinorUnits     = 500_000,
                    RiskLevel          = 20,
                    ObsolescenceRisk   = 10,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterInfrastructure,
                            TargetId    = "infra.auto_scaling",
                            Description = "Enables automatic cloud infrastructure scaling for launched products."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.automated_testing",
                    Name               = "Automated Testing Suite",
                    Track              = ResearchTrack.SoftwareEngineering,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Engineering, SkillCategory.QAReliability },
                    EstimatedDurationDays = 20,
                    CostMinorUnits     = 300_000,
                    RiskLevel          = 10,
                    ObsolescenceRisk   = 5,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterQA,
                            TargetId    = "qa.automated_testing",
                            Description = "Unlocks automated QA pipeline for software products."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.advanced_security",
                    Name               = "Advanced Security Framework",
                    Track              = ResearchTrack.Security,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Engineering },
                    EstimatedDurationDays = 25,
                    CostMinorUnits     = 400_000,
                    RiskLevel          = 30,
                    ObsolescenceRisk   = 15,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterSecurity,
                            TargetId    = "security.advanced_framework",
                            Description = "Enables advanced security hardening across all products."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.ai_code_assist",
                    Name               = "AI Code Assistant",
                    Track              = ResearchTrack.AIAndAutomation,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Engineering, SkillCategory.Research },
                    EstimatedDurationDays = 40,
                    CostMinorUnits     = 800_000,
                    RiskLevel          = 40,
                    ObsolescenceRisk   = 20,
                    PrerequisiteIds    = new List<string> { "research.automated_testing" },
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.NewFeature,
                            TargetId    = "feature.ai_code_assist",
                            Description = "Unlocks AI-assisted code review and generation features."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.hw_miniaturization",
                    Name               = "Hardware Miniaturization",
                    Track              = ResearchTrack.HardwareEngineering,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Hardware },
                    EstimatedDurationDays = 35,
                    CostMinorUnits     = 600_000,
                    RiskLevel          = 25,
                    ObsolescenceRisk   = 10,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterComponents,
                            TargetId    = "component.miniaturized",
                            Description = "Unlocks miniaturized components for hardware products."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.ux_design_system",
                    Name               = "UX Design System",
                    Track              = ResearchTrack.UXProductDesign,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Design, SkillCategory.ProductSense },
                    EstimatedDurationDays = 15,
                    CostMinorUnits     = 200_000,
                    RiskLevel          = 10,
                    ObsolescenceRisk   = 5,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.NewFeature,
                            TargetId    = "feature.design_system",
                            Description = "Unlocks a standardized UX design system for all product work."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.marketing_analytics",
                    Name               = "Marketing Analytics Platform",
                    Track              = ResearchTrack.MarketingAnalytics,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Marketing },
                    EstimatedDurationDays = 20,
                    CostMinorUnits     = 350_000,
                    RiskLevel          = 15,
                    ObsolescenceRisk   = 10,
                    PrerequisiteIds    = new List<string>(),
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterMarketingAnalytics,
                            TargetId    = "marketing.analytics_platform",
                            Description = "Unlocks a data-driven marketing analytics dashboard."
                        }
                    }
                },
                new ResearchProjectDefinition
                {
                    // [Placeholder]
                    Id                 = "research.advanced_manufacturing",
                    Name               = "Advanced Manufacturing",
                    Track              = ResearchTrack.Manufacturing,
                    RequiredSkills     = new List<SkillCategory> { SkillCategory.Hardware, SkillCategory.Operations },
                    EstimatedDurationDays = 30,
                    CostMinorUnits     = 700_000,
                    RiskLevel          = 30,
                    ObsolescenceRisk   = 10,
                    PrerequisiteIds    = new List<string> { "research.hw_miniaturization" },
                    Unlocks            = new List<ResearchUnlock>
                    {
                        new ResearchUnlock
                        {
                            Type        = ResearchUnlockType.BetterManufacturing,
                            TargetId    = "manufacturing.advanced",
                            Description = "Unlocks advanced manufacturing processes for hardware products."
                        }
                    }
                }
            };

        // ─── Lookup by ID ─────────────────────────────────────────────────────────

        private static readonly Dictionary<string, ResearchProjectDefinition> ById =
            BuildIndex();

        private static Dictionary<string, ResearchProjectDefinition> BuildIndex()
        {
            var index = new Dictionary<string, ResearchProjectDefinition>(AllProjects.Count);
            foreach (var definition in AllProjects)
            {
                index[definition.Id] = definition;
            }
            return index;
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the full catalog of all research project definitions.
        /// The returned list is read-only — do not cast or modify entries.
        /// </summary>
        public static IReadOnlyList<ResearchProjectDefinition> GetAllProjects()
        {
            return AllProjects;
        }

        /// <summary>
        /// Returns the definition for the given stable project ID, or null if not found.
        /// </summary>
        /// <param name="projectId">Stable ID of the research project (e.g. "research.cloud_scaling").</param>
        public static ResearchProjectDefinition GetProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return null;
            }

            ById.TryGetValue(projectId, out var definition);
            return definition;
        }
    }
}
