# FT Optix YAML Project Structure Notes

Public FT Optix sample repositories commonly store project source under a `Nodes/` folder.

## 1. Top-level project file

A top-level project YAML file usually starts with:

```yaml
Name: ProjectName
Type: ProjectFolder
Children:
- Name: PasswordPolicy
  Type: PasswordPolicy
- File: UI/UI.yaml
- File: Model/Model.yaml
- File: Converters/Converters.yaml
- File: Alarms/Alarms.yaml
- File: Recipes/Recipes.yaml
- File: Loggers/Loggers.yaml
- File: DataStores/DataStores.yaml
- File: Reports/Reports.yaml
- File: 'OPC-UA/OPC-UA.yaml'
- File: CommDrivers/CommDrivers.yaml
- File: NetLogic/NetLogic.yaml
- File: Security/Security.yaml
- File: Translations/Translations.yaml
- File: Retentivity/Retentivity.yaml
```

Pattern observed:

```text
ProjectFolder
  ├─ inline config nodes
  ├─ File: UI/UI.yaml
  ├─ File: Model/Model.yaml
  ├─ File: NetLogic/NetLogic.yaml
  └─ other project category files
```

## 2. Category folders

Common category files look like this:

```yaml
Name: Model
Type: ModelCategoryFolder
```

```yaml
Name: UI
Type: UICategoryFolder
```

```yaml
Name: NetLogic
Type: NetLogicCategoryFolder
```

## 3. File splitting pattern

A category file can include child files.

Example:

```yaml
Name: Model
Type: ModelCategoryFolder
Children:
- File: AIOTags/AIOTags.yaml
```

The child file then contains the actual folder and variables:

```yaml
Name: AIOTags
Type: FolderType
Children:
- Name: Variable1
  Type: BaseDataVariableType
  DataType: Int32
- Name: Variable2
  Type: BaseDataVariableType
  DataType: Int32
```

This is very useful for generation. A generator can create one YAML file per folder or feature.

## 4. Node basics

Most YAML nodes follow this shape:

```yaml
- Name: NodeName
  Id: g=optional_guid
  Type: SomeType
  DataType: SomeDataType
  Value: optional_value
  Children:
  - Name: ChildNode
    Type: ChildType
```

Common keys:

| Key | Meaning |
|---|---|
| `Name` | node browse/name in project tree |
| `Id` | optional stable node id, often `g=...` |
| `Type` | FT Optix node type |
| `DataType` | variable data type |
| `Value` | initial/default value |
| `Children` | nested child nodes |
| `File` | include another YAML file |
| `ReferenceType` | non-default reference relation |
| `Supertype` | custom object/panel based on another type |
| `ModellingRule` | optional/required modelling rule |

## 5. UI root pattern

UI file commonly starts as:

```yaml
Name: UI
Type: UICategoryFolder
Children:
- Name: DefaultStyleSheet
  Type: StyleSheet
- Name: NativePresentationEngine
  Type: NativePresentationEngine
```

A presentation engine can point to a start window:

```yaml
- Name: NativePresentationEngine
  Type: NativePresentationEngine
  ReferenceType: HasOrderedComponent
  Children:
  - Name: StartWindow
    Type: NodePointer
    DataType: NodeId
    Value: "/Objects/ProjectName/UI/MainWindow"
```

## 6. Important difference from XML

YAML uses natural nesting:

```yaml
Children:
- Name: Parent
  Children:
  - Name: Child
```

XML import uses explicit references:

```text
ParentNodeId + HasComponent reference
```

So YAML is much easier for full project generation.

## 7. Generator implication

A future generator should probably produce:

```text
Nodes/<ProjectName>.yaml
Nodes/Model/Model.yaml
Nodes/Model/<Feature>/<Feature>.yaml
Nodes/UI/UI.yaml
```

Instead of trying to generate one giant file.
