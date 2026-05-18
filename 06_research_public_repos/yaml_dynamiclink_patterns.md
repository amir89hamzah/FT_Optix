# YAML DynamicLink Patterns

DynamicLink is one of the most important FT Optix patterns for UI generation.

It connects a UI property to another node path.

## 1. Label text bound to nearby value

A UI label can bind its `Text` variable to another node using a relative path.

Pattern:

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

Meaning:

```text
LabelValue/Text reads from ../../Value
```

This is useful for reusable UI components.

## 2. Label text bound to description and unit

A common reusable object can expose:

```text
Description
Value
Unit
```

Then UI labels bind to each field:

```yaml
- Name: LabelDesc
  Type: Label
  Children:
  - Name: Text
    Type: BaseDataVariableType
    DataType: LocalizedText
    Children:
    - Name: DynamicLink
      Type: DynamicLink
      DataType: NodePath
      Value: "../../Description"

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

- Name: LabelUnit
  Type: Label
  Children:
  - Name: Text
    Type: BaseDataVariableType
    DataType: LocalizedText
    Children:
    - Name: DynamicLink
      Type: DynamicLink
      DataType: NodePath
      Value: "../../Unit"
```

## 3. Alias-based DynamicLink

A reusable UI component can use an `Alias` node.

Pattern:

```yaml
- Name: AxisObjectAlias
  Type: Alias
  DataType: NodeId
  Children:
  - Name: Kind
    Type: PropertyType
    DataType: NodeId
    Value: "/Objects/ProjectName/UI/Components/Variables/AxisObject"
```

Then child controls can bind through the alias:

```yaml
- Name: Checked
  Type: BaseDataVariableType
  DataType: Boolean
  Children:
  - Name: DynamicLink
    Type: DynamicLink
    DataType: NodePath
    Value: "{AxisObjectAlias}/Enabled"
```

Meaning:

```text
Read/write the Enabled variable of whatever node is assigned to AxisObjectAlias.
```

## 4. DynamicLink mode

Some DynamicLinks contain a `Mode` child:

```yaml
- Name: Mode
  Type: BaseVariableType
  DataType: DynamicLinkMode
  ModellingRule: Optional
  Value: 2
```

Observed use:

```text
Mode = 2
```

Likely practical meaning:

```text
read/write or bidirectional behavior
```

This needs confirmation by FT Optix testing.

## 5. NodeId pointer via DynamicLink

For event handler method calls, an ObjectPointer can dynamically point to a NetLogic node:

```yaml
- Name: ns=3;ObjectPointer
  Type: NodePointer
  DataType: NodeId
  Children:
  - Name: Kind
    Type: PropertyType
    DataType: NodeId
    Value: "/Types/ObjectTypes/BaseObjectType"
  - Name: DynamicLink
    Type: DynamicLink
    DataType: NodePath
    Value: "../../../../../DeleteDriveFromListLogic@NodeId"
    Children:
    - Name: Mode
      Type: BaseVariableType
      DataType: DynamicLinkMode
      ModellingRule: Optional
      Value: 2
```

The important part is:

```text
@NodeId
```

That means the DynamicLink is not reading the object value. It is reading the target node id.

## 6. Generator implication

A UI generator should support these DynamicLink target types:

```text
relative value path       ../../Value
relative unit path        ../../Unit
alias path                {AliasName}/VariableName
node id path              SomeNode@NodeId
absolute project path     /Objects/ProjectName/Model/SomeVariable
```

Start with relative path first. Alias and event handlers are more advanced.
