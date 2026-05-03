# Phase 5 Remaining Implementation Documents — Index

This package contains all remaining Phase 5 implementation documents after Phase 5P Company Creation was created separately.

## Included documents
- 5A — Founder Portal Screen: `Phase_5A_Founder_Portal_Screen_Implementation_Document.docx` / `Phase_5A_Founder_Portal_Screen_Implementation_Document.md`
- 5B — Company Screen: `Phase_5B_Company_Screen_Implementation_Document.docx` / `Phase_5B_Company_Screen_Implementation_Document.md`
- 5C — Employees Screen: `Phase_5C_Employees_Screen_Implementation_Document.docx` / `Phase_5C_Employees_Screen_Implementation_Document.md`
- 5D — Recruitment Hub Screen: `Phase_5D_Recruitment_Hub_Screen_Implementation_Document.docx` / `Phase_5D_Recruitment_Hub_Screen_Implementation_Document.md`
- 5E — Teams Screen: `Phase_5E_Teams_Screen_Implementation_Document.docx` / `Phase_5E_Teams_Screen_Implementation_Document.md`
- 5F — Products Screen: `Phase_5F_Products_Screen_Implementation_Document.docx` / `Phase_5F_Products_Screen_Implementation_Document.md`
- 5G — Product Detail Screen: `Phase_5G_Product_Detail_Screen_Implementation_Document.docx` / `Phase_5G_Product_Detail_Screen_Implementation_Document.md`
- 5H — Contracts Screen: `Phase_5H_Contracts_Screen_Implementation_Document.docx` / `Phase_5H_Contracts_Screen_Implementation_Document.md`
- 5I — Finance Screen: `Phase_5I_Finance_Screen_Implementation_Document.docx` / `Phase_5I_Finance_Screen_Implementation_Document.md`
- 5J — Market Screen: `Phase_5J_Market_Screen_Implementation_Document.docx` / `Phase_5J_Market_Screen_Implementation_Document.md`
- 5K — Competitors Screen: `Phase_5K_Competitors_Screen_Implementation_Document.docx` / `Phase_5K_Competitors_Screen_Implementation_Document.md`
- 5L — Research Screen: `Phase_5L_Research_Screen_Implementation_Document.docx` / `Phase_5L_Research_Screen_Implementation_Document.md`
- 5M — Reports / Inbox Screen: `Phase_5M_Reports__Inbox_Screen_Implementation_Document.docx` / `Phase_5M_Reports__Inbox_Screen_Implementation_Document.md`
- 5N — Calendar Screen: `Phase_5N_Calendar_Screen_Implementation_Document.docx` / `Phase_5N_Calendar_Screen_Implementation_Document.md`
- 5O — Settings Screen: `Phase_5O_Settings_Screen_Implementation_Document.docx` / `Phase_5O_Settings_Screen_Implementation_Document.md`

## Global rules
- Use the locked dark founder dashboard visual language: deep blue-black background, dark navy raised panels, charcoal surfaces, subtle blue-grey borders, off-white primary text, muted grey-blue secondary text, and restrained teal/cyan accents.
- The UI must feel like a premium desktop management dashboard for a serious technology-company simulation. It must not look like default Unity UI, mobile-first UI, toy-like tycoon UI, cyberpunk neon overload, or flat black debug panels.
- All in-game Phase 5 screens use the Phase 4 MainGameShell unless this document explicitly says otherwise: top status bar, left sidebar navigation, central screen host, optional right drawer, modal layer, tooltip layer, notification layer, and debug layer.
- Every screen must be information-first. Show values, status, trend, timeframe, and drill-down links where relevant. Do not prescribe an optimal strategy.
- View owns VisualElement references, local display state, callbacks, and semantic class toggles only.
- Controller/Presenter handles UI interactions, screen-local validation, routing requests, modal/drawer requests, and placeholder command hooks. It must not contain core simulation rules.
- ViewModel contains display-ready text, row data, button states, selected IDs, visibility flags, semantic visual states, warning text, and drill-down route IDs. It must not hold VisualElement references, UIDocument references, raw colours, raw pixel values, save data, or scene references.
- State-changing actions must route through Application/Core once Phase 6 wiring exists. Phase 5 may use placeholder/static ViewModels and command stubs only.

Phase 5 builds production-ready UI surfaces with placeholder/static ViewModels. Phase 6 connects those screens to Application/Core logic.