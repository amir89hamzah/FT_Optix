# YAML UI Patterns

This note collects reusable UI patterns observed from public FT Optix sample repositories.

## 1. StyleSheet pattern

A UI file commonly defines a default stylesheet:

```yaml
Name: UI
Type: UICategoryFolder
Children:
- Name: DefaultStyleSheet
  Type: StyleSheet
  Children:
  - Name: ButtonStyles
    Type: BaseObjectType
    Children:
    - Name: Default
      Type: ButtonStyle
    - Name: Accent
      Type: ButtonStyle
      Children:
      - Name: Color
        Type: BaseDataVariableType
        DataType: Color
        Value: "#0fafff"
```

Useful style categories observed:

```text
ButtonStyles
InputBoxStyles
SwitchStyles
GaugeStyles
NavigationPanelStyles
ChartStyles
DataListStyles
ToolTipStyles
ScrollBarStyles
AccordionStyles
PieChartStyles
HistogramChartStyles
LabelStyles
RadioButtonStyles
CheckBoxStyles
RectangleStyles
ScreenStyles
```

## 2. NativePresentationEngine pattern

A UI project can define the native presentation engine and start window:

```yaml
- Name: NativePresentationEngine
  Type: NativePresentationEngine
  ReferenceType: HasOrderedComponent
  Children:
  - Name: StartWindow
    Type: NodePointer
    DataType: NodeId
    Value: "/Objects/ProjectName/UI/MainWindow"
    Children:
    - Name: Kind
      Type: PropertyType
      DataType: NodeId
      Value: "/Types/ObjectTypes/BaseObjectType/BaseUIObject/Window"
  - Name: StyleSheet
    Type: NodePointer
    DataType: NodeId
    Value: "/Objects/ProjectName/UI/DefaultStyleSheet"
```

## 3. Basic layout pattern

Reusable components often use layout objects such as:

```text
RowLayout
ColumnLayout
Panel
Rectangle
Label
Button
CheckBox
```

Example component root:

```yaml
- Name: DataElement
  Supertype: RowLayout
  ReferenceType: HasComponent
  Children:
  - Name: Width
    Type: BaseVariableType
    DataType: Size
    ModellingRule: Optional
    Value: 210.0
  - Name: Height
    Type: BaseVariableType
    DataType: Size
    ModellingRule: Optional
    Value: 30.0
```

## 4. Label pattern

A basic label:

```yaml
- Name: LabelValue
  Type: Label
  Children:
  - Name: Text
    Type: BaseDataVariableType
    DataType: LocalizedText
    Value: {"LocaleId":"en-US","Text":"Hello"}
```

A label with dynamic text:

```yaml
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

## 5. Button event handler pattern

A button can contain an event handler for mouse click:

```yaml
- Name: MouseClickEventHandler1
  Type: EventHandler
  Children:
  - Name: ListenEventType
    Type: PropertyType
    DataType: NodeId
    Value: "/Types/EventTypes/BaseEventType/MouseEvent/MouseClickEvent"
  - Name: MethodsToCall
    Type: BaseObjectType
    Children:
    - Name: MethodContainer1
      Type: BaseObjectType
      Children:
      - Name: ns=3;ObjectPointer
        Type: NodePointer
        DataType: NodeId
      - Name: ns=3;Method
        Type: BaseDataVariableType
        DataType: String
        Value: "RemoveDevice"
```

This is an advanced pattern and should be tested after basic UI generation works.

## 6. Resource path pattern

Images can be referenced by project directory resource paths:

```yaml
- Name: ImagePath
  Type: BaseDataVariableType
  DataType: ResourceUri
  ModellingRule: Optional
  Value: "ns=18;%PROJECTDIR%/delete_24dp_000000.svg"
```

For generated projects, asset copying becomes part of the problem.

## 7. Generator priority

Recommended UI generation order:

```text
1. Label with static text
2. Rectangle / panel with size and position
3. Label with DynamicLink to local variable
4. Label with DynamicLink to Model path
5. Button with static style
6. Button with event handler
7. Image with ResourceUri
8. PanelLoader / navigation
```
