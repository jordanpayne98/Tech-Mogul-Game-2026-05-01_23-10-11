namespace Project.Core.Interfaces.Tuning
{
    /// <summary>
    /// Domain-specific tuning interface for the team management system.
    /// Consumed by TeamService and TeamProgressTickProcessor without depending on Infrastructure.
    /// All prototype values are [Placeholder] — update as GDD_05 values are confirmed.
    /// </summary>
    public interface ITeamTuning
    {
        // ─── Team — General ───────────────────────────────────────────────────────

        /// <summary>Base daily progress points a team produces before any multipliers. Prototype: 10.</summary>
        int BaseTeamCapacity { get; }

        /// <summary>Starting cohesion score for newly created teams. Prototype: 30.</summary>
        int DefaultCohesion { get; }

        /// <summary>Starting morale score for newly created teams. Prototype: 60.</summary>
        int DefaultMorale { get; }

        /// <summary>Maximum number of members allowed in a single team. Prototype: 8.</summary>
        int MaxTeamSize { get; }

        /// <summary>Minimum number of members required to create a team. Prototype: 1.</summary>
        int MinTeamSize { get; }

        // ─── Team — Progress ──────────────────────────────────────────────────────

        /// <summary>
        /// Progress multiplier applied when a Lead+ seniority member exists in the team.
        /// Prototype: 1.1.
        /// </summary>
        float LeadCoordinationBonusMultiplier { get; }

        /// <summary>Floor for daily progress output. Team always produces at least this many points. Prototype: 1.</summary>
        int MinDailyProgress { get; }

        // ─── Team — Cohesion ──────────────────────────────────────────────────────

        /// <summary>Cohesion points added per day while the team has an active assignment. Prototype: 1.</summary>
        float CohesionGrowthRatePerDay { get; }

        /// <summary>Cohesion points deducted when a member is added to or removed from a team. Prototype: 5.</summary>
        int CohesionMemberChangePenalty { get; }

        // ─── Team — Workload ──────────────────────────────────────────────────────

        /// <summary>
        /// Workload percentage above which an employee is considered overloaded.
        /// Binary workload model: assigned = 100, unassigned = 0.
        /// Overload is impossible in the MVP one-assignment model (100 &lt; 120).
        /// Code path exists for future multi-assignment support. Prototype: 120.
        /// </summary>
        float OverloadThresholdPercent { get; }
    }
}
