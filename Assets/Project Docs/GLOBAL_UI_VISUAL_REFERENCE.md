# GLOBAL_UI_VISUAL_REFERENCE - Dark Founder Dashboard Style

**Status:** Locked global UI visual reference  
**Applies to:** Phase 4 UI Shell and all future runtime UI  
**Purpose:** Replace screenshot references with a complete text description Bezi can follow directly.

---

## 1. Core Visual Target

The UI must look like a **premium dark desktop management dashboard** for a serious tech-company simulation.

The target feel is:

```text
Dark professional
Founder / executive dashboard
Modern SaaS control panel
Football Manager-style information density
Startup/company operations console
Premium but restrained
Readable before decorative
```

The UI must not look like:

```text
Default Unity UI
Plain grey debug panels
Mobile app UI
Toy-like tycoon UI
Bright arcade UI
Cyberpunk neon overload
Flat black boxes with no depth
Generic web form with no game identity
```

---

## 2. Overall Colour Impression

The screen should primarily use:

```text
Deep blue-black backgrounds
Dark navy panels
Charcoal raised surfaces
Subtle teal/cyan accent
Muted grey-blue borders
Off-white primary text
Cool grey secondary text
Semantic green / amber / red / blue / purple only where meaningful
```

The UI should feel dark, but not empty or pure black.

### 2.1 Background Hierarchy

Use this visual hierarchy:

```text
Root background:
- Almost black blue/navy
- Slight gradient or depth allowed
- Not flat #000000

Shell background:
- Dark blue-black
- Slightly lighter than root
- Used behind sidebar/topbar/content

Panel background:
- Raised dark navy/charcoal
- Clearly visible against shell
- Uses subtle border and slight shadow

Selected state:
- Dark teal-tinted panel background
- Teal border or left rail
- Optional subtle glow

Primary action:
- Teal/cyan filled button
- Bright enough to stand out
- Not over-saturated neon
```

---

## 3. Lighting and Depth

The UI should use **subtle depth**.

Allowed:

```text
Soft panel shadows
Fine 1px borders
Slight inner highlight on panels
Subtle background gradients
Low-opacity teal glow on active states
Glass-like dark panels
Slight transparency effect if readable
```

Avoid:

```text
Heavy bloom
Large neon glows
Bright outlines on every card
Strong shadows on every small element
Noisy decorative backgrounds behind dense text
```

Depth should make the UI readable, not decorative.

---

## 4. Main Menu Visual Spec

The main menu is separate from the in-game shell.

### 4.1 Layout

Use a full-screen cinematic menu layout:

```text
MainMenuRoot
|-- BackgroundImageLayer
|-- DarkGradientOverlay
|-- LogoArea top-left
|-- PrimaryMenuColumn left side
|-- PlayerProfileArea top-right
|-- NewsPanel right side
|-- CommunityPanel below NewsPanel
|-- FooterCopyright bottom-left
`-- BuildVersion bottom-right
```

### 4.2 Background

The background should be:

```text
Dark city skyline / tech district / corporate night scene
Blue-black colour grade
Subtle vignette
Low contrast behind UI
No bright areas behind menu buttons
```

If no final background art exists, use a placeholder dark gradient:

```text
Top: deep navy-black
Middle: dark blue-black
Bottom: near black
Optional faint city silhouette placeholder
```

### 4.3 Logo Area

Position:

```text
Top-left
Approx 40px from left
Approx 40px from top
```

Style:

```text
Large white brand text
Teal/cyan circular or geometric accent
Clean modern startup identity
No cartoon styling
```

Placeholder:

```text
TECH MOGUL
```

Do not use "Start Up" unless the project name changes.

### 4.4 Primary Menu Column

Position:

```text
Left side
Starts around 25% screen height
Width around 440px at 1920x1080
Vertical stack
```

Buttons:

```text
Continue
New Game
Load Game
Settings
Credits
Exit
```

Button style:

```text
Height: 88px for main menu buttons
Width: 440px
Border radius: 8px
Background: dark translucent panel
Border: subtle blue-grey
Icon left
Label centre-left
Large readable text
Hover: brighter dark panel + teal border
Selected/default: teal gradient fill
```

Continue button:

```text
Primary teal filled button
White/inverse text
Play icon on left
Slight glow allowed
```

Other buttons:

```text
Dark raised button
Teal outline icon
Off-white label
```

### 4.5 Right Panels

Right side should contain card panels.

#### Player Profile Area

Top-right:

```text
Circular avatar with initials
Founder/player name
Role label underneath
```

Example placeholder:

```text
AM
Alex Morgan
Founder
```

#### News Panel

Panel width:

```text
500px to 560px
```

Contains:

```text
Panel title: Latest News
Image thumbnail area
Patch/update title
Short body text
Link-style button: View Patch Notes ->
```

#### Community Panel

Below news:

```text
Title: Join the Community
Short text
Icon buttons row
```

If no community links exist, keep placeholder icons disabled or omit panel until needed.

### 4.6 Footer

Bottom-left:

```text
(c) 2026 Tech Mogul. All rights reserved.
```

Bottom-right:

```text
Build v0.1
```

---

## 5. In-Game Shell Visual Spec

The runtime shell is the main game UI.

### 5.1 Layout

Use this structure:

```text
MainGameShell
|-- TopStatusBar
|-- SidebarNavigation
|-- MainContentArea / ScreenHost
|-- RightDrawerHost
|-- ModalHost
|-- TooltipHost
|-- NotificationHost
`-- DebugHost
```

