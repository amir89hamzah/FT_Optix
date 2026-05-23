# Probe 05F - XML Pattern Extraction for Live Tags and UI Bindings

## Status

```text
TESTED / PATTERN EXTRACTED
```

## Purpose

This probe records what the exported FactoryTalk Optix XML teaches the generator.

Manual runtime screenshots prove that the HMI works, but screenshots are not enough for code generation.

The exported XML provides the practical project representation that a future VS Code / C# / NetLogic generator must reproduce.

## Input XML files inspected

The user exported these FT Optix nodes after the live tag tests:

```text
ui.xml
commdrivers.xml
```

The files contain the working manual project patterns for:

```text
- REAL live tag read display
- REAL live tag read/write input
- BOOL live tag to operator text mapping
- DINT command write
- RA EtherNet/IP driver, station, route, and imported controller tags
```

## Confirmed communication driver pattern

The communication driver export contains:

```text
CommDrivers
└─ RAEtherNet_IPDriver1
   └─ RAEtherNet_IPStation1
      ├─ Route = 127.0.0.1
      └─ Tags
         └─ Controller Tags
            ├─ PumpA_Speed    DataType i=10  SymbolName PumpA_Speed
            ├─ PumpA_Running  DataType i=1   SymbolName PumpA_Running
            └─ PumpA_Command  DataType i=6   SymbolName PumpA_Command
```

Observed full live tag path format:

```text
/Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/{TagName}
```

Concrete paths:

```text
/Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Speed
/Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Running
/Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Command
```

## Data type observations

Observed controller tag XML datatypes:

```text
PumpA_Running  BOOL   -> DataType i=1
PumpA_Speed    REAL   -> DataType i=10
PumpA_Command  DINT   -> DataType i=6
```

Each imported controller tag also has a child variable:

```text
SymbolName = {TagName}
```

## Pattern 1 - REAL read display

### Manual object

```text
Label1
```

### XML pattern

```text
Label1
└─ Text
   └─ StringFormatter1
      ├─ Format = {0:n1}
      └─ Source0
         └─ DynamicLink = /Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Speed
```

### Meaning for generator

For a read-only numeric display, generate a Label whose Text property uses a StringFormatter and whose Source0 dynamic link points to the live REAL tag.

### Candidate generator intent

```json
{
  "type": "numericDisplay",
  "tag": "PumpA_Speed",
  "format": "{0:n1}",
  "mode": "read"
}
```

## Pattern 2 - REAL read/write editable value

### Manual object

```text
EditableLabel1
```

### XML pattern

```text
EditableLabel1
└─ Text
   └─ StringFormatter1
      ├─ Format = {0:n1}
      ├─ Mode = 2
      └─ Source0
         └─ DynamicLink = /Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Speed
            └─ Mode = 2
```

### Meaning for generator

For an editable REAL value, generate an EditableLabel whose Text property uses a StringFormatter and whose Source0 dynamic link points to the live tag with ReadWrite mode.

Mode value observed:

```text
Mode = 2
```

This matches the previously tested DynamicLink mode finding:

```text
FTOptix.CoreBase.DynamicLinkMode.ReadWrite
```

### Candidate generator intent

```json
{
  "type": "editableNumeric",
  "tag": "PumpA_Speed",
  "format": "{0:n1}",
  "mode": "readWrite"
}
```

## Pattern 3 - BOOL live tag to text mapping

### Manual object

```text
Label2
```

### XML pattern

```text
Label2
└─ Text
   └─ KeyValueConverter1
      ├─ Source
      │  └─ DynamicLink = /Objects/Test/CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Controller Tags/PumpA_Running
      └─ Pairs
         ├─ Pair
         │  ├─ Key = false
         │  └─ Value = Not run
         └─ Pair1
            ├─ Key = true
            └─ Value = Running
```

### Meaning for generator

For operator-facing BOOL status text, generate a Label whose Text property uses a KeyValueConverter.

The converter source dynamic link points to the BOOL controller tag.

The converter pairs define the text mapping.

### Candidate generator intent

```json
{
  "type": "statusText",
  "tag": "PumpA_Running",
  "converter": {
    "type": "keyValue",
    "false": "Not run",
    "true": "Running"
  },
  "mode": "read"
}
```

## Pattern 4 - DINT command write

### Manual object

```text
EditableLabel2
```

### XML pattern

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

### Runtime observation

Manual runtime test showed:

```text
FT Optix runtime value = 7789
Studio 5000 / Logix Designer PumpA_Command = 7789
Studio Output = No Errors in the latest screenshot
```

### Meaning for generator

For a DINT command write field, an EditableLabel with StringFormatter format `{0:d}` and ReadWrite dynamic link mode can write to the DINT live controller tag.

### Candidate generator intent

```json
{
  "type": "commandInput",
  "tag": "PumpA_Command",
  "dataType": "DINT",
  "format": "{0:d}",
  "mode": "readWrite"
}
```

## What this teaches the LLM / generator

The useful knowledge is not only that the HMI works.

The useful generator knowledge is:

```text
1. Live tag paths use /Objects/{Project}/CommDrivers/{Driver}/{Station}/Tags/Controller Tags/{TagName}
2. Imported controller tags are UA variables under Controller Tags.
3. Imported controller tags keep SymbolName = original Logix tag name.
4. REAL read displays can use Label.Text -> StringFormatter -> Source0.DynamicLink.
5. Editable REAL writes can use EditableLabel.Text -> StringFormatter -> Source0.DynamicLink with Mode = 2.
6. BOOL status text can use Label.Text -> KeyValueConverter -> Source.DynamicLink plus false/true pairs.
7. DINT command writes can use EditableLabel.Text -> StringFormatter {0:d} with Mode = 2.
```

## Why this matters

This probe answers the user's concern:

```text
Manual tests alone prove the GUI works.
XML pattern extraction teaches the generator how to recreate it.
```

This is the bridge from manual FT Optix work to generated VS Code / NetLogic / JSON output.

## Next generator direction

The next safe generator probe should not attempt a complete HMI.

It should generate one minimal screen with:

```text
- Label for PumpA_Speed using StringFormatter {0:n1}
- EditableLabel for PumpA_Command using StringFormatter {0:d} and ReadWrite mode
- Label for PumpA_Running using KeyValueConverter false/true text mapping
```

The generator should use the known live tag path pattern and should not create alarms, trends, recipes, or dashboards yet.

## Open question before code generation

The XML proves the project representation.

The next C# probe must determine the exact NetLogic API calls needed to create:

```text
- StringFormatter under a Text property
- KeyValueConverter under a Text property
- Key/value Pair nodes
- DynamicLink child with mode ReadWrite
```

If the API is unclear, create these nodes by the smallest possible C# probe and compare exported XML with this pattern.
