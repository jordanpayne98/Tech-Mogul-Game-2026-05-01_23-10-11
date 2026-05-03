# UI Sizing and Spacing Standard

## 1. Purpose

This document defines the locked runtime UI sizing, spacing, density, layout scaling, panel padding, modal sizing, row height, button height, and responsive behaviour rules for the project.

Bezi must use this document when creating UI Toolkit UXML, USS, screens, modals, components, shell layouts, lists, tables, forms, overlays, debug UI, and reusable UI components.

This document exists so Bezi does not invent one-off spacing, sizing, padding, density, modal dimensions, button sizes, row heights, or screen-specific layout rules.

---

## 2. Status

Status: Locked foundation standard.

This is not a feature page.

This is not a screen list.

This is not optional guidance.

Feature pages define what appears on a screen.

This document defines how UI scale, spacing, sizing, density, and layout rhythm must work when UI exists.

---

## 3. Required References

This document must be used alongside:

```text
/Pages/Private/Project Docs/Project Rules.md
/Pages/Private/Project Docs/Coding Standard.md
/Pages/Private/Project Docs/UI Architecture.md
/Pages/Private/Project Docs/Colour Palette.md
/Pages/Private/Project Docs/Typography.md
```

---

## 4. Core Rule

```text
UI must be designed for 1920×1080 first, but must scale safely to other common desktop resolutions.
```

Do not build fragile fixed-position UI.

Do not use random spacing values per screen.

Do not use one-off component sizes.

Do not make dense UI unreadable.

Do not make layouts wasteful at 1920×1080.

---

## 5. Target Resolution and Scaling

### 5.1 Primary Design Resolution

Primary target resolution:

```text
1920×1080
```

All main UI should look correct at 1920×1080.

This means:

```text
Text is readable.
Panels have enough breathing room.
Tables/lists can show useful information.
Modals do not dominate the entire screen.
Buttons are easy to identify.
Spacing feels dense but not cramped.
```

### 5.2 Minimum Practical Resolution

Minimum practical desktop resolution:

```text
1366×768
```

The UI does not need to look equally spacious at 1366×768, but it must remain usable.

At smaller widths or heights:

```text
Panels may stack.
Non-critical columns may collapse.
Scrolling may be used.
Large modals may reduce max height.
Dense layouts may use compact spacing.
```

Do not allow essential controls to disappear off-screen.

### 5.3 High Resolution Behaviour

At higher resolutions such as:

```text
2560×1440
3440×1440
3840×2160
```

UI should not stretch endlessly.

Use max widths for dense content areas.

Use flexible empty space carefully.

Wide screens should improve readability and layout capacity, not create huge stretched panels.

### 5.4 Scaling Principle

Use responsive UI Toolkit layout:

```text
Flex layouts
Percentage widths where appropriate
Min/max dimensions
Shared spacing tokens
Reusable classes
Scroll views for overflow
```

Avoid:

```text
Large absolute-positioned layouts
Hard-coded screen coordinates
Screen-specific magic numbers
Pixel-perfect assumptions that break at other resolutions
```

---

## 6. Density Direction

The project uses a:

```text
Professional desktop-style dense UI.
```

The UI should feel:

```text
Structured
Readable
Efficient
Information-rich
Controlled
Consistent
```

The UI should not feel:

```text
Mobile-first
Oversized
Toy-like
Sparse without reason
Cramped or unreadable
Randomly spaced
```

---

## 7. Spacing Token System

All spacing must use approved spacing tokens.

Recommended file:

```text
Assets/Project/UI/USS/Theme/theme-spacing.uss
```

Required spacing variables:

```css
:root {
    --space-0: 0px;
    --space-1: 2px;
    --space-2: 4px;
    --space-3: 6px;
    --space-4: 8px;
    --space-5: 12px;
    --space-6: 16px;
    --space-7: 20px;
    --space-8: 24px;
    --space-9: 32px;
    --space-10: 40px;
    --space-11: 48px;
    --space-12: 64px;
}
```

Rules:

