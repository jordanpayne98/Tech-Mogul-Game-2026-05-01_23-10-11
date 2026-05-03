# UI Architecture

## 1. Purpose

This document defines the runtime UI architecture for the project.

Runtime UI must use Unity UI Toolkit only.

Bezi must follow this document for all runtime UI work.

---

## 2. UI Toolkit Only Rule

Runtime UI must use Unity UI Toolkit only.

Allowed:

- UIDocument
- VisualElement
- UXML
- USS
- UI Builder
- UI Toolkit runtime data binding
- UI Toolkit custom controls
- UI Toolkit style classes

Not allowed for runtime game UI:

- UGUI Canvas
- CanvasScaler
- RectTransform-based runtime UI
- TextMeshProUGUI as screen UI
- IMGUI runtime screens
- OnGUI runtime UI
- Third-party UI frameworks unless explicitly approved

Editor tools may use IMGUI or UI Toolkit depending on the tool, but player-facing runtime UI must be UI Toolkit.

---

## 3. Required UI Reference Documents

All runtime UI work must follow:

```text
/Pages/Private/Project Docs/UI Architecture.md
/Pages/Private/Project Docs/Colour Palette.md
/Pages/Private/Project Docs/Typography.md
/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md
```

These documents define the global UI rules. Feature pages define what appears on a screen.

Feature pages must not invent one-off layout structures, component styles, colours, fonts, button sizes, modal sizes, or spacing rules without updating the relevant global UI source first.

---

## 4. UI Architecture Principle

UI must be decoupled from core logic.

Use this structure where practical:

```text
View
  Owns visual references
  Displays data
  Emits UI interactions

Controller / Presenter
  Handles UI events
  Calls Application services
  Builds or receives ViewModels
  Sends display data to Views

ViewModel
  Contains display-ready data only

Application/Core
  Owns rules, validation, and state changes
```

UI must not directly own core project rules.

---

## 5. Dependency Direction

Allowed:

```text
UI View → UI Controller
UI Controller → Application
Application → Core
UI View → ViewModel
```

Forbidden:

```text
Core → UI Toolkit
Core → VisualElement
Core → UIDocument
Core → UXML
Core → USS
Application → VisualElement
Application → UIDocument
```

Core logic must not know that UI Toolkit exists.

---

## 6. UI Layering Model

Use this UI layering model:

```text
Root UIDocument
├── Application Shell
├── Screen Layer
├── Modal Layer
├── Tooltip Layer
├── Notification Layer
└── Debug Layer
```

Each layer has one purpose.

Screens should not manually manage unrelated UI layers.

Navigation should be centralized.

---

## 7. Root UIDocument Rule

The root UIDocument should own the main UI tree.

The root should provide containers for:

- Shell
- Screens
- Modals
- Tooltips
- Notifications
- Debug UI, if needed later

Do not create many unrelated root UIDocuments unless the UI architecture is explicitly changed.

---

## 8. Screen Rule

A screen is a major UI surface.

A screen should:

- Display one major area
- Own its visual layout
- Bind to a controller or presenter
- Receive a ViewModel
- Send user intent outward
- Handle empty/loading/error states where needed

A screen must not:

- Own save/load
- Own core rules
- Mutate runtime state directly
- Contain unrelated global navigation rules
- Directly control unrelated screens

---

## 9. Modal Rule

A modal is a focused temporary overlay.

Modal rules:

- Clear open behaviour
- Clear close behaviour
- Clear confirm/cancel behaviour
- Input validation before submission
- No permanent state change until confirmed unless explicitly designed
- No nested modal chains unless explicitly approved

Use modals for focused decisions or detailed temporary interactions.

Do not use modals for information that belongs on a screen.

---

## 10. Component Rule

Reusable UI patterns should become components.

Examples of neutral reusable components:

- Base button
- Panel
- List row
- Tab button
- Dropdown
- Input field
- Confirmation modal
- Header bar
- Navigation item
- Status label
- Empty state
- Loading state
- Warning message

Do not duplicate the same UI structure repeatedly.

Generic UI components must not contain game-specific business logic.

---

## 11. Required UI File Pattern

Each major screen should normally use:

```text
ScreenName.uxml
ScreenName.uss
ScreenNameView.cs
ScreenNameController.cs
ScreenNameViewModel.cs
```

Each modal should normally use:

```text
ModalName.uxml
ModalName.uss
ModalNameView.cs
ModalNameController.cs
ModalNameViewModel.cs, if needed
```

Each reusable component should normally use:

```text
ComponentName.uxml
ComponentName.uss
ComponentNameView.cs, if needed
ComponentNameViewModel.cs, if needed
```

Do not create one giant UXML file for the whole game UI.

Do not create one giant controller for every screen.

---

## 12. UI Folder Structure

Use this structure:

```text
Assets/
  Project/
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
    Scripts/
      Presentation/
        UI/
          Screens/
          Modals/
          Components/
          Routing/
          Controllers/
          ViewModels/
```

