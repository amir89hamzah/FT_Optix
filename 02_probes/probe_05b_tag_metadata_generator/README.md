# Probe 05B - Tag source intent metadata generator result

## Status

```text
TESTED / PASS
```

Status basis:

```text
User-confirmed FT Optix manual test in project chat with screenshots.
```

Date recorded:

```text
2026-05-22
```

## What was tested

Probe 05B used a DesignTime NetLogic method to read the hardened Probe 05A JSON file and create Model nodes in FT Optix while preserving source intent as helper metadata variables.

This probe intentionally did not create real DynamicLinks yet.

## Input JSON

The manual test used the Probe 05A valid JSON copied to the path expected by the DesignTime NetLogic template:

```text
C:\Temp\ftoptix_probe05b_tag_backed_variables.json
```

Source repo file:

```text
06_specs/examples/valid_tag_backed_variables.json
```

## C# template

```text
09_netlogic_probes/probe_05b_tag_metadata_generator/DesignTimeTagMetadataGenerator.cs
```

The FT Optix project used a DesignTime NetLogic class named:

```text
DesignTimeNetLogic2
```

The method run was:

```text
GenerateTagMetadataModel()
```

## Observed FT Optix result

The Model folder contained:

```text
Model
└─ AI_TagSchemaProbe_01
   ├─ PumpA
   └─ PumpB
```

Observed PumpA behavior:

```text
CurrentSpeed                  Float    0
CurrentSpeed_Role             String   measured
CurrentSpeed_SourceKind       String   plcTag
CurrentSpeed_SourceTag        String   PumpA.CurrentSpeed
CurrentSpeed_SourceMode       String   read
CurrentSpeed_DisplayUnit      String   Hz
CurrentSpeed_DisplayDecimals  UInt32   1

SetSpeed                      Float    0
SetSpeed_Role                 String   setpoint
SetSpeed_SourceKind           String   plcTag
SetSpeed_SourceTag            String   PumpA.SetSpeed
SetSpeed_SourceMode           String   readWrite
SetSpeed_DisplayUnit          String   Hz
SetSpeed_DisplayDecimals      UInt32   1
SetSpeed_DisplayMin           Double   0
SetSpeed_DisplayMax           Double   100
```

Observed PumpB behavior:

```text
CurrentSpeed                  Float    78.400002
CurrentSpeed_Role             String   measured
CurrentSpeed_SourceKind       String   mock
CurrentSpeed_DisplayUnit      String   Hz
CurrentSpeed_DisplayDecimals  UInt32   1

SetSpeed                      Float    80
SetSpeed_Role                 String   setpoint
SetSpeed_SourceKind           String   mock
SetSpeed_DisplayUnit          String   Hz
SetSpeed_DisplayDecimals      UInt32   1
SetSpeed_DisplayMin           Double   0
SetSpeed_DisplayMax           Double   100

Running                       Boolean  True
Running_Role                  String   status
Running_SourceKind            String   mock
```

The value `78.400002` is acceptable for a 32-bit Float display and should be handled later by HMI formatting / display precision.

## Result

Probe 05B is accepted as passed.

It proves:

1. DesignTime NetLogic can read the hardened Probe 05A JSON file.
2. The generator can create a clean Model root under `Model`.
3. The generator can create object nodes from JSON.
4. The generator can create typed variables from JSON.
5. `plcTag` variables are not pretending to have live values yet.
6. PLC tag intent is preserved as metadata helper variables.
7. `mock` values can be written safely because the JSON declares them as mock values.

## Next checkpoint

Do not jump directly to full PLC / FT Echo integration.

Next planned probe:

```text
Probe 05C - DynamicLink pattern discovery
```

Probe 05C should focus only on discovering the smallest reliable DesignTime NetLogic API pattern for creating FT Optix `DynamicLink` nodes.
