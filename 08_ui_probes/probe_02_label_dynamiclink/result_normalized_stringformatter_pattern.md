# Result - Normalized DynamicLink Pattern with StringFormatter

## Source

Uploaded `Nodes.zip` after the working runtime test.

## Working behavior

Runtime displayed:

```text
"Hello from YAML project source"
```

This proves that the UI label can read the Model variable using a generated YAML DynamicLink.

## Important normalized YAML pattern

FT Optix normalized the working `Label2/Text` into this structure:

```yaml
- Name: Label2
  Type: Label
  Children:
  - Name: Text
    Type: BaseDataVariableType
    DataType: LocalizedText
    Children:
    - Name: StringFormatter1
      Type: StringFormatter
      Children:
      - Name: Format
        Type: BaseDataVariableType
        DataType: LocalizedText
        ReferenceType: HasParameter
        ModellingRule: Optional
        Value: {"LocaleId":"","Text":"{0:j}"}
      - Name: ns=2;Source0
        Type: BaseDataVariableType
        DataType: BaseDataType
        ReferenceType: HasSource
        Children:
        - Name: DynamicLink
          Type: DynamicLink
          DataType: NodePath
          Value: "/Objects/NewHMIProject/Model/StatusText@Value"
```

## Key discovery

The first direct DynamicLink pattern was accepted by FT Optix but displayed `####` in the designer:

```yaml
Text -> DynamicLink -> /Objects/NewHMIProject/Model/StatusText
```

The working pattern uses:

```text
Text
└─ StringFormatter1
   └─ Source0
      └─ DynamicLink -> /Objects/NewHMIProject/Model/StatusText@Value
```

## Meaning

For a `Label.Text` property with `DataType: LocalizedText`, linking directly to a `String` variable may not display cleanly.

The safer generated pattern is:

```text
LocalizedText Text property
  -> StringFormatter
  -> Source0 DynamicLink to ModelVariable@Value
```

## Note about quotes

The runtime displayed quotes around the string:

```text
"Hello from YAML project source"
```

This is likely caused by the formatter string:

```text
{0:j}
```

The `j` format appears to output JSON-style formatting.

Next test should try:

```text
{0}
```

instead of:

```text
{0:j}
```

Expected result:

```text
Hello from YAML project source
```

without quotation marks.
