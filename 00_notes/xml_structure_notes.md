# XML Structure Notes

FT Optix XML export/import uses OPC UA NodeSet style XML.

## Minimal working model pattern

A generated folder can be represented as a `UAObject`:

```xml
<UAObject NodeId="ns=1;g=..." BrowseName="1:AI_ModelProbe_01" WriteMask="69876">
  <DisplayName>AI_ModelProbe_01</DisplayName>
  <References>
    <Reference ReferenceType="i=40">i=61</Reference>
  </References>
</UAObject>
```

A generated variable can be represented as a `UAVariable`:

```xml
<UAVariable NodeId="ns=1;g=..." ParentNodeId="ns=1;g=..." BrowseName="1:StatusText" DataType="i=12" AccessLevel="3" UserAccessLevel="3">
  <DisplayName>StatusText</DisplayName>
  <References>
    <Reference ReferenceType="i=40">i=63</Reference>
    <Reference ReferenceType="i=47" IsForward="false">PARENT_NODE_ID</Reference>
  </References>
  <Value>
    <uax:String>Hello from generated XML</uax:String>
  </Value>
</UAVariable>
```

## Deterministic NodeIds

The scripts use deterministic UUIDv5 values so that regeneration produces stable `NodeId` values.

This helps compare generated XML with FT Optix exported-back XML.

## Current limitation

These notes are based on Model-folder tests only. UI objects, panels, pages, event handlers, dynamic links, and image assets need separate probes.