Screen arrangement:

```text
TopStatusBar spans full width.
SidebarNavigation sits left, below or aligned with top bar depending on layout.
MainContentArea fills remaining central space.
RightDrawerHost overlays or docks on the right.
ModalHost overlays all shell content.
```

### 5.2 Top Status Bar

Height:

```text
56px to 64px
```

Visual style:

```text
Dark navy/black panel
Subtle bottom border
Segmented metric blocks
Clean spacing
No bright background
```

Content from left to right:

```text
Company logo/icon
Company name
Company type/subtitle
Cash
Net / Month
Runway
Current date centred
Speed controls
Continue button
Settings button
```

Example layout:

```text
[Logo] Debug Corp
       Indie Studio

[Cash]
GBP 512K
Available Funds

[Net / Month]
-GBP 18K
Burn Rate

[Runway]
28 days
Est. Runway

                Day 12 - Month 2 - Year 1

[1x] [2x] [3x] [▶ Continue] [Settings]
```

Metric blocks:

```text
Width: 130px to 160px
Border-left/right subtle
Label small uppercase
Value large
Sub-label tiny muted
```

Status colours:

```text
Positive value: green
Negative net/month: red/danger
Warning runway: amber
Neutral cash: white/off-white
```

Continue button:

```text
Primary teal button
Height: 40px
Width: 150px to 180px
Play icon left
Label: Continue
```

Speed buttons:

```text
Segmented control
Each segment 64px x 40px
Active speed uses teal border/fill
Inactive speed dark
```

Settings button:

```text
Square icon button
40px x 40px
Dark raised
Gear icon
```

---

## 6. Sidebar Navigation Visual Spec

### 6.1 Layout

Sidebar width:

```text
240px expanded
64px collapsed, if implemented later
```

Default:

```text
Expanded
Always visible in main game
Dark panel background
Right border
Vertical scroll if needed
```

### 6.2 Sidebar Header

Top of sidebar:

```text
Company logo
Company name
Company type/subtitle
```

Example:

```text
[Logo]
Debug Corp
Indie Studio
```

Logo:

```text
Geometric teal/cyan icon placeholder
40px to 48px
```

### 6.3 Navigation Groups

Use grouped navigation with uppercase teal group labels.

Groups:

```text
OVERVIEW
- Dashboard
- Inbox
- Calendar
- Reputation

HR PORTAL
- Candidates
- Employees
- Teams
- Development

FINANCE
- Overview
- Transactions
- Reports

PRODUCTION
- Products
- Milestones
- Releases / Updates
- QA & Testing

MARKET
- Market Overview
- Pricing
- Marketing

COMPETITORS
- Competitor List
- Industry Trends
```

Group label style:

```text
Small uppercase
Teal/cyan
Bold/semi-bold
Letter spacing slight
Margin top between groups
Chevron on right for collapse/expand
```

Nav item style:

```text
Height: 34px to 38px
Icon left 16px
Label text
Optional badge right
Rounded 4px to 6px
Muted text when inactive
```

Active nav item:

```text
Teal-tinted dark background
Teal vertical rail on left
Teal text/icon
Subtle glow allowed
```

Hover nav item:

```text
Slightly lighter dark background
Text becomes brighter
```

Badge style:

```text
Small pill
Dark raised background
Muted text
For unread count, use info/accent styling
```

