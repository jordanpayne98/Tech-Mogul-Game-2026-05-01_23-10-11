namespace Project.Presentation.UI.Screens.Contracts
{
    /// <summary>
    /// Display data for one contract table row in the Contracts screen list.
    /// Immutable after construction. No Unity dependencies.
    /// Created by ContractsController and collected in ContractsViewModel.Rows.
    /// All string fields are display-ready — no raw numbers, colours, or VisualElement references.
    /// SemanticState drives USS state class toggling in the View.
    /// [Placeholder] — Phase 5 uses static data. Core simulation wiring deferred to Phase 6+.
    /// </summary>
    public sealed class ContractRowViewModel
    {
        /// <summary>Stable contract identifier. Used to identify the selected row and open the detail modal.</summary>
        public string Id { get; }

        /// <summary>Display name of the contracting client, e.g. "Apex Ventures".</summary>
        public string Client { get; }

        /// <summary>Human-readable contract type label, e.g. "Software Development".</summary>
        public string ContractType { get; }

        /// <summary>Summary of required skills for the column, e.g. "Backend, QA".</summary>
        public string RequiredSkills { get; }

        /// <summary>Difficulty tier label, e.g. "Standard", "Complex", "Expert".</summary>
        public string Difficulty { get; }

        /// <summary>Formatted deadline string, e.g. "Q3 Y2" or "8 weeks".</summary>
        public string Deadline { get; }

        /// <summary>Formatted payment string, e.g. "$120,000".</summary>
        public string Payment { get; }

        /// <summary>Progress display string for active contracts, e.g. "60%" or "—" when not started.</summary>
        public string Progress { get; }

        /// <summary>Name of the team currently assigned to this contract, or "—" if unassigned.</summary>
        public string AssignedTeam { get; }

        /// <summary>Quality target display string, e.g. "80% or higher".</summary>
        public string QualityTarget { get; }

        /// <summary>Current status label shown in the Status column, e.g. "Available", "In Progress", "Completed".</summary>
        public string Status { get; }

        /// <summary>
        /// Semantic visual state for USS class toggling.
        /// Accepted values: "normal", "warning", "danger", "success", "muted".
        /// </summary>
        public string SemanticState { get; }

        /// <summary>True when clicking this row should open the contract detail modal.</summary>
        public bool IsClickable { get; }

        public ContractRowViewModel(
            string id,
            string client,
            string contractType,
            string requiredSkills,
            string difficulty,
            string deadline,
            string payment,
            string progress,
            string assignedTeam,
            string qualityTarget,
            string status,
            string semanticState,
            bool isClickable)
        {
            Id            = id;
            Client        = client;
            ContractType  = contractType;
            RequiredSkills = requiredSkills;
            Difficulty    = difficulty;
            Deadline      = deadline;
            Payment       = payment;
            Progress      = progress;
            AssignedTeam  = assignedTeam;
            QualityTarget = qualityTarget;
            Status        = status;
            SemanticState = semanticState;
            IsClickable   = isClickable;
        }
    }
}
