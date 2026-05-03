# Colour Palette

## 1. Purpose

This document defines the official runtime UI colour palette, theme tokens, semantic colour roles, and colour usage rules for the project.

Bezi must use this document when creating UI Toolkit USS files, screens, modals, components, shell layouts, debug UI, overlays, charts, warnings, status labels, and interactive states.

This document exists so Bezi does not invent one-off colours per screen, modal, button, chart, warning, badge, or status label.

The goal is:

```text
A consistent dark professional desktop-style UI.
A clear semantic colour system.
Readable text and data at all times.
Consistent status colours across all UI.
Consistent shader colour inputs if shaders are later approved.
No one-off feature colours.
No random neon/glow palettes.
```

This document defines colour ownership.

---

## 2. Authority

This is a mandatory project rule source.

It is not a feature design page.

It is not optional guidance.

It is not a final screen list.

Bezi must follow this document whenever implementing runtime UI.

Feature pages may define content, data, states, actions, warnings, edge cases, and player-facing flow.

Feature pages must not invent one-off colours, component styles, colour meanings, shader colours, status colours, chart palettes, or screen-specific theme variants.

If a new global colour pattern is genuinely required, update this document first.

---

## 3. Dependencies

This document must be used alongside:

```text
/Pages/Private/Project Docs/Project Rules.md
/Pages/Private/Project Docs/Coding Standard.md
/Pages/Private/Project Docs/UI Architecture.md
/Pages/Private/Project Docs/Typography.md
/Pages/Private/Project Docs/UI Sizing and Spacing Standard.md
```

This document does not define:

```text
Game systems
Feature lists
Screen lists
Layout skeletons
Exact dimensions
Component behaviour
Shader animation speeds
Simulation formulas
Feature-specific data
```

Those belong in the relevant GDD, implementation, UI architecture, formula, tuning, or feature documents.

---

## 4. Core Rule

```text
Colour must be semantic, tokenised, centralized, and consistent.
```

Do not choose colours manually per screen.

Do not hard-code colours in C#.

Do not use inline colours in UXML.

Use UI Toolkit USS variables and semantic classes.

---

## 5. Visual Theme Direction

The foundation UI should feel like:

```text
A dark professional desktop-style interface.
```

Target look:

```text
Deep charcoal / blue-black shell
Slightly raised dark panels
Subtle blue-grey borders
Off-white primary text
Muted grey-blue secondary text
Teal primary accent
Green success
Amber warning
Red/pink danger
Blue information
Purple special/advanced state
Professional, restrained glow only where approved
```

Avoid:

```text
Mobile app brightness
Bright toy-like panels
Full neon cyberpunk palette
Over-saturated backgrounds
Random feature-specific colours
Huge glowing gradients
Unreadable low-contrast text
Colour-only warnings
```

---

## 6. Base Theme Tokens

These are the locked foundation colour tokens.

If an existing shared USS file already contains equivalent tokens, align it to these names and values instead of duplicating.

Recommended file:

```text
Assets/Project/UI/USS/Theme/theme-colors.uss
```

---

## 6.1 Root and Shell Backgrounds

```css
--color-bg-root: #080B0E;
--color-bg-shell: #10151B;
--color-bg-sidebar: #0D1218;
--color-bg-topbar: #10151B;
--color-bg-screen: #0B0F14;
```


| Token                | Use                                         |
| -------------------- | ------------------------------------------- |
| `--color-bg-root`    | Absolute lowest/root background             |
| `--color-bg-shell`   | Main application shell background           |
| `--color-bg-sidebar` | Sidebar or persistent navigation background |
| `--color-bg-topbar`  | Top bar/header/status strip background      |
| `--color-bg-screen`  | Main screen content area background         |


---

## 6.2 Panels and Surfaces

```css
--color-bg-panel: #171D24;
--color-bg-panel-raised: #202731;
--color-bg-panel-hover: #242D38;
--color-bg-panel-selected: #173C39;
--color-bg-panel-disabled: #11161C;
--color-bg-modal: #151B22;
--color-bg-modal-raised: #1E2630;
--color-bg-tooltip: #111820;
--color-bg-dropdown: #151D25;
```


