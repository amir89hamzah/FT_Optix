# Generator guardrails - PLC tags and DynamicLinks

## Key correction

The numeric values used in early probes are **test scaffolding only**.

In a real FT Optix HMI project, most process values should not be hardcoded constants. They should normally be linked to PLC tags through the appropriate FT Optix communication path, such as OPC-UA, CommDrivers, or another configured data source.

## Why early probes used constants

Probes 04A to 04E used constants such as:

```text
SetSpeed = 50
CurrentSpeed = 47.5
Running = True
```

Those values were used only to prove that the generator can create:

```text
objects
variables
data types
values
JSON parsing
file reading
validation
delete/recreate behavior
```

They are not the desired final HMI data strategy.

## Required direction from now on

Future JSON schema and generator work should distinguish between:

```text
1. staticValue / defaultValue
   Used only for test, simulation, default values, or offline mockups.

2. plcTag / dynamicLink
   Used for real runtime values from PLC or data source.
```

A future variable spec should look more like this:

```json
{
  "name": "CurrentSpeed",
  "dataType": "Float",
  "source": {
    "kind": "dynamicLink",
    "path": "<PLC or FT Optix tag path here>",
    "mode": "read"
  }
}
```

or, for writeable setpoints:

```json
{
  "name": "SetSpeed",
  "dataType": "Float",
  "source": {
    "kind": "dynamicLink",
    "path": "<PLC or FT Optix tag path here>",
    "mode": "readWrite"
  }
}
```

## Design rule

Do not let the generator drift into creating a fake HMI full of constants.

Constants are acceptable only for:

```text
probe validation
mock data
initial/default values
recipe default rows
UI labels/text
engineering units
limits used for display configuration
```

Runtime process values should be driven by:

```text
PLC tags
OPC-UA tags
CommDriver tags
DynamicLink references
expression/converter outputs based on live tags
```

## Impact on future probes

The next major probes should move from constant values toward link-aware JSON:

```text
05A: JSON schema extension for tag-backed variables
05B: create variables with DynamicLink targets
05C: recipe setpoint variables linked to PLC setpoints
06: datalogger variables linked to live tags
07: alarm input values linked to live tags
08: UI display widgets linked to live tags
```

## Open questions

- What exact FT Optix C# API is best for creating DynamicLinks?
- What is the correct browse path format for project-local model variables vs PLC/CommDriver tags?
- Should the JSON schema store raw FT Optix DynamicLink paths, PLC tag symbolic names, or both?
- How should read-only measured values differ from read/write setpoints?
- How should scaling, units, limits, and alarm thresholds be represented?
