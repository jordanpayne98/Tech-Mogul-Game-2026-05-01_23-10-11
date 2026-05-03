# Tuning Reference

## 1. Purpose

This document defines the project's centralized tuning configuration standard.

Tuning values are designer-facing adjustable values used by runtime systems, UI behaviour, validation, timing, thresholds, multipliers, probabilities, clamp rules, limits, cooldowns, and refresh intervals.

Bezi must not scatter tuning values across unrelated scripts.

---

## 2. Required Files

Bezi must create and maintain:

```text
Assets/Project/Scripts/Infrastructure/Tuning/TuningConfig.cs
Assets/Resources/TuningConfig.asset
/Pages/Private/Tuning/Tuning Registry.md
```

This document defines the tuning standard and entry format. Actual tuning value entries are recorded in `/Pages/Private/Tuning/Tuning Registry.md`.

The asset must be named exactly:

```text
TuningConfig.asset
```

The asset must be located at:

```text
Assets/Resources/TuningConfig.asset
```

---

## 3. What Belongs in TuningConfig

Use `TuningConfig` for values that are:

```text
Frequently adjusted during development
Designer-facing
Balance-related
Timing-related
Threshold-related
Multiplier-related
Probability-related
Clamp-related
UI-feel-related, if not part of theme styling
```

Allowed examples:

```text
Default duration values
Default threshold values
Default probability values
Default multiplier values
Default clamp values
Default limits
Default cooldown values
Default refresh intervals
Default animation timing values, if not handled by USS/theme
```

Use genre-neutral categories until the GDD defines actual systems.

Initial suggested sections:

```text
General
Timing
Validation
UIBehaviour
SaveLoad
Performance
Placeholder
```

Do not add game-specific tuning sections until those systems are defined in the GDD.

---

## 4. What Does Not Belong in TuningConfig

Do not use `TuningConfig` for:

```text
Static content definitions
Save data
Runtime state
UI visual theme tokens
Debug logging toggles
Scene references
Prefab references, unless explicitly approved
Large content tables
Player progress
Generated runtime data
```

Use the correct system instead:

```text
Debug toggles → DebugConfig
Formulas/calculation documentation → FORMULAS.md
Visual style tokens → USS/theme files
Persistent state → SaveData
Static content → DefinitionData
Runtime values → RuntimeState
```

---

## 5. TuningConfig ScriptableObject Rule

`TuningConfig` must be a ScriptableObject.

It must be:

```text
Inspector-editable
Grouped with clear headers
Documented with tooltips where useful
Safe to load from Resources
Validated where possible
Genre-neutral until systems are defined
```

Example structure:

```csharp
using UnityEngine;

[CreateAssetMenu(
    fileName = "TuningConfig",
    menuName = "Project/Tuning Config")]
public sealed class TuningConfig : ScriptableObject
{
    [Header("General")]
    [SerializeField, Min(0f)]
    private float defaultActionDelaySeconds = 0.25f;

    [Header("Validation")]
    [SerializeField, Min(0)]
    private int maxGeneratedItems = 100;

    public float DefaultActionDelaySeconds => defaultActionDelaySeconds;
    public int MaxGeneratedItems => maxGeneratedItems;
}
```

---

## 6. TuningConfig Access Rule

Access to tuning values must be deliberate.

Do not let unrelated code repeatedly call `Resources.Load<TuningConfig>()`.

Preferred pattern:

```text
Bootstrapper loads TuningConfig once
TuningConfig is passed to systems/controllers that need it
Systems read from TuningConfig through explicit dependency
```

Acceptable fallback:

```text
TuningConfigProvider
```

Only if the project architecture defines it.

Avoid:

```csharp
Resources.Load<TuningConfig>("TuningConfig")
```

being scattered throughout the codebase.

---

## 7. Formula Relationship

If a tuning value affects a formula, the formula must be documented in:

```text
/Pages/Private/Project Docs/Formula Reference.md
```

The formula entry must reference the tuning value.

Example:

```text
Uses tuning value:
- TuningConfig.DefaultActionDelaySeconds
```

If a tuning value changes mathematical behaviour, update:

```text
/Pages/Private/Formula/Formula Registry.md
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Changelog/Changelog.md
```

---

## 8. Tuning Entry Format

Every meaningful tuning value must have an entry using this format:

```markdown
## Tuning ID: tuning.example_value

### Name
Readable tuning name.

### Location
TuningConfig field/property name.

### Purpose
What this value controls.

### Type
float / int / bool / enum / curve / etc.

### Default Value
Current default value.

### Valid Range
Minimum and maximum.

### Used By
Files, systems, or formulas that read this value.

### Related Formula
Formula ID from FORMULAS.md, if relevant.

### Status
Locked / Placeholder / Pending GDD

### Notes
Edge cases, pending decisions, or design warnings.
```

---

## 9. Tuning ID Rule

Every meaningful tuning value must have a stable tuning ID.

Examples:

```text
tuning.default_action_delay
tuning.max_generated_items
tuning.validation_warning_duration
tuning.ui_refresh_interval
```

Do not use undefined game-specific IDs before the GDD defines systems.

---

## 10. Validation Rule

Tuning values must be constrained where possible.

Use:

```text
Min attributes
Range attributes
OnValidate checks
Clear warnings for invalid config
Clamp rules where appropriate
```

Invalid tuning values should not silently break the project.

Example:

```csharp
private void OnValidate()
{
    if (maxGeneratedItems < 0)
    {
        maxGeneratedItems = 0;
    }
}
```

---

## 11. Placeholder Tuning Rule

Placeholder tuning values are allowed only when needed for temporary implementation.

They must be clearly marked:

```text
[Placeholder]
[Temp]
[Mock]
```

Placeholder values must be documented in:

```text
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Changelog/Changelog.md
```

Do not treat placeholder tuning values as final design.

---

## 12. No Magic Numbers Rule

Avoid magic numbers in code.

Bad:

```csharp
if (value > 75)
{
    // ...
}
```

Good:

```csharp
if (value > _tuningConfig.ValidationThreshold)
{
    // ...
}
```

Small purely local technical constants are allowed when they are not design/balance values.

Example:

```csharp
private const int FirstIndex = 0;
```

---

## 13. Debugging Relationship

`TuningConfig` must stay separate from `DebugConfig`.

Use:

```text
TuningConfig = design/balance/timing values
DebugConfig = logging/filtering/development diagnostics
```

Do not mix debug logging toggles into `TuningConfig`.

Do not mix balance/tuning values into `DebugConfig`.

---

## 14. Required Completion Checks

After implementing the tuning config foundation, Bezi must check:

```text
TuningConfig.cs exists
TuningConfig.asset exists at Assets/Resources/TuningConfig.asset
/Pages/Private/Tuning/Tuning Registry.md exists
/Pages/Private/Changelog/Changelog.md was updated
/Pages/Private/Formula/Formula Registry.md was updated if any formula uses tuning values
No game-specific tuning categories were added
No Canvas/UGUI runtime UI was added
Project compiles
No new console errors
```

---

## 15. Final Rule

All important adjustable values must be centralized, documented, and easy to inspect.

Do not scatter tuning values across scripts.

Do not create game-specific tuning categories before the GDD defines those systems.

Do not let TUNING.md, FORMULAS.md, and code drift apart.
