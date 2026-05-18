# Probe 01 - Basic Model Folder

## Objective

Test whether generated OPC UA `UANodeSet` XML can be imported into FT Optix under the `Model` folder.

## Expected structure

```text
AI_ModelProbe_01
├─ StatusText
├─ TestNumber
├─ TestBool
└─ Notes
```

## Result

Success in FT Optix Studio v1.7.2.51.

## Important observation

FT Optix accepted:

- one generated folder
- String variable
- Float variable
- Boolean variable
- generated UUID-based `NodeId`
- `ParentNodeId`
- reverse `HasComponent` reference
