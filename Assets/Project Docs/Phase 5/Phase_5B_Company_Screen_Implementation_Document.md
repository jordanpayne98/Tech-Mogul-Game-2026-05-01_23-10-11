# Phase 5B — Company Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.company`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_16.4, GDD_03, Appendix A.1
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the company profile screen showing identity, founder/founding team summary, reputation/status, focus, milestones, and company history.

## 2. Scope
- Company identity card and profile fields.
- Reputation/status cards using semantic status presentation.
- Founder/founding team summary card.
- Company focus display as identity/preference.
- Milestones/history list with placeholder data support.

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
- ScreenHeader with company icon, title “Company”, and profile subtitle.
- Top profile row: identity card, reputation/status card, and founder/founding team card.
- Middle section: company focus, current standing, and high-level metrics.
- Bottom section: milestones/history list.
- Optional right drawer for company identity edit placeholder.

## 6. Required Screen Content
### Identity fields
- Company name
- Company ID
- Founded date
- Headquarters
- Logo/icon
- Company colour
- Starting/current focus

### Status fields
- Reputation summary
- Company stage
- Current health state
- Major milestones count
- Active product count
- Employee count

### History items
- Company founded
- First hire
- First team created
- First contract accepted
- First product launched
- Major report milestones

## 7. Interactions
- Edit identity opens an edit modal or drawer placeholder.
- Milestone rows open a detail modal if detail data exists.
- Founder/founding team card links to the relevant profile or setup data when available.
- Focus display must not imply mechanical bonuses unless later explicitly implemented.

## 8. Required States
- No milestones yet state.
- Missing company profile data state.
- Archived/old milestone state.
- Reputation unknown state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/Company/CompanyScreen.uxml`
- `Assets/Project/UI/USS/Screens/Company/CompanyScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/Company/CompanyView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Company/CompanyController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/Company/CompanyViewModel.cs`

Optional helper ViewModels, if needed:
- `CompanyIdentityViewModel.cs`
- `CompanyStatusCardViewModel.cs`
- `CompanyFounderSummaryViewModel.cs`
- `CompanyMilestoneRowViewModel.cs`

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
- `CompanyIdentityViewModel`
- `CompanyStatusCardViewModel`
- `CompanyFounderSummaryViewModel`
- `CompanyMilestoneRowViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `company`
- `company__header`
- `company__toolbar`
- `company__tabs`
- `company__content`
- `company__panel`
- `company__row`
- `company__card`
- `company__drawer`
- `company__modal`
- `company__footer`

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
Company focus/specialisation is identity and preference display only. Do not show it as a mechanical bonus source unless a later GDD update changes that.
