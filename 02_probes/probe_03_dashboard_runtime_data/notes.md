# Probe 03 - Dashboard Runtime Data Model

## Objective

Test a larger Model-folder structure that looks more like runtime HMI data.

## Expected structure

```text
AI_ModelProbe_03
├─ ImportStatus
├─ ProbeVersion
├─ PageDescriptions
├─ Dashboard
│  └─ EnergyConsumption
├─ ProductionLine
└─ AlarmTable
```

## Purpose

This probe prepares model data that could later be bound to UI objects.

It does not create UI pages yet.

## Result

Pending test.

## Things to check in FT Optix

- Folder appears under `Model`
- Nested folders appear correctly
- Dashboard/EnergyConsumption values appear correctly
- ProductionLine sections appear correctly
- AlarmTable rows appear correctly
- No red error/warning icon
