# Typography

## 1. Purpose

This document defines the project's UI typography rules.

Bezi must use this document when creating UI Toolkit UXML, USS, components, screens, modals, shell layouts, debug UI, and documentation-related UI.

Runtime UI must use the approved project fonts only.

---

## 2. Mandatory Font Files

The project uses these font files:

```text
Main UI Font:
Inter-VariableFont_opsz,wght.ttf

Secondary UI Font:
SpaceGrotesk-VariableFont_wght.ttf

Technical / Debug Font:
JetBrainsMono[wght].ttf
```

Bezi must not replace these fonts with other fonts unless explicitly instructed.

---

## 3. Main UI Font

Primary UI font:

```text
Inter-VariableFont_opsz,wght.ttf
```

Use Inter for:

- Body text
- Labels
- Buttons
- Forms
- Lists
- Tables
- Tooltips
- Notifications
- Standard modals
- General runtime UI

Inter is the default font for most UI.

---

## 4. Secondary UI Font

Secondary UI font:

```text
SpaceGrotesk-VariableFont_wght.ttf
```

Use Space Grotesk for:

- Main menu title text
- Major screen titles
- Section headers
- Large dashboard-style labels
- Brand-like UI moments
- High-emphasis headings

Do not use Space Grotesk for dense body text, long descriptions, tables, or small metadata unless explicitly approved.

---

## 5. Technical / Debug Font

Technical/debug font:

```text
JetBrainsMono[wght].ttf
```

Use JetBrains Mono for:

- Debug overlays
- IDs
- Logs
- Formula inspectors
- Save-data inspectors
- Developer tools
- Console-style text
- Technical readouts

Do not use JetBrains Mono as the main UI font.

---

## 6. Font Asset Rules

UI Toolkit must use Unity Font Assets generated from the approved font files.

Recommended source font locations:

```text
Assets/Project/UI/Assets/Fonts/Inter/Inter-VariableFont_opsz,wght.ttf
Assets/Project/UI/Assets/Fonts/SpaceGrotesk/SpaceGrotesk-VariableFont_wght.ttf
Assets/Project/UI/Assets/Fonts/JetBrainsMono/JetBrainsMono[wght].ttf
```

Recommended generated font asset names:

```text
Inter-Variable SDF.asset
SpaceGrotesk-Variable SDF.asset
JetBrainsMono-Variable SDF.asset
```

If Unity creates different asset names, Bezi must keep the naming clear and update the USS references accordingly.

Do not rely on Unity default fonts for final runtime UI.

---

## 7. Font Usage Hierarchy

Use this hierarchy:

| Role | Font | Usage |
|---|---|---|
| Main UI | Inter | Standard runtime UI |
| Secondary / Display | Space Grotesk | Titles, headers, high-emphasis UI |
| Technical / Debug | JetBrains Mono | Debug, IDs, logs, technical readouts |

Do not invent screen-specific fonts.

Do not mix fonts randomly.

---

## 8. Type Scale

Use this baseline type scale:

| Token | Size | Font | Weight | Usage |
|---|---:|---|---:|---|
| `--font-size-display` | `32px` | Space Grotesk | 700 | Rare major title |
| `--font-size-title` | `24px` | Space Grotesk | 600 | Screen title |
| `--font-size-heading` | `18px` | Space Grotesk | 600 | Section heading |
| `--font-size-body` | `14px` | Inter | 400 | Standard UI text |
| `--font-size-body-strong` | `14px` | Inter | 500 | Emphasised UI text |
| `--font-size-small` | `12px` | Inter | 400 | Metadata/helper text |
| `--font-size-tiny` | `10px` | Inter | 400 | Very low-priority labels |
| `--font-size-code` | `12px` | JetBrains Mono | 400 | Debug/technical text |

Do not create one-off text sizes per screen.

If a new size is required, add it to this document and to the typography USS file.

---

## 9. Line Height Rules

Use readable line heights.

| Text Type | Recommended Line Height |
|---|---:|
| Display / title | `1.15` |
| Heading | `1.2` |
| Body text | `1.35` |
| Dense table/list rows | `1.2` |
| Tooltips | `1.35` |
| Debug monospace text | `1.25` |

Avoid cramped text.

Avoid large blocks of tiny text.

---

## 10. Required USS Typography Variables

Bezi must create or update:

```text
Assets/Project/UI/USS/Theme/theme-typography.uss
```

Required structure:

```css
:root {
    --font-primary: resource("Inter-Variable SDF");
    --font-secondary: resource("SpaceGrotesk-Variable SDF");
    --font-mono: resource("JetBrainsMono-Variable SDF");

    --font-size-display: 32px;
    --font-size-title: 24px;
    --font-size-heading: 18px;
    --font-size-body: 14px;
    --font-size-body-strong: 14px;
    --font-size-small: 12px;
    --font-size-tiny: 10px;
    --font-size-code: 12px;
}
```

If Unity requires a different resource path or syntax after import, Bezi must adjust the USS correctly while preserving the same semantic tokens.

---

## 11. Required USS Classes

Bezi should create reusable typography classes.

```css
.text-display {
    -unity-font-definition: var(--font-secondary);
    font-size: var(--font-size-display);
    -unity-font-style: bold;
}

.text-title {
    -unity-font-definition: var(--font-secondary);
    font-size: var(--font-size-title);
    -unity-font-style: bold;
}

.text-heading {
    -unity-font-definition: var(--font-secondary);
    font-size: var(--font-size-heading);
    -unity-font-style: bold;
}

.text-body {
    -unity-font-definition: var(--font-primary);
    font-size: var(--font-size-body);
}

.text-body-strong {
    -unity-font-definition: var(--font-primary);
    font-size: var(--font-size-body-strong);
    -unity-font-style: bold;
}

.text-small {
    -unity-font-definition: var(--font-primary);
    font-size: var(--font-size-small);
}

.text-tiny {
    -unity-font-definition: var(--font-primary);
    font-size: var(--font-size-tiny);
}

.text-code {
    -unity-font-definition: var(--font-mono);
    font-size: var(--font-size-code);
}
```

Bezi may adjust exact USS syntax if Unity requires it, but the semantic class structure must remain.

---

## 12. C# Font Rule

C# must not assign fonts directly unless there is a specific technical reason.

Preferred:

```csharp
element.AddToClassList("text-title");
```

Avoid:

```csharp
element.style.unityFontDefinition = ...
element.style.fontSize = 17;
```

Use USS classes for typography.

---

## 13. Placeholder Rule

If a required font file has not been imported yet, Bezi may temporarily use a clearly marked placeholder font asset.

The placeholder must be documented in:

```text
/Pages/Private/Project Docs/Changelog.md
/Pages/Private/Project Docs/Typography.md
```

Placeholder fonts must be replaced before final UI work.

---

## 14. Final Rule

Use:

```text
Inter-VariableFont_opsz,wght.ttf = main UI font
SpaceGrotesk-VariableFont_wght.ttf = secondary/display UI font
JetBrainsMono[wght].ttf = technical/debug font
```

Do not invent fonts per screen.

Centralize typography through UI Toolkit Font Assets and USS theme variables.
