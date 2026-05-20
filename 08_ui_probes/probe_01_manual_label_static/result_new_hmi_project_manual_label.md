# UI Probe 01 - Manual Static Label Result

## Project

```text
NewHMIProject
```

## Manual action in FT Optix Studio

A `Panel` was created under:

```text
UI/MainWindow (type)
```

Then a `Label` was added under the panel.

Manual label text:

```text
Manual Test Label
```

Manual font size:

```text
40
```

## Observed YAML pattern

The generated UI YAML was found in:

```text
Nodes/UI/UI.yaml
```

Relevant structure:

```yaml
- Name: MainWindow
  Id: g=01a7e8933afd7c95fe894b0859308138
  Supertype: Window
  Children:
  - Name: Width
    Type: BaseDataVariableType
    DataType: Size
    ModellingRule: Optional
    Value: 1920.0
  - Name: Height
    Type: BaseDataVariableType
    DataType: Size
    ModellingRule: Optional
    Value: 1080.0
  - Name: Caption
    Type: BaseDataVariableType
    DataType: String
    Children:
    - Name: DynamicLink
      Type: DynamicLink
      DataType: NodePath
      Value: "/Objects/NewHMIProject@BrowseName"
  - Name: Panel1
    Type: Panel
    Children:
    - Name: Width
      Type: BaseVariableType
      DataType: Size
      ModellingRule: Optional
      Value: 300.0
    - Name: Height
      Type: BaseVariableType
      DataType: Size
      ModellingRule: Optional
      Value: 300.0
    - Name: Label1
      Type: Label
      Children:
      - Name: Text
        Type: BaseDataVariableType
        DataType: LocalizedText
        ModellingRule: Optional
        Value: {"LocaleId":"en-US","Text":"Manual Test Label"}
      - Name: Width
        Type: BaseVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 150.0
      - Name: Height
        Type: BaseVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 50.0
      - Name: FontSize
        Type: BaseDataVariableType
        DataType: Size
        ModellingRule: Optional
        Value: 40.0
```

## Important findings

1. `MainWindow` is stored inline inside `Nodes/UI/UI.yaml`.
2. The start window points to `/Objects/NewHMIProject/UI/MainWindow`.
3. A generated label can likely be inserted under `MainWindow/Panel1/Children`.
4. Static label text uses `DataType: LocalizedText` with JSON-style value.
5. Font size uses `DataType: Size`.

## Next probe

Create a second label under the same panel, but replace static `Text` value with a `DynamicLink` to:

```text
/Objects/NewHMIProject/Model/StatusText
```
