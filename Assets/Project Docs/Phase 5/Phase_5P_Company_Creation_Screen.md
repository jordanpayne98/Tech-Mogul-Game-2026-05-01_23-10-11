# Phase 5P — Company Creation Screen Implementation Document

**Status:** Locked Phase 5 UI implementation source  
**Screen:** Company Creation / New Game Setup  
**Phase:** Phase 5 — MVP Screens  
**Plan ID:** 5P  
**Depends On:** Phase 4 UI Shell, theme foundation, component foundation, screen/modal routing foundation  
**Primary references:** Project Rules, Coding Standard, UI Architecture, Colour Palette, Typography, UI Sizing and Spacing Standard, Global UI Visual Reference, Tech Mogul GDD, Roadmap  
**Implementation type:** UI Toolkit screen implementation  
**Runtime UI system:** Unity UI Toolkit only  
**Canvas/UGUI allowed:** No  

---

## 1. Purpose

The Company Creation Screen is the first major screen the player uses when starting a new game.

It creates the player’s initial company identity, founder/founding team setup, background flavour, starting cash preset, sandbox settings, and final review before entering the playable company simulation.

This screen must feel like a **premium dark founder setup flow**, not a plain Unity form.

The intended experience is:

```text
The player is setting up a new technology startup.
The screen feels focused, professional, dark, modern, and founder-facing.
The screen uses a structured wizard layout with a clear left-side stepper, central setup form, right-side live summary, and fixed footer actions.
The UI looks complete and production-ready even before Phase 6 wiring connects it to real simulation logic.
```

---

## 2. Scope

This document defines the **Phase 5P UI implementation** for the Company Creation Screen.

Phase 5P must implement:

```text
Full-screen Company Creation screen layout
Vertical stepper wizard navigation
Central form/content panel
Right-side live summary/preview panel
Footer action row
Step validation display
Placeholder/static ViewModels
UI Toolkit UXML/USS files
View, Controller/Presenter, and ViewModel files
All required visual states
All required empty/default states
All required disabled/invalid states
```

Phase 5P must not implement:

```text
Final company creation simulation logic
Final save-game creation
Final market generation
Final competitor generation
Final candidate generation
Final employee/team generation
Final finance simulation
Loans
Emergency funding
Equity mechanics
Founder life-sim mechanics
Runtime Canvas/UGUI
One-off colours
One-off fonts
One-off spacing
Hard-coded styling in C#
```

---

## 3. Core Design Rule

```text
Company Creation is a full-screen setup wizard.
It is not part of the in-game dashboard shell.
It does not show the in-game sidebar.
It does not show the in-game top status bar.
It does not show cash/runway/Continue controls.
It does not show active reports, inbox, or time controls.
```

Reason:

```text
The player has not created a company yet.
There is no current cash/runway/time/company state to display.
The screen must feel like a focused setup flow before the main game begins.
```

---

## 4. Visual Target

The screen must look like a **dark professional startup founder setup interface**.

Target visual qualities:

```text
Premium dark SaaS-style setup screen
Founder / startup onboarding flow
Modern executive dashboard visual language
Dark navy / blue-black background
Dark raised cards and panels
Thin blue-grey borders
Teal/cyan primary accent
Off-white primary text
Muted grey-blue secondary text
Clear form hierarchy
Large readable title
Spacious but dense enough for desktop
No mobile app styling
No default Unity grey panels
No bright arcade colours
No cyberpunk neon overload
```

The screen should feel like it belongs to the same UI family as:

```text
Founder Portal
Dashboard
Employee list
Employee detail modal
Main menu
```

But it should use a **focused setup layout**, not the normal in-game shell.

---

## 5. Screen Layout Overview

Use this exact high-level structure:

```text
CompanyCreationScreen
|-- BackgroundLayer
|-- ScreenFrame
|   |-- HeaderArea
|   |   |-- ScreenTitleBlock
|   |   `-- CloseButton
|   |
|   |-- ContentArea
|   |   |-- LeftStepperPanel
|   |   |-- MainFormPanel
|   |   `-- RightSummaryPanel
|   |
|   `-- FooterArea
|       |-- BackButton
|       |-- HelperText / ValidationMessage
|       `-- NextOrConfirmButton
```

At 1920×1080, the screen should read visually as:

```text
Top-left:
- Large page title
- Short subtitle

Top-right:
- Close X button

Left:
- Vertical numbered stepper
- Current step highlighted in teal

Centre:
- Main dark raised form panel
- Current step content

Right:
- Live summary / preview panel
- Updates as selections are made

Bottom-left:
- Back button

Bottom-centre:
- Helper or validation text

Bottom-right:
- Next button or Confirm Create Company button
```

---

## 6. Layout Dimensions and Positioning

### 6.1 Root

The root should fill the screen.

```text
Width: 100%
Height: 100%
Primary target resolution: 1920×1080
Minimum practical resolution: 1366×768
```

Use flexible UI Toolkit layout, not fragile absolute positioning for the main layout.

### 6.2 Outer padding

At 1920×1080:

```text
Left/right outer padding: 48px to 64px
Top padding: 44px to 56px
Bottom padding: 40px to 48px
```

Use the project spacing tokens where available.

### 6.3 Header area

```text
Height: approximately 96px to 120px
Layout: horizontal
Title block left
Close button right
```

Title block:

```text
Title font: Space Grotesk / secondary display font
Title size: large display/title size
Title weight: bold/semi-bold
Subtitle font: Inter / primary UI font
Subtitle size: body/heading size
Subtitle colour: secondary text
```

Example title:

```text
Create Your Company
```

Example subtitle:

```text
Define your startup identity, founder profile, and sandbox setup.
```

### 6.4 Content area

Content area should fill the space between header and footer.

At 1920×1080:

```text
Left stepper width: 260px to 300px
Main form panel width: 900px to 980px
Right summary panel width: 460px to 520px
Gap between columns: 24px
```

Approximate layout:

```text
[Left Stepper 280px] [Main Form Panel flexible / approx 940px] [Summary Panel approx 500px]
```

### 6.5 Footer area

```text
Height: 80px to 96px
Back button left
Validation/helper text centre
Next/Confirm button right
```

Buttons:

```text
Back button: secondary, large, approx 200px × 56px
Next button: primary, large, approx 240px × 56px
Confirm Create Company button: primary, large, approx 280px × 56px
```

---

## 7. Background

Use a dark, premium, low-distraction background.

Required look:

```text
Deep blue-black / near-black background
Subtle vignette around edges
Optional very subtle radial gradient behind main form area
Optional very subtle abstract panel glow
No bright image behind forms
No noisy decorative texture behind text
No pure black flat background
```

Recommended visual structure:

```text
Root background: almost black blue/navy
Screen frame: transparent/flex layout
Panels: raised dark navy/charcoal
```

---

## 8. Header Area

### 8.1 Screen title block

Top-left content:

```text
Create Your Company
Define your startup identity, founder profile, and sandbox setup.
```

Rules:

```text
Title must be large and readable.
Subtitle must be muted but readable.
Title must not be centred.
Title must align with the main content grid.
```

### 8.2 Close button

Top-right:

```text
Square icon button
Shows X icon
Dark raised background
Subtle border
Hover brightens background and border
```

Behaviour:

```text
Clicking close opens a confirmation modal if unsaved setup progress exists.
If no setup progress exists, it may return to Main Menu directly.
Phase 5 may implement placeholder callback only.
```

Do not use a tiny close icon with no hit area.

---

## 9. Left Stepper Panel

### 9.1 Purpose

The left stepper shows the player’s progress through setup.

It should make the flow feel structured and premium.

### 9.2 Layout

Use a vertical stepper inside a raised dark panel.

```text
Panel width: 260px to 300px
Panel background: dark raised panel
Panel border: subtle blue-grey
Panel corner radius: standard/large
Panel padding: 16px to 24px
Stepper direction: vertical
```

### 9.3 Step list

Use these exact steps:

```text
1. Company
2. Background
3. Founders
4. Founder Details
5. Team & Budget
6. Sandbox Setup
7. Review
```

### 9.4 Step row structure

Each step row:

```text
Number circle
Step label
Optional state icon/marker
Vertical connector line to next step
```

Example:

```text
[1] Company
 |
[2] Background
 |
[3] Founders
 |
[4] Founder Details
 |
[5] Team & Budget
 |
[6] Sandbox Setup
 |
[7] Review
```

### 9.5 Step states

Use these states:

```text
Complete
Active
Available
Future
Invalid
Disabled
```

Visual rules:

