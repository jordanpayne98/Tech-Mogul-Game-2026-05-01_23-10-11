using System;
using Project.Core.Debugging;
using Project.Core.Definitions.Employee;
using Project.Core.Events.Employee;
using Project.Core.Interfaces;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Requests.Employee;
using Project.Core.Results.Employee;
using Project.Core.Runtime;
using Project.Core.Runtime.Employee;

namespace Project.Application.UseCases.Employee
{
    /// <summary>
    /// Application-layer use case for sending a hire offer to a candidate and resolving the outcome.
    /// In MVP, offer resolution is immediate — no negotiation phase.
    /// Acceptance converts the candidate to an employee in the same operation (Locked Decision #9).
    ///
    /// On rejection, the candidate's OfferStatus is set to Rejected. The profile and state
    /// remain in session lists for historical reference. The candidate cannot be re-offered.
    ///
    /// On acceptance, the candidate ID is removed from RecruitmentState.CandidateIds but
    /// the profile and state are retained for audit purposes.
    /// </summary>
    public sealed class SendOfferUseCase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeTuning  _tuning;
        private readonly IEventBus        _eventBus;

        public SendOfferUseCase(
            IEmployeeService employeeService,
            IEmployeeTuning  tuning,
            IEventBus        eventBus)
        {
            _employeeService = employeeService;
            _tuning          = tuning;
            _eventBus        = eventBus;
        }

        /// <summary>
        /// Validates the offer, evaluates acceptance probability, resolves outcome, and converts on acceptance.
        /// </summary>
        /// <param name="request">The hire offer request.</param>
        /// <param name="sessionState">Active session state to update.</param>
        /// <param name="random">Shared random instance.</param>
        public HireCandidateResult Execute(HireCandidateRequest request, GameSessionState sessionState, Random random)
        {
            // ── Validation ────────────────────────────────────────────────────────

            if (request == null)
            {
                return HireCandidateResult.Failed("Request is null.");
            }

            CandidateProfile candidateProfile = FindCandidateProfile(request.CandidateId, sessionState);
            if (candidateProfile == null)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[SendOfferUseCase] Candidate not found: {request.CandidateId}");
                return HireCandidateResult.Failed("Candidate not found.");
            }

            CandidateRuntimeState candidateState = FindCandidateState(request.CandidateId, sessionState);
            if (candidateState == null)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[SendOfferUseCase] Candidate state not found for: {request.CandidateId}");
                return HireCandidateResult.Failed("Candidate state not found.");
            }

            if (candidateState.OfferStatus != OfferStatus.None)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[SendOfferUseCase] Candidate already has an active offer: {request.CandidateId} " +
                    $"status={candidateState.OfferStatus}");
                return HireCandidateResult.Failed("Candidate already has an active offer.");
            }

            if (request.OfferedSalaryMinorUnits <= 0)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[SendOfferUseCase] Offered salary must be positive. candidateId={request.CandidateId}");
                return HireCandidateResult.Failed("Offered salary must be positive.");
            }

            // ── Offer evaluation ──────────────────────────────────────────────────

            int companyReputation = sessionState.CompanyState.Reputation;

            int acceptanceScore = _employeeService.EvaluateOfferAcceptance(
                candidateProfile,
                request.OfferedSalaryMinorUnits,
                companyReputation,
                _tuning);

            bool accepted = _employeeService.ResolveOffer(acceptanceScore, random);

            // ── Rejection path ────────────────────────────────────────────────────

            if (!accepted)
            {
                candidateState.OfferStatus = OfferStatus.Rejected;

                _eventBus.Publish(new CandidateOfferRespondedEvent(candidateProfile.Id, OfferStatus.Rejected));

                DebugLogger.Log(
                    DebugCategory.Simulation,
                    $"[SendOfferUseCase] Offer rejected: candidate={request.CandidateId} " +
                    $"acceptanceScore={acceptanceScore} offeredSalary={request.OfferedSalaryMinorUnits}");

                return HireCandidateResult.Failed("Candidate rejected the offer.");
            }

            // ── Acceptance path ───────────────────────────────────────────────────

            candidateState.OfferStatus = OfferStatus.Accepted;

            _eventBus.Publish(new CandidateOfferRespondedEvent(candidateProfile.Id, OfferStatus.Accepted));

            // Employee counter is 1-based from the current count.
            int nextEmployeeCounter = sessionState.EmployeeProfiles.Count + 1;

            EmployeeProfile employeeProfile = _employeeService.ConvertToEmployeeProfile(
                candidateProfile,
                sessionState.TimeState.CurrentDate,
                nextEmployeeCounter);

            EmployeeRuntimeState employeeState = _employeeService.CreateEmployeeState(
                candidateProfile,
                employeeProfile.Id,
                request.OfferedSalaryMinorUnits,
                _tuning,
                random);

            sessionState.EmployeeProfiles.Add(employeeProfile);
            sessionState.EmployeeStates.Add(employeeState);
            sessionState.CompanyState.EmployeeIds.Add(employeeProfile.Id);

            // Candidate exits the active pool but profile/state remain for historical reference.
            sessionState.RecruitmentState.CandidateIds.Remove(candidateProfile.Id);

            _eventBus.Publish(new EmployeeHiredEvent(employeeProfile.Id, sessionState.CompanyState.CompanyId));

            DebugLogger.Log(
                DebugCategory.Simulation,
                $"[SendOfferUseCase] Candidate hired: candidate={candidateProfile.Id} → employee={employeeProfile.Id} " +
                $"role={candidateProfile.Role} seniority={candidateProfile.Seniority} " +
                $"salary={request.OfferedSalaryMinorUnits} acceptanceScore={acceptanceScore}");

            return HireCandidateResult.Succeeded(employeeProfile.Id);
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private static CandidateProfile FindCandidateProfile(string candidateId, GameSessionState sessionState)
        {
            foreach (CandidateProfile profile in sessionState.CandidateProfiles)
            {
                if (profile.Id == candidateId)
                {
                    return profile;
                }
            }

            return null;
        }

        private static CandidateRuntimeState FindCandidateState(string candidateId, GameSessionState sessionState)
        {
            foreach (CandidateRuntimeState state in sessionState.CandidateStates)
            {
                if (state.CandidateId == candidateId)
                {
                    return state;
                }
            }

            return null;
        }
    }
}
