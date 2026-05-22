# Probe 05C - DynamicLink pattern discovery result

## Status

```text
05C-1 LOCAL MANUAL DYNAMICLINK TESTED / PASS
05C-2 EXPORTED DYNAMICLINK STORAGE PATTERN / PASS
```

Date recorded:

```text
2026-05-22
```

## Scope

This test used a local FT Optix Model-to-Model DynamicLink.

FT Echo was not required.

PLC live tag verification was not part of this test.

## Manual test structure

```text
Model
└─ AI_DynamicLinkProbe_01
   ├─ SourceSpeed
   └─ LinkedSpeed
```

`LinkedSpeed` was linked to:

```text
../SourceSpeed
```

## Observed behavior

Initial runtime/emulator display:

```text
12.3
```

After changing `SourceSpeed` to `45.6`, the linked runtime/emulator display changed to:

```text
45.599998
```

The value `45.599998` is acceptable Float display behavior and should be handled later by display precision / formatting.

## Exported XML

The exported NodeSet was saved as:

```text
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
```

## Export pattern found

FT Optix exported the DynamicLink as a child `UAVariable` under the linked variable.

DynamicLink node:

```xml
<UAVariable BrowseName="1:DynamicLink" DataType="ns=3;i=6">
  <Value>
    <uax:String>../SourceSpeed</uax:String>
  </Value>
</UAVariable>
```

The DynamicLink's parent is `LinkedSpeed` through `ParentNodeId` and a reverse reference:

```xml
<Reference ReferenceType="ns=2;i=19" IsForward="false">...</Reference>
```

Mode node:

```xml
<UAVariable BrowseName="2:Mode" DataType="ns=2;i=14">
  <Value>
    <uax:Int32>2</uax:Int32>
  </Value>
</UAVariable>
```

The `Mode` variable is a child of `DynamicLink`.

Observed meaning for this probe:

```text
DynamicLink value = ../SourceSpeed
Mode value        = 2
```

The UI did not show a clear `Read` selector during manual setup, but export confirms that FT Optix stores mode explicitly as a child named `Mode`.

## Result

Probe 05C-1 and 05C-2 are accepted as passed.

They prove:

1. A local FT Optix DynamicLink can connect one Model variable to another Model variable.
2. The relative path `../SourceSpeed` works for sibling variables under the same parent.
3. Runtime/emulator evaluation updates the linked value.
4. FT Echo is not needed for this local DynamicLink pattern discovery step.
5. FT Optix stores DynamicLink as a child variable named `DynamicLink`.
6. FT Optix stores the link path as the `DynamicLink` variable value.
7. FT Optix stores mode as a child variable named `Mode` with integer value `2` in this export.

## Next checkpoint

Recommended next step:

```text
Probe 05C-3 - DesignTime NetLogic creates the same local DynamicLink automatically
```

Goal:

```text
Use DesignTime NetLogic to create:
Model/AI_DynamicLinkNetLogicProbe_01/SourceSpeed
Model/AI_DynamicLinkNetLogicProbe_01/LinkedSpeed
Model/AI_DynamicLinkNetLogicProbe_01/LinkedSpeed/DynamicLink = ../SourceSpeed
Model/AI_DynamicLinkNetLogicProbe_01/LinkedSpeed/DynamicLink/Mode = 2
```

Only after this works should the project move to PLC-backed DynamicLinks and FT Echo verification.