```text
Use --space-4, --space-5, --space-6, and --space-8 most often.
Use --space-1, --space-2, and --space-3 only for tight internal detail.
Use --space-10, --space-11, and --space-12 only for major structural spacing.
Do not invent values like 13px, 17px, 23px, or 37px unless explicitly justified.
```

---

## 8. Layout Width Tokens

Recommended file:

```text
Assets/Project/UI/USS/Theme/theme-sizing.uss
```

Required width variables:

```css
:root {
    --width-content-narrow: 640px;
    --width-content-standard: 960px;
    --width-content-wide: 1280px;
    --width-content-max: 1600px;

    --width-sidebar-compact: 64px;
    --width-sidebar-standard: 240px;
    --width-sidebar-wide: 280px;

    --width-panel-small: 320px;
    --width-panel-medium: 480px;
    --width-panel-large: 720px;
    --width-panel-xlarge: 960px;
}
```

| Token | Use |
|---|---|
| `--width-content-narrow` | Focused content, forms, smaller flows |
| `--width-content-standard` | Standard readable content width |
| `--width-content-wide` | Wider management/data screens |
| `--width-content-max` | Max width for large desktop layouts |
| `--width-sidebar-compact` | Icon-only navigation, if used |
| `--width-sidebar-standard` | Standard side navigation, if used |
| `--width-sidebar-wide` | Wider side panels, if required |
| `--width-panel-small` | Small side panels |
| `--width-panel-medium` | Medium cards/forms |
| `--width-panel-large` | Large detail panels |
| `--width-panel-xlarge` | Large modals or complex panels |

---

## 9. Height Tokens

Required height variables:

```css
:root {
    --height-topbar: 56px;
    --height-footer: 40px;

    --height-button-compact: 28px;
    --height-button-standard: 34px;
    --height-button-large: 40px;

    --height-input-standard: 34px;
    --height-input-large: 40px;

    --height-row-compact: 28px;
    --height-row-standard: 34px;
    --height-row-large: 44px;

    --height-tab-standard: 36px;
    --height-chip: 24px;
}
```

Rules:

```text
Use compact heights for dense lists and utility controls.
Use standard heights for normal buttons, rows, and inputs.
Use large heights only for primary actions, onboarding, major flows, or touch-friendly controls if later required.
```

---

## 10. Padding Tokens

Required padding variables:

```css
:root {
    --padding-panel-compact: 12px;
    --padding-panel-standard: 16px;
    --padding-panel-large: 24px;

    --padding-card-compact: 10px;
    --padding-card-standard: 14px;
    --padding-card-large: 20px;

    --padding-modal-standard: 24px;
    --padding-modal-large: 32px;

    --padding-input-x: 10px;
    --padding-button-x: 14px;
    --padding-button-large-x: 18px;
}
```

| Token | Use |
|---|---|
| `--padding-panel-compact` | Dense panels |
| `--padding-panel-standard` | Default panels |
| `--padding-panel-large` | Important or spacious panels |
| `--padding-card-compact` | Dense cards/list items |
| `--padding-card-standard` | Standard cards |
| `--padding-card-large` | Emphasised cards |
| `--padding-modal-standard` | Standard modal body |
| `--padding-modal-large` | Large modal body |
| `--padding-input-x` | Horizontal input padding |
| `--padding-button-x` | Standard button horizontal padding |
| `--padding-button-large-x` | Large button horizontal padding |

---

## 11. Gap Tokens

Required gap variables:

```css
:root {
    --gap-tight: 4px;
    --gap-small: 8px;
    --gap-standard: 12px;
    --gap-large: 16px;
    --gap-section: 24px;
    --gap-major: 32px;
}
```

Usage:

```text
--gap-tight = icon/text pairs, dense metadata
--gap-small = small form groups, row internals
--gap-standard = normal component spacing
--gap-large = panel internal group spacing
--gap-section = between sections
--gap-major = major screen regions
```

---

## 12. Border Radius Tokens

Required radius variables:

```css
:root {
    --radius-none: 0px;
    --radius-small: 4px;
    --radius-standard: 6px;
    --radius-large: 8px;
    --radius-xl: 12px;
    --radius-pill: 999px;
}
```

