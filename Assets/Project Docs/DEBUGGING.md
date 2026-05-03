# Debugging Standard

## 1. Purpose

This document defines the centralized debug logging system for the project.

The project must use one consistent debug logging framework so Bezi does not scatter raw `Debug.Log()` calls across unrelated scripts.

The system must remain genre-neutral until the GDD defines actual systems.

---

## 2. Required Files

Bezi must create and maintain:

```text
Assets/Project/Scripts/Core/Debugging/DebugCategory.cs
Assets/Project/Scripts/Core/Debugging/DebugLogger.cs
Assets/Project/Scripts/Infrastructure/Debugging/DebugConfig.cs
Assets/Resources/DebugConfig.asset
/Pages/Private/Project Docs/Debugging Standard.md
```

If the final folder structure changes later, the same separation must be preserved:

```text
DebugCategory = shared enum
DebugLogger = logging API
DebugConfig = Unity ScriptableObject config
DebugConfig.asset = inspector-editable settings
DEBUGGING.md = human-readable instructions
```

---

## 3. DebugConfig Rule

`DebugConfig` must be a ScriptableObject that controls logging from one central inspector asset.

It must include:

- Master enable/disable switch
- Category-specific toggles
- Verbose category toggles where needed
- Inspector-friendly headers
- Safe loading from Resources
- Clear fallback behaviour if missing

The asset must be located at:

```text
Assets/Resources/DebugConfig.asset
```

The asset must be named exactly:

```text
DebugConfig.asset
```

---

## 4. DebugLogger Rule

`DebugLogger` must be the standard way to write development logs from project code.

Use:

```csharp
DebugLogger.Log(DebugCategory category, string message, Object context = null);
DebugLogger.LogWarning(DebugCategory category, string message, Object context = null);
DebugLogger.LogError(DebugCategory category, string message, Object context = null);
```

Do not scatter raw `Debug.Log()` calls throughout production code unless there is a specific reason.

Acceptable exceptions:

- Very early bootstrap errors before DebugLogger is available
- Temporary investigation logs that are removed before completion
- Unity/package-generated logs

---

## 5. Initial Debug Categories

Use only genre-neutral categories until the GDD defines actual systems.

Initial categories:

```csharp
public enum DebugCategory
{
    General,
    Bootstrap,
    Configuration,
    DataDefinitions,
    Validation,
    SaveLoad,
    UI,
    Navigation,
    Input,
    Audio,
    Visual,
    Performance,
    Testing
}
```

Do not add game-specific debug categories until the related GDD system exists.

Not allowed before GDD definition:

```text
Combat
Enemy
Player
Inventory
Quest
Cards
Buildings
Economy
Research
Dialogue
Crafting
Units
Levels
Missions
```

---

## 6. Debug Category Expansion Rule

When adding a new debug category, Bezi must update:

```text
DebugCategory.cs
DebugConfig.cs
DebugLogger.cs
DebugConfig.asset if needed
/Pages/Private/Project Docs/Debugging Standard.md
/Pages/Private/Project Docs/Changelog.md
```

A new debug category may be added only when:

- The related system exists
- The category is useful for filtering logs
- The name matches GDD terminology

Do not create categories speculatively.

---

## 7. DebugConfig Behaviour

`DebugConfig` must expose category toggles through public properties that respect the master switch.

Pattern:

```csharp
public bool LogUI => enableDebugLogs && logUI;
public bool LogNavigation => enableDebugLogs && logNavigation;
public bool LogSaveLoad => enableDebugLogs && logSaveLoad;
```

`ShouldLog()` must return false if:

```text
DebugConfig is missing
Master logging is disabled
The selected category is disabled
```

Broken setup should produce one clear warning, not repeated spam.

---

## 8. DebugLogger Formatting

Logs should have consistent formatting.

Recommended format:

```text
[Category] Message
```

Warnings:

```text
[Category Warning] Message
```

Errors:

```text
[Category Error] Message
```

Use Unity rich text colours in the Editor to make categories easier to scan.

Colour coding must remain simple and readable.

Do not use colour as the only way to communicate meaning.

---

## 9. Suggested Neutral Colour Groups

Use neutral colour groups only.

```text
General / Configuration: White or light grey
Bootstrap / SaveLoad: Blue
UI / Navigation: Cyan or blue
Validation / Warnings: Yellow
Errors: Red
Performance: Orange
Testing: Purple
Visual / Audio: Green or pink
```

Do not use category colour names tied to undefined game systems.

---

## 10. Debug Logging Usage Rules

Use debug logs for:

```text
Important initialization steps
Missing required setup
Invalid configuration
Save/load operations
UI routing failures
Validation failures
Unexpected state
Performance investigation
Temporary development diagnostics
```

Avoid logging:

```text
Every frame
Every UI refresh
Every minor value change
Large object dumps
Repeated warnings without cooldown
Sensitive user/system data
```

---

## 11. Expensive Log Rule

If building the log message is expensive, check the category first.

Use this pattern:

```csharp
if (DebugConfig.ShouldLog(DebugCategory.Performance))
{
    string message = BuildExpensiveDebugString();
    DebugLogger.Log(DebugCategory.Performance, message, this);
}
```

Do not build expensive strings if the category is disabled.

---

## 12. Context Rule

Where possible, pass a Unity context object.

Good:

```csharp
DebugLogger.Log(DebugCategory.UI, "Screen opened: screen.main_menu", this);
```

Less useful:

```csharp
DebugLogger.Log(DebugCategory.UI, "Screen opened: screen.main_menu");
```

Context makes Unity Console logs clickable.

---

## 13. Default Debug Settings

Default development settings:

```text
enableDebugLogs = true

High-level categories enabled:
General
Bootstrap
Configuration
Validation
SaveLoad
UI
Navigation

Verbose categories disabled by default:
Performance
Testing
Very detailed feature-specific categories added later
```

Production/build logging rules can be defined later.

Until then, debug logging should be development-focused.

---

## 14. No Debug Spam Rule

Debug logs must be useful.

Good:

```text
[SaveLoad] Save completed. Slot: slot_01
[Validation Warning] Action failed. Reason: Missing required selection.
[Navigation Error] Screen ID not registered: screen.example
```

Bad:

```text
Clicked.
Started.
Updated.
Loop ran.
Value changed.
```

---

## 15. Debug Logging and Architecture

Debug logging must not break decoupling.

The debug system may be used across layers, but it must not create gameplay dependencies.

Do not make core game rules depend on Unity scene objects just for logging.

Do not make UI depend on debug configuration for normal behaviour.

Debug logging must observe behaviour, not control gameplay behaviour.

---

## 16. Debug Tooling Future Expansion

The project may later add:

```text
Runtime debug overlay
Debug category toggle screen
Log-to-file support
Formula inspector
Save data inspector
State viewer
Performance counters
Simulation event viewer
```

Do not implement these until requested.

For now, implement only the centralized debug logging foundation unless the user asks for extra debugging tools.

---

## 17. Required Completion Checks

After implementing the debug logging foundation, Bezi must check:

```text
DebugCategory.cs exists
DebugLogger.cs exists
DebugConfig.cs exists
DebugConfig.asset exists at Assets/Resources/DebugConfig.asset
DebugLogger respects DebugConfig settings
Categories are genre-neutral
No Canvas/UGUI runtime UI was added
No game-specific assumptions were added
Project compiles
No new console errors
/Pages/Private/Project Docs/Debugging Standard.md was updated
/Pages/Private/Project Docs/Changelog.md was updated
```

---

## 18. Final Rule

Use the centralized debug logging system.

Do not scatter raw logs.

Do not create game-specific debug categories before the GDD defines those systems.

Keep debug behaviour documented and synchronized with implementation.