```text
Active step:
- Teal/cyan filled number circle
- Dark/inverse number text
- Teal label
- Teal border or glow on row/panel
- Slight dark teal background

Complete step:
- Teal outline or muted teal filled circle
- Optional check mark
- Label brighter than future steps
- Connector line above/below may be teal

Available step:
- Dark circle
- Subtle border
- Secondary text

Future step:
- Muted circle
- Muted text
- No strong hover

Invalid step:
- Warning or danger marker
- Do not rely on colour alone
- Include icon or validation marker
```

### 9.6 Navigation behaviour

Phase 5 behaviour:

```text
Player can click completed or current steps.
Player cannot skip to invalid future steps unless explicitly allowed.
Future invalid steps should look unavailable.
```

If clicked future step is unavailable:

```text
Show validation helper text in footer.
Do not silently ignore click.
```

---

## 10. Main Form Panel

### 10.1 Purpose

The main form panel contains the current step’s interactive content.

### 10.2 Visual style

```text
Raised dark navy/charcoal panel
Subtle 1px blue-grey border
Soft shadow
Large border radius
Internal padding: 24px to 32px
```

The panel should feel like a polished desktop application card, not a raw form.

### 10.3 Internal structure

Each step should use:

```text
Step heading
Step subtitle/help text
Main content section
Validation area if needed
```

Example:

```text
Company Identity
Choose the basic identity for your new technology company.
```

### 10.4 Form controls

Use project-standard controls:

```text
Text input
Dropdown
Choice cards
Toggle cards
Slider/range if needed
Seed input
Colour selector placeholder
Icon/logo selector placeholder
```

Do not use Unity default styling.

### 10.5 Form layout

Use two-column forms where sensible.

Example:

```text
[Company Name]        [Headquarters]
[Industry Focus]      [Company Colour]
[Logo/Icon]           [Optional short name]
```

Rules:

```text
Labels sit above fields.
Inputs are 34px or 40px high according to sizing standard.
Inputs use dark background, subtle border, readable text.
Focused input uses teal/cyan border.
Invalid input uses danger border plus validation text.
```

---

## 11. Right Summary Panel

### 11.1 Purpose

The right summary panel gives the player a live preview of their setup choices.

It must not recommend the best option. It should only summarize selected values, warnings, and consequences.

### 11.2 Visual style

```text
Raised dark panel
Slightly darker or slightly different background than main form
Subtle border
Large border radius
Internal padding: 24px to 32px
```

### 11.3 Summary panel title

Use:

```text
Company Summary
```

or on founder-focused steps:

```text
Founder Summary
```

But the panel should remain in the same right-side location throughout the wizard.

### 11.4 Summary rows

Use label/value rows:

```text
Company Name          Not set
Focus                 Not selected
Headquarters          Not selected
Background            Will be defined
Founder Setup         Will be selected
Starting Cash         £50,000
Sandbox               Standard
```

Labels:

```text
Secondary muted text
```

Values:

```text
Primary text if set
Muted text if not set
Warning colour if risky
Danger colour only for severe/invalid setup
```

### 11.5 Empty state

Before selections:

```text
Select options to build your company summary.
```

### 11.6 Warning area

Warnings should appear as small status rows or chips.

Examples:

```text
Low starting cash increases early pressure.
High competitor density increases market pressure.
Hardcore mode can end the save.
```

Rules:

```text
Warnings must use warning chip/card styling.
Warnings must include text, not colour alone.
Do not show fake precise formulas.
Do not tell the player what to choose.
```

---

## 12. Footer Area

### 12.1 Layout

```text
Back button left
Helper/validation text centre
Next/Confirm button right
```

### 12.2 Back button

Style:

```text
Secondary button
Large height
Left arrow icon
Text: Back
```

Behaviour:

```text
Disabled on Step 1, or returns to main menu depending final UX decision.
For Phase 5, use disabled-on-step-1 unless main menu routing is already implemented.
```

### 12.3 Next button

Style:

```text
Primary teal filled button
Large height
Right arrow icon
Text: Next
```

Behaviour:

```text
Moves to next valid step.
Disabled if current step has required invalid fields.
```

### 12.4 Final button

On Step 7:

```text
Confirm Create Company
```

Style:

```text
Primary teal filled button
Wider than normal Next button
```

Behaviour:

```text
Phase 5: call placeholder callback or route to expected command stub if available.
Phase 6: connect to Application/Core company creation.
```

### 12.5 Validation text

Footer centre should show:

```text
Current step helper text
or first blocking validation message
```

Examples:

```text
Enter a company name to continue.
Select a founder setup option.
Review your setup before creating the company.
```

---

# 13. Step-by-Step Content Specification

## Step 1 — Company

