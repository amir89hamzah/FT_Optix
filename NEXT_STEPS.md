# Next Steps

## Current stage

Model-folder XML import is working in FT Optix Studio v1.7.2.51.

Confirmed by manual import test:

- Probe 01: success
- Probe 02: success
- Probe 03: pending / next

A BoilerDemo reference sample was reviewed from a `Nodes.zip` export plus runtime screenshots. The raw sample should not be committed until sanitized, but the observed patterns are useful as ground truth for future probes. See:

```text
00_notes/boilerdemo_reference_review.md
```

## Immediate actions

1. Run the generator:

```bash
python 05_scripts/generate_probes.py
```

2. Import each generated XML into FT Optix under the `Model` folder.

3. For the next manual test, prioritize:

```text
04_generated/AI_ModelProbe_03_DashboardRuntimeData.xml
```

4. Export the imported Probe 03 node back from FT Optix.

5. Save exported-back XML into:

```text
02_probes/probe_03_dashboard_runtime_data/exported_back_from_ftoptix.xml
```

6. Compare generated vs exported-back:

```bash
python 05_scripts/compare_ftoptix_export.py \
  04_generated/AI_ModelProbe_03_DashboardRuntimeData.xml \
  02_probes/probe_03_dashboard_runtime_data/exported_back_from_ftoptix.xml
```

7. Record the result in a probe README:

```text
02_probes/probe_03_dashboard_runtime_data/README.md
```

## BoilerDemo-informed next probes

The BoilerDemo sample suggests changing the future probe sequence slightly. Focus on small Model and binding patterns before full UI generation.

- Probe 04: Model object type / instance pattern based on `Model/Pumps` (`MyPump`, `Pump1`, speed/command/alarm variables).
- Probe 05: recipe target model pattern based on `Model/Recipes` plus later `Recipes/RecipeSchema`.
- Probe 06: datalogger-ready model variables and `VariablesToLog` / datastore pattern.
- Probe 07: alarm controller pattern using `InputValue`, `DynamicLink`, `Message`, and `Severity`.
- Probe 08: simple navigation/screen pattern based on `NavigationPanelItem` only after Model/binding patterns are stable.

## Things to verify

- Which FT Optix YAML fields are required vs generated automatically by Studio.
- Whether a minimal `Nodes/Model/...yaml` pattern can be manually added and opened safely by Studio.
- Which patterns should be generated as XML imports under `Model`, and which require project YAML editing.
- How FT Optix normalizes `DynamicLink` paths after save/export.
- What is the smallest possible UI screen/navigation node set that Studio accepts.

## Rule

Do not rush into full dashboard XML. FT Optix XML generation should be built by small passing probes.
