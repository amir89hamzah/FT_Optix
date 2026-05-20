# Probe 04B - DesignTime NetLogic JSON model generator

## Status

Result: **passed / accepted as full pass by manual visual verification**.

Probe 04A proved that DesignTime NetLogic can create Model objects and variables natively in FT Optix Studio.

Probe 04B tested the next step:

```text
Can a DesignTime NetLogic method parse a JSON spec and generate Model nodes from it?
```

Result: yes, the embedded JSON spec generated the expected root object and two pump child objects with typed variables.

## Why this matters

This is the bridge toward an AI-assisted FT Optix generator.

Target future workflow:

```text
AI / Python / external tool creates JSON spec
    -> DesignTime NetLogic reads the JSON
    -> FT Optix Studio creates native project nodes
    -> NodeSet XML / YAML export used for verification and learning
```

## First test approach

For the first manual test, the JSON was embedded directly inside the C# method as a string constant.

This avoided file-path, project-directory, and permission problems while testing the important part first:

```text
JSON parse -> recursive node creation -> FT Optix native Model nodes
```

Later probes can move the JSON into a project file, CSV, external file, or generated import package.

## Visual verification observed

FT Optix Studio Project View showed:

```text
Model
└─ AI_JsonProbe_01
   ├─ Pump1
   └─ Pump2
```

Selecting `Pump1` showed the expected child variables in the Properties panel:

```text
SetSpeed: Float = 50
CurrentSpeed: Float = 47.5
Running: Boolean = True
```

This confirms the DesignTime NetLogic parsed the embedded JSON and created multiple objects with typed child variables.

A warning was also observed after a repeated run:

```text
AI_JsonProbe_01 already exists. Delete it manually before running again.
```

This is expected for the current safe behavior. The probe deliberately skips generation if the target root already exists, instead of deleting or overwriting nodes automatically.

Older errors about the previous NetLogic class name may remain visible in Studio Output. Those are not part of the successful 04B run if the current project tree contains `AI_JsonProbe_01`, `Pump1`, and `Pump2`.

## Target generated structure

The intended generated structure:

```text
Model
└─ AI_JsonProbe_01
   ├─ StatusText
   ├─ GeneratedBy
   ├─ Pump1
   │  ├─ SetSpeed
   │  ├─ CurrentSpeed
   │  └─ Running
   └─ Pump2
      ├─ SetSpeed
      ├─ CurrentSpeed
      └─ Running
```

Note: FT Optix may show root-level variables such as `StatusText` and `GeneratedBy` in the Properties panel when `AI_JsonProbe_01` is selected, rather than as expanded tree children.

## Files

```text
DesignTimeJsonModelGenerator.cs
model_spec_example.json
```

`DesignTimeJsonModelGenerator.cs` is a FT Optix NetLogic template, not a standalone console app.

`model_spec_example.json` is the same example spec in standalone JSON form for future tooling.

## Key implementation lessons

1. The C# class name must exactly match the FT Optix NetLogic object name. For example, if the object is named `DesignTimeNetLogic1`, the C# class must be:

```csharp
public class DesignTimeNetLogic1 : BaseNetLogic
```

2. The template needs the NetLogic namespace:

```csharp
using FTOptix.NetLogic;
```

3. If using `OpcUa.DataTypes.*`, this alias is useful:

```csharp
using OpcUa = UAManagedCore.OpcUa;
```

4. `variable.Value` should be assigned typed values directly. Do not assign a generic `object` return value to `variable.Value`.

5. Repeated runs should be handled intentionally. The current safe behavior is skip-if-existing.

## Manual test steps

1. Open FT Optix Studio.
2. Create a **DesignTime NetLogic**.
3. Right-click it and choose:

```text
Edit with .NET code editor (external)
```

4. Paste/adapt code from:

```text
09_netlogic_probes/probe_04b_json_model_generator/DesignTimeJsonModelGenerator.cs
```

5. Ensure the class name matches the NetLogic object name.
6. Save the `.cs` file in VS Code.
7. Return to FT Optix Studio.
8. Right-click the DesignTime NetLogic and run the exported method.
9. Check Project View under `Model` for:

```text
AI_JsonProbe_01
```

## Outcome

This probe proves the most important generator path:

```text
JSON spec -> DesignTime NetLogic -> native FT Optix Model nodes
```

That means future work should focus on expanding the JSON schema and DesignTime NetLogic generator capabilities, not guessing raw YAML/XML structures first.

## Next possible probes

- 04C: Read JSON from a project file instead of embedded string.
- 04D: Add overwrite/delete/recreate behavior for repeated runs.
- 05: JSON-driven recipe target model.
- 06: JSON-driven DataLogger-ready model.
- 07: JSON-driven DynamicLink pattern.
- 08: JSON-driven UI/navigation/screen probe.