### Purpose

Collect the company’s basic visible identity.

### Main heading

```text
Company Identity
```

### Subtitle

```text
Choose the name, focus, and headquarters for your new technology company.
```

### Fields

```text
Company Name
Industry Focus
Headquarters
Company Colour
Logo/Icon
```

### Field details

#### Company Name

```text
Required: Yes
Input type: Text input
Placeholder: Enter company name...
Validation:
- Cannot be empty
- Reasonable max length, e.g. 32 characters for display safety
- Must show validation if invalid
```

#### Industry Focus

```text
Required: Yes
Input type: Dropdown or choice card grid
Options:
- Consumer Software
- Enterprise SaaS
- Developer Tools
- Games & Entertainment
- Hardware Devices
- Cloud Infrastructure
- Security
- AI & Automation
- Platform Ecosystems
```

Important rule:

```text
Company focus is cosmetic/preference only in the current design.
It must not lock, boost, penalise, gate, or restrict gameplay.
```

#### Headquarters

```text
Required: Yes
Input type: Dropdown
Recommended first options:
- London, UK
- Manchester, UK
- Birmingham, UK
- Edinburgh, UK
- Remote-first
- Custom / TBD placeholder if needed
```

#### Company Colour

```text
Required: No
Input type: colour swatch selector placeholder
Must use approved palette-compatible colours only
No arbitrary neon colour picker unless later approved
```

#### Logo/Icon

```text
Required: No
Input type: icon selector placeholder
Use simple geometric placeholder icons
No image upload required in first implementation unless already supported
```

### Right summary content

```text
Company Name
Focus
Headquarters
Logo/Icon
Company Colour
```

---

## Step 2 — Background

### Purpose

Let the player choose the company’s starting flavour/background.

### Main heading

```text
Company Background
```

### Subtitle

```text
Choose the origin story for your startup. This shapes presentation and early identity.
```

### Choice cards

Use these options:

```text
Design Studio
Enterprise Consultancy
Game Studio
Growth Agency
Hardware Startup
Software Startup
```

### Choice card structure

Each card must include:

```text
Title
Short description
Founder tags/flavour tags
Starting strengths text
Starting risks text
Difficulty label
```

### Important implementation rule

```text
For the first version, background is flavour/setup presentation only unless explicitly given mechanics later.
Do not create hidden stat bonuses.
Do not create permanent penalties.
Do not gate products, contracts, employees, research, markets, or competitors.
```

### Example card wording

#### Design Studio

```text
A design-led startup focused on usability, polish, and customer-facing products.
Tags: Design, UX, Brand
Strengths: Strong identity and product presentation flavour.
Risks: May need technical hiring support early.
Difficulty: Standard
```

#### Enterprise Consultancy

```text
A service-first company with experience solving business problems for clients.
Tags: B2B, Operations, Clients
Strengths: Strong contract/company-service flavour.
Risks: Product identity may take longer to define.
Difficulty: Standard
```

#### Game Studio

```text
A creative software startup focused on entertainment and interactive products.
Tags: Games, Creativity, Community
Strengths: Strong creative-product flavour.
Risks: Market outcomes may be volatile.
Difficulty: Standard
```

#### Growth Agency

```text
A marketing-led startup focused on users, reach, and customer acquisition.
Tags: Marketing, Growth, Analytics
Strengths: Strong go-to-market flavour.
Risks: Needs strong product execution to retain users.
Difficulty: Standard
```

#### Hardware Startup

```text
A company attempting physical technology products from an early stage.
Tags: Hardware, Prototyping, Supply
Strengths: Strong hardware identity.
Risks: Hardware usually carries more cost and complexity.
Difficulty: Hard
```

#### Software Startup

```text
A product-led software company building digital tools and platforms.
Tags: Software, Product, Scale
Strengths: Flexible early product identity.
Risks: Competitive software markets.
Difficulty: Standard
```

### Right summary content

```text
Selected background
Tags
Strengths
Risks
Difficulty
```

---

## Step 3 — Founders

### Purpose

Choose whether the company starts with one founder or a founding team.

### Main heading

```text
Founder Setup
```

### Subtitle

```text
Choose whether your startup begins with a solo founder or a small founding team.
```

### Options

```text
Solo Founder
Co-Founders
```

### Solo Founder card

```text
Title: Solo Founder

Description:
You start with one founder identity and full control over the company direction.

Advantages:
+ Simpler setup
+ Clear company identity
+ Lower early leadership complexity
+ Faster early pivots

Trade-offs:
- Narrower starting skill coverage
- More pressure on one founder profile
- Less founding-team flavour
```

