# Next Steps

## Current stage

Model-folder XML import-export is working in FT Optix Studio v1.7.2.51.

Confirmed by manual test:

- Probe 01: success
- Probe 02: success
- Probe 03: success, exported back and compared
- Probe 04 XML: generated, but now secondary / hold while DesignTime NetLogic path is evaluated
- Probe 04A-04E: DesignTime NetLogic JSON generation path was explored manually; it proved the engine direction but exposed the need to avoid ambiguous constant runtime values
- Probe 05A: JSON schema hardening for tag-backed variables has started

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

Prioritize Probe 05A before deeper Recipe/Datalogger/Alarm/UI probes.

### Probe 05A - JSON schema hardening for tag-backed variables

Goal:

```text
Make AI-generated JSON explicit about variable role and source intent before any live PLC / FT Echo / DynamicLink test.
```

Current files:

```text
06_specs/tag_backed_variables.schema.json
06_specs/examples/valid_tag_backed_variables.json
06_specs/examples/invalid_ambiguous_value.json
05_scripts/validate_json_specs.py
```

Expected validation commands:

```bash
python 05_scripts/validate_json_specs.py \
  06_specs/examples/valid_tag_backed_variables.json

python 05_scripts/validate_json_specs.py \
  06_specs/examples/invalid_ambiguous_value.json
```

Expected result:

```text
valid_tag_backed_variables.json       -> OK
invalid_ambiguous_value.json          -> FAIL
```

The invalid example must fail because it uses a raw `value` directly on a variable instead of declaring:

```text
source.kind = plcTag
source.kind = mock
source.kind = static
```

### Probe 05B - Preserve source intent in generated FT Optix Model nodes

Next after Probe 05A passes.

Goal:

```text
Use DesignTime NetLogic to read the hardened JSON and create Model variables while preserving source intent as metadata/helper variables, without creating real DynamicLinks yet.
```

Example generated structure:

```text
Model
└─ AI_TagSchemaProbe_01
   ├─ PumpA
   │  ├─ CurrentSpeed
   │  ├─ CurrentSpeed_SourceKind
   │  ├─ CurrentSpeed_SourceTag
   │  ├─ SetSpeed
   │  ├─ SetSpeed_SourceKind
   │  └─ SetSpeed_SourceTag
   └─ PumpB
      ├─ CurrentSpeed
      └─ CurrentSpeed_SourceKind
```

### Probe 05C - DynamicLink pattern discovery

Only after 05A/05B are stable.

Goal:

```text
Learn the smallest reliable FT Optix DesignTime NetLogic API pattern to create DynamicLink nodes.
```

This can be explored without a real PLC first, but FT Echo / dummy PLC will eventually be needed to prove live values.

## Optional XML action

Probe 04 XML is still available:

```text
04_generated/AI_ModelProbe_04_PumpObjectType.xml
```

It can be tested later to check whether FT Optix accepts `UAObjectType` and object instances through NodeSet XML import. This is useful, but less important than proving the JSON -> DesignTime NetLogic generation path.

## Last completed verification

Probe 03 result:

```text
Expected nodes: 100
Exported nodes: 100
Missing paths: 0
Extra paths: 0
UAObjects: 21
UAVariables: 79
```

FT Optix preserved all browse paths, structure, data types, values, and descriptions. FT Optix normalized namespace URI, model URI, node IDs, and access-level attributes on export.

## BoilerDemo-informed future probes

Focus on small Model and binding patterns before full UI generation.

- Probe 05A: JSON schema hardening for tag-backed variables. **Current priority.**
- Probe 05B: Generate Model variables from hardened JSON and preserve source intent as metadata/helper variables.
- Probe 05C: DynamicLink pattern discovery using DesignTime NetLogic.
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

## Things to verify

- Whether FT Optix has any official external SDK equivalent to Studio 5000 LDSDK. Current evidence still points to NetLogic rather than a full external SDK.
- Whether DesignTime NetLogic can create all required objects: screens, panels, navigation, dataloggers, recipes, alarms, dynamic links, and aliases.
- Whether a DesignTime NetLogic can import a JSON spec generated by AI and create a project structure safely.
- Whether FT Optix accepts a minimal `UAObjectType` through NodeSet XML import.
- Whether imported `UAObject` instances preserve a custom `HasTypeDefinition` after export.
- Which FT Optix YAML fields are required vs generated automatically by Studio.
- How FT Optix normalizes `DynamicLink` paths after save/export.
- How to create DynamicLink nodes safely from DesignTime NetLogic.

## Rule

Do not rush into full dashboard XML. FT Optix generation should be built by small passing probes, with hardened JSON and DesignTime NetLogic treated as the likely primary automation route.