Rules:

```text
Use --radius-standard for most panels/buttons.
Use --radius-large for modals and major cards.
Use --radius-pill for chips/badges only.
Do not randomly vary corner radius per screen.
```

---

## 13. Border Width Tokens

Required border variables:

```css
:root {
    --border-width-thin: 1px;
    --border-width-standard: 1px;
    --border-width-strong: 2px;
    --border-width-focus: 2px;
}
```

Rules:

```text
Most borders should be 1px.
Focus and selected state borders may use 2px.
Do not use thick decorative borders unless a polish rule explicitly approves it.
```

---

## 14. Shadow and Elevation Tokens

Runtime UI should use subtle elevation only.

Required variables:

```css
:root {
    --shadow-panel: 0 4px 12px rgba(0, 0, 0, 0.22);
    --shadow-modal: 0 16px 40px rgba(0, 0, 0, 0.45);
    --shadow-tooltip: 0 8px 24px rgba(0, 0, 0, 0.35);
}
```

Rules:

```text
Use shadows sparingly.
Do not use heavy bright glows for normal elevation.
Do not make every card cast a strong shadow.
Elevation must support readability, not decoration.
```

---

## 15. Standard Layout Classes

Bezi should create reusable USS classes for common spacing and sizing.

Recommended file:

```text
Assets/Project/UI/USS/Theme/theme-layout.uss
```

Suggested classes:

```css
.layout-root {
    flex-grow: 1;
    background-color: var(--color-bg-root);
}

.layout-screen {
    flex-grow: 1;
    padding: var(--space-8);
    background-color: var(--color-bg-screen);
}

.layout-screen--dense {
    padding: var(--space-6);
}

.layout-row {
    flex-direction: row;
}

.layout-column {
    flex-direction: column;
}

.layout-gap-tight {
    gap: var(--gap-tight);
}

.layout-gap-small {
    gap: var(--gap-small);
}

.layout-gap-standard {
    gap: var(--gap-standard);
}

.layout-gap-large {
    gap: var(--gap-large);
}

.layout-gap-section {
    gap: var(--gap-section);
}

.panel {
    padding: var(--padding-panel-standard);
    border-radius: var(--radius-standard);
    border-width: var(--border-width-standard);
}

.panel--compact {
    padding: var(--padding-panel-compact);
}

.panel--large {
    padding: var(--padding-panel-large);
}

.card {
    padding: var(--padding-card-standard);
    border-radius: var(--radius-standard);
}

.card--compact {
    padding: var(--padding-card-compact);
}

.card--large {
    padding: var(--padding-card-large);
}
```

Bezi may adjust exact USS syntax if Unity requires it, but the token-based structure must remain.

---

## 16. Screen Layout Rules

### 16.1 General Screen Padding

Default screen padding at 1920×1080:

```text
24px
```

Use:

```css
padding: var(--space-8);
```

Dense screens may use:

```css
padding: var(--space-6);
```

Do not use less than `16px` outer padding for main screens unless the UI spec explicitly requires a dense full-frame layout.

### 16.2 Content Width

Most content should not stretch endlessly.

For readable content, use max widths:

```text
Standard content max = 960px
Wide content max = 1280px
Maximum major content width = 1600px
```

Use full width only for:

```text
Dashboards
Large data views
Large tables
Grid-heavy views
Split-panel layouts
Screens that explicitly need full available width
```

### 16.3 Screen Regions

Use clear region spacing.

Recommended region gaps:

```text
Between major screen regions: 24px to 32px
Between panels in a grid: 16px to 24px
Inside panels: 12px to 16px
Inside dense rows: 4px to 8px
```

---

## 17. Modal Sizing Rules

Modals must be sized by purpose.

Use these modal size bands:

```css
:root {
    --modal-width-small: 420px;
    --modal-width-standard: 640px;
    --modal-width-large: 860px;
    --modal-width-xlarge: 1080px;

    --modal-max-height: 86%;
}
```