### Co-Founders card

```text
Title: Co-Founders

Description:
You start with a small founding team and define multiple founder profiles.

Advantages:
+ Broader founding identity
+ More roleplay variety
+ Shared startup story
+ Stronger early company flavour

Trade-offs:
- More setup required
- More profiles to define
- Equity/disagreement references are flavour only unless later implemented
```

### Important rule

```text
Equity is not a live first-version mechanic.
Founder disagreement is not a live first-version mechanic.
Any references to equity or disagreements must be treated as flavour/deferred text only.
```

### Right summary content

```text
Founder setup type
Number of founders
Pending founder details
```

---

## Step 4 — Founder Details

### Purpose

Define the founder or founding team identity.

### Main heading

```text
Founder Details
```

### Subtitle

```text
Define the founder profile that represents your startup’s leadership.
```

### Fields

```text
First Name
Last Name
Age
Nationality
Location
Founder Background
Primary Skill Profile
Optional Avatar/Portrait
```

### Field details

#### First Name / Last Name

```text
Required: Yes
Type: Text input
Validation: Cannot be empty
```

#### Age

```text
Required: Yes
Type: Number input or dropdown
Recommended valid range: 18 to 80
```

#### Nationality

```text
Required: Yes
Type: Dropdown
First implementation may include a limited list.
```

#### Location

```text
Required: Yes
Type: Dropdown
May default from company headquarters.
```

#### Founder Background

Use founder background options such as:

```text
Engineer
Product Designer
Sales Founder
Hardware Specialist
Research Founder
Serial Founder
Bootstrapped Founder
```

For Phase 5, display these as setup choices and summary text.

Do not implement live mechanical effects unless already defined in Core/Application.

#### Primary Skill Profile

Use readable choice cards, not raw stat editing.

Recommended options:

```text
Technical
Product
Commercial
Design
Hardware
Research
Operations
```

### Co-founder handling

If Co-Founders was selected:

```text
Do not create extra top-level wizard steps.
Use internal tabs/cards inside Step 4:
- Founder 1
- Founder 2
Optional Founder 3 only if later approved.
```

### Avatar/portrait placeholder

Visual style:

```text
Large circular avatar placeholder
Initials preview
Upload/photo area may be placeholder only
Use dashed circular border or dark avatar card
Teal plus/add icon
```

If image upload is not implemented:

```text
Show placeholder avatar selection only.
Do not implement file picker unless requested.
```

### Right summary content

```text
Founder name
Age
Nationality
Location
Background
Primary skill profile
Status
```

Use muted text for incomplete fields:

```text
Will be selected
Will be defined
Not set
```

---

## Step 5 — Team & Budget

### Purpose

Set the starting company resources and optional starting team setup.

### Main heading

```text
Team & Budget
```

### Subtitle

```text
Choose your starting cash preset and early company setup.
```

### Required starting cash decision

The first version standard/default start is:

```text
£50,000
```

### Starting cash presets

Use this preset structure:

```text
Lean Start — £35,000
Standard Startup — £50,000
Supported Start — £75,000
Sandbox Custom — configurable
```

Default selected:

```text
Standard Startup — £50,000
```

### Starting cash rules

```text
No loans in first version.
No emergency funding in first version.
No base operating cost in first version.
No employee overhead in first version.
Payroll is the only recurring employee-related cost.
Negative cash blocks hiring until cash is positive again.
```

### Starting team choice

Recommended first implementation options:

```text
No Starting Team
Small Founding Team Placeholder
Custom Starting Team Placeholder
```

If real starting employees are not implemented:

```text
Mark team options as placeholder UI only.
Do not create employees directly from this screen in Phase 5 unless the Application/Core logic already exists.
```

### Budget allocation

Optional for Phase 5 as placeholder UI:

```text
Development reserve
Marketing reserve
Support reserve
Hiring reserve
```

If included, treat as setup preview only until finance logic exists.

### Right summary content

```text
Starting cash
Starting team choice
Budget allocation summary
Warnings
```

Warnings:

```text
Lean Start increases early pressure.
Sandbox Custom may affect balance.
No starting team means the first action will likely be hiring.
```

Avoid recommendation wording.

---

## Step 6 — Sandbox Setup

### Purpose

Configure world generation and difficulty-like sandbox settings.

### Main heading

