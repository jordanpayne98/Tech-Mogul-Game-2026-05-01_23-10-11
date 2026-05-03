# Phase 5L — Research Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.research`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_13, GDD_16.3, GDD_18.2
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the research screen for research tracks, available projects, locked/future projects, assigned research, completion estimates, and unlock previews.

## 2. Scope
- Research track tabs.
- Available project list.
- Locked/future project list.
- Assigned research section.
- Research detail modal.
- Placeholder/static research ViewModel support.

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
- ScreenHeader with title “Research”.
- Track tabs or side list.
- Available research project cards/list.
- Assigned research panel.
- Locked/future project section.
- Research detail modal or right drawer.

## 6. Required Screen Content
### Tracks
- Software Engineering
- Hardware Engineering
- Cloud Infrastructure
- Security
- AI & Automation
- UX/Product Design
- Developer Tools
- Manufacturing
- Marketing Analytics
- Support Operations
- Platform Ecosystems

### Project fields
- Name
- Track
- Required skill
- Duration
- Cost
- Unlocks
- Risk level
- Obsolescence risk
- Prerequisites
- Assigned team
- Completion estimate
- Related products

### Detail sections
- Purpose
- Requirements
- Expected benefits
- Unlocks
- Related products
- Assigned team options
- Warnings

## 7. Interactions
- Track tab filters projects.
- Project row/card opens detail modal.
- Assign Team action is placeholder until Phase 6 wiring.
- Locked project shows prerequisites, not hidden formulas.
- Completion report links to Reports/Inbox if available.

## 8. Required States
- No available projects state.
- No team assigned state.
- Locked/future state.
- Research in progress state.
- Research completed state.
- Obsolescence warning state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Research/ResearchScreen.uxml`
- `Assets/Project/UI/USS/Screens/Research/ResearchScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Research/ResearchView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Research/ResearchController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Research/ResearchViewModel.cs`

Optional helper ViewModels, if needed:
- `ResearchTrackViewModel.cs`
- `ResearchProjectRowViewModel.cs`
- `ResearchDetailViewModel.cs`
- `ResearchUnlockViewModel.cs`
- `AssignedResearchViewModel.cs`

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
- `ResearchTrackViewModel`
- `ResearchProjectRowViewModel`
- `ResearchDetailViewModel`
- `ResearchUnlockViewModel`
- `AssignedResearchViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `research`
- `research__header`
- `research__toolbar`
- `research__tabs`
- `research__content`
- `research__panel`
- `research__row`
- `research__card`
- `research__drawer`
- `research__modal`
- `research__footer`

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
Research UI can show locked future projects and expected benefits, but must not reveal exact hidden formulas.
