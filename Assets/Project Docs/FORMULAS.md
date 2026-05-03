# Formula Reference

## 1. Purpose

This document records every important formula, calculation, scoring rule, weighting rule, probability rule, rounding rule, clamp rule, and derived-value rule used by the project.

The formula reference exists so the project's mathematical behaviour can be inspected without searching through code.

Bezi must update this document whenever implementation adds or changes a formula.

---

## 2. Current Status

Status: Foundation document.

This document defines the formula standard and entry format. Actual formula entries are recorded in `/Pages/Private/Formula/Formula Registry.md`.

No game-specific formulas are locked yet.

Bezi must not add game-specific formula entries until the GDD defines the related system or the user explicitly instructs it.

---

## 3. Formula Documentation Rule

Every important formula must have:

- Stable formula ID
- Name
- Purpose
- Formula
- Inputs
- Output
- Rounding rules
- Clamp rules
- Update timing
- Source
- Implemented code location
- Status
- Notes

If code and this document conflict, Bezi must report the conflict.

Do not silently choose one.

---

## 4. Formula ID Rules

Formula IDs must be stable and semantic.

Examples:

```text
formula.example_score
formula.example_cost
formula.example_probability
formula.example_duration
```

Do not use undefined game-specific formula IDs before the GDD defines the relevant systems.

If a formula is temporary, mark it clearly:

```text
Status: Placeholder
```

If a formula is locked by the GDD, mark it clearly:

```text
Status: Locked
```

---

## 5. Tuning Relationship

If a formula uses a tuning value, document the tuning dependency.

Example:

```text
Uses tuning value:
- TuningConfig.DefaultActionDelaySeconds
```

If a tuning value changes formula behaviour, Bezi must update:

```text
/Pages/Private/Formula/Formula Registry.md
/Pages/Private/Tuning/Tuning Registry.md
/Pages/Private/Changelog/Changelog.md
```

---

## 6. Formula Entry Template

Use this template for every formula:

```markdown
## Formula ID: formula.example_id

### Name
Readable formula name.

### Status
Locked / Placeholder / Pending GDD

### Purpose
What this formula is used for.

### Formula
Result = InputA + InputB

### Inputs
| Input | Type | Range | Description |
|---|---:|---:|---|
| InputA | int | 0+ | Description |
| InputB | int | 0+ | Description |

### Output
| Output | Type | Range | Description |
|---|---:|---:|---|
| Result | int | 0+ | Description |

### Rounding Rules
Describe exact rounding.

### Clamp Rules
Describe min/max limits.

### Update Timing
When the formula is calculated.

### Tuning Dependencies
List tuning values used, if any.

### Source
GDD page, implementation page, or user prompt that defined the formula.

### Implemented In
Code file and method where the formula exists.

### Notes
Any edge cases or pending decisions.
```

---

## 7. Rounding Standard

Every formula that produces decimals but outputs whole numbers must define rounding.

Do not rely on implicit default rounding.

Allowed rounding terms:

```text
Round down
Round up
Round to nearest
Banker's rounding
Truncate
Keep decimal precision
```

Use the method required by the GDD or implementation page.

If not specified, mark the formula as pending GDD decision.

---

## 8. Clamp Standard

Every formula with a minimum or maximum output must define clamp rules.

Example:

```text
Clamp Result between 0 and 100.
```

If no clamp is required, state:

```text
No clamp.
```

---

## 9. Placeholder Formula Rule

Placeholder formulas are allowed only when required for scaffolding.

They must be clearly marked:

```text
Status: Placeholder
```

They must be updated or removed once the GDD defines the final behaviour.

Placeholder formulas must not be treated as final balance.

---
