# Official and Public FT Optix Sample Repositories

This note lists public repositories found during research. The main purpose is to identify reusable FT Optix project-source patterns.

## High-value repositories

### FactoryTalk-Optix/Optix_Sample_AllenBradleyDrivesMonitoring

Reason to study:

- good UI component examples
- `DynamicLink` examples
- `Alias` examples
- button event handler examples
- NetLogic attached to UI component examples
- real project folder structure

Observed files:

```text
Nodes/Optix_Sample_AllenBradleyDrivesMonitoring.yaml
Nodes/UI/UI.yaml
Nodes/UI/DrivesMonitoring/Components/UserInterface/UserInterface.yaml
Nodes/Model/Model.yaml
Nodes/NetLogic/NetLogic.yaml
```

### FactoryTalk-Optix/Optix_Sample_AzureIoTOperations

Reason to study:

- clearer Model folder example
- `Model.yaml` includes a child file
- child model file contains simple variables
- good example for splitting model nodes into subfolders/files

Observed files:

```text
Nodes/Model/Model.yaml
Nodes/Model/AIOTags/AIOTags.yaml
Nodes/UI/UI.yaml
```

### FactoryTalk-Optix/Optix_Sample_StoredProceduresNetLogic

Reason to study:

- good NetLogic YAML example
- method definitions under NetLogic
- method input argument structure

Observed files:

```text
Nodes/NetLogic/NetLogic.yaml
```

### FactoryTalk-Optix/Optix_Sample_AlarmsSearchByNameClickForDetails

Reason to study:

- alarm-related project structure
- main project file includes AlarmTypes and Alarms folders
- UI stylesheet and NativePresentationEngine examples

Observed files:

```text
Nodes/Optix_Sample_AlarmsSearchByNameClickForDetails.yaml
Nodes/UI/UI.yaml
```

## Main research finding

Public FT Optix projects commonly expose source structure as YAML under a `Nodes/` directory.

This is different from the standalone XML import probe approach.

Practical meaning:

```text
XML import probes
  → good for Model import testing

YAML project source
  → better for learning full project layout and UI patterns
```
