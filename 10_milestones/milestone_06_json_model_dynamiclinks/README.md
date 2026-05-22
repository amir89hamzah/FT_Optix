# Milestone 06 - JSON to Model variables to DynamicLinks

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
2026-05-23
```

## Purpose

Milestone 06 combines the proven pieces from Probe 05A, 05B, and 05C into one cleaner generator path.

Previous proof points:

```text
05A - JSON schema validator passes
05B - DesignTime NetLogic can read hardened JSON and create Model variables
05C - DesignTime NetLogic can create a working local DynamicLink
```

Milestone 06 combines them:

```text
Hardened JSON
→ DesignTime NetLogic
→ Model objects and variables
→ local simulated source variables
→ DynamicLinks for plcTag-intent variables
```

## Scope

FT Echo is not required for this milestone.

This milestone does not connect to a real PLC yet.

For `source.kind = plcTag`, the generator creates a local simulated source under `_LocalSources` and links the generated variable to that source. This proves the DynamicLink path in a safe local model before using FT Echo or a dummy PLC.

## Input JSON

The existing Probe 05A valid example was used:

```text
06_specs/examples/valid_tag_backed_variables.json
```

It was copied to:

```text
C:\Temp\ftoptix_milestone06_tag_backed_variables.json
```

## C# template

```text
DesignTimeJsonModelDynamicLinkGenerator.cs
```

Working FT Optix method:

```text
GenerateJsonModelWithLocalDynamicLinks()
```

A compile issue was found and fixed during testing:

```text
Cannot implicitly convert type 'object' to 'UAManagedCore.UAValue'
```

Fix:

```text
Replace object-returning GetNeutralValue(...) assignment with SetNeutralValue(variable, dataType), which assigns typed values directly.
```

## Generated structure observed

```text
Model
└─ AI_JsonDynamicLinkProbe_01
   ├─ _LocalSources
   │  └─ PumpA
   │     ├─ CurrentSpeed
   │     ├─ SetSpeed
   │     ├─ Running
   │     └─ OperatorCommand
   ├─ PumpA
   │  ├─ CurrentSpeed          -> DynamicLink to ../_LocalSources/PumpA/CurrentSpeed
   │  ├─ SetSpeed              -> DynamicLink to ../_LocalSources/PumpA/SetSpeed
   │  ├─ Running               -> DynamicLink to ../_LocalSources/PumpA/Running
   │  ├─ OperatorCommand       -> DynamicLink to ../_LocalSources/PumpA/OperatorCommand
   │  └─ DisplayName           -> static value
   └─ PumpB
      ├─ CurrentSpeed          -> mock value
      ├─ SetSpeed              -> mock value
      └─ Running               -> mock value
```

## Observed PumpA behavior

PumpA variables were generated with DynamicLinks to local simulated sources.

Observed examples:

```text
PumpA/CurrentSpeed -> ../_LocalSources/PumpA/CurrentSpeed
PumpA/SetSpeed     -> ../_LocalSources/PumpA/SetSpeed
PumpA/Running      -> ../_LocalSources/PumpA/Running
```

Metadata preserved the original PLC tag intent:

```text
CurrentSpeed_SourceKind          = plcTag
CurrentSpeed_GenerationSourceKind = localDynamicLink
CurrentSpeed_SourceTag           = PumpA.CurrentSpeed
CurrentSpeed_SourceMode          = read
```

## Observed PumpB behavior

PumpB variables were generated as mock values because the JSON declares `source.kind = mock`.

Observed examples:

```text
PumpB/CurrentSpeed = 78.400002
PumpB/SetSpeed     = 80
PumpB/Running      = True
```

The value `78.400002` is acceptable Float display behavior and should be handled later by display precision / formatting.

## Runtime confirmation

A UI label was linked to:

```text
Model/AI_JsonDynamicLinkProbe_01/PumpA/CurrentSpeed
```

Runtime/emulator showed the linked value following its local source.

Observed values:

```text
Initial linked display: 12.3
After local source change: 45.6
```

The label formatting was changed from JSON-compliant display to numeric formatting:

```text
0.0 (1,234.0)
```

This avoided overly long/raw float display and showed `45.6` cleanly.

## Result

Milestone 06 is accepted as passed.

It proves:

1. DesignTime NetLogic can read the hardened JSON.
2. The generator can create a combined model root.
3. The generator can create local source variables for future `plcTag` intent.
4. The generator can create generated process variables.
5. The generator can create functional DynamicLinks from generated variables to local source variables.
6. Mock/static values remain direct values.
7. Runtime/emulator confirms the DynamicLink behavior.
8. FT Echo is not required for this local simulation milestone.

## Next checkpoint

The next major step is:

```text
Milestone 07 / Probe 05D - FT Echo or dummy PLC live tag verification
```

At that point, FT Echo becomes relevant because the source will no longer be local `_LocalSources`, but a real communication/tag path.

Do not jump into full dashboard, Recipe, Datalogger, or Alarm generation yet.
