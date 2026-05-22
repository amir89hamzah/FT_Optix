# Probe 05C-3 - DesignTime NetLogic DynamicLink generator

## Purpose

Probe 05C-1 proved that a manual local DynamicLink works in FT Optix.

Probe 05C-2 exported that local DynamicLink and found this storage pattern:

```text
LinkedSpeed
└─ DynamicLink = ../SourceSpeed
   └─ Mode = 2
```

Probe 05C-3 tries to create the same local DynamicLink automatically with DesignTime NetLogic.

## Important scope limit

FT Echo is not required for this probe.

PLC live tag verification is not part of this probe.

This probe only checks whether DesignTime NetLogic can create a local Model-to-Model DynamicLink.

## C# file

```text
DesignTimeDynamicLinkGenerator.cs
```

Paste the contents into a FT Optix DesignTime NetLogic class.

If the class name in your FT Optix project is different, change the class declaration to match the file/class that FT Optix created.

For example:

```csharp
public class DesignTimeNetLogic2 : BaseNetLogic
```

## Method to run

```text
GenerateLocalDynamicLinkProbe()
```

## Expected generated structure

```text
Model
└─ AI_DynamicLinkNetLogicProbe_01
   ├─ SourceSpeed
   └─ LinkedSpeed
      └─ DynamicLink
         └─ Mode
```

Expected values:

```text
SourceSpeed       = 12.3
LinkedSpeed       = 0 before runtime evaluation
DynamicLink       = ../SourceSpeed
Mode              = 2
```

## Manual runtime test

After running the method:

1. Link a UI Label text/value to `Model/AI_DynamicLinkNetLogicProbe_01/LinkedSpeed`, or inspect the value in runtime/emulator.
2. Run the emulator.
3. Confirm the displayed linked value follows `SourceSpeed`.
4. Change `SourceSpeed` to `45.6`.
5. Confirm the linked value changes at runtime/emulator.

## Pass criteria

Probe 05C-3 passes if:

```text
1. The method compiles.
2. The method runs without error.
3. AI_DynamicLinkNetLogicProbe_01 appears under Model.
4. LinkedSpeed has a DynamicLink child.
5. DynamicLink value is ../SourceSpeed.
6. DynamicLink has Mode child with value 2.
7. Runtime/emulator shows linked behavior.
```

## If it does not work

If the method creates the nodes but the link does not behave dynamically, export the generated node and compare it against:

```text
02_probes/probe_05c_dynamiclink_pattern_discovery/exported_AI_DynamicLinkProbe_01.xml
```

The likely difference to inspect first is the reference type between `LinkedSpeed` and `DynamicLink`.
