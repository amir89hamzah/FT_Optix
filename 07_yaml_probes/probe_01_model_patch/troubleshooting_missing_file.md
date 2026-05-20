# Troubleshooting - Missing file error

## Error pattern

FT Optix Studio shows:

```text
Exception caught: Missing file '.../Nodes/Model/AI_YAMLProbe_01/AI_YAMLProbe_01.yaml'
```

This means FT Optix did not reach YAML parsing yet.

It is still trying to locate the file referenced by `Model.yaml`.

## Check 1 - Model.yaml include path

If `Model.yaml` says:

```yaml
Name: Model
Type: ModelCategoryFolder
Children:
- File: AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

then the file must exist at:

```text
Nodes/Model/AI_YAMLProbe_01/AI_YAMLProbe_01.yaml
```

## Check 2 - exact project folder

Make sure the file was copied into the exact same project folder that FT Optix is opening.

Watch out for similar folder names such as:

```text
TEST2
TEST 2
TEST_2
```

FT Optix may be opening one folder while Windows Explorer is showing another.

## Check 3 - file extension

Make sure the file is not actually:

```text
AI_YAMLProbe_01.yaml.txt
```

Enable this in Windows Explorer:

```text
View > File name extensions
```

## Check 4 - file name spelling

The filename must match exactly:

```text
AI_YAMLProbe_01.yaml
```

The folder name must match exactly:

```text
AI_YAMLProbe_01
```

## Check 5 - test with absolute path evidence

Right-click the YAML file and choose Properties.

Compare the Location path with the path shown in the FT Optix error.

They must match exactly up to:

```text
Nodes/Model/AI_YAMLProbe_01
```

## Meaning

If the error is still `Missing file`, the problem is path/location/name.

If the file is found but the YAML is wrong, FT Optix should show a different YAML parsing or node error.
