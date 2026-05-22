# Probe 05D - FT Echo / Dummy PLC Live Tag Verification

## Status

```text
PLANNED
```

## Purpose

Verify that `source.kind = plcTag` can eventually connect to real or dummy controller tags, not only to local `_LocalSources` variables.

Milestone 06 already proved local DynamicLinks. Probe 05D begins the live tag proof.

## Why this matters

The repo's larger goal is LLM-generated runnable FactoryTalk Optix HMIs.

For that to be real, an LLM must be able to move from:

```text
JSON plcTag intent
→ FT Optix variable
→ live/dummy PLC tag binding
→ runtime value update
```

This probe proves that bridge.

## FT Echo requirement

FT Echo is not needed for Milestone 06.

FT Echo or another dummy/live controller source is needed for this probe because the test must verify real communication/tag-backed runtime behavior.

## Scope

This probe must stay small.

Do not test these yet:

```text
- dashboard generation
- alarm generation
- trends
- dataloggers
- recipes
- full HMI screen generation
```

Only verify live tag connection behavior first.

## Minimum tag set

Create or expose these dummy/live controller tags:

```text
PumpA.CurrentSpeed      Float / REAL
PumpA.SetSpeed          Float / REAL
PumpA.Running           Boolean / BOOL
PumpA.OperatorCommand   String
```

Suggested initial values:

```text
PumpA.CurrentSpeed = 12.3
PumpA.SetSpeed = 50.0
PumpA.Running = false
PumpA.OperatorCommand = ""
```

## Test 05D-1 - Manual live read tag

### Objective

Prove FT Optix can manually read one live/dummy controller tag.

### Tag

```text
PumpA.CurrentSpeed
```

### Manual steps

```text
1. Start FT Echo or dummy/live controller.
2. Confirm PumpA.CurrentSpeed exists and has value 12.3.
3. In FT Optix, manually create or bind a test variable/display to PumpA.CurrentSpeed.
4. Run FT Optix runtime/emulator.
5. Confirm display shows 12.3.
6. Change controller tag value to 45.6.
7. Confirm FT Optix runtime display updates to 45.6.
```

### Expected result

```text
FT Optix runtime display follows the controller tag value.
```

### Pass criteria

```text
12.3 displayed initially.
45.6 displayed after controller tag change.
No runtime link failure.
```

## Test 05D-2 - Manual readWrite tag

### Objective

Prove FT Optix can write a runtime value back to the controller tag.

### Tag

```text
PumpA.SetSpeed
```

### Manual steps

```text
1. Bind FT Optix input/display to PumpA.SetSpeed.
2. Run runtime/emulator.
3. Change SetSpeed from FT Optix runtime.
4. Confirm PumpA.SetSpeed changes in FT Echo / dummy controller.
```

### Expected result

```text
Value written from FT Optix reaches controller tag.
```

## Test 05D-3 - Manual Boolean read tag

### Objective

Prove Boolean datatype mapping works.

### Tag

```text
PumpA.Running
```

### Manual steps

```text
1. Bind FT Optix indicator/display to PumpA.Running.
2. Set controller tag false.
3. Confirm FT Optix shows false.
4. Set controller tag true.
5. Confirm FT Optix shows true.
```

### Expected result

```text
FT Optix runtime follows Boolean tag changes.
```

## Test 05D-4 - Manual write command tag

### Objective

Prove a command value can be written from FT Optix to the controller tag.

### Tag

```text
PumpA.OperatorCommand
```

### Manual steps

```text
1. Bind an FT Optix text input or command control to PumpA.OperatorCommand.
2. Run runtime/emulator.
3. Write START from FT Optix.
4. Confirm controller tag becomes START.
```

### Expected result

```text
Controller tag receives command string from FT Optix.
```

## Test 05D-5 - Generator live DynamicLink

Run this only after 05D-1 to 05D-4 pass manually.

### Objective

Update the JSON/NetLogic path so `source.kind = plcTag` can generate or attach the live tag DynamicLink automatically.

### Expected generator behavior

```text
For source.kind = plcTag:
- create the generated Model variable
- attach DynamicLink to the real communication/tag path
- preserve SourceKind
- preserve SourceTag
- preserve SourceMode
- record GenerationSourceKind = liveDynamicLink or equivalent
```

## Result log

Fill this after testing:

```text
Test date:
FT Optix version:
FT Echo / controller source:
Controller type:
Tag source path:

05D-1 Manual read tag:
Expected:
Actual:
Status: PASS / FAIL

05D-2 Manual readWrite tag:
Expected:
Actual:
Status: PASS / FAIL

05D-3 Boolean read tag:
Expected:
Actual:
Status: PASS / FAIL

05D-4 Write command tag:
Expected:
Actual:
Status: PASS / FAIL

05D-5 Generator live DynamicLink:
Expected:
Actual:
Status: PASS / FAIL / NOT RUN
```

## Decision rule

If manual live tag binding fails, do not edit the generator yet.

If manual live tag binding passes, record the exact FT Optix path/API/pattern, then create a generator probe.
