# BoilerDemo reference review

Status: reference sample received in chat as `Nodes.zip` plus runtime screenshots.

Do not commit the raw screenshots or ZIP unless they are reviewed and sanitized first. Screenshots can expose machine paths, remote-session IDs, usernames, project/customer names, or other sensitive details.

## Why this sample matters

The sample appears to be a complete FT Optix project node export, not just generated XML. It is useful as a ground-truth reference for how FT Optix Studio stores project structure in YAML.

Observed top-level node folders:

```text
Nodes/
├─ Alarms
├─ CommDrivers
├─ DataStores
├─ Loggers
├─ Model
├─ NetLogic
├─ OPC-UA
├─ Recipes
├─ Reports
├─ Retentivity
├─ Security
├─ Translations
└─ UI
```

Runtime screenshots show the following pages/tabs:

```text
Overview
DataLogger
Recipes
Alarms
Trends
Dashboard
```

The YAML has matching UI concepts such as screens, navigation items, datalogger view, recipe editor/list, alarm live/historical pages, and trends.

## Important patterns found

### 1. Navigation panel pattern

The UI uses a main window/navigation layout with navigation items pointing to screens, for example:

```text
Overview   -> UI/Screens/MainPage
DataLogger -> UI/Screens/DataPage
Recipes    -> UI/Screens/RecipesPage
Alarms     -> UI/Screens/AlarmsPage
Trends     -> UI/Screens/Trends
Dashboard  -> UI/Screens/Dashboard
```

This is a strong candidate for a later UI/navigation probe after Model-folder probes are stable.

### 2. DynamicLink pattern

Many nodes use a child named `DynamicLink`:

```yaml
- Name: DynamicLink
  Type: DynamicLink
  DataType: NodePath
  Value: "relative/or/absolute/node/path"
  Children:
  - Name: Mode
    Type: BaseVariableType
    DataType: DynamicLinkMode
    Value: 2
```

This should become its own reusable pattern note and later an XML/YAML probe.

### 3. Model object type / instance pattern

The sample has a pump model pattern with a custom object type and an instance:

```text
Model/Pumps
├─ Pump1 : MyPump
└─ MyPump : BaseObjectType
```

Common variables include:

```text
SetSpeed
CurrentSpeed
Command
Alarm
InputDatalink
OutputDatalink
Cw
Ccw
UseRunFeedback
MinSpeed
MaxSpeed
JoggingMode
```

This is a good candidate for the next Model-only probe because it is smaller and safer than full UI generation.

### 4. Recipe pattern

Recipe data is split between model target variables and recipe schema:

```text
Model/Recipes
├─ BoilerLevelSetpoint
└─ BoilerTempSetpoint

Recipes/RecipeSchema
├─ Root
├─ Store -> DataStores/EmbeddedDatabase
├─ TargetNode -> Model/Recipes
└─ EditModel
```

The recipe UI page then calls recipe methods such as save, delete, apply, load, refresh, and copy operations.

### 5. DataLogger pattern

Data logging uses a chain like:

```text
Loggers/DataLogger
├─ SamplingMode
├─ Store -> DataStores/EmbeddedDatabase
└─ VariablesToLog
   └─ VariableToLog + DynamicLink

DataStores/EmbeddedDatabase
└─ SQLiteStoreTable/DataLogger
```

Runtime page shows a table plus trends, matching this structure.

### 6. Alarm pattern

Alarm controllers include:

```text
ExclusiveLevelAlarmController
DigitalAlarmController
```

Typical children/properties:

```text
InputValue + DynamicLink
LowLimit or boolean trigger
Message
Severity
LastEvent
```

The UI has live/historical alarm tables and actions such as acknowledge, acknowledge all, confirm, and confirm all.

## Updated direction

Keep the project approach probe-first. Do not jump directly into full dashboard XML.

Recommended near-term sequence:

1. Finish Probe 03: dashboard/runtime Model-folder XML import-export verification.
2. Add a small YAML inspection script for uploaded FT Optix `Nodes` folders or ZIP files.
3. Add Probe 04: Model object type / instance pattern based on the pump sample.
4. Add Probe 05: Recipe target model pattern.
5. Add Probe 06: DataLogger-ready model variables and datastore/logging pattern.
6. Add Probe 07: simple navigation/screen UI pattern only after the Model and binding patterns are stable.

## Verification questions for later

- Which FT Optix YAML fields are required vs generated automatically by Studio?
- Can a minimal `Nodes/Model/...yaml` pattern be manually added and opened safely by Studio?
- Which patterns must be generated as XML import under Model, and which require project YAML editing?
- How does FT Optix normalize DynamicLink paths after save/export?
- What is the smallest possible UI screen/navigation node set that Studio accepts?
