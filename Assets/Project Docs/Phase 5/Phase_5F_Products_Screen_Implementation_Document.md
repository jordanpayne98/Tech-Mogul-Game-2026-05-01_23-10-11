# Phase 5F — Products Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.products`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_06-GDD_08, GDD_16.4, Appendix A.5
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the product portfolio overview screen for browsing products, statuses, revenue, users, quality, support load, and product creation entry points.

## 2. Scope
- Product portfolio table/cards.
- Product status tabs.
- Search/filter toolbar.
- Create Product entry point.
- Product quick summary drawer.
- Placeholder/static product ViewModel support.

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
- ScreenHeader with title “Products”.
- Tabs by product status/category.
- Toolbar with search, filters, product family dropdown, and Create Product button.
- Product portfolio table or card/table hybrid.
- Product quick summary drawer on selection.
- Route to Product Detail full screen.

## 6. Required Screen Content
### Tabs
- All Products
- In Development
- Ready for Launch
- Launched
- Supported
- Cancelled / Sunset

### Columns
- Product Name
- Family
- Type
- Status
- Assigned Team
- Phase
- Release Target
- Review Score
- Recent Review Score
- Revenue This Month
- Active Users / Units
- Support Load
- Actions

### Software MVP types
- Web Platform
- Productivity App
- Business SaaS
- Developer Tool
- Game

### Hardware MVP types
- Peripheral
- Laptop/Desktop Device
- Server Device

## 7. Interactions
- Create Product opens Product Creation wizard entry point or placeholder modal.
- Product row opens quick summary drawer or navigates to Product Detail.
- Status chips link to relevant product detail tab where possible.
- No product phase transitions occur directly from the portfolio table in Phase 5.

## 8. Required States
- No products yet state.
- Filtered empty state.
- Ready-for-launch warning/decision state.
- High support load warning state.
- Cancelled/sunset muted state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Products/ProductsScreen.uxml`
- `Assets/Project/UI/USS/Screens/Products/ProductsScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Products/ProductsView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Products/ProductsController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Products/ProductsViewModel.cs`

Optional helper ViewModels, if needed:
- `ProductRowViewModel.cs`
- `ProductFilterViewModel.cs`
- `ProductSummaryDrawerViewModel.cs`
- `ProductStatusChipViewModel.cs`

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
- `ProductRowViewModel`
- `ProductFilterViewModel`
- `ProductSummaryDrawerViewModel`
- `ProductStatusChipViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `products`
- `products__header`
- `products__toolbar`
- `products__tabs`
- `products__content`
- `products__panel`
- `products__row`
- `products__card`
- `products__drawer`
- `products__modal`
- `products__footer`

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
Products Screen is portfolio-level only. Deep management belongs in Product Detail.
