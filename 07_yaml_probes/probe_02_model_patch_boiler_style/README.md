# YAML Probe 02 - BoilerDemo-style Model Patch

## Objective

Repeat YAML Model patch test using the same style observed in the BoilerDemo project:

```yaml
Name: Model
Id: g=...
Type: ModelCategoryFolder
Children:
- File: Recipes/Recipes.yaml
- File: Pumps/Pumps.yaml
- File: Widgets/Widgets.yaml
```

This probe uses a folder-per-feature layout instead of a flat file layout.

## Recommended project folder layout

```text
NewHMIProject/
└─ Nodes/
   └─ Model/
      ├─ Model.yaml
      └─ AI_YAMLProbe_01/
         └─ AI_YAMLProbe_01.yaml
```

## Important rule

Do not delete existing lines in `Model.yaml`.

If `Model.yaml` already has an `Id: g=...`, keep it.

Only add this line under `Children:`:

```yaml
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

## Example Model.yaml

If your current `Model.yaml` has only:

```yaml
Name: Model
Type: ModelCategoryFolder
```

change it to:

```yaml
Name: Model
Type: ModelCategoryFolder
Children:
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

If your current `Model.yaml` has an Id:

```yaml
Name: Model
Id: g=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
Type: ModelCategoryFolder
Children:
- File: ExistingFolder/ExistingFile.yaml
```

change it to:

```yaml
Name: Model
Id: g=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
Type: ModelCategoryFolder
Children:
- File: ExistingFolder/ExistingFile.yaml
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

## Expected result

After opening the project in FT Optix Studio, the Model folder should show:

```text
AI_YAMLProbe_01
├─ StatusText
├─ TestNumber
├─ TestBool
├─ Notes
└─ RuntimeData
   ├─ SpeedPercent
   ├─ Running
   └─ LastMessage
```
