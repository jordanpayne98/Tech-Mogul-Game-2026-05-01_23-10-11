namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for company creation.
    /// Consumed by CompanyService and CreateCompanyUseCase without depending on Infrastructure.
    /// All cash values are in minor currency units (e.g. pence for GBP).
    /// </summary>
    public interface ICompanyTuning
    {
        // ─── Starting Capital ─────────────────────────────────────────────────────

        /// <summary>Starting cash for the Garage capital preset in minor currency units.</summary>
        long GarageCashMinorUnits { get; }

        /// <summary>Starting cash for the Bootstrapped capital preset in minor currency units.</summary>
        long BootstrappedCashMinorUnits { get; }

        /// <summary>Starting cash for the Seed Funded capital preset in minor currency units.</summary>
        long SeedFundedCashMinorUnits { get; }

        /// <summary>Starting cash for the Venture Start capital preset in minor currency units.</summary>
        long VentureStartCashMinorUnits { get; }

        /// <summary>Starting cash for the Sandbox preset in minor currency units.</summary>
        long SandboxDefaultCashMinorUnits { get; }

        // ─── Reputation ───────────────────────────────────────────────────────────

        /// <summary>Base reputation score applied to all new companies before founder bonus.</summary>
        int DefaultReputation { get; }

        /// <summary>Additional reputation bonus for the Engineer founder background.</summary>
        int EngineerReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Product Designer founder background.</summary>
        int ProductDesignerReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Sales Founder background.</summary>
        int SalesFounderReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Hardware Specialist founder background.</summary>
        int HardwareSpecialistReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Research Founder background.</summary>
        int ResearchFounderReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Serial Founder background.</summary>
        int SerialFounderReputationBonus { get; }

        /// <summary>Additional reputation bonus for the Bootstrapped Founder background.</summary>
        int BootstrappedFounderReputationBonus { get; }
    }
}
