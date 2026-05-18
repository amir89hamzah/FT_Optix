# Next Steps

## Current stage

Model-folder XML import is working in FT Optix Studio v1.7.2.51.

Confirmed by manual import test:

- Probe 01: success
- Probe 02: success
- Probe 03: pending / next

## Immediate actions

1. Run the generator:

```bash
python 05_scripts/generate_probes.py
```

2. Import each generated XML into FT Optix under the `Model` folder.

3. Export the imported node back from FT Optix.

4. Save exported-back XML into:

```text
02_probes/<probe_name>/exported_back_from_ftoptix.xml
```

5. Compare generated vs exported-back:

```bash
python 05_scripts/compare_ftoptix_export.py 04_generated/AI_ModelProbe_02_ProcessModel.xml 02_probes/probe_02_nested_model/exported_back_from_ftoptix.xml
```

## Future probes

- Probe 04: simple UI binding target model data
- Probe 05: navigation command model
- Probe 06: alarm-table model structure
- Probe 07: UI/Page XML after Model import pattern is stable

## Rule

Do not rush into full dashboard XML. FT Optix XML generation should be built by small passing probes.
