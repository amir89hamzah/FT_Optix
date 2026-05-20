# Probe 04A - DesignTime NetLogic model generator

## Status

Result: **planned / ready for manual DesignTime NetLogic test**.

This probe pivots from XML-first generation to FT Optix native project generation through DesignTime NetLogic C#.

## Why this probe exists

NodeSet XML import/export has already proven useful for Model-folder verification, especially Probe 03. However, for full FT Optix project generation, DesignTime NetLogic is likely a better primary automation route because it uses FT Optix APIs from inside Studio.

This probe tests the smallest useful case:

```text
Can a DesignTime NetLogic method create Model objects and variables natively?
```

## Target generated structure

After running the method, FT Optix should show:

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
```

This is a draft C# NetLogic body/template. It is not a standalone .NET console program. It must be copied into a FT Optix DesignTime NetLogic class or adapted to the class name created by FT Optix Studio.

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

## What to record

Record:

- Whether the code compiled.
- Whether the exported method appeared in FT Optix Studio.
- Whether running it created the Model nodes.
- Whether values and data types were correct.
- Whether export-back preserved the structure.
- Any errors from VS Code, C# SDK, NuGet, or FT Optix Studio.

## Expected outcome

If this works, DesignTime NetLogic becomes the preferred path for future generation:

```text
AI / JSON spec
    -> DesignTime NetLogic C# importer
    -> native FT Optix project nodes
    -> XML/YAML export only for verification and learning
```

## Fallback

If the NetLogic tooling is not ready on the current PC, do not block the project. Continue using XML import/export probes while preparing a proper FT Optix + VS Code + .NET workstation.
