# Probe 04E - JSON validation before generation

## Status

Result: **planned / ready for manual test**.

Probe 04D proved the repeatable loop:

```text
edit JSON -> run DesignTime NetLogic -> generated Model nodes are recreated
```

Probe 04E adds validation before generation.

## Goal

Before creating any FT Optix nodes, the DesignTime NetLogic should validate the JSON spec and stop with clear Studio Output messages if something is wrong.

This protects the project from partially generated or confusing structures.

## First validation scope

This first validation probe checks:

```text
rootName is required
generationMode must be skip or deleteAndRecreate
object names must not be empty
variable names must not be empty
dataType must be String, Float, Double, Boolean, Int32, or UInt32
child names must not be duplicated under the same parent
variable value must be present
```

## Files

```text
DesignTimeJsonValidationGenerator.cs
valid_model_spec.json
invalid_missing_rootName.json
invalid_unknown_dataType.json
invalid_duplicate_child.json
```

For the manual test, copy one JSON file at a time into:

```text
C:\Temp\ftoptix_model_spec.json
```

The C# template reads this fixed path:

```text
C:\Temp\ftoptix_model_spec.json
```

## Manual test - valid JSON

1. Copy content from:

```text
09_netlogic_probes/probe_04e_json_validation/valid_model_spec.json
```

into:

```text
C:\Temp\ftoptix_model_spec.json
```

2. Paste/adapt the C# template into the FT Optix DesignTime NetLogic file.

3. Ensure the class name matches the FT Optix NetLogic object name. For the default object:

```csharp
public class DesignTimeNetLogic1 : BaseNetLogic
```

4. Save the file in VS Code.

5. Run `Method1` from FT Optix Studio.

Expected generated structure:

```text
Model
└─ AI_ValidationProbe_01
   ├─ StatusText
   ├─ PumpValid
   │  ├─ SetSpeed
   │  ├─ CurrentSpeed
   │  └─ Running
   └─ TankValid
      ├─ Level
      └─ Temperature
```

## Manual test - invalid JSON

Repeat the test using each invalid JSON file.

Expected result:

```text
No nodes are generated.
Studio Output shows a clear validation error.
```

Example expected errors:

```text
Probe04E validation failed: rootName is required.
Probe04E validation failed: Unknown dataType 'Real' at AI_InvalidProbe_UnknownDataType/Pump1/SetSpeed.
Probe04E validation failed: Duplicate child name 'Pump1' under AI_InvalidProbe_DuplicateChild.
```

## Safety behavior

Probe 04E focuses on validation. It uses conservative generation behavior:

```text
If the target root already exists, stop and log a warning.
```

Delete/recreate behavior was proven in Probe 04D and can be merged later after validation is stable.

## Why this matters

A real AI-assisted generator must fail safely and explain what is wrong with the input spec.

Without validation, a bad JSON spec could create partial project nodes and make debugging difficult.

## Expected outcome

If this works, the generator foundation becomes:

```text
JSON file -> validation -> native FT Optix generation
```

After this, future probes can safely add more FT Optix-specific patterns.

## Next after success

- 05: recipe target model from JSON.
- 06: datalogger-ready model from JSON.
- 07: dynamic link creation from JSON.
- 08: UI/navigation/screen creation from JSON.
