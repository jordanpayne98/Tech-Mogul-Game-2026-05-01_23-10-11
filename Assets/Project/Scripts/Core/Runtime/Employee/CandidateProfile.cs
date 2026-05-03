using System.Collections.Generic;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Employee
{
    /// <summary>
    /// Per-save data describing a recruitment candidate's identity and visible attributes.
    /// Some fields represent player-visible estimates rather than true values.
    /// Mutable offer/interest state is stored in <see cref="CandidateRuntimeState"/>.
    /// </summary>
    public sealed class CandidateProfile
    {
        /// <summary>Stable unique identifier for this candidate record.</summary>
        public string Id;

        /// <summary>Display name of the candidate.</summary>
        public string Name;

        /// <summary>The role this candidate is applying for or best suited to.</summary>
        public EmployeeRole Role;

        /// <summary>The candidate's seniority level.</summary>
        public Seniority Seniority;

        /// <summary>The candidate's minimum salary expectation in minor currency units (e.g. cents).</summary>
        public long SalaryExpectationMinorUnits;

        /// <summary>
        /// Skill scores visible to the player during recruitment.
        /// Values are 0–100 integers per <see cref="SkillCategory"/>.
        /// These may differ from true values depending on assessment accuracy.
        /// </summary>
        public Dictionary<SkillCategory, int> VisibleSkills;

        /// <summary>Player-visible estimate of the candidate's current overall ability (0–100).</summary>
        public int CurrentAbilityEstimate;

        /// <summary>Lower bound of the candidate's hidden potential range (0–100).</summary>
        public int PotentialMin;

        /// <summary>Upper bound of the candidate's hidden potential range (0–100).</summary>
        public int PotentialMax;

        /// <summary>Personality and background traits visible to the player.</summary>
        public List<EmployeeTrait> Traits;

        /// <summary>The candidate's preferred working arrangement.</summary>
        public WorkPreference WorkPreference;

        /// <summary>The earliest in-game date this candidate is available to start.</summary>
        public GameDateTime AvailabilityDate;

        /// <summary>
        /// Confidence level in the visible skill and ability estimates (0–100).
        /// Higher values indicate more reliable assessments.
        /// </summary>
        public int ConfidenceLevel;
    }
}
