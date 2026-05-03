# Phase 5M — Reports / Inbox Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.reports_inbox`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_14, GDD_16.3, GDD_18.6
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the inbox/report hub that collects decision-required messages, finance reports, product reports, hiring updates, market events, competitor launches, support warnings, and archived reports.

## 2. Scope
- Three-panel inbox layout.
- Category rail.
- Report list.
- Report preview/detail panel.
- Search/filter toolbar.
- Archive/delete/read state actions.
- Placeholder/static report ViewModel support.

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
- ScreenHeader with title “Reports / Inbox”.
- Top toolbar with search, category filter, priority filter, unread toggle, and sort.
- Left category rail.
- Centre report list.
- Right report preview/detail panel.
- Optional modal for full report detail.

## 6. Required Screen Content
### Categories
- All
- Requires Decision
- Finance
- Products
- Employees
- Teams
- Hiring
- Market
- Competitors
- Infrastructure
- Support
- Contracts
- Research
- Archived

### Report fields
- Title
- Date
- Category
- Summary
- Key numbers
- What changed
- Cause indicators
- Related entities
- Required decision flag
- Read/archive/delete state

### Required MVP report types
- Monthly finance report
- Candidate offer response
- Contract completion report
- Product progress milestone report
- Product launch report
- Monthly product performance report
- Market trend report
- Competitor launch report
- Infrastructure warning report
- Support/bug report

### Actions
- Open
- Archive
- Delete
- Pin
- Mark unread
- Jump to related entity
- Create task placeholder

## 7. Interactions
- Category selection filters report list.
- Report row opens preview/detail panel.
- Archive/delete actions update placeholder UI state only in Phase 5.
- Related entity links route to Product, Team, Employee, Market, Competitor, Contract, Research, or Finance screens.
- Report text must explain outcomes without prescribing perfect solutions.

## 8. Required States
- Empty inbox state.
- Filtered empty state.
- Unread report state.
- Requires decision state.
- Archived state.
- Deleted placeholder state.
- Missing related entity state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/ReportsInbox/ReportsInboxScreen.uxml`
- `Assets/Project/UI/USS/Screens/ReportsInbox/ReportsInboxScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/ReportsInbox/ReportsInboxView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/ReportsInbox/ReportsInboxController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/ReportsInbox/ReportsInboxViewModel.cs`

Optional helper ViewModels, if needed:
- `ReportCategoryViewModel.cs`
- `ReportRowViewModel.cs`
- `ReportDetailViewModel.cs`
- `ReportActionViewModel.cs`
- `ReportRelatedEntityViewModel.cs`

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
- `ReportCategoryViewModel`
- `ReportRowViewModel`
- `ReportDetailViewModel`
- `ReportActionViewModel`
- `ReportRelatedEntityViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `reports-inbox`
- `reports-inbox__header`
- `reports-inbox__toolbar`
- `reports-inbox__tabs`
- `reports-inbox__content`
- `reports-inbox__panel`
- `reports-inbox__row`
- `reports-inbox__card`
- `reports-inbox__drawer`
- `reports-inbox__modal`
- `reports-inbox__footer`

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
Reports should explain outcomes, changes, causes, and related links. They must not say “You should hire QA” or prescribe the best strategy.
