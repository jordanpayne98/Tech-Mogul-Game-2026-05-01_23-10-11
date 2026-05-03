# Phase 5H — Contracts Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.contracts`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_12, GDD_16.3, GDD_18.2
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the contracts board for available, active, completed, and failed/expired contracts, with contract details and team assignment entry points.

## 2. Scope
- Contract board UI.
- Active/completed contract lists.
- Contract filters.
- Contract detail modal.
- Assign Team modal placeholder.
- Placeholder/static contract ViewModel support.

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
- ScreenHeader with title “Contracts”.
- Tabs for contract states.
- Toolbar with type/difficulty dropdowns, search, filters, and board refresh placeholder.
- Main contract table/list.
- Right filter drawer.
- Contract detail modal and Assign Team modal.

## 6. Required Screen Content
### Tabs
- Available
- Active
- Completed
- Failed / Expired

### Columns
- Client
- Contract Type
- Required Skills
- Difficulty
- Deadline
- Payment
- Progress
- Assigned Team
- Quality Target
- Status
- Actions

### Filters
- Type
- Difficulty
- Deadline
- Payment
- Required Roles
- Client Segment
- Reputation Requirement

### Detail sections
- Client summary
- Requirements
- Deadline/payment
- Skill fit summary
- Milestones
- Potential outcomes
- Related reports

## 7. Interactions
- Contract row opens detail modal.
- Accept/Assign opens Assign Team modal placeholder.
- Active contracts link to assigned team details.
- Completed contracts link to completion report.
- Phase 5 must not resolve contract outcomes or payments.

## 8. Required States
- No available contracts state.
- No active contracts state.
- Deadline warning state.
- Skill gap warning state.
- Completed success state.
- Failed/expired muted/danger state depending severity.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Contracts/ContractsScreen.uxml`
- `Assets/Project/UI/USS/Screens/Contracts/ContractsScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Contracts/ContractsView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Contracts/ContractsController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Contracts/ContractsViewModel.cs`

Optional helper ViewModels, if needed:
- `ContractRowViewModel.cs`
- `ContractFilterViewModel.cs`
- `ContractDetailViewModel.cs`
- `ContractRequirementViewModel.cs`
- `AssignTeamOptionViewModel.cs`

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
- `ContractRowViewModel`
- `ContractFilterViewModel`
- `ContractDetailViewModel`
- `ContractRequirementViewModel`
- `AssignTeamOptionViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `contracts`
- `contracts__header`
- `contracts__toolbar`
- `contracts__tabs`
- `contracts__content`
- `contracts__panel`
- `contracts__row`
- `contracts__card`
- `contracts__drawer`
- `contracts__modal`
- `contracts__footer`

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
Contracts provide short-term work and early cash options. Phase 5 shows contract data and assignment hooks only; outcome logic belongs to Core/Application.
