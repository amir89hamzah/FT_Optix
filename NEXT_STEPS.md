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
- Probe 05C-4: DesignTime NetLogic explicit DynamicLink mode syntax using `FTOptix.CoreBase.DynamicLinkMode.ReadWrite`. **Tested / pass**.
- Probe 05D-1: manual FactoryTalk Logix Echo / RA EtherNet/IP live REAL tag read using `PumpA_Speed`. **Tested / pass**.
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

## DynamicLink mode finding

Probe 05C-4 confirmed that explicit mode syntax compiles and works when the enum is fully qualified:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.ReadWrite);
```

Use this mapping for future generator work:

```text
source.mode = read      -> FTOptix.CoreBase.DynamicLinkMode.Read
source.mode = write     -> FTOptix.CoreBase.DynamicLinkMode.Write
source.mode = readWrite -> FTOptix.CoreBase.DynamicLinkMode.ReadWrite
```

Exported ReadWrite DynamicLinks showed:

```text
Mode = 2
```

## Live tag finding

Probe 05D-1 confirmed manual live read from FactoryTalk Logix Echo through RA EtherNet/IP into FT Optix runtime.

Observed manual path:

```text
FactoryTalk Logix Echo / Studio 5000 controller tag
→ FT Optix RAEtherNetIPDriver station
→ RA EtherNet/IP tag importer
→ imported controller tag under CommDrivers
→ UI label DynamicLink
→ runtime value update
```

Test tag:

```text
PumpA_Speed   REAL
```

Observed result:

```text
PumpA_Speed = 123.4  → FT Optix runtime display = 123.4
PumpA_Speed = 567.77 → FT Optix runtime display = 567.8
```

The `567.77 -> 567.8` display is accepted as formatting/rounding behavior.

## Current priority

### Probe 05D-2 - Manual readWrite live tag

Status:

```text
PLANNED / NEXT CHECKPOINT
```

Purpose:

```text
Prove FT Optix can write a runtime value back to a FactoryTalk Logix Echo / dummy PLC tag.
```

Recommended first test tag:

```text
PumpA_Speed
```

Reason:

```text
PumpA_Speed is already imported and proven readable in 05D-1.
Use the known-good path first before creating a separate SetSpeed tag.
```

Expected result:

```text
Change value from FT Optix runtime
→ PumpA_Speed changes in Studio 5000 / Logix Designer monitor view
```

Do not proceed to generator live DynamicLinks, dashboard generation, alarms, recipes, dataloggers, or trends until the manual live write/readWrite checkpoint is clear.

## Milestone 07 / Probe 05D test order

### Probe 05D-1 - Manual read tag

Status:

```text
TESTED / PASS
```

Actual tested tag:

```text
PumpA_Speed
```

Observed result:

```text
123.4 -> 123.4 displayed in FT Optix runtime
567.77 -> 567.8 displayed in FT Optix runtime
```

### Probe 05D-2 - Manual readWrite tag

Objective:

```text
Prove FT Optix can write from runtime back to a live/dummy controller tag.
```

Recommended tag:

```text
PumpA_Speed
```

Expected result:

```text
Change value in FT Optix runtime
→ PumpA_Speed changes in Studio 5000 / Logix Designer
```

### Probe 05D-3 - Manual Boolean read tag

Objective:

```text
Prove Boolean datatype mapping works for live/dummy controller tags.
```

Suggested tag:

```text
PumpA_Running
```

Expected result:

```text
false → FT Optix shows false
true  → FT Optix shows true
```

### Probe 05D-4 - Manual write command tag

Objective:

```text
Prove a command string or command value can be written from FT Optix to the controller tag.
```

Suggested tag:

```text
PumpA_Command
```

Expected result:

```text
FT Optix writes a test command/value
→ PumpA_Command changes in Studio 5000 / Logix Designer
```

### Probe 05D-5 - Generator live DynamicLink

Objective:

```text
Update the generator only after manual live tag read/write binding passes.
```

Expected generator behavior:

```text
source.kind = plcTag
→ create generated Model variable
→ attach DynamicLink to real communication/tag path
→ preserve SourceKind, SourceTag, SourceMode metadata
→ apply SourceMode using FTOptix.CoreBase.DynamicLinkMode
```

## Completed records

```text
02_probes/probe_05a_json_schema_hardening/README.md
02_probes/probe_05b_tag_metadata_generator/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
02_probes/probe_05c4_dynamiclink_mode_syntax/README.md
02_probes/probe_05c4_dynamiclink_mode_syntax/exported_AI_JsonDynamicLinkProbe_01_readwrite.xml
02_probes/probe_05d_ft_echo_live_tag_verification/README.md
09_netlogic_probes/probe_05b_tag_metadata_generator/
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/
10_milestones/milestone_06_json_model_dynamiclinks/
```

## Later, not now

After live read/write passes, the repo should move toward generated HMI screens in this order:

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
