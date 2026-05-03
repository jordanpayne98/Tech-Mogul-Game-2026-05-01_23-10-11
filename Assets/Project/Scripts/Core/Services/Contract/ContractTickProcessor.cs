using System;
using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Definitions.Contract;
using Project.Core.Definitions.Team;
using Project.Core.Definitions.Time;
using Project.Core.Events.Contract;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Time;
using Project.Core.Results.Time;
using Project.Core.Runtime;
using Project.Core.Runtime.Contract;
using Project.Core.Runtime.Time;

namespace Project.Core.Services.Contract
{
    /// <summary>
    /// Daily tick processor for the contract system.
    /// Runs at ProcessingOrder 500, after TeamProgressTickProcessor (Order 100) has written
    /// RawProgressPoints to AssignmentRuntimeState.
    ///
    /// Per daily boundary:
    ///   - Derives ProgressPercent from RawProgressPoints for each active contract.
    ///   - Updates milestone completion, quality score.
    ///   - Enforces deadlines (immediate failure on miss).
    ///   - Detects completion (progress >= 100) and determines outcome.
    ///   - Computes and stores PaymentDueMinorUnits on ContractRuntimeState.
    ///   - Expires Available contracts past their ExpiryDate.
    ///
    /// Per monthly boundary:
    ///   - Generates new contracts for the board.
    ///
    /// Does NOT unassign teams on completion/failure — that is the player's responsibility.
    /// Does NOT mutate FinanceRuntimeState or CompanyRuntimeState — that is Plan 2H.
    ///
    /// Defined in Plan 2G, GDD_12.
    /// </summary>
    public sealed class ContractTickProcessor : ITickProcessor
    {
        private readonly IContractService _contractService;
        private readonly IContractTuning  _tuning;
        private readonly IEventBus        _eventBus;
        private readonly GameSessionState _sessionState;
        private readonly Random           _random;

        public string ProcessorName   => "ContractTickProcessor";
        public int    ProcessingOrder => 500;

        public ContractTickProcessor(
            IContractService contractService,
            IContractTuning  tuning,
            IEventBus        eventBus,
            GameSessionState sessionState,
            Random           random)
        {
            _contractService = contractService;
            _tuning          = tuning;
            _eventBus        = eventBus;
            _sessionState    = sessionState;
            _random          = random;
        }

        /// <inheritdoc/>
        public TickResult ProcessTick(TickContext context)
        {
            var interruptions = new List<InterruptionRequest>();

            if (context.IsDayBoundary)
            {
                ProcessActiveContracts(context, interruptions);
                ProcessBoardExpiry(context);
            }

            if (context.IsMonthBoundary)
            {
                ProcessMonthlyBoardRefresh(context);
            }

            return interruptions.Count > 0
                ? TickResult.Succeeded(interruptions)
                : TickResult.Succeeded();
        }

        // ── Active contract processing ─────────────────────────────────────────────

