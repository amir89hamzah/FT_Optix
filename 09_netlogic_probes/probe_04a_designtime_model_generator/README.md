# Probe 04A - DesignTime NetLogic model generator

## Status

Result: **manual DesignTime NetLogic execution and export-back successful**.

This probe pivots from XML-first generation to FT Optix native project generation through DesignTime NetLogic C#.

## Why this probe exists

NodeSet XML import/export has already proven useful for Model-folder verification, especially Probe 03. However, for full FT Optix project generation, DesignTime NetLogic is likely a better primary automation route because it uses FT Optix APIs from inside Studio.

This probe tests the smallest useful case:

```text
Can a DesignTime NetLogic method create Model objects and variables natively?
```

Result: yes, a DesignTime NetLogic method created an object and variables under `Model`, and the result exported back to NodeSet XML successfully.

## Manual verification observed

A DesignTime NetLogic was created in FT Optix Studio and edited with VS Code.

The method was executed from FT Optix Studio using:

```text
Right-click DesignTimeNetLogic1 -> Execute Method
```

Observed generated structure in Project View:

```text
Model
└─ AI_NetLogicProbe_01
   └─ Pump1
      ├─ SetSpeed
      └─ CurrentSpeed
```

The properties panel for `Pump1` showed:

```text
Type: Object
SetSpeed: Float = 50
CurrentSpeed: Float = 47.5
```

Selecting `AI_NetLogicProbe_01` showed the root variables in the properties panel:

```text
StatusText: String = Created by DesignTime NetLogic
TestNumber: Float = 123.449997
Running: Boolean = True
```

Note: FT Optix showed `Pump1` with the default Object icon. This is expected for a plain object created by `InformationModel.MakeObject(...)`. It is not a problem.

## Export-back verification

Export-back file:

```text
09_netlogic_probes/probe_04a_designtime_model_generator/exported_back_from_ftoptix.xml
```

The exported XML contains exactly the expected browse-path structure:

```text
AI_NetLogicProbe_01
AI_NetLogicProbe_01/StatusText
AI_NetLogicProbe_01/TestNumber
AI_NetLogicProbe_01/Running
AI_NetLogicProbe_01/Pump1
AI_NetLogicProbe_01/Pump1/SetSpeed
AI_NetLogicProbe_01/Pump1/CurrentSpeed
```

Exported node count:

```text
UAObject: 2
UAVariable: 5
Total: 7
```

Exported values:

```text
StatusText   String  Created by DesignTime NetLogic
TestNumber   Float   123.45
Running      Boolean true
SetSpeed     Float   50
CurrentSpeed Float   47.5
```

FT Optix normalized the project namespace to:

```text
NewHMIProject
```

and exported variables with:

```text
AccessLevel="0"
```

This matches previous export-back behavior from XML probes.

## Target generated structure

The intended complete target structure was achieved:

```text
Model
└─ AI_NetLogicProbe_01
   ├─ StatusText
   ├─ TestNumber
   ├─ Running
   └─ Pump1
      ├─ SetSpeed
      └─ CurrentSpeed
```

## Files

```text
DesignTimeModelGenerator.cs
exported_back_from_ftoptix.xml
```

`DesignTimeModelGenerator.cs` is a draft C# NetLogic body/template. It is not a standalone .NET console program. It must be copied into a FT Optix DesignTime NetLogic class or adapted to the class name created by FT Optix Studio.

## Manual test steps

1. Open FT Optix Studio.
2. Create a **DesignTime NetLogic**, not Runtime NetLogic.
3. Name it something like:

```text
DesignTimeModelGenerator
```

4. Right-click it and choose:

```text
Edit with .NET code editor (external)
```

5. Paste/adapt the code from:

```text
09_netlogic_probes/probe_04a_designtime_model_generator/DesignTimeModelGenerator.cs
```

6. Build/save from the editor if required.
7. Return to FT Optix Studio.
8. Right-click the DesignTime NetLogic and run the exported method:

```text
GenerateModelProbe
```

If using the default generated method name, run:

```text
Method1
```

9. Check Project View under `Model` for:

```text
AI_NetLogicProbe_01
```

10. Save the project.
11. Export `AI_NetLogicProbe_01` to NodeSet XML.
12. Save the exported XML as:

```text
09_netlogic_probes/probe_04a_designtime_model_generator/exported_back_from_ftoptix.xml
```

## What to record next

Record in a future probe:

- Whether repeated execution should overwrite, skip, or delete/recreate the generated node.
- Whether JSON input can drive node creation.
- Whether the same approach can create DynamicLinks, aliases, UI screens, panels, data loggers, recipes, and alarms.
- Any errors from VS Code, C# SDK, NuGet, or FT Optix Studio.

## Expected outcome

This result supports making DesignTime NetLogic the preferred path for future generation:

```text
AI / JSON spec
    -> DesignTime NetLogic C# importer
    -> native FT Optix project nodes
    -> XML/YAML export only for verification and learning
```

## Fallback

If the NetLogic tooling is not ready on a machine, do not block the project. Continue using XML import/export probes while preparing a proper FT Optix + VS Code + .NET workstation.
