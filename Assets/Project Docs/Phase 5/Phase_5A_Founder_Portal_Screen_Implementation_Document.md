# Phase 5A — Founder Portal Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.founder_portal`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_14.2, GDD_16.4, GDD_18.2
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the command-centre dashboard that gives the Founder a readable overview of company health, active work, inbox pressure, upcoming decisions, and recent activity.

## 2. Scope
- Production-ready Founder Portal dashboard UI.
- Dashboard KPI cards, summary cards, recent activity strip, up-next card, and quick action entry points.
- Placeholder/static ViewModel support for Phase 5.
- Navigation hooks to related screens and reports.

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
- ScreenHeader with icon tile, title “Founder Portal” or “Main Dashboard”, and short subtitle.
- KPIGrid across the top with four to six cards.
- MainDashboardGrid below the KPI row, split into larger operational cards and a right-side column.
- RecentActivityStrip near the bottom.
- Optional QuickActions card in the right column.

## 6. Required Screen Content
### KPI cards
- Cash / Available Funds
- Net This Month / Burn Rate
- Runway
- Revenue Month-to-Date
- Active Alerts
- Active Products

### Main cards
- Inbox / Requires Decision
- Active Products / Project Progress
- Team Workload
- Hiring Pipeline
- Upcoming Milestones
- Market / Competitor News
- Infrastructure / Support
- Recent Activity

### Quick actions
- New Product
- Hire Employee
- Create Contract
- View Reports
- Market Overview
- Settings

## 7. Interactions
- Clicking a KPI card routes to its detailed screen or report list.
- Clicking a report/inbox item opens report detail through the Reports/Inbox system.
- Quick actions route to the target screen or open the relevant creation modal/wizard if it exists.
- No dashboard card may directly mutate simulation state in Phase 5.

## 8. Required States
- Empty company data state for new/placeholder saves.
- Loading state while ViewModel is unavailable.
- Error state if required data fails to bind.
- Warning states for low runway, high workload, unread decision reports, and support/infrastructure risk.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/FounderPortal/FounderPortalScreen.uxml`
- `Assets/Project/UI/USS/Screens/FounderPortal/FounderPortalScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/FounderPortal/FounderPortalView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/FounderPortal/FounderPortalController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/FounderPortal/FounderPortalViewModel.cs`

Optional helper ViewModels, if needed:
- `KpiCardViewModel.cs`
- `DashboardCardViewModel.cs`
- `RecentActivityItemViewModel.cs`
- `QuickActionViewModel.cs`
- `UpNextItemViewModel.cs`

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
- `KpiCardViewModel`
- `DashboardCardViewModel`
- `RecentActivityItemViewModel`
- `QuickActionViewModel`
- `UpNextItemViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `founder-portal`
- `founder-portal__header`
- `founder-portal__toolbar`
- `founder-portal__tabs`
- `founder-portal__content`
- `founder-portal__panel`
- `founder-portal__row`
- `founder-portal__card`
- `founder-portal__drawer`
- `founder-portal__modal`
- `founder-portal__footer`

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
Founder Portal is the main command centre. It shows summaries and navigation only. Detailed work belongs on the relevant screen.
