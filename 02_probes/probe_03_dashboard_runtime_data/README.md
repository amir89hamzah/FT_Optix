# Probe 03 - Dashboard runtime data model

## Status

Result: **manual import visually successful, exported back from FT Optix received and compared**.

Environment observed from test:

```text
FT Optix Studio project: NewHMIProject
Import target: Model folder
Imported node: AI_ModelProbe_03
Export-back file: exported_back_from_ftoptix.xml
```

## Visual verification

After import, FT Optix Studio showed:

```text
Model
└─ AI_ModelProbe_03
   ├─ PageDescriptions
   ├─ ProductionLine
   ├─ AlarmTable
   └─ Dashboard
      └─ EnergyConsumption
```

Observed child variable examples:

```text
Dashboard/EnergyConsumption/Drilling/Value
Dashboard/EnergyConsumption/Drilling/Unit
Dashboard/EnergyConsumption/Drilling/Max
Dashboard/EnergyConsumption/Drilling/Min

AlarmTable/Row03/Timestamp
AlarmTable/Row03/Severity
AlarmTable/Row03/Message
AlarmTable/Row03/Acked
AlarmTable/Row03/Active
AlarmTable/Row03/Confirmed
```

## Compare summary

The exported-back XML was compared against the generated Probe 03 XML by reconstructed browse path.

Structural result:

```text
Expected nodes: 100
Exported nodes: 100
Missing paths: 0
Extra paths: 0
UAObjects: 21
UAVariables: 79
```

Data result:

```text
Browse paths: match
Folder/variable structure: match
Variable data types: match
Variable values: match
Descriptions: match
```

Data type counts:

```text
String  i=12 : 31
Float   i=10 : 24
Boolean i=1  : 15
UInt32  i=7  : 9
```

## FT Optix normalization observed

FT Optix normalized several XML details on export:

1. `NamespaceUris/Uri` changed from the generated model URI to the active project namespace:

```text
Generated: AI_Generated_FTOptix_Test
Exported : NewHMIProject
```

2. `ModelUri` changed the same way:

```text
Generated: AI_Generated_FTOptix_Test
Exported : NewHMIProject
```

3. All generated `NodeId` values were replaced by FT Optix-generated `NodeId` values.

4. Variable access attributes changed:

```text
Generated: AccessLevel="3" UserAccessLevel="3"
Exported : AccessLevel="0"
```

`UserAccessLevel` was omitted in the exported-back XML.

## Interpretation

Probe 03 is valid for Model-folder import. FT Optix accepts the generated `UANodeSet` structure and preserves all browse paths, object/variable hierarchy, data types, values, and descriptions.

The generator does not yet produce byte-identical export-back XML because FT Optix rewrites namespace, model URI, node IDs, and access-level attributes. This is acceptable for now, but the generator may later be adjusted if export-back matching becomes the priority.

## Next action

Use this result as the baseline before adding Probe 04.

Recommended Probe 04:

```text
Model object type / instance pattern based on BoilerDemo Model/Pumps:
- MyPump type
- Pump1 instance
- speed, command, alarm, min/max variables
```