        private void ProcessActiveContracts(TickContext context, List<InterruptionRequest> interruptions)
        {
            // Snapshot to avoid modification during iteration
            var activeStates = _sessionState.ContractStates
                .Where(s => s.Status == ContractStatus.Accepted || s.Status == ContractStatus.InProgress)
                .ToList();

            foreach (var contractState in activeStates)
            {
                var profile = _sessionState.ContractProfiles
                    .FirstOrDefault(p => p.Id == contractState.ContractId);

                if (profile == null)
                {
                    DebugLogger.LogWarning(DebugCategory.Simulation,
                        $"[ContractTickProcessor] No ContractProfile found for ContractId: {contractState.ContractId}. Skipping.");
                    continue;
                }

                // Find the matching assignment (team must be assigned to this contract)
                var assignment = _sessionState.AssignmentStates
                    .FirstOrDefault(a =>
                        a.TargetType == AssignmentTargetType.Contract
                        && a.TargetId == contractState.ContractId);

                if (assignment == null)
                {
                    // No team assigned yet — skip progress derivation but still check deadline
                    if (_contractService.IsDeadlineExceeded(profile, context.CurrentDate))
                    {
                        FailContract(contractState, profile, context.CurrentDate,
                            "Deadline exceeded", interruptions);
                    }

                    continue;
                }

                // Transition Accepted → InProgress on first tick with an assignment
                if (contractState.Status == ContractStatus.Accepted)
                {
                    contractState.Status = ContractStatus.InProgress;
                }

                // Derive progress
                int requiredPoints  = _contractService.ComputeContractRequiredPoints(profile, _tuning);
                int progressPercent = _contractService.DeriveProgressPercent(
                    assignment.RawProgressPoints, requiredPoints);

                contractState.ProgressPercent = progressPercent;

                // Milestone check
                int newMilestoneCount = _contractService.ComputeCompletedMilestones(
                    progressPercent, profile.MilestoneCount);

                for (int i = contractState.MilestonesCompleted; i < newMilestoneCount; i++)
                {
                    _eventBus.Publish(new ContractMilestoneCompletedEvent(contractState.ContractId, i));

                    DebugLogger.Log(DebugCategory.Simulation,
                        $"[ContractTickProcessor] Milestone {i} reached. ContractId: {contractState.ContractId}");
                }

                contractState.MilestonesCompleted = newMilestoneCount;

                // Quality update
                var teamMembers = GetTeamMembers(assignment.TeamId);

                int elapsedDays  = ComputeElapsedDays(contractState.AcceptedDate, context.CurrentDate);
                int deadlineDays = ComputeElapsedDays(contractState.AcceptedDate, profile.Deadline);

                contractState.QualityScore = _contractService.ComputeQualityScore(
                    profile, teamMembers, elapsedDays, deadlineDays, _tuning);

                // Deadline check — immediate failure
                if (_contractService.IsDeadlineExceeded(profile, context.CurrentDate))
                {
                    FailContract(contractState, profile, context.CurrentDate,
                        "Deadline exceeded", interruptions);
                    continue;
                }

                // Completion check
                if (progressPercent >= 100)
                {
                    CompleteContract(contractState, profile, context.CurrentDate, interruptions);
                }
            }
        }

        // ── Board expiry ──────────────────────────────────────────────────────────

        private void ProcessBoardExpiry(TickContext context)
        {
            var expiredIds = _contractService.GetExpiredContractIds(
                _sessionState.ContractProfiles,
                _sessionState.ContractStates,
                context.CurrentDate);

            foreach (var contractId in expiredIds)
            {
                var state = _sessionState.ContractStates
                    .FirstOrDefault(s => s.ContractId == contractId);

                if (state != null)
                {
                    state.Status = ContractStatus.Expired;
                }

                _sessionState.ContractBoardState.AvailableContractIds.Remove(contractId);

                DebugLogger.Log(DebugCategory.Simulation,
                    $"[ContractTickProcessor] Contract expired. ContractId: {contractId}");
            }
        }

        // ── Monthly board refresh ─────────────────────────────────────────────────

        private void ProcessMonthlyBoardRefresh(TickContext context)
        {
            var newProfiles = _contractService.GenerateContracts(
                _tuning.MonthlyContractRefreshCount,
                _tuning,
                context.CurrentDate,
                _random);

            foreach (var profile in newProfiles)
            {
                var state = _contractService.CreateContractRuntimeState(profile.Id);

                _sessionState.ContractProfiles.Add(profile);
                _sessionState.ContractStates.Add(state);
                _sessionState.ContractBoardState.AvailableContractIds.Add(profile.Id);
            }

            _sessionState.ContractBoardState.LastBoardRefreshDate = context.CurrentDate;

            _eventBus.Publish(new ContractBoardRefreshedEvent(newProfiles.Count, context.CurrentDate));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContractTickProcessor] Board refreshed. Added {newProfiles.Count} contracts. Date: {context.CurrentDate}");
        }

        // ── Terminal state helpers ────────────────────────────────────────────────

