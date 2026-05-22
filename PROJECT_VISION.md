# Project Vision - FT_Optix

## One-line vision

FT_Optix is a public, reproducible knowledge base that teaches humans and LLMs how to build runnable FactoryTalk Optix HMIs from natural-language intent and hardened JSON specs.

## What success looks like

A user should be able to say:

```text
Build an HMI for a pump station with two pumps.
Each pump has current speed, set speed, running status, start/stop command, high-speed alarm, and trend.
Use PLC tags for PumpA and mock data for PumpB.
```

Then an LLM should be able to:

```text
1. Read this repo.
2. Understand what is already proven in FT Optix.
3. Produce a valid JSON spec.
4. Validate the JSON spec.
5. Generate FT Optix Model nodes.
6. Generate DynamicLinks using tested API patterns.
7. Connect live PLC/dummy tags after Milestone 07 is proven.
8. Generate UI screens and controls.
9. Add alarms and trends after those patterns are proven.
10. Provide exact manual test steps and expected results.
```

## Why the repo must be proof-driven

LLMs can easily invent code that looks plausible but does not compile in FactoryTalk Optix.

This repo avoids that by recording:

```text
- What was tested.
- What compiled.
- What did not compile.
- What runtime result was observed.
- What export pattern FT Optix produced.
- What the next safe checkpoint is.
```

Every new capability should be added as a small probe before it becomes part of the full HMI generator.

## Current proven foundation

The repo currently proves this path:

```text
Hardened JSON
→ DesignTime NetLogic
→ FT Optix Model variables
→ source metadata preservation
→ local DynamicLinks
→ runtime/emulator value update
```

Milestone 06 is tested and passed.

## Current gap

The next missing proof is live tag binding:

```text
source.kind = plcTag
→ FT Echo / dummy PLC tag
→ FT Optix runtime value update
```

This is Milestone 07 / Probe 05D.

## Non-goals right now

Do not jump directly into these before live tag proof:

```text
- Full dashboard generation
- Alarm generation
- Datalogger generation
- Recipe generation
- Trend generation
- Complete one-prompt HMI generation
```

Those are real goals, but they depend on the lower-level Model, DynamicLink, and live tag paths being reliable.

## Development philosophy

```text
Small proven probe
→ record result
→ promote to milestone
→ use in generator
→ only then build higher-level HMI features
```

## Public safety and privacy

This repo must stay safe for public use.

Do not commit:

```text
- real PLC IP addresses
- customer project files
- production HMI backups
- plant screenshots
- usernames or email addresses
- credentials
- license keys
- proprietary tag lists
```

Use sanitized names such as:

```text
PumpA.CurrentSpeed
PumpA.SetSpeed
PumpA.Running
PumpA.OperatorCommand
```