| Modal Size | Width | Use |
|---|---:|---|
| Small | `420px` | Simple confirmation, small choice |
| Standard | `640px` | Forms, focused detail |
| Large | `860px` | Multi-section detail |
| XLarge | `1080px` | Complex workflow or large detail surface |

Rules:

```text
Modals must not exceed 86% of screen height.
Modal content must scroll internally if needed.
Primary action row should remain visible where possible.
Do not use full-screen modals unless explicitly required.
Do not create arbitrary modal widths.
```

---

## 18. Button Sizing Rules

Buttons must use standard heights.

```text
Compact button = 28px
Standard button = 34px
Large button = 40px
```

Use compact buttons for:

```text
Dense toolbars
Table row actions
Small utility controls
```

Use standard buttons for:

```text
Normal UI actions
Forms
Modals
Screen actions
```

Use large buttons for:

```text
Primary onboarding-style actions
Major call-to-action flows
Important standalone actions
```

Minimum button width:

```text
Compact: 72px
Standard: 88px
Large: 112px
```

Icon-only buttons:

```text
Compact: 28×28px
Standard: 34×34px
Large: 40×40px
```

Rules:

```text
Do not create one-off button heights.
Do not make primary and secondary buttons different heights in the same row.
Do not use tiny buttons for destructive or important actions.
```

---

## 19. Input Sizing Rules

Inputs must use standard heights.

```text
Standard input = 34px
Large input = 40px
```

Use standard input for most forms.

Use large input only for major flows or prominent search/filter fields.

Horizontal input padding:

```text
10px
```

Rules:

```text
Labels must be close enough to their input to clearly belong together.
Validation messages must appear directly below or beside the related input.
Do not use inconsistent input heights within the same form.
```

---

## 20. List and Table Density Rules

Use standard row heights.

```text
Compact row = 28px
Standard row = 34px
Large row = 44px
```

Use compact rows for:

```text
Dense lists
Log-style views
Metadata-heavy tables
Developer/debug lists
```

Use standard rows for:

```text
Most tables
Most selectable lists
Standard data views
```

Use large rows for:

```text
Rows with avatars/icons
Rows with multi-line text
Rows requiring more visual breathing room
```

Rules:

```text
Do not use row heights below 28px.
Do not over-pad dense tables.
Do not colour dense rows aggressively.
Use chips, rails, icons, and text labels for state.
```

---

## 21. Chip and Badge Sizing Rules

Chip height:

```text
24px
```

Chip horizontal padding:

```text
8px to 10px
```

Chip gap between icon and text:

```text
4px
```

Rules:

```text
Chips must remain readable.
Do not use chips as large buttons unless explicitly designed.
Do not create one-off chip heights.
```

---

## 22. Icon Sizing Rules

Use these icon sizes:

```css
:root {
    --icon-size-xs: 12px;
    --icon-size-small: 16px;
    --icon-size-standard: 20px;
    --icon-size-large: 24px;
    --icon-size-xl: 32px;
}
```

| Token | Use |
|---|---|
| `--icon-size-xs` | Tiny metadata, dense table indicators |
| `--icon-size-small` | Buttons, chips, secondary labels |
| `--icon-size-standard` | Standard navigation/action icons |
| `--icon-size-large` | Major buttons, modal headers |
| `--icon-size-xl` | Rare large visual emphasis |

Rules:

```text
Do not use random icon sizes.
Do not use oversized icons in dense UI.
Icon-only buttons must still have readable tooltip/accessibility labels where needed.
```

---

## 23. Tooltip and Notification Sizing

Tooltip:

```text
Max width = 360px
Padding = 8px to 12px
```

Notification:

```text
Width = 360px to 480px
Padding = 12px to 16px
```

Rules:

```text
Tooltips should be short.
Notifications should not block core UI unless explicitly designed.
Long tooltip text should become a modal or help panel instead.
```

---

## 24. Scroll Behaviour

Use scroll views when content exceeds available space.

Rules:

```text
Do not allow content to overflow off-screen.
Do not hide important primary actions below long scroll content without clear affordance.
Avoid nested scroll views unless necessary.
Tables/lists may scroll independently.
Modal bodies may scroll while header/footer remain fixed where practical.
```