Example:

```text
Inbox    [7]
Employees selected with teal rail and background
```

---

## 7. Dashboard / Founder Portal Visual Spec

The dashboard should be a dense operational overview.

### 7.1 Layout

```text
DashboardScreen
|-- ScreenHeader
|-- KPIGrid
|-- MainDashboardGrid
|   |-- ProductProgressCard
|   |-- TeamOverviewCard
|   |-- UpcomingMilestonesCard
|   `-- RightColumn
|       |-- UpNextCard
|       `-- QuickActionsCard
`-- RecentActivityStrip
```

### 7.2 Screen Header

Style:

```text
Icon tile left
Title large
Subtitle muted
```

Example:

```text
[grid icon] Main Dashboard
Overview of your company's performance and key metrics.
```

Icon tile:

```text
48px x 48px
Dark raised panel
Subtle border
Muted icon
```

### 7.3 KPI Cards

Top row of four cards:

```text
Total Cash
Net / Month
Runway
Revenue (MTD)
```

Card size:

```text
Flexible width
Height around 160px to 190px
```

Card contents:

```text
Small label
Large value
Trend text
Small sparkline chart
Icon top-right
```

Style:

```text
Dark raised card
1px border
Subtle teal chart line unless semantic state requires red/green
Rounded corners
```

Negative card:

```text
Value red/danger
Trend red/danger
Chart red/danger only if it represents decline/loss
```

Positive card:

```text
Trend green/success
```

### 7.4 Main Cards

Use card panels with:

```text
Title
Optional info icon
Content list/chart
Footer link/action
```

Examples:

```text
Project/Product Progress
Team Overview
Upcoming Milestones
Up Next
Quick Actions
Recent Activity
```

Dashboard must show data and navigation only. Do not include recommendation text like "You should hire QA".

---

## 8. List / Table Screen Visual Spec

This pattern applies to screens like Employees, Candidates, Teams, Products, Contracts, Reports, Competitors.

### 8.1 Layout

```text
ListScreen
|-- ScreenHeader
|-- TabBar
|-- Toolbar
|-- DataTable
|-- PaginationFooter
`-- RightFilterDrawer
```

### 8.2 Header

Example:

```text
[people icon] Employees
Manage your team, roles, and development.
```

### 8.3 Tabs

Tabs appear below header.

Example:

```text
All Employees [24]
By Team [6]
On Leave [2]
Terminated [0]
```

Style:

```text
Text tabs
Active tab teal text with underline
Count badge as small pill
Inactive tabs muted
```

### 8.4 Toolbar

Contains:

```text
Team dropdown or category dropdown left
Search field right or centre
Filters button
Export button, if relevant
```

Search field:

```text
Dark input
Search icon
Placeholder text
Width 300px to 420px
```

Filter button:

```text
Dark secondary button
Funnel icon
Label: Filters
```

Export button:

```text
Dark secondary button
Export/download icon
Label: Export
```

### 8.5 Table

Table style:

```text
Dark panel background
Header row
Thin dividers
Compact rows
Subtle hover
Selected row teal border/rail
```

Row height:

```text
Standard: 44px if avatar + two-line text
Compact: 34px for plain data rows
```

Employee row example:

```text
[checkbox] [avatar ES] Emily Stewart
                      UID Designer

Role: Senior
Team: Design
Level: 5
Morale: 92% + progress bar
Salary: GBP 62K
Status: Active
Start Date: 14 Feb 2025
Actions: ...
```

Selected row:

```text
Teal border
Subtle dark teal background
Optional left rail
```

Do not use bright full-row fills.

---

## 9. Right Drawer / Filter Panel Visual Spec

Right drawer is used for filters, inspectors, and contextual side panels.

### 9.1 Layout

```text
RightDrawer
|-- DrawerHeader
|   |-- Title
|   `-- CloseButton
|-- DrawerBody
|   |-- Field groups
|   |-- Dropdowns
|   |-- Sliders
|   `-- Inputs
`-- DrawerFooter
    |-- Clear button
    `-- Apply button
```

Width:

```text
360px to 400px
```

Visual style:

```text
Dark raised panel
Left border
Slight shadow
Full height under top bar or full height depending shell
```

Filter drawer example fields:

```text
Search
Team dropdown
Role dropdown
Status dropdown
Level min/max
Morale slider
Salary range
Clear Filters
Apply Filters
```

Buttons:

