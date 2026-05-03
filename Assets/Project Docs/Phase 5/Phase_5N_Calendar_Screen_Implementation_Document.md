# Phase 5N — Calendar Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.calendar`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_02, GDD_14.2-GDD_14.3, GDD_16.3
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the calendar screen for upcoming reports, deadlines, product dates, payroll, candidate responses, research completion estimates, and market/competitor events.

## 2. Scope
- Month/week calendar view.
- Upcoming events list.
- Event detail modal.
- Filters by event type.
- Placeholder/static calendar ViewModel support.

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
- ScreenHeader with title “Calendar”.
- Toolbar with month/week toggle, current date selector, event type filters, and search.
- Calendar grid main area.
- Upcoming events list right or below depending width.
- Event detail modal.

## 6. Required Screen Content
### Event types
- Product target release date
- Contract deadline
- Monthly finance report
- Candidate response expected
- Research completion estimate
- Product report
- Payroll date
- Market/competitor report
- Infrastructure/support warning
- Launch day

### Event fields
- Title
- Date/time
- Category
- Priority
- Related entity
- Decision required flag
- Summary
- Route target

### Views
- Month view
- Week view
- Upcoming list

## 7. Interactions
- Click event opens detail modal.
- Click related entity routes to appropriate screen.
- Month/week toggle switches layout.
- Filters update placeholder event list.
- No complex scheduling/rescheduling logic is implemented in Phase 5.

## 8. Required States
- No upcoming events state.
- Today highlight state.
- Decision-required event state.
- Deadline warning state.
- Past/completed event muted state.
- Filtered empty state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Calendar/CalendarScreen.uxml`
- `Assets/Project/UI/USS/Screens/Calendar/CalendarScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Calendar/CalendarView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Calendar/CalendarController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Calendar/CalendarViewModel.cs`

Optional helper ViewModels, if needed:
- `CalendarViewModel.cs`
- `CalendarDayViewModel.cs`
- `CalendarEventViewModel.cs`
- `UpcomingEventRowViewModel.cs`
- `CalendarFilterViewModel.cs`

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
- `CalendarViewModel`
- `CalendarDayViewModel`
- `CalendarEventViewModel`
- `UpcomingEventRowViewModel`
- `CalendarFilterViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `calendar`
- `calendar__header`
- `calendar__toolbar`
- `calendar__tabs`
- `calendar__content`
- `calendar__panel`
- `calendar__row`
- `calendar__card`
- `calendar__drawer`
- `calendar__modal`
- `calendar__footer`

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
Calendar is informational in Phase 5. It does not implement a complex scheduling system.
