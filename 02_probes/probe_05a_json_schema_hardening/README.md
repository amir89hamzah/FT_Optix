# Probe 05A - JSON schema hardening result

## Status

```text
TESTED / PASS
```

Status basis:

```text
User-confirmed validation pass in project chat.
```

Date recorded:

```text
2026-05-22
```

## What was tested

Probe 05A validates the hardened JSON structure for tag-backed variables before any FT Optix / NetLogic / DynamicLink test.

This confirms the project should no longer accept ambiguous raw runtime values such as:

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "role": "measured",
  "value": 78.4
}
```

Runtime or test values must instead declare their source intent explicitly:

```text
source.kind = plcTag
source.kind = mock
source.kind = static
```

## Files involved

```text
06_specs/tag_backed_variables.schema.json
06_specs/examples/valid_tag_backed_variables.json
06_specs/examples/invalid_ambiguous_value.json
05_scripts/validate_json_specs.py
```

## Expected validation behavior

Valid example:

```bash
python 05_scripts/validate_json_specs.py \
  06_specs/examples/valid_tag_backed_variables.json
```

Expected:

```text
OK   06_specs/examples/valid_tag_backed_variables.json
```

Invalid example:

```bash
python 05_scripts/validate_json_specs.py \
  06_specs/examples/invalid_ambiguous_value.json
```

Expected:

```text
FAIL 06_specs/examples/invalid_ambiguous_value.json
```

## Result

Probe 05A is accepted as passed and can be used as the gate before Probe 05B.

## Next checkpoint

Do not proceed silently into later probes.

Next manual checkpoint:

```text
Probe 05B - DesignTime NetLogic reads the hardened JSON and creates Model nodes with source metadata.
```

Before running FT Optix, prepare a clear step-by-step manual test plan and expected result.