| Token                       | Use                                               |
| --------------------------- | ------------------------------------------------- |
| `--color-bg-panel`          | Standard panels, cards, containers                |
| `--color-bg-panel-raised`   | Elevated cards, detail panels, nested sections    |
| `--color-bg-panel-hover`    | Hovered rows, cards, buttons, selectable surfaces |
| `--color-bg-panel-selected` | Selected card/row background                      |
| `--color-bg-panel-disabled` | Disabled/locked surfaces                          |
| `--color-bg-modal`          | Modal shell                                       |
| `--color-bg-modal-raised`   | Raised modal sections                             |
| `--color-bg-tooltip`        | Tooltip surface                                   |
| `--color-bg-dropdown`       | Dropdown/context menu surface                     |


---

## 6.3 Text Colours

```css
--color-text-primary: #E6EAF0;
--color-text-secondary: #9CA7B5;
--color-text-muted: #65717F;
--color-text-disabled: #4F5A66;
--color-text-inverse: #071013;
--color-text-accent: #73E0D1;
--color-text-warning: #F0C66A;
--color-text-danger: #FF8FA0;
--color-text-success: #70E2A3;
--color-text-info: #8CC4FF;
--color-text-special: #C4B5FD;
```


| Token                    | Use                                 |
| ------------------------ | ----------------------------------- |
| `--color-text-primary`   | Main readable text                  |
| `--color-text-secondary` | Secondary text, subtitles, metadata |
| `--color-text-muted`     | Low-priority labels and helper text |
| `--color-text-disabled`  | Disabled text                       |
| `--color-text-inverse`   | Text on bright/accent backgrounds   |
| `--color-text-accent`    | Accent labels and selected values   |
| `--color-text-warning`   | Warning text                        |
| `--color-text-danger`    | Danger/error text                   |
| `--color-text-success`   | Success text                        |
| `--color-text-info`      | Informational text                  |
| `--color-text-special`   | Special/advanced/unusual state text |


---

## 6.4 Borders and Dividers

```css
--color-border-subtle: #2B343F;
--color-border-strong: #3A4654;
--color-border-hover: #4B5A68;
--color-border-selected: #45CBB7;
--color-border-focus: #73E0D1;
--color-border-warning: #E2A93B;
--color-border-danger: #F06C82;
--color-border-success: #35D08A;
--color-border-info: #5AA7FF;
--color-border-special: #A78BFA;
--color-divider: #25303A;
```


| Token                     | Use                                   |
| ------------------------- | ------------------------------------- |
| `--color-border-subtle`   | Default panel/card/table borders      |
| `--color-border-strong`   | Higher-emphasis panel borders         |
| `--color-border-hover`    | Hover border state                    |
| `--color-border-selected` | Selected card/row/tab accent          |
| `--color-border-focus`    | Keyboard/controller focus ring        |
| `--color-border-warning`  | Warning panels and validation         |
| `--color-border-danger`   | Danger/destructive panels and errors  |
| `--color-border-success`  | Success panels and validation         |
| `--color-border-info`     | Informational panels                  |
| `--color-border-special`  | Special/advanced/unusual state panels |
| `--color-divider`         | Internal separators                   |


---

## 6.5 Accent and Semantic Colours

```css
--color-accent: #45CBB7;
--color-accent-hover: #62DCCA;
--color-accent-pressed: #2FAE9D;
--color-accent-muted: #164E45;
--color-accent-soft: rgba(69, 203, 183, 0.14);

--color-success: #35D08A;
--color-success-muted: #143F2C;
--color-success-soft: rgba(53, 208, 138, 0.14);

--color-warning: #E2A93B;
--color-warning-muted: #4A3512;
--color-warning-soft: rgba(226, 169, 59, 0.16);

--color-danger: #F06C82;
--color-danger-muted: #4A1F2A;
--color-danger-soft: rgba(240, 108, 130, 0.16);

--color-info: #5AA7FF;
--color-info-muted: #183A5C;
--color-info-soft: rgba(90, 167, 255, 0.14);

--color-special: #A78BFA;
--color-special-muted: #322A55;
--color-special-soft: rgba(167, 139, 250, 0.14);
```


| Colour            | Meaning                                                  |
| ----------------- | -------------------------------------------------------- |
| Accent / Teal     | Primary actions, selected states, active navigation      |
| Success / Green   | Positive outcomes, completed success, valid state        |
| Warning / Amber   | Attention, risk, caution, potential problem              |
| Danger / Red-Pink | Critical risk, destructive action, error, severe failure |
| Info / Blue       | Neutral information, help, guidance                      |
| Special / Purple  | Rare, advanced, unusual, premium, or exceptional state   |


---

## 7. Severity Colour Rules

Use the same severity meaning everywhere.


