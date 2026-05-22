# Milestone 06 - JSON to Model variables to DynamicLinks

## Purpose

Milestone 06 combines the proven pieces from Probe 05A, 05B, and 05C into one cleaner generator path.

Previous proof points:

```text
05A - JSON schema validator passes
05B - DesignTime NetLogic can read hardened JSON and create Model variables
05C - DesignTime NetLogic can create a working local DynamicLink
```

Milestone 06 now combines them:

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

Use the existing Probe 05A valid example:

```text
06_specs/examples/valid_tag_backed_variables.json
```

Copy it to:

```text
C:\Temp\ftoptix_milestone06_tag_backed_variables.json
```

## C# template

```text
DesignTimeJsonModelDynamicLinkGenerator.cs
```

Paste the contents into a FT Optix DesignTime NetLogic class.

If FT Optix created the file/class as `DesignTimeNetLogic2.cs`, change the class declaration to:

```csharp
public class DesignTimeNetLogic2 : BaseNetLogic
```

## Method to run

```text
GenerateJsonModelWithLocalDynamicLinks()
```

## Expected generated structure

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
   │  ├─ CurrentSpeed          -> DynamicLink to _LocalSources/PumpA/CurrentSpeed
   │  ├─ SetSpeed              -> DynamicLink to _LocalSources/PumpA/SetSpeed
   │  ├─ Running               -> DynamicLink to _LocalSources/PumpA/Running
   │  ├─ OperatorCommand       -> DynamicLink to _LocalSources/PumpA/OperatorCommand
   │  └─ DisplayName           -> static value
   └─ PumpB
      ├─ CurrentSpeed          -> mock value
      ├─ SetSpeed              -> mock value
      └─ Running               -> mock value
```

## Expected behavior

For `PumpA`, values are linked to local simulated source variables.

For example:

```text
_LocalSources/PumpA/CurrentSpeed = 12.3
PumpA/CurrentSpeed follows that value through DynamicLink
```

If the source value is changed to:

```text
_LocalSources/PumpA/CurrentSpeed = 45.6
```

then a UI label linked to:

```text
Model/AI_JsonDynamicLinkProbe_01/PumpA/CurrentSpeed
```

should update at runtime/emulator.

For `PumpB`, values are written directly because the JSON declares them as mock values.

## Pass criteria

Milestone 06 passes if:

```text
1. The method compiles.
2. The method runs without error.
3. AI_JsonDynamicLinkProbe_01 appears under Model.
4. _LocalSources is created.
5. PumpA variables are linked to _LocalSources variables.
6. PumpB mock values are written directly.
7. Runtime/emulator shows at least one PumpA linked value following its local source.
```

## Why this milestone matters

This is the bridge between safe AI JSON generation and real PLC-backed FT Optix generation.

Once this works, the next major step can be:

```text
Milestone 07 / Probe 05D - FT Echo or dummy PLC live tag verification
```

At that point, FT Echo becomes relevant because the source is no longer local `_LocalSources`, but a real communication/tag path.
