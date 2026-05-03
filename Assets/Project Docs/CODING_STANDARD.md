# Coding Standard

## 1. Purpose

This document defines the coding standard for the project.

The goal is a clean, decoupled, maintainable codebase that can grow safely as the GDD expands.

Bezi must follow this document for all code generation, refactoring, fixes, and implementation plans.

---

## 2. Core Principles

All code must be:

- Readable
- Modular
- Decoupled
- Easy to debug
- Easy to test
- Easy to extend
- Safe to refactor
- Consistent with project structure

Avoid:

- Spaghetti code
- God classes
- Hidden dependencies
- Large all-in-one scripts
- Copy-pasted logic
- Hard-coded design values
- UI controlling core logic
- Core logic depending on UI
- Save/load mixed into unrelated systems
- Unnecessary `Update()` loops

---

## 3. Architecture Layers

Use these conceptual layers:

```text
Core
Application
Infrastructure
Presentation
Composition
```

---

## 4. Core Layer

Core contains pure project rules, state models, requests, results, events, and interfaces.

Core may contain:

- Runtime data models
- Static data models
- Interfaces
- Requests
- Results
- Domain events
- Pure logic
- Validation that does not depend on Unity scene objects

Core must not contain:

- UI Toolkit
- UIDocument
- VisualElement
- MonoBehaviour
- Scene references
- GameObject references
- Direct file IO
- PlayerPrefs
- Unity UI logic
- Runtime screen navigation

Core should be testable without opening a Unity scene.

---

## 5. Application Layer

Application coordinates use cases.

Application may contain:

- Use case classes
- Command handlers
- Validation flow
- System coordination
- ViewModel builders
- Result creation
- Application services

Application should not contain:

- UI layout code
- UXML/USS references
- VisualElement logic
- Scene object manipulation
- Save file implementation details unless the use case is explicitly save/load

---

## 6. Infrastructure Layer

Infrastructure contains Unity-specific implementation details and external-facing services.

Infrastructure may contain:

- Save/load implementation
- Asset loading
- Definition registry
- Resources loading
- File access
- Unity adapters
- Platform services
- DebugConfig ScriptableObject
- TuningConfig ScriptableObject

Infrastructure should implement interfaces defined by Core/Application where useful.

---

## 7. Presentation Layer

Presentation contains UI Toolkit code only.

Presentation may contain:

- UI Toolkit views
- UI controllers
- Presenters
- ViewModels
- Screen routing
- Modal routing
- UXML files
- USS files
- UI components

Presentation must not own core project rules.

Presentation must not directly mutate persistent runtime state without going through Application/Core.

---

## 8. Composition Layer

Composition wires the project together.

Composition may contain:

- Bootstrapper
- Scene entry points
- Dependency setup
- Initial service registration
- Startup validation

Composition must not become a god layer.

Composition must not contain core gameplay rules.

---

## 9. Dependency Direction

Preferred direction:

```text
Presentation → Application → Core
Infrastructure → Core/Application interfaces
Composition → all layers for setup only
```

Forbidden direction:

```text
Core → Presentation
Core → UI Toolkit
Core → Infrastructure
Core → Composition
Application → UI Toolkit views
Application → UXML/USS
```

Core logic must not know UI Toolkit exists.

---

## 10. Assembly Definition Rule

Use Assembly Definitions once the structure is established.

Recommended assemblies:

```text
Project.Core
Project.Application
Project.Infrastructure
Project.Presentation.UI
Project.Composition
Project.Tests
```

Allowed references:

```text
Project.Application → Project.Core
Project.Infrastructure → Project.Core
Project.Infrastructure → Project.Application
Project.Presentation.UI → Project.Core
Project.Presentation.UI → Project.Application
Project.Composition → all runtime assemblies
Project.Tests → assemblies under test
```

Forbidden references:

```text
Project.Core → Project.Presentation.UI
Project.Core → Project.Infrastructure
Project.Core → Project.Composition
Project.Application → Project.Presentation.UI
```

If assembly definitions are not created yet, still follow the same dependency direction.

---

## 11. Folder Structure

Use this baseline folder structure:

```text
Assets/
  Project/
    Scripts/
      Core/
        Debugging/
        Definitions/
        Events/
        Interfaces/
        Requests/
        Results/
        Runtime/
      Application/
        UseCases/
        ViewModels/
        Validation/
      Infrastructure/
        Debugging/
        Tuning/
        SaveLoad/
        Definitions/
        Unity/
      Presentation/
        UI/
          Screens/
          Modals/
          Components/
          Routing/
          Controllers/
          ViewModels/
      Composition/
      Tests/
    UI/
      UXML/
        Shell/
        Screens/
        Modals/
        Components/
      USS/
        Theme/
        Shell/
        Screens/
        Modals/
        Components/
      Assets/
        Icons/
        Fonts/
    Data/
      Definitions/
      Config/
    Scenes/
    Prefabs/
    Art/
    Audio/
  Resources/
    DebugConfig.asset
    TuningConfig.asset
ProjectDocs/
```

Do not create duplicate parallel structures.

Do not scatter related code across random folders.

---

## 12. Namespace Rules

Namespaces should mirror folder structure.

Use a neutral temporary root namespace until the final project namespace is locked.

Example:

```csharp
namespace Project.Core
{
}

namespace Project.Application
{
}

namespace Project.Infrastructure
{
}

namespace Project.Presentation.UI
{
}
```

Do not use inconsistent namespaces.

Bad:

```csharp
namespace Stuff
namespace Scripts
namespace Random
```

---

## 13. C# Naming Standard

Use this naming style:

```text
Classes: PascalCase
Structs: PascalCase
Enums: PascalCase
Interfaces: IPascalCase
Methods: PascalCase
Properties: PascalCase
Events: PascalCase
Private fields: _camelCase
Local variables: camelCase
Parameters: camelCase
Constants: PascalCase
```

Example:

```csharp
public sealed class RuntimeService
{
    private readonly RuntimeState _runtimeState;

    public bool IsInitialized { get; private set; }

    public RuntimeResult Execute(RuntimeRequest request)
    {
        return RuntimeResult.Success();
    }
}
```

---

## 14. File Rules

Use one primary class per file.

File name must match the main class name.

Good:

```text
RuntimeService.cs
SaveSystem.cs
ScreenRouter.cs
TuningConfig.cs
DebugLogger.cs
```

Bad:

```text
Stuff.cs
Managers.cs
Everything.cs
NewScript.cs
DataHolder.cs
```

Small helper types may be placed in the same file only when tightly coupled and not reused elsewhere.

---

## 15. Class Responsibility Rule

Each class should have one clear reason to change.

Split classes when they mix unrelated responsibilities.

Separate:

- Runtime rules
- Validation
- Save/load
- UI binding
- Input mapping
- Navigation
- Debug logging
- Tuning config
- Definition loading
- Audio feedback
- Visual feedback

Do not create one script that handles many unrelated concerns.

---

## 16. MonoBehaviour Rule

MonoBehaviours must be thin.

MonoBehaviours may:

- Hold Unity scene references
- Own Unity lifecycle entry points
- Connect UIDocument to UI controllers
- Start bootstrap flow
- Bridge Unity events into application code

MonoBehaviours must not:

- Contain large core systems
- Own permanent project rules
- Become giant manager classes
- Mix UI, save, rules, audio, and navigation together
- Search for unrelated systems every frame

---

## 17. No God Classes

Avoid classes that control the whole project.

Avoid:

```text
GameManager
MainManager
EverythingManager
MasterController
GlobalController
SystemManager
```

A bootstrapper is allowed only if it wires dependencies together.

A bootstrapper must not contain core rules.

---

## 18. Data Type Separation

Separate these data types:

```text
DefinitionData = static design data
RuntimeData = active runtime state
SaveData = serialized state
ConfigData = tunable configuration
ViewModel = display-ready UI data
RequestData = action input
ResultData = action output
EventData = notification data
```

Do not use one class for all purposes.

Neutral example:

```csharp
public sealed class FeatureDefinition
{
    public string Id;
    public string DisplayName;
}

public sealed class FeatureRuntimeData
{
    public string DefinitionId;
    public bool IsActive;
}

public sealed class FeatureSaveData
{
    public string DefinitionId;
    public bool IsActive;
}

public sealed class FeatureRequest
{
    public string DefinitionId;
}

public sealed class FeatureResult
{
    public bool Success;
    public string FailureReason;
}
```

---

## 19. Request / Result Pattern

Actions that change state should use request/result patterns where useful.

Preferred flow:

```text
User interaction
→ Controller
→ Request
→ Application/Core service
→ Result
→ ViewModel refresh
→ View update
```

