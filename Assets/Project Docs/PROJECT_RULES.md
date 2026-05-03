# Project Rules

## 1. Purpose

This document defines the mandatory project rules Bezi must follow when working on this project.

The project is being designed from the ground up through a GDD-first process. Bezi must not invent game systems, mechanics, UI screens, rules, progression, stats, balancing, art direction, or architecture that has not been explicitly defined.

These rules are mandatory, not optional guidance.

---

## 2. Required Reference Documents

Before creating a plan or implementing work, Bezi must follow these documents:

1. `/Pages/Private/Project Docs/Project Rules.md`
2. `/Pages/Private/Project Docs/Coding Standard.md`
3. `/Pages/Private/Project Docs/UI Architecture.md`
4. `/Pages/Private/Project Docs/Colour Palette.md`
5. `/Pages/Private/Project Docs/Typography.md`
6. `/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md`
7. `/Pages/Private/Project Docs/Debugging Standard.md`
8. `/Pages/Private/Project Docs/Tuning Reference.md`
9. `/Pages/Private/Project Docs/Formula Reference.md`, if formulas/calculations are involved
10. `Assets/Project Docs/Tech_Mogul_Complete_GDD_v0_1.md` (master GDD)
11. Relevant GDD page or implementation page for the current task
12. Current user prompt

If a prompt conflicts with these documents, Bezi must mention the conflict before proceeding.

If a GDD page conflicts with these documents, Bezi must mention the conflict before proceeding.

---

## 3. Source of Truth Order

Use this priority order:

1. Current user prompt
2. Current GDD / implementation page (`Assets/Project Docs/Tech_Mogul_Complete_GDD_v0_1.md`)
3. `/Pages/Private/Project Docs/Project Rules.md`
4. `/Pages/Private/Project Docs/Coding Standard.md`
5. `/Pages/Private/Project Docs/UI Architecture.md`
6. `/Pages/Private/Project Docs/Colour Palette.md`
7. `/Pages/Private/Project Docs/Typography.md`
8. `/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md`
9. Existing project files
10. Temporary technical assumptions only when unavoidable

Bezi must not silently choose between conflicting sources.

---

## 4. No Carry-Over From Other Projects

This is a fresh project.

Do not use assumptions, mechanics, architecture, UI direction, or systems from any previous project.

Do not assume:

- Genre
- Player role
- Core loop
- Game systems
- Economy
- Time system
- Save structure
- Input style
- Art direction beyond the locked UI rules
- UI screen list
- Progression
- Multiplayer
- Platform target
- Data model
- Simulation style

Only use what has been explicitly defined in this project.

---

## 5. No Game-Specific Assumptions

Until the GDD defines a system, Bezi must use neutral terminology only.

Allowed neutral terms:

- Feature
- Entity
- Action
- Definition
- RuntimeState
- SaveData
- Config
- Request
- Result
- ViewModel
- Screen
- Modal
- Component
- Service
- System

Do not introduce game-specific concepts unless they have been defined in the GDD.

Examples of concepts Bezi must not assume:

- Employees
- Products
- Contracts
- Inventory
- Combat
- Quests
- Characters
- Buildings
- Shops
- Skills
- Resources
- Enemies
- Dialogue
- Crafting
- Research
- Cards
- Units
- Levels
- Missions

---

## 6. GDD-First Workflow

Major features require a GDD page, implementation page, or direct user instruction before implementation.

A proper implementation page should define:

- Purpose
- Player-facing behaviour
- Rules
- Data requirements
- UI requirements
- Edge cases
- Save/load impact
- Dependencies
- Testing checklist
- Out-of-scope items

If these are missing, Bezi must not invent permanent design.

Temporary placeholders are allowed only when explicitly requested or unavoidable for technical scaffolding. All placeholders must be clearly marked.

---

## 7. Scope Control

Bezi must implement only the requested scope.

If asked to create data structures, do not build UI.

If asked to create UI structure, do not implement full core logic.

If asked to fix compile errors, do not refactor unrelated systems.

If asked to plan, do not implement.

If asked to implement, do not redesign the feature.

Avoid "while I was there" changes.

---

## 8. Decoupled Architecture Requirement

The project must use clean, decoupled architecture.

Bezi must avoid:

- Spaghetti code
- God classes
- Hidden dependencies
- Large all-in-one scripts
- UI directly owning game rules
- Core logic depending on UI
- Save/load logic mixed into UI
- Copy-pasted logic
- Hard-coded design assumptions
- Uncontrolled global state
- Unnecessary `Update()` loops

The detailed rules are defined in:

`/Pages/Private/Project Docs/Coding Standard.md`

Bezi must follow that document.

---

## 9. UI Toolkit Only

Runtime UI must use Unity UI Toolkit only.

Allowed:

- UIDocument
- VisualElement
- UXML
- USS
- UI Builder
- UI Toolkit runtime binding
- UI Toolkit custom controls
- UI Toolkit style classes

Not allowed for runtime UI:

- UGUI Canvas
- CanvasScaler
- RectTransform-based runtime UI
- TextMeshProUGUI as screen UI
- IMGUI runtime screens
- OnGUI runtime UI
- Third-party UI frameworks unless explicitly approved

