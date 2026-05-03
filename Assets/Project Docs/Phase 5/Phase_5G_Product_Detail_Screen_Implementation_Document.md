# Phase 5G — Product Detail Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.product_detail`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_06-GDD_08, GDD_16.4, GDD_15.3-GDD_15.4
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the full product management surface for a single product, including lifecycle, assigned teams, quality/risk, budget, support, reports, history, and competitor context.

## 2. Scope
- Full Product Detail screen layout.
- Product header and status/action bar.
- Tabs for major product areas.
- Right summary/risk panel.
- Placeholder Product Detail ViewModel.
- Navigation back to Products screen.

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
- Product header with product icon, name, type, status, market, and key metadata.
- Status/action bar below header.
- Horizontal tab bar.
- Main content region for active tab.
- Right summary/risk panel fixed or sticky within content.
- Back/breadcrumb link to Products.

## 6. Required Screen Content
### Tabs
- Overview
- Development
- Quality
- Budget
- Marketing
- Support
- Reports
- History
- Competitors

### Header fields
- Product name
- Product ID
- Family
- Type
- Status
- Target market
- Customer segment
- Price model
- Launch target/date

### Overview cards
- Current phase
- Assigned teams
- Progress
- Revenue/users
- Support load
- Market position

### Quality fields
- Overall Score
- Quality
- Creativity
- Stability
- Bug risk
- QA confidence
- Infrastructure readiness
- Support readiness

### Budget fields
- Development budget
- Pre-launch marketing monthly budget
- Post-launch marketing monthly budget
- Post-launch support monthly budget

## 7. Interactions
- Tabs switch local detail content.
- Reports tab links to Reports/Inbox detail.
- Competitors tab links to competitor/product comparison.
- Launch/Delay/Cancel actions are placeholders unless Application/Core wiring exists.
- No product lifecycle state changes are implemented in Presentation.

## 8. Required States
- Missing product state.
- No assigned team state.
- In-development state.
- Ready-for-launch decision state.
- Launched supported state.
- Cancelled/sunset state.
- High risk warning state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/ProductDetail/ProductDetailScreen.uxml`
- `Assets/Project/UI/USS/Screens/ProductDetail/ProductDetailScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/ProductDetail/ProductDetailView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/ProductDetail/ProductDetailController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/ProductDetail/ProductDetailViewModel.cs`

Optional helper ViewModels, if needed:
- `ProductDetailViewModel.cs`
- `ProductDetailTabViewModel.cs`
- `ProductRiskSummaryViewModel.cs`
- `ProductMetricCardViewModel.cs`
- `ProductHistoryRowViewModel.cs`

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
- `ProductDetailViewModel`
- `ProductDetailTabViewModel`
- `ProductRiskSummaryViewModel`
- `ProductMetricCardViewModel`
- `ProductHistoryRowViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `product-detail`
- `product-detail__header`
- `product-detail__toolbar`
- `product-detail__tabs`
- `product-detail__content`
- `product-detail__panel`
- `product-detail__row`
- `product-detail__card`
- `product-detail__drawer`
- `product-detail__modal`
- `product-detail__footer`

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
Product Detail is a full screen, not a modal. It is the deep management surface for one product.
