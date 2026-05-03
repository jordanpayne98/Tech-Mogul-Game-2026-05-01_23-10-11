using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Team;

namespace Project.Core.Services.Team
{
    /// <summary>
    /// Static mapping class that returns the expected roles and skill categories
    /// for each AssignmentType. Used by TeamService to compute role coverage and skill fit.
    ///
    /// All mappings are [Placeholder] for MVP. Later plans may override these via
    /// target-entity data when domain plans (2E–2G, 2K) are implemented.
    /// </summary>
    public static class TeamAssignmentRequirementRules
    {
        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the employee roles relevant to the given assignment type.
        /// [Placeholder] — MVP static mappings. Override via target-entity data in future plans.
        /// </summary>
        public static List<EmployeeRole> GetRequiredRoles(AssignmentType assignmentType)
        {
            switch (assignmentType)
            {
                case AssignmentType.ProductResearch:
                    return new List<EmployeeRole> { EmployeeRole.Researcher };

                case AssignmentType.ProductConcept:
                    return new List<EmployeeRole> { EmployeeRole.ProductDesigner, EmployeeRole.ProductManager };

                case AssignmentType.ProductDesign:
                    return new List<EmployeeRole> { EmployeeRole.ProductDesigner };

                case AssignmentType.SoftwareDevelopment:
                    return new List<EmployeeRole> { EmployeeRole.SoftwareEngineer };

                case AssignmentType.HardwarePrototyping:
                    return new List<EmployeeRole> { EmployeeRole.HardwareEngineer };

                case AssignmentType.QATesting:
                    return new List<EmployeeRole> { EmployeeRole.QAEngineer };

                case AssignmentType.LaunchPreparation:
                    return new List<EmployeeRole> { EmployeeRole.MarketingSpecialist, EmployeeRole.ProductManager };

                case AssignmentType.PostLaunchSupport:
                    return new List<EmployeeRole> { EmployeeRole.SupportSpecialist };

                case AssignmentType.ProductUpdates:
                    return new List<EmployeeRole> { EmployeeRole.SoftwareEngineer };

                case AssignmentType.MarketingCampaign:
                    return new List<EmployeeRole> { EmployeeRole.MarketingSpecialist };

                case AssignmentType.InfrastructureScaling:
                    return new List<EmployeeRole> { EmployeeRole.InfrastructureEngineer };

                case AssignmentType.ContractWork:
                    return new List<EmployeeRole> { EmployeeRole.SoftwareEngineer };

                case AssignmentType.ResearchProject:
                    return new List<EmployeeRole> { EmployeeRole.Researcher };

                case AssignmentType.ManufacturingSetup:
                    return new List<EmployeeRole> { EmployeeRole.HardwareEngineer, EmployeeRole.OperationsSpecialist };

                case AssignmentType.CrisisResponse:
                    return new List<EmployeeRole> { EmployeeRole.SoftwareEngineer, EmployeeRole.InfrastructureEngineer };

                default:
                    return new List<EmployeeRole>();
            }
        }

        /// <summary>
        /// Returns the skill categories relevant to the given assignment type.
        /// [Placeholder] — MVP static mappings. Override via target-entity data in future plans.
        /// </summary>
        public static List<SkillCategory> GetRequiredSkills(AssignmentType assignmentType)
        {
            switch (assignmentType)
            {
                case AssignmentType.ProductResearch:
                    return new List<SkillCategory> { SkillCategory.Research };

                case AssignmentType.ProductConcept:
                    return new List<SkillCategory> { SkillCategory.ProductSense, SkillCategory.Design };

                case AssignmentType.ProductDesign:
                    return new List<SkillCategory> { SkillCategory.Design };

                case AssignmentType.SoftwareDevelopment:
                    return new List<SkillCategory> { SkillCategory.Engineering };

                case AssignmentType.HardwarePrototyping:
                    return new List<SkillCategory> { SkillCategory.Hardware };

                case AssignmentType.QATesting:
                    return new List<SkillCategory> { SkillCategory.QAReliability };

                case AssignmentType.LaunchPreparation:
                    return new List<SkillCategory> { SkillCategory.Marketing, SkillCategory.Operations };

                case AssignmentType.PostLaunchSupport:
                    return new List<SkillCategory> { SkillCategory.Support };

                case AssignmentType.ProductUpdates:
                    return new List<SkillCategory> { SkillCategory.Engineering };

                case AssignmentType.MarketingCampaign:
                    return new List<SkillCategory> { SkillCategory.Marketing };

                case AssignmentType.InfrastructureScaling:
                    return new List<SkillCategory> { SkillCategory.Infrastructure };

                case AssignmentType.ContractWork:
                    return new List<SkillCategory> { SkillCategory.Engineering };

                case AssignmentType.ResearchProject:
                    return new List<SkillCategory> { SkillCategory.Research };

                case AssignmentType.ManufacturingSetup:
                    return new List<SkillCategory> { SkillCategory.Hardware, SkillCategory.Operations };

                case AssignmentType.CrisisResponse:
                    return new List<SkillCategory> { SkillCategory.Engineering, SkillCategory.Infrastructure };

                default:
                    return new List<SkillCategory>();
            }
        }
    }
}