Keep UXML, USS, and C# organized.

Do not scatter UI Toolkit files randomly across the project.

---

## 13. View Rule

A View owns visual references only.

A View may:

- Query VisualElements from its root
- Register UI callbacks
- Unregister UI callbacks
- Display ViewModel data
- Toggle style classes
- Show validation messages
- Control local visual state

A View must not:

- Own core rules
- Directly mutate persistent runtime state
- Save or load
- Create permanent project entities directly
- Search the scene for unrelated systems
- Decide permanent design values
- Own unrelated navigation

---

## 14. Controller / Presenter Rule

A Controller or Presenter handles interaction between a View and the Application layer.

A Controller may:

- Receive UI callbacks
- Build requests
- Call Application services
- Handle results
- Request refreshed ViewModels
- Tell the View what to display

A Controller must not:

- Contain large core rules
- Become a god class
- Manage unrelated screens
- Save/load unless the use case is specifically save/load
- Directly style large parts of the UI

---

## 15. ViewModel Rule

ViewModels contain display-ready data.

A ViewModel may contain:

- Text already formatted for display
- Boolean UI states
- Lists of row data
- Warning/error text
- Button enabled/disabled states
- Selected IDs
- Display labels
- Progress values
- Visibility flags
- Semantic visual states

A ViewModel must not:

- Contain core rules
- Mutate runtime state
- Save data
- Hold scene references
- Hold VisualElement references
- Hold UIDocument references
- Hold raw colours for ordinary UI state
- Hold raw pixel values for ordinary layout state

---

## 16. UI Toolkit Binding Rule

Use UI Toolkit runtime binding when it makes the UI cleaner.

Use binding for:

- Display state
- Repeated data
- Form-like UI
- Screens with many fields
- ViewModel-driven surfaces

Use explicit controller callbacks for:

- Button clicks
- Confirm/cancel actions
- Navigation
- Commands that mutate state
- Validation and result handling

Do not force binding everywhere if a simple `Bind(viewModel)` method is clearer.

---

## 17. UXML Rule

Use UXML for screen and component structure unless there is a clear reason to generate UI in C#.

Use C# generated UI for:

- Dynamic repeated content
- Custom controls
- Data-driven generated elements
- Cases where UXML would be harder to maintain

Do not build every screen entirely in C# if UXML would be clearer.

---

## 18. USS Rule

Use USS for styling.

C# may toggle classes.

C# should not manually apply large amounts of visual styling unless there is a specific runtime reason.

Centralize:

- Colours
- Fonts
- Spacing
- Borders
- Corner radius
- Panel styles
- Button styles
- Text styles
- Icon sizes
- Animation timings, where applicable

Do not hard-code styling repeatedly in C#.

---

## 19. Colour Palette Rule

All runtime UI colours must follow:

`/Pages/Private/Project Docs/Colour Palette.md`

Use semantic colour tokens instead of one-off colours.

Good:

```css
background-color: var(--color-bg-panel);
color: var(--color-text-primary);
border-color: var(--color-border-subtle);
```

Bad:

```css
background-color: #1b1b1b;
color: #ffffff;
border-color: #333333;
```

C# must not hard-code visual colours unless there is a specific runtime technical reason.

If C# needs to change colour state, it should usually toggle USS classes instead.

---

## 20. Typography Rule

All runtime UI typography must follow:

`/Pages/Private/Project Docs/Typography.md`

Use:

```text
Inter-VariableFont_opsz,wght.ttf for standard UI.
SpaceGrotesk-VariableFont_wght.ttf for display titles and high-emphasis headings.
JetBrainsMono[wght].ttf for technical/debug text.
```

Use USS classes and theme variables for font family, weight, size, and hierarchy.

Do not set fonts directly in C# unless technically required.

---

## 21. Sizing and Spacing Rule

All runtime UI sizing, spacing, density, panel padding, row height, button height, modal sizing, and layout scaling must follow:

`/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md`

Use USS variables and reusable layout classes.

Do not hard-code one-off spacing values per screen.

Do not create screen-specific density rules unless the GDD/UI page explicitly requires them and the global sizing document is updated first.

---

## 22. USS Naming Standard

Use consistent class names.

Recommended style:

```text
screen-name
screen-name__section
screen-name__element

component-name
component-name__element

is-active
is-disabled
is-selected
is-hidden
has-warning
has-error
```

Bad:

```text
box1
new-style
thing-red
button-final2
```

Good:

```text
main-shell
main-shell__screen-layer
base-button
base-button__label
is-selected
has-warning
```

---

## 23. Theme Rule

Create shared theme USS files.

Required theme files:

```text
Assets/Project/UI/USS/Theme/theme-colors.uss
Assets/Project/UI/USS/Theme/theme-typography.uss
Assets/Project/UI/USS/Theme/theme-spacing.uss
Assets/Project/UI/USS/Theme/theme-sizing.uss
Assets/Project/UI/USS/Theme/theme-layout.uss
Assets/Project/UI/USS/Theme/theme-components.uss, if needed
```