        private void FailContract(
            ContractRuntimeState     contractState,
            ContractProfile          profile,
            GameDateTime             currentDate,
            string                   reason,
            List<InterruptionRequest> interruptions)
        {
            contractState.Status               = ContractStatus.Failed;
            contractState.Outcome              = ContractOutcome.Failed;
            contractState.CompletedDate        = currentDate;
            contractState.PaymentDueMinorUnits = _contractService.ComputePaymentDue(
                profile, ContractOutcome.Failed, _tuning);

            float reputationChange = _contractService.ComputeReputationChange(ContractOutcome.Failed, _tuning);

            _eventBus.Publish(new ContractFailedEvent(
                contractState.ContractId,
                reason,
                contractState.PaymentDueMinorUnits,
                reputationChange));

            interruptions.Add(new InterruptionRequest(
                InterruptionType.ContractFailed,
                contractState.ContractId,
                $"Contract failed. Reason: {reason}. ContractId: {contractState.ContractId}"));

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContractTickProcessor] Contract failed. ContractId: {contractState.ContractId}, Reason: {reason}, Payment: {contractState.PaymentDueMinorUnits}");
        }

        private void CompleteContract(
            ContractRuntimeState     contractState,
            ContractProfile          profile,
            GameDateTime             currentDate,
            List<InterruptionRequest> interruptions)
        {
            ContractOutcome outcome = _contractService.DetermineOutcome(contractState.QualityScore, _tuning);

            contractState.Status               = ContractStatus.Completed;
            contractState.Outcome              = outcome;
            contractState.CompletedDate        = currentDate;
            contractState.PaymentDueMinorUnits = _contractService.ComputePaymentDue(
                profile, outcome, _tuning);

            float reputationChange = _contractService.ComputeReputationChange(outcome, _tuning);

            if (outcome == ContractOutcome.Failed)
            {
                // Quality too low — treat as failure
                _eventBus.Publish(new ContractFailedEvent(
                    contractState.ContractId,
                    "Quality below minimum threshold",
                    contractState.PaymentDueMinorUnits,
                    reputationChange));

                interruptions.Add(new InterruptionRequest(
                    InterruptionType.ContractFailed,
                    contractState.ContractId,
                    $"Contract failed due to low quality. ContractId: {contractState.ContractId}"));
            }
            else
            {
                _eventBus.Publish(new ContractCompletedEvent(
                    contractState.ContractId,
                    outcome,
                    contractState.PaymentDueMinorUnits,
                    reputationChange));

                // Only raise an interruption for Excellent outcomes — Accepted outcomes complete silently
                if (outcome == ContractOutcome.Excellent)
                {
                    interruptions.Add(new InterruptionRequest(
                        InterruptionType.ContractCompleted,
                        contractState.ContractId,
                        $"Contract completed with Excellent outcome. ContractId: {contractState.ContractId}"));
                }
            }

            DebugLogger.Log(DebugCategory.Simulation,
                $"[ContractTickProcessor] Contract completed. ContractId: {contractState.ContractId}, Outcome: {outcome}, Quality: {contractState.QualityScore}, Payment: {contractState.PaymentDueMinorUnits}");
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private List<Runtime.Employee.EmployeeRuntimeState> GetTeamMembers(string teamId)
        {
            var teamState = _sessionState.TeamStates
                .FirstOrDefault(t => t.TeamId == teamId);

            if (teamState == null || teamState.MemberIds == null)
            {
                return new List<Runtime.Employee.EmployeeRuntimeState>();
            }

            return _sessionState.EmployeeStates
                .Where(e => teamState.MemberIds.Contains(e.EmployeeId))
                .ToList();
        }

        private static int ComputeElapsedDays(GameDateTime from, GameDateTime to)
        {
            if (from == null || to == null)
            {
                return 0;
            }

            int elapsedHours = to.TotalElapsedHours - from.TotalElapsedHours;

            return Math.Max(0, elapsedHours / GameDateTime.HoursPerDay);
        }
    }
}
