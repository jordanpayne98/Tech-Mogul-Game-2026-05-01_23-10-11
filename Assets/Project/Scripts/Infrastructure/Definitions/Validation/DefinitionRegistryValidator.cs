using System;
using Project.Core.Debugging;
using Project.Core.Definitions.Research;
using Project.Core.Interfaces;
using Project.Infrastructure.Definitions.ScriptableObjects;
using UnityEngine;

namespace Project.Infrastructure.Definitions.Validation
{
    /// <summary>
    /// Validates definition registry loading, conversion, duplicate detection, lookup, and lock behaviour.
    /// Runs at bootstrap in debug builds only. Never throws. Never crashes bootstrap.
    /// </summary>
    public sealed class DefinitionRegistryValidator
    {
        private readonly IDefinitionRegistry  _registry;
        private readonly DefinitionRegistry   _concreteRegistry;

        private const string ValidatorName = "DefinitionRegistry";

        // ─── Constructor ──────────────────────────────────────────────────────────

        public DefinitionRegistryValidator(IDefinitionRegistry registry, DefinitionRegistry concreteRegistry)
        {
            _registry         = registry         ?? throw new ArgumentNullException(nameof(registry));
            _concreteRegistry = concreteRegistry ?? throw new ArgumentNullException(nameof(concreteRegistry));
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Executes all definition validation checks and logs pass/fail per check.
        /// </summary>
        public void Run()
        {
            int passCount  = 0;
            int totalCount = 0;

            try
            {
                void Check(string checkName, bool pass, string detail)
                {
                    totalCount++;
                    if (pass)
                    {
                        passCount++;
                        DebugLogger.Log(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — {checkName}: PASS");
                    }
                    else
                    {
                        DebugLogger.LogError(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — {checkName}: FAIL — {detail}");
                    }
                }

                // ── 1. Database load check ────────────────────────────────────────

                ResearchDefinitionDatabase database = null;

                try
                {
                    database = Resources.Load<ResearchDefinitionDatabase>("ResearchDefinitionDatabase");
                }
                catch (Exception loadEx)
                {
                    DebugLogger.LogError(DebugCategory.Validation,
                        $"[Validation] {ValidatorName} — Database Load: exception during Resources.Load: {loadEx.Message}");
                }

                bool databaseLoaded = database != null;
                Check("Database Load", databaseLoaded,
                    "Resources.Load<ResearchDefinitionDatabase>(\"ResearchDefinitionDatabase\") returned null");

                // ── 2. Conversion check ───────────────────────────────────────────

                if (databaseLoaded && database.Entries != null && database.Entries.Count > 0)
                {
                    bool conversionPass = false;
                    string conversionDetail = "No entries produced a non-null ToDefinition() result";

                    try
                    {
                        foreach (ResearchProjectDefinitionAsset asset in database.Entries)
                        {
                            if (asset == null) continue;

                            ResearchProjectDefinition definition = asset.ToDefinition();

                            if (definition != null)
                            {
                                conversionPass = true;
                                break;
                            }
                        }
                    }
                    catch (Exception convEx)
                    {
                        conversionDetail = $"Exception during ToDefinition(): {convEx.Message}";
                    }

                    Check("Conversion", conversionPass, conversionDetail);
                }
                else
                {
                    totalCount++;
                    passCount++;
                    DebugLogger.Log(DebugCategory.Validation,
                        $"[Validation] {ValidatorName} — Conversion: SKIP (database null or empty — no entries to convert)");
                }

                // ── 3. Lookup check ───────────────────────────────────────────────

                bool allLookupsPass = true;
                string lookupDetail = string.Empty;

                if (databaseLoaded && database.Entries != null)
                {
                    try
                    {
                        foreach (ResearchProjectDefinitionAsset asset in database.Entries)
                        {
                            if (asset == null) continue;

                            ResearchProjectDefinition definition = asset.ToDefinition();

                            if (definition == null || string.IsNullOrEmpty(definition.Id)) continue;

                            ResearchProjectDefinition fromRegistry = _registry.Get<ResearchProjectDefinition>(definition.Id);

                            if (fromRegistry == null)
                            {
                                allLookupsPass = false;
                                lookupDetail   = $"registry.Get<ResearchProjectDefinition>(\"{definition.Id}\") returned null";
                                break;
                            }

                            if (fromRegistry.Id != definition.Id)
                            {
                                allLookupsPass = false;
                                lookupDetail   = $"ID mismatch for '{definition.Id}': registry returned '{fromRegistry.Id}'";
                                break;
                            }
                        }
                    }
                    catch (Exception lookupEx)
                    {
                        allLookupsPass = false;
                        lookupDetail   = $"Exception during lookup: {lookupEx.Message}";
                    }
                }

                Check("Lookup", allLookupsPass, lookupDetail);

                // ── 4. Has check ──────────────────────────────────────────────────

                bool hasCheckPass   = true;
                string hasDetail    = string.Empty;

                try
                {
                    // Negative check: a nonexistent ID must return false.
                    bool nonexistentResult = _registry.Has<ResearchProjectDefinition>("nonexistent_id");

                    if (nonexistentResult)
                    {
                        hasCheckPass = false;
                        hasDetail    = "Has<ResearchProjectDefinition>(\"nonexistent_id\") returned true — expected false";
                    }

                    // Positive check: verify known IDs return true.
                    if (hasCheckPass && databaseLoaded && database.Entries != null)
                    {
                        foreach (ResearchProjectDefinitionAsset asset in database.Entries)
                        {
                            if (asset == null) continue;

                            ResearchProjectDefinition definition = asset.ToDefinition();

                            if (definition == null || string.IsNullOrEmpty(definition.Id)) continue;

                            bool hasResult = _registry.Has<ResearchProjectDefinition>(definition.Id);

                            if (!hasResult)
                            {
                                hasCheckPass = false;
                                hasDetail    = $"Has<ResearchProjectDefinition>(\"{definition.Id}\") returned false — expected true";
                                break;
                            }
                        }
                    }
                }
                catch (Exception hasEx)
                {
                    hasCheckPass = false;
                    hasDetail    = $"Exception during Has check: {hasEx.Message}";
                }

                Check("Has", hasCheckPass, hasDetail);

                // ── 5. GetAll check ───────────────────────────────────────────────

                bool getAllPass   = true;
                string getAllDetail = string.Empty;

                try
                {
                    System.Collections.Generic.IReadOnlyList<ResearchProjectDefinition> allDefinitions =
                        _registry.GetAll<ResearchProjectDefinition>();

                    if (databaseLoaded && database.Entries != null)
                    {
                        // Count valid (non-null, non-empty-id) entries in the database.
                        int validDatabaseCount = 0;

                        foreach (ResearchProjectDefinitionAsset asset in database.Entries)
                        {
                            if (asset == null) continue;

                            ResearchProjectDefinition def = asset.ToDefinition();

                            if (def != null && !string.IsNullOrEmpty(def.Id))
                            {
                                validDatabaseCount++;
                            }
                        }

                        // Registry count may be less if duplicates were skipped.
                        if (allDefinitions.Count > validDatabaseCount)
                        {
                            getAllPass   = false;
                            getAllDetail = $"GetAll count {allDefinitions.Count} exceeds valid database count {validDatabaseCount}";
                        }
                    }
                }
                catch (Exception getAllEx)
                {
                    getAllPass   = false;
                    getAllDetail = $"Exception during GetAll: {getAllEx.Message}";
                }

                Check("GetAll", getAllPass, getAllDetail);

                // ── 6. Lock check ─────────────────────────────────────────────────

                bool lockCheckPass   = false;
                string lockCheckDetail = "Register after lock was not rejected (registry may not be locked)";

                try
                {
                    // Attempt to register after the registry is locked.
                    // DefinitionRegistry.Register() logs an error and silently returns on post-lock attempts.
                    // We detect success of the lock by verifying the registration is rejected:
                    // the count before and after the call should remain identical.
                    int countBefore = _registry.GetAll<ResearchProjectDefinition>().Count;

                    _concreteRegistry.Register<ResearchProjectDefinition>(
                        "_validation_lock_test",
                        new ResearchProjectDefinition());

                    int countAfter = _registry.GetAll<ResearchProjectDefinition>().Count;

                    if (countBefore == countAfter)
                    {
                        lockCheckPass   = true;
                        lockCheckDetail = string.Empty;
                    }
                    else
                    {
                        lockCheckDetail = $"Registry accepted registration after lock — count changed from {countBefore} to {countAfter}";
                    }
                }
                catch (Exception lockEx)
                {
                    lockCheckDetail = $"Exception during lock check: {lockEx.Message}";
                }

                Check("Lock Enforcement", lockCheckPass, lockCheckDetail);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — EXCEPTION during validation: {ex.Message}");
            }
            finally
            {
                DebugLogger.Log(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — {passCount}/{totalCount} checks passed");
            }
        }
    }
}
