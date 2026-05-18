# Import Rules

Observed in FT Optix Studio v1.7.2.51:

## Current import target

XML import is currently tested under:

```text
Model folder
```

## Working XML basics

The successful probes use OPC UA `UANodeSet` XML with:

- `NamespaceUris`
- `Models`
- `Aliases`
- `UAObject` for folders
- `UAVariable` for variables
- `NodeId`
- `ParentNodeId`
- `BrowseName`
- `DisplayName`
- `References`
- `Value`

## Reference types used in working probes

```text
i=40  HasTypeDefinition
i=47  HasComponent
```

For child nodes, the reverse parent reference uses:

```xml
<Reference ReferenceType="i=47" IsForward="false">PARENT_NODE_ID</Reference>
```

## Type definitions used in working probes

```text
i=61  FolderType
i=63  BaseDataVariableType
```

## Data types tested

```text
i=1    Boolean
i=6    Int32
i=7    UInt32
i=10   Float
i=12   String
i=294  DateTime
```

## Practical rule

Start with Model-only nodes. After Model probes are stable, attempt UI/page XML separately.