Neutral example:

```csharp
public sealed class ExecuteActionRequest
{
    public string ActionId { get; }

    public ExecuteActionRequest(string actionId)
    {
        ActionId = actionId;
    }
}

public sealed class ExecuteActionResult
{
    public bool Success { get; }
    public string FailureReason { get; }

    private ExecuteActionResult(bool success, string failureReason)
    {
        Success = success;
        FailureReason = failureReason;
    }

    public static ExecuteActionResult Succeeded()
    {
        return new ExecuteActionResult(true, string.Empty);
    }

    public static ExecuteActionResult Failed(string reason)
    {
        return new ExecuteActionResult(false, reason);
    }
}
```

Do not let UI buttons directly mutate runtime state.

---

## 20. Event Rule

Events may be used for decoupled notification.

Use events for:

- State changed notifications
- UI refresh notifications
- Cross-cutting notifications
- Decoupled reactions

Do not use events to hide messy dependencies.

Do not create long event chains that are hard to debug.

Prefer direct request/result calls for direct user actions.

---

## 21. Stable ID Rule

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

## 22. Save/Load Rule

Save/load must be separate from UI.

Save data should be plain serializable data.

Save data should not contain:

- VisualElement references
- UIDocument references
- MonoBehaviour references
- Scene object references
- Runtime-only object references

Use stable IDs for persistent references.

---

## 23. Debug Logging Rule

Use centralized debug logging.

Standard logging API:

```csharp
DebugLogger.Log(DebugCategory category, string message, Object context = null);
DebugLogger.LogWarning(DebugCategory category, string message, Object context = null);
DebugLogger.LogError(DebugCategory category, string message, Object context = null);
```

Do not scatter raw `Debug.Log()` calls throughout production code unless there is a specific reason.

Acceptable exceptions:

- Very early bootstrap errors before DebugLogger is available
- Temporary investigation logs removed before completion
- Unity or package-generated logs

---

## 24. Tuning Config Rule

Important adjustable values must use `TuningConfig` or another approved centralized config.

Do not scatter tuning values across scripts.

Use `TuningConfig` for:

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
- UI theme tokens
- Static content definitions

---

## 25. No Magic Numbers

Avoid hard-coded design values.

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

Small purely technical constants are allowed.

Example:

```csharp
private const int FirstIndex = 0;
```

---

## 26. Update Loop Rule

Do not use `Update()` by default.

Use `Update()` only when required.

Avoid:

- Rebuilding UI every frame
- Searching scene objects every frame
- Recalculating unrelated state every frame
- Polling when events/commands would work

Prefer:

- Commands
- Events
- Scheduled ticks
- Explicit refresh calls
- UI Toolkit callbacks

---

## 27. Error Handling Rule

Do not fail silently.

Normal invalid actions should return clear failure results.

Broken setup may throw exceptions or log clear errors.

Examples of broken setup:

- Required UI element missing from UXML
- Duplicate stable IDs
- Required dependency missing
- Invalid project configuration
- Missing required asset

Do not spam logs during normal gameplay.

---

## 28. Comments Rule

Use comments to explain intent, architecture, or non-obvious behaviour.

Good:

```csharp
// Stored as a stable ID so saved data survives display name changes.
public string DefinitionId;
```

Bad:

```csharp
// Set active to true.
isActive = true;
```

Do not leave obsolete comments after changing logic.

---

## 29. Placeholder Rule

Placeholders must be clearly marked.

Use:

```text
[Placeholder]
[Temp]
[Mock]
```

Placeholder values must not become permanent design by accident.

---

## 30. Refactor Rule

Do not refactor unrelated code during a task.

Refactoring is allowed only when:

- The user requests it
- The implementation page requires it
- Current code blocks the requested feature
- The project cannot compile without it

If refactoring is required, keep it minimal and document it in `CHANGELOG.md`.

---

## 31. Testing Rule

After code changes, Bezi must check:

- Project compiles
- No new console errors
- Dependency direction was preserved
- No UI Toolkit dependency was added to Core
- No Canvas/UGUI runtime UI was added
- No unrelated refactors were made
- No hidden scene searches were added
- No unnecessary `Update()` loops were added
- Documentation was updated where required

---

## 32. Final Rule

Keep the code decoupled.

Do not create spaghetti code.

Do not invent game-specific systems before the GDD defines them.

Follow this document for all code.
