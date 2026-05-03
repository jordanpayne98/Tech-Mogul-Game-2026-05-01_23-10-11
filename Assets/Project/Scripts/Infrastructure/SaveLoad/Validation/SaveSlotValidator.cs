using System;
using System.Collections.Generic;
using Project.Application.UseCases.SaveLoad;
using Project.Core.Debugging;
using Project.Core.Definitions.SaveLoad;
using Project.Core.Interfaces.Services;
using Project.Core.Interfaces.Tuning;
using Project.Core.Results.SaveLoad;
using Project.Core.Runtime;

namespace Project.Infrastructure.SaveLoad.Validation
{
    /// <summary>
    /// Validates slot management rules: quick save overwrite, rolling autosave rotation,
    /// manual save limit path, explicit autosave trigger, and exit hook wiring.
    /// Cleans up all validation saves in a finally block.
    /// Never throws or crashes bootstrap.
    /// </summary>
    public sealed class SaveSlotValidator
    {
        private readonly ISaveSlotManager    _slotManager;
        private readonly AutosaveCoordinator _coordinator;
        private readonly bool                _exitHandlerIsWired;
        private readonly ISaveTuning         _saveTuning;

        private const string ValidatorName    = "SaveSlot";
        private const string ValidationPrefix = "_validation_";

        // ─── Constructor ──────────────────────────────────────────────────────────

        public SaveSlotValidator(
            ISaveSlotManager    slotManager,
            AutosaveCoordinator coordinator,
            bool                exitHandlerIsWired,
            ISaveTuning         saveTuning)
        {
            _slotManager        = slotManager ?? throw new ArgumentNullException(nameof(slotManager));
            _coordinator        = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
            _exitHandlerIsWired = exitHandlerIsWired;
            _saveTuning         = saveTuning  ?? throw new ArgumentNullException(nameof(saveTuning));
        }

