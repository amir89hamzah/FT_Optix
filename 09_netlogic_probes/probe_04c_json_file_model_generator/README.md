# Probe 04C - DesignTime NetLogic reads JSON from file

## Status

Result: **planned / next test**.

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

## Safety behavior

Initial behavior should be conservative:

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
