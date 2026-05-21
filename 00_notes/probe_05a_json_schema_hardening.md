# Probe 05A - JSON schema hardening for tag-backed variables

## Why this probe exists

Probe 04A-04E proved that a DesignTime NetLogic path can read JSON, validate it, and create or recreate Model nodes in FT Optix.

However, the earlier JSON shape allowed raw variable values such as:

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "value": 78.4
}
```

That is unsafe as a long-term HMI generation pattern because it does not say whether the number is:

- a mock test value
- a default value
- a static configuration value
- a real runtime process value that should come from a PLC tag

For real HMI work, runtime values should normally come from PLC tags, OPC UA nodes, CommDrivers, or DynamicLinks. Probe 05A therefore hardens the JSON before any DynamicLink / FT Echo / live PLC test.

## Goal

Make every generated variable declare:

```text
name
DataType
role
source.kind
```

The allowed source kinds are:

```text
plcTag  -> future live source / DynamicLink intent
mock    -> test-only value, safe without FT Echo or PLC
static  -> configuration or diagnostic constant, not runtime process data
```

## Current files

```text
06_specs/tag_backed_variables.schema.json
06_specs/examples/valid_tag_backed_variables.json
06_specs/examples/invalid_ambiguous_value.json
05_scripts/validate_json_specs.py
```

## Example plcTag variable

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "role": "measured",
  "source": {
    "kind": "plcTag",
    "tag": "PumpA.CurrentSpeed",
    "mode": "read"
  },
  "display": {
    "unit": "Hz",
    "decimals": 1
  }
}
```

## Example setpoint

```json
{
  "name": "SetSpeed",
  "dataType": "Float",
  "role": "setpoint",
  "source": {
    "kind": "plcTag",
    "tag": "PumpA.SetSpeed",
    "mode": "readWrite"
  },
  "display": {
    "unit": "Hz",
    "decimals": 1,
    "min": 0,
    "max": 100
  }
}
```

## Example mock variable

Mock variables are allowed so the schema and generator can be tested before FT Echo or a dummy PLC is installed.

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "role": "measured",
  "source": {
    "kind": "mock",
    "value": 78.4
  },
  "display": {
    "unit": "Hz",
    "decimals": 1
  }
}
```

## Guardrails

The validator rejects ambiguous raw values:

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "role": "measured",
  "value": 78.4
}
```

It also checks:

- unsupported extra properties
- invalid FT Optix-safe names
- duplicate object names
- duplicate variable names inside one object
- invalid `dataType`
- invalid `role`
- invalid `source.kind`
- mock/static value type mismatch
- setpoints without `display.min` and `display.max`
- measured/status/diagnostic PLC variables not using `read`
- setpoints not using `readWrite`
- commands not using `write` or `readWrite`
- static values used as runtime measured/setpoint/status values

## How to run

```bash
python 05_scripts/validate_json_specs.py \
  06_specs/examples/valid_tag_backed_variables.json
```

Expected:

```text
OK   06_specs/examples/valid_tag_backed_variables.json
```

Negative test:

```bash
python 05_scripts/validate_json_specs.py \
  06_specs/examples/invalid_ambiguous_value.json
```

Expected:

```text
FAIL 06_specs/examples/invalid_ambiguous_value.json
```

## What Probe 05A does not do yet

Probe 05A does not create real DynamicLinks yet.

For `source.kind = plcTag`, the current goal is only to preserve source intent safely in JSON. Probe 05B or later should discover how to create the correct FT Optix `DynamicLink` nodes using DesignTime NetLogic and, later, verify them against FT Echo / dummy PLC tags.
