# LLM Build Guide - FactoryTalk Optix HMI Generation

## Purpose

This guide tells an LLM how to use this repo to build toward a runnable FactoryTalk Optix HMI without guessing unsupported FT Optix APIs.

## Required reading order

Before making changes, read:

```text
1. PROJECT_VISION.md
2. README.md
3. NEXT_STEPS.md
4. Latest milestone README under 10_milestones/
5. Relevant probe README under 02_probes/
6. Relevant DesignTime NetLogic C# file
```

## Current project state

```text
Milestone 06 = PASS
Probe 05C-4 = PASS
Current priority = Milestone 07 / Probe 05D
```

Milestone 06 proved:

```text
Hardened JSON
→ DesignTime NetLogic
→ Model nodes
→ _LocalSources
→ local DynamicLinks
→ runtime value update
```

Probe 05C-4 proved the exact explicit DynamicLink mode syntax:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.ReadWrite);
```

Milestone 07 must prove:

```text
source.kind = plcTag
→ FT Echo or dummy PLC live tag
→ FT Optix runtime value update
```

## Core build pipeline

The final target pipeline is:

```text
User natural language
→ LLM produces HMI intent model
→ LLM produces hardened JSON spec
→ schema and semantic validation
→ DesignTime NetLogic creates FT Optix Model
→ DynamicLinks connect variables to sources
→ UI screens bind to Model variables
→ commands write to controller tags
→ alarms, trends, recipes, logging added after their probes pass
```

## JSON source rules

Do not use ambiguous raw `value` for variables that may represent PLC data.

Use explicit source kinds:

```text
source.kind = plcTag
source.kind = mock
source.kind = static
```

Expected meaning:

```text
plcTag = live or future live controller tag intent
mock   = simulated value for tests and demos
static = configuration or non-runtime-changing value
```

## DynamicLink rule

Known working basic API:

```csharp
generatedVariable.SetDynamicLink(sourceVariable);
```

Known working explicit mode API from Probe 05C-4:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.Read);
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.Write);
generatedVariable.SetDynamicLink(sourceVariable, FTOptix.CoreBase.DynamicLinkMode.ReadWrite);
```

Use the fully qualified enum name unless the namespace import has been verified:

```text
FTOptix.CoreBase.DynamicLinkMode
```

Known earlier failing shorthand:

```csharp
generatedVariable.SetDynamicLink(sourceVariable, DynamicLinkMode.ReadWrite);
```

That failed because `DynamicLinkMode` was not available unqualified in the tested C# context.

Suggested JSON `source.mode` mapping for future generator work:

```text
read      -> FTOptix.CoreBase.DynamicLinkMode.Read
write     -> FTOptix.CoreBase.DynamicLinkMode.Write
readWrite -> FTOptix.CoreBase.DynamicLinkMode.ReadWrite
```

Do not invent alternate overloads unless a new probe is created and tested.

## When to use FT Echo

FT Echo is not required for local Model-to-Model DynamicLink tests.

FT Echo or a dummy/live controller is required when testing:

```text
source.kind = plcTag
→ live communication/tag path
```

That begins at Milestone 07 / Probe 05D.

## How to add a new capability

Every new FT Optix capability must follow this path:

```text
1. Create a small probe folder.
2. Define objective.
3. Define files used.
4. Define manual FT Optix steps.
5. Define expected result.
6. Run the test.
7. Record actual result.
8. Mark PASS or FAIL.
9. Record working API/pattern.
10. Record failing API/pattern if relevant.
11. Only then promote the pattern into a milestone or generator.
```

## Do not jump scope

Do not generate these until their lower-level proofs pass:

```text
- full dashboard
- alarm system
- trend system
- datalogger
- recipe system
- complete one-prompt HMI
```

## Current next action for an LLM

The next correct action is not to edit the Milestone 06 generator silently.

The next correct action is to prepare and run:

```text
02_probes/probe_05d_ft_echo_live_tag_verification/
```

Start with manual live tag read before generator automation.

## Probe 05D test order

```text
1. Manual read tag: PumpA.CurrentSpeed
2. Manual readWrite tag: PumpA.SetSpeed
3. Manual Boolean read tag: PumpA.Running
4. Manual write command tag: PumpA.OperatorCommand
5. Generator-created live DynamicLink after manual tests pass
```

## Minimum dummy tag list

Use sanitized tags:

```text
PumpA.CurrentSpeed      Float / REAL
PumpA.SetSpeed          Float / REAL
PumpA.Running           Boolean / BOOL
PumpA.OperatorCommand   String
```

Suggested test values:

```text
CurrentSpeed = 12.3 then 45.6
SetSpeed = 50.0 then value written from FT Optix
Running = false then true
OperatorCommand = START
```

## Response style for future LLMs

When continuing this repo, explain:

```text
Objective
Step
Expected result
Whether FT Echo is needed now
What file will be updated
What test must be run manually
```

Do not work silently.
