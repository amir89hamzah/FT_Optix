# Probe 05C - DynamicLink pattern discovery result

## Status

```text
05C-1 LOCAL MANUAL DYNAMICLINK TESTED / PASS
05C-2 EXPORTED DYNAMICLINK STORAGE PATTERN / PASS
05C-3 DESIGNTIME NETLOGIC DYNAMICLINK GENERATOR / PASS
```

Date recorded:

```text
2026-05-22
```

## Scope

This test used local FT Optix Model-to-Model DynamicLinks.

FT Echo was not required.

PLC live tag verification was not part of this test.

## 05C-1 manual local DynamicLink

Manual test structure:

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

Observed behavior:

```text
Initial runtime/emulator display: 12.3
After SourceSpeed changed to 45.6: 45.599998
```

The value `45.599998` is acceptable Float display behavior and should be handled later by display precision / formatting.

## 05C-2 exported XML storage pattern

The exported NodeSet was saved as:

```text
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
```

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

## 05C-3 DesignTime NetLogic DynamicLink generator

Template used:

```text
09_netlogic_probes/probe_05c_dynamiclink_netlogic_generator/DesignTimeDynamicLinkGenerator.cs
```

The first template attempt used:

```csharp
linkedSpeed.SetDynamicLink(sourceSpeed, DynamicLinkMode.ReadWrite);
```

That did not compile because `DynamicLinkMode` was not available in the current FT Optix C# context.

The working line was:

```csharp
linkedSpeed.SetDynamicLink(sourceSpeed);
```

Observed generated object:

```text
Model
└─ AI_DynamicLinkNetLogicProbe_01
```

When the root object was selected, FT Optix displayed child variables as properties rather than as an expanded tree in the screenshot:

```text
SourceSpeed  Float   12.3 / later 45.599998
LinkedSpeed  Float   ../SourceSpeed
Probe        String  Probe 05C-3 - DesignTime NetLogic DynamicLink generator
ExpectedLink String  ../SourceSpeed
ExpectedMode String  2
Note         String  If runtime follows SourceSpeed, DesignTime NetLogic DynamicLink creation works.
```

Runtime/emulator behavior:

```text
Initial linked display: 12.3
After SourceSpeed changed to 45.6: 45.599998
```

This confirms that DesignTime NetLogic can create a functional local DynamicLink automatically.

## Result

Probe 05C-1, 05C-2, and 05C-3 are accepted as passed.

They prove:

1. A local FT Optix DynamicLink can connect one Model variable to another Model variable.
2. The relative path `../SourceSpeed` works for sibling variables under the same parent.
3. Runtime/emulator evaluation updates the linked value.
4. FT Echo is not needed for local DynamicLink pattern discovery.
5. FT Optix stores DynamicLink as a child variable named `DynamicLink`.
6. FT Optix stores the link path as the `DynamicLink` variable value.
7. FT Optix stores mode as a child variable named `Mode` with integer value `2` in the manual export.
8. DesignTime NetLogic can create the local DynamicLink automatically using `SetDynamicLink(sourceSpeed)`.

## Next checkpoint

Recommended next step:

```text
Probe 05C-4 - update the hardened JSON generator so plcTag variables can create DynamicLink intent safely when a local source is available, or prepare Probe 05D for FT Echo / dummy PLC live tag verification.
```

Do not jump into full dashboard, Recipe, Datalogger, or Alarm generation yet.

FT Echo becomes relevant when the project goal changes from local Model-to-Model links to real PLC-backed tag verification.
