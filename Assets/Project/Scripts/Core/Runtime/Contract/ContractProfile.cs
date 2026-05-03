using System.Collections.Generic;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Employee;
using Project.Core.Runtime.Time;

namespace Project.Core.Runtime.Contract
{
    /// <summary>
    /// Per-save data describing a single contract available to or accepted by the player.
    /// Holds all static and structural information about the contract.
    /// Mutable runtime tracking is held separately in <see cref="ContractRuntimeState"/>.
    /// No reputation impact field — failed contracts affect payment only per GDD_12.
    /// Defined in GDD_12, Appendix A.6.
    /// </summary>
    public sealed class ContractProfile
    {
        // -------------------------------------------------------------------------
        // Identity
        // -------------------------------------------------------------------------

        /// <summary>Stable unique identifier for this contract instance. Never changes after creation.</summary>
        public string Id;

        /// <summary>Display name of the client who posted the contract.</summary>
        public string ClientName;

        // -------------------------------------------------------------------------
        // Classification
        // -------------------------------------------------------------------------

        /// <summary>The category of work this contract requires.</summary>
        public ContractType Type;

        /// <summary>The difficulty tier affecting required skill levels and payment range.</summary>
        public ContractDifficulty Difficulty;

        // -------------------------------------------------------------------------
        // Requirements
        // -------------------------------------------------------------------------

        /// <summary>Employee roles required to work on this contract.</summary>
        public List<EmployeeRole> RequiredRoles;

        /// <summary>Skill categories that must be covered by the assigned team.</summary>
        public List<SkillCategory> RequiredSkills;

        // -------------------------------------------------------------------------
        // Dates
        // -------------------------------------------------------------------------

        /// <summary>The in-game date this contract appeared in the marketplace.</summary>
        public GameDateTime PostedDate;

        /// <summary>The in-game date after which this contract expires if not accepted.</summary>
        public GameDateTime ExpiryDate;

        /// <summary>The in-game deadline by which the accepted contract must be completed.</summary>
        public GameDateTime Deadline;

        // -------------------------------------------------------------------------
        // Payment
        // -------------------------------------------------------------------------

        /// <summary>Standard payment in minor currency units on successful completion.</summary>
        public long BasePaymentMinorUnits;

        /// <summary>Additional bonus payment in minor currency units for an Excellent outcome.</summary>
        public long ExcellentBonusMinorUnits;

        /// <summary>
        /// Partial payment in minor currency units received on a Failed outcome.
        /// Zero indicates no payment on failure.
        /// </summary>
        public long FailurePaymentMinorUnits;

        // -------------------------------------------------------------------------
        // Quality and milestones
        // -------------------------------------------------------------------------

        /// <summary>Minimum quality score (0–100) required for a successful outcome.</summary>
        public int QualityTarget;

        /// <summary>Total number of milestones this contract is divided into.</summary>
        public int MilestoneCount;
    }
}