```text
Sandbox Setup
```

### Subtitle

```text
Configure the market, competitors, technology pace, and failure mode for this save.
```

### Fields

```text
Market Size
Competitor Density
Technology Pace
Economic Volatility
Hiring Difficulty
Failure Mode
Market Seed
```

### Field options

#### Market Size

```text
Small
Standard
Large
```

Default:

```text
Standard
```

#### Competitor Density

```text
Low
Standard
High
```

Default:

```text
Standard
```

#### Technology Pace

```text
Slow
Standard
Fast
```

Default:

```text
Standard
```

#### Economic Volatility

```text
Calm
Standard
Chaotic
```

Default:

```text
Standard
```

#### Hiring Difficulty

```text
Easy
Standard
Hard
```

Default:

```text
Standard
```

#### Failure Mode

```text
Forgiving
Standard
Hardcore
Sandbox
```

Default:

```text
Standard
```

#### Market Seed

```text
Random seed by default
Manual seed input available
Randomise Seed button
```

### Important implementation rule

```text
Phase 5 only builds the setup UI and ViewModel shape.
Actual market/competitor/world generation belongs to Application/Core and later integration.
```

### Right summary content

```text
Market size
Competitor density
Technology pace
Economic volatility
Hiring difficulty
Failure mode
Market seed
Warnings
```

Warnings:

```text
High competitor density increases market pressure.
Fast technology pace may make products age faster.
Chaotic economy creates more volatile reports/events.
Hardcore failure mode can end the save.
```

Do not include exact hidden formulas.

---

## Step 7 — Review

### Purpose

Give the player a final review before creating the company.

### Main heading

```text
Review Company Setup
```

### Subtitle

```text
Check your company, founder, budget, and sandbox settings before starting.
```

### Review sections

Use cards/sections:

```text
Company Identity
Company Background
Founder Setup
Founder Details
Team & Budget
Sandbox Setup
Warnings
```

### Each section should show

```text
Section title
Key selected values
Edit button or step link
Incomplete/invalid marker if needed
```

### Final warnings

Show any relevant warnings in one section:

```text
Low starting cash increases early pressure.
Hardcore mode can end the save.
High competitor density increases market pressure.
Some optional fields are not set.
```

### Final action

Footer right button becomes:

```text
Confirm Create Company
```

Rules:

```text
Button is disabled if required fields are invalid.
Button text must not be “Next” on final step.
Clicking it should call a placeholder callback in Phase 5 or the real creation command if already implemented.
```

---

## 14. Required ViewModel Structure

Create display-ready ViewModels. Do not place core rules in the ViewModel.

Recommended files:

```text
CompanyCreationViewModel.cs
CompanyCreationStepViewModel.cs
CompanyCreationSummaryViewModel.cs
CompanyCreationValidationViewModel.cs
```

Minimum ViewModel data:

```text
CurrentStep
Steps
CanGoBack
CanGoNext
CanConfirm
FooterHelperText
FooterValidationMessage
CompanyName
IndustryFocus
Headquarters
CompanyColour
LogoId
BackgroundId
FounderSetupType
FounderProfiles
StartingCashPreset
StartingCashAmount
StartingTeamChoice
SandboxSettings
SummaryRows
WarningRows
```

Step ViewModel fields:

```text
StepId
StepNumber
DisplayName
State
IsClickable
ValidationMessage
```

Step states enum:

```text
Future
Available
Active
Complete
Invalid
Disabled
```

Visual state should be semantic, not raw colours.

Good:

```text
StepState = Active
WarningLevel = Warning
ButtonState = Disabled
```

Bad:

```text
StepColour = #45CBB7
ButtonWidth = 247
```

---

## 15. Required Controller Behaviour

Create a controller/presenter that handles screen interactions.

Responsibilities:

```text
Initialize screen with placeholder/default ViewModel
Move next/back between steps
Validate current step
Update ViewModel after input changes
Update right summary panel
Update footer button states
Handle close request
Handle confirm request placeholder
```

The controller must not:

```text
Own company creation simulation rules
Write save files directly
Generate markets directly
Generate competitors directly
Mutate core runtime state directly
Hard-code visual colours
Hard-code layout dimensions
Create Canvas/UGUI objects
```

---

## 16. Required View Behaviour

The View owns UI Toolkit VisualElement references only.

The View may:

```text
Query visual elements from UXML
Register button callbacks
Unregister button callbacks
Bind ViewModel values to labels/fields
Toggle semantic USS classes
Display validation messages
Show/hide step content
```

