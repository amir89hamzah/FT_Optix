# Probe 05C-4 - DynamicLink Mode Syntax Discovery

## Status

```text
TESTED / PASS
```

## Purpose

Find the exact FactoryTalk Optix C# syntax for creating DynamicLinks with an explicit mode from DesignTime NetLogic.

Earlier probes proved that this compiles and works:

```csharp
generatedVariable.SetDynamicLink(sourceVariable);
```

But this shorter syntax did not compile in the tested project context:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, DynamicLinkMode.ReadWrite);
```

Probe 05C-4 identifies the correct mode enum namespace.

## Test context

Project:

```text
Test
```

NetLogic file:

```text
DesignTimeNetLogic2.cs
```

Method:

```text
GenerateJsonModelWithLocalDynamicLinks()
```

Tested line in Milestone 06 generator branch:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.ReadWrite);
```

## IntelliSense discovery

Visual Studio Code / FT Optix workspace IntelliSense showed the overload:

```csharp
IUAVariable.SetDynamicLink(
    IUAVariable source,
    FTOptix.CoreBase.DynamicLinkMode mode = FTOptix.CoreBase.DynamicLinkMode.Read
)
```

Available enum values shown:

```text
FTOptix.CoreBase.DynamicLinkMode.Read
FTOptix.CoreBase.DynamicLinkMode.Write
FTOptix.CoreBase.DynamicLinkMode.ReadWrite
```

## Compile result

```text
PASS
```

Observed IDE status:

```text
No problems have been detected in the workspace.
```

## Runtime result

Runtime/emulator value following was verified after changing the generated local source value.

Observed:

```text
Initial display: 12.3
After local source change: 45.6
```

The generated variable continued to follow the local source after switching to the explicit `ReadWrite` mode syntax.

## Export result

The generated object was exported as:

```text
02_probes/probe_05c4_dynamiclink_mode_syntax/exported_AI_JsonDynamicLinkProbe_01_readwrite.xml
```

DynamicLink nodes were observed for the generated PumpA plcTag-intent variables:

```text
PumpA/CurrentSpeed    -> ../../_LocalSources/PumpA/CurrentSpeed
PumpA/SetSpeed        -> ../../_LocalSources/PumpA/SetSpeed
PumpA/Running         -> ../../_LocalSources/PumpA/Running
PumpA/OperatorCommand -> ../../_LocalSources/PumpA/OperatorCommand
```

Each exported DynamicLink had a child mode variable:

```xml
<UAVariable BrowseName="2:Mode" ...>
  <Value>
    <uax:Int32>2</uax:Int32>
  </Value>
</UAVariable>
```

Observed mode value:

```text
Mode = 2
```

For this probe, `FTOptix.CoreBase.DynamicLinkMode.ReadWrite` exported as `Mode = 2`.

## Finding

Use the fully qualified enum name in generated NetLogic code:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.ReadWrite);
```

Do not use the unqualified enum unless the correct namespace is imported and verified:

```csharp
DynamicLinkMode.ReadWrite
```

## Impact

This resolves the earlier ambiguity from Probe 05C-3.

The generator can now map JSON `source.mode` to explicit FactoryTalk Optix DynamicLink modes:

```text
source.mode = read      -> FTOptix.CoreBase.DynamicLinkMode.Read
source.mode = write     -> FTOptix.CoreBase.DynamicLinkMode.Write
source.mode = readWrite -> FTOptix.CoreBase.DynamicLinkMode.ReadWrite
```

This should be used in a future generator update, but the current Milestone 06 PASS state should not be changed silently.

## Next checkpoint

Proceed to Milestone 07 / Probe 05D only after this result is recorded:

```text
FT Echo or dummy PLC live tag verification
```

Probe 05D should start with manual live tag binding before generator automation.
