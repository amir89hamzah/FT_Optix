# Probe 05B - Tag source intent metadata generator

## Purpose

Probe 05B continues from Probe 05A.

Probe 05A hardened the JSON shape so every runtime variable must explicitly declare whether its source is:

```text
plcTag
mock
static
```

Probe 05B uses a DesignTime NetLogic method to read that hardened JSON and create Model nodes in FT Optix.

Important: this probe does **not** create real DynamicLinks yet.

Instead, it preserves the source intent beside each generated variable using helper metadata variables such as:

```text
CurrentSpeed_SourceKind
CurrentSpeed_SourceTag
CurrentSpeed_SourceMode
```

This lets us verify that the JSON generator understands PLC-backed intent before we attempt the more fragile DynamicLink API pattern.

## Input JSON

Use the existing valid Probe 05A example:

```text
06_specs/examples/valid_tag_backed_variables.json
```

For quick FT Optix testing, copy that file to:

```text
C:\Temp\ftoptix_probe05b_tag_backed_variables.json
```

The C# file in this folder uses that path by default.

## C# file

```text
DesignTimeTagMetadataGenerator.cs
```

Paste the contents into a FT Optix **DesignTime NetLogic** class, or use it as a reference when editing `DesignTimeNetLogic1.cs`.

## Method to run

```text
GenerateTagMetadataModel()
```

## Expected Model structure

After running the method, the Model folder should contain:

```text
Model
└─ AI_TagSchemaProbe_01
   ├─ PumpA
   │  ├─ CurrentSpeed
   │  ├─ CurrentSpeed_Role
   │  ├─ CurrentSpeed_SourceKind
   │  ├─ CurrentSpeed_SourceTag
   │  ├─ CurrentSpeed_SourceMode
   │  ├─ CurrentSpeed_DisplayUnit
   │  ├─ CurrentSpeed_DisplayDecimals
   │  ├─ SetSpeed
   │  ├─ SetSpeed_Role
   │  ├─ SetSpeed_SourceKind
   │  ├─ SetSpeed_SourceTag
   │  ├─ SetSpeed_SourceMode
   │  ├─ SetSpeed_DisplayUnit
   │  ├─ SetSpeed_DisplayDecimals
   │  ├─ SetSpeed_DisplayMin
   │  ├─ SetSpeed_DisplayMax
   │  └─ ...
   └─ PumpB
      ├─ CurrentSpeed
      ├─ CurrentSpeed_Role
      ├─ CurrentSpeed_SourceKind
      └─ ...
```

For `source.kind = plcTag`, the main variable is created with the correct data type but no runtime constant value is written.

For `source.kind = mock` or `source.kind = static`, the helper can write the supplied value because the JSON makes the value's purpose explicit.

## Why helper metadata first

This is safer than trying DynamicLinks immediately because it lets us test these things independently:

1. JSON schema shape is correct.
2. DesignTime NetLogic can read the JSON file.
3. FT Optix Model nodes can be created from that JSON.
4. PLC tag intent is preserved without pretending live PLC values exist.

## Next probe

Probe 05C should discover the smallest reliable DesignTime NetLogic pattern for creating actual FT Optix `DynamicLink` nodes.

Probe 05D can then use FT Echo or another dummy PLC source to prove that live tag values update correctly.
