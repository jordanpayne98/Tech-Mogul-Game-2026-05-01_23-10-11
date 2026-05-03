# Phase 5K — Competitors Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.competitors`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_10.5-GDD_10.7, GDD_16.4, GDD_18.5
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the competitors screen for competitor companies, archetypes, product portfolios, recent launches, market positions, and comparison entry points.

## 2. Scope
- Competitor table/list.
- Competitor detail drawer/modal.
- Competitor product list.
- Comparison entry points.
- Search/filter toolbar.
- Placeholder/static competitor ViewModel support.

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
- ScreenHeader with title “Competitors”.
- Toolbar with search, archetype/market filters, and status filters.
- Competitor table.
- Right competitor detail drawer.
- Competitor product list inside drawer or modal.
- Comparison action routes to Product Detail/Market when available.

## 6. Required Screen Content
### Columns
- Company Name
- Archetype
- Main Market
- Reputation
- Product Count
- Recent Launch
- Market Position
- Trend
- Actions

### Archetypes
- Incumbent Giant
- Aggressive Startup
- Research Lab
- Hardware Manufacturer
- Enterprise Specialist
- Consumer Brand
- Low-Cost Competitor
- Platform Holder

### Detail sections
- Company summary
- Product portfolio
- Recent launches
- Market focus
- Known strengths
- Known risks
- Related reports

## 7. Interactions
- Competitor row opens detail drawer/modal.
- Competitor product row opens product comparison/detail placeholder.
- Recent launch links to report detail if available.
- Filter/search update placeholder list state.
- No advanced competitor AI decisions are implemented in Phase 5.

## 8. Required States
- No competitors generated state.
- No products known state.
- Recent launch info state.
- Dominant competitor warning state.
- Unknown competitor data state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Competitors/CompetitorsScreen.uxml`
- `Assets/Project/UI/USS/Screens/Competitors/CompetitorsScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Competitors/CompetitorsView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Competitors/CompetitorsController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Competitors/CompetitorsViewModel.cs`

Optional helper ViewModels, if needed:
- `CompetitorRowViewModel.cs`
- `CompetitorFilterViewModel.cs`
- `CompetitorDetailViewModel.cs`
- `CompetitorProductRowViewModel.cs`
- `CompetitorComparisonLinkViewModel.cs`

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
- `CompetitorRowViewModel`
- `CompetitorFilterViewModel`
- `CompetitorDetailViewModel`
- `CompetitorProductRowViewModel`
- `CompetitorComparisonLinkViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `competitors`
- `competitors__header`
- `competitors__toolbar`
- `competitors__tabs`
- `competitors__content`
- `competitors__panel`
- `competitors__row`
- `competitors__card`
- `competitors__drawer`
- `competitors__modal`
- `competitors__footer`

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
Competitors make the world feel alive. Phase 5 presents competitor data and comparison hooks only.
