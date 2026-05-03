using System.Collections.Generic;
using Project.Core.Definitions.Employee;

namespace Project.Core.Definitions.Research
{
    /// <summary>
    /// Static content definition for a single research project.
    /// These are designed-in game entries, not player-created data.
    /// Loaded from definition data; never modified at runtime.
    /// </summary>
    public sealed class ResearchProjectDefinition
    {
        /// <summary>
        /// Stable ID for this research project (e.g. "research.cloud_auto_scaling").
        /// Must not change after data is persisted.
        /// </summary>
        public string Id;

        /// <summary>Display name shown to the player. May change without breaking save data.</summary>
        public string Name;

        /// <summary>The research track this project belongs to.</summary>
        public ResearchTrack Track;

        /// <summary>
        /// Skill categories that a team must cover to work effectively on this project.
        /// Referenced from <see cref="Project.Core.Definitions.Employee.SkillCategory"/>.
        /// </summary>
        public List<SkillCategory> RequiredSkills;

        /// <summary>
        /// Estimated real-time game duration in days to complete this project under normal conditions.
        /// Actual duration may vary based on team and tuning.
        /// </summary>
        public int EstimatedDurationDays;

        /// <summary>
        /// Monetary cost to initiate this research project, expressed in minor currency units (e.g. cents).
        /// Stored as long to support large values without overflow.
        /// </summary>
        public long CostMinorUnits;

        /// <summary>Capabilities unlocked when this project reaches Completed status.</summary>
        public List<ResearchUnlock> Unlocks;

        /// <summary>
        /// Designer-assigned risk level for this project on a scale of 0–100.
        /// Higher values indicate greater chance of overrun, failure, or negative outcomes.
        /// </summary>
        public int RiskLevel;

        /// <summary>
        /// Designer-assigned obsolescence risk on a scale of 0–100.
        /// Higher values indicate a greater chance this project becomes irrelevant due to future tech.
        /// </summary>
        public int ObsolescenceRisk;

        /// <summary>
        /// Stable IDs of research projects that must be Completed before this project becomes Available.
        /// References other <see cref="Id"/> values.
        /// </summary>
        public List<string> PrerequisiteIds;
    }
}
