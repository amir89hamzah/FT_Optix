# XML Import Format vs YAML Project Source Format

This note compares two FT Optix-related formats used during the learning project.

## 1. XML import format

The XML probe files in this repository use OPC UA `UANodeSet` style XML.

Typical root:

```xml
<UANodeSet>
  <NamespaceUris>
    <Uri>AI_Generated_FTOptix_Test</Uri>
  </NamespaceUris>
  <Models>
    <Model ModelUri="AI_Generated_FTOptix_Test" />
  </Models>
  <UAObject ... />
  <UAVariable ... />
</UANodeSet>
```

A folder is represented as `UAObject`.

A variable is represented as `UAVariable`.

A child relationship is represented by `ParentNodeId` plus a reverse `HasComponent` reference.

Example concept:

```xml
<UAVariable
  NodeId="ns=1;g=..."
  ParentNodeId="ns=1;g=..."
  BrowseName="1:StatusText"
  DataType="i=12">
  <DisplayName>StatusText</DisplayName>
  <References>
    <Reference ReferenceType="i=40">i=63</Reference>
    <Reference ReferenceType="i=47" IsForward="false">PARENT_NODE_ID</Reference>
  </References>
  <Value>
    <uax:String>Hello</uax:String>
  </Value>
</UAVariable>
```

### XML strengths

- Good for import/export probe testing.
- Good for Model-folder generation.
- Uses OPC UA concepts directly.
- Explicit `NodeId`, `BrowseName`, `ParentNodeId`, `References`, and `DataType`.
- Easier to validate as a standalone import file.

### XML weaknesses

- Verbose.
- Harder for humans to read.
- Parent-child relation is not visually obvious.
- Easy to break if `NodeId`, `ParentNodeId`, or references are wrong.
- So far, this project has only confirmed import under the `Model` folder.
- Full UI/page import through XML is still unconfirmed.

## 2. YAML project source format

Public FT Optix sample projects on GitHub commonly store project nodes as YAML under a `Nodes/` folder.

Typical shape:

```yaml
Name: UI
Type: UICategoryFolder
Children:
- Name: DefaultStyleSheet
  Type: StyleSheet
  Children:
  - Name: ButtonStyles
    Type: BaseObjectType
```

A UI component can be represented as nested YAML:

```yaml
- Name: DataElement
  Supertype: RowLayout
  Children:
  - Name: LabelValue
    Type: Label
    Children:
    - Name: Text
      Type: BaseDataVariableType
      DataType: LocalizedText
      Children:
      - Name: DynamicLink
        Type: DynamicLink
        DataType: NodePath
        Value: "../../Value"
```

### YAML strengths

- Much easier to read.
- The hierarchy is obvious because of `Children` nesting.
- Closer to how public FT Optix projects are stored in Git repositories.
- Better for full project generation.
- Better for UI, page, panel, style, DynamicLink, and NetLogic learning.
- More natural for LLM generation than verbose XML.

### YAML weaknesses

- It may require a full FT Optix project folder structure.
- It may not be importable through the same UI flow as XML import.
- Generated YAML must fit the expected project layout under `Nodes/`.
- Need more testing before editing a real project folder directly.

## 3. Main comparison table

| Area | XML import format | YAML project source format |
|---|---|---|
| Main use | Import/export probes | Full project source tree |
| Current confirmed target | Model folder | Public sample project structure |
| Root style | `UANodeSet` | `Nodes/*.yaml` files |
| Folder representation | `UAObject` + `FolderType` | `Name` + `Type` + `Children` |
| Variable representation | `UAVariable` | `Name`, `Type`, `DataType`, `Value` |
| Parent-child relation | `ParentNodeId` + reference | YAML nesting under `Children` |
| Readability | Low | High |
| Good for LLM generation | Medium | High |
| Good for quick import test | High | Medium / unconfirmed |
| Good for full UI project | Unconfirmed | High potential |
| Risk | broken NodeId/reference | broken project folder structure |

## 4. Practical strategy

Use both formats, but for different jobs.

```text
XML
→ small import probes
→ Model-folder validation
→ exact OPC UA node relationship testing

YAML
→ public FT Optix project study
→ UI/page/style/DynamicLink learning
→ possible full project generation later
```

## 5. Recommended next work

### Stage A - keep XML probes

Continue XML work for:

- Model folders
- variables
- data types
- repeated runtime data structure
- alarm model structure

### Stage B - start YAML research

Add notes and examples for:

- `Nodes/UI/UI.yaml`
- `Nodes/Model/Model.yaml`
- `DynamicLink` pattern
- `Label` pattern
- `Panel`, `RowLayout`, `ColumnLayout`
- project-level folder structure

### Stage C - generate a small YAML project patch

Do not generate a full FT Optix project yet.

First test a tiny YAML component:

```text
1 Label
1 DynamicLink
1 Model variable
```

Then open the project in FT Optix and see whether the software accepts or normalizes it.

## 6. Blunt conclusion

XML is the safer path for controlled import experiments.

YAML is likely the better path for learning and generating full FT Optix project source.

The correct long-term project should support both:

```text
generate XML probes
and
learn/generate YAML project nodes
```
