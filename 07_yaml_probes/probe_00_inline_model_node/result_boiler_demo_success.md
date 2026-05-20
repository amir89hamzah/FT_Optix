# Result - BoilerDemo Inline Model Node Success

## Test project

```text
BoilerDemo
```

## Test method

Instead of adding a new YAML file using `File:` include, the generated node was pasted directly into:

```text
Nodes/Model/Model.yaml
```

## Result

```text
Success
```

FT Optix Studio opened the project successfully and displayed the generated node under `Model`.

Observed generated node:

```text
Model
└─ AI_InlineProbe_01
   ├─ StatusText
   └─ TestNumber
```

The selected `StatusText` variable showed:

```text
Hello from inline YAML
```

## Meaning

This confirms that FT Optix accepts manually added generated YAML nodes when they are inserted inline into an existing category YAML file.

## Important conclusion

The generated node format is valid enough for FT Optix.

The previous failure with:

```yaml
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

was most likely related to file include/path/project-source registration behavior, not the node content itself.

## Next strategy

Use inline YAML generation first.

Later, investigate `File:` include behavior separately.

Recommended next probes:

1. Inline Model node in a blank NewHMIProject.
2. Inline nested Model structure.
3. Inline UI Label with static text.
4. Inline UI Label with DynamicLink to Model variable.
