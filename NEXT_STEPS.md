# Next Steps

## Current stage

Model-folder XML import-export is working in FT Optix Studio v1.7.2.51.

Confirmed by manual test:

- Probe 01: success
- Probe 02: success
- Probe 03: success, exported back and compared
- Probe 04 XML: generated, but now secondary / hold while DesignTime NetLogic path is evaluated
- Probe 04A-04E: DesignTime NetLogic JSON generation path was explored manually; it proved the engine direction but exposed the need to avoid ambiguous constant runtime values
- Probe 05A: JSON schema hardening for tag-backed variables is **tested / pass**. Result recorded in `02_probes/probe_05a_json_schema_hardening/README.md`.
- Probe 05B: DesignTime NetLogic reads hardened JSON and creates Model nodes with source intent metadata. **Tested / pass**. Result recorded in `02_probes/probe_05b_tag_metadata_generator/README.md`.
- Probe 05C-1: Local manual DynamicLink from `LinkedSpeed` to sibling `SourceSpeed` is **tested / pass**. Result recorded in `02_probes/probe_05c_dynamiclink_pattern_discovery/README.md`.
- Probe 05C-2: Exported DynamicLink storage pattern is **tested / pass**. Export stored in `02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml`.
- Probe 05C-3: DesignTime NetLogic DynamicLink generator template has been added. Manual FT Optix test is pending.

A BoilerDemo reference sample was reviewed from a `Nodes.zip` export plus runtime screenshots. The raw sample should not be committed until sanitized, but the observed patterns are useful as ground truth for future probes. See:

```text
00_notes/boilerdemo_reference_review.md
```

NetLogic / C# API and tooling notes from Rockwell Knowledgebase PDFs were also recorded. See:

```text
00_notes/netlogic_csharp_api_and_tooling_notes.md
```

Probe 05A schema hardening notes are recorded here:

```text
00_notes/probe_05a_json_schema_hardening.md
```

Probe 05B DesignTime NetLogic template is recorded here:

```text
09_netlogic_probes/probe_05b_tag_metadata_generator/
```

Probe 05C DynamicLink discovery result is recorded here:

```text
02_probes/probe_05c_dynamiclink_pattern_discovery/README.md
```

Probe 05C-3 DesignTime NetLogic template is recorded here:

```text
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/
```

## Strategy update

The current best automation strategy is:

```text
AI/JSON spec                     -> desired project/model intent
JSON schema + semantic validator -> safety gate before generation
DesignTime NetLogic C# API       -> primary project-generation path
NodeSet XML import/export        -> verification and regression-test path
YAML project files               -> pattern research and diff/reference path
```

Reason:

- XML import/export has proven useful for Model-folder structure verification.
- YAML is excellent for learning FT Optix project patterns from real samples, but direct manual editing may be fragile.
- FT Optix C# / NetLogic supports `.NET 6` bindings and additional NuGet packages, and examples/snippets are available from the FactoryTalk Optix GitHub resources.
- NetLogic tooling may require VS Code / C# / .NET / NuGet setup and may hit cache or NuGet restore issues on some machines.
- Runtime HMI values must not drift into fake constants. JSON must say whether a variable is backed by a future PLC tag, a mock test value, or a safe static config value.

## Immediate actions

Run Probe 05C-3 manual FT Optix test.

### Probe 05C-3 - DesignTime NetLogic creates the same local DynamicLink automatically

Status:

```text
TEMPLATE ADDED / MANUAL TEST PENDING
```

Template:

```text
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/DesignTimeDynamicLinkGenerator.cs
```

Goal:

```text
Use DesignTime NetLogic to create:
Model/AI_DynamicLinkNetLogicProbe_01/SourceSpeed
Model/AI_DynamicLinkNetLogicProbe_01/LinkedSpeed
and link LinkedSpeed to SourceSpeed.
```

Important note:

```text
The first code attempt uses linkedSpeed.SetDynamicLink(sourceSpeed, DynamicLinkMode.ReadWrite).
If this does not compile in the user's FT Optix version, capture the compiler error and update the probe based on the exact API available.
```

Expected generated structure:

```text
Model
└─ AI_DynamicLinkNetLogicProbe_01
   ├─ SourceSpeed
   └─ LinkedSpeed
```

Expected runtime behavior:

```text
Changing SourceSpeed should update LinkedSpeed / linked UI display.
```

Scope limit:

```text
No FT Echo requirement yet.
No full PLC tag browser requirement yet.
No dashboard generation.
No Recipe/Datalogger/Alarm generation yet.
```

FT Echo should be reserved for Probe 05D when the goal becomes live PLC tag verification.

## Completed Probe 05C findings

### 05C-1 local manual DynamicLink

Status:

```text
TESTED / PASS
```

Observed working local pattern:

```text
Model
└─ AI_DynamicLinkProbe_01
   ├─ SourceSpeed
   └─ LinkedSpeed -> ../SourceSpeed
```

Observed runtime/emulator behavior:

```text
SourceSpeed = 12.3  -> linked display 12.3
SourceSpeed = 45.6  -> linked display 45.599998
```

### 05C-2 exported storage pattern

Status:

```text
TESTED / PASS
```

Export showed:

```text
LinkedSpeed
└─ DynamicLink = ../SourceSpeed
   └─ Mode = 2
```

## BoilerDemo-informed future probes

Focus on small Model and binding patterns before full UI generation.

- Probe 05A: JSON schema hardening for tag-backed variables. **Tested / pass.**
- Probe 05B: Generate Model variables from hardened JSON and preserve source intent as metadata/helper variables. **Tested / pass.**
- Probe 05C-1: Local manual DynamicLink pattern. **Tested / pass.**
- Probe 05C-2: Export / inspect DynamicLink storage pattern. **Tested / pass.**
- Probe 05C-3: DesignTime NetLogic creates the same local DynamicLink automatically. **Template added; manual FT Optix test pending.**
- Probe 05D: FT Echo / dummy PLC live tag verification.
- Probe 06: recipe target model pattern based on `Model/Recipes` plus later `Recipes/RecipeSchema`.
- Probe 07: datalogger-ready model variables and `VariablesToLog` / datastore pattern.
- Probe 08: alarm controller pattern using `InputValue`, `DynamicLink`, `Message`, and `Severity`.
- Probe 09: simple navigation/screen pattern based on `NavigationPanelItem` only after Model/binding patterns are stable.

## Tooling checks for NetLogic machines

Before serious DesignTime NetLogic work, verify:

```text
FactoryTalk Optix Studio installed
Visual Studio Code installed
C# extension installed
.NET Install Tool installed
C# Snippets installed
NuGet Package Manager GUI installed
NuGet/network/proxy works if packages must be restored
```

Known failure types from Rockwell Knowledgebase:

```text
NU1301: Unable to load service index from nuget.org
NU1100: Unable to resolve Microsoft.NETCore.App.Ref / WindowsDesktop.App.Ref / AspNetCore.App.Ref for net6.0
```

Known cleanup actions:

```text
Clear VS Code cache folders under AppData/Roaming/Code
Delete AppData/Roaming/NuGet/NuGet.config
```

## Rule

Do not rush into full dashboard XML. FT Optix generation should be built by small passing probes, with hardened JSON and DesignTime NetLogic treated as the likely primary automation route.

Do not proceed silently between probes. Record status, explain the next objective, provide manual test steps, then continue only after the checkpoint is clear.
