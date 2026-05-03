# Phase 5E — Teams Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.teams`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_05, GDD_16.4, Appendix A.4
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the team management screen for viewing team composition, workload, cohesion, assignments, and capacity planning.

## 2. Scope
- Team summary cards.
- Team list/table.
- Team detail drawer or modal.
- Create Team modal placeholder.
- Assignment status display.
- Capacity/workload and role-gap presentation.

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
- ScreenHeader with title “Teams”.
- Top row of team summary cards.
- Main team table/list.
- Optional right drawer for selected team detail.
- Create Team modal for focused setup.
- Assignment status region below or inside team detail.

## 6. Required Screen Content
### Summary cards
- Total Teams
- Available Teams
- Overloaded Teams
- Average Morale
- Average Cohesion
- Open Role Gaps

### Table columns
- Team Name
- Function
- Members
- Lead
- Current Assignment
- Capacity
- Workload
- Morale
- Cohesion
- Role Gaps
- Status
- Actions

### Team detail sections
- Members
- Lead
- Skill coverage
- Workload
- Current assignment
- Assignment history
- Role gaps
- Burnout risk

### Team functions
- Core Software
- Hardware Lab
- Product Design
- QA & Reliability
- Infrastructure
- Marketing
- Support
- Research
- Growth / Sales
- Operations

## 7. Interactions
- Create Team opens modal placeholder.
- Team row opens detail drawer/modal.
- Assignment buttons route to Products, Contracts, or Research screens later.
- Members list may link to Employee Profile modal.
- Do not implement final assignment rules in Presentation.

## 8. Required States
- No teams yet state with Create Team call-to-action.
- Unassigned team state.
- Overloaded team warning state.
- Missing lead/role gap warning state.
- Team unavailable state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Teams/TeamsScreen.uxml`
- `Assets/Project/UI/USS/Screens/Teams/TeamsScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Teams/TeamsView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Teams/TeamsController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Teams/TeamsViewModel.cs`

Optional helper ViewModels, if needed:
- `TeamRowViewModel.cs`
- `TeamSummaryCardViewModel.cs`
- `TeamDetailViewModel.cs`
- `TeamMemberMiniViewModel.cs`
- `TeamGapViewModel.cs`

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
- `TeamRowViewModel`
- `TeamSummaryCardViewModel`
- `TeamDetailViewModel`
- `TeamMemberMiniViewModel`
- `TeamGapViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `teams`
- `teams__header`
- `teams__toolbar`
- `teams__tabs`
- `teams__content`
- `teams__panel`
- `teams__row`
- `teams__card`
- `teams__drawer`
- `teams__modal`
- `teams__footer`

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
Teams are the production engine. The screen shows suitability, workload, and gaps without calculating final gameplay outcomes in the UI.
