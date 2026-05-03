Phase 4 UI Conflict Resolution

Use this as the authoritative response to Bezi. These decisions should be added to the Global UI Visual Reference and reflected in the roadmap/GDD so Bezi no longer sees competing sources.

Authority Lock
This conflict resolution overrides the older Phase 4 UI wording wherever conflicts exist.

The Visual Reference is authoritative for visual style and reusable UI patterns.

The GDD remains authoritative for required game destinations, systems, and first-playable scope.

Where the Visual Reference renamed, omitted, or added navigation items, this conflict resolution defines the final Phase 4 navigation structure.
Conflict 1 — Sidebar Navigation Structure
Decision

Use the grouped sidebar structure from the Visual Reference, but preserve the GDD-required destinations.

The sidebar should not be a flat list.

The grouped sidebar is now authoritative for layout.

The GDD is still authoritative for which major destinations must exist.

Final Phase 4 Sidebar
OVERVIEW
- Dashboard
- Inbox
- Calendar
- Company

PEOPLE
- Recruitment
- Employees
- Teams

PRODUCTS & WORK
- Products
- Contracts
- Research
- Infrastructure

MARKET
- Market
- Competitors

FINANCE
- Finance
- Reports
Utility access
Settings

Settings should be accessible from the top-right settings button. It may also appear as a bottom sidebar utility item later, but it should not be mixed into the main business navigation groups.

Naming Rules
Portal becomes Dashboard as the player-facing label.
Stable ID remains screen.portal unless the project has already standardized on screen.dashboard.

Recruitment stays Recruitment.
Do not rename it to Candidates.
Candidates are content inside Recruitment.

Research stays Research.
Do not rename it to Development.

Contracts must remain present.
Infrastructure must remain present.
Settings must remain accessible.
Deferred / Not Phase 4 Sidebar Items

The following Visual Reference items are not Phase 4 top-level routes unless a later GDD page promotes them:

Reputation
Transactions
Milestones
Releases / Updates
QA & Testing
Pricing
Marketing
Industry Trends
Development

These may become:

Sub-tabs
Filter categories
Dashboard cards
Quick links
Future screen routes

They must not be added as full sidebar destinations in Phase 4 unless explicitly requested.

Conflict 2 — Main Menu Screen
Decision

The Main Menu is in scope for Phase 4 as a standalone UI foundation screen.

It should be added as a new Phase 4 plan.

Recommended roadmap update:

Plan 4G — Main Menu Foundation
Plan 4H — Shell Validation

Existing Plan 4F can remain Shell Validation only if no new plan numbering is desired, but the clean roadmap structure is:

4A — USS Theme Foundation
4B — UI Shell Structure
4C — Screen Router and Navigation System
4D — Modal System
4E — Wizard and Right Drawer Foundation
4F — Shell Styling and Polish
4G — Main Menu Foundation
4H — Phase 4 Validation
Main Menu Scope in Phase 4

Allowed:

Full-screen main menu layout
Dark cinematic background placeholder
Logo/title area
Continue/New Game/Load Game/Settings/Credits/Exit buttons
Patch/news panel placeholder
Build version text
Button hover/selected/disabled states
Routing to placeholder screens/flows

Forbidden in Phase 4:

Real save slot loading
Real community links
Real patch note fetching
Real profile account system
Real settings logic beyond opening placeholder settings
Conflict 3 — Wizard System Scope
Decision

The generic WizardShell framework is Phase 4 scope.

Specific wizard content is not Phase 4 scope.

Phase 4 builds the reusable wizard structure so later company setup, founder setup, product creation, and release planning flows all use the same pattern.

Phase 4 Wizard Scope

Build generic:

WizardShell
WizardHeader
WizardStepper
WizardStepChip
WizardBody
WizardPreviewPanel
WizardFooter
WizardChoiceCard
WizardValidationMessage

Phase 4 may include a placeholder wizard preview only to validate the layout.

Example placeholder:

[Placeholder]
Company Setup Wizard
This flow will be implemented in Phase 5P.
Deferred to Phase 5 / Later
Real company setup fields
Real founder setup fields
Real sandbox setup logic
Real product creation steps
Real product budget/timeline validation
Real team assignment logic
Conflict 4 — Right Drawer System
Decision

