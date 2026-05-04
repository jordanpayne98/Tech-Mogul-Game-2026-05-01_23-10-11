using System.Collections.Generic;

namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Display data for the Contract Detail modal.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and passed to the contract detail modal view.
    /// All data is display-ready — no raw colours, pixel values, or VisualElement references.
    /// SemanticState drives USS state class toggling in the View.
    /// [Placeholder] — Phase 5 uses static data. CanAccept and CanAssignTeam are false in Phase 5.
    /// Core simulation wiring and outcome resolution deferred to Phase 6+ per Section 14 lock.
    /// </summary>
    public sealed class ContractDetailViewModel
    {
        // ── Core identity ────────────────────────────────────────────────────

        /// <summary>Stable contract identifier. Matches ContractRowViewModel.Id.</summary>
        public string ContractId { get; }

        /// <summary>Display name of the contracting client, e.g. "Apex Ventures".</summary>
        public string Client { get; }

        /// <summary>Human-readable contract type label, e.g. "Software Development".</summary>
        public string ContractType { get; }

        /// <summary>Difficulty tier label, e.g. "Standard", "Complex", "Expert".</summary>
        public string Difficulty { get; }

        /// <summary>Formatted deadline string, e.g. "Q3 Y2" or "8 weeks".</summary>
        public string Deadline { get; }

        /// <summary>Formatted payment string, e.g. "$120,000".</summary>
        public string Payment { get; }

        /// <summary>Quality target display string, e.g. "80% or higher".</summary>
        public string QualityTarget { get; }

        /// <summary>Current status label, e.g. "Available", "In Progress", "Completed".</summary>
        public string Status { get; }

        // ── Detail sections ──────────────────────────────────────────────────

        /// <summary>Brief description of the client and their context for this contract.</summary>
        public string ClientSummary { get; }

        /// <summary>Structured list of individual contract requirements with fit state.</summary>
        public IReadOnlyList<ContractRequirementViewModel> Requirements { get; }

        /// <summary>Plain-text summary of how the player's current team skills match contract needs.</summary>
        public string SkillFitSummary { get; }

        /// <summary>Ordered list of milestone display strings, e.g. "Milestone 1 — Initial Prototype".</summary>
        public IReadOnlyList<string> Milestones { get; }

        /// <summary>Ordered list of potential outcome descriptions shown as informational text.</summary>
        public IReadOnlyList<string> PotentialOutcomes { get; }

        /// <summary>Ordered list of stable route IDs for related reports, e.g. "report.contract_history".</summary>
        public IReadOnlyList<string> RelatedReports { get; }

        // ── Action state ─────────────────────────────────────────────────────

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>
        /// True when the player may accept this contract.
        /// Always false in Phase 5 per Section 14 lock. Phase 6+ wires core acceptance logic.
        /// </summary>
        public bool CanAccept { get; }

        /// <summary>
        /// True when the player may assign a team to this contract.
        /// Always false in Phase 5 per Section 14 lock. Phase 6+ wires core assignment logic.
        /// </summary>
        public bool CanAssignTeam { get; }

        public ContractDetailViewModel(
            string contractId,
            string client,
            string contractType,
            string difficulty,
            string deadline,
            string payment,
            string qualityTarget,
            string status,
            string clientSummary,
            IReadOnlyList<ContractRequirementViewModel> requirements,
            string skillFitSummary,
            IReadOnlyList<string> milestones,
            IReadOnlyList<string> potentialOutcomes,
            IReadOnlyList<string> relatedReports,
            string semanticState,
            bool canAccept,
            bool canAssignTeam)
        {
            ContractId       = contractId;
            Client           = client;
            ContractType     = contractType;
            Difficulty       = difficulty;
            Deadline         = deadline;
            Payment          = payment;
            QualityTarget    = qualityTarget;
            Status           = status;
            ClientSummary    = clientSummary;
            Requirements     = requirements;
            SkillFitSummary  = skillFitSummary;
            Milestones       = milestones;
            PotentialOutcomes = potentialOutcomes;
            RelatedReports   = relatedReports;
            SemanticState    = semanticState;
            CanAccept        = canAccept;
            CanAssignTeam    = canAssignTeam;
        }
    }
}
