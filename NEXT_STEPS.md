# Next Steps

## Current stage

The project has moved from many small probes into a combined generator milestone.

Confirmed by manual test:

- Probe 01: Model-folder XML basic import success.
- Probe 02: nested Model folder structure success.
- Probe 03: success, exported back and compared.
- Probe 04 XML: generated, now secondary.
- Probe 05A: JSON schema hardening for tag-backed variables is **tested / pass**.
- Probe 05B: DesignTime NetLogic reads hardened JSON and creates Model nodes with source intent metadata. **Tested / pass**.
- Probe 05C-1: local manual DynamicLink from `LinkedSpeed` to sibling `SourceSpeed`. **Tested / pass**.
- Probe 05C-2: exported DynamicLink storage pattern. **Tested / pass**.
- Probe 05C-3: DesignTime NetLogic creates local DynamicLink automatically with `linkedSpeed.SetDynamicLink(sourceSpeed)`. **Tested / pass**.
- Milestone 06: combined JSON -> Model variables -> local DynamicLinks template added. **Manual FT Optix test pending**.

## Main strategy

```text
AI/JSON spec
→ JSON schema + semantic validator
→ DesignTime NetLogic C# generator
→ FT Optix native Model nodes
→ local DynamicLinks first
→ later FT Echo / PLC-backed DynamicLinks
```

FT Echo is not needed for Milestone 06. FT Echo becomes relevant only when the source is a real PLC/dummy controller tag instead of local simulated source variables.

## Current priority

### Milestone 06 - JSON to Model variables to local DynamicLinks

Status:

```text
TEMPLATE ADDED / MANUAL TEST PENDING
```

Files:

```text
10_milestones/milestone_06_json_model_dynamiclinks/README.md
10_milestones/milestone_06_json_model_dynamiclinks/DesignTimeJsonModelDynamicLinkGenerator.cs
```

Goal:

```text
Read hardened JSON from Probe 05A.
Create Model objects and variables.
For source.kind = mock, write the mock value directly.
For source.kind = static, write the static config value directly.
For source.kind = plcTag, create a local simulated source under _LocalSources and DynamicLink the generated variable to that source.
```

Expected output:

```text
Model
└─ AI_JsonDynamicLinkProbe_01
   ├─ _LocalSources
   │  └─ PumpA
   │     ├─ CurrentSpeed
   │     ├─ SetSpeed
   │     ├─ Running
   │     └─ OperatorCommand
   ├─ PumpA
   │  ├─ CurrentSpeed    -> DynamicLink to _LocalSources/PumpA/CurrentSpeed
   │  ├─ SetSpeed        -> DynamicLink to _LocalSources/PumpA/SetSpeed
   │  ├─ Running         -> DynamicLink to _LocalSources/PumpA/Running
   │  ├─ OperatorCommand -> DynamicLink to _LocalSources/PumpA/OperatorCommand
   │  └─ DisplayName     -> static value
   └─ PumpB
      ├─ CurrentSpeed    -> mock value
      ├─ SetSpeed        -> mock value
      └─ Running         -> mock value
```

Manual test input path expected by the template:

```text
C:\Temp\ftoptix_milestone06_tag_backed_variables.json
```

Copy this repo file to that path before running the method:

```text
06_specs/examples/valid_tag_backed_variables.json
```

Method to run in FT Optix:

```text
GenerateJsonModelWithLocalDynamicLinks()
```

Pass criteria:

```text
1. Method compiles.
2. Method runs without error.
3. AI_JsonDynamicLinkProbe_01 appears under Model.
4. _LocalSources appears under the generated root.
5. PumpA variables are DynamicLinked to local source variables.
6. PumpB mock values are written directly.
7. Runtime/emulator shows at least one PumpA linked value following its local source.
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

After Milestone 06 passes, the next major direction is:

```text
Milestone 07 / Probe 05D - FT Echo or dummy PLC live tag verification
```

Do not jump into full dashboard, Recipe, Datalogger, Alarm, or UI generation yet.

## Rule

Do not proceed silently between stages. Record status, explain the next objective, provide manual test steps, then continue only after the checkpoint is clear.
