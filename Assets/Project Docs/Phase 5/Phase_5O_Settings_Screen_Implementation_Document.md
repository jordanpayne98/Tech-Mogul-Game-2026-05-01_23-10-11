# Phase 5O — Settings Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.settings`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_16.3, GDD_17.3-GDD_17.4, GDD_02.5
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the settings screen for general options, UI/accessibility preferences, Continue interruption preferences, save/load entry points, and development-safe placeholder settings.

## 2. Scope
- Settings category tabs.
- General settings section.
- UI/accessibility section.
- Continue interruption filters section.
- Save/load section placeholders.
- Confirmation modals for destructive settings actions if needed.

## 3. Out of Scope
- Final simulation logic.
- Save/load implementation unless already provided by Phase 3 and explicitly wired later.
- Market, competitor, finance, employee, product, contract, research, or report generation logic.
- Tuning/formula changes unless this screen directly introduces a documented adjustable runtime value.
- Runtime debug UI.
- Decorative shaders, particles, sound effects, or complex animations unless already approved by Phase 4 polish rules.

## 4. Visual Target
- Use the locked dark founder dashboard visual language: deep blue-black background, dark navy raised panels, charcoal surfaces, subtle blue-grey borders, off-white primary text, muted grey-blue secondary text, and restrained teal/cyan accents.
- The UI must feel like a premium desktop management dashboard for a serious technology-company simulation. It must not look like default Unity UI, mobile-first UI, toy-like tycoon UI, cyberpunk neon overload, or flat black debug panels.
- All in-game Phase 5 screens use the Phase 4 MainGameShell unless this document explicitly says otherwise: top status bar, left sidebar navigation, central screen host, optional right drawer, modal layer, tooltip layer, notification layer, and debug layer.
- Every screen must be information-first. Show values, status, trend, timeframe, and drill-down links where relevant. Do not prescribe an optimal strategy.
- Use UI Toolkit only. Do not add Canvas, CanvasScaler, RectTransform-based runtime UI, TextMeshProUGUI screen UI, IMGUI runtime screens, or third-party UI frameworks.

## 5. Layout Specification
- ScreenHeader with title “Settings”.
- Left or top category tabs.
- Main settings panel with grouped cards/sections.
- Optional right summary/help panel for selected setting.
- Footer/action row for Apply/Reset if needed.

## 6. Required Screen Content
### Categories
- General
- UI
- Accessibility
- Continue Interruptions
- Save / Load
- Audio Placeholder
- Controls Placeholder

### Continue interruption options
- Critical only
- Important and critical
- All reports
- Custom filters by category

### Save/load placeholders
- Manual save slots
- Rolling autosaves
- Quick save
- Named company saves
- Autosave frequency

### Accessibility options
- Text scale placeholder
- Colourblind-safe labels
- Reduced motion placeholder
- Keyboard focus visibility
- Tooltip delay placeholder

## 7. Interactions
- Tabs switch settings category.
- Apply/Reset buttons update placeholder ViewModel state in Phase 5.
- Save/load buttons route to placeholder modals unless real Phase 3/6 wiring exists.
- Destructive actions require confirmation modal.
- No settings may directly mutate unrelated simulation state from the UI.

## 8. Required States
- Unsaved changes state.
- Setting disabled/unavailable state.
- Placeholder setting state clearly marked.
- Invalid custom setting state.
- Save/load unavailable state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Settings/SettingsScreen.uxml`
- `Assets/Project/UI/USS/Screens/Settings/SettingsScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Settings/SettingsView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Settings/SettingsController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Settings/SettingsViewModel.cs`

Optional helper ViewModels, if needed:
- `SettingsViewModel.cs`
- `SettingsCategoryViewModel.cs`
- `SettingRowViewModel.cs`
- `ContinueFilterViewModel.cs`
- `SaveLoadOptionViewModel.cs`

## 11. Required ViewModel Content
The ViewModel must expose semantic display state, not raw UI styling values.

Required common fields:
- `ScreenTitle`
- `ScreenSubtitle`
- `IsLoading`
- `HasError`
- `ErrorMessage`
- `EmptyStateTitle`
- `EmptyStateBody`
- `SelectedId`
- `VisibleTabs`
- `ActiveTabId`
- `SearchText`
- `FilterState`
- `Rows`
- `Cards`
- `Actions`
- `RouteTargets`

Plan-specific helper models:
- `SettingsViewModel`
- `SettingsCategoryViewModel`
- `SettingRowViewModel`
- `ContinueFilterViewModel`
- `SaveLoadOptionViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `settings`
- `settings__header`
- `settings__toolbar`
- `settings__tabs`
- `settings__content`
- `settings__panel`
- `settings__row`
- `settings__card`
- `settings__drawer`
- `settings__modal`
- `settings__footer`

Use state classes:
- `is-active`
- `is-selected`
- `is-disabled`
- `is-hidden`
- `is-loading`
- `is-empty`
- `has-warning`
- `has-error`

Forbidden styling:
- Hard-coded colours in C#
- Inline UXML colours
- One-off screen-specific font sizes
- One-off screen-specific button heights
- Random margins or absolute positioning for major layouts

## 13. Completion Criteria
- Screen opens through its route or a test entry point.
- No runtime Canvas/UGUI was added.
- UXML, USS, View, Controller/Presenter, and ViewModel files exist in the approved folder structure.
- Screen uses shared theme tokens for colours, typography, sizing, spacing, borders, and states.
- Required tabs, toolbar, content areas, drawers, modals, empty/loading/error states, and disabled states are implemented where specified.
- All interactive elements provide visible feedback for default, hover, focus, selected, disabled, and invalid states where applicable.
- Placeholder/static data is clearly marked and does not pretend to be final simulation data.
- No core gameplay rule is placed in Presentation.
- Project compiles and no new console errors are introduced.
- Changelog is updated after implementation.

## 14. Final Lock
Settings can display interruption filter and save/load options, but full behaviour wiring belongs in Phase 6 or later infrastructure integration.