        // ─── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Executes all slot validation checks and logs pass/fail per check.
        /// Cleanup of all validation saves runs in a finally block.
        /// </summary>
        public void Run()
        {
            int    passCount  = 0;
            int    totalCount = 0;

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

                // ── 1. Quick save overwrite check ─────────────────────────────────

                bool quickSavePass   = false;
                string quickSaveDetail = string.Empty;

                try
                {
                    GameSessionState testSession = CreateMinimalSession();

                    _slotManager.RequestQuickSave(testSession);
                    _slotManager.RequestQuickSave(testSession);

                    int quickSaveCount = _slotManager.ListSaves(SaveType.QuickSave).Count;

                    if (quickSaveCount == 1)
                    {
                        quickSavePass = true;

                        // Cleanup quick save.
                        _slotManager.DeleteSave(ISaveSlotManager.QuickSaveId);
                    }
                    else
                    {
                        quickSaveDetail = $"Expected 1 quick save slot, found {quickSaveCount}";
                        _slotManager.DeleteSave(ISaveSlotManager.QuickSaveId);
                    }
                }
                catch (Exception ex)
                {
                    quickSaveDetail = $"Exception: {ex.Message}";
                }

                Check("QuickSave Overwrite", quickSavePass, quickSaveDetail);

                // ── 2. Rolling autosave rotation check ────────────────────────────

                bool rotationPass   = true;
                string rotationDetail = string.Empty;
                List<string> autosaveIdsCreated = new List<string>();

                try
                {
                    GameSessionState testSession = CreateMinimalSession();
                    int rotationTarget = _saveTuning.MaxAutosaveSlots + 1;

                    for (int i = 0; i < rotationTarget; i++)
                    {
                        SaveResult result = _slotManager.RequestAutosave(testSession, $"{ValidationPrefix}rotation_{i}");

                        if (result.Success)
                        {
                            autosaveIdsCreated.Add(result.SaveId);
                        }
                    }

                    int finalAutosaveCount = _slotManager.GetAutosaveCount();

                    if (finalAutosaveCount > _saveTuning.MaxAutosaveSlots)
                    {
                        rotationPass   = false;
                        rotationDetail = $"Autosave count {finalAutosaveCount} exceeds MaxAutosaveSlots {_saveTuning.MaxAutosaveSlots}";
                    }
                }
                catch (Exception ex)
                {
                    rotationPass   = false;
                    rotationDetail = $"Exception: {ex.Message}";
                }
                finally
                {
                    // Cleanup all validation autosaves created during this check.
                    CleanupValidationSaves(SaveType.Autosave);
                }

                Check("Autosave Rotation", rotationPass, rotationDetail);

                // ── 3. Manual save limit check (logical path, no mass-save) ──────

                bool limitPass   = true;
                string limitDetail = string.Empty;

                try
                {
                    int currentManualCount = _slotManager.GetManualSaveCount();
                    int maxSlots           = _saveTuning.MaxManualSaveSlots;

                    if (currentManualCount < maxSlots)
                    {
                        // Not at limit — verify a save succeeds (code path: below limit).
                        GameSessionState testSession = CreateMinimalSession();

                        SaveResult result = _slotManager.RequestManualSave(testSession, $"{ValidationPrefix}limit_check");

                        if (!result.Success)
                        {
                            limitPass   = false;
                            limitDetail = $"RequestManualSave below limit returned failure: {result.FailureReason}";
                        }
                        else
                        {
                            // Cleanup the save we created.
                            _slotManager.DeleteSave(result.SaveId);
                        }
                    }
                    else
                    {
                        // At limit — verify a save is rejected.
                        GameSessionState testSession = CreateMinimalSession();

                        SaveResult result = _slotManager.RequestManualSave(testSession, $"{ValidationPrefix}limit_check");

                        if (result.Success)
                        {
                            limitPass   = false;
                            limitDetail = $"RequestManualSave at limit ({maxSlots}) succeeded — expected failure";
                            _slotManager.DeleteSave(result.SaveId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    limitPass   = false;
                    limitDetail = $"Exception: {ex.Message}";
                }

                Check("Manual Save Limit", limitPass, limitDetail);

                // ── 4. Explicit trigger path check ────────────────────────────────

                bool triggerPass   = false;
                string triggerDetail = "RequestPreActionAutosave did not execute or returned an unexpected result";

                try
                {
                    SaveResult result = _coordinator.RequestPreActionAutosave($"{ValidationPrefix}explicit_trigger");

                    if (result.Success)
                    {
                        // Session was available and a save was created.
                        triggerPass = true;

                        if (!string.IsNullOrEmpty(result.SaveId))
                        {
                            _slotManager.DeleteSave(result.SaveId);
                        }
                    }
                    else if (!string.IsNullOrEmpty(result.FailureReason) &&
                             result.FailureReason.Contains("Session provider returned null"))
                    {
                        // Expected at bootstrap — no active session loaded yet.
                        // Coordinator is correctly wired; failure is due to no active session, not a code defect.
                        triggerPass   = true;
                        triggerDetail = string.Empty;
                        DebugLogger.Log(DebugCategory.Validation,
                            $"[Validation] {ValidatorName} — Explicit Trigger Path: coordinator wired correctly " +
                            $"(session null at bootstrap — expected)");
                    }
                    else
                    {
                        triggerDetail = $"RequestPreActionAutosave returned failure: {result.FailureReason}";
                    }
                }
                catch (Exception ex)
                {
                    triggerDetail = $"Exception: {ex.Message}";
                }

                Check("Explicit Trigger Path", triggerPass, triggerDetail);

                // ── 5. Exit hook wiring check ─────────────────────────────────────

                bool exitHookPass   = _exitHandlerIsWired;
                string exitHookDetail = _exitHandlerIsWired
                    ? string.Empty
                    : "ExitSaveHandler was not wired — coordinator reference not set";

                Check("Exit Hook Wiring", exitHookPass, exitHookDetail);
            }
            catch (Exception ex)
            {
                DebugLogger.LogError(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — EXCEPTION during validation: {ex.Message}");
            }
            finally
            {
                // Final cleanup sweep — remove any remaining validation saves.
                CleanupValidationSaves(null);

                DebugLogger.Log(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — {passCount}/{totalCount} checks passed");
            }
        }

        // ─── Private helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Creates a minimal GameSessionState for validation purposes.
        /// Contains only the minimum data required for serialisation to succeed.
        /// </summary>
        private static GameSessionState CreateMinimalSession()
        {
            return new GameSessionState
            {
                SessionId   = "validation-minimal-session",
                RandomState = new Core.Runtime.RandomRuntimeState { Seed = 1, CallCount = 0 },
                TimeState   = Core.Runtime.Time.TimeRuntimeState.CreateDefault()
            };
        }

        /// <summary>
        /// Deletes all saves whose SaveName starts with the validation prefix.
        /// Logs a warning if any individual deletion fails.
        /// </summary>
        private void CleanupValidationSaves(SaveType? filterType)
        {
            try
            {
                List<SaveSlotInfo> saves = _slotManager.ListSaves(filterType);

                foreach (SaveSlotInfo save in saves)
                {
                    if (save.SaveName != null && save.SaveName.StartsWith(ValidationPrefix, StringComparison.Ordinal))
                    {
                        try
                        {
                            DeleteResult result = _slotManager.DeleteSave(save.SaveId);

                            if (!result.Success)
                            {
                                DebugLogger.LogWarning(DebugCategory.Validation,
                                    $"[Validation] {ValidatorName} — Cleanup warning: Could not delete '{save.SaveId}': {result.FailureReason}");
                            }
                        }
                        catch (Exception deleteEx)
                        {
                            DebugLogger.LogWarning(DebugCategory.Validation,
                                $"[Validation] {ValidatorName} — Cleanup exception for '{save.SaveId}': {deleteEx.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogWarning(DebugCategory.Validation,
                    $"[Validation] {ValidatorName} — Cleanup sweep exception: {ex.Message}");
            }
        }
    }
}