---

## 25. Responsive Breakpoints

Use these conceptual desktop breakpoints:

```text
Small desktop: 1366×768 to 1599×899
Standard desktop: 1600×900 to 1919×1079
Primary target: 1920×1080
Large desktop: 2560×1440+
Ultrawide: width above 3000px
```

Behaviour:

```text
Small desktop:
- Use compact spacing where needed.
- Prefer scroll views over clipping.
- Reduce outer padding from 24px to 16px where needed.
- Avoid opening oversized modals.

Standard desktop:
- Use standard spacing.
- Keep layouts efficient.

Primary target:
- Use full default layout.
- Main screens should look best here.

Large desktop:
- Respect max widths.
- Avoid endless stretched panels.
- Use additional space for readable split layouts only if designed.

Ultrawide:
- Keep central content constrained.
- Do not stretch dense UI across the entire width without reason.
```

---

## 26. Scaling Implementation Rules

Use USS classes and layout constraints.

Allowed:

```text
flex-grow
flex-shrink
min-width
max-width
min-height
max-height
percentage widths
ScrollView overflow
shared sizing tokens
```

Avoid:

```text
Absolute positioning for major layouts
Hard-coded screen coordinates
Per-resolution duplicated UXML
Separate UI trees for every resolution
One-off pixel values per screen
```

---

## 27. UI Toolkit USS Variable Requirements

Bezi must create or update:

```text
Assets/Project/UI/USS/Theme/theme-spacing.uss
Assets/Project/UI/USS/Theme/theme-sizing.uss
Assets/Project/UI/USS/Theme/theme-layout.uss
```

These files must contain the spacing, sizing, and reusable layout classes defined in this document.

If an existing shared theme file already contains equivalent tokens, Bezi must align names and values instead of creating duplicates.

---

## 28. C# Sizing Rule

C# must not hard-code layout sizes unless there is a specific runtime technical reason.

Preferred:

```csharp
element.AddToClassList("panel--compact");
element.AddToClassList("layout-gap-standard");
```

Avoid:

```csharp
element.style.width = 437;
element.style.marginLeft = 13;
element.style.paddingTop = 19;
```

If runtime sizing is unavoidable, it must use named constants or values from an approved config, not unexplained magic numbers.

---

## 29. ViewModel Rule

ViewModels should expose semantic layout states, not raw pixel values.

Preferred:

```text
Density = Compact
LayoutMode = Split
VisualState = Selected
OverflowState = Scrollable
```

Not preferred:

```text
PanelWidth = 743
TopMargin = 17
RowHeight = 31
```

Raw size values in ViewModels are allowed only for data visualisation or technical UI where the value is genuinely data-driven.

---

## 30. Forbidden Sizing and Spacing Usage

Forbidden:

```text
One-off spacing per screen
Random margins
Hard-coded C# layout colours or sizes
Hard-coded UXML inline sizes for ordinary layout
Absolute positioning for major UI surfaces
Different button heights in the same action row
Different row heights in the same table without reason
Tiny unreadable dense UI
Oversized mobile-style controls in desktop UI
Full-screen modals for small tasks
Panels stretched endlessly on ultrawide screens
Content clipped off-screen
Important actions hidden without clear scroll/overflow behaviour
```

---

## 31. Required Completion Checks

After implementing UI sizing or spacing work, Bezi must check:

```text
UI is correct at 1920×1080.
UI remains usable at 1366×768.
UI does not stretch badly at 2560×1440.
No Canvas/UGUI runtime UI was added.
No hard-coded one-off layout values were added unnecessarily.
Theme spacing/sizing tokens were used.
Major layouts use flex/min/max/scroll behaviour.
Buttons use approved heights.
Rows use approved heights.
Modals use approved size bands.
C# does not directly control ordinary layout sizes.
```

---

## 32. Final Rule

Design for 1920×1080 first.

Scale safely to smaller and larger desktop resolutions.

Use shared spacing, sizing, and layout tokens.

Do not invent one-off sizes.

Do not create fragile fixed layouts.

Keep the UI dense, readable, professional, and maintainable.