```text
Clear Filters = secondary full-width
Apply Filters = primary teal full-width
```

---

## 10. Detail Modal Visual Spec

Large detail modals should feel like focused management dossiers.

### 10.1 Layout

```text
DetailModal
|-- ModalHeader
|   |-- Avatar/Icon
|   |-- IdentityBlock
|   |-- Status/metadata
|   `-- CloseButton
|-- ModalTabs
|-- ModalBody
|   |-- InfoCardsGrid
|   |-- MetricsCards
|   |-- ActivityList
|   `-- Skill/Progress sections
`-- ModalFooter
    |-- Secondary actions
    `-- Primary action
```

Width:

```text
Standard detail modal: 860px
Large detail modal: 1080px
Maximum height: 86% screen height
```

Style:

```text
Dark glass-like raised panel
Strong border compared to normal panels
Subtle modal shadow
Dimmed backdrop
Close button top-right
```

### 10.2 Header

Employee modal example:

```text
[Avatar ES]

Emily Stewart       [Senior]
UID Designer

32 years old - London, UK (Remote)

Employee ID: EMP-0007
Joined: 14 Feb 2025
[Active]
```

Avatar:

```text
Circular
Initials
Teal ring
Small status dot
```

### 10.3 Tabs

Example:

```text
Overview
Skills
Performance
Development
Compensation
History
```

Style:

```text
Horizontal tabs
Active tab teal text + underline
Inactive muted
```

### 10.4 Body Cards

Cards use a 2-3 column layout.

Example cards:

```text
Role & Team
Performance & Morale
Compensation
Core Skills
Recent Activity
```

Each card:

```text
Dark raised surface
Subtle border
Small uppercase section label
Readable value hierarchy
```

### 10.5 Footer

Footer sticks to bottom where practical.

Example:

```text
[Close]       [Message] [Schedule 1:1] [Edit Profile]
```

Primary action:

```text
Teal filled
Right aligned
```

Secondary actions:

```text
Dark raised
Bordered
```

---

## 11. Wizard / Setup Flow Visual Spec

This is the global pattern for multi-step creation flows.

Use it for:

```text
Company setup
Founder setup
Product creation
Team creation
Contract creation if multi-step
Research setup if multi-step
Release/update setup
Hardware manufacturing setup
Sandbox setup
```

### 11.1 Wizard Layout

```text
WizardShell
|-- WizardHeader
|   |-- ContextLabel
|   |-- WizardTitle
|   `-- StepCounter
|-- WizardStepper
|-- WizardBody
|   |-- MainStepContent
|   `-- PreviewPanel
`-- WizardFooter
    |-- LeftActions
    |-- HelperText
    `-- RightActions
```

Visual direction:

```text
Dark full-screen workflow
No left in-game sidebar
No top game status bar
Uses same colour/theme tokens
More focused and calmer than dashboard
```

### 11.2 Header

Top area:

```text
Height: around 72px
Bottom border
Left aligned context + title
Right aligned step count
```

Example:

```text
NEW GAME
New Company Setup                                      Step 3 of 7
```

Context label:

```text
Small uppercase
Teal
```

Title:

```text
Large Space Grotesk-style heading
Off-white
```

Step count:

```text
Small muted text
```

### 11.3 Horizontal Stepper

Stepper sits below header.

Height:

```text
72px to 80px
```

Layout:

```text
Centered horizontal sequence
Circular numbered chips
Thin connector lines
Small labels below chips
```

Example:

```text
1 Company - 2 Background - 3 Founders - 4 Founder Details - 5 Team & Budget - 6 Preview - 7 Review
```

Chip size:

```text
Active chip: 30px to 34px circle
Inactive chip: 28px to 30px circle
```

States:

```text
Complete:
- Teal outline or filled dark teal
- Number visible
- Connector before it teal

Active:
- Filled teal/cyan
- Dark/inverse number
- Label bright

Available/future:
- Dark grey circle
- Muted border
- Muted number and label

Locked/disabled:
- Very muted grey
- Low opacity

Warning:
- Amber border or small warning marker

Error:
- Red/danger border or small error marker
```

Connector line:

```text
Width: 32px to 48px between chips
Height: 1px or 2px
Muted grey by default
Teal for completed path
```

### 11.4 Body

Body split:

```text
MainStepContent: 60% to 65% width
PreviewPanel: 35% to 40% width
```

Vertical divider between main and preview.

