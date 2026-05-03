# Phase 5D — Recruitment Hub Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source
**Screen route ID:** `screen.recruitment`
**Runtime UI:** Unity UI Toolkit only
**Canvas/UGUI allowed:** No
**Primary GDD references:** GDD_04.6-GDD_04.9, GDD_16.4, Appendix A.3
**Depends on:** Phase 4 UI Shell, theme foundation, routing, modal layer, and reusable component foundation

## 1. Purpose
Build the recruitment hub for candidate browsing, shortlists, job posts, offers, and candidate details.

## 2. Scope
- Candidate pool list/table.
- Shortlist and offer status tabs.
- Job post section placeholder.
- Candidate detail modal.
- Offer modal placeholder.
- Search/filter toolbar and right filter drawer.

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
- ScreenHeader with title “Recruitment Hub”.
- Tabs for candidate workflow states.
- Toolbar with role/category dropdown, search, filters, and create job post action.
- Candidate table or card/table hybrid.
- Right filter drawer.
- Candidate detail modal and offer modal.

## 6. Required Screen Content
### Tabs
- Candidate Pool
- Shortlist
- Job Posts
- Offers Sent
- Accepted
- Rejected

### Candidate columns
- Name
- Role
- Seniority
- Salary Expectation
- Visible Skills
- Potential Estimate
- Availability
- Interest
- Offer Status
- Confidence

### Filters
- Role
- Seniority
- Salary range
- Availability
- Interest
- Skill tags
- Offer status
- Confidence level

### Candidate detail sections
- Profile
- Known information
- Interview/reveal status
- Skills
- Traits confidence
- Offer history

## 7. Interactions
- Candidate row opens Candidate Detail modal.
- Shortlist action toggles placeholder shortlist state.
- Send Offer opens Offer modal.
- Create Job Post opens job post modal placeholder.
- Team assignment target may be displayed but final assignment must be handled after hiring through Application/Core.

## 8. Required States
- No candidates available state.
- Candidate information uncertainty state with hidden/unknown chips.
- Offer pending state.
- Offer accepted/rejected state.
- Filtered empty state.

## 9. Architecture Rules
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.
- All persistent references must use stable IDs such as screen.founder_portal, modal.employee_profile, product.example, report.example.

## 10. Required Files
- `Assets/Project/UI/UXML/Screens/RecruitmentHub/RecruitmentHubScreen.uxml`
- `Assets/Project/UI/USS/Screens/RecruitmentHub/RecruitmentHubScreen.uss`
- `Assets/Project/Scripts/Presentation/UI/Screens/RecruitmentHub/RecruitmentHubView.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/RecruitmentHub/RecruitmentHubController.cs`
- `Assets/Project/Scripts/Presentation/UI/Screens/RecruitmentHub/RecruitmentHubViewModel.cs`

Optional helper ViewModels, if needed:
- `CandidateRowViewModel.cs`
- `CandidateFilterViewModel.cs`
- `CandidateProfileViewModel.cs`
- `OfferViewModel.cs`
- `JobPostRowViewModel.cs`

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
- `CandidateRowViewModel`
- `CandidateFilterViewModel`
- `CandidateProfileViewModel`
- `OfferViewModel`
- `JobPostRowViewModel`

## 12. Styling and Naming Rules
Use clear BEM-style classes. Recommended class roots:
- `recruitment-hub`
- `recruitment-hub__header`
- `recruitment-hub__toolbar`
- `recruitment-hub__tabs`
- `recruitment-hub__content`
- `recruitment-hub__panel`
- `recruitment-hub__row`
- `recruitment-hub__card`
- `recruitment-hub__drawer`
- `recruitment-hub__modal`
- `recruitment-hub__footer`

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
Recruitment shows partial candidate information. It must not reveal hidden candidate data as certain unless the ViewModel explicitly marks it as revealed.