The View must not:

```text
Create final company state
Create save data
Call save/load directly
Own gameplay rules
Own final setup formulas
Use hard-coded colours
Use hard-coded layout styling in C#
```

---

## 17. Required UI Files

Create or update these files:

```text
Assets/Project/UI/UXML/Screens/CompanyCreation/CompanyCreationScreen.uxml
Assets/Project/UI/USS/Screens/CompanyCreation/CompanyCreationScreen.uss

Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationView.cs
Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationController.cs
Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationViewModel.cs
Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationStep.cs
Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationStepViewModel.cs
Assets/Project/Scripts/Presentation/UI/Screens/CompanyCreation/CompanyCreationSummaryViewModel.cs
```

Only create extra files if needed for clean separation.

Do not create one giant script.

---

## 18. Required UXML Naming

Use clear names.

Recommended element names:

```text
company-creation-root
company-creation-background
company-creation-header
company-creation-title
company-creation-subtitle
company-creation-close-button

company-creation-content
company-creation-stepper-panel
company-creation-stepper-list

company-creation-main-panel
company-creation-step-title
company-creation-step-subtitle
company-creation-step-content

company-creation-summary-panel
company-creation-summary-title
company-creation-summary-list
company-creation-warning-list

company-creation-footer
company-creation-back-button
company-creation-helper-text
company-creation-next-button
company-creation-confirm-button
```

---

## 19. Required USS Class Naming

Use BEM-style classes.

Recommended classes:

```text
company-creation
company-creation__background
company-creation__header
company-creation__title-block
company-creation__title
company-creation__subtitle
company-creation__close-button

company-creation__content
company-creation__stepper-panel
company-creation__stepper-item
company-creation__stepper-number
company-creation__stepper-label
company-creation__stepper-connector

company-creation__main-panel
company-creation__step-heading
company-creation__step-description
company-creation__form-grid
company-creation__field
company-creation__choice-grid
company-creation__choice-card

company-creation__summary-panel
company-creation__summary-row
company-creation__summary-label
company-creation__summary-value
company-creation__warning-row

company-creation__footer
company-creation__footer-helper
company-creation__footer-validation
```

State classes:

```text
is-active
is-complete
is-invalid
is-disabled
is-selected
is-clickable
has-warning
has-error
```

Forbidden class names:

```text
box1
panel2
new-style
temp-card
green-button
final-button
leftThing
rightThing
```

---

## 20. Styling Rules

All styling must use project theme tokens.

Use:

```text
theme-colors.uss
theme-typography.uss
theme-spacing.uss
theme-sizing.uss
theme-layout.uss
```

Do not hard-code random colours.

Visual mapping:

```text
Background = deep blue-black/root background token
Main panels = panel/raised panel tokens
Borders = subtle/strong border tokens
Primary text = primary text token
Secondary text = secondary text token
Accent = teal/cyan accent token
Warnings = warning token
Errors = danger token
Success/complete = success or accent, depending context
```

Company Creation should use:

```text
Primary action button = teal/cyan accent
Selected choice card = dark teal-tinted background + teal border
Active step = teal/cyan filled circle + dark/inverse text
Completed step = teal outline/check mark
Invalid step = warning/danger marker + text
```

---

## 21. Interaction States

Every interactive element must have visible states:

```text
Default
Hover
Pressed
Focused
Selected
Disabled
Invalid
```

Required examples:

```text
Choice card hover: slightly brighter panel + stronger border
Choice card selected: teal border + dark teal background tint
Input focus: teal border
Input invalid: danger border + validation text
Next disabled: disabled button styling
Close hover: brighter dark panel
Stepper active: teal highlight
```

Do not rely on colour alone. Use text, icons, borders, or state markers.

---

## 22. Placeholder Data Rules

Phase 5 may use placeholder/static data, but it must be clearly marked.

Allowed:

```text
[Placeholder] default founder name
[Placeholder] sample seed value
[Placeholder] sample summary values
```

Not allowed:

```text
Fake values presented as final simulation results
Hidden gameplay bonuses from background choices
Hard-coded permanent starting employees
Generated market data
Generated competitors
```

If placeholder data is used, document it in the changelog.

---

## 23. Validation Rules

Required validation:

```text
Company name must be entered.
Industry focus must be selected.
Headquarters must be selected.
Founder setup must be selected.
Founder required fields must be valid.
Starting cash preset must be selected.
Sandbox setup must have valid default values.
Review step must confirm all required setup values are valid.
```