Body starts below stepper and ends above footer.

Main content padding:

```text
24px
```

Preview padding:

```text
24px
```

### 11.5 Main Step Content

Use for:

```text
Forms
Choice cards
Selection grids
Budget controls
Sliders
Dropdowns
Team/role pickers
```

Step heading:

```text
Large section title
Short explanatory subtitle
```

Example:

```text
Company Background
Choose your company's origin story. This affects your starting conditions and early opportunities.
```

### 11.6 Choice Cards

Choice cards are used for backgrounds, founder count, product type, sandbox presets, etc.

Card style:

```text
Dark raised panel
1px border
Rounded corners
Padding 16px
Hover border brighter
Selected border teal
Selected background dark teal tint
```

Card contents:

```text
Title
Description
Tags/chips
Advantages/risks if needed
Difficulty/status label
```

Grid:

```text
2 or 3 columns depending width
Gap 12px to 16px
```

Selected card:

```text
Teal border
Dark teal overlay/tint
Optional subtle glow
```

Disabled card:

```text
Lower opacity
Muted text
No strong hover
```

### 11.7 Preview Panel

Right panel title:

```text
Preview
```

Empty preview message:

```text
Select options to see a preview.
```

Preview content examples:

```text
Company background summary
Founder summary
Starting strengths
Starting risks
Recommended founder roles
Difficulty
Budget impact
Team setup summary
Product concept summary
Launch readiness summary
```

Preview style:

```text
Dark panel, slightly different from main background
Left divider or internal card border
Text hierarchy clear
Positive lines green
Risk lines red/danger
Warnings amber
Tags use chips
```

Important: preview explains selected options but does not recommend the "best" choice.

### 11.8 Footer

Footer fixed at bottom.

Height:

```text
56px to 64px
```

Style:

```text
Dark bar
Top border
Left actions
Centre helper text
Right actions
```

Left:

```text
Back
Cancel
Randomise
Randomise Current Step
```

Centre:

```text
Small muted helper text
Validation message if needed
```

Right:

```text
Save & Exit
Continue
Confirm / Create / Launch
```

Button rules:

```text
Continue disabled until current step is valid
Save & Exit only where partial save exists
Confirm only on final review step
Randomise only where randomisation is valid
Footer remains visible while body scrolls
```

---

## 12. Company Setup Wizard Specific Steps

Final company setup wizard:

```text
1. Company
2. Background
3. Founders
4. Founder Details
5. Team & Budget
6. Sandbox Setup
7. Review
```

### Step 1 - Company

Fields:

```text
Company Name
Industry Focus
Headquarters
Optional logo/icon
Optional company colour
```

Preview:

```text
Company identity summary
Selected focus
Selected location
```

### Step 2 - Background

Choice cards:

```text
Design Studio
Enterprise Consultancy
Game Studio
Growth Agency
Hardware Startup
Software Startup
```

Each card should include:

```text
Description
Recommended founder tags
Starting strengths
Starting risks
Difficulty
```

Important project rule:

```text
Company specialisation/focus is cosmetic/preference only unless the GDD explicitly defines a non-cosmetic effect.
Do not create locking, gating, permanent boosts, or penalties from company specialisation.
```

### Step 3 - Founders

Choice cards:

```text
Solo Founder
Co-Founders
```

Solo Founder card:

```text
Advantages:
+ Full creative control
+ Lower initial salary costs
+ Simpler decision-making
+ Faster early pivots

Trade-offs:
- Narrower skill coverage
- Higher burnout risk
- Slower early development
- Single point of failure
```

Co-Founders card:

```text
Advantages:
+ Broader skill coverage
+ Shared workload
+ Complementary strengths
+ Better team morale start

Trade-offs:
- Higher initial salary costs
- Potential disagreements
- Slower early decisions
- Equity split complexity
```

If equity is not implemented yet, mark equity references as flavour/deferred, not live mechanics.

### Step 4 - Founder Details

Fields:

```text
Founder name
Age
Nationality
Location
Background
Primary skill profile
Optional portrait/avatar
```

If multiple founders exist, use sub-tabs or founder cards inside this step instead of adding many top-level stepper chips.

### Step 5 - Team & Budget

Fields:

```text
Starting team choice
Starting cash preset
Optional budget allocation
Initial hiring/candidate options if implemented
```

### Step 6 - Sandbox Setup

Fields:

```text
Market size
Competitor density
Technology pace
Economic volatility
Hiring difficulty
Failure mode
Market seed
```