Exact theme files may change later, but styling must remain centralized.

Do not create a new visual language for every screen.

---

## 24. Screen Router Rule

Navigation must be centralized.

Use a ScreenRouter or equivalent.

Preferred pattern:

```csharp
_screenRouter.OpenScreen("screen.example");
```

Do not let buttons manually enable/disable many unrelated UI surfaces.

Navigation IDs must be stable IDs.

---

## 25. Modal Router Rule

Modal navigation must be centralized.

Preferred pattern:

```csharp
_modalRouter.OpenModal("modal.example");
_modalRouter.CloseModal();
```

Screens should request modals through the modal router.

Do not let individual screens create unmanaged modal stacks.

---

## 26. UI State Separation

Keep these separate:

```text
Game state
UI navigation state
Temporary form state
Selection state
Hover/focus state
Modal state
Save data
```

Temporary UI state should not be saved unless the GDD explicitly requires it.

---

## 27. UI Refresh Rule

UI should refresh only when needed.

Preferred refresh triggers:

- Screen opened
- Relevant state changed
- Command completed
- Data source updated
- Manual refresh requested
- Scheduled tick, if explicitly designed

Avoid refreshing the full UI every frame.

Do not rebuild large UI trees unnecessarily.

---

## 28. List and Table Rule

For list-heavy UI, use consistent row components.

Lists should define:

- Empty state
- Selected state
- Hover state
- Disabled state
- Loading state, if needed
- Error state, if needed

Do not add sorting, filtering, pagination, grouping, or search unless the GDD/UI spec requests it.

---

## 29. UI Feedback Rule

Every player action should give appropriate feedback.

Examples:

- Button disabled when action is unavailable
- Validation message for invalid form input
- Confirmation for destructive action
- Success state after completed action
- Failure reason when command fails

Do not silently ignore clicks.

Do not allow invalid actions to appear valid.

---

## 30. Input Rule

Input handling must be deliberate.

Do not hard-code shortcuts across random UI files.

If shortcuts are added, they should go through an input layer or UI navigation layer.

Buttons should have predictable focus, cancel, and confirmation behaviour where relevant.

---

## 31. Accessibility Baseline

Do not rely only on colour to communicate important meaning.

Important information should use text, icons, labels, or layout as appropriate.

Text must remain readable.

Buttons must have clear labels unless icon-only buttons are explicitly approved and have tooltips or accessible labels.

---

## 32. Placeholder UI Rule

Placeholder UI must be obvious.

Use labels such as:

```text
[Placeholder]
[Temp]
[Mock]
```

Do not make placeholder UI look like final design unless explicitly requested.

Do not add fake data unless the prompt asks for mock data.

---

## 33. UI Polish Rule

Polish is separate from functional implementation unless requested.

Do not add the following unless the UI spec requires them:

- Animations
- Shaders
- Particles
- Screen transitions
- Sound effects
- Decorative panels
- Complex hover effects
- Complex responsive behaviour
- Charts

When polish is added, it must not reduce readability or usability.

---

## 34. Debug UI Rule

Do not build runtime debug UI unless requested.

The project may later add:

- Debug overlay
- Tuning inspector
- Formula inspector
- State viewer
- Save data viewer
- Debug category toggle screen

For now, only build debug UI when a GDD page or prompt requests it.

---

## 35. UI and Debug Logging

UI navigation and major UI failures should use the centralized debug logger.

Use neutral categories:

- UI
- Navigation
- Validation
- Configuration
- Performance

Do not add game-specific debug categories until the GDD defines those systems.

---

## 36. UI and Tuning

UI behaviour values may use `TuningConfig` only when they are runtime behaviour values.

Examples:

- Notification display duration
- Tooltip delay
- Repeated input delay
- UI refresh interval

Visual style values should usually stay in USS/theme files.

Do not put colours, fonts, spacing, and panel styling into `TuningConfig` unless explicitly approved.

---

## 37. UI Testing Rule

After UI implementation, Bezi must check:

- Screen opens correctly
- UI Toolkit only was used
- No Canvas/UGUI runtime UI was added
- Buttons are wired correctly
- Invalid actions are blocked or handled
- Empty states work
- Missing data does not crash UI
- Modal close/cancel/confirm works
- Navigation returns to expected screen
- No duplicate UI controllers are created
- No console errors are produced
- View does not own core rules
- Core does not depend on UI Toolkit
- Colour palette tokens are used
- Typography tokens/classes are used
- Sizing/spacing tokens/classes are used

If testing is incomplete, Bezi must state what was not tested.

---

## 38. Final Rule

Use UI Toolkit only for runtime UI.

Keep UI decoupled from core logic.

Use Views, Controllers/Presenters, ViewModels, ScreenRouter, and ModalRouter.

Do not invent UI screens before the GDD defines them.

Do not mix UI architecture with core project rules.
