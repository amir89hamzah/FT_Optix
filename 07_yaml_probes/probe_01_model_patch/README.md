# YAML Probe 01 - Model Patch

## Objective

Test whether FT Optix Studio accepts a manually added YAML Model node file inside an existing project source folder.

This is **not** an XML import test.

This is a project-source YAML patch test.

## Expected result in FT Optix

After the patch is applied and the project is opened in FT Optix Studio, the `Model` folder should show:

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

## Files in this probe

```text
patch_files/
└─ Nodes/
   └─ Model/
      ├─ Model.add-this-child-line.yaml
      └─ AI_YAMLProbe_01/
         └─ AI_YAMLProbe_01.yaml
```

## How to test

### 1. Create a blank FT Optix project

Use a disposable test project first.

Do not use a customer project.

### 2. Close FT Optix Studio

Do not edit project source YAML while the project is open.

### 3. Find the project source folder

Look for a folder that contains:

```text
Nodes/
```

Inside it, find:

```text
Nodes/Model/Model.yaml
```

### 4. Copy this folder into the project

Copy:

```text
patch_files/Nodes/Model/AI_YAMLProbe_01/
```

into:

```text
<YourProject>/Nodes/Model/AI_YAMLProbe_01/
```

### 5. Edit `Nodes/Model/Model.yaml`

Add this under `Children:`:

```yaml
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

If the file currently has only:

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

### 6. Open the project in FT Optix Studio

Check whether `AI_YAMLProbe_01` appears under `Model`.

## What to report back

Please capture:

1. Screenshot of Model tree after opening.
2. Any error shown by FT Optix Studio.
3. If successful, export the `AI_YAMLProbe_01` node back to XML.
4. Also copy back the normalized `Nodes/Model/AI_YAMLProbe_01/AI_YAMLProbe_01.yaml` if FT Optix modifies it.

## Why this probe matters

XML import is already confirmed working for Model-folder probes.

This YAML probe checks whether full project-source generation is possible by writing FT Optix YAML files directly.
