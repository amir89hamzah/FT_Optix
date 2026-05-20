# Recommended Test - Flat YAML File

This is the recommended first YAML patch test because it avoids folder/path confusion.

## Project source folder

Use the FT Optix project folder that contains:

```text
NewHMIProject.optix
Nodes/
ProjectFiles/
ApplicationFiles/
```

For the current test example:

```text
C:\Users\nicodemus\Desktop\Cuba\NewHMIProject
```

## Do not edit root project file first

The root file:

```text
Nodes/NewHMIProject.yaml
```

already includes:

```yaml
- File: Model/Model.yaml
```

So for Model testing, only edit:

```text
Nodes/Model/Model.yaml
```

## File placement

Put the generated probe YAML directly here:

```text
Nodes/Model/AI_YAMLProbe_01.yaml
```

Final folder layout:

```text
NewHMIProject/
└─ Nodes/
   └─ Model/
      ├─ Model.yaml
      └─ AI_YAMLProbe_01.yaml
```

## Model.yaml

Use this content:

```yaml
Name: Model
Type: ModelCategoryFolder
Children:
- File: AI_YAMLProbe_01.yaml
```

## AI_YAMLProbe_01.yaml

Use the file from:

```text
patch_files_flat/Nodes/Model/AI_YAMLProbe_01.yaml
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
```

## If still failing

If the error says `Missing file`, it is still a path/name issue.

If the error changes to YAML parsing or object/type error, then FT Optix has found the file and is now reading the content.
