using System.Collections.Generic;
using System.Linq;
using Project.Core.Debugging;
using Project.Core.Results.Game;
using Project.Core.Runtime;

namespace Project.Application.Validation
{
    /// <summary>
    /// Application-layer validator that checks the structural integrity of a GameSessionState
    /// after initialisation. Returns a list of errors if any are found.
    /// Does not check gameplay correctness — only structural consistency.
    /// Defined in Plan 2N.
    /// </summary>
    public sealed class GameSessionValidator
    {
        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Validates all structural constraints on the given session state.
        /// </summary>
        /// <param name="session">The session state to validate.</param>
        /// <returns>A result indicating whether the session is valid and listing all errors.</returns>
        public GameSessionValidationResult Validate(GameSessionState session)
        {
            var errors = new List<string>();

            // 1. Session not null
            if (session == null)
            {
                errors.Add("Session is null.");
                return GameSessionValidationResult.Invalid(errors);
            }

            // 2. TimeState
            if (session.TimeState == null)
            {
                errors.Add("TimeState is null.");
            }
            else if (session.TimeState.CurrentDate == null)
            {
                errors.Add("TimeState.CurrentDate is null.");
            }

            // 3. CompanyState
            if (session.CompanyState == null)
            {
                errors.Add("CompanyState is null.");
            }
            else if (string.IsNullOrEmpty(session.CompanyState.CompanyId))
            {
                errors.Add("CompanyState.CompanyId is null or empty.");
            }

            // 4. FinanceState
            if (session.FinanceState == null)
            {
                errors.Add("FinanceState is null.");
            }
            else if (session.FinanceState.CashMinorUnits < 0)
            {
                errors.Add($"FinanceState.CashMinorUnits is negative ({session.FinanceState.CashMinorUnits}).");
            }

            // 5. Founder
            if (session.FounderProfile == null)
            {
                errors.Add("FounderProfile is null.");
            }

            // 6. RandomState
            if (session.RandomState == null)
            {
                errors.Add("RandomState is null.");
            }
            else if (session.RandomState.Seed == 0)
            {
                errors.Add("RandomState.Seed is 0 — seed must be non-zero for determinism.");
            }

            // 7. MarketStates
            if (session.MarketCategoryStates == null || session.MarketCategoryStates.Count < 1)
            {
                errors.Add("MarketCategoryStates is null or empty — market must be initialised.");
            }

            // 8. CompetitorStates
            if (session.CompetitorStates == null)
            {
                errors.Add("CompetitorStates is null.");
            }

            // 9. InboxState
            if (session.InboxState == null)
            {
                errors.Add("InboxState is null.");
            }

            // 10. Cross-reference consistency
            ValidateCrossReferences(session, errors);

            // 11. Duplicate IDs
            ValidateNoDuplicateIds(session, errors);

            if (errors.Count > 0)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[GameSessionValidator] Session invalid — {errors.Count} error(s) found.");

                foreach (string error in errors)
                {
                    DebugLogger.LogWarning(DebugCategory.Validation, $"  • {error}");
                }

                return GameSessionValidationResult.Invalid(errors);
            }

            DebugLogger.Log(DebugCategory.Validation, "[GameSessionValidator] Session is structurally valid.");
            return GameSessionValidationResult.Valid();
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        private static void ValidateCrossReferences(GameSessionState session, List<string> errors)
        {
            if (session.CompanyState == null || session.EmployeeStates == null || session.TeamStates == null ||
                session.ProductStates == null || session.ContractStates == null || session.ResearchProjectStates == null)
            {
                return;
            }

            // Employee ID cross-reference
            var employeeStateIds = new HashSet<string>(
                session.EmployeeStates.Where(e => e != null).Select(e => e.EmployeeId));

            foreach (string id in session.CompanyState.EmployeeIds ?? new List<string>())
            {
                if (!employeeStateIds.Contains(id))
                {
                    errors.Add($"CompanyState.EmployeeIds references unknown EmployeeId '{id}'.");
                }
            }

            // Team ID cross-reference
            var teamStateIds = new HashSet<string>(
                session.TeamStates.Where(t => t != null).Select(t => t.TeamId));

            foreach (string id in session.CompanyState.TeamIds ?? new List<string>())
            {
                if (!teamStateIds.Contains(id))
                {
                    errors.Add($"CompanyState.TeamIds references unknown TeamId '{id}'.");
                }
            }

            // Product ID cross-reference
            var productStateIds = new HashSet<string>(
                session.ProductStates.Where(p => p != null).Select(p => p.ProductId));

            foreach (string id in session.CompanyState.ProductIds ?? new List<string>())
            {
                if (!productStateIds.Contains(id))
                {
                    errors.Add($"CompanyState.ProductIds references unknown ProductId '{id}'.");
                }
            }

            // Contract ID cross-reference
            var contractStateIds = new HashSet<string>(
                session.ContractStates.Where(c => c != null).Select(c => c.ContractId));

            foreach (string id in session.CompanyState.ContractIds ?? new List<string>())
            {
                if (!contractStateIds.Contains(id))
                {
                    errors.Add($"CompanyState.ContractIds references unknown ContractId '{id}'.");
                }
            }

            // Research ID cross-reference
            if (session.ResearchState != null)
            {
                var researchStateIds = new HashSet<string>(
                    session.ResearchProjectStates.Where(r => r != null).Select(r => r.ProjectId));

                foreach (string id in session.ResearchState.ActiveProjectIds ?? new List<string>())
                {
                    if (!researchStateIds.Contains(id))
                    {
                        errors.Add($"ResearchState.ActiveProjectIds references unknown ProjectId '{id}'.");
                    }
                }
            }
        }

        private static void ValidateNoDuplicateIds(GameSessionState session, List<string> errors)
        {
            CheckForDuplicates("EmployeeState.EmployeeId",
                session.EmployeeStates?.Where(e => e != null).Select(e => e.EmployeeId), errors);

            CheckForDuplicates("TeamState.TeamId",
                session.TeamStates?.Where(t => t != null).Select(t => t.TeamId), errors);

            CheckForDuplicates("ProductState.ProductId",
                session.ProductStates?.Where(p => p != null).Select(p => p.ProductId), errors);

            CheckForDuplicates("ContractState.ContractId",
                session.ContractStates?.Where(c => c != null).Select(c => c.ContractId), errors);

            CheckForDuplicates("ResearchProjectState.ProjectId",
                session.ResearchProjectStates?.Where(r => r != null).Select(r => r.ProjectId), errors);
        }

        private static void CheckForDuplicates(string label, IEnumerable<string> ids, List<string> errors)
        {
            if (ids == null) return;

            var seen = new HashSet<string>();

            foreach (string id in ids)
            {
                if (!seen.Add(id))
                {
                    errors.Add($"Duplicate {label} found: '{id}'.");
                }
            }
        }
    }
}
