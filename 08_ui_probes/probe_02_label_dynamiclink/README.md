# UI Probe 02 - Label DynamicLink to Model Variable

## Objective

Create a second label under `UI/MainWindow/Panel1` and bind its text to a Model variable.

Target Model variable:

```text
/Objects/NewHMIProject/Model/StatusText
```

Expected runtime/design value:

```text
Hello from YAML project source
```

## Current confirmed Model variable

`Nodes/Model/Model.yaml` contains:

```yaml
- Name: StatusText
  Type: BaseDataVariableType
  DataType: String
  Value: "Hello from YAML project source"
```

## Edit target

Edit:

```text
Nodes/UI/UI.yaml
```

Find:

```yaml
- Name: Panel1
  Type: Panel
  Children:
```

Then add `Label2` under the same `Children:` level as `Label1`.

## YAML to add

```yaml
    - Name: Label2
      Type: Label
      Children:
      - Name: Text
        Type: BaseDataVariableType
        DataType: LocalizedText
        ModellingRule: Optional
        Children:
        - Name: DynamicLink
          Type: DynamicLink
          DataType: NodePath
          Value: "/Objects/NewHMIProject/Model/StatusText"
      - Name: Width
        Type: BaseVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 500.0
      - Name: Height
        Type: BaseVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 50.0
      - Name: TopMargin
        Type: BaseVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 70.0
      - Name: FontSize
        Type: BaseDataVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 30.0
```

## Expected result

After opening the project in FT Optix Studio:

```text
Panel1
├─ Label1 = Manual Test Label
└─ Label2 = Hello from YAML project source
```

## Interpretation

If Label2 appears and shows the model value, then YAML UI generation with DynamicLink works.

If the project opens but Label2 is blank, the label node is valid but the DynamicLink path or data type needs adjustment.

If the project fails to open, the YAML format for DynamicLink needs adjustment.
