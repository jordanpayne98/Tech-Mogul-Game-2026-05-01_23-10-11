# Changelog

This file records implementation changes made to the project.

Bezi must update this file after every implementation task that changes project files.

This is not the GDD.

This is not a design source of truth.

It is a human-readable implementation history.

---

## 2. Working Registry Location

This document is a **standard**. It defines rules and entry format only. It must not contain actual changelog entries.

Actual changelog entries are recorded in:

```text
/Pages/Private/Changelog/Changelog.md
```

---

## Changelog Rules

Each update should summarize:

- What changed
- Files touched
- What was added
- What was removed
- What was refactored
- What placeholders remain
- Follow-up required

Avoid vague entries.

Bad:

```text
Updated stuff.
Improved UI.
Cleaned code.
```

Good:

```text
Added ScreenRouter.cs to centralize UI Toolkit screen navigation.
Created MainShell.uxml and MainShell.uss for root UI structure.
Added placeholder screen IDs pending final GDD screen list.
```

---

## Entry Format

Use this format:

```markdown
## [YYYY-MM-DD] — Short Task Title

### Added
- Added [specific feature/system/file].

### Changed
- Changed [specific behaviour/file/structure].

### Fixed
- Fixed [specific issue].

### Removed
- Removed [specific file/old behaviour].

### Refactored
- Refactored [specific code area] to improve [reason].

### Documentation
- Updated [specific documentation file].

### Placeholders
- Added placeholder [specific placeholder] pending GDD decision.

### Follow-up Required
- [Specific follow-up item].
```

If a section has no entries, omit that section for that update.
