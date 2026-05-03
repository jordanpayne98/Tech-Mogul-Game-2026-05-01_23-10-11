# Phase 5J — Market Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.market`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_10, GDD_16.4, GDD_18.2
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the market overview screen showing market categories, demand, growth, trends, customer preferences, rankings, recent launches, and player position.

## 2. Scope
- Market category grid/list.
- Trend strip.
- Market detail table.
- Category detail drawer/modal.
- Placeholder/static market ViewModel support.

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
- ScreenHeader with title “Market”.
- Top trend/opportunity strip.
- Market category cards or table.
- Detail table for selected category.
- Right drawer/modal for category detail.
- Filter/search toolbar.

## 6. Required Screen Content
### Category fields
- Category name
- Demand
- Growth rate
- Competitive intensity
- Current leaders
- Technology expectations
- Price sensitivity
- Support expectations
- Trend modifiers

### Customer segments
- Consumers
- Small businesses
- Enterprises
- Developers
- Gamers
- Creators
- Education
- Government
- Hardware enthusiasts
- Budget buyers
- Premium buyers

### Reports/rows
- Product rankings
- Average price
- Preference shifts
- Opportunity/risk indicators
- Recent launches
- Player company position

## 7. Interactions
- Category click opens detail drawer/modal.
- Recent launch link routes to competitor/product detail when available.
- Player position links to product portfolio or Product Detail.
- Trend item opens report detail if a related report exists.
- No market simulation logic is implemented in Presentation.

## 8. Required States
- No market data loaded state.
- Unknown/hidden market state.
- High growth info state.
- High competition warning state.
- Player not present in market state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Market/MarketScreen.uxml`
- `Assets/Project/UI/USS/Screens/Market/MarketScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Market/MarketView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Market/MarketController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Market/MarketViewModel.cs`

Optional helper ViewModels, if needed:
- `MarketCategoryRowViewModel.cs`
- `MarketTrendViewModel.cs`
- `MarketDetailViewModel.cs`
- `MarketRankingRowViewModel.cs`
- `CustomerPreferenceViewModel.cs`

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
- `MarketCategoryRowViewModel`
- `MarketTrendViewModel`
- `MarketDetailViewModel`
- `MarketRankingRowViewModel`
- `CustomerPreferenceViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `market`
- `market__header`
- `market__toolbar`
- `market__tabs`
- `market__content`
- `market__panel`
- `market__row`
- `market__card`
- `market__drawer`
- `market__modal`
- `market__footer`

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
Market Screen shows data, not recommendations. It must not tell the player what to build.
