# Probe 02 - Nested Model Structure

## Objective

Test whether FT Optix accepts nested folders and multiple variable types under the `Model` folder.

## Expected structure

```text
AI_ModelProbe_02
├─ ImportStatus
├─ ProbeVersion
├─ Navigation
├─ Dashboard
├─ ProductionLine
└─ AlarmSummary
```

## Result

Success in FT Optix Studio v1.7.2.51.

## Tested concepts

- nested folders
- repeated stage objects
- String, Float, Boolean, Int32, UInt32, DateTime variables
- stable parent-child structure

## Next action

Export `AI_ModelProbe_02` back from FT Optix and compare the exported XML against the generated XML.
