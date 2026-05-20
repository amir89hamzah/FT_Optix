# Next Steps

## Current stage

Model-folder XML import-export is working in FT Optix Studio v1.7.2.51.

Confirmed by manual test:

- Probe 01: success
- Probe 02: success
- Probe 03: success, exported back and compared
- Probe 04: generated, next manual test

A BoilerDemo reference sample was reviewed from a `Nodes.zip` export plus runtime screenshots. The raw sample should not be committed until sanitized, but the observed patterns are useful as ground truth for future probes. See:

```text
00_notes/boilerdemo_reference_review.md
```

## Immediate actions

1. Run the generator:

```bash
python 05_scripts/generate_probes.py
```

2. For the next manual test, prioritize:

```text
04_generated/AI_ModelProbe_04_PumpObjectType.xml
```

3. Import Probe 04 into FT Optix under the `Model` folder.

4. Check whether the import accepts the custom object type and object instances:

```text
AI_MyPump : BaseObjectType
AI_ModelProbe_04/Pumps/Pump1
AI_ModelProbe_04/Pumps/Pump2
```

5. Export the imported Probe 04 node back from FT Optix.

6. Save exported-back XML into:

```text
02_probes/probe_04_pump_object_type/exported_back_from_ftoptix.xml
```

7. Record the result in:

```text
02_probes/probe_04_pump_object_type/README.md
```

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

## BoilerDemo-informed next probes

The BoilerDemo sample suggests changing the future probe sequence slightly. Focus on small Model and binding patterns before full UI generation.

- Probe 04: Model object type / instance pattern based on `Model/Pumps` (`MyPump`, `Pump1`, speed/command/alarm variables). **Generated, pending import-export test.**
- Probe 05: recipe target model pattern based on `Model/Recipes` plus later `Recipes/RecipeSchema`.
- Probe 06: datalogger-ready model variables and `VariablesToLog` / datastore pattern.
- Probe 07: alarm controller pattern using `InputValue`, `DynamicLink`, `Message`, and `Severity`.
- Probe 08: simple navigation/screen pattern based on `NavigationPanelItem` only after Model/binding patterns are stable.

## Things to verify

- Whether FT Optix accepts a minimal `UAObjectType` through NodeSet XML import.
- Whether imported `UAObject` instances preserve a custom `HasTypeDefinition` after export.
- Which FT Optix YAML fields are required vs generated automatically by Studio.
- Whether a minimal `Nodes/Model/...yaml` pattern can be manually added and opened safely by Studio.
- Which patterns should be generated as XML imports under `Model`, and which require project YAML editing.
- How FT Optix normalizes `DynamicLink` paths after save/export.
- What is the smallest possible UI screen/navigation node set that Studio accepts.

## Rule

Do not rush into full dashboard XML. FT Optix XML generation should be built by small passing probes.
