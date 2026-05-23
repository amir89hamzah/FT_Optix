# Probe 05D - FT Echo / Dummy PLC Live Tag Verification

## Status

```text
05D-1 MANUAL LIVE READ TAG TESTED / PASS
05D-2 MANUAL READWRITE TAG TESTED / PASS
05D-3 MANUAL BOOLEAN READ + TEXT MAPPING TESTED / PASS
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

Actual tags used so far:

```text
PumpA_Speed             REAL
PumpA_Running           BOOL
```

This is acceptable because the purpose is to prove live datatype and read/write paths before testing the final naming/schema path.

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
- Controller tags used:
  - PumpA_Speed   REAL
  - PumpA_Running BOOL

FactoryTalk Optix:
- Communication driver path used: CommDrivers / RAEtherNetIPDriver1 / RAEtherNetIPStation1
- Tag importer route: 127.0.0.1
- Imported controller tags:
  - PumpA_Speed
  - PumpA_Running
- Read display object: Label linked to PumpA_Speed
- Write/readWrite object: EditableLabel linked to PumpA_Speed
- BOOL text display object: Label using Key-value converter linked to PumpA_Running
```

## Test 05D-1 - Manual live read tag

### Objective

Prove FT Optix can manually read one live/dummy controller tag.

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

### Observed result

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

### Actual tested tag

```text
PumpA_Speed
```

### Manual steps used

```text
1. Use the same imported RA EtherNet/IP tag proven in 05D-1: PumpA_Speed.
2. Add an FT Optix EditableLabel object.
3. Bind EditableLabel.Text to PumpA_Speed.
4. Run FT Optix emulator/runtime.
5. Edit the black EditableLabel value in the FT Optix runtime.
6. Confirm the Studio 5000 / Logix Designer monitor value for PumpA_Speed changes.
7. Confirm the red read-only display linked to the same tag also follows the updated value.
```

### Observed result

```text
FT Optix runtime EditableLabel value = 888.5
Studio 5000 / Logix Designer PumpA_Speed monitor value ≈ 888.5
FT Optix runtime read display also shows 888.5
```

### Status

```text
PASS
```

### Finding

Manual live read/write through FactoryTalk Logix Echo and RA EtherNet/IP tag import works for a REAL controller tag.

The practical manual write path is:

```text
FT Optix runtime EditableLabel
→ imported RA EtherNet/IP tag PumpA_Speed
→ FactoryTalk Logix Echo / Studio 5000 controller tag
→ red read-only FT Optix runtime display follows updated value
```

This proves the minimum manual live read/write path required before attempting generator-created live DynamicLinks.

## Test 05D-3 - Manual Boolean read tag with text mapping

### Objective

Prove Boolean datatype mapping works and prove a BOOL value can be converted into user-facing HMI text.

### Actual tested tag

```text
PumpA_Running
```

### Actual tested datatype

```text
BOOL
```

### Converter used

```text
Key-value converter
```

### Converter configuration

```text
Source Dynamic Link: PumpA_Running
Key/value mapping:
- False → Not run
- True  → Running
```

### Manual steps used

```text
1. Create a Studio 5000 / Logix Designer controller tag named PumpA_Running with datatype BOOL.
2. Import or expose PumpA_Running through the same RA EtherNet/IP driver/tag importer path used by 05D-1 and 05D-2.
3. Add an FT Optix Label object.
4. Open the Label.Text complex dynamic link editor.
5. Add a Key-value converter.
6. Set Source Dynamic Link to PumpA_Running.
7. Configure values:
   - False = Not run
   - True  = Running
8. Run FT Optix emulator/runtime.
9. Set PumpA_Running = 1 / true in Studio 5000.
10. Confirm FT Optix runtime label shows Running.
11. Set PumpA_Running = 0 / false in Studio 5000.
12. Confirm FT Optix runtime label shows Not run.
```

### Expected result

```text
PumpA_Running = 1 / true  → FT Optix runtime label shows Running
PumpA_Running = 0 / false → FT Optix runtime label shows Not run
```

### Observed result

```text
PumpA_Running = 1 → FT Optix runtime label = Running
PumpA_Running = 0 → FT Optix runtime label = Not run
```

### Status

```text
PASS
```

### Finding

FactoryTalk Optix can read a live BOOL controller tag through RA EtherNet/IP and can convert that BOOL into localized/user-facing text using a Key-value converter inside a complex dynamic link.

The practical BOOL display path is:

```text
Studio 5000 BOOL tag PumpA_Running
→ FT Optix imported RA EtherNet/IP tag PumpA_Running
→ Label.Text complex dynamic link
→ Key-value converter
→ Running / Not run runtime text
```

This confirms that HMI status text can be generated later from JSON/UI intent without exposing raw `0/1` or `true/false` values to the operator.

## Test 05D-4 - Manual write command tag

### Objective

Prove a command value can be written from FT Optix to the controller tag.

### Suggested tag

```text
PumpA_Command
```

### Suggested datatype options

Start with a simple command datatype before STRING if required:

```text
DINT command code:
0 = None
1 = Start
2 = Stop
```

or, if STRING write works cleanly in the local project:

```text
STRING command text:
START
STOP
```

### Manual steps

```text
1. Create/import a command tag such as PumpA_Command.
2. Bind an FT Optix input or command control to the command tag.
3. Run runtime/emulator.
4. Write START/STOP or numeric command values from FT Optix.
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
Base tag source path: RAEtherNetIPDriver1 / RAEtherNetIPStation1 / Tags / Controller Tags

05D-1 Manual read tag:
Expected: FT Optix runtime display follows live controller REAL tag value.
Actual: PumpA_Speed 123.4 displayed as 123.4; PumpA_Speed 567.77 displayed as 567.8.
Status: PASS

05D-2 Manual readWrite tag:
Expected: Value written from FT Optix reaches controller tag and the read display follows.
Actual: FT Optix runtime EditableLabel wrote 888.5 to PumpA_Speed; Studio 5000 monitor value updated; FT Optix read display also showed 888.5.
Status: PASS

05D-3 Boolean read + text mapping:
Expected: PumpA_Running false/true is reflected as Not run/Running in FT Optix runtime.
Actual: PumpA_Running 1 displayed as Running; PumpA_Running 0 displayed as Not run.
Status: PASS

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

05D-1, 05D-2, and 05D-3 passed, so the next safe step is:

```text
Probe 05D-4 manual command write using PumpA_Command
```

Do not move to 05D-5 generator-created live DynamicLinks until the remaining command write check is clear or explicitly deferred.