### Step 7 - Review

Show final summary:

```text
Company identity
Founder/founding team
Starting focus
Starting cash
Sandbox settings
Warnings
Confirm Create Company button
```

---

## 13. Product Creation Wizard Specific Steps

Product creation should use the same WizardShell.

### 13.1 Software Product Steps

```text
1. Product Type
2. Market & Audience
3. Concept / Scope
4. Platforms / Compatibility
5. Teams & Roles
6. Budget & Timeline
7. Preview
8. Review
```

### 13.2 Hardware Product Steps

```text
1. Product Type
2. Market & Audience
3. Hardware Tier / Components
4. Manufacturing Plan
5. Teams & Roles
6. Budget & Timeline
7. Preview
8. Review
```

Product preview panel should show:

```text
Product name
Product type
Target market
Supported platforms
Assigned teams
Role coverage
Budget impact
Estimated timeline
Risk indicators
Expected support burden
```

Use descriptive risk states, not precise hidden formulas.

---

## 14. Component List Required From Phase 4

Phase 4 should create the following reusable visual components or styling patterns.

```text
BaseButton
IconButton
SegmentedControl
NavGroup
NavItem
TopBarMetric
StatusPill
TagChip
DataCard
KpiCard
Panel
Card
SearchInput
DropdownField
FilterDrawer
DataTable
TableRow
TableHeader
PaginationFooter
EmptyState
PlaceholderScreen
ModalFrame
DetailModalFrame
ConfirmModal
NotificationToast
TooltipFrame
WizardShell
WizardHeader
WizardStepper
WizardStepChip
WizardBody
WizardPreviewPanel
WizardFooter
WizardChoiceCard
WizardValidationMessage
```

Components must be generic and reusable. They must not contain Employees/Products/Finance-specific business logic.

---

## 15. USS Class Naming Guidance

Use clear class names.

Examples:

```text
main-shell
main-shell__topbar
main-shell__sidebar
main-shell__screen-host
main-shell__right-drawer-host
main-shell__modal-host

top-status-bar
top-status-bar__metric
top-status-bar__continue-button

sidebar
sidebar__header
sidebar__group
sidebar__group-label
sidebar__nav-item
sidebar__nav-item-badge

dashboard
dashboard__kpi-grid
dashboard__content-grid

data-table
data-table__header
data-table__row
data-table__cell

detail-modal
detail-modal__header
detail-modal__tabs
detail-modal__body
detail-modal__footer

wizard-shell
wizard-shell__header
wizard-shell__stepper
wizard-shell__body
wizard-shell__main
wizard-shell__preview
wizard-shell__footer

wizard-step-chip
wizard-choice-card

is-active
is-selected
is-disabled
is-complete
is-warning
is-error
has-badge
```

Avoid:

```text
box1
thing
left-panel2
new-style
temp-button
final-card
green-card
```

---

## 16. Phase 4 Visual Completion Bar

Phase 4 is complete only when the UI foundation visually supports this standard:

```text
Main menu can be shown without looking like a placeholder.
Main game shell has a polished dark dashboard look.
Sidebar uses grouped navigation and active states.
Top bar shows company/finance/time placeholders in the correct style.
Screen host can show polished placeholder screens.
Right drawer opens and matches the filter panel style.
Generic modal opens and matches the detail/confirmation style.
WizardShell can show a company setup placeholder flow with top step chips.
Buttons, cards, chips, panels, inputs, tabs, and nav items all use shared styling.
No feature-specific gameplay logic is implemented.
No fake final data is treated as real.
No Canvas/UGUI is used.
```

---

## 17. Bezi Prompt Addition

Use this wording in your Bezi task:

```text
Bezi does not have access to the screenshot references. Use the Global UI Visual Reference text as the full visual source of truth.

The target UI is a premium dark desktop management dashboard: deep blue-black backgrounds, dark glass-like panels, thin borders, teal/cyan active states, grouped left sidebar navigation, top status bar with company/finance/time controls, right-side drawers, large centred modals, dense readable data tables, and full-screen wizard/setup flows with horizontal numbered step chips.

Do not copy any third-party branding, exact names, or fake data from examples. Do not invent unrelated screens or systems. Build only the Phase 4 UI foundation: theme, shell, routing, modal system, right drawer host, reusable components, and wizard foundation. Feature screens and simulation wiring belong to later phases.
```
