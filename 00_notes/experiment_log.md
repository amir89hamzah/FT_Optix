# Experiment Log

## 2026-05-18

### Probe 01

Generated a basic Model-only XML file.

Result:

```text
Success
```

FT Optix accepted:

- root folder
- String variable
- Float variable
- Boolean variable

### Probe 02

Generated nested Model structure with process-like runtime values.

Result:

```text
Success
```

FT Optix accepted:

- nested folders
- multiple data types
- repeated ProductionLine stage structure

### Probe 03

Generated dashboard-style runtime model data.

Result:

```text
Pending test
```

Purpose:

- repeated object structure
- dashboard data model
- alarm-table row structure
- page description path structure

## Working principle

Do not jump directly to full FT Optix UI/dashboard generation.

Use this loop:

```text
Small XML probe → import into FT Optix → screenshot/result → export back → compare → improve generator
```