Add RightDrawerHost to the Phase 4 shell.

The right drawer is now part of the global UI architecture.

Updated root UI layering:

Root UIDocument
├── Application Shell
├── Screen Layer
├── Right Drawer Layer
├── Modal Layer
├── Tooltip Layer
├── Notification Layer
└── Debug Layer
Phase 4 Right Drawer Scope

Allowed:

RightDrawerHost container
Generic drawer open/close behaviour
Generic drawer frame styling
Filter drawer visual pattern
Placeholder filter content

Forbidden in Phase 4:

Real employee filtering
Real product filtering
Real market filtering
Real inspector logic
Screen-specific drawer business rules

The right drawer should be built as a generic shell-level pattern.

Conflict 5 — Component Library Size
Decision

Do not build all 40 listed components as full functional C# components in Phase 4.

Phase 4 builds:

Theme primitives
Shell components
Navigation components
Modal frame components
Wizard foundation components
Right drawer frame components
Reusable visual classes

Phase 5 builds screen-specific components when the relevant screen is implemented.

Phase 4 Components

Build as actual reusable components or clear reusable USS/UXML patterns:

BaseButton
IconButton
SegmentedControl
NavGroup
NavItem
TopBarMetric
StatusPill
TagChip
Panel
Card
SearchInput
DropdownField
RightDrawerFrame
FilterDrawerFrame
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
Phase 5 Components

Build when the relevant feature screens need them:

DataCard
KpiCard
DataTable
TableRow
TableHeader
PaginationFooter
Advanced filter controls
Dashboard metric cards
Screen-specific list rows
Screen-specific detail panels
Important Rule
Phase 4 components must be generic.
They must not contain Employees, Products, Finance, Market, Contracts, or Research business logic.
Conflict 6 — Topbar Content
Decision

Use a hybrid topbar.

The Visual Reference is authoritative for layout style.

The GDD is authoritative for required persistent information.

Final Phase 4 Topbar Layout
[Company Identity]
[Cash Metric]
[Net / Month Metric]
[Runway Metric]
                    [Current Date]
[Alert Count]
[Global Search]
[1x] [2x] [3x]
[Continue]
[Settings]
Required Topbar Content
Company logo/icon
Company name
Company type/subtitle
Cash
Net / Month
Runway
Current date
Active alerts count
Global search access
Speed controls: 1x / 2x / 3x
Continue button
Settings button
Search Rule

At 1920×1080:

Use a compact global search button or search field depending available space.

At smaller widths:

Collapse search to an icon button.

Do not remove global search entirely.

Alerts Rule

Active alerts count should appear as:

Small alert/bell button with badge

Do not place alert count inside the sidebar only.

Main Navigation Rule

The GDD phrase “main navigation” is satisfied by the left sidebar.

Do not duplicate all navigation in the topbar.

Final Authoritative Phase 4 Structure
Phase 4 — UI Shell and Navigation

4A — USS Theme Foundation
- Colours
- Typography
- Spacing
- Sizing
- Layout
- Component base classes

4B — Main Game Shell Structure
- TopStatusBar
- SidebarNavigation
- ScreenHost
- RightDrawerHost
- ModalHost
- TooltipHost
- NotificationHost
- DebugHost

4C — Screen Router and Navigation System
- Stable screen IDs
- Sidebar selected states
- Placeholder routes
- Standalone flow routing support

4D — Modal System
- ModalRouter
- Confirm modal
- Info/warning/error modal
- Detail modal frame

4E — Wizard and Right Drawer Foundation
- Generic WizardShell
- Horizontal step chips
- Preview panel
- Fixed footer
- Generic RightDrawerFrame

4F — Shell Styling and Polish
- Premium dark dashboard styling
- Glass-like panels
- Teal active states
- Hover/focus/disabled states
- No advanced shaders yet

4G — Main Menu Foundation
- Full-screen main menu
- Cinematic dark background placeholder
- Menu buttons
- Patch/news placeholder
- Build version
- Routes to placeholder flows

4H — Phase 4 Validation
- 1920×1080 check
- 1366×768 usability check
- 2560×1440 scaling check
- No Canvas/UGUI
- No gameplay logic
- No fake final data
- Routing works
- Modal works
- Right drawer works
- Wizard placeholder works