The detailed UI rules are defined in:

`/Pages/Private/Project Docs/UI Architecture.md`

Bezi must follow that document.

---

## 10. Colour Palette Rule

Bezi must follow:

`/Pages/Private/Project Docs/Colour Palette.md`

Runtime UI colours must be defined through UI Toolkit USS variables and semantic colour tokens.

Do not invent new colours inside individual screens.

Do not hard-code colours directly into C# files.

Do not use inline colours in UXML.

If a colour is missing, Bezi must add a semantic token to `/Pages/Private/Project Docs/Colour Palette.md` and the relevant theme USS file instead of hard-coding a one-off value.

---

## 11. Typography Rule

Bezi must follow:

`/Pages/Private/Project Docs/Typography.md`

Runtime UI must use:

```text
Main UI Font:
Inter-VariableFont_opsz,wght.ttf

Secondary UI Font:
SpaceGrotesk-VariableFont_wght.ttf

Technical / Debug Font:
JetBrainsMono[wght].ttf
```

Do not invent new fonts per screen.

Do not use Unity default fonts for final runtime UI.

Do not hard-code font styling in C# unless technically required.

Typography must be centralized through UI Toolkit Font Assets and USS theme variables.

---

## 12. UI Sizing and Spacing Rule

Bezi must follow:

`/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md`

Runtime UI must be designed primarily for 1920×1080, while remaining scalable to other common desktop resolutions.

Do not invent one-off sizes, spacing, padding, button heights, modal sizes, row heights, or density rules per screen.

All sizing and spacing must use approved UI Toolkit USS variables and semantic layout classes.

Major layouts must be responsive and must not rely on fragile absolute positioning unless explicitly approved.

---

## 13. Required Plan Splitting

Bezi must split large implementation work into multiple smaller plans.

No single plan should be so large that it becomes difficult to:

- Review
- Test
- Debug
- Roll back
- Implement safely
- Understand dependency order

Plans must be split efficiently by dependency order.

Recommended order:

1. Data structures / definitions
2. Core logic
3. Infrastructure / persistence
4. UI Toolkit structure
5. UI Toolkit styling
6. UI Toolkit wiring
7. Integration
8. Validation and testing
9. Polish

Do not combine all of these into one plan unless the feature is very small.

---

## 14. Required Plan Format

Every implementation plan must use this structure:

```text
Plan [Number] — [Title]

Purpose:
- What this plan achieves.

Depends On:
- Required previous plans or files.

Scope:
- What this plan will implement.

Out of Scope:
- What this plan will not implement.

Files to Create:
- path/to/file

Files to Modify:
- path/to/file

Implementation Steps:
1. Step
2. Step
3. Step

Architecture Rules:
- How decoupling is maintained.
- Which layer each file belongs to.
- How UI Toolkit-only rules are preserved.

Testing:
- What Bezi must test after this plan.

Completion Criteria:
- Exact conditions that prove this plan is complete.
```

---

## 15. Plan Naming Rules

Use numbered plan parts.

Example:

```text
Plan 1A — ProjectDocs Setup
Plan 1B — Debug Logging Foundation
Plan 1C — Tuning Config Foundation
Plan 1D — UI Toolkit Shell Structure
Plan 1E — Integration Testing
```

If one plan is still too large, split again:

```text
Plan 1A-1 — Create Documentation Folder
Plan 1A-2 — Create Initial Documentation Files
```

---

## 16. Required Documentation Files

The project uses a two-tier documentation system:

**Standards** (rules, formats, entry templates — must not contain actual entries):

```text
/Pages/Private/Project Docs/Changelog.md
/Pages/Private/Project Docs/Formula Reference.md
/Pages/Private/Project Docs/Debugging Standard.md
/Pages/Private/Project Docs/Tuning Reference.md
```

**Working registries** (actual entries — must not redefine rules):

```text
/Pages/Private/Changelog/Changelog.md
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Formula/Formula Registry.md
```

Standards define format and rules only. Working registries record actual entries only.

Do not create additional `.md` files unless requested.

---

## 17. Changelog Rule

Bezi must update `/Pages/Private/Changelog/Changelog.md` after every implementation task that changes project files.

The entry format and rules are defined in `/Pages/Private/Project Docs/Changelog.md`.

The changelog must summarize:

- What changed
- Files touched
- What was added
- What was removed
- What was refactored
- What placeholders remain
- Follow-up required

Use clear entries.

Bad:

```text
Improved stuff.
Cleaned code.
Updated UI.
```

Good:

```text
Added ScreenRouter.cs to centralize UI Toolkit screen navigation.
Created MainShell.uxml and MainShell.uss for root UI structure.
Added placeholder screen IDs pending final GDD screen list.
```

---

## 18. Formula Reference Rule

Bezi must create and maintain `/Pages/Private/Formula/Formula Registry.md`.

The entry format and rules are defined in `/Pages/Private/Project Docs/Formula Reference.md`.

This registry must document every important formula, calculation, scoring rule, probability rule, weighting rule, rounding rule, clamp rule, and derived-value rule.

Every formula must have:

