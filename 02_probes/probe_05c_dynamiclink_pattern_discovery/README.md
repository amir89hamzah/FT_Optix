# Probe 05C - DynamicLink pattern discovery result

## Status

```text
05C-1 LOCAL MANUAL DYNAMICLINK TESTED / PASS
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

## Result

Probe 05C-1 is accepted as passed.

It proves:

1. A local FT Optix DynamicLink can connect one Model variable to another Model variable.
2. The relative path `../SourceSpeed` works for sibling variables under the same parent.
3. Runtime/emulator evaluation updates the linked value.
4. FT Echo is not needed for this local DynamicLink pattern discovery step.

## Mode note

An explicit `Read` mode selector was not found during the manual UI test.

For 05C-1, this is acceptable because the link is functional. The mode/default mode should be inspected later through export or project file review.

## Next checkpoint

Recommended next step:

```text
Probe 05C-2 - inspect/export the local DynamicLink pattern
```

Goal:

```text
Learn how FT Optix stores the DynamicLink path, mode/default mode, and relative path after save/export.
```

Only after that should a DesignTime NetLogic generator attempt to create the same local DynamicLink automatically.
