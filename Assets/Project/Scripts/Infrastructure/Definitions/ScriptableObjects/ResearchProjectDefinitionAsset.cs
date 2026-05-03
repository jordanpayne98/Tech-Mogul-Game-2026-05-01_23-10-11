using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Definitions.Research;
using UnityEngine;

namespace Project.Infrastructure.Definitions.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject wrapper for a single ResearchProjectDefinition.
    /// Inspector-editable. Converted to a pure Core object at bootstrap via ToDefinition().
    /// Individual .asset files live in Assets/Project/Data/Definitions/Research/.
    ///
    /// Long currency note: Unity [SerializeField] does not support long.
    /// CostMajorUnits is stored as int in the Inspector.
    /// ToDefinition() converts it to CostMinorUnits = CostMajorUnits * 100L.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewResearchProjectDefinition",
        menuName = "Project/Definitions/Research Project Definition")]
    public sealed class ResearchProjectDefinitionAsset : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField,
         Tooltip("Stable ID for this research project. Must not change after data is persisted. " +
                 "Use format: research.snake_case_name (e.g. research.cloud_auto_scaling).")]
        private string _id;

        [SerializeField,
         Tooltip("Display name shown to the player. May change without breaking save data.")]
        private string _name;

        [Header("Classification")]
        [SerializeField,
         Tooltip("The research track this project belongs to.")]
        private ResearchTrack _track;

        [SerializeField,
         Tooltip("Skill categories that a team must cover to work effectively on this project.")]
        private List<SkillCategory> _requiredSkills = new List<SkillCategory>();

        [Header("Cost and Duration")]
        [SerializeField, Min(1),
         Tooltip("Estimated duration in game days to complete this project under normal conditions.")]
        private int _estimatedDurationDays = 30;

        [SerializeField, Min(0),
         Tooltip("Cost to initiate this research project in major currency units (e.g. dollars). " +
                 "Converted to minor units (x100) at runtime via ToDefinition().")]
        private int _costMajorUnits;

        [Header("Unlocks")]
        [SerializeField,
         Tooltip("Capabilities unlocked when this project reaches Completed status.")]
        private List<ResearchUnlock> _unlocks = new List<ResearchUnlock>();

        [Header("Risk")]
        [SerializeField, Range(0, 100),
         Tooltip("Designer-assigned risk level on a scale of 0–100. " +
                 "Higher values indicate greater chance of overrun, failure, or negative outcomes.")]
        private int _riskLevel;

        [SerializeField, Range(0, 100),
         Tooltip("Designer-assigned obsolescence risk on a scale of 0–100. " +
                 "Higher values indicate a greater chance this project becomes irrelevant due to future tech.")]
        private int _obsolescenceRisk;

        [Header("Prerequisites")]
        [SerializeField,
         Tooltip("Stable IDs of research projects that must be Completed before this project becomes Available.")]
        private List<string> _prerequisiteIds = new List<string>();

        // ─── Conversion ───────────────────────────────────────────────────────────

        /// <summary>
        /// Converts this ScriptableObject wrapper into a pure Core ResearchProjectDefinition.
        /// Called by DefinitionLoader at bootstrap. Must not be called at runtime after locking.
        /// </summary>
        public ResearchProjectDefinition ToDefinition()
        {
            return new ResearchProjectDefinition
            {
                Id                  = _id,
                Name                = _name,
                Track               = _track,
                RequiredSkills      = new List<SkillCategory>(_requiredSkills),
                EstimatedDurationDays = _estimatedDurationDays,
                // Stored as major units in Inspector; convert to minor units (cents equivalent).
                CostMinorUnits      = _costMajorUnits * 100L,
                Unlocks             = new List<ResearchUnlock>(_unlocks),
                RiskLevel           = _riskLevel,
                ObsolescenceRisk    = _obsolescenceRisk,
                PrerequisiteIds     = new List<string>(_prerequisiteIds)
            };
        }
    }
}