| Severity | Colour Token                                       | Meaning                                           |
| -------- | -------------------------------------------------- | ------------------------------------------------- |
| Neutral  | `--color-text-secondary` / `--color-border-subtle` | Normal state                                      |
| Info     | `--color-info`                                     | Helpful or informational state                    |
| Positive | `--color-success`                                  | Good outcome, valid state, improvement            |
| Warning  | `--color-warning`                                  | Needs attention, risk, caution                    |
| Danger   | `--color-danger`                                   | Critical risk, destructive action, severe failure |
| Special  | `--color-special`                                  | Rare, advanced, unusual, exceptional              |
| Disabled | Disabled text/surface tokens                       | Unavailable or inactive                           |
| Unknown  | Muted text + subtle/info border                    | Hidden, estimated, unavailable, or uncertain data |


Rules:

```text
Warning is not the same as Danger.
Danger should be rare.
Success should not be used for ordinary selected state.
Accent should not be used for every positive value.
Special should not be used as generic decoration.
Unknown should not look the same as Disabled.
```

---

## 8. Component Colour Rules

### 8.1 Buttons

Primary buttons:

```text
Background = --color-accent
Hover = --color-accent-hover
Pressed = --color-accent-pressed
Text = --color-text-inverse
```

Secondary buttons:

```text
Background = --color-bg-panel-raised
Border = --color-border-strong
Text = --color-text-primary
Hover background = --color-bg-panel-hover
```

Ghost buttons:

```text
Background = transparent
Text = --color-text-secondary
Hover background = --color-bg-panel-hover
Hover text = --color-text-primary
```

Danger buttons:

```text
Background = --color-danger
Hover = --color-danger
Text = --color-text-inverse
```

Disabled buttons:

```text
Background = --color-bg-panel-disabled
Border = --color-border-subtle
Text = --color-text-disabled
```

---

### 8.2 Cards and Panels

Default card:

```text
Background = --color-bg-panel
Border = --color-border-subtle
Text = --color-text-primary
```

Hover card:

```text
Background = --color-bg-panel-hover
Border = --color-border-hover
```

Selected card:

```text
Background = --color-bg-panel-selected
Border = --color-border-selected
Accent = --color-accent
```

Warning card:

```text
Background = --color-warning-soft
Border = --color-border-warning
Text/icon = --color-text-warning
```

Danger card:

```text
Background = --color-danger-soft
Border = --color-border-danger
Text/icon = --color-text-danger
```

Success card:

```text
Background = --color-success-soft
Border = --color-border-success
Text/icon = --color-text-success
```

Info card:

```text
Background = --color-info-soft
Border = --color-border-info
Text/icon = --color-text-info
```

Special card:

```text
Background = --color-special-soft
Border = --color-border-special
Text/icon = --color-text-special
```

---

### 8.3 Tables and Rows

Default row:

```text
Background = transparent or --color-bg-panel
Border/divider = --color-divider
Text = --color-text-primary
```

Hover row:

```text
Background = --color-bg-panel-hover
```

Selected row:

```text
Background = --color-bg-panel-selected
Left rail/border = --color-accent
```

Warning row:

```text
Warning chip + optional --color-warning-soft background
```

Danger row:

```text
Danger chip + optional --color-danger-soft background
```

Rules:

```text
Do not colour entire dense tables aggressively.
Prefer chips, rails, or subtle row backgrounds for status.
Selected state must be visible without relying on shader.
```

---

### 8.4 Inputs and Dropdowns

Input field:

```text
Background = --color-bg-panel
Border = --color-border-subtle
Text = --color-text-primary
Placeholder = --color-text-muted
```

Input focus:

```text
Border = --color-border-focus
Optional focus glow = --color-accent-soft
```

Invalid input:

```text
Border = --color-border-danger
Validation text = --color-text-danger
```

Warning input:

```text
Border = --color-border-warning
Validation text = --color-text-warning
```

Dropdown menu:

```text
Background = --color-bg-dropdown
Border = --color-border-strong
Option hover = --color-bg-panel-hover
Option selected = --color-bg-panel-selected
```

---

### 8.5 Tabs

Inactive tab:

```text
Text = --color-text-secondary
```

Hover tab:

```text
Text = --color-text-primary
Background = --color-bg-panel-hover
```

Active tab:

```text
Text = --color-text-primary
Underline/accent = --color-accent
```

Disabled tab:

```text
Text = --color-text-disabled
```

---

### 8.6 Chips, Badges, and Tags

Neutral:

```text
Background = --color-bg-panel-raised
Border = --color-border-subtle
Text = --color-text-secondary
```

Info:

```text
Background = --color-info-soft
Border = --color-info
Text = --color-text-info
```

Positive:

```text
Background = --color-success-soft
Border = --color-success
Text = --color-text-success
```

Warning:

```text
Background = --color-warning-soft
Border = --color-warning
Text = --color-text-warning
```

Danger:

```text
Background = --color-danger-soft
Border = --color-danger
Text = --color-text-danger
```

Special:

```text
Background = --color-special-soft
Border = --color-special
Text = --color-text-special
```

Unknown:

```text
Background = --color-bg-panel-raised
Border = --color-border-subtle
Text = --color-text-muted
Optional icon = info/unknown
```

---

## 9. Data Visualisation Colour Rules

Data visualisation colours are optional and should only be used when charts, graphs, or visual data components are explicitly required by a GDD or UI page.

### 9.1 Chart Palette

```css
--color-chart-1: #45CBB7;
--color-chart-2: #5AA7FF;
--color-chart-3: #A78BFA;
--color-chart-4: #E2A93B;
--color-chart-5: #35D08A;
--color-chart-6: #F06C82;
--color-chart-grid: #25303A;
--color-chart-axis: #65717F;
--color-chart-label: #9CA7B5;
```

### 9.2 Chart Rules

```text
Use consistent chart colours.
Use semantic colours for success, warning, danger, and info states.
Do not use random colours per chart.
Charts must remain readable on dark backgrounds.
Grid lines must be subtle.
Chart tooltip colours must follow tooltip tokens.
Do not rely on colour alone; use labels, legends, or tooltips.
```

Do not add charts unless the GDD/UI specification requires them.

---

## 10. Shader and Glow Colour Rules

Shader/glow colour usage is allowed only when the relevant UI architecture or polish page approves shader use.

Approved colour mappings:

```text
Primary glow = --color-accent
Success glow = --color-success
Warning glow = --color-warning
Danger glow = --color-danger
Info glow = --color-info
Special glow = --color-special
```

Rules:

```text
Glow must be restrained.
Glow must not replace readable borders or labels.
Glow must not be required to understand state.
Shader-only status states are forbidden.
Shader colours must use palette tokens.
Do not create one-off shader colours.
```

---

## 11. Accessibility and Contrast Rules

### 11.1 General Contrast

```text
Primary text must be readable on all panels.
Secondary text must remain readable at 1920×1080.
Muted text may be subtle but must not become unreadable.
Disabled text must remain readable enough to explain unavailable states.
```

### 11.2 Colour Alone Rule

Never rely on colour alone for:

```text
Warnings
Danger states
Disabled states
Selected states
Locked states
Unknown states
Validation states
Risk states
Unavailable states
```

Always pair colour with at least one of:

```text
Text label
Icon
Tooltip
Border treatment
Pattern or shape
Status chip
```

---

## 12. Forbidden Colour Usage

Forbidden:

```text
Hard-coded colours in C#
Hard-coded colours in UXML
Inline colours for ordinary UI state
One-off colours per screen
One-off chart palettes
One-off button colours
One-off dropdown colours
One-off warning colours
Different selected colours per feature
Bright neon backgrounds
High-saturation full-screen panels
Colour-only warnings
Shader-only status states
Using success green for selected state
Using accent teal for every positive metric
Using danger red for ordinary negative numbers that are not critical
```

---

## 13. Token Naming Rules

Use consistent token names.

Preferred categories:

```text
--color-bg-*
--color-text-*
--color-border-*
--color-accent-*
--color-success-*
--color-warning-*
--color-danger-*
--color-info-*
--color-special-*
--color-chart-*
```

Examples:

```css
--color-bg-panel
--color-text-primary
--color-border-selected
--color-accent
--color-warning-soft
--color-chart-1
```

Do not use vague names:

```css
--blue1
--nice-green
--warning2
--modalColor
--buttonSpecial
--glowThing
```

---

## 14. Required USS Variables

Bezi must create or update:

```text
Assets/Project/UI/USS/Theme/theme-colors.uss
```

Required structure:

```css
:root {
    --color-bg-root: #080B0E;
    --color-bg-shell: #10151B;
    --color-bg-sidebar: #0D1218;
    --color-bg-topbar: #10151B;
    --color-bg-screen: #0B0F14;

    --color-bg-panel: #171D24;
    --color-bg-panel-raised: #202731;
    --color-bg-panel-hover: #242D38;
    --color-bg-panel-selected: #173C39;
    --color-bg-panel-disabled: #11161C;
    --color-bg-modal: #151B22;
    --color-bg-modal-raised: #1E2630;
    --color-bg-tooltip: #111820;
    --color-bg-dropdown: #151D25;

    --color-text-primary: #E6EAF0;
    --color-text-secondary: #9CA7B5;
    --color-text-muted: #65717F;
    --color-text-disabled: #4F5A66;
    --color-text-inverse: #071013;
    --color-text-accent: #73E0D1;
    --color-text-warning: #F0C66A;
    --color-text-danger: #FF8FA0;
    --color-text-success: #70E2A3;
    --color-text-info: #8CC4FF;
    --color-text-special: #C4B5FD;

    --color-border-subtle: #2B343F;
    --color-border-strong: #3A4654;
    --color-border-hover: #4B5A68;
    --color-border-selected: #45CBB7;
    --color-border-focus: #73E0D1;
    --color-border-warning: #E2A93B;
    --color-border-danger: #F06C82;
    --color-border-success: #35D08A;
    --color-border-info: #5AA7FF;
    --color-border-special: #A78BFA;
    --color-divider: #25303A;

    --color-accent: #45CBB7;
    --color-accent-hover: #62DCCA;
    --color-accent-pressed: #2FAE9D;
    --color-accent-muted: #164E45;
    --color-accent-soft: rgba(69, 203, 183, 0.14);

    --color-success: #35D08A;
    --color-success-muted: #143F2C;
    --color-success-soft: rgba(53, 208, 138, 0.14);

    --color-warning: #E2A93B;
    --color-warning-muted: #4A3512;
    --color-warning-soft: rgba(226, 169, 59, 0.16);

    --color-danger: #F06C82;
    --color-danger-muted: #4A1F2A;
    --color-danger-soft: rgba(240, 108, 130, 0.16);

    --color-info: #5AA7FF;
    --color-info-muted: #183A5C;
    --color-info-soft: rgba(90, 167, 255, 0.14);

    --color-special: #A78BFA;
    --color-special-muted: #322A55;
    --color-special-soft: rgba(167, 139, 250, 0.14);

    --color-chart-1: #45CBB7;
    --color-chart-2: #5AA7FF;
    --color-chart-3: #A78BFA;
    --color-chart-4: #E2A93B;
    --color-chart-5: #35D08A;
    --color-chart-6: #F06C82;
    --color-chart-grid: #25303A;
    --color-chart-axis: #65717F;
    --color-chart-label: #9CA7B5;
}
```

---

## 15. Implementation Rules

### 15.1 USS

Colour implementation belongs in USS.

Use:

```text
Assets/Project/UI/USS/Theme/theme-colors.uss
```

or the approved shared theme token file.

Do not duplicate colour tokens across screen-specific USS files.

### 15.2 C#

C# may only apply semantic classes.

Allowed:

```text
row.EnableInClassList("row--warning", vm.IsWarning);
chip.EnableInClassList("chip--danger", vm.IsCritical);
button.EnableInClassList("btn--primary", vm.IsPrimaryAction);
```

Forbidden:

```text
button.style.backgroundColor = new Color(...);
row.style.borderBottomColor = Color.red;
label.style.color = new StyleColor(...);
```

### 15.3 ViewModel

ViewModels should expose semantic states, not colours.

Preferred:

```text
Severity = Warning
AvailabilityState = Disabled
ConfidenceState = Medium
VisualState = Selected
```

Not preferred:

```text
TextColor = "#E2A93B"
GlowColor = "#45CBB7"
BackgroundColor = "#173C39"
```

---

## 16. Adding New Colours

Before adding a new colour, Bezi must check:

```text
Can an existing token be used?
Is this a semantic need or a one-off visual preference?
Should this be a new token?
Does the token belong in COLOUR_PALETTE.md?
Does the token belong in theme-colors.uss?
```

If a new colour is added, update:

```text
/Pages/Private/Project Docs/Colour Palette.md
Assets/Project/UI/USS/Theme/theme-colors.uss
/Pages/Private/Project Docs/Changelog.md
```

Do not add one-off colours inside screen USS files unless explicitly approved.

---

## 17. Final Rule

Use the colour tokens from this document.

Do not invent screen-specific colours.

Do not hard-code colours in C#.

Do not create a new palette per screen.

Keep colours centralized, semantic, readable, and replaceable.
