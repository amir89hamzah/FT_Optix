# YAML Probe 00 - Inline Model Node

## Purpose

This diagnostic probe avoids `File:` include completely.

Use it when FT Optix shows a `Missing file` error for a generated child YAML file.

## Idea

Instead of adding:

```yaml
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

add the test node directly inside `Nodes/Model/Model.yaml`.

## Example

If `Model.yaml` currently contains:

```yaml
Name: Model
Id: g=66fec071d515e7b13a7b27eca2317830
Type: ModelCategoryFolder
Children:
- File: Recipes/Recipes.yaml
- File: Pumps/Pumps.yaml
- File: Widgets/Widgets.yaml
```

change it to:

```yaml
Name: Model
Id: g=66fec071d515e7b13a7b27eca2317830
Type: ModelCategoryFolder
Children:
- File: Recipes/Recipes.yaml
- File: Pumps/Pumps.yaml
- File: Widgets/Widgets.yaml
- Name: AI_InlineProbe_01
  Type: FolderType
  Children:
  - Name: StatusText
    Type: BaseDataVariableType
    DataType: String
    Value: "Hello from inline YAML"
  - Name: TestNumber
    Type: BaseDataVariableType
    DataType: Float
    Value: 123.45
```

## Expected result

If this works, FT Optix accepts direct inline YAML node edits.

If this fails, the problem is not only `File:` path. It may be generated node content, unsupported edit method, or project normalization behavior.

## Next diagnosis

- Inline works, File include fails: path, filename, extension, or file encoding issue.
- Inline fails too: generated node format needs adjustment.
