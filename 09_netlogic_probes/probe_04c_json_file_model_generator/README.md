# Probe 04C - DesignTime NetLogic reads JSON from file

## Status

Result: **ready for manual test**.

Probe 04B proved this path:

```text
embedded JSON string -> DesignTime NetLogic -> native FT Optix Model nodes
```

Probe 04C tests the more realistic workflow:

```text
JSON file on disk -> DesignTime NetLogic reads file -> native FT Optix Model nodes
```

## Why this matters

For an AI-assisted generator, the AI or an external script should be able to create a JSON spec file. FT Optix Studio should then load that spec through DesignTime NetLogic and create project nodes natively.

This avoids direct YAML editing and keeps FT Optix Studio responsible for writing its own project structure.

## Files

```text
DesignTimeJsonFileModelGenerator.cs
ftoptix_model_spec.json
```

`DesignTimeJsonFileModelGenerator.cs` is intentionally copy-paste friendly for a default FT Optix DesignTime NetLogic named:

```text
DesignTimeNetLogic1
```

If your NetLogic object has a different name, rename the C# class to match it exactly.

## Proposed workflow

```text
1. External tool writes model_spec.json.
2. User runs a DesignTime NetLogic method in FT Optix Studio.
3. NetLogic reads the JSON file.
4. NetLogic creates Model nodes using FT Optix C# API.
5. User exports NodeSet XML only for verification.
```

## First file-location idea

Use a simple fixed file path first, then improve later.

For the first test, place JSON at:

```text
C:\Temp\ftoptix_model_spec.json
```

Reason: it avoids guessing the FT Optix project directory API while proving file read first.

Later versions should read from a project-relative location such as:

```text
ProjectFiles\AI_Generator\model_spec.json
```

or expose the path as a NetLogic variable/property.

## Manual test steps

1. Create this folder in Windows if it does not exist:

```text
C:\Temp
```

2. Copy the JSON content from:

```text
09_netlogic_probes/probe_04c_json_file_model_generator/ftoptix_model_spec.json
```

into:

```text
C:\Temp\ftoptix_model_spec.json
```

3. In FT Optix Studio, create or reuse a DesignTime NetLogic.

4. Edit it with VS Code.

5. Paste/adapt the code from:

```text
09_netlogic_probes/probe_04c_json_file_model_generator/DesignTimeJsonFileModelGenerator.cs
```

6. Make sure the class name matches the NetLogic object name. For a default object named `DesignTimeNetLogic1`, the class must be:

```csharp
public class DesignTimeNetLogic1 : BaseNetLogic
```

7. Save the `.cs` file.

8. In FT Optix Studio, run:

```text
Method1
```

or:

```text
GenerateFromJsonFile
```

9. Check the Model folder for:

```text
AI_JsonFileProbe_01
```

## Target generated structure

Use a different root name from 04B to avoid collision:

```text
Model
└─ AI_JsonFileProbe_01
   ├─ StatusText
   ├─ GeneratedBy
   ├─ PumpA
   │  ├─ SetSpeed
   │  ├─ CurrentSpeed
   │  └─ Running
   └─ PumpB
      ├─ SetSpeed
      ├─ CurrentSpeed
      └─ Running
```

Note: FT Optix may show root variables such as `StatusText` and `GeneratedBy` in the Properties panel when the root object is selected.

## Safety behavior

Initial behavior is conservative:

```text
If AI_JsonFileProbe_01 already exists, log a warning and stop.
```

Do not delete or overwrite automatically until Probe 04D.

## Expected result

If this works, the project has proven the core AI generator architecture:

```text
external generated JSON -> FT Optix DesignTime NetLogic -> native project nodes
```

## Next after success

- 04D: overwrite/delete/recreate behavior for repeated generation.
- 04E: validate JSON schema and report useful errors.
- 05: recipe target model from JSON.
- 06: datalogger-ready model from JSON.
- 07: dynamic link creation from JSON.
- 08: UI/navigation/screen creation from JSON.
