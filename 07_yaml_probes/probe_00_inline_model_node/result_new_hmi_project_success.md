# Result - NewHMIProject Inline Model YAML Success

## Test project

```text
NewHMIProject
```

## Test method

The generated YAML nodes were pasted directly into:

```text
Nodes/Model/Model.yaml
```

No external `File:` include was used for the generated probe.

## Result

```text
Success
```

FT Optix Studio opened the project successfully and displayed the generated Model nodes.

Observed under `Model`:

```text
StatusText
TestNumber
TestBool
Notes
RuntimeData
```

Observed selected variable:

```text
StatusText = Hello from YAML project source
```

## Important observation

The generated nodes were accepted directly under `Model`, without needing a parent folder wrapper.

This means both of the following inline styles should be tested and supported by the generator:

```text
Model
├─ StatusText
├─ TestNumber
└─ RuntimeData
```

and:

```text
Model
└─ AI_InlineProbe_01
   ├─ StatusText
   └─ TestNumber
```

## Conclusion

Manual inline YAML modification works in a blank FT Optix project.

Previous `Missing file` failures were related to external YAML file include/path behavior, not basic generated node structure.

## Next step

Move to YAML Probe 03:

```text
Model variable → UI Label DynamicLink
```

The next test should create a UI label that reads from:

```text
/Objects/NewHMIProject/Model/StatusText
```

or a relative path if created inside a reusable component.
