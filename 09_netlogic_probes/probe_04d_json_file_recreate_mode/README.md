# Probe 04D - JSON file generator with delete-and-recreate mode

## Status

Result: **manual DesignTime NetLogic delete-and-recreate execution visually successful**.

Probe 04C proved:

```text
external JSON file -> DesignTime NetLogic -> native FT Optix Model nodes
```

Probe 04D added controlled repeat-run behavior.

## Goal

When the generated root already exists, the generator should support a safe mode:

```text
generationMode: deleteAndRecreate
```

This means only the generated root object is deleted and recreated. It should not delete arbitrary project folders.

## Why this matters

A real generator will run repeatedly while the JSON spec changes. Manual delete-before-run is too slow.

This probe checks whether a DesignTime NetLogic can:

```text
1. Read JSON from file.
2. Detect existing generated root.
3. Delete only that generated root.
4. Recreate the root and children from JSON.
```

Result: yes, the root was recreated from the changed JSON file.

## Files

```text
DesignTimeJsonFileRecreateGenerator.cs
ftoptix_model_spec_recreate.json
```

For the first test, copy the JSON file content into:

```text
C:\Temp\ftoptix_model_spec.json
```

The C# template reads this fixed path:

```text
C:\Temp\ftoptix_model_spec.json
```

## Manual verification observed

Before Probe 04D, `AI_JsonFileProbe_01` existed from Probe 04C with `PumpA` and `PumpB`.

After changing the JSON file and rerunning the DesignTime NetLogic method, FT Optix Studio showed:

```text
Model
└─ AI_JsonFileProbe_01
   ├─ PumpA
   ├─ PumpB
   └─ PumpC
```

Selecting `PumpA` showed updated values:

```text
SetSpeed: Float = 75
CurrentSpeed: Float = 73.199997
Running: Boolean = True
```

This proves the generator read the file at:

```text
C:\Temp\ftoptix_model_spec.json
```

and recreated the generated root object from the updated JSON content.

## Expected generated structure

The test uses the same root name as Probe 04C, but different values and an extra pump, so it is easy to see whether delete-and-recreate worked.

Expected after running 04D:

```text
Model
└─ AI_JsonFileProbe_01
   ├─ StatusText
   ├─ GeneratedBy
   ├─ GenerationMode
   ├─ PumpA
   │  ├─ SetSpeed       = 75
   │  ├─ CurrentSpeed   = 73.2
   │  └─ Running        = True
   ├─ PumpB
   │  ├─ SetSpeed       = 10
   │  ├─ CurrentSpeed   = 0
   │  └─ Running        = False
   └─ PumpC
      ├─ SetSpeed       = 40
      ├─ CurrentSpeed   = 39.5
      └─ Running        = True
```

If the previous 04C root existed with only PumpA and PumpB, success is indicated by:

```text
PumpC appears
PumpA values update to 75 / 73.2 / True
PumpB values update to 10 / 0 / False
```

## Manual test steps

1. Copy the JSON content from:

```text
09_netlogic_probes/probe_04d_json_file_recreate_mode/ftoptix_model_spec_recreate.json
```

into:

```text
C:\Temp\ftoptix_model_spec.json
```

2. In FT Optix, open the DesignTime NetLogic with VS Code.

3. Use the 04D C# template, or modify the 04C code to support `generationMode: deleteAndRecreate`.

4. Make sure the C# class name exactly matches the FT Optix NetLogic object name.

For a default NetLogic named `DesignTimeNetLogic1`, the class must be:

```csharp
public class DesignTimeNetLogic1 : BaseNetLogic
```

5. Save the file in VS Code.

6. Wait for the Problems tab to show no compile errors.

7. Return to FT Optix Studio.

8. Right-click the DesignTime NetLogic and run:

```text
Method1
```

or:

```text
GenerateFromJsonFile
```

9. Check `Model/AI_JsonFileProbe_01`.

## Safety rule

Only delete the root object named by `rootName` in the JSON file. Do not delete `Model`, `UI`, `Alarms`, `Recipes`, or other top-level project nodes.

## Expected outcome

This works. We now have a repeatable generator loop:

```text
edit JSON -> run DesignTime NetLogic -> generated nodes are recreated
```

This is much closer to a real AI-assisted FT Optix generation workflow.

## Next after success

- 04E: JSON schema validation and better error messages.
- 05: recipe target model from JSON.
- 06: datalogger-ready model from JSON.
- 07: dynamic link creation from JSON.
- 08: UI/navigation/screen creation from JSON.