Validation display:

```text
Field-level message near invalid input
Footer-level message for blocking step issue
Invalid step marker in stepper
Summary panel may show incomplete values as muted text
```

Example validation messages:

```text
Enter a company name to continue.
Select an industry focus.
Select a founder setup option.
Complete required founder details.
Choose a starting cash preset.
Review all required setup sections before creating the company.
```

---

## 24. Responsive Behaviour

At 1920×1080:

```text
Show all three columns:
- Stepper
- Main form
- Summary
```

At smaller desktop resolutions:

```text
Keep stepper visible if practical.
Allow main form panel to scroll internally.
Allow summary panel to shrink or stack below only if necessary.
Footer must remain visible.
No required controls may clip off-screen.
```

At high resolutions:

```text
Do not stretch panels endlessly.
Use max widths.
Keep central layout readable.
```

---

## 25. Accessibility Requirements

Must support:

```text
Readable text contrast
Keyboard focus states
Button labels
No colour-only state communication
Clear validation messages
Large enough click targets
Consistent tab order where practical
```

Icon-only close button must have:

```text
Tooltip or accessible label: Close
```

---

## 26. Routing and Screen Lifecycle

Expected route ID:

```text
screen.company_creation
```

Expected entry point:

```text
Main Menu → New Game → Company Creation Screen
```

Expected exit points:

```text
Close / Cancel → Main Menu or confirmation modal
Confirm Create Company → main game entry once Phase 6/Application wiring exists
```

Phase 5 may use placeholder routing if the real router exists but company creation command does not.

---

## 27. Out-of-Scope Mechanics

Do not implement these in this screen:

```text
Loans
Emergency funding
Equity allocation
Founder disagreements
Founder salary system
Founder life simulation
Detailed legal setup
Office selection with gameplay effects
Full candidate generation
Starting employee simulation
Starting market generation
Competitor generation
Technology tree generation
Save file writing, unless provided by existing Application/Core service
```

---

## 28. Completion Criteria

This implementation is complete when:

```text
Company Creation screen opens from the intended route or test entry point.
The screen fills the display and does not show the in-game sidebar/topbar.
The vertical stepper displays all 7 steps.
The active step is clearly highlighted.
The main panel displays the correct content for each step.
The right summary panel updates from the ViewModel.
Back and Next move through steps.
Final step shows Confirm Create Company.
Required invalid states disable progression.
Validation messages are visible.
Choice cards show hover and selected states.
Inputs show focus and invalid states.
The UI matches the dark premium founder setup visual target.
No Canvas/UGUI runtime UI is added.
No core gameplay logic is placed in the View.
No save creation or simulation generation is directly implemented in the UI.
No one-off colours/fonts/sizing are introduced.
Project compiles.
No new console errors are introduced.
Changelog is updated.
```

---

## 29. Required Bezi Response After Implementation

After implementation, respond using the project response format:

```text
Completed:
- Specific completed change.
- Specific completed change.

Files Changed:
- path/to/file
- path/to/file

Architecture Notes:
- How decoupling was maintained.
- Which architecture layers were touched.
- Whether UI Toolkit-only was followed.
- Any placeholder used.

Documentation Updated:
- Changelog entry path.
- Formula/tuning/debug docs if relevant.

Testing:
- What was tested.
- What could not be tested.

Issues / Follow-up:
- Any unresolved ambiguity.
- Any required next step.
```

---

## 30. Final Lock

```text
LOCKED — Company Creation Screen

The Company Creation Screen is a full-screen UI Toolkit wizard used before entering the main game.

It uses:
- Large title and subtitle top-left
- Close button top-right
- Vertical stepper on the left
- Large central form panel
- Right-side live summary panel
- Fixed bottom footer with Back, helper/validation text, and Next/Confirm action

Steps:
1. Company
2. Background
3. Founders
4. Founder Details
5. Team & Budget
6. Sandbox Setup
7. Review

Standard starting cash is £50,000.
Company focus/specialisation is cosmetic/preference only.
Background choices are flavour/setup presentation only unless later explicitly given mechanics.
No loans, no emergency funding, no base operating cost, and no employee overhead exist in the first version.
Payroll is the only recurring employee-related cost.

Phase 5P builds the production-ready screen structure and placeholder/static ViewModel support.
Actual company creation, save creation, market generation, competitor generation, and simulation wiring belong to Application/Core and later integration phases.
```
