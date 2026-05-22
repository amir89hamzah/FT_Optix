# Next Steps

## Current project framing

FT_Optix is a reproducible LLM-to-FactoryTalk-Optix HMI build path.

The repo exists so that a user can give HMI intent in natural language, an LLM can read the repo when it does not know the FactoryTalk Optix pattern, and the LLM can generate a runnable FT Optix HMI through proven steps instead of guessing.

```text
User HMI intent
→ LLM reads repo
→ hardened JSON spec
→ DesignTime NetLogic generator
→ FT Optix Model nodes
→ DynamicLinks
→ FT Echo / PLC live tags
→ UI screens
→ controls, alarms, trends
→ runnable HMI
```

## Confirmed status

Confirmed by manual tests:

- Probe 01: Model-folder XML basic import success.
- Probe 02: nested Model folder structure success.
- Probe 03: success, exported back and compared.
- Probe 04 XML: generated, now secondary.
- Probe 05A: JSON schema hardening for tag-backed variables is **tested / pass**.
- Probe 05B: DesignTime NetLogic reads hardened JSON and creates Model nodes with source intent metadata. **Tested / pass**.
- Probe 05C-1: local manual DynamicLink from `LinkedSpeed` to sibling `SourceSpeed`. **Tested / pass**.
- Probe 05C-2: exported DynamicLink storage pattern. **Tested / pass**.
- Probe 05C-3: DesignTime NetLogic creates local DynamicLink automatically with `linkedSpeed.SetDynamicLink(sourceSpeed)`. **Tested / pass**.
- Milestone 06: combined JSON -> Model variables -> `_LocalSources` -> local DynamicLinks -> runtime/emulator value update. **Tested / pass**.

## Main strategy

```text
AI / natural-language intent
→ hardened JSON spec
→ JSON schema + semantic validator
→ DesignTime NetLogic C# generator
→ FT Optix native Model nodes
→ local DynamicLinks first
→ FT Echo / PLC-backed DynamicLinks
→ generated UI screens
→ commands, alarms, trends, recipes, logging
```

FT Echo is not needed for Milestone 06. FT Echo becomes relevant only when the source is a real PLC/dummy controller tag instead of local simulated source variables.

## Current priority

### Milestone 07 / Probe 05D - FT Echo or dummy PLC live tag verification

Status:

```text
PLANNED / NEXT CHECKPOINT
```

Purpose:

```text
Prove that source.kind = plcTag can bind to FT Echo or dummy PLC live tags, not only to local _LocalSources.
```

Target flow:

```text
Hardened JSON plcTag intent
→ generated FT Optix variable
→ DynamicLink / tag binding to communication path
→ FT Echo or dummy PLC tag
→ runtime value update
```

Do not proceed to dashboard, full UI generation, alarms, recipes, dataloggers, or trends until the live tag checkpoint is clear.

## Milestone 07 test order

### Probe 05D-1 - Manual read tag

Objective:

```text
Prove FT Optix can manually read one FT Echo / dummy PLC tag.
```

Test tag:

```text
PumpA.CurrentSpeed
```

Expected result:

```text
Controller tag value 12.3 → FT Optix runtime display 12.3
Controller tag value 45.6 → FT Optix runtime display 45.6
```

Pass criteria:

```text
FT Optix runtime follows live/dummy controller tag changes.
```

### Probe 05D-2 - Manual readWrite tag

Objective:

```text
Prove FT Optix can write from runtime back to a live/dummy controller tag.
```

Test tag:

```text
PumpA.SetSpeed
```

Expected result:

```text
Change value in FT Optix runtime
→ PumpA.SetSpeed changes in FT Echo / dummy controller
```

### Probe 05D-3 - Manual Boolean read tag

Objective:

```text
Prove Boolean datatype mapping works for live/dummy controller tags.
```

Test tag:

```text
PumpA.Running
```

Expected result:

```text
false → FT Optix shows false
true  → FT Optix shows true
```

### Probe 05D-4 - Manual write command tag

Objective:

```text
Prove a command string can be written from FT Optix to the controller tag.
```

Test tag:

```text
PumpA.OperatorCommand
```

Expected result:

```text
FT Optix writes START
→ PumpA.OperatorCommand = START in FT Echo / dummy controller
```

### Probe 05D-5 - Generator live DynamicLink

Objective:

```text
Update the generator only after manual live tag binding passes.
```

Expected generator behavior:

```text
source.kind = plcTag
→ create generated Model variable
→ attach DynamicLink to real communication/tag path
→ preserve SourceKind, SourceTag, SourceMode metadata
```

## Completed records

```text
02_probes/probe_05a_json_schema_hardening/README.md
02_probes/probe_05b_tag_metadata_generator/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
09_netlogic_probes/probe_05b_tag_metadata_generator/
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/
10_milestones/milestone_06_json_model_dynamiclinks/
```

## Later, not now

After Milestone 07 passes, the repo should move toward generated HMI screens in this order:

```text
1. JSON to simple overview screen
2. UI labels and displays linked to Model variables
3. UI input controls for readWrite variables
4. Command buttons for write variables
5. Alarms
6. Trends and datalogging
7. Recipes
8. One-prompt HMI build test
```

## Rule

Do not proceed silently between stages. Record status, explain the next objective, provide manual test steps, then continue only after the checkpoint is clear.

If an LLM does not know how to perform the next FactoryTalk Optix step, it must create a small probe first and record:

```text
Objective
Files used
Manual steps
Expected result
Actual result
PASS / FAIL
Known working API or pattern
Known failing API or pattern
Next decision
```
