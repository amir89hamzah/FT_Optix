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

Confirmed by manual tests and XML pattern extraction:

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
- Probe 05D-2: manual FactoryTalk Logix Echo / RA EtherNet/IP live REAL tag read/write using `PumpA_Speed` and FT Optix `EditableLabel`. **Tested / pass**.
- Probe 05D-3: manual FactoryTalk Logix Echo / RA EtherNet/IP live BOOL tag read using `PumpA_Running` and FT Optix Key-value converter text mapping. **Tested / pass**.
- Probe 05D-4: manual FactoryTalk Logix Echo / RA EtherNet/IP live DINT command write using `PumpA_Command` and FT Optix `EditableLabel`. **Tested / pass**.
- Probe 05F: exported XML pattern extraction for live tag UI bindings. **Pattern extracted**.
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
→ controls, alarms, trends, recipes, logging
```

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

Probe 05D-1 through 05D-4 confirmed manual live read, write, BOOL display mapping, and DINT command write through FactoryTalk Logix Echo and RA EtherNet/IP into FT Optix runtime.

Test tags:

```text
PumpA_Speed     REAL
PumpA_Running   BOOL
PumpA_Command   DINT
```

Observed live tag path format from exported XML:

```text
/Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/{TagName}
```

Observed manual read path:

```text
FactoryTalk Logix Echo / Studio 5000 controller tag
→ FT Optix RAEtherNet_IPDriver station
→ RA EtherNet/IP tag importer
→ imported controller tag under CommDrivers
→ UI label DynamicLink
→ runtime value update
```

Observed manual write path:

```text
FT Optix runtime EditableLabel
→ imported RA EtherNet/IP tag PumpA_Speed
→ FactoryTalk Logix Echo / Studio 5000 controller tag
→ read-only FT Optix display linked to same tag follows updated value
```

Observed BOOL text mapping path:

```text
Studio 5000 BOOL tag PumpA_Running
→ imported RA EtherNet/IP tag PumpA_Running
→ Label.Text complex dynamic link
→ Key-value converter
→ Running / Not run runtime text
```

Observed DINT command write path:

```text
FT Optix runtime EditableLabel2
→ StringFormatter {0:d}
→ ReadWrite DynamicLink to PumpA_Command
→ Studio 5000 / Logix Designer PumpA_Command monitor value updates
```

## XML patterns now known

### REAL read display

```text
Label.Text
└─ StringFormatter1
   ├─ Format = {0:n1}
   └─ Source0.DynamicLink = live PumpA_Speed path
```

### REAL editable read/write

```text
EditableLabel.Text
└─ StringFormatter1
   ├─ Format = {0:n1}
   ├─ Mode = 2
   └─ Source0.DynamicLink = live PumpA_Speed path
      └─ Mode = 2
```

### BOOL status text

```text
Label.Text
└─ KeyValueConverter1
   ├─ Source.DynamicLink = live PumpA_Running path
   └─ Pairs
      ├─ false -> Not run
      └─ true  -> Running
```

### DINT command input

```text
EditableLabel.Text
└─ StringFormatter1
   ├─ Format = {0:d}
   ├─ Mode = 2
   └─ Source0.DynamicLink = live PumpA_Command path
      └─ Mode = 2
```

## Current priority

### Probe 05G - Minimal generator for live-tag UI bindings

Status:

```text
PLANNED / NEXT CHECKPOINT
```

Purpose:

```text
Create the smallest possible generator or NetLogic probe that recreates the proven XML patterns.
```

Do not generate a full HMI yet.

Minimum generated screen should attempt only:

```text
1. Label for PumpA_Speed using StringFormatter {0:n1}
2. EditableLabel for PumpA_Command using StringFormatter {0:d} and ReadWrite mode
3. Label for PumpA_Running using KeyValueConverter false/true text mapping
```

Key open technical question:

```text
Which exact FT Optix C# API calls create StringFormatter, KeyValueConverter, converter pairs, and dynamic link mode nodes under a UI object's Text property?
```

If the API is unclear, create a tiny C# probe first and compare the exported XML with Probe 05F.

## Completed records

```text
02_probes/probe_05a_json_schema_hardening/README.md
02_probes/probe_05b_tag_metadata_generator/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/README.md
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
02_probes/probe_05c4_dynamiclink_mode_syntax/README.md
02_probes/probe_05c4_dynamiclink_mode_syntax/exported_AI_JsonDynamicLinkProbe_01_readwrite.xml
02_probes/probe_05d_ft_echo_live_tag_verification/README.md
02_probes/probe_05f_xml_pattern_extraction/README.md
09_netlogic_probes/probe_05b_tag_metadata_generator/
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/
10_milestones/milestone_06_json_model_dynamiclinks/
```

## Later, not now

After minimal live-tag UI generation is proven, the repo should move toward generated HMI screens in this order:

```text
1. JSON to simple overview screen
2. UI labels and displays linked to live/model variables
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
