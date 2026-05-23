# Probe 05D - FT Echo / Dummy PLC Live Tag Verification

## Status

```text
05D-1 MANUAL LIVE READ TAG TESTED / PASS
05D-2 MANUAL READWRITE TAG TESTED / PASS
05D-3 MANUAL BOOLEAN READ + TEXT MAPPING TESTED / PASS
05D-4 MANUAL WRITE COMMAND TAG TESTED / PASS
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
PumpA_Command           DINT
```

This is acceptable because the purpose is to prove live datatype and read/write paths before testing the final naming/schema path.

## Tested environment notes

Observed from manual screenshots and exported XML:

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
  - PumpA_Command DINT

FactoryTalk Optix:
- Communication driver path used: CommDrivers / RAEtherNet_IPDriver1 / RAEtherNet_IPStation1
- Tag importer route: 127.0.0.1
- Imported controller tags:
  - PumpA_Speed
  - PumpA_Running
  - PumpA_Command
- Read display object: Label linked to PumpA_Speed
- Write/readWrite object: EditableLabel linked to PumpA_Speed
- BOOL text display object: Label using Key-value converter linked to PumpA_Running
- Command write object: EditableLabel linked to PumpA_Command
```

## Test 05D-1 - Manual live read tag

### Objective

Prove FT Optix can manually read one live/dummy controller tag.

### Actual tested tag

```text
PumpA_Speed
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

## Test 05D-4 - Manual write command tag

### Objective

Prove a command value can be written from FT Optix to the controller tag.

### Actual tested tag

```text
PumpA_Command
```

### Actual tested datatype

```text
DINT
```

### Manual object used

```text
EditableLabel2
```

### XML pattern observed

```text
EditableLabel2
└─ Text
   └─ StringFormatter1
      ├─ Format = {0:d}
      ├─ Mode = 2
      └─ Source0
         └─ DynamicLink = /Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Command
            └─ Mode = 2
```

### Observed result

Latest clean runtime screenshot showed:

```text
FT Optix runtime command value = 7789
Studio 5000 / Logix Designer PumpA_Command = 7789
Studio Output = No Errors
```

Earlier StringFormatter inverse conversion messages were treated as stale/old after the latest clean runtime screenshot showed no active errors.

### Status

```text
PASS
```

### Finding

FT Optix can write a DINT command value to a live/dummy controller tag through an EditableLabel using a StringFormatter and ReadWrite DynamicLink mode.

The practical command write path is:

```text
FT Optix runtime EditableLabel2
→ StringFormatter {0:d}
→ ReadWrite DynamicLink to PumpA_Command
→ imported RA EtherNet/IP tag PumpA_Command
→ FactoryTalk Logix Echo / Studio 5000 controller tag
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
Base tag source path: RAEtherNet_IPDriver1 / RAEtherNet_IPStation1 / Tags / Controller Tags

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
Actual: FT Optix runtime EditableLabel2 wrote 7789 to PumpA_Command; Studio 5000 monitor value updated; latest runtime screenshot showed no active Studio Output errors.
Status: PASS

05D-5 Generator live DynamicLink:
Expected: Generator attaches generated variable to live communication/tag path.
Actual: Not run.
Status: PLANNED
```

## Decision rule

If manual live tag binding fails, do not edit the generator yet.

If manual live tag binding passes, record the exact FT Optix path/API/pattern, then create a generator probe.

05D-1, 05D-2, 05D-3, and 05D-4 passed.

The next safe step is not another manual GUI test.

The next safe step is generator-focused:

```text
Probe 05F XML pattern extraction → Probe 05G minimal generated UI/live-tag binding
```

Do not move to dashboards, alarms, recipes, trends, or dataloggers yet.