- Stable formula ID
- Purpose
- Inputs
- Output
- Formula
- Rounding rules
- Clamp rules
- Update timing
- Source GDD page or prompt
- Implemented code location

If code and `Formula Reference.md` conflict, Bezi must report the conflict.

---

## 19. Tuning Config Rule

Bezi must create and maintain a centralized tuning configuration system.

Required files:

```text
Assets/Project/Scripts/Infrastructure/Tuning/TuningConfig.cs
Assets/Resources/TuningConfig.asset
/Pages/Private/Tuning/Tuning Registry.md
```

The entry format and rules are defined in `/Pages/Private/Project Docs/Tuning Reference.md`.

Tuning values must not be scattered across scripts.

Use `TuningConfig` for:

- Designer-facing values
- Timing values
- Thresholds
- Multipliers
- Probabilities
- Clamp values
- Limits
- Cooldowns
- Refresh intervals

Do not use `TuningConfig` for:

- Debug logging toggles
- Save data
- Runtime state
- UI theme styling
- Static content definitions
- Large content tables

If a tuning value affects a formula, update both:

```text
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Formula/Formula Registry.md
```

---

## 20. Debug Logging Rule

Bezi must create and maintain a centralized debug logging system.

Required files:

```text
Assets/Project/Scripts/Core/Debugging/DebugCategory.cs
Assets/Project/Scripts/Core/Debugging/DebugLogger.cs
Assets/Project/Scripts/Infrastructure/Debugging/DebugConfig.cs
Assets/Resources/DebugConfig.asset
/Pages/Private/Project Docs/Debugging Standard.md
```

The debug system must include:

- Master logging switch
- Category toggles
- Inspector-editable `DebugConfig`
- Static `DebugLogger`
- Category filtering
- Optional colour-coded logs
- Context object support
- Genre-neutral initial categories only

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

Do not add game-specific debug categories until the relevant GDD system exists.

---

## 21. No Magic Numbers

Important adjustable values must not be hard-coded across scripts.

Use one of the following:

- `TuningConfig`
- Static definition data
- Named constants for purely technical local values
- GDD-defined values documented in `FORMULAS.md`

Bad:

```csharp
if (value > 75)
{
}
```

Good:

```csharp
if (value > _tuningConfig.ValidationThreshold)
{
}
```

---

## 22. Stable ID Rule

Persistent references must use stable IDs.

Good:

```text
screen.main_menu
modal.confirm_action
definition.example
setting.audio_volume
```

Bad:

```text
Main Menu
Confirm Action
Example
Audio Volume
```

Display names may change.

Stable IDs should not change without migration.

---

## 23. Placeholder Rule

Placeholders must be clearly marked.

Use:

```text
[Placeholder]
[Temp]
[Mock]
```

Placeholder values are not final design.

Placeholder values must be documented in:

```text
/Pages/Private/Changelog/Changelog.md
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Formula/Formula Registry.md
```

where relevant.

---

## 24. Refactor Rule

Do not refactor unrelated code during a task.

Refactoring is allowed only when:

- The user requests it
- The implementation page requires it
- Current code blocks the requested feature
- The project cannot compile without it

If refactoring is necessary, Bezi must state exactly what changed and why.

---

## 25. Testing Requirement

After implementation, Bezi must check:

- Project compiles
- No new console errors
- No Canvas/UGUI runtime UI was added
- New files follow `/Pages/Private/Project Docs/Coding Standard.md`
- UI follows `/Pages/Private/Project Docs/UI Architecture.md`
- Colour palette follows `/Pages/Private/Project Docs/Colour Palette.md`
- Typography follows `/Pages/Private/Project Docs/Typography.md`
- Sizing/spacing follows `/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md`
- No game-specific assumptions were introduced
- Documentation was updated
- Changelog was updated
- Formula and tuning docs were updated where relevant
- Debug docs were updated where relevant

If testing cannot be completed, Bezi must state what was not tested.

---

## 26. Required Bezi Response Format

After implementation, Bezi must respond with:

```text
Completed:
- Specific completed change.
- Specific completed change.

Files Changed:
- path/to/file
- path/to/file

Architecture Notes:
- How decoupling was maintained.
- Which architecture layers were touched.
- Whether UI Toolkit-only was followed.
- Any placeholder used.

Documentation Updated:
- CHANGELOG.md
- FORMULAS.md, if relevant
- TUNING.md, if relevant
- DEBUGGING.md, if relevant

Testing:
- What was tested.
- What could not be tested.

Issues / Follow-up:
- Any unresolved ambiguity.
- Any required next step.
```

Do not respond with vague summaries.

---

## 27. Final Rule

Build only what the GDD and current prompt define.

Follow:

```text
/Pages/Private/Project Docs/Project Rules.md
/Pages/Private/Project Docs/Coding Standard.md
/Pages/Private/Project Docs/UI Architecture.md
/Pages/Private/Project Docs/Colour Palette.md
/Pages/Private/Project Docs/Typography.md
/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md
```

Use UI Toolkit only for runtime UI.

Keep the architecture decoupled.

Avoid spaghetti code.

Do not invent the game before it has been designed.
