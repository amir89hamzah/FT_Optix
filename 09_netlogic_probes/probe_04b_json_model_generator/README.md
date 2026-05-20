# Probe 04B - DesignTime NetLogic JSON model generator

## Status

Result: **planned / ready for manual DesignTime NetLogic test**.

Probe 04A proved that DesignTime NetLogic can create Model objects and variables natively in FT Optix Studio.

Probe 04B is the next step:

```text
Can a DesignTime NetLogic method parse a JSON spec and generate Model nodes from it?
```

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

For the first manual test, the JSON is embedded directly inside the C# method as a string constant.

This avoids file-path, project-directory, and permission problems while testing the important part first:

```text
JSON parse -> recursive node creation -> export-back verification
```

Later probes can move the JSON into a project file, CSV, external file, or generated import package.

## Target generated structure

After running the method, FT Optix should show:

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

## Files

```text
DesignTimeJsonModelGenerator.cs
model_spec_example.json
```

`DesignTimeJsonModelGenerator.cs` is a FT Optix NetLogic template, not a standalone console app.

`model_spec_example.json` is the same example spec in standalone JSON form for future tooling.

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

5. Save the `.cs` file in VS Code.
6. Return to FT Optix Studio.
7. Right-click the DesignTime NetLogic and run:

```text
GenerateFromEmbeddedJson
```

If you kept the default generated method name, paste the method body under `Method1` and run:

```text
Method1
```

8. Check Project View under `Model` for:

```text
AI_JsonProbe_01
```

9. Save the project.
10. Export `AI_JsonProbe_01` to NodeSet XML.
11. Save as:

```text
09_netlogic_probes/probe_04b_json_model_generator/exported_back_from_ftoptix.xml
```

## What to check

- Does the code compile?
- Does the exported method appear in FT Optix Studio?
- Does `AI_JsonProbe_01` appear under `Model`?
- Are `Pump1` and `Pump2` created?
- Are all child variables created with the expected values and data types?
- Does export-back preserve the browse path structure?

## Expected result

If this works, the project has proven the most important generator path:

```text
JSON spec -> DesignTime NetLogic -> native FT Optix Model nodes
```

That would make future work more about expanding the JSON schema and C# generator capabilities rather than guessing raw YAML/XML structures.

## Next possible probes after success

- 04C: Read JSON from a project file instead of embedded string.
- 04D: Add overwrite/delete/recreate behavior for repeated runs.
- 05: JSON-driven recipe target model.
- 06: JSON-driven DataLogger-ready model.
- 07: JSON-driven DynamicLink pattern.
- 08: JSON-driven UI/navigation/screen probe.
