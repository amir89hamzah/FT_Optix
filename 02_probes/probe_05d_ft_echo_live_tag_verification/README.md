# Probe 05D - FT Echo / Dummy PLC Live Tag Verification

## Status

```text
05D-1 MANUAL LIVE READ TAG TESTED / PASS
05D-2 MANUAL READWRITE TAG PLANNED
05D-3 MANUAL BOOLEAN READ TAG PLANNED
05D-4 MANUAL WRITE COMMAND TAG PLANNED
05D-5 GENERATOR LIVE DYNAMICLINK PLANNED
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

Original planned tag set:

```text
PumpA.CurrentSpeed      Float / REAL
PumpA.SetSpeed          Float / REAL
PumpA.Running           Boolean / BOOL
PumpA.OperatorCommand   String
```

During 05D-1, the actual available Logix test tag was:

```text
PumpA_Speed             REAL
```

This is acceptable for 05D-1 because the purpose is to prove one live REAL tag read path before testing the final naming/schema path.

## Tested environment notes

Observed from manual screenshots:

```text
FactoryTalk Logix Echo Dashboard:
- Device name: Test
- Type: ControlLogix 5580 Emulator
- IP address: 127.0.0.1
- Controller switch: On
- Service connected

Studio 5000 / Logix Designer:
- Controller path used for browsing: Emulate Ethernet / 127.0.0.1
- Test controller visible in FactoryTalk Linx Network Browser
- Controller tag used: PumpA_Speed
- Data type: REAL

FactoryTalk Optix:
- Communication driver path used: CommDrivers / RAEtherNetIPDriver1 / RAEtherNetIPStation1
- Tag importer route: 127.0.0.1
- Imported controller tag: PumpA_Speed
```

## Test 05D-1 - Manual live read tag

### Objective

Prove FT Optix can manually read one live/dummy controller tag.

### Planned tag

```text
PumpA.CurrentSpeed
```

### Actual tested tag

```text
PumpA_Speed
```

### Manual steps used

```text
1. Start FactoryTalk Logix Echo.
2. Confirm emulated controller Test is On and service is connected.
3. In Studio 5000 / Logix Designer, confirm controller tag PumpA_Speed exists as REAL.
4. In FactoryTalk Optix, add or use an RA EtherNet/IP driver and station.
5. Use the RA EtherNet/IP tag importer in Online mode.
6. Browse route 127.0.0.1.
7. Import controller tag PumpA_Speed.
8. Bind an FT Optix runtime label/display to PumpA_Speed.
9. Run FT Optix emulator/runtime.
10. Change PumpA_Speed in Logix Designer.
11. Confirm FT Optix runtime display follows the controller tag value.
```

### Expected result

```text
FT Optix runtime display follows the controller tag value.
```

### Observed result

Observed live values:

```text
PumpA_Speed = 123.4  → FT Optix runtime display = 123.4
PumpA_Speed = 567.77 → FT Optix runtime display = 567.8
```

The `567.77 -> 567.8` display is acceptable because the FT Optix label formatting rounds to one decimal place.

### Status

```text
PASS
```

### Finding

Manual live read through FactoryTalk Logix Echo and RA EtherNet/IP tag import works.

The practical manual binding path is:

```text
Logix Echo / Studio 5000 controller tag
→ FT Optix RAEtherNetIPDriver station
→ RA EtherNet/IP tag importer
→ imported controller tag under CommDrivers
→ UI label DynamicLink
→ runtime value update
```

## Test 05D-2 - Manual readWrite tag

### Objective

Prove FT Optix can write a runtime value back to the controller tag.

### Suggested next tag

Use the same proven tag first:

```text
PumpA_Speed
```

Then add a final-schema tag later if needed:

```text
PumpA_SetSpeed
```

### Manual steps

```text
1. Bind FT Optix input/display to PumpA_Speed or PumpA_SetSpeed.
2. Run runtime/emulator.
3. Change the value from FT Optix runtime.
4. Confirm the controller tag changes in Studio 5000 / Logix Designer.
```

### Expected result

```text
Value written from FT Optix reaches controller tag.
```

## Test 05D-3 - Manual Boolean read tag

### Objective

Prove Boolean datatype mapping works.

### Suggested tag

```text
PumpA_Running
```

### Manual steps

```text
1. Create/import a BOOL tag.
2. Bind FT Optix indicator/display to the BOOL tag.
3. Set controller tag false.
4. Confirm FT Optix shows false.
5. Set controller tag true.
6. Confirm FT Optix shows true.
```

### Expected result

```text
FT Optix runtime follows Boolean tag changes.
```

## Test 05D-4 - Manual write command tag

### Objective

Prove a command value can be written from FT Optix to the controller tag.

### Suggested tag

```text
PumpA_Command
```

### Manual steps

```text
1. Create/import a STRING or integer command tag.
2. Bind an FT Optix text input or command control to the command tag.
3. Run runtime/emulator.
4. Write START or a test value from FT Optix.
5. Confirm controller tag receives the value.
```

### Expected result

```text
Controller tag receives command value from FT Optix.
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
- apply SourceMode using FTOptix.CoreBase.DynamicLinkMode
- record GenerationSourceKind = liveDynamicLink or equivalent
```

## Result log

```text
Test date: 2026-05-23
FT Optix version: not recorded in screenshot
FT Echo / controller source: FactoryTalk Logix Echo
Controller type: ControlLogix 5580 Emulator
Controller/device name: Test
Tag source path: RAEtherNetIPDriver1 / RAEtherNetIPStation1 / Tags / Controller Tags / PumpA_Speed

05D-1 Manual read tag:
Expected: FT Optix runtime display follows live controller REAL tag value.
Actual: PumpA_Speed 123.4 displayed as 123.4; PumpA_Speed 567.77 displayed as 567.8.
Status: PASS

05D-2 Manual readWrite tag:
Expected: Value written from FT Optix reaches controller tag.
Actual: Not run.
Status: PLANNED

05D-3 Boolean read tag:
Expected: Boolean controller tag change is reflected in FT Optix runtime.
Actual: Not run.
Status: PLANNED

05D-4 Write command tag:
Expected: Command value written from FT Optix reaches controller tag.
Actual: Not run.
Status: PLANNED

05D-5 Generator live DynamicLink:
Expected: Generator attaches generated variable to live communication/tag path.
Actual: Not run.
Status: PLANNED
```

## Decision rule

If manual live tag binding fails, do not edit the generator yet.

If manual live tag binding passes, record the exact FT Optix path/API/pattern, then create a generator probe.

05D-1 passed, so the next safe step is 05D-2 manual read/write using an imported RA EtherNet/IP tag.
