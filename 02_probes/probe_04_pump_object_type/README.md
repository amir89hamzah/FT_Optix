# Probe 04 - Pump object type and instance

## Status

Result: **generated, waiting for manual FT Optix import/export test**.

This probe is inspired by the BoilerDemo `Model/Pumps` pattern.

## Goal

Verify whether FT Optix accepts a minimal custom OPC UA object type plus object instances through NodeSet XML import under the `Model` folder.

This is the first step beyond simple Folder + Variable structures.

## Generated file

Run:

```bash
python 05_scripts/generate_probes.py
```

Then import this file under the FT Optix `Model` folder:

```text
04_generated/AI_ModelProbe_04_PumpObjectType.xml
```

## Expected imported structure

Expected visible Model structure:

```text
Model
└─ AI_ModelProbe_04
   ├─ ImportStatus
   ├─ ProbeVersion
   └─ Pumps
      ├─ Pump1
      │  ├─ SetSpeed
      │  ├─ CurrentSpeed
      │  ├─ Command
      │  ├─ Alarm
      │  ├─ MinSpeed
      │  ├─ MaxSpeed
      │  └─ UseRunFeedback
      └─ Pump2
         ├─ SetSpeed
         ├─ CurrentSpeed
         ├─ Command
         ├─ Alarm
         ├─ MinSpeed
         ├─ MaxSpeed
         └─ UseRunFeedback
```

The XML also declares a custom object type:

```text
AI_MyPump : BaseObjectType
```

With mandatory type variables:

```text
SetSpeed
CurrentSpeed
Command
Alarm
MinSpeed
MaxSpeed
UseRunFeedback
```

## What to check manually

After import, check:

1. Does `AI_ModelProbe_04` appear under `Model`?
2. Do `Pump1` and `Pump2` appear under `AI_ModelProbe_04/Pumps`?
3. Are `Pump1` and `Pump2` shown as object instances or plain folders?
4. Does the custom type `AI_MyPump` appear anywhere in FT Optix type view?
5. Do all child variables import with correct values and data types?
6. Can the imported node be exported back to NodeSet XML?

## Export-back target

If import succeeds, export `AI_ModelProbe_04` back from FT Optix and save as:

```text
02_probes/probe_04_pump_object_type/exported_back_from_ftoptix.xml
```

## Interpretation rules

Possible outcomes:

### Outcome A - full type success

FT Optix imports `AI_MyPump` as an object type, and `Pump1` / `Pump2` preserve that type relationship after export.

This means the generator can start supporting custom object type patterns.

### Outcome B - instance structure success, type relationship normalized away

FT Optix imports `Pump1` / `Pump2` with all child variables, but does not preserve the generated custom type relationship.

This is still useful. It means we can continue with explicit object instances and child variables, then revisit custom type generation later.

### Outcome C - import error

FT Optix rejects the XML because of the `UAObjectType` or modelling-rule references.

In that case, the next version should use only explicit `UAObject` instances first, without custom `UAObjectType` declarations.
